using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;
using TextBox = System.Windows.Forms.TextBox;




namespace linkedlist_quanly
{

    public class CreatePostForm : Form
    {
        private RoundedPictureBox textBox;
        private Button postButton;
        private RoundedPictureBox mediaButton;
        private RoundedPictureBox closeButton;
        private TextBox inputBox;
        private string selectedMediaPath = "";
        private System.Windows.Forms.Label mediaPreviewLabel;
        private SocialMediaLinkedList postList;
        private string currentUser;

        public CreatePostForm(SocialMediaLinkedList postList, string currentUser)
        {
            this.postList = postList;
            this.currentUser = currentUser;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Form settings - reduced size
            this.Size = new Size(350, 300); // Reduced height
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;

            // Bo tròn form
            GraphicsPath path = new GraphicsPath();
            int radius = 20;
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(this.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(this.Width - radius, this.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, this.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            this.Region = new Region(path);

            // Close button
            closeButton = new RoundedPictureBox
            {
                Size = new Size(20, 20),
                Location = new Point(this.Width - 23, 5),
                BackColor = Color.Transparent,
                CornerRadius = 15,
                DisplayText = "×",
                TextColor = Color.Gray,
                TextFont = new Font("Arial", 16, FontStyle.Bold),
                Cursor = Cursors.Hand,
            };

            closeButton.MouseEnter += (s, e) => closeButton.TextColor = Color.FromArgb(1, 95, 105);
            closeButton.MouseLeave += (s, e) => closeButton.TextColor = Color.Gray;
            closeButton.Click += (s, e) => this.Close();

            // Text input area - adjusted height
            textBox = new RoundedPictureBox
            {
                Size = new Size(310, 180), // Reduced height
                Location = new Point(20, 35),
                BackColor = Color.White,
                CornerRadius = 15,
                BorderColor = Color.FromArgb(227, 229, 228),
                ShowBorder = true,
                BorderThickness = 1
            };

            // Input box - adjusted height
            inputBox = new TextBox
            {
                Multiline = true,
                Size = new Size(290, 160), // Reduced height
                Location = new Point(10, 10),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                Font = new Font("Arial", 12),
                Text = "Bạn đang nghĩ gì?"
            };

            inputBox.Enter += (s, e) => {
                if (inputBox.Text == "Bạn đang nghĩ gì?")
                    inputBox.Text = "";
            };

            inputBox.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(inputBox.Text))
                    inputBox.Text = "Bạn đang nghĩ gì?";
            };

            textBox.Controls.Add(inputBox);

            // Media preview label - adjusted position
            mediaPreviewLabel = new System.Windows.Forms.Label
            {
                Location = new Point(50, 220), // Moved up
                Size = new Size(200, 20),
                ForeColor = Color.Gray,
                Visible = false
            };

            // Bottom panel for buttons - moved up closer to input box
            Panel bottomPanel = new Panel
            {
                Size = new Size(350, 50),
                Location = new Point(0, 230), // Moved up
                BackColor = Color.White
            };

            // Media selection button
            mediaButton = new RoundedPictureBox
            {
                Size = new Size(25, 25),
                Location = new Point(20, 12), // Relative to bottom panel
                Image = Image.FromFile("Resources/photos.png"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent,
                CornerRadius = 12,
                Cursor = Cursors.Hand
            };

            mediaButton.MouseEnter += (s, e) => mediaButton.BackColor = Color.FromArgb(227, 229, 228);
            mediaButton.MouseLeave += (s, e) => mediaButton.BackColor = Color.Transparent;
            mediaButton.Click += MediaButton_Click;

            // Post button
            postButton = new Button
            {
                Text = "Đăng",
                Size = new Size(80, 30),
                Location = new Point(250, 10), // Relative to bottom panel
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(1, 95, 105),
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            postButton.MouseEnter += (s, e) => postButton.BackColor = Color.FromArgb(1, 120, 130);
            postButton.MouseLeave += (s, e) => postButton.BackColor = Color.FromArgb(1, 95, 105);
            postButton.Click += PostButton_Click;

            // Add buttons to bottom panel
            bottomPanel.Controls.AddRange(new Control[] { mediaButton, postButton });

            // Add all controls to form
            this.Controls.AddRange(new Control[] {
        textBox,
        closeButton,
        mediaPreviewLabel,
        bottomPanel
    });

            // Form drag functionality
            this.MouseDown += (s, e) => {
                if (e.Button == MouseButtons.Left)
                {
                    NativeWin32.ReleaseCapture();
                    NativeWin32.SendMessage(this.Handle, NativeWin32.WM_NCLBUTTONDOWN, NativeWin32.HTCAPTION, 0);
                }
            };
        }

        // Keep existing methods
        private void MediaButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Media files (*.jpg;*.jpeg;*.png;*.gif;*.mp4)|*.jpg;*.jpeg;*.png;*.gif;*.mp4";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedMediaPath = ofd.FileName;
                    string fileName = Path.GetFileName(selectedMediaPath);
                    mediaPreviewLabel.Text = $"Selected: {fileName}";
                    mediaPreviewLabel.Visible = true;
                }
            }
        }

        private void PostButton_Click(object sender, EventArgs e)
        {
            string content = inputBox.Text;
            if (content == "Bạn đang nghĩ gì?" || string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show("Vui lòng nhập nội dung bài viết!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string mediaPath = null;
                if (!string.IsNullOrEmpty(selectedMediaPath))
                {
                    string mediaDirectory = Path.Combine(Application.StartupPath, "Media");
                    if (!Directory.Exists(mediaDirectory))
                    {
                        Directory.CreateDirectory(mediaDirectory);
                    }

                    string fileName = Path.GetFileName(selectedMediaPath);
                    string newPath = Path.Combine(mediaDirectory, $"{DateTime.Now.Ticks}_{fileName}");
                    File.Copy(selectedMediaPath, newPath);
                    mediaPath = newPath;
                }

                postList.AddPost(content, mediaPath, currentUser);
                MessageBox.Show("Đăng bài thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }
    }


    //Hỗ trợ kéo thả form
    public static class NativeWin32
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    }

    /*------------------TẠO FORM ĐĂNG BÀI------------------------------------*/
    public class PostManager    //thêm tính năng quản lý post ở đây
    {
        private const string POSTS_FILE = "posts.txt";
        private string filePath;

        public PostManager()
        {
            string debugPath = Path.GetDirectoryName(Application.ExecutablePath);
            filePath = Path.Combine(debugPath, POSTS_FILE);

            // Create the file if it doesn't exist
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
        }

        public void SavePost(Post post)
        {
            // Check if post already exists
            var existingPosts = LoadAllPosts();
            bool isDuplicate = existingPosts.Any(p =>
                p.Author == post.Author &&
                p.Content == post.Content &&
                Math.Abs((p.PostTime - post.PostTime).TotalSeconds) < 1); // Compare within 1 second

            if (!isDuplicate)
            {
                string postLine = $"{post.Author}|{post.Content}|{post.MediaReference}|{post.PostTime:yyyy-MM-dd HH:mm:ss}\n";
                File.AppendAllText(filePath, postLine);
            }
        }

        public List<PostData> LoadAllPosts()
        {
            var posts = new List<PostData>();

            if (!File.Exists(filePath))
                return posts;

            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split('|');
                if (parts.Length >= 4)
                {
                    posts.Add(new PostData
                    {
                        Author = parts[0],
                        Content = parts[1],
                        MediaReference = parts[2] == "null" ? null : parts[2],
                        PostTime = DateTime.Parse(parts[3])
                    });
                }
            }

            return posts;
        }

        public void InitializeDefaultPosts(string username)    //default post khi tạo một tài khoản mới
        {
            var posts = LoadAllPosts();
            if (!posts.Any(p => p.Author == username))
            {
                var defaultPosts = new List<Post>
                {
                    new Post("Hello! This is my first post!", null, username),
                    new Post("Just joined this amazing platform!", null, username)
                };

                foreach (var post in defaultPosts)
                {
                    SavePost(post);
                }
            }
        }
    }

    public class PostData        //quan ly post
    {
        public string Content { get; set; }
        public string MediaReference { get; set; }
        public DateTime PostTime { get; set; }
        public string Author { get; set; }
        public int Likes { get; set; }
    }
    public class User        //quan ly user
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string AvatarPath { get; set; } // Thêm đường dẫn ảnh đại diện
    }
    public class UserManager  //quản lý tài khoản của người dùng
    {
        private const string USER_FILE = "users.txt";
        private string filePath;

        public UserManager()
        {
            string debugPath = Path.GetDirectoryName(Application.ExecutablePath);
            filePath = Path.Combine(debugPath, USER_FILE);

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
        }

        public bool RegisterUser(string username, string password)
        {
            try
            {
                // Check if username already exists
                if (File.Exists(filePath))
                {
                    var users = File.ReadAllLines(filePath);
                    if (users.Any(u => u.Split('|')[0] == username))
                    {
                        return false;
                    }
                }

                // Save user with default avatar path
                string defaultAvatarPath = "Resources/default-avatar.png";
                File.AppendAllText(filePath, $"{username}|{password}|{defaultAvatarPath}\n");

                // Add initial posts for new user
                var postManager = new PostManager();
                postManager.InitializeDefaultPosts(username);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool ValidateUser(string username, string password)    //kiểm tra, xác thực tài khoản mới tạo
        {
            try
            {
                if (!File.Exists(filePath)) return false;

                var users = File.ReadAllLines(filePath);
                return users.Any(u =>
                {
                    var parts = u.Split('|');
                    return parts[0] == username && parts[1] == password;
                });
            }
            catch (Exception)
            {
                return false;
            }
        }
        public string GetUserAvatar(string username)    //đặt default avatar cho người dùng
        {
            try
            {
                if (!File.Exists(filePath)) return null;

                var users = File.ReadAllLines(filePath);
                var userLine = users.FirstOrDefault(u => u.Split('|')[0] == username);
                if (userLine != null)
                {
                    var parts = userLine.Split('|');
                    return parts.Length >= 3 ? parts[2] : "Resources/default-avatar.png";
                }
            }
            catch (Exception) { }

            return "Resources/default-avatar.png";
        }

        public bool UpdateUserAvatar(string username, string newAvatarPath)    //update avatar
        {
            try
            {
                if (!File.Exists(filePath)) return false;

                var users = File.ReadAllLines(filePath).ToList();
                for (int i = 0; i < users.Count; i++)
                {
                    var parts = users[i].Split('|');
                    if (parts[0] == username)
                    {
                        // Update avatar path while keeping username and password
                        users[i] = $"{parts[0]}|{parts[1]}|{newAvatarPath}";
                        File.WriteAllLines(filePath, users);
                        return true;
                    }
                }
            }
            catch (Exception) { }

            return false;
        }
    }
    public class LoginForm : CustomizedForm   //tạo form đăng nhập 
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private UserManager userManager;

        public string LoggedInUser { get; private set; }

        private Image ResizeImage(Image img, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(resizedImage))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic; // Set high-quality interpolation
                g.DrawImage(img, 0, 0, width, height);
            }
            return resizedImage;
        }
        //Round form
        private void RoundedForm_Load(object sender, EventArgs e)
        {
            int radius = 60; // Bán kính bo tròn
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(0, 0, radius, radius, 180, 90); // Góc trên bên trái
            path.AddArc(this.Width - radius, 0, radius, radius, 270, 90); // Góc trên bên phải
            path.AddArc(this.Width - radius, this.Height - radius, radius, radius, 0, 90); // Góc dưới bên phải
            path.AddArc(0, this.Height - radius, radius, radius, 90, 90); // Góc dưới bên trái
            path.CloseFigure();
            this.Region = new Region(path);
        }
        public LoginForm()
        {
            userManager = new UserManager();
            InitializeComponents();
            this.MaximizeBox = false;
            this.Text = "Đăng nhập";
            //this.Load += MainForm_Load;
            this.StartPosition = FormStartPosition.CenterScreen; // Đặt vị trí form ở giữa màn hình
            this.FormBorderStyle = FormBorderStyle.None; // Bỏ viền form
            this.Load += new EventHandler(RoundedForm_Load);            //Di chuyển 
            // Subscribe to the MouseDown, MouseMove, and MouseUp events
            this.MouseDown += new MouseEventHandler(Form1_MouseDown);
            this.MouseMove += new MouseEventHandler(Form1_MouseMove);
            this.MouseUp += new MouseEventHandler(Form1_MouseUp);
            // Enable double buffering
            this.DoubleBuffered = true;
            //Change Cursor focus
            this.Shown += new EventHandler(MainForm_Shown); // Use Shown event
        }
        //Change cursor focus
        private void MainForm_Shown(object sender, EventArgs e)
        {
            // Set focus to the form itself or another control
            this.ActiveControl = null; // This will remove focus from any control
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // Call the base method
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // Enable anti-aliasing

            // Set up the font and brush for drawing text
            Font drawFont = new Font("Arial", 13);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            // Define the position to draw the text
            float y = 635F; // Center vertically in the form

            // Measure the width of the text "or"
            string text = "or";
            SizeF textSize = e.Graphics.MeasureString(text, drawFont);
            float textWidth = textSize.Width;

            // Calculate the starting point for the lines
            float lineY = y + drawFont.Height / 2; // Center the lines with respect to the text
            float startX1 = (this.ClientSize.Width - textWidth) / 2 - 150; // Start point for the left line (longer)
            float startX2 = (this.ClientSize.Width + textWidth) / 2 + 10; // Start point for the right line

            // Set the pen color to grey
            Pen greyPen = new Pen(Color.Gray);
            // Draw the line before the text
            e.Graphics.DrawLine(greyPen, startX1, lineY, startX1 + 140, lineY); // Line before "or" (longer)

            // Draw the text "or"
            e.Graphics.DrawString(text, drawFont, drawBrush, (this.ClientSize.Width - textWidth) / 2, y); // Draw the text "or"

            // Draw the line after the text
            e.Graphics.DrawLine(greyPen, startX2, lineY, startX2 + 140, lineY); // Line after "or" (longer)

            // Add the text "4 sheep"
            string sheepText = "Made by 4sheep";
            Font sheepFont = new Font("Arial", 10); // Font size for "4 sheep"
            SolidBrush sheepBrush = new SolidBrush(Color.Black); // Set text color to grey

            // Measure the width of the text "4 sheep"
            SizeF sheepTextSize = e.Graphics.MeasureString(sheepText, sheepFont);
            float sheepTextWidth = sheepTextSize.Width;

            // Define the position to draw "4 sheep"
            float sheepY = 760; // Position below "or"

            // Draw the text "4 sheep"
            e.Graphics.DrawString(sheepText, sheepFont, sheepBrush, (this.ClientSize.Width - sheepTextWidth) / 2, sheepY); // Draw "4 sheep"

            // Clean up resources
            drawFont.Dispose();
            drawBrush.Dispose();
            greyPen.Dispose();
            sheepFont.Dispose();
            sheepBrush.Dispose();
        }
        private void InitializeComponents()
        {

            this.Size = new Size(320, 650);

            RoundedPictureBox picAvatar = new RoundedPictureBox
            {
                Size = new Size(270, 160),
                Location = new Point(25, 70),
                Image = ResizeImage(Image.FromFile("Resources/logo.png"), 270, 160),
                SizeMode = PictureBoxSizeMode.CenterImage, // Adjust image to fit
                BackColor = Color.Transparent // Optional: make background transparent
            };


            txtUsername = new TextBox
            {
                Location = new Point(20, 240 ),
                Multiline = true,
                Size = new Size(280, 40),
                Font = new Font("Arial", 35)
            };
            System.Windows.Forms.Label lblUsernamePlaceholder = new System.Windows.Forms.Label
            {
                Text = "Nhập tên",
                Location = new Point(25, 245),
                Size = new Size(200, 30),
                Font = new Font("Arial", 15),
                ForeColor = Color.Gray,
                BackColor = Color.White 

            };
            // Event handlers for Username TextBox
            txtUsername.Enter += (s, e) => {
                lblUsernamePlaceholder.Visible = false; // Hide placeholder when focused
                
            };
            lblUsernamePlaceholder.Click += (s, e) =>
            {
                txtUsername.Focus();
            };
            txtUsername.Leave += (s, e) => {
                if (string.IsNullOrEmpty(txtUsername.Text))
                {
                    lblUsernamePlaceholder.Visible = true; // Show placeholder if no text
                }
            };
            txtPassword = new TextBox
            {
                Location = new Point(20, 300),
                Multiline = true,
                Size = new Size(280, 40),
                Font = new Font("Arial", 35),
                PasswordChar = '•'
            };
            System.Windows.Forms.Label lblPasswordPlaceholder = new System.Windows.Forms.Label
            {
                Text = "Nhập mật khẩu",
                Location = new Point(20, 305),
                Size = new Size(200, 30),
                Font = new Font("Arial", 15),
                ForeColor = Color.Gray,
                BackColor = Color.White

            };
            // Event handlers for Username TextBox
            txtPassword.Enter += (s, e) => {
                lblPasswordPlaceholder.Visible = false; // Hide placeholder when focused

            };
            lblPasswordPlaceholder.Click += (s, e) =>
            {
                txtPassword.Focus();
            };
            txtPassword.Leave += (s, e) => {
                if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    lblPasswordPlaceholder.Visible = true; // Show placeholder if no text
                }
            };
            RoundedPictureBox btnLogin = new RoundedPictureBox
            {
                CornerRadius = 45,
                DisplayText = "Đăng nhập",
                BackColor = Color.FromArgb(255, 1, 95, 105),
                TextStartX = (280 - "Đăng nhập".Length) / 2 + 18,
                TextColor = Color.White, // Set the text color to 
                TextFont = new Font("Georgia", 10, FontStyle.Regular), // Set the font style
                Location = new Point(20, 355),
                Size = new Size(280, 40),
                
            };
            btnLogin.Click += BtnLogin_Click;
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = Color.FromArgb(255, 5, 115, 125); // Optional: hover effect
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = Color.FromArgb(255, 1, 95, 105); // Reset hover effect*/

            RoundedPictureBox btnRegister = new RoundedPictureBox
            {
                CornerRadius = 30,
                DisplayText = "Đăng ký",
                BackColor = Color.FromArgb(213, 111, 53), // Set the text color to 
                TextStartX = (280 - "Đăng ký".Length) / 2 + 8,
                TextColor = Color.White, // Set the text color to 
                TextFont = new Font("Georgia", 10, FontStyle.Regular), // Set the font style
                Location = new Point(35, 550),
                Size = new Size(250, 30),
                
            };
            btnRegister.Click += BtnRegister_Click;
            btnRegister.MouseEnter += (s, e) => btnRegister.BackColor = Color.FromArgb(255, 223, 131, 73); // Optional: hover effect
            btnRegister.MouseLeave += (s, e) => btnRegister.BackColor = Color.FromArgb(213, 111, 53); // Reset hover effect*/

            this.Controls.AddRange(new Control[] {
                 lblUsernamePlaceholder, txtUsername,
                lblPasswordPlaceholder, txtPassword,
                btnLogin, btnRegister, 
                picAvatar 
            });
            // Initially show placeholders
            lblUsernamePlaceholder.Visible = true;
            lblPasswordPlaceholder.Visible = true;

        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            if (userManager.ValidateUser(txtUsername.Text, txtPassword.Text))
            {
                LoggedInUser = txtUsername.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!");
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            using (var registerForm = new RegisterForm())
            {
                registerForm.ShowDialog();
            }
        }

        private void TxtUsername_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                string avatarPath = userManager.GetUserAvatar(txtUsername.Text);
                try
                {
                    //picAvatar.Image = Image.FromFile(avatarPath);
                }
                catch
                {
                    //picAvatar.Image = Image.FromFile("Resources/default-avatar.png");
                }
            }
        }
        //Xử lý phần di chuyển 
        // Mouse down event to start dragging the form
        // Import necessary functions from user32.dll
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int HTCAPTION = 0x0002;

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        // Optionally, you can handle the MouseMove event if you want to do something while dragging
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            // You can add any code here if you want to do something while dragging
        }

        // Optionally, you can handle the MouseUp event if you want to do something after dragging
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            // You can add any code here if you want to do something after dragging
        }

        // Make sure to subscribe to the MouseDown event in the designer or constructor
        private void Form1_Load(object sender, EventArgs e)
        {
            this.MouseDown += new MouseEventHandler(Form1_MouseDown);
            this.MouseMove += new MouseEventHandler(Form1_MouseMove);
            this.MouseUp += new MouseEventHandler(Form1_MouseUp);
        }
    }

    public class RegisterForm : Form                //tạo form cho đăng ký
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private Button btnRegister;
        private UserManager userManager;

        public RegisterForm()
        {
            userManager = new UserManager();
            InitializeComponents();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.Text = "Đăng ký";
        }

        private void InitializeComponents()
        {
            this.Size = new Size(300, 250);

            System.Windows.Forms.Label lblUsername = new System.Windows.Forms.Label
            {
                Text = "Tên:",
                Location = new Point(20, 20),
                Size = new Size(100, 20)
            };

            txtUsername = new TextBox
            {
                Location = new Point(120, 20),
                Size = new Size(150, 20)
            };

            System.Windows.Forms.Label lblPassword = new System.Windows.Forms.Label
            {
                Text = "Mật khẩu:",
                Location = new Point(20, 50),
                Size = new Size(100, 20)
            };

            txtPassword = new TextBox
            {
                Location = new Point(120, 50),
                Size = new Size(150, 20),
                PasswordChar = '•'
            };

            System.Windows.Forms.Label lblConfirmPassword = new System.Windows.Forms.Label
            {
                Text = "Xác nhận MK:",
                Location = new Point(20, 80),
                Size = new Size(100, 20)
            };

            txtConfirmPassword = new TextBox
            {
                Location = new Point(120, 80),
                Size = new Size(150, 20),
                PasswordChar = '•'
            };

            btnRegister = new Button
            {
                Text = "Đăng ký",
                Location = new Point(120, 120),
                Size = new Size(100, 30)
            };
            btnRegister.Click += BtnRegister_Click;

            this.Controls.AddRange(new Control[] {
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                lblConfirmPassword, txtConfirmPassword,
                btnRegister
            });
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            if (txtPassword.Text.Length < 8)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 8 ký tự!");
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!");
                return;
            }

            if (userManager.RegisterUser(txtUsername.Text, txtPassword.Text))
            {
                MessageBox.Show("Đăng ký thành công!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập đã tồn tại!");
            }
        }
    }
    public class Post
    {
        public string Content { get; set; }
        public string MediaReference { get; set; }
        public DateTime PostTime { get; set; }
        public string Author { get; set; }
        public Post Next { get; set; }
        public string SharedBy { get; set; }
        public Post OriginalPost { get; set; }
        public List<Comment> Comments { get; set; }
        public int Likes { get; set; }
        public Dictionary<string, bool> LikedUsers { get; set; }

        public Post(string content, string mediaReference, string author, DateTime? postTime = null)
        {
            Content = content;
            MediaReference = mediaReference;
            Author = author;
            PostTime = postTime ?? DateTime.Now;
            Next = null;
            SharedBy = null;
            OriginalPost = null;
            Comments = new List<Comment>();
            Likes = 0;
            LikedUsers = new Dictionary<string, bool>();
        }

        public Post(Post originalPost, string sharedBy)
        {
            Content = originalPost.Content;
            MediaReference = originalPost.MediaReference;
            Author = originalPost.Author;
            PostTime = DateTime.Now;
            Next = null;
            SharedBy = sharedBy;
            OriginalPost = originalPost;
            Comments = new List<Comment>();
            Likes = 0;
            LikedUsers = new Dictionary<string, bool>();
        }

        public void AddComment(string text, string author)
        {
            Comments.Add(new Comment(text, author, DateTime.Now));
        }
    }

    public class Comment
    {
        public string Text { get; set; }
        public string Author { get; set; }
        public DateTime PostTime { get; set; }

        public Comment(string text, string author, DateTime postTime)
        {
            Text = text;
            Author = author;
            PostTime = postTime;
        }
    }

    public class SocialMediaLinkedList
    {
        private Post Head;
        private PostManager postManager;

        public SocialMediaLinkedList()
        {
            postManager = new PostManager();
            LoadPostsFromStorage();
        }

        private void LoadPostsFromStorage()
        {
            var posts = postManager.LoadAllPosts();
            Head = null;

            foreach (var post in posts.OrderByDescending(p => p.PostTime))
            {
                AddPost(post.Content, post.MediaReference, post.Author, post.PostTime);
            }
        }

        public void AddPost(string content, string mediaReference, string author)
        {
            Post newPost = new Post(content, mediaReference, author);
            AddPostToList(newPost);
            postManager.SavePost(newPost);
        }

        // Method overload cho post với thời gian cụ thể
        public void AddPost(string content, string mediaReference, string author, DateTime postTime)
        {
            Post newPost = new Post(content, mediaReference, author, postTime);
            AddPostToList(newPost);
            postManager.SavePost(newPost);
        }

        public void SharePost(Post originalPost, string sharedBy)
        {
            Post sharedPost = new Post(originalPost, sharedBy);
            AddPostToList(sharedPost);
        }

        private void AddPostToList(Post newPost)
        {
            if (Head == null)
            {
                Head = newPost;
                return;
            }

            if (newPost.PostTime >= Head.PostTime)
            {
                newPost.Next = Head;
                Head = newPost;
                return;
            }

            Post current = Head;
            while (current.Next != null && current.Next.PostTime > newPost.PostTime)
            {
                current = current.Next;
            }
            newPost.Next = current.Next;
            current.Next = newPost;
        }

        public List<Post> GetAllPosts()
        {
            List<Post> posts = new List<Post>();
            Post current = Head;
            while (current != null)
            {
                posts.Add(current);
                current = current.Next;
            }
            return posts;
        }

        public List<Post> GetUserPosts(string author)
        {
            List<Post> userPosts = new List<Post>();
            Post current = Head;

            while (current != null)
            {
                if (current.Author == author || current.SharedBy == author)
                {
                    userPosts.Add(current);
                }
                current = current.Next;
            }

            userPosts.Sort((p1, p2) => p2.PostTime.CompareTo(p1.PostTime));
            return userPosts;
        }

        public void DeletePost(DateTime postTime)
        {
            Post current = Head;
            Post previous = null;

            while (current != null && current.PostTime != postTime)
            {
                previous = current;
                current = current.Next;
            }

            if (current != null)
            {
                if (previous == null)
                {
                    Head = current.Next;
                }
                else
                {
                    previous.Next = current.Next;
                }
            }
        }

            public void ShufflePosts()
            {
            if (Head == null || Head.Next == null)
            {
                return; // Danh sách rỗng hoặc chỉ có một phần tử
            }

            // Chuyển danh sách liên kết thành danh sách mảng
            List<Post> posts = GetAllPosts();

            // Trộn ngẫu nhiên danh sách mảng
            Random random = new Random();
            for (int i = posts.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                (posts[i], posts[j]) = (posts[j], posts[i]); // Hoán đổi
            }

            // Tái tạo danh sách liên kết
            Head = posts[0];
            Post current = Head;
            for (int i = 1; i < posts.Count; i++)
            {
                current.Next = posts[i];
                current = current.Next;
            }
            current.Next = null; // Đảm bảo nút cuối cùng không trỏ tới đâu
            }

    }
}