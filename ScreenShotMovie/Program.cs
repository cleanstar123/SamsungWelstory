using System;
using System.Diagnostics;
using System.IO;
using System.Drawing;

namespace ScreenShotMovie
{
    class Program
    {
        // args[6] : 저장 경로
        // args[7] : 썸네일 파일명
        // args[8] : 썸네일 넓이
        // args[9] : 썸네일 높이
        static void Main(string[] args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("ffmpeg.exe");

            startInfo.WindowStyle            = ProcessWindowStyle.Hidden;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute        = false;

            string fileName     = string.Format("{0}_{1}", System.Guid.NewGuid(), Path.GetFileName(args[7]));
            string uniqFileName = GetUniqName(args[6], fileName, Path.GetExtension(args[7]));

            startInfo.Arguments = string.Format("{0} {1} {2} {3} {4} {5} {6}", args[0], args[1], args[2], args[3], args[4], args[5], Path.Combine(args[6], uniqFileName));

            var process = Process.Start(startInfo);

            while (!process.StandardOutput.EndOfStream) { }

            CreateThumbnail(Path.Combine(args[6], uniqFileName), Path.Combine(args[6], args[7]), int.Parse(args[8]), int.Parse(args[9]));

            File.Delete(Path.Combine(args[6], uniqFileName));
        }

        public static void CreateThumbnail(string sourceFilePath, string targetFilePath, int thumnailWidth, int thumnailHeight)
        {
            byte[] bytes = File.ReadAllBytes(sourceFilePath);
            MemoryStream ms = new MemoryStream(bytes);

            using (Image bigImage = Image.FromStream(ms))
            {
                int width = 0;
                int height = 0;
                float per = 1;


                // 원본 이미지가 썸네일 기준 크기 보다 크면 조정
                if (bigImage.Width > thumnailWidth || bigImage.Height > thumnailHeight)
                {
                    if (bigImage.Width > bigImage.Height)
                    {
                        per = (float)thumnailWidth / (float)bigImage.Width;
                    }
                    else
                    {
                        per = (float)thumnailHeight / (float)bigImage.Height;
                    }
                }


                // Algorithm simplified for purpose of example.
                width = (int)((float)bigImage.Width * per);
                height = (int)((float)bigImage.Height * per);


                // Now create a thumbnail
                using (Image smallImage = bigImage.GetThumbnailImage(width, height, new Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero))
                {
                    smallImage.Save(targetFilePath);
                }
            }
        }

        public static bool ThumbnailCallback()
        {
            return true;
        }

        public static string GetUniqName(string DirName, string fn, string fe)
        {
            FileInfo info = new FileInfo(DirName + "\\" + fn + fe);
            int i = 1;
            while (true)
            {
                if (info.Exists)
                {
                    info = new FileInfo(DirName + "\\" + fn + "_" + i + fe);
                    i++;
                    continue;
                }
                else
                {
                    break;
                }
            }

            return info.Name;
        }
    }
}
