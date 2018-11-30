using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading;
using Gml.Converters;
using Gml.Helpers;
using Gml.Helpers.Extensions;
using Gml.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MailMessage = Gml.Components.MailMessage;

namespace Gml
{
    public class GmailClient : IMailClient, IEquatable<GmailClient>
    {
        private const string UserId = "me";
        private readonly string _applicationName;

        private readonly GmailService _gmailService;
        public readonly MailAddress Mail;

        public GmailClient(GmailConfiguration config)
        {
            Mail = new MailAddress($"{config.User}@gmail.com");
            _gmailService = Authorize(config);
        }

        /// Gets or sets Application name to be used in the User-Agent header. Default value is
        /// <c>null</c>
        /// .
        public GmailClient(GmailConfiguration config, string applicationName) : this(config)
        {
            _applicationName = applicationName;
        }

        public bool Equals(GmailClient other)
        {
            return other != null &&
                   Mail.Address.Equals(other.Mail.Address, StringComparison.OrdinalIgnoreCase) &&
                   Mail.DisplayName.Equals(other.Mail.DisplayName, StringComparison.OrdinalIgnoreCase);
        }

        public void Delete(IMailMessage message)
        {
            if (!(message is MailMessage mailMessage))
                throw new InvalidCastException(
                    $"Unable to delete message of type '{message.GetType().FullName}'. Expected type '{message.GetType().FullName}'");
            Delete(mailMessage.Id);
        }

        public void Delete(params ISearchMailFilter[] conditions)
        {
            var ids = GetMessageIds(conditions);
            if (ids.Any())
                _gmailService.Users.Messages
                    .BatchDelete(new BatchDeleteMessagesRequest {Ids = ids}, UserId)
                    .Execute();
        }

        public IList<IMailMessage> GetMessages(params ISearchMailFilter[] conditions)
        {
            var ids = GetMessageIds(conditions);
            return ids
                .Select(id => new MailMessage(this, id)).Cast<IMailMessage>()
                .ToList();
        }

        public void Send(IMailMessageModel messageModel)
        {
            var message = BuildMessage(messageModel);
            _gmailService.Users.Messages.Send(message, UserId).Execute();
        }

        public Message GetMessage(string id)
        {
            return _gmailService.Users.Messages.Get(UserId, id).Execute();
        }

        private void Send(Message message)
        {
            _gmailService.Users.Messages.Send(message, UserId).Execute();
        }

        public void SendInPlainTextMode(IMailMessageModel messageModel)
        {
            var message = BuildPlainTextMessage(messageModel);
            Send(message);
        }

        public byte[] GetAttachmentBytes(string messageId, string attachmentId)
        {
            var messagePartBody = _gmailService.Users
                .Messages
                .Attachments
                .Get(UserId, messageId, attachmentId)
                .Execute();
            return Base64StringHelper.DecodeBytes(messagePartBody.Data);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GmailClient);
        }

        public override int GetHashCode()
        {
            return Mail.Address.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Mail.Address}";
        }

        #region Private Methods

        private void Delete(string id)
        {
            _gmailService.Users.Messages.Delete(UserId, id).Execute();
        }

        private GmailService Authorize(GmailConfiguration config)
        {
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var credPath = Path.Combine(Path.GetDirectoryName(assemblyPath), "Gmail");
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets {ClientId = config.ClientId, ClientSecret = config.ClientSecret},
                config.Scope,
                config.User,
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;

            return new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName
            });
        }

        private IList<string> GetMessageIds(params ISearchMailFilter[] conditions)
        {
            var request = _gmailService.Users.Messages.List(UserId);
            request.Q = BuildFilterQuery(conditions);

            var messages = request.Execute().Messages;
            return messages == null ? new List<string>() : messages.Select(x => x.Id).ToList();
        }

        private static string BuildFilterQuery(ISearchMailFilter[] conditions)
        {
            return conditions.IsEmpty()
                ? string.Empty
                : string.Join(" ", conditions.Select(x => x.GetCondition()));
        }

        private Message BuildMessage(IMailMessageModel messageModel)
        {
            return messageModel.ConvertToMessage(Mail);
        }

        private Message BuildPlainTextMessage(IMailMessageModel messageModel)
        {
            return messageModel.ConvertToPlainTextMessage(Mail);
        }

        #endregion
    }
}