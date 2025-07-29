using VolunteerTracker.Services;

namespace VolunteerTracker.Forms
{
    public partial class LoginForm : Form
    {
        private readonly SupabaseService _supabaseService;
        private bool _isSignUpMode = false;

        public LoginForm(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Volunteer Tracker - Login";
            this.Size = new Size(450, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(15, 15, 25);

            // Add gradient background paint event
            this.Paint += LoginForm_Paint;

            // Title Label with rocket emoji
            var titleLabel = new Label
            {
                Text = "🚀 Volunteer Service Tracker",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(147, 112, 219), // Medium Purple
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(10)
            };

            // Email Label and TextBox
            var emailLabel = new Label
            {
                Text = "Email:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(147, 112, 219), // Medium Purple
                BackColor = Color.Transparent,
                Location = new Point(75, 120),
                Size = new Size(100, 25)
            };

            var emailTextBox = new TextBox
            {
                Location = new Point(75, 150),
                Size = new Size(300, 35),
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(30, 30, 50),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Password Label and TextBox
            var passwordLabel = new Label
            {
                Text = "Password:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(147, 112, 219), // Medium Purple
                BackColor = Color.Transparent,
                Location = new Point(75, 200),
                Size = new Size(120, 25)
            };

            var passwordTextBox = new TextBox
            {
                Location = new Point(75, 230),
                Size = new Size(300, 35),
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(30, 30, 50),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            // Action Button
            var actionButton = new Button
            {
                Text = "Sign In",
                Location = new Point(75, 290),
                Size = new Size(300, 45),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                BackColor = Color.FromArgb(75, 0, 130), // Dark Purple
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            // Toggle Mode Button
            var toggleButton = new Button
            {
                Text = "Need an account? Sign Up",
                Location = new Point(75, 350),
                Size = new Size(300, 35),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(147, 112, 219), // Medium Purple
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            // Status Label
            var statusLabel = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(244, 67, 54), // Material Red
                BackColor = Color.Transparent,
                Location = new Point(75, 400),
                Size = new Size(300, 80),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Add controls to form
            this.Controls.AddRange(new Control[] 
            { 
                titleLabel, emailLabel, emailTextBox, passwordLabel, 
                passwordTextBox, actionButton, toggleButton, statusLabel 
            });

            // Event handlers
            actionButton.Click += async (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(emailTextBox.Text) || string.IsNullOrWhiteSpace(passwordTextBox.Text))
                {
                    statusLabel.Text = "Please enter both email and password.";
                    return;
                }

                actionButton.Enabled = false;
                actionButton.Text = _isSignUpMode ? "Creating Account..." : "Signing In...";
                statusLabel.Text = "";

                try
                {
                    bool success;
                    if (_isSignUpMode)
                    {
                        success = await _supabaseService.SignUpAsync(emailTextBox.Text, passwordTextBox.Text);
                        if (success)
                        {
                            statusLabel.ForeColor = Color.FromArgb(76, 175, 80); // Material Green
                            statusLabel.Text = "Account created successfully! Please check your email for verification.";
                        }
                    }
                    else
                    {
                        success = await _supabaseService.SignInAsync(emailTextBox.Text, passwordTextBox.Text);
                        if (success)
                        {
                            this.Hide();
                            var mainForm = new MainForm(_supabaseService);
                            mainForm.FormClosed += (s, args) => this.Close();
                            mainForm.Show();
                        }
                    }
                }
                catch (Exception ex)
                {
                    statusLabel.ForeColor = Color.FromArgb(244, 67, 54); // Material Red
                    statusLabel.Text = ex.Message;
                }
                finally
                {
                    actionButton.Enabled = true;
                    actionButton.Text = _isSignUpMode ? "Sign Up" : "Sign In";
                }
            };

            toggleButton.Click += (sender, e) =>
            {
                _isSignUpMode = !_isSignUpMode;
                actionButton.Text = _isSignUpMode ? "Sign Up" : "Sign In";
                toggleButton.Text = _isSignUpMode ? "Already have an account? Sign In" : "Need an account? Sign Up";
                statusLabel.Text = "";
            };

            // Enter key support
            this.AcceptButton = actionButton;
        }

        private void LoginForm_Paint(object sender, PaintEventArgs e)
        {
            // Create gradient background from purple at top to black at bottom
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new Point(0, 0),
                new Point(0, this.Height),
                Color.FromArgb(75, 0, 130), // Purple at top
                Color.Black)) // Black at bottom
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }
    }
} 