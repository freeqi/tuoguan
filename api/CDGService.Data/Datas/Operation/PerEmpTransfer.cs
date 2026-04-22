using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 员工预调记录
    /// </summary>
    public class PerEmpTransfer
    {
        public string Id { get; set; }

        /// <summary>
        /// 人员ID
        /// </summary>
        public string EmpId { get; set; }
        [ForeignKey(nameof(EmpId))]
        public virtual Employee employee { get; set; }
        /// <summary>
        /// 原部门
        /// </summary>
        public string OriginalDepId { get; set; }
        /// <summary>
        /// 原部门
        /// </summary>
        [ForeignKey(nameof(OriginalDepId))]
        public virtual SystemDictionary OriginalDep { get; set; }
        /// <summary>
        /// 原职位
        /// </summary>
        public string OriginalPositionId { get; set; }
        /// <summary>
        /// 原职位
        /// </summary>
        [ForeignKey(nameof(OriginalPositionId))]
        public virtual SystemDictionary OriginalPosition { get; set; }
        /// <summary>
        /// 原机构
        /// </summary>
        public string OriginalCenterId { get; set; }
        /// <summary>
        /// 原机构
        /// </summary>
        [ForeignKey(nameof(OriginalCenterId))]
        public virtual CenterDialysis OriginalCenter { get; set; }
        /// <summary>
        /// 调动部门
        /// </summary>
        public string PresentDepId { get; set; }
        /// <summary>
        /// 调动部门
        /// </summary>
        [ForeignKey(nameof(PresentDepId))]
        public virtual SystemDictionary depDic { get; set; }
        /// <summary>
        /// 调动职位
        /// </summary>
        public string PresentPositionId { get; set; }
        /// <summary>
        /// 调动职位
        /// </summary>
        [ForeignKey(nameof(PresentPositionId))]
        public virtual SystemDictionary PosDic { get; set; }
        /// <summary>
        /// 调动机构
        /// </summary>
        public string PresentCenterId { get; set; }
        /// <summary>
        /// 调动机构
        /// </summary>
        [ForeignKey(nameof(PresentCenterId))]
        public virtual CenterDialysis Center { get; set; }
        /// <summary>
        /// 预调动时间
        /// </summary>
        public DateTime TransferDate { get; set; }

        /// <summary>
        /// 调动状态 1有效 2 撤回（无效）3 删除
        /// </summary>
        public int DataState { get; set; }
        /// <summary>
        /// 是否已执行调动
        /// </summary>
        public bool IsTransfer { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Founder { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? FounderDate { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string Modifier { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifierDate { get; set; }
        /// <summary>
        /// 调回时间
        /// </summary>
        public DateTime BackDate { get; set; }
    }
}
