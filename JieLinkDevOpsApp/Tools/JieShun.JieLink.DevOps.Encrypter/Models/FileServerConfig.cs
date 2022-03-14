using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JieShun.JieLink.DevOps.Encrypter.Models
{
    public class FileServerConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PicSaveDay { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int LogSaveDay { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PackageSaveDay { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AdSaveDay { get; set; }
    }

    public class FileServerConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public FileServerConfiguration FileServerConfiguration { get; set; }
    }
}
