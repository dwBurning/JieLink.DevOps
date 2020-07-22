using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface
{
    public interface IStartup
    {
        /// <summary>
        /// 启动器的名字
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 优先级
        /// </summary>
        int Priority { get; }
        void Start();

        void Exit();

    }
}
