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
    public class DashboardController : ControllerBase
    {
        private readonly DashboardManager _dashboardManager;

        public DashboardController(DashboardManager dashboardManager)
        {
            _dashboardManager = dashboardManager;
        }

        /// <summary>
        /// 获取仪表盘统计数据
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<DashboardStatisticsOutput>> GetDashboardStatistics()
        {
            try
            {
                var statistics = await _dashboardManager.GetDashboardStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
