using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DataAnalysisController : ControllerBase
    {
        private readonly DataAnalysisManager _dataAnalysisManager;

        public DataAnalysisController(DataAnalysisManager dataAnalysisManager)
        {
            _dataAnalysisManager = dataAnalysisManager;
        }

        /// <summary>
        /// 获取设备故障统计数据
        /// </summary>
        [HttpPost("equipment-fault")]
        public async Task<ActionResult<EquipmentFaultStatisticsOutput>> GetEquipmentFaultStatistics([FromBody] EquipmentFaultStatisticsInput input)
        {
            try
            {
                var statistics = await _dataAnalysisManager.GetEquipmentFaultStatisticsAsync(input);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 获取耗材进销存统计数据
        /// </summary>
        [HttpPost("consumable-inventory")]
        public async Task<ActionResult<ConsumableInventoryStatisticsOutput>> GetConsumableInventoryStatistics([FromBody] ConsumableInventoryStatisticsInput input)
        {
            try
            {
                var statistics = await _dataAnalysisManager.GetConsumableInventoryStatisticsAsync(input);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
