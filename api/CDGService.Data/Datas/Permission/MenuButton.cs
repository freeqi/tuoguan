using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 菜单按钮
    /// </summary>
    public class MenuButton : ISoftDelete
    {

        public string Id { get; set; }
        /// <summary>
        /// 按钮名称
        /// </summary>
        public string   ButtonName { get; set; }
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
        ///创建人
        /// </summary>
        public string Founder { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime  FounderDate { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string Modifier { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime  ModifierDate { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }

    }
}
