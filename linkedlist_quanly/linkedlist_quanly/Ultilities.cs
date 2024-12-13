using linkedlist_quanly;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class RoundedPictureBox : PictureBox
{
    public int CornerRadius { get; set; } = 15; // Default corner radius
    public string DisplayText { get; set; } = string.Empty; // Text to display
    public Color TextColor { get; set; } = Color.Black; // Default text color
    public Font TextFont { get; set; } = SystemFonts.DefaultFont; // Default font for the text
    public int TextStartX { get; set; } = 0; // X position to start drawing the text

    // New properties for border customization
    public Color BorderColor { get; set; } = Color.Black; // Default border color
    public int BorderThickness { get; set; } = 2; // Default border thickness
    public bool ShowBorder { get; set; } = false; // Option to show or hide border

    public RoundedPictureBox()
    {
        this.DoubleBuffered = true; // Enable double buffering
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        // Create a graphics object for the rounded rectangle
        GraphicsPath path = new GraphicsPath();
        path.AddArc(0, 0, CornerRadius, CornerRadius, 180, 90); // Top-left
        path.AddArc(Width - CornerRadius, 0, CornerRadius, CornerRadius, 270, 90); // Top-right
        path.AddArc(Width - CornerRadius, Height - CornerRadius, CornerRadius, CornerRadius, 0, 90); // Bottom-right
        path.AddArc(0, Height - CornerRadius, CornerRadius, CornerRadius, 90, 90); // Bottom-left
        path.CloseFigure();

        // Set the region of the PictureBox to the rounded rectangle
        this.Region = new Region(path);

        // Draw the image
        base.OnPaint(pe);

        // Draw the border if ShowBorder is true
        if (ShowBorder)
        {
            using (Pen borderPen = new Pen(BorderColor, BorderThickness))
            {
                pe.Graphics.DrawPath(borderPen, path);
            }
        }

        // Draw the text if it is not empty
        if (!string.IsNullOrEmpty(DisplayText))
        {
            // Measure the size of the text
            Size textSize = TextRenderer.MeasureText(DisplayText, TextFont);

            // Calculate the position to draw the text
            PointF textPosition = new PointF(TextStartX, (ClientRectangle.Height - textSize.Height) / 2);

            // Draw the text
            using (Brush textBrush = new SolidBrush(TextColor))
            {
                pe.Graphics.DrawString(DisplayText, TextFont, textBrush, textPosition);
            }
        }
    }
}


public class CustomizedForm : Form
{
    // P/Invoke declarations for dragging the form
    [DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    private const int WM_NCLBUTTONDOWN = 0x00A1;
    private const int HTCAPTION = 0x0002;


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        ApplyRoundedCorners();
        AdjustControlSizes(this); // Call the DPI adjustment method
        this.MouseDown += new MouseEventHandler(Form_MouseDown);
    }

    private void ApplyRoundedCorners()
    {
        int radius = 60; // Radius for rounded corners
        GraphicsPath path = new GraphicsPath();
        path.StartFigure();
        path.AddArc(0, 0, radius, radius, 180, 90); // Top-left corner
        path.AddArc(this.Width - radius, 0, radius, radius, 270, 90); // Top-right corner
        path.AddArc(this.Width - radius, this.Height - radius, radius, radius, 0, 90); // Bottom-right corner
        path.AddArc(0, this.Height - radius, radius, radius, 90, 90); // Bottom-left corner
        path.CloseFigure();
        this.Region = new Region(path);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        ApplyRoundedCorners(); // Reapply rounded corners on resize
    }

    private void Form_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
        }
    }

    public void AdjustControlSizes(Control control)
    {
        // Get the current DPI
        using (Graphics g = control.CreateGraphics())
        {
            float dpiX = g.DpiX; // Get the current DPI in X direction
            float scalingFactor = dpiX / 96.0f; // Calculate scaling factor based on 96 DPI

            // Adjust the size of the control
            control.Width = (int)(control.Width * scalingFactor);
            control.Height = (int)(control.Height * scalingFactor);

            // Adjust the font size with a maximum limit
            float newFontSize = control.Font.Size * scalingFactor;

            // Set a maximum font size limit (e.g., 20 points)
            if (newFontSize > 20)
            {
                newFontSize = 20;
            }
            // Set a minimum font size limit (e.g., 8 points)
            else if (newFontSize < 8)
            {
                newFontSize = 8;
            }

            control.Font = new Font(control.Font.FontFamily, newFontSize);

            // Adjust the location
            control.Left = (int)(control.Left * scalingFactor);
            control.Top = (int)(control.Top * scalingFactor);

            // Recursively adjust child controls
            foreach (Control child in control.Controls)
            {
                AdjustControlSizes(child);
            }
        }
    }


}