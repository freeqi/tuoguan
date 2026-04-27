using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Dto;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Extenstions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Controllers
{
    [EnableCors("any")] //启用跨域
    [Route("api/[controller]")]
    public class ConsumableController : Controller
    {
        private readonly ConsumableManager _consumableManager;

        public ConsumableController(ConsumableManager consumableManager)
        {
            _consumableManager = consumableManager;
        }

        /// <summary>
        /// 获取耗材列表
        /// </summary>
        [HttpGet("list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<ConsumableOutput[]>> GetConsumablesAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _consumableManager.GetConsumablesAsync();
                return new ServiceMessage<ConsumableOutput[]>(result.ToArray());
            });
        }

        /// <summary>
        /// 根据ID获取耗材
        /// </summary>
        [HttpGet("detail")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<ConsumableOutput>> GetConsumableByIdAsync(string id)
        {
            return Task.Run(async () =>
            {
                var result = await _consumableManager.GetConsumableByIdAsync(id);
                return new ServiceMessage<ConsumableOutput>(result);
            });
        }

        /// <summary>
        /// 添加耗材
        /// </summary>
        [HttpPost("add")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> AddConsumableAsync([FromBody] ConsumableInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _consumableManager.AddConsumableAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 更新耗材
        /// </summary>
        [HttpPost("update")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> UpdateConsumableAsync([FromBody] ConsumableInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _consumableManager.UpdateConsumableAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 删除耗材
        /// </summary>
        [HttpPost("delete")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> DeleteConsumableAsync(string id)
        {
            return Task.Run(async () =>
            {
                var result = await _consumableManager.DeleteConsumableAsync(id);
                return new ServiceMessage<bool>(result);
            });
        }
    }
}
