using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace PartialViewInterface.Utils
{
    public static class DESEncrypt
    {

        public static string Encrypt(string Text)
        {
            return DESEncrypt.Encrypt(Text, "JsG3PbDb");
        }


        public static string Encrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(Text);
            descryptoServiceProvider.Key = Encoding.ASCII.GetBytes(sKey.MD5Lower().Substring(0, 8));
            descryptoServiceProvider.IV = Encoding.ASCII.GetBytes(sKey.MD5Lower().Substring(0, 8));
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, descryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in memoryStream.ToArray())
            {
                stringBuilder.AppendFormat("{0:X2}", b);
            }
            return stringBuilder.ToString();
        }


        public static string Decrypt(string Text)
        {
            string result;
            try
            {
                result = DESEncrypt.Decrypt(Text, "JsG3PbDb");
            }
            catch (Exception)
            {
                throw new Exception("字符串解密失败");
            }
            return result;
        }

        public static string Decrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider();
            int num = Text.Length / 2;
            byte[] array = new byte[num];
            for (int i = 0; i < num; i++)
            {
                int num2 = Convert.ToInt32(Text.Substring(i * 2, 2), 16);
                array[i] = (byte)num2;
            }
            descryptoServiceProvider.Key = Encoding.ASCII.GetBytes(sKey.MD5Lower().Substring(0, 8));
            descryptoServiceProvider.IV = Encoding.ASCII.GetBytes(sKey.MD5Lower().Substring(0, 8));
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, descryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(array, 0, array.Length);
            cryptoStream.FlushFinalBlock();
            return Encoding.Default.GetString(memoryStream.ToArray());
        }
        public static string MD5Lower(this string str)
        {
            string text = string.Empty;
            MD5 md = MD5.Create();
            byte[] array = md.ComputeHash(Encoding.Default.GetBytes(str));
            for (int i = 0; i < array.Length; i++)
            {
                text += array[i].ToString("x").PadLeft(2, '0');
            }
            return text;
        }
    }
}
