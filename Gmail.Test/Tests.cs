using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using FluentAssertions;
using Gmail.Test.DocumentTemplates;
using Gml;
using Gml.Components;
using Gml.Filters;
using Gml.Interfaces;
using Xunit;
using MailMessage = Gml.Components.MailMessage;

namespace Gmail.Test
{
    public class Tests : IClassFixture<TestFixture>
    {
        public Tests(TestFixture fixture)
        {
            _client = fixture.Client;
            FileProvider = fixture.FileProvider;
            _wrapper = GMailService.ForClient(_client);
        }

        private readonly GmailClientWrapper _wrapper;
        private readonly GmailClient _client;
        public IDocumentTemplatesProvider FileProvider;

        private static readonly TimeSpan WaitForMessageSentTimeout = TimeSpan.FromSeconds(2);

        private static readonly string[] MailAddress =
            {"xxxx@gmail.com", "xxx@gmail.com", "xxxxx@gmail.com"};

        [InlineData(TestFiles.Txt)]
        [InlineData(TestFiles.Pdf)]
        [InlineData(TestFiles.Doc)]
        [InlineData(TestFiles.Png)]
        [InlineData(TestFiles.Excel)]
        [InlineData(TestFiles.Jpeg)]
        public void Send_Message_With_Attachment(string fileFormat)
        {
            var message = GetTestMessage(fileFormat);
            _client.Send(message);
            Thread.Sleep(WaitForMessageSentTimeout);

            var messagesSent = _client.GetMessages(new SubjectFilter(message.Subject));
            messagesSent.Should().HaveCount(1);
            messagesSent.Select(msg => msg.Attachments.First().Data).Should()
                .AllBeEquivalentTo(message.Attachments.First().Data, "Attachment data did not match");
        }

        [InlineData(TestFiles.Txt)]
        [InlineData(TestFiles.Pdf)]
        [InlineData(TestFiles.Doc)]
        [InlineData(TestFiles.Png)]
        [InlineData(TestFiles.Excel)]
        public void Save_Attachment_To_File(string fileFormat)
        {
            var file = FileProvider.GetFiles().First(f => f.Name.Equals(fileFormat));
            var message = GetTestMessage(fileFormat);
            _client.Send(message);
            Thread.Sleep(WaitForMessageSentTimeout);

            var messagesReceived = _client.GetMessages(new SubjectFilter(message.Subject));

            foreach (var msg in messagesReceived)
            {
                var fileReceived = Path.Combine(Path.GetTempPath(),
                    $"{Guid.NewGuid()}.{Path.GetExtension(file.PhysicalPath)}");
                msg.Attachments.First().SaveTo(fileReceived);
                File.ReadAllBytes(file.PhysicalPath).Should().BeEquivalentTo(File.ReadAllBytes(fileReceived),
                    "Received and saved files are not equal");
            }
        }

        [InlineData("@gmail.com")]
        [InlineData(",@gmail.com")]
        [InlineData("test@g,mail.com")]
        [InlineData("test@gma il.com")]
        [InlineData("test@gmailcom")]
        public void Rise_Exception_For_Incorrect_Email_Address(string address)
        {
            MailMessageModel Message()
            {
                return new MailMessageModel(address);
            }

            Assert.Throws<ArgumentOutOfRangeException>(() => Message());
        }

        private IMailMessageModel GetTestMessage()
        {
            return MailMessageBuilder.To(MailAddress)
                .WithSubject($"PSC GmailClient Test {DateTime.Now}")
                .WithBody(RandomString(5)).Build();
        }

        private IMailMessageModel GetTestMessage(string filePath)
        {
            return MailMessageBuilder.To(MailAddress)
                .WithSubject($"PSC GmailClient Test {DateTime.Now}")
                .WithBody(RandomString(5)).WithAttachments(FileProvider.GetFileByName(filePath).PhysicalPath).Build();
        }

        private void VerifyMimiTypeForPlainText(IMailMessage messagesSent)
        {
            var msgId = ((MailMessage) messagesSent).Id;
            _client.GetMessage(msgId).Payload.MimeType.Should().Be(MediaTypeNames.Text.Plain);
        }

        private void VerifyMimiType(IMailMessage messagesSent)
        {
            var msgId = ((MailMessage) messagesSent).Id;
            _client.GetMessage(msgId).Payload.Parts.Should()
                .ContainSingle(x => x.MimeType == MediaTypeNames.Text.Plain).And
                .ContainSingle(x => x.MimeType == MediaTypeNames.Text.Html);
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 \n\t";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static bool IsDateInPastInterval(DateTime date, DateTime dateToCheck, TimeSpan interval)
        {
            var startInterval = dateToCheck.Add(-interval);
            var stopInterval = dateToCheck;

            return date >= startInterval && date <= stopInterval;
        }

        [Fact]
        public void Delete_Message()
        {
            var message = MailMessageBuilder.To(MailAddress)
                .WithSubject($"PSC GmailClient Test for Delete {DateTime.Now}").Build();
            _client.Send(message);
            Thread.Sleep(WaitForMessageSentTimeout);

            var messagesSent = _client.GetMessages(new SubjectFilter(message.Subject));
            _client.Delete(messagesSent.First());
            _client.GetMessages(new SubjectFilter(message.Subject)).Should().BeEmpty();
        }

        [Fact]
        public void GetMessages_Not_Return_Null()
        {
            var messagesReceived = _client.GetMessages(new SubjectFilter("GetMessages_Not_Return_Null"));
            messagesReceived.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void Send_Message()
        {
            var message = GetTestMessage();
            foreach (var m in MailAddress) message.Cc.Add(new MailAddress(m));

            _client.Send(message);
            Thread.Sleep(WaitForMessageSentTimeout);

            var messagesSent = _client.GetMessages(new SubjectFilter(message.Subject));
            VerifyMimiType(messagesSent[0]);
            messagesSent.Should().HaveCount(1).And
                .ContainSingle(x => x.To.First().Equals(message.To[0].Address), "To Address did not match").And
                .ContainSingle(x => x.Cc.First().Equals(message.Cc[0].Address), "CC Address did not match").And
                .ContainSingle(x => x.Subject.Equals(message.Subject), "Subject did not match").And
                .ContainSingle(x => x.Body.TrimEnd().Equals(message.Body.TrimEnd()), "Body did not match").And
                .ContainSingle(
                    x => IsDateInPastInterval(x.ReceivedTime, DateTime.Now.ToUniversalTime(), TimeSpan.FromSeconds(5)),
                    "Time did not match");
        }

        [Fact]
        public void Send_Message_To_Multiply_Addresses()
        {
            var message = GetTestMessage();
            foreach (var m in MailAddress) message.Cc.Add(new MailAddress(m));

            _client.Send(message);
            Thread.Sleep(WaitForMessageSentTimeout);
            var messagesSent = _client.GetMessages(new SubjectFilter(message.Subject));

            messagesSent.Should().HaveCount(1).And
                .ContainSingle(x => x.To.Count == MailAddress.Length).And
                .ContainSingle(x => x.To[0].Equals(message.To[0].Address), "To{1} Address did not match").And
                .ContainSingle(x => x.To[1].Equals(message.To[1].Address), "To{2} Address did not match").And
                .ContainSingle(x => x.To[2].Equals(message.To[2].Address), "To{3} Address did not match").And
                .ContainSingle(x => x.Cc.Count == MailAddress.Length).And
                .ContainSingle(x => x.Cc[0].Equals(message.Cc[0].Address), "CC{1} Address did not match").And
                .ContainSingle(x => x.Cc[1].Equals(message.Cc[1].Address), "CC{2} Address did not match").And
                .ContainSingle(x => x.Cc[2].Equals(message.Cc[2].Address), "CC{3} Address did not match").And
                .ContainSingle(x => x.Subject.Equals(message.Subject), "Subject did not match").And
                .ContainSingle(x => x.Body.TrimEnd().Equals(message.Body.TrimEnd()), "Body did not match");
        }

        [Fact]
        public void Send_Message_With_Plain_Text_Mode()
        {
            var message = GetTestMessage();
            _client.SendInPlainTextMode(message);
            Thread.Sleep(WaitForMessageSentTimeout);
            var messagesSent = _client.GetMessages(new SubjectFilter(message.Subject));

            VerifyMimiTypeForPlainText(messagesSent[0]);
            messagesSent.Should().HaveCount(1).And
                .ContainSingle(x => x.To.First().Equals(message.To[0].Address), "To Address did not match").And
                .ContainSingle(x => x.Subject.Equals(message.Subject), "Subject did not match").And
                .ContainSingle(x => x.Body.TrimEnd().Equals(message.Body.TrimEnd()),
                    "Body did not match").And
                .ContainSingle(
                    x => IsDateInPastInterval(x.ReceivedTime, DateTime.Now.ToUniversalTime(), TimeSpan.FromSeconds(5)),
                    "Time did not match");
        }

        [Fact]
        public void SendMessages_With_GmailClientWrapper()
        {
            const string subj = "Hello";
            _wrapper.Send(b => b.WithSubject(subj), MailAddress);
            var messagesReceived = _wrapper.GetMessages(By.Subject(subj));
            messagesReceived.Count.Should().BeGreaterThan(0);
        }
    }
}