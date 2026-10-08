# HS DEVS AUTH - C# Client SDK (`HSAUTH.cs`)

Official zero-dependency C# client SDK for HS DEVS AUTH applications.

## Features
- **Zero External Dependencies**: Works out-of-the-box with .NET Framework 4.6+, .NET Core 3.1+, .NET 6, .NET 7, and .NET 8.
- **Multiple Namespaces**: Supports both `namespace HSAUTH` and `namespace HSAuth`.
- **Hardware ID (HWID) Protection**: Automatically grabs Windows MachineGuid, WindowsIdentity SID, or cross-platform fallback hashes to bind and lock licenses.
- **In-App Live Messaging & Remote Killswitch**: Native event callback `OnMessageReceived` triggers whenever you broadcast alerts or send an instant killswitch from your dashboard.
- **Remote Variables & Files**: Download protected strings and binary files straight from the cloud.
- **100% Free**: Up to 5 free apps and unlimited license key generations on [HS DEVS AUTH](https://hsauth.mujahid.xyz).

---

## Quick Start

### 1. Download `HSAUTH.cs`
Save [HSAUTH.cs](https://hsauth.mujahid.xyz/sdk/HSAUTH.cs) directly into your C# project.

### 2. Instantiate and Initialize
```csharp
using HSAUTH;

public static api HSAuthApp = new api(
    name: "YourAppName",
    ownerid: "your_app_identifier_from_dashboard",
    version: "1.0",
    apiUrl: "https://hsauth.mujahid.xyz/api/1.3/"
);

static void Main()
{
    // Initialize session
    HSAuthApp.init();

    // Optional: Real-time broadcast / killswitch listener
    HSAuthApp.OnMessageReceived += (msg) =>
    {
        Console.WriteLine($"[ALERT {msg.type}]: {msg.title} - {msg.content}");
        if (msg.type == "killswitch") Environment.Exit(0);
    };

    // Authenticate with License Key
    HSAuthApp.license("HSAUTH-XXXX-XXXX-XXXX-XXXX");

    if (HSAuthApp.response.success)
    {
        Console.WriteLine("Welcome, authenticated user!");
        HSAuthApp.start_heartbeat(30); // keep-alive every 30 seconds
    }
    else
    {
        Console.WriteLine("Failed: " + HSAuthApp.response.message);
    }
}
```

---

## Methods Reference
| Method | Description |
|---|---|
| `HSAuthApp.init()` | Establishes connection, verifies version, and starts session. |
| `HSAuthApp.login(user, pass)` | Authenticates with username and password. |
| `HSAuthApp.register(user, pass, key)` | Registers new account linked to a license key. |
| `HSAuthApp.license(key)` | Authenticates directly via license key. |
| `HSAuthApp.check()` | Verifies session validity and checks for active broadcast messages. |
| `HSAuthApp.var(varid)` | Retrieves the decrypted value of a remote variable. |
| `HSAuthApp.download(fileid)` | Downloads binary file data as `byte[]`. |
| `HSAuthApp.log(msg)` | Sends security or diagnostic log to developer dashboard. |
| `HSAuthApp.start_heartbeat(secs)` | Launches background keep-alive thread. |
| `HSAuthApp.logout()` | Terminates session on the server. |
