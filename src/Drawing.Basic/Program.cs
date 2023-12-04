using System.Drawing;
using System.Drawing.Imaging;

// Configurable parameters
string text = "Hello World";
int fontSize = 20;
string fontFamilyName = "Arial";
Color backgroundColor = Color.White;
Color fontColor = Color.Black;

// Create the image with specified dimensions and background color
using (var bitmap = new Bitmap(500, 200))
{
    using (Graphics graphics = Graphics.FromImage(bitmap))
    {
        // Set background color
        graphics.Clear(backgroundColor);

        // Create font
        using (FontFamily fontFamily = new FontFamily(fontFamilyName))
        {
            using (var font = new Font(fontFamily, fontSize))
            {
                // Create brush for font color
                using (Brush brush = new SolidBrush(fontColor))
                {
                    // Calculate position to center the text
                    var format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    // Draw text on the image
                    graphics.DrawString(text, font, brush, new RectangleF(0, 0, bitmap.Width, bitmap.Height), format);
                }
            }
        }
    }

    // Save the image to a file (JPEG format)
    bitmap.Save($"output.{DateTime.UtcNow:yyyyMMddhhmmss}.jpg", ImageFormat.Jpeg);
}

Console.WriteLine("Image created successfully. Check the output.jpg file.");