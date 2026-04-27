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
    public class InventoryController : ControllerBase
    {
        private readonly InventoryManager _inventoryManager;

        public InventoryController(InventoryManager inventoryManager)
        {
            _inventoryManager = inventoryManager;
        }

        /// <summary>
        /// 获取耗材入库记录列表
        /// </summary>
        [HttpGet("inbounds")]
        public async Task<ActionResult<List<ConsumableInboundOutput>>> GetInbounds()
        {
            try
            {
                var inbounds = await _inventoryManager.GetInboundsAsync();
                return Ok(inbounds);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 添加耗材入库记录
        /// </summary>
        [HttpPost("inbounds")]
        public async Task<ActionResult<bool>> AddInbound([FromBody] ConsumableInboundInput input)
        {
            try
            {
                var result = await _inventoryManager.AddInboundAsync(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 获取耗材出库记录列表
        /// </summary>
        [HttpGet("outbounds")]
        public async Task<ActionResult<List<ConsumableOutboundOutput>>> GetOutbounds()
        {
            try
            {
                var outbounds = await _inventoryManager.GetOutboundsAsync();
                return Ok(outbounds);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 添加耗材出库记录
        /// </summary>
        [HttpPost("outbounds")]
        public async Task<ActionResult<bool>> AddOutbound([FromBody] ConsumableOutboundInput input)
        {
            try
            {
                var result = await _inventoryManager.AddOutboundAsync(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 获取耗材库存预警列表
        /// </summary>
        [HttpGet("warnings")]
        public async Task<ActionResult<List<ConsumableInventoryWarningOutput>>> GetInventoryWarnings()
        {
            try
            {
                var warnings = await _inventoryManager.GetInventoryWarningsAsync();
                return Ok(warnings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 处理库存预警
        /// </summary>
        [HttpPut("warnings/{id}/handle")]
        public async Task<ActionResult<bool>> HandleInventoryWarning(string id, [FromBody] HandleWarningRequest request)
        {
            try
            {
                var result = await _inventoryManager.HandleInventoryWarningAsync(id, request.ProcessingStatus, request.Handler, request.ProcessingResult);
                if (!result)
                {
                    return NotFound(new { message = "库存预警不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 查询耗材库存
        /// </summary>
        [HttpPost("query")]
        public async Task<ActionResult<List<ConsumableInventoryOutput>>> QueryInventory([FromBody] ConsumableInventoryQueryInput input)
        {
            try
            {
                var inventory = await _inventoryManager.QueryInventoryAsync(input);
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 处理预警请求
        /// </summary>
        public class HandleWarningRequest
        {
            public int ProcessingStatus { get; set; }
            public string Handler { get; set; }
            public string ProcessingResult { get; set; }
        }
    }
}
