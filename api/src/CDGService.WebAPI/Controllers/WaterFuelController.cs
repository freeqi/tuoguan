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
    public class WaterFuelController : Controller
    {

        private readonly WaterFuelManager _waterFuelManager;
        public WaterFuelController(WaterFuelManager waterFuelManager)
        {
            _waterFuelManager = waterFuelManager;
        }
        //AddUpdateWaterFuel

        /// <summary>
        /// 添加修改
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("AddUpdateWaterFuel")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> AddUpdateWaterFuelAsync([FromBody]WaterFuelInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _waterFuelManager.AddUpdateWaterFuel(inPut);
                return new ServiceMessage<bool>(result);
            });
        }
        // public Task<bool> DelWaterFuel(string Id)
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DelWaterFuel/{id}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> DelWaterFuel([FromRoute]string id)
        {
            return Task.Run(async () =>
            {
                var result = await _waterFuelManager.DelWaterFuel(id);
                return new ServiceMessage<bool>(result);
            });
        }

        // public Task<PageData<WaterFuelOutPut[]>> GetWaterFuelListAsync(WaterFuelQueryInPut input)
        // public Task<bool> DelWaterFuel(string Id)
        /// <summary>
        ///数据列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("WaterFuelList")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<WaterFuelOutPut[]>> GetWaterFuelListAsync([FromBody]WaterFuelQueryInPut input)
        {
            return Task.Run(async () =>
            {
                var result = await _waterFuelManager.GetWaterFuelListAsync(input);
                return new ServiceMessage<WaterFuelOutPut[]>(result.Result, result.Count);
            });
        }

        // public Task<WaterFuelStatisOutPut[]> GetWaterFuelStatissAsync(WaterFuelQueryInPut input)

        /// <summary>
        ///统计
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("WaterFuelStatis")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<WaterFuelStatisOutPut[]>> GetWaterFuelStatissAsync([FromBody]WaterFuelQueryInPut input)
        {
            return Task.Run(async () =>
            {
                var result = await _waterFuelManager.GetWaterFuelStatissAsync(input);
                return new ServiceMessage<WaterFuelStatisOutPut[]>(result);
            });
        }

    }
}
