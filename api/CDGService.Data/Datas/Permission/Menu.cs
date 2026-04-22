using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 菜单
    /// </summary>
    public class Menu : ISoftDelete
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }
        /// <summary>
        /// 父级菜单
        /// </summary>
        public string ParentMenuCode { get; set; }
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
        /// 创建人
        /// </summary>
        public string Founder { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime FounderDate { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string Modifier { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifierDate { get; set; }
        /// <summary>
        /// 菜单等级 
        /// 1一级菜单、2二级菜单、3三级菜单、4四级菜单
        /// </summary>
        public int MenuType { get; set; }
        public bool IsDelete { get; set; }
    }
}
