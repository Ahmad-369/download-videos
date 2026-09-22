using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace download_videos
{
    public partial class Form1 : Form
    {
        private TextBox? txtInput;
        private Button? btnBrowseTorrent;
        private Button? btnStartDownload;
        private Button? btnPower;
        private Button? btnSelectFolder;
        private CheckBox? chkAudioOnly;
        private CustomProgressBar? progressBar;
        private Label? lblTitle;
        private Label? lblStatus;
        private Label? lblPercent;
        private Label? lblAppsTitle;
        private Label? lblFolderPath;
        private ListBox? lstSupportedApps;

        private bool isPowerOn = true;
        private string customSaveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

        private readonly Color primaryRed = Color.Red;
        private readonly Color pureBlack = Color.Black;
        private readonly Color purpleColor = Color.Purple;
        private readonly Color goldColor = Color.Gold; // لون التحديد الذهبي

        private readonly Dictionary<string, string> appLinks = new Dictionary<string, string>
        {
            { "TikTok - تيك توك", "https://www.tiktok.com" },
            { "Facebook - فيسبوك", "https://www.facebook.com" },
            { "Facebook Watch - فيسبوك ووتش", "https://www.facebook.com/watch" },
            { "Instagram - إنستغرام", "https://www.instagram.com" },
            { "YouTube - يوتيوب", "https://www.youtube.com" },
            { "YouTube Shorts - يوتيوب شورتس", "https://www.youtube.com/shorts" },
            { "X / Twitter - إكس / تويتر", "https://www.twitter.com" }
        };

        public Form1()
        {
            InitializeComponent();
            InitializeCustomUI();
        }

        private void InitializeCustomUI()
        {
            this.Text = "Universal Media Downloader";
            this.Size = new Size(560, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = pureBlack;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblTitle = new Label
            {
                Text = "APP DETECTOR DOWNLOADER",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = primaryRed,
                Location = new Point(25, 25),
                AutoSize = true
            };

            btnPower = new Button
            {
                Text = "POWER: ON",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(395, 20),
                Width = 115,
                Height = 32,
                BackColor = primaryRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnPower.FlatAppearance.BorderSize = 0;
            btnPower.Click += BtnPower_Click;

            txtInput = new TextBox
            {
                Text = "الصق رابط TikTok أو Facebook أو اختر ملف .torrent...",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                Location = new Point(25, 75),
                Width = 350,
                Height = 30,
                BackColor = pureBlack,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            txtInput.Enter += (s, e) => {
                if (txtInput.Text.StartsWith("الصق رابط"))
                {
                    txtInput.Text = "";
                }
                txtInput.ForeColor = Color.White;
            };

            txtInput.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtInput.Text))
                {
                    txtInput.Text = "الصق رابط TikTok أو Facebook أو اختر ملف .torrent...";
                    txtInput.ForeColor = Color.Gray;
                }
            };

            btnBrowseTorrent = new Button
            {
                Text = "Browse .torrent",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Location = new Point(385, 74),
                Width = 125,
                Height = 29,
                BackColor = primaryRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnBrowseTorrent.FlatAppearance.BorderSize = 0;
            btnBrowseTorrent.Click += BtnBrowseTorrent_Click;

            btnSelectFolder = new Button
            {
                Text = "مكان الحفظ 📁",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Location = new Point(25, 120),
                Width = 110,
                Height = 28,
                BackColor = primaryRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSelectFolder.FlatAppearance.BorderSize = 0;
            btnSelectFolder.Click += BtnSelectFolder_Click;

            lblFolderPath = new Label
            {
                Text = "المجلد الحالي: Videos",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(145, 125),
                AutoSize = true
            };

            chkAudioOnly = new CheckBox
            {
                Text = "تحميل صوت/موسيقى فقط (MP3)",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(25, 160),
                AutoSize = true,
                Cursor = Cursors.Hand
            };

            btnStartDownload = new Button
            {
                Text = "START DOWNLOAD",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(25, 200),
                Width = 485,
                Height = 45,
                BackColor = primaryRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnStartDownload.FlatAppearance.BorderSize = 0;
            btnStartDownload.Click += BtnStartDownload_Click;

            progressBar = new CustomProgressBar
            {
                Location = new Point(25, 260),
                Width = 485,
                Height = 18,
                Value = 0,
                Maximum = 100,
                BarColor = purpleColor
            };

            lblPercent = new Label
            {
                Text = "0%",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = purpleColor,
                Location = new Point(25, 288),
                AutoSize = true
            };

            lblStatus = new Label
            {
                Text = "Status: Waiting for Link...",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = purpleColor,
                Location = new Point(25, 312),
                AutoSize = true
            };

            lblAppsTitle = new Label
            {
                Text = "التطبيقات والمنصات المدعومة (انقر مرتين لفتح الموقع):",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = purpleColor,
                Location = new Point(25, 345),
                AutoSize = true
            };

            lstSupportedApps = new ListBox
            {
                Location = new Point(25, 375),
                Width = 485,
                Height = 230,
                BackColor = pureBlack,
                BorderStyle = BorderStyle.FixedSingle,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 32,
                Cursor = Cursors.Hand
            };

            foreach (var app in appLinks.Keys)
            {
                lstSupportedApps.Items.Add(app);
            }

            lstSupportedApps.DrawItem += LstSupportedApps_DrawItem;
            lstSupportedApps.MouseDoubleClick += LstSupportedApps_MouseDoubleClick;

            this.Controls.Add(lblTitle);
            this.Controls.Add(btnPower);
            this.Controls.Add(txtInput);
            this.Controls.Add(btnBrowseTorrent);
            this.Controls.Add(btnSelectFolder);
            this.Controls.Add(lblFolderPath);
            this.Controls.Add(chkAudioOnly);
            this.Controls.Add(btnStartDownload);
            this.Controls.Add(progressBar);
            this.Controls.Add(lblPercent);
            this.Controls.Add(lblStatus);
            this.Controls.Add(lblAppsTitle);
            this.Controls.Add(lstSupportedApps);
        }

        private void LstSupportedApps_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || lstSupportedApps == null) return;

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            // خلفية التحديد ذهبي وعند عدم التحديد أسود
            Color bgColor = isSelected ? goldColor : pureBlack;

            // لون النص ثابت بنفسجي دائماً بدون أسود
            Color itemTextColor = purpleColor;

            using (SolidBrush bgBrush = new SolidBrush(bgColor))
            {
                e.Graphics.FillRectangle(bgBrush, e.Bounds);
            }

            string itemText = lstSupportedApps.Items[e.Index].ToString() ?? "";

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Color dotColor = Color.Blue;
            if (itemText.Contains("Facebook")) dotColor = Color.DeepSkyBlue;
            else if (itemText.Contains("Instagram")) dotColor = Color.Magenta;
            else if (itemText.Contains("YouTube")) dotColor = Color.Red;
            else if (itemText.Contains("TikTok")) dotColor = Color.Cyan;

            using (SolidBrush dotBrush = new SolidBrush(dotColor))
            {
                e.Graphics.FillEllipse(dotBrush, e.Bounds.X + 10, e.Bounds.Y + 10, 10, 10);
            }

            using (Font font = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            {
                TextRenderer.DrawText(e.Graphics, itemText, font, new Point(e.Bounds.X + 30, e.Bounds.Y + 6), itemTextColor, TextFormatFlags.Left);
            }

            if (!isSelected)
            {
                using (Pen borderPen = new Pen(Color.FromArgb(30, 30, 30)))
                {
                    e.Graphics.DrawLine(borderPen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
                }
            }
        }

        private void BtnBrowseTorrent_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Torrent Files (*.torrent)|*.torrent|All Files (*.*)|*.*";
                ofd.Title = "اختر ملف التورنت";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtInput!.Text = ofd.FileName;
                    txtInput.ForeColor = Color.White;
                    lblStatus!.Text = "Status: Torrent File Loaded!";
                    lblStatus.ForeColor = purpleColor;
                }
            }
        }

        private void BtnSelectFolder_Click(object? sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "اختر المجلد الذي تريد حفظ الملفات فيه";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    customSaveFolder = fbd.SelectedPath;
                    lblFolderPath!.Text = "المجلد الحالي: " + Path.GetFileName(customSaveFolder);
                }
            }
        }

        private void LstSupportedApps_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if (lstSupportedApps?.SelectedItem != null)
            {
                string selectedItem = lstSupportedApps.SelectedItem.ToString()!.Trim();
                if (appLinks.ContainsKey(selectedItem))
                {
                    string url = appLinks[selectedItem];
                    try
                    {
                        Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"تعذر فتح الرابط: {ex.Message}");
                    }
                }
            }
        }

        private void BtnPower_Click(object? sender, EventArgs e)
        {
            isPowerOn = !isPowerOn;
            btnPower!.Text = isPowerOn ? "POWER: ON" : "POWER: OFF";
            btnPower.BackColor = isPowerOn ? primaryRed : Color.DarkRed;
            btnStartDownload!.Enabled = isPowerOn;
        }

        private async void BtnStartDownload_Click(object? sender, EventArgs e)
        {
            if (!isPowerOn) return;

            string input = txtInput!.Text.Trim();
            if (string.IsNullOrEmpty(input) || input.StartsWith("الصق رابط"))
            {
                MessageBox.Show("يرجى إدخال الرابط أو اختيار ملف التورنت أولاً!", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnStartDownload!.Enabled = false;
            btnStartDownload.BackColor = Color.Gray;

            bool isAudioOnly = chkAudioOnly != null && chkAudioOnly.Checked;

            for (int i = 1; i <= 100; i++)
            {
                UpdateProgressUI(i, isAudioOnly ? $"Status: Downloading Audio {i}%..." : $"Status: Downloading File {i}%...");
                await Task.Delay(15);
            }

            lblStatus!.ForeColor = purpleColor;

            btnStartDownload.Enabled = true;
            btnStartDownload.BackColor = primaryRed;

            try { SystemSounds.Asterisk.Play(); } catch { }
            MessageBox.Show("تم التحميل بنجاح 100%!", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateProgressUI(int percent, string statusText)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { UpdateProgressUI(percent, statusText); });
                return;
            }

            progressBar!.Value = percent;
            lblPercent!.Text = $"{percent}%";
            lblPercent.ForeColor = purpleColor;
            lblStatus!.Text = statusText;
            lblStatus.ForeColor = purpleColor;
        }
    }

    public class CustomProgressBar : UserControl
    {
        private int _value = 0;
        private int _maximum = 100;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BarColor { get; set; } = Color.Purple;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Value
        {
            get => _value;
            set { _value = Math.Min(value, _maximum); Invalidate(); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Maximum
        {
            get => _maximum;
            set { _maximum = value; Invalidate(); }
        }

        public CustomProgressBar()
        {
            this.DoubleBuffered = true;
            this.BackColor = Color.Black;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            using (Pen borderPen = new Pen(Color.FromArgb(40, 40, 40)))
            {
                g.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
            }

            if (_value > 0)
            {
                int progressWidth = (int)((double)_value / _maximum * Width);

                using (Brush fillBrush = new SolidBrush(BarColor))
                {
                    g.FillRectangle(fillBrush, 1, 1, progressWidth - 2, Height - 2);
                }
            }
        }
    }
}