using System;
using System.Diagnostics;
using System.IO;

namespace ConvertMP4
{
    class Program
    {
        // args[0] : i
        // args[1] : 동영상 원본 경로
        // args[2] : 저장 경로
        static void Main(string[] args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("ffmpeg.exe");

            startInfo.WindowStyle            = ProcessWindowStyle.Hidden;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute        = false;

            startInfo.Arguments = string.Format("{0} {1} {2}", args[0], args[1], args[2]);

            var process = Process.Start(startInfo);

            while (!process.StandardOutput.EndOfStream) { }

            File.Delete(args[1]);
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
