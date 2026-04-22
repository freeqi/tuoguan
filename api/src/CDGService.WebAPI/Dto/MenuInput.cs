using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    /// <summary>
    /// 菜单输入实体
    /// </summary>
    public class MenuInput
    {

        /// <summary>
        /// ID
        /// </summary>
        public  string  Id { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }
        /// <summary>
        /// 父级菜单
        /// </summary>
        public  string  ParentMenuCode { get; set; }
        /// <summary>
        /// 菜单链接
        /// </summary>
        public string MenuUrl { get; set; }
        /// <summary>
        /// 菜单序号
        /// </summary>
        public int MenuSortNo { get; set; }
        public int HideInMenu { get; set; }
        
        /// <summary>
        /// 图标
        /// </summary>
        public string IconUrl { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int DataState { get; set; }

    }

    /// <summary>
    /// 按钮输入实体
    /// </summary>
    public class MenuButtonInput
    {
        public string  Id { get; set; }
        /// <summary>
        /// 按钮名称
        /// </summary>
        public string  ButtonName { get; set; }
        /// <summary>
        /// 按钮编码
        /// </summary>
        public string  ButtonCode { get; set; }
        /// <summary>
        /// 菜单ID
        /// </summary>
        public string  MenuId { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int SortNo { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }


    }


    public class MenuButtonShearch
    {
        public string  Id { get; set; }
        public string   MenuId{ get; set; }

    }
}

