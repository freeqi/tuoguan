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
    public class EquipmentMonitoringController : ControllerBase
    {
        private readonly EquipmentMonitoringManager _equipmentMonitoringManager;

        public EquipmentMonitoringController(EquipmentMonitoringManager equipmentMonitoringManager)
        {
            _equipmentMonitoringManager = equipmentMonitoringManager;
        }

        /// <summary>
        /// 获取设备监控记录列表
        /// </summary>
        [HttpGet("monitorings")]
        public async Task<ActionResult<List<EquipmentMonitoringOutput>>> GetMonitorings()
        {
            try
            {
                var monitorings = await _equipmentMonitoringManager.GetMonitoringsAsync();
                return Ok(monitorings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取设备监控记录
        /// </summary>
        [HttpGet("monitorings/{id}")]
        public async Task<ActionResult<EquipmentMonitoringOutput>> GetMonitoringById(string id)
        {
            try
            {
                var monitoring = await _equipmentMonitoringManager.GetMonitoringByIdAsync(id);
                if (monitoring == null)
                {
                    return NotFound(new { message = "监控记录不存在" });
                }
                return Ok(monitoring);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 添加设备监控记录
        /// </summary>
        [HttpPost("monitorings")]
        public async Task<ActionResult<bool>> AddMonitoring([FromBody] EquipmentMonitoringInput input)
        {
            try
            {
                var result = await _equipmentMonitoringManager.AddMonitoringAsync(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 更新设备监控记录
        /// </summary>
        [HttpPut("monitorings")]
        public async Task<ActionResult<bool>> UpdateMonitoring([FromBody] EquipmentMonitoringInput input)
        {
            try
            {
                var result = await _equipmentMonitoringManager.UpdateMonitoringAsync(input);
                if (!result)
                {
                    return NotFound(new { message = "监控记录不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 删除设备监控记录
        /// </summary>
        [HttpDelete("monitorings/{id}")]
        public async Task<ActionResult<bool>> DeleteMonitoring(string id)
        {
            try
            {
                var result = await _equipmentMonitoringManager.DeleteMonitoringAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "监控记录不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 获取设备故障预警列表
        /// </summary>
        [HttpGet("fault-warnings")]
        public async Task<ActionResult<List<EquipmentFaultWarningOutput>>> GetFaultWarnings()
        {
            try
            {
                var warnings = await _equipmentMonitoringManager.GetFaultWarningsAsync();
                return Ok(warnings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取设备故障预警
        /// </summary>
        [HttpGet("fault-warnings/{id}")]
        public async Task<ActionResult<EquipmentFaultWarningOutput>> GetFaultWarningById(string id)
        {
            try
            {
                var warning = await _equipmentMonitoringManager.GetFaultWarningByIdAsync(id);
                if (warning == null)
                {
                    return NotFound(new { message = "故障预警不存在" });
                }
                return Ok(warning);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 添加设备故障预警
        /// </summary>
        [HttpPost("fault-warnings")]
        public async Task<ActionResult<bool>> AddFaultWarning([FromBody] EquipmentFaultWarningInput input)
        {
            try
            {
                var result = await _equipmentMonitoringManager.AddFaultWarningAsync(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 更新设备故障预警
        /// </summary>
        [HttpPut("fault-warnings")]
        public async Task<ActionResult<bool>> UpdateFaultWarning([FromBody] EquipmentFaultWarningInput input)
        {
            try
            {
                var result = await _equipmentMonitoringManager.UpdateFaultWarningAsync(input);
                if (!result)
                {
                    return NotFound(new { message = "故障预警不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 处理设备故障预警
        /// </summary>
        [HttpPut("fault-warnings/{id}/handle")]
        public async Task<ActionResult<bool>> HandleFaultWarning(string id, [FromBody] HandleWarningRequest request)
        {
            try
            {
                var result = await _equipmentMonitoringManager.HandleFaultWarningAsync(id, request.ProcessingStatus, request.Handler, request.ProcessingResult);
                if (!result)
                {
                    return NotFound(new { message = "故障预警不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 删除设备故障预警
        /// </summary>
        [HttpDelete("fault-warnings/{id}")]
        public async Task<ActionResult<bool>> DeleteFaultWarning(string id)
        {
            try
            {
                var result = await _equipmentMonitoringManager.DeleteFaultWarningAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "故障预警不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 获取设备故障记录列表
        /// </summary>
        [HttpGet("fault-records")]
        public async Task<ActionResult<List<EquipmentFaultRecordOutput>>> GetFaultRecords()
        {
            try
            {
                var records = await _equipmentMonitoringManager.GetFaultRecordsAsync();
                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取设备故障记录
        /// </summary>
        [HttpGet("fault-records/{id}")]
        public async Task<ActionResult<EquipmentFaultRecordOutput>> GetFaultRecordById(string id)
        {
            try
            {
                var record = await _equipmentMonitoringManager.GetFaultRecordByIdAsync(id);
                if (record == null)
                {
                    return NotFound(new { message = "故障记录不存在" });
                }
                return Ok(record);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 添加设备故障记录
        /// </summary>
        [HttpPost("fault-records")]
        public async Task<ActionResult<bool>> AddFaultRecord([FromBody] EquipmentFaultRecordInput input)
        {
            try
            {
                var result = await _equipmentMonitoringManager.AddFaultRecordAsync(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 更新设备故障记录
        /// </summary>
        [HttpPut("fault-records")]
        public async Task<ActionResult<bool>> UpdateFaultRecord([FromBody] EquipmentFaultRecordInput input)
        {
            try
            {
                var result = await _equipmentMonitoringManager.UpdateFaultRecordAsync(input);
                if (!result)
                {
                    return NotFound(new { message = "故障记录不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 解决设备故障
        /// </summary>
        [HttpPut("fault-records/{id}/resolve")]
        public async Task<ActionResult<bool>> ResolveFault(string id, [FromBody] ResolveFaultRequest request)
        {
            try
            {
                var result = await _equipmentMonitoringManager.ResolveFaultAsync(id, request.FaultResolveTime, request.Handler, request.HandlingMethod);
                if (!result)
                {
                    return NotFound(new { message = "故障记录不存在" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 删除设备故障记录
        /// </summary>
        [HttpDelete("fault-records/{id}")]
        public async Task<ActionResult<bool>> DeleteFaultRecord(string id)
        {
            try
            {
                var result = await _equipmentMonitoringManager.DeleteFaultRecordAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "故障记录不存在" });
                }
                return Ok(result);
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

        /// <summary>
        /// 解决故障请求
        /// </summary>
        public class ResolveFaultRequest
        {
            public DateTime FaultResolveTime { get; set; }
            public string Handler { get; set; }
            public string HandlingMethod { get; set; }
        }
    }
}
