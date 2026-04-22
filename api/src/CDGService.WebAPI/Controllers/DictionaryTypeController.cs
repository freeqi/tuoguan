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
    public class DictionaryTypeController : Controller
    {


        private readonly DictionaryTypeManager _dictionaryTypeManager;

        public DictionaryTypeController(DictionaryTypeManager dictionaryTypeManager)
        {

            _dictionaryTypeManager = dictionaryTypeManager;
        }


        /// <summary>
        ///新增/修改字典类型      
        /// Id为0或null.视为新增
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input">字典类型信息参数</param>
        /// <returns></returns>
        [HttpPost("CreateUpdate")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CreateUpdateDictionaryTypeAsync([FromBody]DictionaryTypeInput input)
        {
            return Task.Run(async () =>
            {

                var result = await _dictionaryTypeManager.CreateUpdateDictionaryTypeAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 删除字典类型。
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
                    var result = await _dictionaryTypeManager.DelDictionaryTypeAsync(id);

                    return new ServiceMessage<bool>(result);
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
            });
        }

        /// <summary>
        ///  获取字典类型数据列表
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input">查询字典类型条件参数</param>
        /// <returns></returns>
        [HttpPost("DictionaryTypeList")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<DictionaryTypeOutPut[]>> GetDialysisQueryableAsync([FromBody]DictionaryTypeSearchInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _dictionaryTypeManager.GetDictionaryTypesQueryableAsync(input);
                return new ServiceMessage<DictionaryTypeOutPut[]>(result);
            });

        }
    }
}
