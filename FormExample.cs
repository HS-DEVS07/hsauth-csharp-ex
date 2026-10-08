using System;
using System.Drawing;
using System.Windows.Forms;
using HSAUTH;

namespace HSAuthWinFormsExample
{
    public class LoginForm : Form
    {
        public static api HSAuthApp = new api(
            name: "MyApp",
            ownerid: "your_app_identifier_here",
            version: "1.0",
            apiUrl: "https://hsauth.mujahid.xyz/api/1.3/"
        );

        private TextBox txtUser;
        private TextBox txtPass;
        private TextBox txtKey;
        private Label lblStatus;
        private Button btnLogin;
        private Button btnRegister;
        private Button btnLicense;

        public LoginForm()
        {
            this.Text = "HS DEVS AUTH - Secure Login";
            this.Size = new Size(380, 440);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(15, 23, 42);
            this.ForeColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label title = new Label
            {
                Text = "HS DEVS AUTH",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(99, 102, 241),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(title);

            Label lblU = new Label { Text = "Username", Location = new Point(20, 70), AutoSize = true };
            txtUser = new TextBox { Location = new Point(20, 95), Width = 320, BackColor = Color.FromArgb(30, 41, 59), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            Label lblP = new Label { Text = "Password", Location = new Point(20, 130), AutoSize = true };
            txtPass = new TextBox { Location = new Point(20, 155), Width = 320, UseSystemPasswordChar = true, BackColor = Color.FromArgb(30, 41, 59), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            Label lblK = new Label { Text = "License Key (For Register or Key-Only)", Location = new Point(20, 190), AutoSize = true };
            txtKey = new TextBox { Location = new Point(20, 215), Width = 320, BackColor = Color.FromArgb(30, 41, 59), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            btnLogin = new Button
            {
                Text = "Login",
                Location = new Point(20, 260),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(99, 102, 241),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White
            };
            btnLogin.Click += BtnLogin_Click;

            btnRegister = new Button
            {
                Text = "Register",
                Location = new Point(130, 260),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(51, 65, 85),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White
            };
            btnRegister.Click += BtnRegister_Click;

            btnLicense = new Button
            {
                Text = "License Only",
                Location = new Point(240, 260),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(51, 65, 85),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White
            };
            btnLicense.Click += BtnLicense_Click;

            lblStatus = new Label
            {
                Text = "Initializing HS DEVS AUTH...",
                Location = new Point(20, 320),
                Width = 320,
                Height = 50,
                ForeColor = Color.DarkGray
            };

            this.Controls.Add(lblU);
            this.Controls.Add(txtUser);
            this.Controls.Add(lblP);
            this.Controls.Add(txtPass);
            this.Controls.Add(lblK);
            this.Controls.Add(txtKey);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnRegister);
            this.Controls.Add(btnLicense);
            this.Controls.Add(lblStatus);

            this.Load += LoginForm_Load;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            try
            {
                HSAuthApp.init();
                if (HSAuthApp.response.success)
                {
                    lblStatus.Text = "Status: Ready. Connected to HS DEVS AUTH.";
                    lblStatus.ForeColor = Color.MediumSeaGreen;

                    // Listen for Live Broadcast In-App Messages
                    HSAuthApp.OnMessageReceived += (msg) =>
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            MessageBox.Show(this, msg.content, $"Broadcast: {msg.title}", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            if (msg.type == "killswitch")
                            {
                                Application.Exit();
                            }
                        });
                    };
                }
                else
                {
                    lblStatus.Text = "Error: " + HSAuthApp.response.message;
                    lblStatus.ForeColor = Color.IndianRed;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Init Error: " + ex.Message;
                lblStatus.ForeColor = Color.IndianRed;
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Authenticating...";
            HSAuthApp.login(txtUser.Text, txtPass.Text);
            HandleResponse();
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Registering...";
            HSAuthApp.register(txtUser.Text, txtPass.Text, txtKey.Text);
            HandleResponse();
        }

        private void BtnLicense_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Validating License...";
            HSAuthApp.license(txtKey.Text);
            HandleResponse();
        }

        private void HandleResponse()
        {
            if (HSAuthApp.response.success)
            {
                lblStatus.Text = "Success! " + HSAuthApp.response.message;
                lblStatus.ForeColor = Color.MediumSeaGreen;
                HSAuthApp.start_heartbeat(30);

                MessageBox.Show($"Welcome, {HSAuthApp.user_data.username}!\nHWID: {HSAuthApp.user_data.hwid}", "Authentication Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lblStatus.Text = "Failed: " + HSAuthApp.response.message;
                lblStatus.ForeColor = Color.IndianRed;
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }
}
