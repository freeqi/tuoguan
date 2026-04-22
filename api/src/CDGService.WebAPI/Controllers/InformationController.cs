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
    public class InformationController : Controller
    {
        private readonly InformationManager _informationManager;
        public InformationController(InformationManager informationManager)
        {
            _informationManager = informationManager;
        }
        //     public Task<InformationReceiveOutPut[]> GetInformationAsync(int selType)

        /// <summary>
        /// 公告发布接收人
        /// 把登录操作获取的token，放到header里，key="token"
        /// selType 1按机构 2 按职务 
        /// </summary>
        /// <returns> </returns>
        [HttpGet("Information/Userlist/{selType}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<InformationReceiveOutPut[]>> GetInformationAsync([FromRoute]int selType)
        {
            return Task.Run(async () =>
            {
                var result = await _informationManager.GetInformationAsync(selType);
                return new ServiceMessage<InformationReceiveOutPut[]>(result);
            });
        }

        // public Task<bool> SendMesgAsync(InformationInput input)
        /// <summary>
        /// 新增公告发布
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Information/Add")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> SendMesgAsync([FromBody]InformationInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _informationManager.SendMesgAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }

        //   public Task<bool> MesgAudit(string InfoId,int State)

        [HttpGet("Information/MesgAudit/{InfoId}/{State}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> MesgAuditAsync([FromRoute]string InfoId, int State)
        {
            return Task.Run(async () =>
            {
                var result = await _informationManager.MesgAudit(InfoId, State);
                return new ServiceMessage<bool>(result);
            });
        }
        // public Task<bool> MesgSendAsync(string InfoId)

        [HttpGet("Information/MesgSend/{InfoId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> MesgSendAsync([FromRoute]string InfoId)
        {
            return Task.Run(async () =>
            {
                var result = await _informationManager.MesgSendAsync(InfoId);
                return new ServiceMessage<bool>(result);
            });
        }

        [HttpPost("Information/MesgSends")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> MesgSendsAsync([FromBody]string[] InfoIds)
        {
            return Task.Run(async () =>
            {
                var result = await _informationManager.MesgSendsAsync(InfoIds);
                return new ServiceMessage<bool>(result);
            });
        }
        // public Task<bool> InventoryToSendMesgAsync(List<InventoryInformationInput> inputs)
        /// <summary>
        /// 库存处批量发布调价公告
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Information/BatchAdd")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> InventoryToSendMesgAsync([FromBody]InventoryInformationInputs input)
        {
            return Task.Run(async () =>
            {
                var result = await _informationManager.InventoryToSendMesgAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }
        // public Task<bool> CloseMsgAsync(string id)

        /// <summary>
        /// 关闭公告
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Information/close/{id}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CloseMsgAsync([FromRoute]string id)
        {
            return Task.Run(async () =>
            {
                var result = await _informationManager.CloseMsgAsync(id);
                return new ServiceMessage<bool>(result);
            });
        }
        //     public Task<PageData<InformationOutPut[]>> GetMesgAsync(InformationQueryInput input)

        /// <summary>
        ///公告列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Information/List")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<InformationOutPut[]>> GetMesgAsync([FromBody]InformationQueryInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _informationManager.GetMesgAsync(input);
                return new ServiceMessage<InformationOutPut[]>(result.Result, result.Count);
            });
        }
        //    public Task<PageData<FeedbackOutPut[]>> GetFeedbackMesgAsync(FeedbackQueryInput input)
        /// <summary>
        ///反馈列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Feedback/List")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<FeedbackOutPut[]>> GetFeedbackMesgAsync([FromBody]FeedbackQueryInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _informationManager.GetFeedbackMesgAsync(input);
                return new ServiceMessage<FeedbackOutPut[]>(result.Result, result.Count);
            });
        }
        //  public Task<bool> BackAddAsync(BackReplyInput input)


        /// <summary>
        ///反馈信息回复
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("FeedbackReply/Add")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> GetAddFeedbackMesgAsync([FromBody]BackReplyInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _informationManager.BackAddAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }

        //   public Task<FeedbackReplysOutPut> BackAddAsync(string input)

        [HttpPost("GetBackDetailAsync/List/{input}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<FeedbackReplysOutPut>> GetBackDetailAsync([FromRoute]string input)
        {
            return Task.Run(async () =>
            {
                var result = await _informationManager.GetBackDetailAsync(input);
                return new ServiceMessage<FeedbackReplysOutPut>(result);
            });
        }
        //        public Task<bool> BackUpdaeStateAsync(string Id)
        [HttpPost("BackUpdaeState/{Id}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> BackUpdaeStateAsync([FromRoute]string Id)
        {
            return Task.Run(async () =>
            {
                var result = await _informationManager.BackUpdaeStateAsync(Id);
                return new ServiceMessage<bool>(result);
            });
        }
        ///// <summary>
        /////我发布的公告列表
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //[HttpPost("Information/MyList")]
        //[CheckLogin]
        //[ServiceMessageTryCatch]
        //public virtual Task<ServiceMessage<InformationOutPut[]>> GetMyMesgAsync([FromBody]InformationQueryInput input)
        //{
        //    return Task.Run(async () =>
        //    {
        //        var result = await _informationManager.GetMesgAsync(input,true);
        //        return new ServiceMessage<InformationOutPut[]>(result.Result, result.Count);
        //    });
        //}
    }
}
