
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
    /// <summary>   
    ///质控   
    /// </summary>
    [EnableCors("any")] //启用跨域
    [Route("api/[controller]")]
    public class QualityControlController : Controller
    {
        private readonly QualityControlManager _qualityControlManager;
        /// <summary>
        /// 质控     
        /// </summary>
        /// <param name="qualityControlManager"></param>

        public QualityControlController(QualityControlManager qualityControlManager)
        {
            _qualityControlManager = qualityControlManager;

        }

        /// <summary>
        ///  血常规定时检验完成率统计-表格         
        /// </summary>       
        /// <returns></returns>
        [HttpPost("RoutineBloodRecord/Data")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<QualityControlDataOutPut[]>> GetEmployeeQueryableAsync([FromBody] QualityControlQueryInPuts inPut)
        {
            return Task.Run(async () =>
            {

                var result = await _qualityControlManager.GetQualityControlDataQueryableAsync(inPut);
              
                return new ServiceMessage<QualityControlDataOutPut[]>(result.Result, result.Count);
            });
        }

        //
        /// <summary>
        ///  血常规定时检验完成率统计-图型             
        /// </summary>       
        /// <returns></returns>
        [HttpPost("RoutineBloodRecord/Chart")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<LineOutPut>> GetQualityControlChartQueryableAsync([FromBody] QualityControlQueryInPuts inPut)
        {
            return Task.Run(async () =>
            {

                var result = await _qualityControlManager.GetQualityControlChartQueryableAsync(inPut);
                return new ServiceMessage<LineOutPut>(result);
            });
        }

        //GetBloodBiochemicalChartQueryableAsync

        //
        /// <summary>
        ///  血液生化定时检验完成率统计-图型             
        /// </summary>       
        /// <returns></returns>
        [HttpPost("BloodBiochemical/Chart")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<LineOutPut>> GetBloodBiochemicalChartQueryableAsync([FromBody] QualityControlQueryInPuts inPut)
        {
            return Task.Run(async () =>
            {

                var result = await _qualityControlManager.GetBloodBiochemicalChartQueryableAsync(inPut);
                return new ServiceMessage<LineOutPut>(result);
            });
        }

        /*
          /// <summary>
        /// MHD患者的血液生化定时检验完成率  -- 表格
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<PageData<BloodBiochemicalDataOutPut[]>> GetBloodBiochemicalDataQueryableAsync(QualityControlQueryInPuts InPut)
         */
        //
        /// <summary>
        ///  血液生化定时检验完成率统计-表格           
        /// </summary>       
        /// <returns></returns>
        [HttpPost("BloodBiochemical/Data")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<BloodBiochemicalDataOutPut[]>> GetBloodBiochemicalDataQueryableAsync([FromBody] QualityControlQueryInPuts inPut)
        {
            return Task.Run(async () =>
            {

                var result = await _qualityControlManager.GetBloodBiochemicalDataQueryableAsync(inPut);
                return new ServiceMessage<BloodBiochemicalDataOutPut[]>(result.Result, result.Count);
            });
        }

        //
        /// <summary>
        /// 全段甲状旁腺素（IPTH）定时检验完成率-- 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        [HttpPost("IPTH/Chart")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<LineOutPut>> GetIPTHChartQueryableAsync([FromBody]QualityControlQueryInPuts InPut)
        {
            return Task.Run(async () =>
            {

                var result = await _qualityControlManager.GetIPTHChartQueryableAsync(InPut);
                return new ServiceMessage<LineOutPut>(result);
            });
        }
        /// <summary>
        /// 全段甲状旁腺素（IPTH）定时检验完成率-- 表格
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("IPTH/Data")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<QualityControlDataOutPut[]>> GetIPTHDataQueryableAsync([FromBody] QualityControlQueryInPuts inPut)
        {
            return Task.Run(async () =>
            {

                var result = await _qualityControlManager.GetIPTHDataQueryableAsync(inPut);
                return new ServiceMessage<QualityControlDataOutPut[]>(result.Result, result.Count);
            });
        }


        /// <summary>
        /// 尿素清除指数（KT/V）、尿素下降率（URR）记录完成率-- 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        [HttpPost("URR/Chart")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<LineOutPut>> GetURRChartQueryableAsync([FromBody]QualityControlQueryInPuts InPut)
        {
            return Task.Run(async () =>
            {

                var result = await _qualityControlManager.GetURRChartQueryableAsync(InPut);
                return new ServiceMessage<LineOutPut>(result);
            });
        }
        /// <summary>
        /// 尿素清除指数（KT/V）、尿素下降率（URR）记录完成率-- 表格
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("URR/Data")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<QualityControlDataOutPut[]>> GetURRDataQueryableAsync([FromBody] QualityControlQueryInPuts inPut)
        {
            return Task.Run(async () =>
            {

                var result = await _qualityControlManager.GetURRDataQueryableAsync(inPut);
                return new ServiceMessage<QualityControlDataOutPut[]>(result.Result, result.Count);
            });
        }

        /// <summary>
        /// MHD患者的血清前白蛋白定时检验完成率-- 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        [HttpPost("PA/Chart")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<LineOutPut>> GetPAChartQueryableAsync([FromBody]QualityControlQueryInPuts InPut)
        {
            return Task.Run(async () =>
            {

                var result = await _qualityControlManager.GetPAChartQueryableAsync(InPut);
                return new ServiceMessage<LineOutPut>(result);
            });
        }
        /// <summary>
        /// MHD患者的血清前白蛋白定时检验完成率-- 表格
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("PA/Data")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<QualityControlDataOutPut[]>> GetPADataQueryableAsync([FromBody] QualityControlQueryInPuts inPut)
        {
            return Task.Run(async () =>
            {

                var result = await _qualityControlManager.GetPADataQueryableAsync(inPut);
                return new ServiceMessage<QualityControlDataOutPut[]>(result.Result, result.Count);
            });
        }
        /// <summary>
        /// MHD患者的C反映蛋白（CRP）定时检验完成率-- 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        [HttpPost("CRP/Chart")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<LineOutPut>> GetCRPChartQueryableAsync([FromBody]QualityControlQueryInPuts InPut)
        {
            return Task.Run(async () =>
            {

                var result = await _qualityControlManager.GetCRPChartQueryableAsync(InPut);
                return new ServiceMessage<LineOutPut>(result);
            });
        }
        /// <summary>
        /// MHD患者的C反映蛋白（CRP）定时检验完成率-- 表格
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("CRP/Data")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<QualityControlDataOutPut[]>> GetCRPDataQueryableAsync([FromBody] QualityControlQueryInPuts inPut)
        {
            return Task.Run(async () =>
            {

                var result = await _qualityControlManager.GetCRPDataQueryableAsync(inPut);
                return new ServiceMessage<QualityControlDataOutPut[]>(result.Result, result.Count);
            });
        }


        /// <summary>
        /// MHD患者的血清铁蛋白、转铁蛋白饱和度定时检验完成率-- 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        [HttpPost("Ferritin/Chart")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<LineOutPut>> GetFerritinChartQueryableAsync([FromBody]QualityControlQueryInPuts InPut)
        {
            return Task.Run(async () =>
            {

                var result = await _qualityControlManager.GetFerritinChartQueryableAsync(InPut);
                return new ServiceMessage<LineOutPut>(result);
            });
        }
        /// <summary>
        /// MHD患者的血清铁蛋白、转铁蛋白饱和度定时检验完成率-- 表格
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("Ferritin/Data")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<QualityControlDataOutPut[]>> GetFerritinDataQueryableAsync([FromBody] QualityControlQueryInPuts inPut)
        {
            return Task.Run(async () =>
            {

                var result = await _qualityControlManager.GetFerritinDataQueryableAsync(inPut);
                return new ServiceMessage<QualityControlDataOutPut[]>(result.Result, result.Count);
            });
        }

        /// <summary>
        ///MHD患者的B2微球蛋白定时检验完成率	6月-- 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        [HttpPost("B2/Chart")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<LineOutPut>> GetB2ChartQueryableAsync([FromBody]QualityControlQueryInPuts InPut)
        {
            return Task.Run(async () =>
            {

                var result = await _qualityControlManager.GetB2ChartQueryableAsync(InPut);
                return new ServiceMessage<LineOutPut>(result);
            });
        }
        /// <summary>
        /// MHD患者的B2微球蛋白定时检验完成率	6月-- 表格
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("B2/Data")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<QualityControlDataOutPut[]>> GetB2DataQueryableAsync([FromBody] QualityControlQueryInPuts inPut)
        {
            return Task.Run(async () =>
            {

                var result = await _qualityControlManager.GetB2DataQueryableAsync(inPut);
                return new ServiceMessage<QualityControlDataOutPut[]>(result.Result, result.Count);
            });
        }

    }
}
