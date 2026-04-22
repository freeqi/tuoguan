using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 供应商-生产厂家
    /// </summary>
    public class Supplier:ISoftDelete
    {
        public string Id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        public string SysCode { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string SupCode { get; set; }
        /// <summary>
        /// 法人
        /// </summary>
        public string LegalPerson { get; set; }
        /// <summary>
        /// 主营业务
        /// </summary>
        public string MainBusiness { get; set; }
        /// <summary>
        /// 主营类别
        /// </summary>
        public string MainCategories { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string LinkMan { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }


        public string Image1 { get; set; }

        public string Image2 { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifierDate { get; set; }
        public bool IsDelete { get; set; }

        /// <summary>
        /// 是否为中心端自己添加
        /// </summary>
        public int? IsCenterAdd { get; set; }
        public string CenterId { get; set; }
    }
}
