using CDGService.Data.Datas;
using CDGService.Data.Enums;
using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Dto;
using CDGService.WebAPI.Extenstions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Controllers
{
    [EnableCors("any")] //启用跨域
    [Route("api/[controller]")]
    public class EquipmentController : Controller
    {

        private readonly EquipmentManager _equipmentManager;
        MaintenanceRecordManger _maintenanceRecordManger;

        public EquipmentController(EquipmentManager equipmentManager, MaintenanceRecordManger maintenanceRecordManger)
        {


            _equipmentManager = equipmentManager;
            _maintenanceRecordManger = maintenanceRecordManger;
        }

        /// <summary>
        ///  获取设备数据列表
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input">查询员工信息条件参数</param>
        /// <returns></returns>
        [HttpPost("Equipment")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EquipmentOutPut[]>> GetEquipmentQueryableAsync([FromBody] EquipmentInPut input)
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentManager.GetEquipmentQueryableAsync(input);
                return new ServiceMessage<EquipmentOutPut[]>(result.Result, result.Count);
            });
        }
        //

        /// <summary>
        /// 根据设备型号统计
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetEquiByModelList/{centerid}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EmplyeeByStaBisOutPut[]>> GetEquiByModelQueryableAsync(string  centerid)
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentManager.GetEquiByModelQueryableAsync(centerid);
                return new ServiceMessage<EmplyeeByStaBisOutPut[]>(result);

            });

        }

        /// <summary>
        /// 根据设备分布统计
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetEquiByModelDistribution")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<LineOutPut>> GetEquiByModelDistributionQueryableAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentManager.GetEquiByModelDistributionQueryableAsync();
                return new ServiceMessage<LineOutPut>(result);
            });

        }

        //
        /// <summary>
        /// 根据设备使用状态统计
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetEquiBySeate/{centerid}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EmplyeeByStaBisOutPut[]>> GetEquiByStateQueryableAsync(string  centerid)
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentManager.GetEquiByStateQueryableAsync(centerid);
                return new ServiceMessage<EmplyeeByStaBisOutPut[]>(result);
            });

        }
        //  public Task<EmplyeeByStaBisOutPut[]> GetEquiByUseYearQueryableAsync(int CenterId)
        /// <summary>
        /// 根据设备使用年限统计
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetEquiByuseYear/{centerid}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EmplyeeByStaBisOutPut[]>> GetEquiByUseYearQueryableAsync(string  centerid)
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentManager.GetEquiByUseYearQueryableAsync(centerid);
                return new ServiceMessage<EmplyeeByStaBisOutPut[]>(result);
            });

        }


        #region 维修记录
        /// <summary>
        /// 设备维修记录  
        /// </summary>
        /// <param name="EqId">设备ID</param>
        /// <returns></returns>
        [HttpGet("GetMr/{EqId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MaintenanceRecordOutPut[]>> GetMrByiIdAsync(string  EqId)
        {
            return Task.Run(async () =>
            {
                var result = await _maintenanceRecordManger.GetMrByiIdAsync(EqId);
                return new ServiceMessage<MaintenanceRecordOutPut[]>(result);
            });

        }
        ///
        /// <summary>
        /// 设备维保记录
        /// </summary>
        /// <param name="EqId">设备ID</param>
        /// <returns></returns>
        [HttpGet("GetEkr/{EqId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EquipmentKeepRecordOutPut[]>> GetEkrByiIdAsync(string   EqId)
        {
            return Task.Run(async () =>
            {
                var result = await _maintenanceRecordManger.GetEkrByiIdAsync(EqId);
                return new ServiceMessage<EquipmentKeepRecordOutPut[]>(result);
            });

        }
        //

        ///
        /// <summary>
        /// 设备生化检测记录
        /// </summary>
        /// <param name="EqId">设备ID</param>
        /// <returns></returns>
        [HttpGet("GetEbt/{EqId}/{pHEnum}/{TestType}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<BiochemicalTest[]>> GetEbtByiIdAsync(string  EqId, string pHEnum,string TestType)
        {
            return Task.Run(async () =>
            {
                var result = await _maintenanceRecordManger.GetEbtByiIdAsync(EqId, pHEnum, TestType);
                return new ServiceMessage<BiochemicalTest[]>(result);
            });

        }
        #endregion

        #region 设备管理
        /// <summary>
        /// 添加设备
        /// </summary>
        [HttpPost("AddEquipment")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> AddEquipmentAsync([FromBody] EquipmentInfo equipment)
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentManager.AddEquipmentAsync(equipment);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 更新设备
        /// </summary>
        [HttpPost("UpdateEquipment")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> UpdateEquipmentAsync([FromBody] EquipmentInfo equipment)
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentManager.UpdateEquipmentAsync(equipment);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        [HttpPost("DeleteEquipment")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> DeleteEquipmentAsync(string id)
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentManager.DeleteEquipmentAsync(id);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 根据ID获取设备
        /// </summary>
        [HttpGet("GetEquipmentById")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EquipmentInfo>> GetEquipmentByIdAsync(string id)
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentManager.GetEquipmentByIdAsync(id);
                return new ServiceMessage<EquipmentInfo>(result);
            });
        }

        /// <summary>
        /// 获取设备类型列表
        /// </summary>
        [HttpGet("GetEquipmentTypes")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EquipmentType[]>> GetEquipmentTypesAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentManager.GetEquipmentTypesAsync();
                return new ServiceMessage<EquipmentType[]>(result.ToArray());
            });
        }

        /// <summary>
        /// 获取设备供应商列表
        /// </summary>
        [HttpGet("GetEquipmentSuppliers")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EquipmentSupplier[]>> GetEquipmentSuppliersAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentManager.GetEquipmentSuppliersAsync();
                return new ServiceMessage<EquipmentSupplier[]>(result.ToArray());
            });
        }

        /// <summary>
        /// 获取设备制造商列表
        /// </summary>
        [HttpGet("GetEquipmentManufacturers")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EquipmentManufacturers[]>> GetEquipmentManufacturersAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentManager.GetEquipmentManufacturersAsync();
                return new ServiceMessage<EquipmentManufacturers[]>(result.ToArray());
            });
        }

        /// <summary>
        /// 获取设备型号列表
        /// </summary>
        [HttpGet("GetEquipmentModels")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EquipmentModel[]>> GetEquipmentModelsAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentManager.GetEquipmentModelsAsync();
                return new ServiceMessage<EquipmentModel[]>(result.ToArray());
            });
        }

        /// <summary>
        /// 根据设备类型获取设备型号列表
        /// </summary>
        [HttpGet("GetEquipmentModelsByTypeId")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EquipmentModel[]>> GetEquipmentModelsByTypeIdAsync(string typeId)
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentManager.GetEquipmentModelsByTypeIdAsync(typeId);
                return new ServiceMessage<EquipmentModel[]>(result.ToArray());
            });
        }

        /// <summary>
        /// 根据制造商获取设备型号列表
        /// </summary>
        [HttpGet("GetEquipmentModelsByManufacturerId")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EquipmentModel[]>> GetEquipmentModelsByManufacturerIdAsync(string manufacturerId)
        {
            return Task.Run(async () =>
            {
                var result = await _equipmentManager.GetEquipmentModelsByManufacturerIdAsync(manufacturerId);
                return new ServiceMessage<EquipmentModel[]>(result.ToArray());
            });
        }
        #endregion
    }
}
