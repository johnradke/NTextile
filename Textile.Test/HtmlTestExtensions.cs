using System;
using System.Drawing;
using TheArtOfDev.HtmlRenderer.WinForms;

namespace Textile.Test
{
    public static class HtmlTestExtensions
    {
        public static bool RendersEqual(this string html1, string html2, out Bitmap diff)
        {
            var image1 = new Bitmap(HtmlRender.RenderToImage(html1));
            var image2 = new Bitmap(HtmlRender.RenderToImage(html2));

            var maxWidth = Math.Max(image1.Width, image2.Width);
            var maxHeight = Math.Max(image1.Height, image2.Height);
            var yOffset = image1.Height + image2.Height + 2;

            diff = new Bitmap(maxWidth, yOffset + maxHeight);
            var graphics = Graphics.FromImage(diff);
            graphics.FillRectangle(Brushes.Cyan, 0, 0, diff.Width, diff.Height);
            graphics.DrawImage(image1, 0, 0);
            graphics.DrawImage(image2, 0, image1.Height + 1);

            var minWidth = Math.Min(image1.Width, image2.Width);
            var minHeight = Math.Min(image1.Height, image2.Height);
            var equal = true;

            for (var x = 0; x < diff.Width; x++)
            {
                for (var y = 0; y < minHeight; y++)
                {
                    if (x > minWidth || y > minHeight)
                    {
                        equal = false;
                        continue;
                    }

                    var color1 = image1.GetPixel(x, y);
                    var color2 = image2.GetPixel(x, y);
                    if (color1 != color2)
                    {
                        equal = false;
                        var whiteness = 255 - color1.DistanceFrom(color2);
                        diff.SetPixel(x, yOffset + y, Color.FromArgb(255, whiteness, whiteness));
                    }
                    else
                    {
                        diff.SetPixel(x, yOffset + y, Color.White);
                    }
                }
            }

            return equal;
        }

        private static readonly int MaxDistance = (255 ^ 2) * 4;

        public static byte DistanceFrom(this Color color1, Color color2)
        {
            var rcomp = (color1.R - color2.R) ^ 2;
            var gcomp = (color1.G - color2.G) ^ 2;
            var bcomp = (color1.B - color2.B) ^ 2;
            var acomp = (color1.A - color2.A) ^ 2;

            return (byte)Math.Round(((double)rcomp + gcomp + bcomp + acomp) / MaxDistance * 255);
        }
    }
}
