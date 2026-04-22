using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    public class MenuOutPut
    { /// <summary>
      /// ID
      /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string title { get; set; }
        public bool expand { get; set; } = false;
        /// <summary>
        /// 父级菜单
        /// </summary>
        public string   ParentMenuCode { get; set; }
        /// <summary>
        /// 是否有链接
        /// </summary>
        public bool IsHasUrl { get; set; }
        public int HideInMenu { get; set; }
        /// <summary>
        /// 菜单链接
        /// </summary>
        public string MenuUrl { get; set; }
        /// <summary>
        /// 菜单序号
        /// </summary>
        public int MenuSortNo { get; set; }
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
        /// <summary>
        /// 菜单等级 
        /// 1一级菜单、2二级菜单、3三级菜单、4四级菜单
        /// </summary>
        public int MenuType { get; set; }

        public List<MenuOutPut> children { get; set; } = new List<MenuOutPut>();
    }


    public class MenuButtonOutPut
    {

        public string Id  { get; set; }
        /// <summary>
        /// 按钮名称
        /// </summary>
        public string ButtonName { get; set; }
        /// <summary>
        /// 按钮编码
        /// </summary>
        public string ButtonCode { get; set; }
        /// <summary>
        /// 菜单ID
        /// </summary>
        public string MenuId { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int SortNo { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }


    }
}
