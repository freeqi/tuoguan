using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Dto;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Controllers
{
    [EnableCors("any")] //启用跨域
    [Route("api/[controller]")]
    public class EquipmentRentalController : Controller
    {
        private readonly EquipmentRentalManager _equipmentRentalManager;

        public EquipmentRentalController(EquipmentRentalManager equipmentRentalManager)
        {
            _equipmentRentalManager = equipmentRentalManager;
        }

        /// <summary>
        /// 获取设备租用列表
        /// </summary>
        [HttpGet("list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EquipmentRentalOutput[]>> GetEquipmentRentalsAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentRentalManager.GetEquipmentRentalsAsync();
                return new ServiceMessage<EquipmentRentalOutput[]>(result.ToArray());
            });
        }

        /// <summary>
        /// 根据ID获取设备租用
        /// </summary>
        [HttpGet("detail")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EquipmentRentalOutput>> GetEquipmentRentalByIdAsync(string id)
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentRentalManager.GetEquipmentRentalByIdAsync(id);
                return new ServiceMessage<EquipmentRentalOutput>(result);
            });
        }

        /// <summary>
        /// 添加设备租用
        /// </summary>
        [HttpPost("add")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> AddEquipmentRentalAsync([FromBody] EquipmentRentalInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentRentalManager.AddEquipmentRentalAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 更新设备租用
        /// </summary>
        [HttpPost("update")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> UpdateEquipmentRentalAsync([FromBody] EquipmentRentalInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentRentalManager.UpdateEquipmentRentalAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 审批设备租用
        /// </summary>
        [HttpPost("approve")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> ApproveEquipmentRentalAsync(string id, int status, string approver)
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentRentalManager.ApproveEquipmentRentalAsync(id, status, approver);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 归还设备
        /// </summary>
        [HttpPost("return")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> ReturnEquipmentAsync(string id, string actualReturnDate)
        {
            return Task.Run(async () =>
            {
                var date = DateTime.Parse(actualReturnDate);
                var result = await _equipmentRentalManager.ReturnEquipmentAsync(id, date);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 获取设备租用延长申请列表
        /// </summary>
        [HttpGet("extensions/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EquipmentRentalExtensionOutput[]>> GetEquipmentRentalExtensionsAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentRentalManager.GetEquipmentRentalExtensionsAsync();
                return new ServiceMessage<EquipmentRentalExtensionOutput[]>(result.ToArray());
            });
        }

        /// <summary>
        /// 添加设备租用延长申请
        /// </summary>
        [HttpPost("extensions/add")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> AddEquipmentRentalExtensionAsync([FromBody] EquipmentRentalExtensionInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentRentalManager.AddEquipmentRentalExtensionAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 审批设备租用延长申请
        /// </summary>
        [HttpPost("extensions/approve")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> ApproveEquipmentRentalExtensionAsync(string id, int status, string approver)
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentRentalManager.ApproveEquipmentRentalExtensionAsync(id, status, approver);
                return new ServiceMessage<bool>(result);
            });
        }
    }
}
