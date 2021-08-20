using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface
{
    public interface IPartialView
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        string MenuName { get; }

        /// <summary>
        /// 标记
        /// </summary>
        string TagName { get; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        MenuType MenuType { get; }

        /// <summary>
        /// 排序规则
        /// </summary>
        int Order { get; }
    }
}
