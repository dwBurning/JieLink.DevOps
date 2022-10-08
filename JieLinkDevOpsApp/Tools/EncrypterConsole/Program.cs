using PartialViewInterface.Utils;
using System;

namespace EncrypterConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length<2)
            {
                return;
            }
            var cmd = args[0]; // "1":解密  "2":加密
            var text = "";
            var result = "";
            for (int i = 1; i < args.Length; i++)
            {
                text += args[i];
            }
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            if (cmd=="1") //解密
            {
                result = JieShunSM4EncryptHelper.SM4Decrypt(text);
            }
            else if (cmd == "2") //加密
            {
                result = JieShunSM4EncryptHelper.SM4Encrypt(text);
            }
          
            Console.WriteLine(result);
        }
    }
}
