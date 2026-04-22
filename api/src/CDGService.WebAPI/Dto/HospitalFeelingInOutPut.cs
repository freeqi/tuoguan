using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{

    public class OutpatientDetailsLogQueryInPut
    {
        /// <summary>
        /// 中心ID
        /// </summary>
        public string CenterId { get; set; }

        /// <summary>
        /// 日志日期
        /// </summary>
        public DateTime? BeginTime { get; set; }

        public int PageSize { get; set; }
        public int PageNum { get; set; }


    }

    public class OutpatientDetailsLogOutPut
    {
        /// <summary>
        /// 就诊日期
        /// </summary>
        public string DiagnosisDate { get; set; }
        /// <summary>
        ///  发病时间
        /// </summary>
        public string MorbidityDate { get; set; }
        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PatientName { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// 职业
        /// </summary>
        public string Professional { get; set; }
        /// <summary>
        /// 现住详细地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 主要症状
        /// </summary>
        public string Symptoms { get; set; }
        /// <summary>
        /// 诊断
        /// </summary>
        public string Diagnosis { get; set; }
        /// <summary>
        /// 复诊
        /// </summary>
        public string SubsequentVisit { get; set; }
        /// <summary>
        /// 报告日
        /// </summary>
        public string ReportDate { get; set; }
        /// <summary>
        /// 报告人
        /// </summary>
        public string ReportUser { get; set; }
        /// <summary>
        /// 血压
        /// </summary>
        public string BloodPressure { get; set; }
        public string Remark { get; set; }

    }


    public class DisinfectionRoomOutPut
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 中心ID
        /// </summary>
        public string CenterId { get; set; }

        public string CenterName { get; set; }
        /// <summary>
        /// 分区ID
        /// </summary>
        public string PartitionId { get; set; }
        public string PartitionName { get; set; }
        /// <summary>
        /// 消毒时间
        /// </summary>
        public string DisinfectionTime { get; set; }
        /// <summary>
        /// 是否合格
        /// </summary>
        public string IsQualified { get; set; }
        /// <summary>
        /// 消毒人
        /// </summary>
        public string DisinfectionUser { get; set; }
        /// <summary>
        /// 不合格说明
        /// </summary>
        public string UnqualifiedDescribe { get; set; }
        /// <summary>
        /// 不合格后续处理
        /// </summary>
        public string UnqualifiedDeal { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public int DataState { get; set; }
    }

    public class DisinfectionRoomQueryInPut
    {
        /// <summary>
        /// 中心ID
        /// </summary>
        public string CenterId { get; set; }
        /// <summary>
        /// 0不合格1合格2全部
        /// </summary>
        public int? IsQualified { get; set; }
        /// <summary>
        ///日期
        /// </summary>
        public DateTime? BeginTime { get; set; }
        /// <summary>
        ///日期
        /// </summary>
        public DateTime? EndTime { get; set; }

        public int PageSize { get; set; }
        public int PageNum { get; set; }
    }
    public class DisinfectionRoomStatisticsQueryInput
    {
        public string CenterId { get; set; }

        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
    }



    public class InspectionWaterPollutionQueryInPut
    {
        /// <summary>
        /// 中心ID
        /// </summary>
        public string CenterId { get; set; }

        /// <summary>
        ///日期
        /// </summary>
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 0不合格1合格2全部
        /// </summary>
        public int? IsQualified { get; set; }

        /// <summary>
        ///日期
        /// </summary>
        public DateTime? EndTime { get; set; }

        public int PageSize { get; set; }
        public int PageNum { get; set; }
    }
    public class InspectionWaterPollutionOutPut
    {

        public string Id { get; set; }
        public string CenterId { get; set; }

        public string CenterName { get; set; }
        /// <summary>
        /// 水菌落数检验结果
        /// </summary>
        public string WaterColonyCount { get; set; }
        /// <summary>
        /// 标本
        /// </summary>
        public string Specimen { get; set; }
        /// <summary>
        /// 内毒素检验结果
        /// </summary>
        public string Endotoxin { get; set; }
        /// <summary>
        /// 检验日期
        /// </summary>
        public string DisinfectionTime { get; set; }
        /// <summary>
        /// 是否合格
        /// </summary>
        public string IsQualified { get; set; }
        /// <summary>
        /// 消毒人
        /// </summary>
        public string DisinfectionUser { get; set; }
        /// <summary>
        /// 不合格说明
        /// </summary>
        public string UnqualifiedDescribe { get; set; }
        /// <summary>
        /// 不合格后续处理
        /// </summary>
        public string UnqualifiedDeal { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public int DataState { get; set; }
        //public string Founder { get; set; }
        //public DateTime? FounderDate { get; set; }
        //public string Modifier { get; set; }
        //public DateTime? ModifierDate { get; set; }
    }

    /// <summary>
    /// 新入患者传染病监测完成情况
    /// </summary>
    public class PatientInfectiousCheckOutPut
    {
        public string Id { get; set; }
        public string CenterId { get; set; }
        public string CenterName { get; set; }

        public string PatentId { get; set; }
        public string PatentName { get; set; }
        public string PatentSex { get; set; }

        /// <summary>
        /// 内毒素检验结果
        /// </summary>
        public string Endotoxin { get; set; }
        /// <summary>
        /// 检查日期
        /// </summary>
        public string DisinfectionTime { get; set; }
        /// <summary>
        /// 传染病类型
        /// </summary>
        public string InfectiousType { get; set; }
        /// <summary>
        /// 传染病类型
        /// </summary>
        public string InfectiousTypeName { get; set; }
        /// <summary>
        /// 检查是否完成
        /// </summary>
        public string IsQualified { get; set; }
        /// <summary>
        /// 治疗编号
        /// </summary>
        public string CureCode { get; set; }
        /// <summary>
        /// 发病时间
        /// </summary>
        public string MorbidityTime { get; set; }
        public string Remark { get; set; }
        public int DataState { get; set; }

    }

    public class PatientInfectiousCheckQueryInPut
    {
        /// <summary>
        /// 中心ID
        /// </summary>
        public string CenterId { get; set; }

        /// <summary>
        ///日期
        /// </summary>
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 0不合格1合格2全部
        /// </summary>
        public int? IsQualified { get; set; }
        /// <summary>
        /// 生化检测类型  1细菌培养   2内毒素
        /// </summary>
        public string BioType { get; set; }

        /// <summary>
        ///日期
        /// </summary>
        public DateTime? EndTime { get; set; }

        public int PageSize { get; set; }
        public int PageNum { get; set; }
    }


}
