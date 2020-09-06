using PartialViewInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewSyncTool
{
    /// <summary>
    /// 数据库配置信息
    /// </summary>
    public class DbConfigEntity
    {
        public DbConfigEntity()
        {
            CenterDbConnStr = new DbConnEntity();
            BoxDbConnStrs = new List<DbConnEntity>();
        }

        /// <summary>
        /// 中心的数据库配置信息
        /// </summary>
        public DbConnEntity CenterDbConnStr { get; set; }

        /// <summary>
        /// 盒子的数据库配置信息
        /// </summary>
        public List<DbConnEntity> BoxDbConnStrs { get; set; }


    }
}
