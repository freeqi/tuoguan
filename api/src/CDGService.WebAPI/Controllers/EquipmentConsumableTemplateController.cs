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
    public class EquipmentConsumableTemplateController : ControllerBase
    {
        private readonly EquipmentConsumableTemplateManager _equipmentConsumableTemplateManager;

        public EquipmentConsumableTemplateController(EquipmentConsumableTemplateManager equipmentConsumableTemplateManager)
        {
            _equipmentConsumableTemplateManager = equipmentConsumableTemplateManager;
        }

        /// <summary>
        /// 获取设备耗材模板列表
        /// </summary>
        [HttpGet("templates")]
        public async Task<ActionResult<List<EquipmentConsumableTemplateOutput>>> GetTemplates()
        {
            try
            {
                var templates = await _equipmentConsumableTemplateManager.GetTemplatesAsync();
                return Ok(templates);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取设备耗材模板
        /// </summary>
        [HttpGet("templates/{id}")]
        public async Task<ActionResult<EquipmentConsumableTemplateOutput>> GetTemplateById(string id)
        {
            try
            {
                var template = await _equipmentConsumableTemplateManager.GetTemplateByIdAsync(id);
                if (template == null)
                {
                    return NotFound(new { message = "模板不存在" });
                }
                return Ok(template);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 添加设备耗材模板
        /// </summary>
        [HttpPost("templates")]
        public async Task<ActionResult<bool>> AddTemplate([FromBody] EquipmentConsumableTemplateInput input)
        {
            try
            {
                var result = await _equipmentConsumableTemplateManager.AddTemplateAsync(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 更新设备耗材模板
        /// </summary>
        [HttpPut("templates")]
        public async Task<ActionResult<bool>> UpdateTemplate([FromBody] EquipmentConsumableTemplateInput input)
        {
            try
            {
                var result = await _equipmentConsumableTemplateManager.UpdateTemplateAsync(input);
                if (!result)
                {
                    return NotFound(new { message = "模板不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 删除设备耗材模板
        /// </summary>
        [HttpDelete("templates/{id}")]
        public async Task<ActionResult<bool>> DeleteTemplate(string id)
        {
            try
            {
                var result = await _equipmentConsumableTemplateManager.DeleteTemplateAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "模板不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 获取耗材采购申请列表
        /// </summary>
        [HttpGet("purchase-requests")]
        public async Task<ActionResult<List<ConsumablePurchaseRequestOutput>>> GetPurchaseRequests()
        {
            try
            {
                var requests = await _equipmentConsumableTemplateManager.GetPurchaseRequestsAsync();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取耗材采购申请
        /// </summary>
        [HttpGet("purchase-requests/{id}")]
        public async Task<ActionResult<ConsumablePurchaseRequestOutput>> GetPurchaseRequestById(string id)
        {
            try
            {
                var request = await _equipmentConsumableTemplateManager.GetPurchaseRequestByIdAsync(id);
                if (request == null)
                {
                    return NotFound(new { message = "采购申请不存在" });
                }
                return Ok(request);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 添加耗材采购申请
        /// </summary>
        [HttpPost("purchase-requests")]
        public async Task<ActionResult<bool>> AddPurchaseRequest([FromBody] ConsumablePurchaseRequestInput input)
        {
            try
            {
                var result = await _equipmentConsumableTemplateManager.AddPurchaseRequestAsync(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 更新耗材采购申请
        /// </summary>
        [HttpPut("purchase-requests")]
        public async Task<ActionResult<bool>> UpdatePurchaseRequest([FromBody] ConsumablePurchaseRequestInput input)
        {
            try
            {
                var result = await _equipmentConsumableTemplateManager.UpdatePurchaseRequestAsync(input);
                if (!result)
                {
                    return NotFound(new { message = "采购申请不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 审批耗材采购申请
        /// </summary>
        [HttpPut("purchase-requests/{id}/approve")]
        public async Task<ActionResult<bool>> ApprovePurchaseRequest(string id, [FromBody] ApproveRequest approveRequest)
        {
            try
            {
                var result = await _equipmentConsumableTemplateManager.ApprovePurchaseRequestAsync(id, approveRequest.Status, approveRequest.Approver);
                if (!result)
                {
                    return NotFound(new { message = "采购申请不存在" });
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
