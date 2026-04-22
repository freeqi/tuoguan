
using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Dto;
using CDGService.WebAPI.Extenstions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Controllers
{
    [EnableCors("any")] //启用跨域
    [Route("api/[controller]")]
    public class HospitalFeelingController : Controller
    {
        private readonly HospitalFeelingManager _hospitalFeelingManager;

        public HospitalFeelingController(HospitalFeelingManager hospitalFeelingManager)
        {


            _hospitalFeelingManager = hospitalFeelingManager;
        }

        #region 门诊日志

        /// <summary>
        ///  获取门诊日志数据列表
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input">查询门诊日志条件参数</param>
        /// <returns></returns>
        [HttpPost("OutpatientDetailsLog/List")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<OutpatientDetailsLogOutPut[]>> GetDialysisQueryableAsync([FromBody]OutpatientDetailsLogQueryInPut input)
        {
            return Task.Run(async () =>
            {
                var result = await _hospitalFeelingManager.GetOutpatientDetailsLogQueryableAsync(input);
                return new ServiceMessage<OutpatientDetailsLogOutPut[]>(result.Result, result.Count);
            });

        }


        #endregion


        #region  治疗室消毒合格率
        /// <summary>
        ///  治疗室消毒记录
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("DisinfectionRoom/List")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<DisinfectionRoomOutPut[]>> GetDisinfectionRoomQueryableAsync([FromBody]DisinfectionRoomQueryInPut input)
        {
            return Task.Run(async () =>
            {
                var result = await _hospitalFeelingManager.GetDisinfectionRoomQueryableAsync(input);
                return new ServiceMessage<DisinfectionRoomOutPut[]>(result.Result, result.Count);
            });

        }
        /// <summary>
        ///  水污染检测记录
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetInspectionWaterPollution/List")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<InspectionWaterPollutionOutPut[]>> GetInspectionWaterPollutionQueryableAsync([FromBody]InspectionWaterPollutionQueryInPut input)
        {
            return Task.Run(async () =>
            {
                var result = await _hospitalFeelingManager.GetInspectionWaterPollutionQueryableAsync(input);
                return new ServiceMessage<InspectionWaterPollutionOutPut[]>(result.Result, result.Count);
            });

        }

        //GetPatientInfectiousCheckQueryableAsync
        /// <summary>
        ///  新入患者检测记录-发病记录
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetPatientInfectiousCheck/List")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<PatientInfectiousCheckOutPut[]>> GetPatientInfectiousCheckQueryableAsync([FromBody]PatientInfectiousCheckQueryInPut input)
        {
            return Task.Run(async () =>
            {
                var result = await _hospitalFeelingManager.GetPatientInfectiousCheckQueryableAsync(input);
                return new ServiceMessage<PatientInfectiousCheckOutPut[]>(result.Result, result.Count);
            });

        }

        //

        /// <summary>
        ///  治疗室合格率统计
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("DisinfectionRoomStatistics/List")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EmplyeeByStaBisOutPut[]>> GetDisinfectionRoomStatisticsAsync([FromBody]DisinfectionRoomStatisticsQueryInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _hospitalFeelingManager.GetDisinfectionRoomStatisticsAsync(input);
                return new ServiceMessage<EmplyeeByStaBisOutPut[]>(result);
            });

        }

        //
        /// <summary>
        ///  按单月统计所有透析中心
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("DisinfectionRoomStatisticsByCenter/List/{input}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<LineOutPut>> GetDisinfectionRoomStatisticsByCenterAsync([FromRoute]DateTime input)
        {
            return Task.Run(async () =>
            {
                var result = await _hospitalFeelingManager.GetDisinfectionRoomStatisticsByCenterAsync(input);
                return new ServiceMessage<LineOutPut>(result);
            });

        }

        //GetPatientInfectiousChecksStatisticsByCenterAsync
        /// <summary>
        ///  新入患者人数及发病次数!图形
        ///  参数不能为空
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("PatientInfectiousChecksStatisticsByCenter/List")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<LineOutPut>> GetPatientInfectiousChecksStatisticsByCenterAsync([FromBody]PatientInfectiousCheckQueryInPut input)
        {
            return Task.Run(async () =>
            {
                var result = await _hospitalFeelingManager.GetPatientInfectiousChecksStatisticsByCenterAsync(input);
                return new ServiceMessage<LineOutPut>(result);
            });

        }


        // public Task<PageData<PatientInfectiousCheckOutPut[]>> GetInfectiousDiseasesRecordStoreCheckQueryableAsync(PatientInfectiousCheckQueryInPut input = null)

        /// <summary>
        ///  新入患者检测记录-完成情况 数据
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("InfectiousDiseasesRecordCheck/List")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<PatientInfectiousCheckOutPut[]>> GetInfectiousDiseasesRecordStoreCheckQueryableAsync([FromBody]PatientInfectiousCheckQueryInPut input)
        {
            return Task.Run(async () =>
            {
                var result = await _hospitalFeelingManager.GetInfectiousDiseasesRecordStoreCheckQueryableAsync(input);
                return new ServiceMessage<PatientInfectiousCheckOutPut[]>(result.Result, result.Count);
            });

        }


        //GetPatientInfectiousChecksStatisticsByMonthAsync(DisinfectionRoomStatisticsQueryInput input = null)

        ///// <summary>
        /////  统计半年内新入患者人数及发病次数!
        /////  参数为空视为查询所有数据
        ///// 把登录操作获取的token，放到header里，key="token"
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //[HttpPost("GetPatientInfectiousChecksStatisticsByMonth/List")]
        //[CheckLogin]
        //[ServiceMessageTryCatch]
        //public virtual Task<ServiceMessage<LineOutPut>> GetPatientInfectiousChecksStatisticsByMonthAsync([FromBody] DisinfectionRoomStatisticsQueryInput input)
        //{
        //    return Task.Run(async () =>
        //    {
        //        var result = await _hospitalFeelingManager.GetPatientInfectiousChecksStatisticsByMonthAsync(input);
        //        return new ServiceMessage<LineOutPut>(result);
        //    });

        //}

        //
        //GetInspectionWaterPollutionsStatisticsByMonthAsync(DisinfectionRoomStatisticsQueryInput input = null)
        /// <summary>
        ///  统计半年内菌落数检验次数和合格次数
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("EndotoxinByMonth/List")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<LineOutPut>> GetInspectionWaterPollutionsStatisticsByMonthAsync([FromBody] DisinfectionRoomStatisticsQueryInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _hospitalFeelingManager.GetInspectionWaterPollutionsStatisticsByMonthAsync(input);
                return new ServiceMessage<LineOutPut>(result);
            });

        }
        /// <summary>
        ///  统计半年内内毒素检验次数和合格次数
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("WaterColonyCountByMonth/List")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<LineOutPut>> GetInspectionWaterPollutionsStatisticsByMonthsAsync([FromBody] DisinfectionRoomStatisticsQueryInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _hospitalFeelingManager.GetInspectionWaterPollutionsStatisticsByMonthAsync(input);
                return new ServiceMessage<LineOutPut>(result);
            });

        }

        /// <summary>
        ///  统计指定月份水菌落数检验次数和合格次数
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("WaterColonyCountByCenter/List/{input}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<LineOutPut>> GetInspectionWaterPollutionsStatisticsByCenterAsync([FromRoute] DateTime input)
        {
            return Task.Run(async () =>
            {
                var result = await _hospitalFeelingManager.GetInspectionWaterPollutionsStatisticsByMonthAsync(input);
                return new ServiceMessage<LineOutPut>(result);
            });

        }

        /// <summary>
        ///  统计指定月份内毒素检验次数和合格次数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("EndotoxinCountByCenter/List/{input}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<LineOutPut>> GetInspectionEndotoxinStatisticsByCenterAsync([FromRoute] DateTime input)
        {
            return Task.Run(async () =>
            {
                var result = await _hospitalFeelingManager.GetInspectionWaterPollutionsStatisticsByMonthAsync(input);
                return new ServiceMessage<LineOutPut>(result);
            });

        }

        /// <summary>
        ///  统计半年内患者检测完成率 
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("PatientInfectiousChecksOKStatisticsByMonth/List")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<LineOutPut>> GetPatientInfectiousChecksOKStatisticsByMonthAsync([FromBody] DisinfectionRoomStatisticsQueryInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _hospitalFeelingManager.GetPatientInfectiousChecksOKStatisticsByMonthAsync(input);
                return new ServiceMessage<LineOutPut>(result);
            });

        }
        /// <summary>
        ///  统计患者检测完成率(图表)
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetInfectiousDiseasesRecordChartCheckQueryable/List/")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<LineOutPut>> GetInfectiousDiseasesRecordChartCheckQueryableAsync([FromBody] PatientInfectiousCheckQueryInPut input)
        {
            return Task.Run(async () =>
            {
                var result = await _hospitalFeelingManager.GetInfectiousDiseasesRecordChartCheckQueryableAsync(input);
                return new ServiceMessage<LineOutPut>(result);
            });

        }
        //GetInspectionWaterPollutionsStatisticsAsync  GetInspectionWaterPollutionQueryableAsync

        /// <summary>
        /// 内毒素或者标本检查图形
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="BioType">1细菌培养 2 内毒素</param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetInspectionWaterPollutionsStatisticsChart/List/{BioType}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<LineOutPut>> GetInspectionWaterPollutionsStatisticsAsync([FromRoute] int BioType, [FromBody] PatientInfectiousCheckQueryInPut input)
        {
            return Task.Run(async () =>
            {
                string sBioType = "内毒素";
                if (BioType == 1)
                    sBioType = "细菌培养";
                if(input !=null)
                input.BioType = sBioType; 
                var result = await _hospitalFeelingManager.GetInspectionWaterPollutionsStatisticsAsync(input);
                return new ServiceMessage<LineOutPut>(result);
            });

        }


        /// <summary>
        ///  内毒素或者标本检查表格
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="BioType">1细菌培养 2 内毒素</param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetInspectionWaterPollutionQueryableTable/List/{BioType}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<InspectionWaterPollutionOutPut[]>> GetInspectionWaterPollutionQueryableTableAsync([FromRoute] int BioType, [FromBody]PatientInfectiousCheckQueryInPut input)
        {
            return Task.Run(async () =>
            {
                string sBioType = "内毒素";
                if (BioType == 1)
                    sBioType = "细菌培养";
                if (input != null)
                    input.BioType = sBioType;
                var result = await _hospitalFeelingManager.GetInspectionWaterPollutionQueryableAsync(input);
                return new ServiceMessage<InspectionWaterPollutionOutPut[]>(result.Result, result.Count);
            });

        }

        #endregion


    }
}
