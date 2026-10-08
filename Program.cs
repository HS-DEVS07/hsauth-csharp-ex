using System;
using System.Threading;
using HSAUTH;

namespace HSAuthCSharpExample
{
    class Program
    {
        // Replace with your credentials from https://hsauth.mujahid.xyz/dashboard
        public static api HSAuthApp = new api(
            name: "MyApp",
            ownerid: "your_app_identifier_here",
            version: "1.0",
            apiUrl: "https://hsauth.mujahid.xyz/api/1.3/"
        );

        static void Main(string[] args)
        {
            Console.Title = "HS DEVS AUTH - C# Client Example";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
  _    _  _____         _    _ _______ _    _ 
 | |  | |/ ____|  /\   | |  | |__   __| |  | |
 | |__| | (___   /  \  | |  | |  | |  | |__| |
 |  __  |\___ \ / /\ \ | |  | |  | |  |  __  |
 | |  | |____) / ____ \| |__| |  | |  | |  | |
 |_|  |_|_____/_/    \_\\____/   |_|  |_|  |_|
               C# .NET SDK Client
            ");
            Console.ResetColor();

            Console.WriteLine("[*] Connecting to HS DEVS AUTH server...");
            HSAuthApp.init();

            if (!HSAuthApp.response.success)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[!] Init failed: {HSAuthApp.response.message}");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[+] Connected! Application Version: {HSAuthApp.app_data.version}");
            Console.WriteLine($"[+] Active Users: {HSAuthApp.app_data.numUsers} | Online: {HSAuthApp.app_data.numOnlineUsers}");
            Console.ResetColor();

            // Setup In-App Live Messaging & Remote Killswitch Handler
            HSAuthApp.OnMessageReceived += (msg) =>
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"************************************************");
                Console.WriteLine($"[LIVE IN-APP BROADCAST - {msg.type.ToUpper()}]");
                Console.WriteLine($"Title: {msg.title}");
                Console.WriteLine($"Message: {msg.content}");
                Console.WriteLine($"************************************************");
                Console.ResetColor();

                if (msg.type == "killswitch")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[!] REMOTE KILLSWITCH ACTIVATED BY DEVELOPER. CLOSING APP.");
                    Console.ResetColor();
                    Thread.Sleep(3000);
                    Environment.Exit(0);
                }
            };

            bool authenticated = false;

            while (!authenticated)
            {
                Console.WriteLine("\n[1] Login (Username & Password)");
                Console.WriteLine("[2] Register (Username, Password & License Key)");
                Console.WriteLine("[3] License Key Only");
                Console.WriteLine("[4] Exit");
                Console.Write("\nChoose an option [1-4]: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter Username: ");
                        string u = Console.ReadLine();
                        Console.Write("Enter Password: ");
                        string p = Console.ReadLine();
                        Console.WriteLine("[*] Logging in...");
                        HSAuthApp.login(u, p);
                        break;

                    case "2":
                        Console.Write("Enter Desired Username: ");
                        string regUser = Console.ReadLine();
                        Console.Write("Enter Password: ");
                        string regPass = Console.ReadLine();
                        Console.Write("Enter License Key: ");
                        string regKey = Console.ReadLine();
                        Console.WriteLine("[*] Registering...");
                        HSAuthApp.register(regUser, regPass, regKey);
                        break;

                    case "3":
                        Console.Write("Enter License Key: ");
                        string licKey = Console.ReadLine();
                        Console.WriteLine("[*] Validating License...");
                        HSAuthApp.license(licKey);
                        break;

                    case "4":
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        continue;
                }

                if (HSAuthApp.response.success)
                {
                    authenticated = true;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n[+] Success: {HSAuthApp.response.message}");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[!] Error: {HSAuthApp.response.message}");
                    Console.ResetColor();
                }
            }

            // Successfully Authenticated!
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n================= USER PROFILE =================");
            Console.WriteLine($"Username:    {HSAuthApp.user_data.username}");
            Console.WriteLine($"IP Address:  {HSAuthApp.user_data.ip}");
            Console.WriteLine($"Hardware ID: {HSAuthApp.user_data.hwid}");
            Console.WriteLine($"Last Login:  {HSAuthApp.user_data.lastlogin}");
            Console.WriteLine("================================================");
            Console.ResetColor();

            // Start 30s background heartbeat (keeps session alive & receives live broadcasts)
            HSAuthApp.start_heartbeat(30);

            // Log successful session to developer dashboard
            HSAuthApp.log($"User {HSAuthApp.user_data.username} successfully loaded main program.");

            // Feature Demo Menu
            while (true)
            {
                Console.WriteLine("\n--- Application Dashboard Features ---");
                Console.WriteLine("[1] Read Remote Variable");
                Console.WriteLine("[2] Download Protected File");
                Console.WriteLine("[3] View Online Users");
                Console.WriteLine("[4] Send Diagnostic Log");
                Console.WriteLine("[5] Logout & Exit");
                Console.Write("\nSelect action [1-5]: ");

                string act = Console.ReadLine();
                switch (act)
                {
                    case "1":
                        Console.Write("Enter Variable Name/ID: ");
                        string varName = Console.ReadLine();
                        string val = HSAuthApp.var(varName);
                        Console.WriteLine($"Remote Variable Value: {val ?? "Not Found"}");
                        break;

                    case "2":
                        Console.Write("Enter File ID or Name: ");
                        string fid = Console.ReadLine();
                        byte[] fileData = HSAuthApp.download(fid);
                        if (fileData != null)
                        {
                            Console.WriteLine($"[+] Successfully downloaded {fileData.Length} bytes!");
                        }
                        else
                        {
                            Console.WriteLine("[!] File download failed.");
                        }
                        break;

                    case "3":
                        var onlineUsers = HSAuthApp.fetchOnline();
                        Console.WriteLine($"[+] Online Clients count: {onlineUsers.Count}");
                        break;

                    case "4":
                        Console.Write("Enter message to log: ");
                        string m = Console.ReadLine();
                        HSAuthApp.log(m);
                        Console.WriteLine("[+] Logged to HS DEVS AUTH dashboard!");
                        break;

                    case "5":
                        HSAuthApp.logout();
                        Console.WriteLine("Logged out safely. Goodbye!");
                        return;

                    default:
                        Console.WriteLine("Invalid selection.");
                        break;
                }
            }
        }
    }
}
