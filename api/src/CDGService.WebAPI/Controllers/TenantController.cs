using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly TenantManager _tenantManager;

        public TenantController(TenantManager tenantManager)
        {
            _tenantManager = tenantManager;
        }

        /// <summary>
        /// 获取租户列表
        /// </summary>
        [HttpGet("list")]
        [HttpPost("tenantlist")]
        public async Task<IActionResult> GetTenants()
        {
            var result = await _tenantManager.GetTenantsAsync();
            return Ok(new { success = true, result = result, dataCount = result.Count, code = 0 });
        }

        /// <summary>
        /// 根据ID获取租户
        /// </summary>
        [HttpGet("detail")]
        public async Task<IActionResult> GetTenantById(string id)
        {
            var result = await _tenantManager.GetTenantByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// 添加租户
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddTenant([FromBody] TenantInput input)
        {
            var result = await _tenantManager.AddTenantAsync(input);
            return Ok(new { success = result });
        }

        /// <summary>
        /// 更新租户
        /// </summary>
        [HttpPost("update")]
        public async Task<IActionResult> UpdateTenant([FromBody] TenantInput input)
        {
            var result = await _tenantManager.UpdateTenantAsync(input);
            return Ok(new { success = result });
        }

        /// <summary>
        /// 删除租户
        /// </summary>
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteTenant(string id)
        {
            var result = await _tenantManager.DeleteTenantAsync(id);
            return Ok(new { success = result });
        }

        /// <summary>
        /// 分配用户到租户
        /// </summary>
        [HttpPost("assign-user")]
        public async Task<IActionResult> AssignUserToTenant([FromBody] UserTenantInput input)
        {
            var result = await _tenantManager.AssignUserToTenantAsync(input);
            return Ok(new { success = result });
        }

        /// <summary>
        /// 从租户中移除用户
        /// </summary>
        [HttpPost("remove-user")]
        public async Task<IActionResult> RemoveUserFromTenant([FromBody] UserTenantInput input)
        {
            var result = await _tenantManager.RemoveUserFromTenantAsync(input);
            return Ok(new { success = result });
        }

        /// <summary>
        /// 获取租户下的用户
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsersByTenantId(string tenantId)
        {
            var result = await _tenantManager.GetUsersByTenantIdAsync(tenantId);
            return Ok(result);
        }
    }
}
