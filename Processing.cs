using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat16011
{
    class Processing
    {
        public Processing()
        {

        }
        public class YCbCrColor
        {
            public double Y { get; set; }
            public double Cb { get; set; }
            public double Cr { get; set; }
        }

        public static bool ConvertToGrey(Bitmap obj)
        {
            for(int i = 0; i< obj.Width; i++)
                for( int j = 0; j< obj.Height; j++)
                {
                    Color pixel = obj.GetPixel(i, j);
                    int r = pixel.R;
                    int g = pixel.G;
                    int b = pixel.B;
                    int grey = (byte)(.299 * r + .587 * g + .114 * b);

                    r = grey;
                    g = grey;
                    b = grey;

                    obj.SetPixel(i, j, Color.FromArgb(r, g, b));
                }

            return true;
        }

        public static bool ConvertRgbYuv(Bitmap obj)
        {
            for (int i = 0; i < obj.Width; i++)
            {
                for (int j = 0; j < obj.Height; j++)
                {
                    Color pixel = obj.GetPixel(i, j);

                    int r = pixel.R;
                    int g = pixel.G;
                    int b = pixel.B;

                    double Y = 0.299 * r + 0.587 * g + 0.114 * b;
                    double Cb = -0.169 * r - 0.331 * g + 0.5 * b + 128;
                    double Cr = 0.5 * r - 0.419 * g - 0.081 * b + 128;

                    // Zaokruzivanje i osiguravanje da su vrednosti unutar opsega
                    int Y_rounded = Math.Max(0, Math.Min(255, (int)Math.Round(Y)));
                    int Cb_rounded = Math.Max(0, Math.Min(255, (int)Math.Round(Cb)));
                    int Cr_rounded = Math.Max(0, Math.Min(255, (int)Math.Round(Cr)));

                    // Postavljanje piksela na YCbCr vrednosti
                    obj.SetPixel(i, j, Color.FromArgb(Y_rounded, Cb_rounded, Cr_rounded));
                }
            }

            return true;
        }
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }
        public static Color YCbCrToRGB(double Y, double Cb, double Cr)
        {
            double R = Y + 1.402 * (Cr - 128);
            double G = Y - 0.344136 * (Cb - 128) - 0.714136 * (Cr - 128);
            double B = Y + 1.772 * (Cb - 128);

            int r = Clamp((int)Math.Round(R), 0, 255);
            int g = Clamp((int)Math.Round(G), 0, 255);
            int b = Clamp((int)Math.Round(B), 0, 255);

            return Color.FromArgb(r, g, b);
        }
        public static bool ConvertYCbCrToRgb(Bitmap bmp)
        {
           
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    Color ycbcr = bmp.GetPixel(j, i);

                    double y = ycbcr.R;
                    double cb = ycbcr.G - 128;
                    double cr = ycbcr.B - 128;

                    int r = Clamp((int)(y + 1.402 * cr), 0, 255);
                    int g = Clamp((int)(y - 0.344136 * cb - 0.714136 * cr), 0, 255);
                    int b = Clamp((int)(y + 1.772 * cb), 0, 255);

                    bmp.SetPixel(j, i, Color.FromArgb(r, g, b));
                }
            }

            return true;
        }

        public static YCbCrColor RgbToYCbCr(Color rgb)
        {
            YCbCrColor yCbCr = new YCbCrColor();

            yCbCr.Y = 0.299 * rgb.R + 0.587 * rgb.G + 0.114 * rgb.B;
            yCbCr.Cb = 128 - 0.168736 * rgb.R - 0.331264 * rgb.G + 0.5 * rgb.B;
            yCbCr.Cr = 128 + 0.5 * rgb.R - 0.418688 * rgb.G - 0.081312 * rgb.B;

            return yCbCr;
        }

        public static bool TEST(Bitmap bmp){
           
            

                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        Color originalColor = bmp.GetPixel(i, j);
                        YCbCrColor yCbCr = RgbToYCbCr(originalColor);

                  
                        int y = (int)Math.Max(0, Math.Min(255, yCbCr.Y));
                        int cb = (int)Math.Max(0, Math.Min(255, yCbCr.Cb));
                        int cr = (int)Math.Max(0, Math.Min(255, yCbCr.Cr));

                      
                        bmp.SetPixel(i, j, Color.FromArgb(y, cb, cr));
                    }
                }

            return true;
            
        }
        public static bool ConvertRgbYuvDOWNSAMPLING(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    Color rgb = bmp.GetPixel(j, i);

                    double y = 0.299 * rgb.R + 0.587 * rgb.G + 0.114 * rgb.B;
                    double cb = 128 - 0.168736 * rgb.R - 0.331264 * rgb.G + 0.5 * rgb.B;
                    double cr = 128 + 0.5 * rgb.R - 0.418688 * rgb.G - 0.081312 * rgb.B;

                    bmp.SetPixel(j, i, Color.FromArgb(Clamp((int)y, 0, 255), Clamp((int)cb, 0, 255), Clamp((int)cr, 0, 255)));
                }
            }

            // 4:2:2 Downsampling
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j += 4)
                {
                    Color pixel = bmp.GetPixel(j, i);
                    Color pixel2 = bmp.GetPixel(j + 2, i);

                    int y2 = pixel2.R;

                    int y = pixel.R;
                    int cb = pixel.G;
                    int cr = pixel.B;

                    bmp.SetPixel(j, i, Color.FromArgb(y, cb, cr));
                    bmp.SetPixel(j + 2, i, Color.FromArgb(y2, cb, cr));
                }
            }
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 1; j < bmp.Width; j += 4)
                {
                    Color pixel = bmp.GetPixel(j, i);
                    Color pixel2 = bmp.GetPixel(j + 2, i);

                    int y2 = pixel2.R;

                    int y = pixel.R;
                    int cb = pixel.G;
                    int cr = pixel.B;

                    bmp.SetPixel(j, i, Color.FromArgb(y, cb, cr));
                    bmp.SetPixel(j + 2, i, Color.FromArgb(y2, cb, cr));
                }
            }
            return true; 
        }

        public static bool FilterGamma(Bitmap image, double Gred, double Gblue, double Ggreen)
        {
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color originalColor = image.GetPixel(x, y);

                    double r = originalColor.R / 255.0;
                    double g = originalColor.G / 255.0;
                    double b = originalColor.B / 255.0;

                    r = Math.Pow(r, Gred);
                    g = Math.Pow(g, Gblue);
                    b = Math.Pow(b, Ggreen);
            
                    Color gammaCorrectedColor = Color.FromArgb(
                        (int)(r * 255),
                        (int)(g * 255),
                        (int)(b * 255)
                    );
                    image.SetPixel(x, y, gammaCorrectedColor);
                }
            }
            return true;
        }

        public static bool TimeWarp(Bitmap b, Byte factor, bool bSmoothing)
        {
            int nWidth = b.Width;
            int nHeight = b.Height;

            FloatPoint[,] fp = new FloatPoint[nWidth, nHeight];
            Point[,] pt = new Point[nWidth, nHeight];

            Point mid = new Point();
            mid.X = nWidth / 2;
            mid.Y = nHeight / 2;

            double theta, radius;
            double newX, newY;

            for (int x = 0; x < nWidth; ++x)
                for (int y = 0; y < nHeight; ++y)
                {
                    int trueX = x - mid.X;
                    int trueY = y - mid.Y;
                    theta = Math.Atan2((trueY), (trueX));

                    radius = Math.Sqrt(trueX * trueX + trueY * trueY);

                    double newRadius = Math.Sqrt(radius) * factor;

                    newX = mid.X + (newRadius * Math.Cos(theta));
                    if (newX > 0 && newX < nWidth)
                    {
                        fp[x, y].X = newX;
                        pt[x, y].X = (int)newX;
                    }
                    else
                    {
                        fp[x, y].X = 0.0;
                        pt[x, y].X = 0;
                    }

                    newY = mid.Y + (newRadius * Math.Sin(theta));
                    if (newY > 0 && newY < nHeight)
                    {
                        fp[x, y].Y = newY;
                        pt[x, y].Y = (int)newY;
                    }
                    else
                    {
                        fp[x, y].Y = 0.0;
                        pt[x, y].Y = 0;
                    }
                }

            if (bSmoothing)
                OffsetFilterAbs(b, pt);
            else
                OffsetFilterAntiAlias(b, fp);

            return true;
        }

        public static bool OffsetFilterAbs(Bitmap b, Point[,] offset)
        {
            Bitmap bSrc = (Bitmap)b.Clone();

            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int scanline = bmData.Stride;

            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr SrcScan0 = bmSrc.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pSrc = (byte*)(void*)SrcScan0;

                int nOffset = bmData.Stride - b.Width * 3;
                int nWidth = b.Width;
                int nHeight = b.Height;

                int xOffset, yOffset;

                for (int y = 0; y < nHeight; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        xOffset = offset[x, y].X;
                        yOffset = offset[x, y].Y;

                        if (yOffset >= 0 && yOffset < nHeight && xOffset >= 0 && xOffset < nWidth)
                        {
                            p[0] = pSrc[(yOffset * scanline) + (xOffset * 3)];
                            p[1] = pSrc[(yOffset * scanline) + (xOffset * 3) + 1];
                            p[2] = pSrc[(yOffset * scanline) + (xOffset * 3) + 2];
                        }

                        p += 3;
                    }
                    p += nOffset;
                }
            }

            b.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);

            return true;
        }

        public static bool OffsetFilterAntiAlias(Bitmap b, FloatPoint[,] fp)
        {
            Bitmap bSrc = (Bitmap)b.Clone();

            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int scanline = bmData.Stride;

            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr SrcScan0 = bmSrc.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pSrc = (byte*)(void*)SrcScan0;

                int nOffset = bmData.Stride - b.Width * 3;
                int nWidth = b.Width;
                int nHeight = b.Height;

                double xOffset, yOffset;

                double fraction_x, fraction_y, one_minus_x, one_minus_y;
                int ceil_x, ceil_y, floor_x, floor_y;
                Byte p1, p2;

                for (int y = 0; y < nHeight; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        xOffset = fp[x, y].X;
                        yOffset = fp[x, y].Y;

                        // Setup

                        floor_x = (int)Math.Floor(xOffset);
                        floor_y = (int)Math.Floor(yOffset);
                        ceil_x = floor_x + 1;
                        ceil_y = floor_y + 1;
                        fraction_x = xOffset - floor_x;
                        fraction_y = yOffset - floor_y;
                        one_minus_x = 1.0 - fraction_x;
                        one_minus_y = 1.0 - fraction_y;

                        if (floor_y >= 0 && ceil_y < nHeight && floor_x >= 0 && ceil_x < nWidth)
                        {
                            // Blue

                            p1 = (Byte)(one_minus_x * (double)(pSrc[floor_y * scanline + floor_x * 3]) +
                                fraction_x * (double)(pSrc[floor_y * scanline + ceil_x * 3]));

                            p2 = (Byte)(one_minus_x * (double)(pSrc[ceil_y * scanline + floor_x * 3]) +
                                fraction_x * (double)(pSrc[ceil_y * scanline + 3 * ceil_x]));

                            p[x * 3 + y * scanline] = (Byte)(one_minus_y * (double)(p1) + fraction_y * (double)(p2));

                            // Green

                            p1 = (Byte)(one_minus_x * (double)(pSrc[floor_y * scanline + floor_x * 3 + 1]) +
                                fraction_x * (double)(pSrc[floor_y * scanline + ceil_x * 3 + 1]));

                            p2 = (Byte)(one_minus_x * (double)(pSrc[ceil_y * scanline + floor_x * 3 + 1]) +
                                fraction_x * (double)(pSrc[ceil_y * scanline + 3 * ceil_x + 1]));

                            p[x * 3 + y * scanline + 1] = (Byte)(one_minus_y * (double)(p1) + fraction_y * (double)(p2));

                            // Red

                            p1 = (Byte)(one_minus_x * (double)(pSrc[floor_y * scanline + floor_x * 3 + 2]) +
                                fraction_x * (double)(pSrc[floor_y * scanline + ceil_x * 3 + 2]));

                            p2 = (Byte)(one_minus_x * (double)(pSrc[ceil_y * scanline + floor_x * 3 + 2]) +
                                fraction_x * (double)(pSrc[ceil_y * scanline + 3 * ceil_x + 2]));

                            p[x * 3 + y * scanline + 2] = (Byte)(one_minus_y * (double)(p1) + fraction_y * (double)(p2));
                        }
                    }
                }
            }

            b.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);

            return true;
        }

    }




}
