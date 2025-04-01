# MailMimic

Self-hosted solution for testing email functionality in development environments.

## Features

- [x] Seamless integration into existing applications.
- [x] Out of the box setup.
- [x] Advanced configuration options.
- [x] Web Portal for managing mailboxes.
- [ ] SMTP, POP3 and IMAP support.
- [x] TLS and SSL support.

## Hosting

- [x] Sidehost with an existing ASP Core application.
- [ ] Selfhost including Docker support.

## Getting Started

Register MailMimic in the `Startup.cs` file of your ASP Core application.

```csharp
builder.Services.AddMailMimic();
```

### Portal

To sidehost the MailMimic portal, add the following code to the `Startup.cs` file of your ASP Core application.

```csharp
builder.Services.AddMailMimicPortal();

var app = builder.Build();

app.MapMailMimicPortal();
```


## Configuration

MailMimic can be configured using the when registering MailMailMimc.

```csharp
builder.Services.AddMailMimic(options =>
{
	options.Port = 25;
	options.Host = "localhost";
	options.Username = "username";
	options.Password = "password";
	options.UseSsl = false;
	options.UseTls = false;
});
```

Alternatively, `appsettings.json` can be used to automatically bind the options.

```json
{
	"MailMimicConfig": {
		"Port": 465,
		"UseSsl": true,
		"SslThumbprint": "5E0FE7037455634B97E60711A215B5879DB0CB4F"
	}
}
```

## Contribution

TODO