

using System.Collections.Generic;

namespace PartialViewEncrypter.Models
{
    public class Params
    {
        /// <summary>
        /// 命令：直接执行还是生成sql文件 1直接执行 0生成sql文件 
        /// </summary>
        public int cmd { get; set; }
        /// <summary>
        /// 数据库：string 以；分隔
        /// </summary>
        public string database { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>
        public string path { get; set; }
        /// <summary>
        /// connStr
        /// </summary>
        public string connStr { get; set; }

    }

    public enum EnumCMD
    {
        /// <summary>
        /// 加密数据库，生成sql脚本
        /// </summary>
        EncryptToSQL = 0,
        /// <summary>
        /// 加密数据库，直接执行到数据库
        /// </summary>
        EncryptToDatabase,
        /// <summary>
        /// 解密数据库，生成sql脚本
        /// </summary>
        DecryptToSQL,
        /// <summary>
        /// 解密数据库，直接执行到数据库
        /// </summary>
        DecryptToDataBase,
        /// <summary>
        /// 加密文件夹
        /// </summary>
        EncryptFolder,
        /// <summary>
        /// 加密文件
        /// </summary>
        EncryptFile,
        /// <summary>
        /// 解密文件夹
        /// </summary>
        DecryptFolder,
        /// <summary>
        /// 解密文件
        /// </summary>
        DecryptFile,
        /// <summary>
        /// 一键加密人脸
        /// </summary>
        EncryptFileOneKey,
        /// <summary>
        /// 一键解密人脸
        /// </summary>
        DecryptFileOneKey,

    }
}
