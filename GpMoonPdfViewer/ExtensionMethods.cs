using MoonPdfLib.MuPdf;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GpMoonPdfViewer
{
    public static class ExtensionMethods
    {

        public enum Format
        {
            Tiff,
            TiffRenkli,
            Jpg,
            Png
        }

        public static BitmapSource Resize(this BitmapSource bfPhoto, double nWidth, double nHeight, double rotate = 0, int dpiX = 96, int dpiY = 96)
        {
            var rotateTransform = new RotateTransform(rotate);
            var scaleTransform = new ScaleTransform(nWidth / 96 * dpiX / bfPhoto.PixelWidth, nHeight / 96 * dpiY / bfPhoto.PixelHeight, 0, 0);
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(rotateTransform);
            transformGroup.Children.Add(scaleTransform);
            var tb = new TransformedBitmap(bfPhoto, transformGroup);
            tb.Freeze();
            return tb;
        }


        public static byte[] ToTiffJpegByteArray(this BitmapSource bitmapsource, Format format)
        {
            using (var outStream = new MemoryStream())
            {
                switch (format)
                {
                    case Format.TiffRenkli:
                        {
                            var encoder = new TiffBitmapEncoder { Compression = TiffCompressOption.Zip };
                            encoder.Frames.Add(BitmapFrame.Create(bitmapsource));
                            encoder.Save(outStream);
                            return outStream.ToArray();
                        }
                    case Format.Tiff:
                        {
                            var encoder = new TiffBitmapEncoder { Compression = TiffCompressOption.Ccitt4 };
                            encoder.Frames.Add(BitmapFrame.Create(bitmapsource));
                            encoder.Save(outStream);
                            return outStream.ToArray();
                        }
                    case Format.Jpg:
                        {
                            var encoder = new JpegBitmapEncoder { QualityLevel = 75 };
                            encoder.Frames.Add(BitmapFrame.Create(bitmapsource));
                            encoder.Save(outStream);
                            return outStream.ToArray();
                        }
                    case Format.Png:
                        {
                            var encoder = new PngBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(bitmapsource));
                            encoder.Save(outStream);
                            return outStream.ToArray();
                        }
                    default: throw new ArgumentOutOfRangeException(nameof(format), format, null);
                }
            }
        }

        public static BitmapImage PdfExtractSmallPreviewImage(this MoonPdfViewer moonPdfViewer, int sayfano, float zoom = 0.1f)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var bmp = MuPdfWrapper.ExtractPage(moonPdfViewer.Mpp.CurrentSource, sayfano, zoom))
                {
                    return bmp.ToBitmapSource(ImageFormat.Jpeg);
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Result;
        }

        public static BitmapImage ToBitmapSource(this Image bitmap, ImageFormat format, double decodeheight = 0)
        {
            if (bitmap != null)
            {
                var memoryStream = new MemoryStream();
                bitmap.Save(memoryStream, format);
                memoryStream.Position = 0;
                var image = new BitmapImage();
                image.BeginInit();
                if (decodeheight != 0) image.DecodePixelHeight = bitmap.Height > (int)decodeheight ? (int)decodeheight : bitmap.Height;

                image.CacheOption = BitmapCacheOption.None;
                image.StreamSource = memoryStream;
                image.EndInit();
                bitmap.Dispose();
                if (!image.IsFrozen && image.CanFreeze) image.Freeze();

                return image;
            }

            return null;
        }
    }
}
