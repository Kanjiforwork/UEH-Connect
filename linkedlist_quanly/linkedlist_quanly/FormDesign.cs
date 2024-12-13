using linkedlist_quanly.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;



namespace linkedlist_quanly
{
 
    public partial class MainForm : CustomizedForm
    {
        private SocialMediaLinkedList postList;
        private string currentUser = "CurrentUser";
        private bool isProfileView = false;
        private FlowLayoutPanel postsPanel;
        private Random random = new Random();
        bool start = true;

        private void ReloadForm()
        {
            this.Hide(); // Ẩn form hiện tại
            Form newForm = new MainForm(); // Tạo lại instance của Form1
            newForm.Show(); // Hiển thị form mới
            this.Close(); // Đóng form hiện tại
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // Call the base method
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // or SmoothingMode.
        }

        public MainForm()
        {

            // Show login form first
            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    Application.Exit();
                    return;
                }
                currentUser = loginForm.LoggedInUser;
            }
            //InitializeComponent();
            postList = new SocialMediaLinkedList();
            this.StartPosition = FormStartPosition.CenterScreen; // Đặt vị trí form ở giữa màn hình
            this.FormBorderStyle = FormBorderStyle.None; // Bỏ viền form
            //AddSamplePosts();
            InitializeUI();
            //Di chuyển 
            // Enable double buffering
            this.DoubleBuffered = true;
        }

/*        private void AddSamplePosts()
        {
            postList.AddPost(
                "Hello, world!",
                null,
                "Việt Anh",
                new DateTime(2024, 11, 20, 14, 30, 0)
            );

            postList.AddPost(
                "My first video",
                null,
                "Thoại Hào",
                new DateTime(2024, 11, 21, 9, 15, 0)
            );

            postList.AddPost(
                "Funny GIF",
                null,
                "Taylor", // Use currentUser variable instead
                new DateTime(2024, 11, 21, 16, 45, 0)
            );

            postList.AddPost(
                "Beautiful sunset today!",
                null,
                "Bình",
                new DateTime(2024, 11, 22, 10, 20, 0)
            );

            postList.AddPost(
                "Just finished my project!",
                null,
                "John",
                new DateTime(2024, 11, 22, 11, 30, 0)
            );

            postList.AddPost(
                "I am handsome!",
                null,
                "Bình",
                new DateTime(2024, 11, 21, 11, 30, 0)
            );

            postList.AddPost(
                "I get into Harvard!",
                null,
                "OtherUser",
                new DateTime(2024, 11, 20, 11, 30, 0)
            );

            postList.ShufflePosts();
        }*/

        private string FormatTimeAgo(DateTime postTime)
        {
            TimeSpan timeDiff = DateTime.Now - postTime;

            if (timeDiff.TotalSeconds < 60)
                return $"{Math.Floor(timeDiff.TotalSeconds)} giây trước";
            if (timeDiff.TotalMinutes < 60)
                return $"{Math.Floor(timeDiff.TotalMinutes)} phút trước";
            if (timeDiff.TotalHours < 24)
                return $"{Math.Floor(timeDiff.TotalHours)} giờ trước";
            if (timeDiff.TotalDays < 7)
                return $"{Math.Floor(timeDiff.TotalDays)} ngày trước";

            return postTime.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void InitializeUI() 
        {
            this.Size = new Size(320, 650);
            this.BackColor = Color.White;
            
            //Icon Social+ 
            System.Windows.Forms.Label app_name = new System.Windows.Forms.Label
            {
                Text = "UEH Connect",
                ForeColor = Color.FromArgb(255, 1, 95, 105), 
                Location = new Point(3, 7),
                Size = new Size(180, 40), // Kích thước cố định
                Font = new Font("Inter", 12, FontStyle.Bold),
            };
           

            Panel navigationPanel = new Panel
            {
                Size = new Size(320, 40),
                Location = new Point(0, 35),
                BackColor = Color.Transparent
            };

            RoundedPictureBox homeButton = new RoundedPictureBox
            {
                Location = new Point(5, 5),
                Size = new Size(100, 30),
                Image = ResizeImage(Image.FromFile("Resources/home.png"), 30, 30),
                SizeMode = PictureBoxSizeMode.CenterImage, // Adjust image to fit
                BackColor = Color.Transparent // Optional: make background transparent

            };

            // Add mouse event handlers for click behavior
            homeButton.MouseEnter += (s, e) => homeButton.BackColor = Color.FromArgb(255, 227, 229, 228); // Optional: hover effect
            homeButton.MouseLeave += (s, e) => homeButton.BackColor = Color.Transparent; // Reset hover effect*/

            RoundedPictureBox profileButton = new RoundedPictureBox
            {
                Location = new Point(110, 5),
                Size = new Size(100, 30), 
                Image = ResizeImage(Image.FromFile("Resources/profile.png"), 30, 30),
                SizeMode = PictureBoxSizeMode.CenterImage, // Adjust image to fit
                BackColor = Color.Transparent // Optional: make background transparent
            };
            // Add mouse event handlers for click behavior
            profileButton.MouseEnter += (s, e) => profileButton.BackColor = Color.FromArgb(255, 227, 229, 228); // Optional: hover effect
            profileButton.MouseLeave += (s, e) => profileButton.BackColor = Color.Transparent; // Reset hover effect*/

            RoundedPictureBox notificationButton = new RoundedPictureBox
            {
                Location = new Point(220, 5),
                Size = new Size(100, 30),
                Image = ResizeImage(Image.FromFile("Resources/sign-out.png"), 30, 30),
                SizeMode = PictureBoxSizeMode.CenterImage, // Adjust image to fit
                BackColor = Color.Transparent // Optional: make background transparent
            };
            notificationButton.MouseEnter += (s, e) => notificationButton.BackColor = Color.FromArgb(255, 227, 229, 228); // Optional: hover effect
            notificationButton.MouseLeave += (s, e) => notificationButton.BackColor = Color.Transparent; // Reset hover effect*/

            Panel separator0 = new Panel
            {
                Size = new Size(330, 2), // Width of the postsPanel, height of the line
                Location = new Point(0, notificationButton.Bottom),
                BackColor = Color.FromArgb(255, 227, 229, 228), // Color of the line
                Margin = new Padding(0, 0, 0, 10) // Optional margin
            };
            Panel separatorForButton = new Panel
            {
                Size = new Size(330, 5), // Width of the postsPanel, height of the line
                Location = new Point(0, notificationButton.Bottom),
                BackColor = Color.FromArgb(255, 1, 95, 105), // Color of the line
                Margin = new Padding(0, 0, 0, 10) // Optional margin
            };

            navigationPanel.Controls.AddRange(new Control[] { homeButton, profileButton, notificationButton, separator0, separatorForButton });

            Panel uploadPanel  = new Panel
            {
                Size = new Size(320, 40),
                Location = new Point(0, 80)
            };

            RoundedPictureBox uploadButton = new RoundedPictureBox
            {
                CornerRadius = 30,
                Location = new Point(55, 0),
                Size = new Size(255, 30),
                DisplayText = "Bạn đang nghĩ gì?", // Set the text to display
                TextColor = Color.Black, // Set the text color to 
                TextStartX = 11,
                TextFont = new Font("Arial", 10, FontStyle.Regular), // Set the font style
                BackColor = Color.Transparent, // Optional: make background transparent
                ShowBorder = true,
                BorderColor = Color.FromArgb(255, 227, 229, 228),
                BorderThickness = 4
            };
            uploadButton.MouseEnter += (s, e) => uploadButton.BackColor = Color.FromArgb(255, 227, 229, 228); // Optional: hover effect
            uploadButton.MouseLeave += (s, e) => uploadButton.BackColor = Color.Transparent; // Reset hover effect*/



            RoundedPictureBox uehLogo = new RoundedPictureBox
            {
                Location = new Point(9, 2),
                Size = new Size(40, 25),
                Image = ResizeImage(Image.FromFile("Resources/logo.png"), 40, 25),
                SizeMode = PictureBoxSizeMode.CenterImage, // Adjust image to fit
                BackColor = Color.Transparent // Optional: make background transparent
            };
            Panel separator1 = new Panel
            {
                Size = new Size(330, 3), // Width of the postsPanel, height of the line
                Location = new Point (0,uploadButton.Bottom + 8),
                BackColor = Color.Gray, // Color of the line
                Margin = new Padding(0, 0, 0, 10) // Optional margin
            };

            uploadPanel.Controls.AddRange(new Control[] { uploadButton, uehLogo, separator1 });


            uploadButton.Click += (s, e) =>
            {
                using (var createPostForm = new CreatePostForm(postList, currentUser))
                {
                    if (createPostForm.ShowDialog() == DialogResult.OK)
                    {
                        RefreshPosts();  // Refresh ngay sau khi đăng bài thành công
                    }
                }
            };

            TextBox contentBox = new TextBox
            {
                Multiline = true,
                Size = new Size(400, 20),
                Location = new Point(20, 50)
            };

            Button postBtn = new Button
            {
                Text = "Đăng bài",
                Location = new Point(120, 180)
            };

            postsPanel = new FlowLayoutPanel
            {
                Size = new Size(340, 600),
                Location = new Point(0, 127),
                AutoScroll = true,
            };

            homeButton.Click += (s, e) =>
            {
                isProfileView = false;
                RefreshPosts();

            };

            profileButton.Click += (s, e) =>
            {
                isProfileView = true;
                RefreshPosts();
            };
            notificationButton.Click += (s, e) =>
            {
                // Optional: Show a confirmation message before exiting
                if (MessageBox.Show("Bạn có muốn thoát chương trình?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Application.Exit();
                }
            };
            string selectedMediaPath = "";

            postBtn.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(contentBox.Text))
                {
                    postList.AddPost(contentBox.Text, selectedMediaPath, currentUser);
                    RefreshPosts();
                    contentBox.Clear();
                    selectedMediaPath = "";
                }
            };

            this.Controls.AddRange(new Control[] {
                //contentBox,
                navigationPanel,
                uploadPanel,
                //postBtn,
                postsPanel,
                app_name
            });

            RefreshPosts();

        }

        private void RefreshPosts()
        {
            postList.ShufflePosts();
            postsPanel.Controls.Clear();
            var posts = postList.GetAllPosts();


            foreach (var post in posts)
            {
                // Kiểm tra điều kiện hiển thị
                if (isProfileView)
                {
                    // Trong trang cá nhân: hiển thị bài viết của user và bài user đã share
                    if (post.Author != currentUser && post.SharedBy != currentUser)
                        continue;
                }
                else
                {
                    // Trong trang chủ: chỉ hiển thị bài viết gốc (không phải bài share)
                    if (post.SharedBy != null)
                        continue;
                }
                int hasPic = 0;
                int hasComment = 0;
                Panel postPanel = new Panel
                {
                    Location = new Point(0, 0),
                    Size = new Size(320, 200 + hasComment),
                    BorderStyle = BorderStyle.None,
                    
                    Margin = new Padding(0, 0, 0, 10),
                    AutoScroll = true
                };

                // Header panel với thông tin tác giả
                Panel headerPanel = new Panel
                {
                    Size = new Size(300, 60),
                    Location = new Point(0, 0),
                    BackColor = Color.Transparent
                };

                int yOffset = 5;

                // Nếu là bài share, hiển thị thông tin người share trước
                if (post.SharedBy != null)
                {
                    System.Windows.Forms.Label shareInfoLabel = new System.Windows.Forms.Label
                    {
                        Text = $"{post.SharedBy} đã chia sẻ",
                        Font = new Font(this.Font.FontFamily, 7, FontStyle.Regular),
                        Location = new Point(0, yOffset),
                        ForeColor = Color.FromArgb(255, 1, 95, 105),
                        AutoSize = true
                    };

                    System.Windows.Forms.Label shareTimeLabel = new System.Windows.Forms.Label
                    {
                        Text = $"• {FormatTimeAgo(post.PostTime)}",
                        ForeColor = Color.Gray,
                        Location = new Point(shareInfoLabel.Right + 10, yOffset),
                        Font = new Font(this.Font.FontFamily, 7, FontStyle.Regular),
                        AutoSize = true
                    };

                    headerPanel.Controls.AddRange(new Control[] { shareInfoLabel, shareTimeLabel });
                    yOffset += 20;
                }

                // Author info
                System.Windows.Forms.Label authorLabel = new System.Windows.Forms.Label
                {
                    Text = post.Author,
                    ForeColor = Color.FromArgb(255, 1, 95, 105),
                    Font = new Font(this.Font.FontFamily, 8, FontStyle.Bold),
                    Location = new Point(8, yOffset),
                    AutoSize = true
                };

                System.Windows.Forms.Label timeLabel = new System.Windows.Forms.Label
                {
                    Text = $"• {FormatTimeAgo(post.PostTime)}",
                    ForeColor = Color.Gray,
                    Location = new Point(8, authorLabel.Bottom),
                    Font = new Font(this.Font.FontFamily, 5, FontStyle.Regular),
                    AutoSize = true
                };

                headerPanel.Controls.AddRange(new Control[] { authorLabel, timeLabel });

                // Content
                System.Windows.Forms.Label contentLabel = new System.Windows.Forms.Label
                {
                    Text = post.Content,
                    Size = new Size(300, 50),
                    Font = new Font(this.Font.FontFamily, 7, FontStyle.Regular),
                    Location = new Point(10, headerPanel.Bottom)
                };

                PictureBox mediaBox = null;
                if (!string.IsNullOrEmpty(post.MediaReference) && File.Exists(post.MediaReference))
                {
                    mediaBox = new PictureBox
                    {
                        Size = new Size(80,80),
                        Location = new Point((postPanel.Width-100)/2, contentLabel.Bottom),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        BorderStyle = BorderStyle.None
                    };

                    try
                    {
                        using (var stream = new FileStream(post.MediaReference, FileMode.Open, FileAccess.Read))
                        {
                            mediaBox.Image = ResizeImage(Image.FromStream(stream), 80, 80);
                        }
                        hasPic = 40;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading image: {ex.Message}");
                    }
                }

                // Action panel (chứa các nút like, comment, share)
                Panel actionPanel = new Panel
                {
                    Size = new Size(300, 35),
                    Location = new Point(10, contentLabel.Bottom +hasPic),
                    BackColor = Color.Transparent
                };



               

                // Comments section
                Panel commentSection = new Panel
                {
                    Size = new Size(310, 32 + hasPic + hasComment),
                    Location = new Point(10, actionPanel.Bottom + 20),
                    AutoScroll = true
                };

                TextBox commentBox = new TextBox
                {
                    Size = new Size(120, 30),
                    Font = new Font("Ariel", 7),
                    Multiline = true,
                    Location = new Point(55, 0),
                    Text = "Viết bình luận...",
                    ForeColor = Color.Gray
                };

                Button likeButton = new Button
                {
                    Text = $"❤️ {post.Likes}", // Hiển thị biểu tượng tim và số lượt thích
                    Font = new Font("Arial", 10, FontStyle.Regular),
                    Size = new Size(50, 30),  // Thu hẹp kích thước nút like
                    Location = new Point(0, 0), // Đặt nút like ở vị trí bên trái
                    BackColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                likeButton.FlatAppearance.BorderSize = 0;

                // Xử lý sự kiện nhấn nút "like"
                likeButton.Click += (s, e) =>
                {
                    if (post.LikedUsers.ContainsKey(currentUser))
                    {
                        if (post.LikedUsers[currentUser])
                        {
                            post.Likes--; // Giảm số lượt thích
                            likeButton.ForeColor = Color.Black; // Màu nút về màu ban đầu
                        }
                        else
                        {
                            post.Likes++; // Tăng số lượt thích
                            likeButton.ForeColor = Color.Red; // Đổi màu nút thành đỏ
                        }

                        post.LikedUsers[currentUser] = !post.LikedUsers[currentUser];
                    }
                    else
                    {
                        post.LikedUsers.Add(currentUser, true); // Thêm người dùng vào danh sách đã thích
                        post.Likes++;
                        likeButton.ForeColor = Color.Red; // Đổi màu nút thành đỏ
                    }

                    likeButton.Text = $"❤️ {post.Likes}"; // Cập nhật lại số lượt thích
                };
                commentSection.Controls.Add(likeButton);
                commentSection.Controls.Add(commentBox);

                commentBox.Enter += (s, e) =>
                {
                    if (commentBox.Text == "Viết bình luận...")
                    {
                        commentBox.Text = "";
                        commentBox.ForeColor = Color.Black;
                    }
                };

                commentBox.Leave += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(commentBox.Text))
                    {
                        commentBox.Text = "Viết bình luận...";
                        commentBox.ForeColor = Color.Gray;
                    }
                };

                RoundedPictureBox commentButton = new RoundedPictureBox
                {
                    Location = new Point(commentBox.Right + 5, 0),
                    Size = new Size(60, 30),
                    DisplayText = "Bình luận",
                    TextColor = Color.Black,
                    TextStartX = 2,
                    TextFont = new Font("Arial", 8),
                    BackColor = Color.Transparent,
                    ShowBorder = true,
                    BorderColor = Color.FromArgb(255, 227, 229, 228),
                    BorderThickness = 2
                };
                commentButton.MouseEnter += (s, e) => commentButton.BackColor = Color.FromArgb(255, 227, 229, 228);
                commentButton.MouseLeave += (s, e) => commentButton.BackColor = Color.Transparent;
                commentButton.Click += (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(commentBox.Text) && commentBox.Text != "Viết bình luận...")
                    {
                        post.AddComment(commentBox.Text, currentUser);
                        commentBox.Text = "Viết bình luận...";
                        commentBox.ForeColor = Color.Gray;
                        RefreshPosts();
                    }
                };

                if (mediaBox != null)
                {
                    postPanel.Controls.Add(mediaBox);
                }



                // Display existing comments
                int commentY = commentButton.Bottom + 10;
                foreach (var comment in post.Comments)
                {
                    hasComment += 40;
                    System.Windows.Forms.Label commentLabel = new System.Windows.Forms.Label
                    {
                        Size = new Size(100, 15),
                        Text = $"{comment.Author}: {comment.Text}",
                        Location = new Point(0, commentY),
                        Font = new Font("Arial", 5),
                    };

                    System.Windows.Forms.Label commentTimeLabel = new System.Windows.Forms.Label
                    {
                        Size = new Size(100, 15),
                        Text = $"• {FormatTimeAgo(comment.PostTime)}",
                        ForeColor = Color.Gray,
                        Location = new Point(commentLabel.Right + 5, commentY),
                        Font = new Font("Arial", 5),
                    };

                    commentSection.Controls.AddRange(new Control[] { commentLabel, commentTimeLabel });
                    commentY += 15;
                }

                commentSection.Controls.AddRange(new Control[] { commentBox, commentButton });
                commentSection.Height += hasComment;
                // Thêm nút share nếu không phải bài viết của chính mình và không phải bài đã share
                if (post.Author != currentUser && post.SharedBy == null)
                {
                    RoundedPictureBox shareButton = new RoundedPictureBox
                    {
                        Location = new Point(commentButton.Right + 5, 0),
                        Size = new Size(55, 30),
                        DisplayText = "Chia sẻ",
                        TextColor = Color.Black,
                        TextStartX = 5,
                        TextFont = new Font("Arial", 8),
                        BackColor = Color.Transparent,
                        ShowBorder = true,
                        BorderColor = Color.FromArgb(255, 227, 229, 228),
                        BorderThickness = 2
                    };

                    shareButton.MouseEnter += (s, e) => shareButton.BackColor = Color.FromArgb(255, 227, 229, 228);
                    shareButton.MouseLeave += (s, e) => shareButton.BackColor = Color.Transparent;

                    shareButton.Click += (s, e) =>
                    {
                        postList.SharePost(post, currentUser);
                        RefreshPosts();
                        MessageBox.Show("Đã chia sẻ bài viết thành công!");
                    };

                    commentSection.Controls.Add(shareButton);
                }
                // Thêm tất cả controls vào postPanel
                postPanel.Controls.AddRange(new Control[] {
                headerPanel,
                contentLabel,
                actionPanel,
                commentSection
                });

                // Điều chỉnh chiều cao của postPanel
                postPanel.Height = commentSection.Bottom + 7;

                // Separator
                Panel separator = new Panel
                {
                    Location = new Point(0, commentSection.Bottom + 2),
                    Size = new Size(320, 3),
                    BackColor = Color.FromArgb(200, 200, 200),
                    Margin = new Padding(0, 5, 0, 5)
                };
                postPanel.Controls.AddRange(new Control[] { separator });
                // Thêm post panel và separator vào posts panel
                postsPanel.Controls.AddRange(new Control[] { postPanel });
                if (!start)
                {
                    AdjustControlSizes(postPanel);
                }
               

            }
            start = false;
        }


        //Handle button's icon
        private void SetButtonImage(Button button, string imagePath)
            {
            Image originalImage = Image.FromFile(imagePath);
            button.Image = ResizeImage(originalImage, button.Width, button.Height);
            }

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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(428, 406);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.Name = "MainForm";
            this.ResumeLayout(false);

        }
    }
}

