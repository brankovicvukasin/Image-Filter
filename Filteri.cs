using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace Projekat16011
{
    public class ConvMatrix
	{
		public int TopLeft = 0, TopMid = 0, TopRight = 0;
		public int MidLeft = 0, Pixel = 1, MidRight = 0;
		public int BottomLeft = 0, BottomMid = 0, BottomRight = 0;
		public int Factor = 1;
		public int Offset = 0;
		public void SetAll(int nVal)
		{
			TopLeft = TopMid = TopRight = MidLeft = Pixel = MidRight = BottomLeft = BottomMid = BottomRight = nVal;
		}
	}


	public struct FloatPoint
	{
		public double X;
		public double Y;
	}

	public class BitmapFilterConvolution
	{
		public const short EDGE_DETECT_KIRSH = 1;
		public const short EDGE_DETECT_PREWITT = 2;
		public const short EDGE_DETECT_SOBEL = 3;

		public static bool Conv3x3(Bitmap b, ConvMatrix m)
		{
		
			if (0 == m.Factor) return false;

			Bitmap bSrc = (Bitmap)b.Clone();

			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			int stride2 = stride * 2;
			System.IntPtr Scan0 = bmData.Scan0;
			System.IntPtr SrcScan0 = bmSrc.Scan0;

			unsafe
			{
				byte* p = (byte*)(void*)Scan0;
				byte* pSrc = (byte*)(void*)SrcScan0;

				int nOffset = stride - b.Width * 3;
				int nWidth = b.Width - 2;
				int nHeight = b.Height - 2;

				int nPixel;

				for (int y = 0; y < nHeight; ++y)
				{
					for (int x = 0; x < nWidth; ++x)
					{
						nPixel = ((((pSrc[2] * m.TopLeft) + (pSrc[5] * m.TopMid) + (pSrc[8] * m.TopRight) +
							(pSrc[2 + stride] * m.MidLeft) + (pSrc[5 + stride] * m.Pixel) + (pSrc[8 + stride] * m.MidRight) +
							(pSrc[2 + stride2] * m.BottomLeft) + (pSrc[5 + stride2] * m.BottomMid) + (pSrc[8 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

						if (nPixel < 0) nPixel = 0;
						if (nPixel > 255) nPixel = 255;

						p[5 + stride] = (byte)nPixel;

						nPixel = ((((pSrc[1] * m.TopLeft) + (pSrc[4] * m.TopMid) + (pSrc[7] * m.TopRight) +
							(pSrc[1 + stride] * m.MidLeft) + (pSrc[4 + stride] * m.Pixel) + (pSrc[7 + stride] * m.MidRight) +
							(pSrc[1 + stride2] * m.BottomLeft) + (pSrc[4 + stride2] * m.BottomMid) + (pSrc[7 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

						if (nPixel < 0) nPixel = 0;
						if (nPixel > 255) nPixel = 255;

						p[4 + stride] = (byte)nPixel;

						nPixel = ((((pSrc[0] * m.TopLeft) + (pSrc[3] * m.TopMid) + (pSrc[6] * m.TopRight) +
							(pSrc[0 + stride] * m.MidLeft) + (pSrc[3 + stride] * m.Pixel) + (pSrc[6 + stride] * m.MidRight) +
							(pSrc[0 + stride2] * m.BottomLeft) + (pSrc[3 + stride2] * m.BottomMid) + (pSrc[6 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

						if (nPixel < 0) nPixel = 0;
						if (nPixel > 255) nPixel = 255;

						p[3 + stride] = (byte)nPixel;

						p += 3;
						pSrc += 3;
					}
					p += nOffset;
					pSrc += nOffset;
				}
			}

			b.UnlockBits(bmData);
			bSrc.UnlockBits(bmSrc);

			return true;
		}

		public static bool Smooth(Bitmap b, int nWeight )
		{
			ConvMatrix m = new ConvMatrix();
			m.SetAll(1);
			m.Pixel = nWeight;
			m.Factor = nWeight + 8;

			return BitmapFilterConvolution.Conv3x3(b, m);
		}

		public static bool EdgeDetectVertical(Bitmap b)
		{
			Bitmap bmTemp = (Bitmap)b.Clone();

	
			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			BitmapData bmData2 = bmTemp.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;
			System.IntPtr Scan02 = bmData2.Scan0;

			unsafe
			{
				byte* p = (byte*)(void*)Scan0;
				byte* p2 = (byte*)(void*)Scan02;

				int nOffset = stride - b.Width * 3;
				int nWidth = b.Width * 3;

				int nPixel = 0;

				int nStride2 = stride * 2;
				int nStride3 = stride * 3;

				p += nStride3;
				p2 += nStride3;

				for (int y = 3; y < b.Height - 3; ++y)
				{
					p += 3;
					p2 += 3;

					for (int x = 3; x < nWidth - 3; ++x)
					{
						nPixel = ((p2 + nStride3 + 3)[0] +
							(p2 + nStride2 + 3)[0] +
							(p2 + stride + 3)[0] +
							(p2 + 3)[0] +
							(p2 - stride + 3)[0] +
							(p2 - nStride2 + 3)[0] +
							(p2 - nStride3 + 3)[0] -
							(p2 + nStride3 - 3)[0] -
							(p2 + nStride2 - 3)[0] -
							(p2 + stride - 3)[0] -
							(p2 - 3)[0] -
							(p2 - stride - 3)[0] -
							(p2 - nStride2 - 3)[0] -
							(p2 - nStride3 - 3)[0]);

						if (nPixel < 0) nPixel = 0;
						if (nPixel > 255) nPixel = 255;

						p[0] = (byte)nPixel;

						++p;
						++p2;
					}

					p += 3 + nOffset;
					p2 += 3 + nOffset;
				}
			}

			b.UnlockBits(bmData);
			bmTemp.UnlockBits(bmData2);

			return true;
		}

	}

}
