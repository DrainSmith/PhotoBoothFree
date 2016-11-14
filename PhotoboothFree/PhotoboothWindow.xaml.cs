using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using AForge.Video;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using System.Diagnostics;

namespace PhotoboothFree
{
    /// <summary>
    /// Interaction logic for PhotoboothWindow.xaml
    /// </summary>
    public partial class PhotoboothWindow : Window
    {
        VideoCaptureDevice LocalWebCam;
        public FilterInfoCollection LoaclWebCamsCollection;
        private List<BitmapImage> capturedImages = new List<BitmapImage>();
        public byte[] bitmapdata;
        public int width;
        public int height;
        private System.Timers.Timer CountDownTimer;
        int currentSecond;
        int picturesTaken;

        public PhotoboothWindow()
        {
            
            InitializeComponent();
            WindowState = WindowState.Maximized;
            this.Top = 0;
            this.Left = 0;
            var cams = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (cams.Count > 0)
            {
                FilterInfo i = cams[0];

                foreach (FilterInfo c in cams)
                {
                    if (c.Name == Settings.CameraName)
                        i = c;
                }


                LocalWebCam = new VideoCaptureDevice(i.MonikerString);
                LocalWebCam.VideoResolution = LocalWebCam.VideoCapabilities[Settings.VideoCapabilityIndex];
                LocalWebCam.ProvideSnapshots = true;
                //LocalWebCam.DisplayPropertyPage(IntPtr.Zero);
                LocalWebCam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);
                LocalWebCam.Start();
                CountDownTimer = new System.Timers.Timer();
                CountDownTimer.Interval = 1000;
                CountDownTimer.Enabled = true;
                CountDownTimer.Elapsed += CountDownTimer_Elapsed;
                currentSecond = Settings.WaitTimeInSeconds;
                CountDownTextBlock.Text = currentSecond.ToString();

            }
            else
            {
                System.Windows.MessageBox.Show("No cameras could be found", "Photobooth Free");
            }


        }

        private void CountDownTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(
                new Action(delegate
                {
                    if (picturesTaken < 4)
                    {
                        if (currentSecond == 0)
                        {
                            CountDownTimer.Enabled = false;
                            CountDownTextBlock.Text = "Cheese!";
                            currentSecond = Settings.WaitTimeInSeconds +1;
                            picturesTaken += 1;
                            capturedImages.Add(frameHolder.Source as BitmapImage);
                            CountDownTimer.Enabled = true;
                        }
                        else
                        {
                            CountDownTextBlock.Text = currentSecond.ToString();
                        }
                        currentSecond -= 1;
                    }
                    else
                    {
                        CountDownTimer.Enabled = false;
                        CountDownTimer.Elapsed -= CountDownTimer_Elapsed;
                        SaveImages();
                        CountDownTextBlock.Text = "";
                        
                        this.Close();
                    }
                }
                ));

        }

        void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                System.Drawing.Image img = (Bitmap)eventArgs.Frame.Clone();

                MemoryStream ms = new MemoryStream();
                img.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();

                bi.Freeze();
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    frameHolder.Source = bi;
                }));
            }
            catch (Exception ex)
            {
            }
        }

        private void GetImage()
        {
            Dispatcher.Invoke(new Action(delegate {
                
            }), System.Windows.Threading.DispatcherPriority.Normal);
        }

        private void SaveImages()
        {

            if (System.IO.File.Exists(Settings.BackgroundImagePath))
            {
                WriteableBitmap wb;
                BitmapImage fileBitmap = new BitmapImage(new Uri(Settings.BackgroundImagePath, UriKind.Relative));
                wb = new WriteableBitmap(fileBitmap);
                //wb.DpiX = 300;

                int heightOffset = Settings.TopMargin;
                double newWidth = wb.PixelWidth - Settings.SideMargin * 2;

                foreach (var i in capturedImages)
                {
                    double scaleFactor = newWidth / i.PixelWidth;

                    var bitmap = new TransformedBitmap(i, new ScaleTransform(
                        scaleFactor,
                        scaleFactor));
                    var stride = (bitmap.PixelWidth * bitmap.Format.BitsPerPixel + 7) / 8;
                    var buffer = new byte[stride * bitmap.PixelHeight];
                    bitmap.CopyPixels(buffer, stride, 0);

                    wb.WritePixels(
                        new Int32Rect(Settings.SideMargin, heightOffset, bitmap.PixelWidth, bitmap.PixelHeight),
                        buffer, stride, 0);
                    heightOffset += bitmap.PixelHeight + Settings.ImageGap;
                }
                var s = (wb.PixelWidth * wb.Format.BitsPerPixel + 7) / 8;
                var buff = new byte[s * wb.PixelHeight];
                wb.CopyPixels(buff, s, 0);

                var composite = new WriteableBitmap(wb.PixelWidth * 2, wb.PixelHeight, wb.DpiX, wb.DpiY, wb.Format, wb.Palette);
                composite.WritePixels(new Int32Rect(5, 0, wb.PixelWidth, wb.PixelHeight), buff, s, 0);
                composite.WritePixels(new Int32Rect(wb.PixelWidth, 0, wb.PixelWidth, wb.PixelHeight), buff, s, 0);
                PrintWindow pw = new PrintWindow(composite);
                pw.Show();
            }
            else
            {
                System.Windows.MessageBox.Show("No background image could be found. Nothing to attach images on to.", "PhotoboothFree");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LocalWebCam.Stop();
        }
    }
}
