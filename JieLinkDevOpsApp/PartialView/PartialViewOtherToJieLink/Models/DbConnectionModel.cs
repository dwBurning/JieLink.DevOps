using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartialViewOtherToJieLink.Models
{
    
    public class DbConnectionModel
    {
        
        public string Server { get; set; }

        
        public string DbName { get; set; }

        
        public string UserName { get; set; }

        
        public string Pwd { get; set; }

        
        public string ConnectionStr
        {
            get
            {
                return string.Format("Data Source = {0}; database = {1}; User id = {2}; pwd = {3}", new object[]
				{
					this.Server,
					this.DbName,
					this.UserName,
					this.Pwd
				});
            }
        }
    }
}
