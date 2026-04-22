
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
    /// 结果质控
    /// </summary>     
    [EnableCors("any")] //启用跨域
    [Route("api/[controller]")]
    public class ResultControlController : Controller
    {
        private readonly ResultControlManager _resultControlManager;

        /// <summary>
        /// 结果质控     
        /// </summary>
        /// <param name="qualityControlManager"></param>
        public ResultControlController(ResultControlManager resultControlManager)
        {
            _resultControlManager = resultControlManager;
        }
        //QualityHypertensionChartQueryableAsync

        /// <summary>
        ///  高血压质控率 - 图形     
        /// </summary>       
        /// <returns></returns>
        [HttpPost("QualityHypertension/Chart")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<LineOutPut>> QualityHypertensionChartQueryableAsync([FromBody] QualityControlQueryInPut inPut)
        {
            return Task.Run(async () =>
            {

                var result = await _resultControlManager.QualityHypertensionChartQueryableAsync(inPut);

                return new ServiceMessage<LineOutPut>(result);
            });
        }
        /// <summary>
        ///  高血压质控率 - 表格
        /// </summary>       
        /// <returns></returns>
        [HttpPost("QualityHypertension/Data")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<QualityHypertensionDataOutPut[]>> QualityHypertensionDataQueryableAsync([FromBody]QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {

                var result = await _resultControlManager.QualityHypertensionDataQueryableAsync(InPut);

                return new ServiceMessage<QualityHypertensionDataOutPut[]>(result.Result,result.Count);
            });
        }

        //  public Task<LineOutPut> HemoglobinChartQueryableAsync(QualityControlQueryInPut InPut)
        /// <summary>
        ///  肾性贫血控制率 - 图形     
        /// </summary>       
        /// <returns></returns>
        [HttpPost("Hemoglobin/Chart")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<LineOutPut>> HemoglobinChartQueryableAsync([FromBody] QualityControlQueryInPut inPut)
        {
            return Task.Run(async () =>
            {

                var result = await _resultControlManager.HemoglobinChartQueryableAsync(inPut);

                return new ServiceMessage<LineOutPut>(result);
            });
        }

        // public Task<PageData<HemoglobinDataOutPut[]>> HemoglobinDataQueryableAsync(QualityControlQueryInPut InPut)

        /// <summary>
        ///  肾性贫血控制率 - 表格
        /// </summary>       
        /// <returns></returns>
        [HttpPost("Hemoglobin/Data")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<HemoglobinDataOutPut[]>> HemoglobinDataQueryableAsync([FromBody]QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {

                var result = await _resultControlManager.HemoglobinDataQueryableAsync(InPut);

                return new ServiceMessage<HemoglobinDataOutPut[]>(result.Result, result.Count);
            });
        }

        /// <summary>
        /// 血磷控制率-- 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        [HttpPost("P/Chart")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<LineOutPut>> GetPChartQueryableAsync([FromBody]QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {

                var result = await _resultControlManager.GetPChartQueryableAsync(InPut);
                return new ServiceMessage<LineOutPut>(result);
            });
        }
        /// <summary>
        /// 血磷控制率-- 表格
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("P/Data")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<QualityControlDataOutPut[]>> GetPDataQueryableAsync([FromBody] QualityControlQueryInPut inPut)
        {
            return Task.Run(async () =>
            {

                var result = await _resultControlManager.GetPDataQueryableAsync(inPut);
                return new ServiceMessage<QualityControlDataOutPut[]>(result.Result, result.Count);
            });
        }

        /// <summary>
        /// 血钙控制率-- 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        [HttpPost("Ca/Chart")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<LineOutPut>> GetCaChartQueryableAsync([FromBody]QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {

                var result = await _resultControlManager.GetCaChartQueryableAsync(InPut);
                return new ServiceMessage<LineOutPut>(result);
            });
        }
        /// <summary>
        /// 血钙控制率-- 表格
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("Ca/Data")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<QualityControlDataOutPut[]>> GetCaDataQueryableAsync([FromBody] QualityControlQueryInPut inPut)
        {
            return Task.Run(async () =>
            {

                var result = await _resultControlManager.GetCaDataQueryableAsync(inPut);
                return new ServiceMessage<QualityControlDataOutPut[]>(result.Result, result.Count);
            });
        }


        /// <summary>
        /// 全段甲状旁腺素（IPTH）控制率-- 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        [HttpPost("IPTH/Chart")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<LineOutPut>> GetIPTHChartQueryableAsync([FromBody]QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {

                var result = await _resultControlManager.GetIPTHChartQueryableAsync(InPut);
                return new ServiceMessage<LineOutPut>(result);
            });
        }
        /// <summary>
        /// 全段甲状旁腺素（IPTH）控制率-- 表格
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("IPTH/Data")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<QualityControlDataOutPut[]>> GetIPTHDataQueryableAsync([FromBody] QualityControlQueryInPut inPut)
        {
            return Task.Run(async () =>
            {

                var result = await _resultControlManager.GetIPTHDataQueryableAsync(inPut);
                return new ServiceMessage<QualityControlDataOutPut[]>(result.Result, result.Count);
            });
        }



        /// <summary>
        /// 血清白蛋白控制率 -- 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        [HttpPost("Serum/Chart")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<LineOutPut>> GetSerumChartQueryableAsync([FromBody]QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {

                var result = await _resultControlManager.GetSerumChartQueryableAsync(InPut);
                return new ServiceMessage<LineOutPut>(result);
            });
        }
        /// <summary>
        /// 血清白蛋白控制率-- 表格
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("Serum/Data")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<QualityControlDataOutPut[]>> GetSerumDataQueryableAsync([FromBody] QualityControlQueryInPut inPut)
        {
            return Task.Run(async () =>
            {

                var result = await _resultControlManager.GetSerumDataQueryableAsync(inPut);
                return new ServiceMessage<QualityControlDataOutPut[]>(result.Result, result.Count);
            });
        }

        /// <summary>
        /// Kt/V和URR控制率 -- 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        [HttpPost("URR/Chart")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<LineOutPut>> GetKTVChartQueryableAsync([FromBody]QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {

                var result = await _resultControlManager.GetKTVChartQueryableAsync(InPut);
                return new ServiceMessage<LineOutPut>(result);
            });
        }
        /// <summary>
        /// Kt/V和URR控制率- 表格
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("URR/Data")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<QualityControlDataOutPut[]>> GetKTVDataQueryableAsync([FromBody] QualityControlQueryInPut inPut)
        {
            return Task.Run(async () =>
            {

                var result = await _resultControlManager.GetKTVDataQueryableAsync(inPut);
                return new ServiceMessage<QualityControlDataOutPut[]>(result.Result, result.Count);
            });
        }

        /// <summary>
        /// 透析期间体重增长控制率 -- 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        [HttpPost("WeightGain/Chart")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<LineOutPut>> GetWeightGainChartQueryableAsync([FromBody]QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {

                var result = await _resultControlManager.GetWeightGainChartQueryableAsync(InPut);
                return new ServiceMessage<LineOutPut>(result);
            });
        }
        /// <summary>
        ///透析期间体重增长控制率- 表格
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("WeightGain/Data")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<QualityControlDataOutPut[]>> GetWeightGainDataQueryableAsync([FromBody] QualityControlQueryInPut inPut)
        {
            return Task.Run(async () =>
            {

                var result = await _resultControlManager.GetWeightGainDataQueryableAsync(inPut);
                return new ServiceMessage<QualityControlDataOutPut[]>(result.Result, result.Count);
            });
        }




        /// <summary>
        /// 乙型肝炎和丙型肝炎的发病率 -- 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        [HttpPost("HepatitisB/Chart")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<LineOutPut>> GetHepatitisBChartQueryableAsync([FromBody]QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {

                var result = await _resultControlManager.GetHepatitisBChartQueryableAsync(InPut);
                return new ServiceMessage<LineOutPut>(result);
            });
        }
        /// <summary>
        ///乙型肝炎和丙型肝炎的发病率- 表格
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("HepatitisB/Data")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<QualityControlDataOutPut[]>> GetHepatitisBDataQueryableAsync([FromBody] QualityControlQueryInPut inPut)
        {
            return Task.Run(async () =>
            {

                var result = await _resultControlManager.GetHepatitisBDataQueryableAsync(inPut);
                return new ServiceMessage<QualityControlDataOutPut[]>(result.Result, result.Count);
            });
        }
    }
}
