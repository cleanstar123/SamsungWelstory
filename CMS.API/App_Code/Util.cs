using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Text;
using System.Security.Cryptography;

namespace CMS.API.App_Code
{
    public class Util
    {
        /// <summary>
        /// 데이터셋 NULL 체크
        /// </summary>
        /// <param name="ds">데이터셋</param>
        /// <returns>데이터셋이면 NULL이면 FALSE, 데이터셋이 NULL이 아니면 TRUE</returns>
        public static bool IsNullDataset(DataSet ds)
        {
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 중복파일명 체크
        /// </summary>
        /// <param name="DirName">체크할 디렉토리</param>
        /// <param name="fn">파일명</param>
        /// <param name="fe">확장자</param>
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

        /// <summary>
        /// 원본 이미지에 대한 썸네일 이미지를 생성
        /// </summary>
        /// <param name="sourceFilePath">원본이미지 파일 경로</param>
        /// <param name="targetFilePath">생성할 썸네일 이미지 파일 경로</param>
        public static void CreateThumbnail(string sourceFilePath, string targetFilePath, int thumnailWidth, int thumnailHeight)
        {
            byte[] bytes = File.ReadAllBytes(sourceFilePath);
            MemoryStream ms = new MemoryStream(bytes);

            using (Image bigImage = Image.FromStream(ms))
            {
                int width  = 0;
                int height = 0;
                float per  = 1;


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
                width  = (int)((float)bigImage.Width  * per);
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

        /// <summary>
        /// 데이터 테이블 리스트 변환
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName].ToString(), null);
                    else
                        continue;
                }
            }
            return obj;
        }

        /// <summary>
        /// 복호화
        /// </summary>
        /// <param name="textToDecrypt"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(string textToDecrypt, string key)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode      = CipherMode.CBC;
            rijndaelCipher.Padding   = PaddingMode.PKCS7;
            rijndaelCipher.KeySize   = 128;
            rijndaelCipher.BlockSize = 128;

            byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];

            int len = pwdBytes.Length;

            if (len > keyBytes.Length)
            {
                len = keyBytes.Length;
            }

            Array.Copy(pwdBytes, keyBytes, len);

            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV  = keyBytes;

            byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);

            return Encoding.UTF8.GetString(plainText);
        }

        /// <summary>
        /// 암호화
        /// </summary>
        /// <param name="textToEncrypt"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt(string textToEncrypt, string key)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode    = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;

            rijndaelCipher.KeySize   = 128;
            rijndaelCipher.BlockSize = 128;

            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);

            byte[] keyBytes = new byte[16];

            int len = pwdBytes.Length;

            if (len > keyBytes.Length)
            {
                len = keyBytes.Length;
            }

            Array.Copy(pwdBytes, keyBytes, len);

            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV  = keyBytes;

            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();

            byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);

            return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
        }
    }
}