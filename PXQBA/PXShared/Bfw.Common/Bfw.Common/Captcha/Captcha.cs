using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Design;

namespace Bfw.Common.Captcha
{
    /// <summary>
    /// New Captcha Object
    /// </summary>
    public class Captcha
    {
        /// <summary>
        ///     This function is used in generating Captcha image for Secruty checks in login/Registration screens. 
        /// </summary>
        /// <param name="captchaText">Randomly Generated String</param>
        /// <returns>Returns a Bitmap image with the randomly generated string</returns>
        public Bitmap CreateCaptcha(string captchaText){

            if (String.IsNullOrEmpty(captchaText))
            {
                throw new Exception("Captcha String is not passed.");
            }

            //create rectangle;
            Random rn = new Random();

            Bitmap captchaImg = new Bitmap(150, 50,PixelFormat.Format32bppArgb);

            Graphics g = Graphics.FromImage(captchaImg);
            HatchBrush hb = new HatchBrush(HatchStyle.LargeGrid, Color.White, Color.LightGray );
            
            Rectangle rect = new Rectangle(0, 0, 150, 50);
            g.FillRectangle(hb, rect);
            SizeF size;
            float fontSize = rect.Height+4;
            Font font;
            // Adjust the font size until the text fits within the image.
            do
            {
                fontSize--;
                font = new Font("verdana",fontSize+5,FontStyle.Bold);
                size = g.MeasureString(captchaText, font);
            } while (size.Width > rect.Width);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            // Create a path using the text and warp it randomly.
            GraphicsPath path = new GraphicsPath();
            path.AddString(captchaText,font.FontFamily,(int)font.Style,font.Size, rect,format);
            float v = 7F;
            PointF[] points ={new PointF(rn.Next(rect.Width) / v,rn.Next(rect.Height) / v),new PointF(rect.Width - rn.Next(rect.Width) / v,rn.Next(rect.Height) / v),
                              new PointF(rn.Next(rect.Width) / v,rect.Height - rn.Next(rect.Height) / v),new PointF(rect.Width - rn.Next(rect.Width) / v,rect.Height - rn.Next(rect.Height) / v)};
            Matrix matrix = new Matrix();
            matrix.Translate(0F, 0F);
            path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);

            // Draw the text.
            HatchBrush hatchBrush = new HatchBrush(HatchStyle.DarkHorizontal,Color.Black,Color.Black);
            g.FillPath(hatchBrush, path);

            //// Add some random noise.
            int m = Math.Max(rect.Width, rect.Height);
            for (int i = 0; i < (int)(rect.Width * rect.Height / 10F); i++)
            {
                int x = rn.Next(rect.Width);
                int y = rn.Next(rect.Height);
                int w = rn.Next(m / 51);
                int h = rn.Next(m / 51);
                g.FillEllipse(hatchBrush, x, y, w, h);
            }

            // Clean up.
            font.Dispose();
            hatchBrush.Dispose();
            g.Dispose();

            return captchaImg;

        }
        /// <summary>
        ///     This function returns randomly generated string which is used in Captcha image. The string lenght is 7 with alphanumeric charecters.
        /// </summary>
        /// <returns>Returns String value</returns>
        public string getCaptchaText()
        {

            Random r = new Random();
            string s = string.Empty;
            Char[] c = { 'a', 'b', 'c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z','0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};

            for(var i=0; i<7;i++)
            {
                int j = r.Next(0, 62);
                s += c[j];
            }
            return s;
        }


    }
}
