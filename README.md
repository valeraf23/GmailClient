# GmailClient

**Read and send messages, manage attachments, search messages.**

[![NuGet.org](https://img.shields.io/nuget/v/Gml.svg?style=flat-square&label=NuGet.org)](https://www.nuget.org/packages/Gml/)
[![Build status](https://ci.appveyor.com/api/projects/status/3br33u4wb2xpc4kl/branch/master?svg=true)](https://ci.appveyor.com/project/valeraf23/gmailclient/branch/master)
## Installation

#### Install with NuGet Package Manager Console
```
Install-Package Gml
```
#### Install with .NET CLI
```
dotnet add package Gml
```
## Example:

```csharp
GmailConfiguration config = new GmailConfiguration();

Client = new GmailClient(config);

GmailClientWrapper wrapper = GMailService.ForClient(_client);

wrapper
.Send(b => b.WithSubject("Some subject")
.WithBody("Some Body")
.WithCc("address"), "some address");

IMailMessage messagesReceived = _wrapper.GetMessages(By.Subject("Some subject"))[0];
 
 messagesReceived.Subject // "Some subject"
 messagesReceived.Body // "Some Body"
  
  ```
  
