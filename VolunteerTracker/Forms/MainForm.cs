using VolunteerTracker.Services;
using VolunteerTracker.Models;
using VolunteerTracker.Controls;
using System.Drawing.Drawing2D;

namespace VolunteerTracker.Forms
{
    public partial class MainForm : Form
    {
        private readonly SupabaseService _supabaseService;
        private DataGridView? _entriesGrid;
        private Label? _totalHoursLabel;
        private List<VolunteerServiceEntry> _entries = new();

        public MainForm(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
            InitializeComponent();
            this.Load += async (sender, e) => await LoadEntriesAsync();
        }

        private void InitializeComponent()
        {
            this.Text = "Volunteer Service Tracker";
            this.Size = new Size(1200, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;
            this.WindowState = FormWindowState.Maximized;
            this.Resize += MainForm_Resize;
            this.Paint += MainForm_Paint;

            // Header Panel with Space Theme and Gradient Border
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(25, 25, 45), // Deep space blue
                Padding = new Padding(2) // Space for gradient border
            };
            
            // Add paint event for gradient border
            headerPanel.Paint += HeaderPanel_Paint;

            var titleLabel = new Label
            {
                Text = "🚀 Volunteer Service Tracker",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(147, 112, 219), // Medium Purple
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(20, 20),
                Size = new Size(450, 40)
            };

            var signOutButton = new Button
            {
                Text = "Sign Out",
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(139, 0, 0), // Dark Red
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(headerPanel.Width - 120, 25),
                Size = new Size(100, 30)
            };

            signOutButton.Click += async (sender, e) =>
            {
                await _supabaseService.SignOutAsync();
                this.Close();
            };

            headerPanel.Controls.AddRange(new Control[] { titleLabel, signOutButton });

            // Main Container
            var mainContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 0) // Remove any margin
            };

            mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

            // Left Panel - Entry Form
            var leftPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(20)
            };
            
            var entryForm = CreateEntryForm();
            leftPanel.Controls.Add(entryForm);

            // Right Panel - Entries List
            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(20)
            };

            var entriesPanel = CreateEntriesPanel();
            rightPanel.Controls.Add(entriesPanel);

            mainContainer.Controls.Add(leftPanel, 0, 0);
            mainContainer.Controls.Add(rightPanel, 1, 0);

            this.Controls.Add(headerPanel);
            this.Controls.Add(mainContainer);
        }

        private Control CreateEntryForm()
        {
            var formPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent
            };

            var titleLabel = new Label
            {
                Text = "Add New Service Entry",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(0, 10),
                Size = new Size(formPanel.Width - 20, 30)
            };

            // Date of Service
            var dateLabel = new Label
            {
                Text = "Date of Service:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(0, 60),
                Size = new Size(120, 25)
            };

            var datePicker = new DateTimePicker
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(0, 85),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(30, 30, 30),
                Format = DateTimePickerFormat.Short
            };

            // Service Type
            var typeLabel = new Label
            {
                Text = "Service Type:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(0, 135),
                Size = new Size(120, 25)
            };

            var typeComboBox = new ComboBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(0, 160),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(30, 30, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            typeComboBox.Items.AddRange(new object[] { "Community Service", "Charity Work", "Environmental", "Education", "Healthcare", "Other" });

            // Description
            var descLabel = new Label
            {
                Text = "Description:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(0, 210),
                Size = new Size(120, 25)
            };

            var descTextBox = new TextBox
            {
                Location = new Point(0, 235),
                Size = new Size(formPanel.Width - 20, 60),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(30, 30, 30),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Hours
            var hoursLabel = new Label
            {
                Text = "Hours:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(0, 315),
                Size = new Size(120, 25)
            };

            var hoursNumericUpDown = new NumericUpDown
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(0, 340),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(30, 30, 30),
                DecimalPlaces = 1,
                Minimum = 0.1M,
                Maximum = 24.0M,
                Increment = 0.5M,
                Value = 1.0M
            };

            // Supervisor Name
            var supervisorLabel = new Label
            {
                Text = "Supervisor Name:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(0, 390),
                Size = new Size(120, 25)
            };

            var supervisorTextBox = new TextBox
            {
                Location = new Point(0, 415),
                Size = new Size(formPanel.Width - 20, 30),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(30, 30, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Signature Pad
            var signatureLabel = new Label
            {
                Text = "Supervisor Signature:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(0, 465),
                Size = new Size(150, 25)
            };

            var signaturePad = new SignaturePad
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(0, 490),
                Size = new Size(300, 100)
            };

            var clearSignatureButton = new Button
            {
                Text = "Clear Signature",
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(310, 490),
                Size = new Size(90, 30),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(75, 0, 130), // Dark Purple
                FlatStyle = FlatStyle.Flat
            };

            clearSignatureButton.Click += (sender, e) => signaturePad.Clear();

            // Submit Button
            var submitButton = new Button
            {
                Text = "Add Entry",
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(0, 610),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(75, 0, 130), // Dark Purple
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            var statusLabel = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                Location = new Point(0, 660),
                Size = new Size(formPanel.Width - 20, 60),
                TextAlign = ContentAlignment.MiddleLeft,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            submitButton.Click += async (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(typeComboBox.Text) ||
                    string.IsNullOrWhiteSpace(descTextBox.Text) ||
                    string.IsNullOrWhiteSpace(supervisorTextBox.Text) ||
                    !signaturePad.HasSignature)
                {
                    statusLabel.Text = "Please fill in all fields and provide a signature.";
                    return;
                }

                submitButton.Enabled = false;
                submitButton.Text = "Adding...";
                statusLabel.Text = "";

                try
                {
                    var signatureImage = signaturePad.GetSignatureImage();
                    if (signatureImage == null)
                    {
                        statusLabel.Text = "Failed to capture signature.";
                        return;
                    }

                    var signatureUrl = await _supabaseService.UploadSignatureImageAsync(signatureImage, "signature.png");

                    var entry = new VolunteerServiceEntry
                    {
                        DateOfService = datePicker.Value.Date,
                        ServiceType = typeComboBox.Text,
                        Description = descTextBox.Text,
                        Hours = (double)hoursNumericUpDown.Value,
                        SupervisorName = supervisorTextBox.Text,
                        SupervisorSignatureImageUrl = signatureUrl
                    };

                    await _supabaseService.CreateEntryAsync(entry);

                    // Clear form
                    typeComboBox.SelectedIndex = -1;
                    descTextBox.Clear();
                    hoursNumericUpDown.Value = 1.0M;
                    supervisorTextBox.Clear();
                    signaturePad.Clear();

                    statusLabel.ForeColor = Color.Green;
                    statusLabel.Text = "Entry added successfully!";

                    // Reload entries
                    await LoadEntriesAsync();
                }
                catch (Exception ex)
                {
                    statusLabel.ForeColor = Color.Red;
                    statusLabel.Text = $"Error: {ex.Message}";
                }
                finally
                {
                    submitButton.Enabled = true;
                    submitButton.Text = "Add Entry";
                }
            };

            formPanel.Controls.AddRange(new Control[]
            {
                titleLabel, dateLabel, datePicker, typeLabel, typeComboBox,
                descLabel, descTextBox, hoursLabel, hoursNumericUpDown,
                supervisorLabel, supervisorTextBox, signatureLabel, signaturePad,
                clearSignatureButton, submitButton, statusLabel
            });

            return formPanel;
        }

        private Control CreateEntriesPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            _entriesGrid = new DataGridView
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                Location = new Point(0, 20), // Lower the table by 20 pixels from the top
                Size = new Size(panel.Width - 20, panel.Height - 70), // Adjust height to account for top margin
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(75, 0, 130), // Dark Purple
                RowHeadersVisible = false, // Hide row headers to prevent overlap
                ColumnHeadersHeight = 35, // Ensure headers have proper height
                RowTemplate = { Height = 30 }, // Set row height
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(30, 30, 30),
                    ForeColor = Color.White,
                    SelectionBackColor = Color.FromArgb(75, 0, 130),
                    SelectionForeColor = Color.White
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(50, 50, 50),
                    ForeColor = Color.White
                },
                EnableHeadersVisualStyles = false
            };

            _entriesGrid.Columns.Add("Date", "Date");
            _entriesGrid.Columns.Add("Type", "Service Type");
            _entriesGrid.Columns.Add("Hours", "Hours");
            _entriesGrid.Columns.Add("Supervisor", "Supervisor");
            _entriesGrid.Columns.Add("Description", "Description");
            _entriesGrid.Columns.Add("Signature", "Signature");
            _entriesGrid.Columns.Add("Delete", "Delete");

            // Add cell click event for signature viewing
            _entriesGrid.CellClick += EntriesGrid_CellClick;
            _entriesGrid.CellDoubleClick += EntriesGrid_CellDoubleClick;

            // Total Hours Label at the bottom
            _totalHoursLabel = new Label
            {
                Text = "Total Hours: 0.0",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(147, 112, 219), // Medium Purple
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(0, panel.Height - 40),
                Size = new Size(panel.Width - 20, 30),
                TextAlign = ContentAlignment.MiddleRight
            };

            panel.Controls.AddRange(new Control[] { _entriesGrid, _totalHoursLabel });

            return panel;
        }

        private async Task LoadEntriesAsync()
        {
            try
            {
                _entries = await _supabaseService.GetUserEntriesAsync();
                UpdateEntriesDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading entries: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateEntriesDisplay()
        {
            _entriesGrid.Rows.Clear();

            for (int i = 0; i < _entries.Count; i++)
            {
                var entry = _entries[i];
                var rowIndex = _entriesGrid.Rows.Add(
                    entry.DateOfService.ToShortDateString(),
                    entry.ServiceType,
                    entry.Hours.ToString("F1"),
                    entry.SupervisorName,
                    entry.Description.Length > 50 ? entry.Description.Substring(0, 47) + "..." : entry.Description,
                    "View Signature",
                    "Delete"
                );

                // Style the signature button
                var signatureCell = _entriesGrid.Rows[rowIndex].Cells[5]; // Signature column
                signatureCell.Style.BackColor = Color.FromArgb(75, 0, 130); // Dark Purple
                signatureCell.Style.ForeColor = Color.White;
                signatureCell.Style.SelectionBackColor = Color.FromArgb(147, 112, 219); // Medium Purple

                // Style the delete button
                var deleteCell = _entriesGrid.Rows[rowIndex].Cells[6]; // Delete column
                deleteCell.Style.BackColor = Color.FromArgb(139, 0, 0); // Dark Red
                deleteCell.Style.ForeColor = Color.White;
                deleteCell.Style.SelectionBackColor = Color.FromArgb(220, 20, 60); // Crimson
            }

            // Update total hours display
            var totalHours = _supabaseService.CalculateTotalHours(_entries);
            if (_totalHoursLabel != null)
            {
                _totalHoursLabel.Text = $"Total Hours: {totalHours:F1}";
            }

            // Ensure the first row is visible if there are entries
            if (_entries.Count > 0 && _entriesGrid.Rows.Count > 0)
            {
                _entriesGrid.FirstDisplayedScrollingRowIndex = 0;
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            // Force refresh of all controls to update their sizes
            this.SuspendLayout();
            
            // Update form elements if they exist
            if (_entriesGrid != null)
            {
                _entriesGrid.Location = new Point(0, 20); // Maintain top margin
                _entriesGrid.Size = new Size(_entriesGrid.Parent.Width - 20, _entriesGrid.Parent.Height - 70);
            }
            
            if (_totalHoursLabel != null)
            {
                _totalHoursLabel.Size = new Size(_totalHoursLabel.Parent.Width - 20, 30);
                _totalHoursLabel.Location = new Point(0, _totalHoursLabel.Parent.Height - 40);
            }
            

            
            this.ResumeLayout();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            // Create gradient background from purple at top to black at bottom
            using (var brush = new LinearGradientBrush(
                new Point(0, 0),
                new Point(0, this.Height),
                Color.FromArgb(75, 0, 130), // Purple at top
                Color.Black)) // Black at bottom
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }

            // Draw moon decal in bottom left
            DrawMoonDecal(e.Graphics);
        }

        private void DrawMoonDecal(Graphics g)
        {
            // Solar system position and size - much larger and positioned to show only top-right
            int systemSize = 1400; // Much larger solar system
            int systemX = -systemSize + 1000; // Position further right, so most is cut off by left edge
            int systemY = this.Height - systemSize + 1100; // Position further down, showing only top-right

            // Create gradient pen for solar system outline (purple gradient) - made thicker
            using (var systemPen = new Pen(Color.FromArgb(147, 112, 219), 4)) // Medium purple outline, thicker
            {
                // Draw main solar system circle outline (most will be cut off by screen edges)
                g.DrawEllipse(systemPen, systemX, systemY, systemSize, systemSize);
            }

            // Add orbital rings for detail - made thicker
            using (var orbitPen1 = new Pen(Color.FromArgb(75, 0, 130), 3)) // Dark purple, thicker
            {
                // Orbital ring 1
                g.DrawEllipse(orbitPen1, systemX + 100, systemY + 100, systemSize - 200, systemSize - 200);
            }

            using (var orbitPen2 = new Pen(Color.FromArgb(147, 112, 219), 2)) // Medium purple, thicker
            {
                // Orbital ring 2
                g.DrawEllipse(orbitPen2, systemX + 200, systemY + 200, systemSize - 400, systemSize - 400);
                
                // Orbital ring 3
                g.DrawEllipse(orbitPen2, systemX + 300, systemY + 300, systemSize - 600, systemSize - 600);
            }

            // Add subtle glow effect around the outline
            using (var glowPen = new Pen(Color.FromArgb(30, 147, 112, 219), 8)) // Semi-transparent purple glow, thicker
            {
                g.DrawEllipse(glowPen, systemX - 4, systemY - 4, systemSize + 8, systemSize + 8);
            }
        }



        private void HeaderPanel_Paint(object sender, PaintEventArgs e)
        {
            var panel = (Panel)sender;
            var rect = panel.ClientRectangle;
            
            // Create gradient border brush
            using (var borderBrush = new LinearGradientBrush(
                new Point(0, 0),
                new Point(panel.Width, 0),
                Color.FromArgb(147, 112, 219), // Medium purple
                Color.FromArgb(75, 0, 130))) // Dark purple
            {
                // Draw gradient border
                using (var pen = new Pen(borderBrush, 2))
                {
                    // Draw border around the panel
                    e.Graphics.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
                }
            }
            
            // Add subtle glow effect
            using (var glowBrush = new LinearGradientBrush(
                new Point(0, 0),
                new Point(panel.Width, 0),
                Color.FromArgb(30, 147, 112, 219), // Semi-transparent purple
                Color.FromArgb(30, 75, 0, 130))) // Semi-transparent dark purple
            {
                using (var glowPen = new Pen(glowBrush, 4))
                {
                    e.Graphics.DrawRectangle(glowPen, 1, 1, rect.Width - 3, rect.Height - 3);
                }
            }
        }

        private void EntriesGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 5) // Signature column
                {
                    var entry = _entries[e.RowIndex];
                    ShowSignatureImage(entry.SupervisorSignatureImageUrl, entry.SupervisorName);
                }
                else if (e.ColumnIndex == 6) // Delete column
                {
                    var entry = _entries[e.RowIndex];
                    DeleteEntry(entry);
                }
            }
        }

        private void EntriesGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 4) // Description column
            {
                var entry = _entries[e.RowIndex];
                ShowFullDescription(entry.Description, entry.ServiceType, entry.DateOfService);
            }
        }

        private void ShowSignatureImage(string imageUrl, string supervisorName)
        {
            try
            {
                // Create a new form to display the signature
                var signatureForm = new Form
                {
                    Text = $"Signature - {supervisorName}",
                    Size = new Size(500, 400),
                    StartPosition = FormStartPosition.CenterParent,
                    BackColor = Color.Black,
                    ForeColor = Color.White
                };

                var pictureBox = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.White
                };

                // Load the image from URL
                using (var webClient = new System.Net.WebClient())
                {
                    var imageBytes = webClient.DownloadData(imageUrl);
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        pictureBox.Image = Image.FromStream(stream);
                    }
                }

                var closeButton = new Button
                {
                    Text = "Close",
                    Dock = DockStyle.Bottom,
                    Height = 40,
                    BackColor = Color.FromArgb(75, 0, 130),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                closeButton.Click += (sender, e) => signatureForm.Close();

                signatureForm.Controls.Add(pictureBox);
                signatureForm.Controls.Add(closeButton);
                signatureForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading signature: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowFullDescription(string description, string serviceType, DateTime dateOfService)
        {
            // Create a new form to display the full description
            var descriptionForm = new Form
            {
                Text = $"Service Description - {serviceType} ({dateOfService.ToShortDateString()})",
                Size = new Size(600, 400),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.Black,
                ForeColor = Color.White,
                MaximizeBox = true,
                MinimizeBox = false
            };

            var titleLabel = new Label
            {
                Text = $"Service Type: {serviceType}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(147, 112, 219), // Medium Purple
                BackColor = Color.Transparent,
                Location = new Point(20, 20),
                Size = new Size(descriptionForm.Width - 40, 25)
            };

            var dateLabel = new Label
            {
                Text = $"Date: {dateOfService.ToShortDateString()}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(20, 50),
                Size = new Size(descriptionForm.Width - 40, 20)
            };

            var descriptionLabel = new Label
            {
                Text = "Description:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(20, 80),
                Size = new Size(120, 20)
            };

            var descriptionTextBox = new TextBox
            {
                Text = description,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(30, 30, 30),
                Location = new Point(20, 105),
                Size = new Size(descriptionForm.Width - 60, descriptionForm.Height - 200),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            var closeButton = new Button
            {
                Text = "Close",
                Location = new Point(descriptionForm.Width - 120, descriptionForm.Height - 80),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(75, 0, 130),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            closeButton.Click += (sender, e) => descriptionForm.Close();

            descriptionForm.Controls.AddRange(new Control[] { titleLabel, dateLabel, descriptionLabel, descriptionTextBox, closeButton });
            descriptionForm.ShowDialog();
        }

        private async void DeleteEntry(VolunteerServiceEntry entry)
        {
            // Show confirmation dialog
            var result = MessageBox.Show(
                $"Are you sure you want to delete this entry?\n\nDate: {entry.DateOfService.ToShortDateString()}\nService Type: {entry.ServiceType}\nHours: {entry.Hours}\nSupervisor: {entry.SupervisorName}",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Delete from database
                    await _supabaseService.DeleteEntryAsync(entry.Id);
                    
                    // Remove from local list
                    _entries.Remove(entry);
                    
                    // Refresh display
                    UpdateEntriesDisplay();
                    
                    MessageBox.Show("Entry deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting entry: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
} 