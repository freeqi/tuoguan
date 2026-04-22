using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 角色用户实体类
    /// </summary>
    public class RoleUser
    {
        public string Id { get; set; }

        public string  UserId { get; set; }

        /// <summary>
        /// 用户对象
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public string  RoleId { get; set; }

        /// <summary>
        /// 角色对象
        /// </summary>
        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; }


    }
}
