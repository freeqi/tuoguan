
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CDGService.Data.Helper;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Extenstions;
using CDGService.WebAPI.Dto;

namespace CDGService.WebAPI.Controllers
{
    /// <summary>
    /// 病案管理
    /// </summary>
    [EnableCors("any")] //启用跨域
    [Route("api/[controller]")]
    public class MedicalRcordController : Controller
    {
        private readonly MedicalRcordManager _medicalRcordManager;
        public MedicalRcordController(MedicalRcordManager medicalRcordManager)
        {
            _medicalRcordManager = medicalRcordManager;
        }

        /// <summary>
        /// 患者处方列表 
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MedicalRcord/Prescription")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<PrescriptionViewModel[]>> GetPrescriptionByPatientIdAsync([FromBody]PrescriptionInput inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _medicalRcordManager.GetPrescriptionByPatientIdAsync(inPut);
                return new ServiceMessage<PrescriptionViewModel[]>(result.Result, result.Count);
            });
        }

        /// <summary>
        /// 根据处方id查询处方明细
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MedicalRcord/PrescriptionDetail")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<PrescriptionDetailViewModel[]>> GetPrescriptionDetailByPrescriptionIdAsync([FromBody]PrescriptionInput inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _medicalRcordManager.GetPrescriptionDetailByPrescriptionIdAsync(inPut);
                return new ServiceMessage<PrescriptionDetailViewModel[]>(result);
            });
        }


        /// <summary>
        /// 收费清单
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MedicalRcord/ChargeList")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<FeesforListingModel[]>> GetChargeListByPatientIdAsync([FromBody]PrescriptionInput inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)  
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _medicalRcordManager.GetChargeListByPatientIdAsync(inPut);
                return new ServiceMessage<FeesforListingModel[]>(result.Result, result.Count);
            });
        }
        /// <summary>
        /// 根据结算id查询收费明细
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MedicalRcord/FeesforListingDetail")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<FeesforListingDetailModel[]>> GetAllFeesForListingDetailByIdAsync([FromBody]PrescriptionInput inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _medicalRcordManager.GetAllFeesForListingDetailByIdAsync(inPut);
                return new ServiceMessage<FeesforListingDetailModel[]>(result);
            });
        }

    }
}
