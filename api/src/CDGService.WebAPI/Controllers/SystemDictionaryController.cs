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
    public class SystemDictionaryController : Controller
    {


        private readonly SystemDictionaryManger _systemDictionaryManger;

        public SystemDictionaryController(SystemDictionaryManger systemDictionaryManger)
        {
            _systemDictionaryManger = systemDictionaryManger;
        }

        /// <summary>
        ///新增/修改字典     
        /// Id为0或null.视为新增
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input">字典信息参数</param>
        /// <returns></returns>
        [HttpPost("CreateUpdate")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CreateUpdateDictionaryAsync([FromBody]SystemDictionaryInput input)
        {
            return Task.Run(async () =>
            {
                 
                var result = await _systemDictionaryManger.CreateUpdateDictionaryAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }
        /// <summary>
        ///  获取字典数据列表
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input">查询字典类型条件参数</param>
        /// <returns></returns>
        [HttpPost("DictionaryList")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<SystemDictionaryOutPut[]>> GetDialysisQueryableAsync([FromBody]DictionarySearchInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _systemDictionaryManger.GetDictionaryQueryableAsync(input);
                return new ServiceMessage<SystemDictionaryOutPut[]>(result);
            }); 
        }

        /// <summary>
        /// 删除字典 
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpGet("del/{id}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> DeleteMountingAsync([FromRoute] string  id)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var result = await _systemDictionaryManger.DelDictionaryAsync(id);

                    return new ServiceMessage<bool>(result);
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
            });
        }
    }
}
