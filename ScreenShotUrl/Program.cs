using System;
using System.Windows.Forms;

using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;

namespace ScreenShotUrl
{
    class Program
    {
        public static string url             { get; set; }
        public static int    browserWidth    { get; set; }
        public static int    browserHeight   { get; set; }
        public static int    thumbnailWidth  { get; set; }
        public static int    thumbnailHeight { get; set; }
        public static string savePath        { get; set; }
        public static Bitmap bitmap          { get; set; }

        static int Main(string[] args)
        {
            try
            {
                //if (args.Length == 0)
                //{
                //    return -1;
                //}

                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);

                if (args[0].IndexOf("https") >= 0 && args[0].IndexOf("8443") >= 0)
                    url = args[0].Replace("https", "http").Replace("8443", "8080");
                else
                    url = args[0];
                
                browserWidth    = int.Parse(args[1]);
                browserHeight   = int.Parse(args[2]);
                thumbnailWidth  = int.Parse(args[3]);
                thumbnailHeight = int.Parse(args[4]);
                savePath        = args[5];

                Thread m_thread = new Thread(new ThreadStart(_GenerateWebSiteThumbnailImage));

                m_thread.SetApartmentState(ApartmentState.STA);
                m_thread.Start();
                m_thread.Join();
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
                return -1;
            }

            return 0;
        }

        private static void _GenerateWebSiteThumbnailImage()
        {
            WebBrowser m_WebBrowser = new WebBrowser();
            m_WebBrowser.ScrollBarsEnabled = false;
            m_WebBrowser.ScriptErrorsSuppressed = true;
            m_WebBrowser.Navigate(url);
            m_WebBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(WebBrowser_DocumentCompleted);
            while (m_WebBrowser.ReadyState != WebBrowserReadyState.Complete)
                Application.DoEvents();

            m_WebBrowser.Dispose();
        }

        private static void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Thread.Sleep(1500);

            WebBrowser m_WebBrowser = (WebBrowser)sender;
            m_WebBrowser.ClientSize = new Size(browserWidth, browserHeight);
            m_WebBrowser.ScrollBarsEnabled = false;
            bitmap = new Bitmap(m_WebBrowser.Bounds.Width, m_WebBrowser.Bounds.Height);
            m_WebBrowser.BringToFront();
            m_WebBrowser.DrawToBitmap(bitmap, m_WebBrowser.Bounds);


            float per = 1;

            // 넓이 기준
            if (browserWidth >= browserHeight)
            {
                per = (float)thumbnailWidth / (float)browserWidth;   
            }
            // 높이 기준
            else
            {
                per = (float)thumbnailHeight / (float)browserHeight;   
            }

            int width  = (int)((float)browserWidth * per);
            int height = (int)((float)browserHeight * per);


            using (bitmap = (Bitmap)bitmap.GetThumbnailImage(width, height, null, IntPtr.Zero))
            {
                bitmap.Save(savePath, ImageFormat.Jpeg);
            }
        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
