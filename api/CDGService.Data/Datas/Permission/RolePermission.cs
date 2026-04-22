using System;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 角色权限实体类
    /// </summary>
    public class RolePermission
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 按钮code
        /// </summary>
        public string  MenuButtonCode { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public string  Roleld { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string  Founder { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime  FounderDate { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string  Modifier { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime  ModifierDate { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }


    }
}
