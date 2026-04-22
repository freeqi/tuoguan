using Castle.Core.Internal;
using CDGService.Data.Enums;
using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Dto;
using CDGService.WebAPI.Extenstions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Controllers
{
    [EnableCors("any")] //启用跨域
    [Route("api/[controller]")]
    public class CenterDialysisController : Controller
    {


        private readonly CenterDialysisManger _centerDialysisManger;

        public CenterDialysisController(CenterDialysisManger centerDialysisManger)
        {


            _centerDialysisManger = centerDialysisManger;
        }
        /// <summary>
        ///  获取透析中心数据列表
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input">查询透析中心条件参数</param>
        /// <returns></returns>
        [HttpPost("DialysisList")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<CenterDialysisOutPut[]>> GetDialysisQueryableAsync([FromBody]DialysisSearchInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _centerDialysisManger.GetDialysisQueryableAsync(input);
                return new ServiceMessage<CenterDialysisOutPut[]>(result.Result, result.Count);
            });

        }

        //
        /// <summary>
        ///  获取透析中心数据下拉列表
        /// 把登录操作获取的token，放到header里，key="token"        
        /// </summary>
        /// <returns></returns>
        [HttpGet("DialysisDropList")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<CenterListOutPut[]>> GetCenterListOutPuts()
        {
            return Task.Run(async () =>
            {
                var result = await _centerDialysisManger.GetCenterListOutPuts();
                return new ServiceMessage<CenterListOutPut[]>(result);
            });

        }
        /// <summary>
        ///  获取透析中心地图标记列表 
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>        
        /// <returns></returns>
        [HttpPost("DialysisMapList")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<DialysisMapOutPut[]>> GetDialysisMapAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _centerDialysisManger.GetDialysisQueryableAsync();
                return new ServiceMessage<DialysisMapOutPut[]>(result);
            });

        }


        /// <summary>
        ///  根据ID获取透析中心详细信息   
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="id">透析中心ID</param>
        /// <returns></returns>
        [HttpPost("DialysisDetails/{id}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<CenterDialysisOutPut[]>> GetDialysisByIdAsync([FromRoute] string id)
        {
            return Task.Run(async () =>
            {
                var result = await _centerDialysisManger.GetDialysisByiIdAsync(id);

                return new ServiceMessage<CenterDialysisOutPut[]>(result);
            });

        }




        /// <summary>
        /// 创建或修改透析中心
        /// 新增/修改血透中心
        ///  Id为0或null.视为新增
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input">透析中心信息参数</param>
        /// <returns></returns>
        [HttpPost("CreateUpdate")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CreateUpdateDialysisAsync([FromBody]CenterDialysisInput input)
        {
            return Task.Run(async () =>
            {

                var result = await _centerDialysisManger.CreateUpdateDialysisAsync(input);
                return new ServiceMessage<bool>(result);

            });
        }

        /// <summary>
        /// 删除透析中心。
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpGet("del/{id}")]
        [CheckLogin]
        [ServiceMessageTryCatch]     
        public virtual Task<ServiceMessage<bool>> DeleteMountingAsync([FromRoute] string id)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var result = await _centerDialysisManger.DeleteDialysisAsync(id);

                    return new ServiceMessage<bool>(result);
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
            });
        }


        /// <summary>
        /// 根据地区统计透析中心数量
        /// </summary> 
        /// <returns></returns>
        [HttpGet("StatisticalByCity")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<DialysisStatisticalByCityOutPut[]>> DialysisStatisticalByCityAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _centerDialysisManger.DialysisStatisticalByCityAsync();
                return new ServiceMessage<DialysisStatisticalByCityOutPut[]>(result);
            });
        }
        //DialysisStatisticalByYearAsync
        /// <summary>
        /// 根据运行时间统计透析中心数量
        /// </summary> 
        /// <returns></returns>
        [HttpGet("DialysisStatisticalByYear")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<DialysisStatisticalByYearOutPut[]>> DialysisStatisticalByYearAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _centerDialysisManger.DialysisStatisticalByYearAsync();
                return new ServiceMessage<DialysisStatisticalByYearOutPut[]>(result);
            });
        }


        /// <summary>
        /// 统计机构、职工、患者、机器数量
        /// </summary> 
        /// <returns></returns>
        [HttpGet("AllTotal")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<AllTotal>> GetTotalAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _centerDialysisManger.GetTotalAsync();
                return new ServiceMessage<AllTotal>(result);
            });
        }


    }
}
