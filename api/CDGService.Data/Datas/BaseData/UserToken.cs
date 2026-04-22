using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDGService.Data.Datas
{
    public class UserToken
    {
        public string Id { get; set; }

        public string  UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        

        public string Token { get; set; }
         
        public DateTime? LastUpdateTime { get; set; }
         
        public DateTime? LogingTime { get; set; }
        /// <summary>
        /// 是否为中心端采集token
        /// </summary>
        public bool IsCollectToken { get; set; }

        public string remark { get; set; }
        public string CenterId { get; set; }

    }
}