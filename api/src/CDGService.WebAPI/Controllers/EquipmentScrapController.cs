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
    public class EquipmentScrapController : ControllerBase
    {
        private readonly EquipmentScrapManager _equipmentScrapManager;

        public EquipmentScrapController(EquipmentScrapManager equipmentScrapManager)
        {
            _equipmentScrapManager = equipmentScrapManager;
        }

        /// <summary>
        /// 获取设备报废记录列表
        /// </summary>
        [HttpGet("scraps")]
        public async Task<ActionResult<List<EquipmentScrapOutput>>> GetScraps()
        {
            try
            {
                var scraps = await _equipmentScrapManager.GetScrapsAsync();
                return Ok(scraps);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取设备报废记录
        /// </summary>
        [HttpGet("scraps/{id}")]
        public async Task<ActionResult<EquipmentScrapOutput>> GetScrapById(string id)
        {
            try
            {
                var scrap = await _equipmentScrapManager.GetScrapByIdAsync(id);
                if (scrap == null)
                {
                    return NotFound(new { message = "报废记录不存在" });
                }
                return Ok(scrap);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 添加设备报废记录
        /// </summary>
        [HttpPost("scraps")]
        public async Task<ActionResult<bool>> AddScrap([FromBody] EquipmentScrapInput input)
        {
            try
            {
                var result = await _equipmentScrapManager.AddScrapAsync(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 更新设备报废记录
        /// </summary>
        [HttpPut("scraps")]
        public async Task<ActionResult<bool>> UpdateScrap([FromBody] EquipmentScrapInput input)
        {
            try
            {
                var result = await _equipmentScrapManager.UpdateScrapAsync(input);
                if (!result)
                {
                    return NotFound(new { message = "报废记录不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 删除设备报废记录
        /// </summary>
        [HttpDelete("scraps/{id}")]
        public async Task<ActionResult<bool>> DeleteScrap(string id)
        {
            try
            {
                var result = await _equipmentScrapManager.DeleteScrapAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "报废记录不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 获取设备退租记录列表
        /// </summary>
        [HttpGet("returns")]
        public async Task<ActionResult<List<EquipmentReturnOutput>>> GetReturns()
        {
            try
            {
                var returns = await _equipmentScrapManager.GetReturnsAsync();
                return Ok(returns);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取设备退租记录
        /// </summary>
        [HttpGet("returns/{id}")]
        public async Task<ActionResult<EquipmentReturnOutput>> GetReturnById(string id)
        {
            try
            {
                var returnRecord = await _equipmentScrapManager.GetReturnByIdAsync(id);
                if (returnRecord == null)
                {
                    return NotFound(new { message = "退租记录不存在" });
                }
                return Ok(returnRecord);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 添加设备退租记录
        /// </summary>
        [HttpPost("returns")]
        public async Task<ActionResult<bool>> AddReturn([FromBody] EquipmentReturnInput input)
        {
            try
            {
                var result = await _equipmentScrapManager.AddReturnAsync(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 更新设备退租记录
        /// </summary>
        [HttpPut("returns")]
        public async Task<ActionResult<bool>> UpdateReturn([FromBody] EquipmentReturnInput input)
        {
            try
            {
                var result = await _equipmentScrapManager.UpdateReturnAsync(input);
                if (!result)
                {
                    return NotFound(new { message = "退租记录不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 删除设备退租记录
        /// </summary>
        [HttpDelete("returns/{id}")]
        public async Task<ActionResult<bool>> DeleteReturn(string id)
        {
            try
            {
                var result = await _equipmentScrapManager.DeleteReturnAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "退租记录不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 获取设备巡检记录列表
        /// </summary>
        [HttpGet("inspections")]
        public async Task<ActionResult<List<EquipmentInspectionOutput>>> GetInspections()
        {
            try
            {
                var inspections = await _equipmentScrapManager.GetInspectionsAsync();
                return Ok(inspections);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取设备巡检记录
        /// </summary>
        [HttpGet("inspections/{id}")]
        public async Task<ActionResult<EquipmentInspectionOutput>> GetInspectionById(string id)
        {
            try
            {
                var inspection = await _equipmentScrapManager.GetInspectionByIdAsync(id);
                if (inspection == null)
                {
                    return NotFound(new { message = "巡检记录不存在" });
                }
                return Ok(inspection);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 添加设备巡检记录
        /// </summary>
        [HttpPost("inspections")]
        public async Task<ActionResult<bool>> AddInspection([FromBody] EquipmentInspectionInput input)
        {
            try
            {
                var result = await _equipmentScrapManager.AddInspectionAsync(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 更新设备巡检记录
        /// </summary>
        [HttpPut("inspections")]
        public async Task<ActionResult<bool>> UpdateInspection([FromBody] EquipmentInspectionInput input)
        {
            try
            {
                var result = await _equipmentScrapManager.UpdateInspectionAsync(input);
                if (!result)
                {
                    return NotFound(new { message = "巡检记录不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 删除设备巡检记录
        /// </summary>
        [HttpDelete("inspections/{id}")]
        public async Task<ActionResult<bool>> DeleteInspection(string id)
        {
            try
            {
                var result = await _equipmentScrapManager.DeleteInspectionAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "巡检记录不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
