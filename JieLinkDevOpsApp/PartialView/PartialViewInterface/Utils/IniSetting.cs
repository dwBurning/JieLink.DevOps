using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.Utils
{
    public class IniSetting
    {

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);


        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);



        public static long Write(string filePath, string section, string key, string value)
        {
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath))
                {
                }
            }
            return WritePrivateProfileString(section, key, value, filePath);
        }

        public static string Read(string filePath, string section, string key, string refDefaultValue)
        {
            StringBuilder stringBuilder = new StringBuilder(500);
            int privateProfileString = GetPrivateProfileString(section, key, "", stringBuilder, 500, filePath);
            if (privateProfileString < 1)
            {
                return refDefaultValue;
            }
            return stringBuilder.ToString().Trim();
        }


    }
}
