using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace JieShun.Udf.Core
{
    /// <summary>
    /// 数据库连接字符串加密解密
    /// </summary>
    public static class UdfEncrypt
    {
        private const string key = "jsst";
        /// <summary>
        /// 是否加密字符串
        /// </summary>
        /// <param name="Text"></param>
        /// <returns>是否加密</returns>
        public static bool IsEncrypted(string Text)
        {
            string[] words = new string[] { "source", "port", "user", "password", "catalog" };
            foreach (var word in words)
            {
                if (Text.ToLower().Contains(word))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Encrypt(string Text)
        {
            //return Encrypt(Text, key);
            return AesEncrypt(Text, key);
        }
        /// <summary> 
        /// 加密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Encrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(Text);
            dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(MD5Lower(sKey).Substring(0, 8));
            dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(MD5Lower(sKey).Substring(0, 8));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.FlushFinalBlock();
                    StringBuilder stringBuilder = new StringBuilder();
                    byte[] array = memoryStream.ToArray();

                    for (int i = 0; i < array.Length; i++)
                    {
                        byte b = array[i];
                        stringBuilder.AppendFormat("{0:X2}", b);
                    }
                    return stringBuilder.ToString();
                }
            }


        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Decrypt(string Text)
        {
            //return Decrypt(Text, key);
            return AesDecrypt(Text, key);
        }
        /// <summa
        /// 解密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Decrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
            int num = Text.Length / 2;
            byte[] array = new byte[num];
            for (int i = 0; i < num; i++)
            {
                int num2 = Convert.ToInt32(Text.Substring(i * 2, 2), 16);
                array[i] = (byte)num2;
            }
            dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(MD5Lower(sKey).Substring(0, 8));
            dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(MD5Lower(sKey).Substring(0, 8));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(array, 0, array.Length);
                    cryptoStream.FlushFinalBlock();
                    return Encoding.Default.GetString(memoryStream.ToArray());
                }

            }


        }

        private static string MD5Lower(string str)
        {
            string text = string.Empty;
            using (MD5 mD = MD5.Create())
            {
                byte[] array = mD.ComputeHash(Encoding.Default.GetBytes(str));
                for (int i = 0; i < array.Length; i++)
                {
                    text += array[i].ToString("x").PadLeft(2, '0');
                }
                return text;
            }

        }





        #region AES加密解密 
        /// <summary>
        /// 128位处理key 
        /// </summary>
        /// <param name="keyArray">原字节</param>
        /// <param name="key">处理key</param>
        /// <returns></returns>
        private static byte[] GetAesKey(byte[] keyArray, string key)
        {
            byte[] newArray = new byte[16];
            if (keyArray.Length == 16)
                return keyArray;
            if (keyArray.Length < 16)
            {
                for (int i = 0; i < newArray.Length; i++)
                {
                    if (i >= keyArray.Length)
                    {
                        newArray[i] = 0;
                    }
                    else
                    {
                        newArray[i] = keyArray[i];
                    }
                }
            }
            return newArray;
        }
        /// <summary>
        /// 使用AES加密字符串,按128位处理key
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <param name="key">秘钥，需要128位、256位.....</param>
        /// <returns>Base64字符串结果</returns>
        private static string AesEncrypt(string content, string key, bool autoHandle = true)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            if (autoHandle)
            {
                keyArray = GetAesKey(keyArray, key);
            }
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(content);

            SymmetricAlgorithm des = Aes.Create();
            des.Key = keyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = des.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray);
        }
        /// <summary>
        /// 使用AES解密字符串,按128位处理key
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="key">秘钥，需要128位、256位.....</param>
        /// <returns>UTF8解密结果</returns>
        private static string AesDecrypt(string content, string key, bool autoHandle = true)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            if (autoHandle)
            {
                keyArray = GetAesKey(keyArray, key);
            }
            byte[] toEncryptArray = Convert.FromBase64String(content);

            SymmetricAlgorithm des = Aes.Create();
            des.Key = keyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = des.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
        #endregion

        /// <summary>
        /// sm4加密
        /// </summary>
        /// <param name="value">原始字段</param>
        /// <returns>加密数据</returns>
        public static string SM4Encrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            var ciphertext = SM4EncryptHelper.is_sm4_ciphertext(value, value.Length);
            if (ciphertext == (int)SM4_ERROR_CODE.SM4_SUCCESS)
            {
                return value;
            }
            int outlen = 0;
            IntPtr ip = IntPtr.Zero;
            var iflg = SM4EncryptHelper.sm4_encrypt(value, value.Length, out ip, out outlen);

            string result = "";
            if (iflg==(int)SM4_ERROR_CODE.SM4_SUCCESS)
            {
                byte[] array = new byte[outlen];
                Marshal.Copy(ip, array, 0, outlen);
                result = Encoding.UTF8.GetString(array);
            }
            SM4EncryptHelper.sm4_freebuf(out ip);
            return result;
        }
        /// <summary>
        /// sm4解密
        /// </summary>
        /// <param name="text">加密字段</param>
        /// <returns>解密字符串</returns>
        public static string SM4Decrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";
            int deoutlen = 0;
            IntPtr deip = IntPtr.Zero;
            var deiflg = SM4EncryptHelper.sm4_decrypt(text, text.Length, out deip, out deoutlen);
            if (deiflg!=(int)SM4_ERROR_CODE.SM4_SUCCESS)
            {
                SM4EncryptHelper.sm4_freebuf(out deip);
                return text;
            }

            byte[] dearray = new byte[deoutlen];
            Marshal.Copy(deip, dearray, 0, deoutlen);

            var deresult = Encoding.UTF8.GetString(dearray);
            _ = SM4EncryptHelper.sm4_freebuf(out deip);
            return deresult;
        }
        /// <summary>
        /// 文件加密长字符串专用
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] SM4EncryptBinary(byte[] value)
        {
            if (value.Length < 1)
                return value;
            string basestri = Encoding.UTF8.GetString(value);
            var ciphertext = SM4EncryptHelper.is_sm4_ciphertext(basestri, basestri.Length);
            if (ciphertext==(int)SM4_ERROR_CODE.SM4_SUCCESS)
            {
                return value;
            }
            string base64str = Convert.ToBase64String(value);
            int outlen = 0;
            IntPtr ip = IntPtr.Zero;
            var iflg = SM4EncryptHelper.sm4_encrypt_binary(base64str, base64str.Length, out ip, out outlen);
            if (iflg == (int)SM4_ERROR_CODE.SM4_SUCCESS)
            {
                byte[] array = new byte[outlen];
                Marshal.Copy(ip, array, 0, outlen);
                SM4EncryptHelper.sm4_freebuf(out ip);
                return array;
            }
            SM4EncryptHelper.sm4_freebuf(out ip);
            return new byte[0];
        }

        public static byte[] SM4DecryptBinary(byte[] value)
        {
            if (value.Length < 1)
                return value;
            int deoutlen = 0;
            IntPtr deip = IntPtr.Zero;
            string base64str = Encoding.UTF8.GetString(value);
            var deiflg = SM4EncryptHelper.sm4_decrypt_binary(base64str, base64str.Length, out deip, out deoutlen);
            if (deiflg == (int)SM4_ERROR_CODE.SM4_SUCCESS)
            {
                byte[] dearray = new byte[deoutlen];
                Marshal.Copy(deip, dearray, 0, deoutlen);
                var baseDecry = Encoding.UTF8.GetString(dearray);
                var result = Convert.FromBase64String(baseDecry);
                SM4EncryptHelper.sm4_freebuf(out deip);
                return result;
            }
            SM4EncryptHelper.sm4_freebuf(out deip);
            return value;
        }


        //public static bool SM4IsEncrypted(string text)
        //{
        //    return true;
        //}

        private static int GetSM4StringLength(string text)
        {
            byte[] chars = Encoding.Default.GetBytes(text);
            int len = chars.Length;
            int end = len % 16;
            int index = len / 16;
            int outlen = 0;
            if (index > 0)
                outlen += index * 16;
            if (end > 0)
                outlen += 16;
            return outlen;
        }

    }

    public enum SM4_ERROR_CODE
    {
        /// <summary>
        /// 成功
        /// </summary>
        SM4_SUCCESS,
        /// <summary>
        /// 参数错误
        /// </summary>
        SM4_PARAM_ERR,
        /// <summary>
        /// 秘钥长度不为16
        /// </summary>
        SM4_KEY_LEN_ERR,
        /// <summary>
        /// 入参长度错误
        /// </summary>
        SM4_LENGTH_ERR,
        /// <summary>
        /// 文件相关错误
        /// </summary>
        SM4_FILE_ERR,
        /// <summary>
        /// 是密文
        /// </summary>
        SM4_CIPHER_TEXT,
        /// <summary>
        /// 非密文
        /// </summary>
        SM4_NOT_CIPHER,
        /// <summary>
        /// 校验码错误
        /// </summary>
        SM4_CHECK_CODE_ERR,
    }
}