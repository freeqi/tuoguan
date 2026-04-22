using System;
using System.ComponentModel.DataAnnotations;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 角色实体
    /// </summary>
    public class Role : ISoftDelete
    {
        public string Id { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required]
        public string RoleName { get; set; }
        /// <summary>
        /// 角色编码
        /// </summary>
        [Required]
        public string RoleCode { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        public string RoleAliasName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Required]
        public int DataState { get; set; }

        public bool IsAdjust { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>        
        public bool IsDelete { get; set; }
    }
}
