
using CDGService.Data.Enums;
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
    public class MedicalIndexesController : Controller
    {

        private readonly MedicalIndexesManger _medicalIndexesManger;

        public MedicalIndexesController(MedicalIndexesManger medicalIndexesManger)
        {
            _medicalIndexesManger = medicalIndexesManger;
        }

        /// <summary>
        /// 月度指标统计单透析中心
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("ObserveIndicators")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MedicalIndexesMonthOutPut[]>> ObserveIndicatorsAsync([FromBody]MedicalIndexesMonthQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _medicalIndexesManger.ObserveIndicatorsAsync(inPut);
                return new ServiceMessage<MedicalIndexesMonthOutPut[]>(result);
            });
        }
        //   public Task<AllCenterMedicalIndexesMonthOutPut[]> ObserveIndicatorsAsync(ALLCenterMedicalIndexesMonthQueryInPut inPut)
        /// <summary>
        /// 月度指标-所有透析中心单项指标
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("ObserveIndicators/AllCenter")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<AllCenterMedicalIndexesMonthOutPut[]>> AllCenterObserveIndicatorsAsync([FromBody]ALLCenterMedicalIndexesMonthQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _medicalIndexesManger.ObserveIndicatorsAsync(inPut);
                return new ServiceMessage<AllCenterMedicalIndexesMonthOutPut[]>(result);
            });
        }
        //        public List<EnumberEntity> ObserveIndicatorsAsync(MedicalStatisticalType inPut)
        /// <summary>
        /// 月度指标列表
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("Observe/Indicators/{inPut}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<Data.EnumberEntity[]>> EnumberEntityAsync([FromRoute]MedicalStatisticalType inPut)
        {
            return Task.Run(async () =>
            {
               //  _medicalIndexesManger.AddData();
                var result = _medicalIndexesManger.EnumberEntityAsync(inPut);
                return new ServiceMessage<Data.EnumberEntity[]>(result.ToArray());
            });
        }


        //    public List<EnumberEntity> EnumberEntityYearAsync(MedicalStatisticalYearType inPut)
        /// <summary>
        /// 年度指标列表
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("Observe/IndicatorsYear/{inPut}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<Data.EnumberEntity[]>> EnumberEntityYearAsync([FromRoute]MedicalStatisticalYearType inPut)
        {
            return Task.Run(async () =>
            {
                //  _medicalIndexesManger.AddData();
                var result = _medicalIndexesManger.EnumberEntityYearAsync(inPut);
                return new ServiceMessage<Data.EnumberEntity[]>(result.ToArray());
            });
        }
        //public Task<MedicalIndexesYearOutPut[]> ObserveIndicatorsYearAsync(MedicalIndexesYearQueryInPut inPut)

        /// <summary>
        ///年度指标统计单透析中心
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("ObserveIndicatorsYear")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MedicalIndexesYearOutPut[]>> ObserveIndicatorsYearAsync([FromBody]MedicalIndexesYearQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _medicalIndexesManger.ObserveIndicatorsYearAsync(inPut);
                return new ServiceMessage<MedicalIndexesYearOutPut[]>(result);
            });
        }

        /// <summary>
        /// 年度观察指标:(所有透析中心单指标)
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("ObserveIndicatorsYear/AllCenter")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<AllCenterMedicalIndexesYearOutPut[]>> AllCenterObserveIndicatorsYearAsync([FromBody]ALLCenterMedicalIndexesYearQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _medicalIndexesManger.ObserveIndicatorsYearAsync(inPut);
                return new ServiceMessage<AllCenterMedicalIndexesYearOutPut[]>(result);
            });
        }

    }
}
