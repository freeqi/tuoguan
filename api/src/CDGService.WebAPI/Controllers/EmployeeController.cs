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
    public class EmployeeController : Controller
    {

        private readonly EmployeeManger _employeeManger;

        public EmployeeController(EmployeeManger employeeManger)
        {


            _employeeManger = employeeManger;
        }
        /// <summary>
        ///  获取员工数据列表
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input">查询员工信息条件参数</param>
        /// <returns></returns>
        [HttpPost("Employee")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EmployeeOutPut[]>> GetEmployeeQueryableAsync([FromBody]EmployeeQueryInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _employeeManger.GetDialysisQueryableAsync(input);
                return new ServiceMessage<EmployeeOutPut[]>(result.Result, result.Count);
            });
        }
        /// <summary>
        ///  获取员工调动记录
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <returns></returns>
        [HttpPost("Employee/Transfers/{empId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EmployeeTransferOutPut[]>> GetEmployeeTransfersAsync([FromRoute]string empId)
        {
            return Task.Run(async () =>
            {
                var result = await _employeeManger.GetEmployeeTransfersAsync(empId);

                return new ServiceMessage<EmployeeTransferOutPut[]>(result);
            });
        }
        /// <summary>
        /// 创建或修改员工信息        
        /// Id为0或null.视为新增
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input">员工信息数据</param>
        /// <returns></returns>
        [HttpPost("CreateUpdateEmployee")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CreateUpdateEmployeeAsync([FromBody]EmployeeInput input)
        {
            return Task.Run(async () =>
            {

                var result = await _employeeManger.CreateUpdateEmployeeAsync(input);
                return new ServiceMessage<bool>(result);

            });
        }
        /// <summary>
        /// 删除员工信息。
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpGet("del/{id}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> DeleteDialysisAsync([FromRoute]string id)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var result = await _employeeManger.DeleteDialysisAsync(id);

                    return new ServiceMessage<bool>(result);
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
            });
        }

        #region  统计

        /// <summary>
        /// 根据机构统计员工信息
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpPost("GetEmplyeeByDialysisList")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EmplyeeByDiaOutPut[]>> GetEmplyeeByDialysisQueryableAsync([FromBody] empCont EmpType)
        {
            return Task.Run(async () =>
            {
                var result = await _employeeManger.GetEmplyeeByDialysisQueryableAsync(EmpType.empcount);
                return new ServiceMessage<EmplyeeByDiaOutPut[]>(result);

            });

        }
        //
        /// <summary>
        /// 根据性别统计员工信息
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetEmplyeeBySexList/{centerid}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EmplyeeByStaBisOutPut[]>> GetEmplyeeBySexQueryableAsync(string centerid)
        {
            return Task.Run(async () =>
            {
                var result = await _employeeManger.GetEmplyeeBySexQueryableAsync(centerid);
                return new ServiceMessage<EmplyeeByStaBisOutPut[]>(result);

            });

        }
        //职称GetEmplyeeByTitlexQueryableAsync

        //
        /// <summary>
        /// 根据职称统计员工信息
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetEmplyeeByJobTitleList/{centerid}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EmplyeeByStaBisOutPut[]>> GetEmplyeeByTitlexQueryableAsync(string centerid)
        {
            return Task.Run(async () =>
            {
                var result = await _employeeManger.GetEmplyeeByTitlexQueryableAsync(centerid);
                return new ServiceMessage<EmplyeeByStaBisOutPut[]>(result);

            });

        }

        //职位
        //
        /// <summary>
        /// 根据职位统计员工信息
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetEmplyeeByProfessionList/{centerid}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EmplyeeByStaBisOutPut[]>> GetEmplyeeByProfessionQueryableAsync(string centerid)
        {
            return Task.Run(async () =>
            {
                var result = await _employeeManger.GetEmplyeeByProfessionQueryableAsync(centerid);
                return new ServiceMessage<EmplyeeByStaBisOutPut[]>(result);

            });

        }
        //学历 GetEmplyeeByEducationxQueryableAsync
        /// <summary>
        /// 根据学历统计员工信息
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetEmplyeeByEducationxList/{centerid}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EmplyeeByStaBisOutPut[]>> GetEmplyeeByEducationxQueryableAsync(string centerid)
        {
            return Task.Run(async () =>
            {
                var result = await _employeeManger.GetEmplyeeByEducationxQueryableAsync(centerid);
                return new ServiceMessage<EmplyeeByStaBisOutPut[]>(result);

            });

        }
        #endregion
        /// <summary>
        ///  获取员工预调动数据列表 
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input">查询员工信息条件参数</param>
        /// <returns></returns>
        [HttpPost("Employee/PerTransfer")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<PerEmpTransferOutPut[]>> GetPerEmpTransferQueryableAsync([FromBody]PerQuerInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _employeeManger.GetPerEmpTransferQueryableAsync(input);
                return new ServiceMessage<PerEmpTransferOutPut[]>(result.Result, result.Count);
            });
        }

        //   public Task<bool> AddUpdatePerEmpTransferAsync(PerEmpTransferInput input)


        /// <summary>
        /// 新增或修改预调动记录
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpPost("Employee/AddUpdatePer")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> AddUpdatePerEmpTransferAsync([FromBody]PerEmpTransferInput input)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var result = await _employeeManger.AddUpdatePerEmpTransferAsync(input);

                    return new ServiceMessage<bool>(result);
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
            });
        }



        /// <summary>
        /// 新增或修改预调动记录
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpGet("Employee/UpdatePerState/{Id}/{dataState}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> cancelPerEmpTransferAsync([FromRoute]string Id, [FromRoute]int dataState)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var result = await _employeeManger.cancelPerEmpTransferAsync(Id, dataState);

                    return new ServiceMessage<bool>(result);
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
            });
        }


    }

    public class empCont {

        public int empcount { get; set; }
    }
}
