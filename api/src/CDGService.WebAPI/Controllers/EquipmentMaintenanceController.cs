using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EquipmentMaintenanceController : ControllerBase
    {
        private readonly EquipmentMaintenanceManager _equipmentMaintenanceManager;

        public EquipmentMaintenanceController(EquipmentMaintenanceManager equipmentMaintenanceManager)
        {
            _equipmentMaintenanceManager = equipmentMaintenanceManager;
        }

        /// <summary>
        /// 获取设备维保记录列表
        /// </summary>
        [HttpGet("maintenances")]
        public async Task<ActionResult<List<EquipmentMaintenanceOutput>>> GetMaintenances()
        {
            try
            {
                var maintenances = await _equipmentMaintenanceManager.GetMaintenancesAsync();
                return Ok(maintenances);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取设备维保记录
        /// </summary>
        [HttpGet("maintenances/{id}")]
        public async Task<ActionResult<EquipmentMaintenanceOutput>> GetMaintenanceById(string id)
        {
            try
            {
                var maintenance = await _equipmentMaintenanceManager.GetMaintenanceByIdAsync(id);
                if (maintenance == null)
                {
                    return NotFound(new { message = "维保记录不存在" });
                }
                return Ok(maintenance);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 添加设备维保记录
        /// </summary>
        [HttpPost("maintenances")]
        public async Task<ActionResult<bool>> AddMaintenance([FromBody] EquipmentMaintenanceInput input)
        {
            try
            {
                var result = await _equipmentMaintenanceManager.AddMaintenanceAsync(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 更新设备维保记录
        /// </summary>
        [HttpPut("maintenances")]
        public async Task<ActionResult<bool>> UpdateMaintenance([FromBody] EquipmentMaintenanceInput input)
        {
            try
            {
                var result = await _equipmentMaintenanceManager.UpdateMaintenanceAsync(input);
                if (!result)
                {
                    return NotFound(new { message = "维保记录不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 删除设备维保记录
        /// </summary>
        [HttpDelete("maintenances/{id}")]
        public async Task<ActionResult<bool>> DeleteMaintenance(string id)
        {
            try
            {
                var result = await _equipmentMaintenanceManager.DeleteMaintenanceAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "维保记录不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 获取设备维保延长申请列表
        /// </summary>
        [HttpGet("extensions")]
        public async Task<ActionResult<List<EquipmentMaintenanceExtensionOutput>>> GetMaintenanceExtensions()
        {
            try
            {
                var extensions = await _equipmentMaintenanceManager.GetMaintenanceExtensionsAsync();
                return Ok(extensions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取设备维保延长申请
        /// </summary>
        [HttpGet("extensions/{id}")]
        public async Task<ActionResult<EquipmentMaintenanceExtensionOutput>> GetMaintenanceExtensionById(string id)
        {
            try
            {
                var extension = await _equipmentMaintenanceManager.GetMaintenanceExtensionByIdAsync(id);
                if (extension == null)
                {
                    return NotFound(new { message = "维保延长申请不存在" });
                }
                return Ok(extension);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 添加设备维保延长申请
        /// </summary>
        [HttpPost("extensions")]
        public async Task<ActionResult<bool>> AddMaintenanceExtension([FromBody] EquipmentMaintenanceExtensionInput input)
        {
            try
            {
                var result = await _equipmentMaintenanceManager.AddMaintenanceExtensionAsync(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 更新设备维保延长申请
        /// </summary>
        [HttpPut("extensions")]
        public async Task<ActionResult<bool>> UpdateMaintenanceExtension([FromBody] EquipmentMaintenanceExtensionInput input)
        {
            try
            {
                var result = await _equipmentMaintenanceManager.UpdateMaintenanceExtensionAsync(input);
                if (!result)
                {
                    return NotFound(new { message = "维保延长申请不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 审批设备维保延长申请
        /// </summary>
        [HttpPut("extensions/{id}/approve")]
        public async Task<ActionResult<bool>> ApproveMaintenanceExtension(string id, [FromBody] ApproveRequest approveRequest)
        {
            try
            {
                var result = await _equipmentMaintenanceManager.ApproveMaintenanceExtensionAsync(id, approveRequest.Status, approveRequest.Approver);
                if (!result)
                {
                    return NotFound(new { message = "维保延长申请不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 删除设备维保延长申请
        /// </summary>
        [HttpDelete("extensions/{id}")]
        public async Task<ActionResult<bool>> DeleteMaintenanceExtension(string id)
        {
            try
            {
                var result = await _equipmentMaintenanceManager.DeleteMaintenanceExtensionAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "维保延长申请不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 审批请求
        /// </summary>
        public class ApproveRequest
        {
            public int Status { get; set; }
            public string Approver { get; set; }
        }
    }
}
