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
    public class PatientsController : Controller
    {

        private readonly PatientsManager _patientManger;

        public PatientsController(PatientsManager patientManger)
        {


            _patientManger = patientManger;
        }
        /// <summary>
        ///  获取患者数据列表
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input">查询员工信息条件参数</param>
        /// <returns></returns>
        [HttpPost("Patients")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        // [LogRecord("患者信息", LogType.DataAccess)]
        public virtual Task<ServiceMessage<PatientOutPut[]>> GetEmployeeQueryableAsync([FromBody] PatientQueryInPut input)
        {
            return Task.Run(async () =>
            {
                var result = await _patientManger.GetPatientsQueryableAsync(input);
                return new ServiceMessage<PatientOutPut[]>(result.Result, result.Count);
            });
        }

        /// <summary>
        /// 根据年龄段统计患者信息
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetPatientsByAgeList/{centerid}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EmplyeeByStaBisOutPut[]>> GetPatientsByEducationxQueryableAsync(string centerid)
        {
            return Task.Run(async () =>
            {
                var result = await _patientManger.GetPatientByAgeQueryableAsync(centerid);
                return new ServiceMessage<EmplyeeByStaBisOutPut[]>(result);

            });

        }
        /// <summary>
        /// 根据性别统计患者信息
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetPatientsBySexList/{centerid}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EmplyeeByStaBisOutPut[]>> GetPatientBySexQueryableAsync(string centerid)
        {
            return Task.Run(async () =>
            {
                var result = await _patientManger.GetPatientBySexQueryableAsync(centerid);
                return new ServiceMessage<EmplyeeByStaBisOutPut[]>(result);

            });

        }

        /// <summary>
        /// 根据血源性统计患者信息
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetPatientsByBloodBorneList/{centerid}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EmplyeeByStaBisOutPut[]>> GetPatientByBloodBorneQueryableAsync(string centerid)
        {
            return Task.Run(async () =>
            {
                var result = await _patientManger.GetPatientByBloodBorneQueryableAsync(centerid);
                return new ServiceMessage<EmplyeeByStaBisOutPut[]>(result);

            });

        }
        //医保类型 
        /// <summary>
        /// 根据医保类型统计患者信息
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetPatientsBySIInsuredTypeList/{centerid}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EmplyeeByStaBisOutPut[]>> GetPatientBySIInsuredTypeQueryableAsync(string centerid)
        {
            return Task.Run(async () =>
            {
                var result = await _patientManger.GetPatientBySIInsuredTypeQueryableAsync(centerid);
                return new ServiceMessage<EmplyeeByStaBisOutPut[]>(result);

            });

        }

        //GetPatientByHospitalStateQueryableAsync
        //在院状态
        /// <summary>
        /// 根据在院状态统计患者信息
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetPatientByHospitalState/{centerid}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EmplyeeByStaBisOutPut[]>> GetPatientByHospitalStateQueryableAsync(string centerid)
        {
            return Task.Run(async () =>
            {
                var result = await _patientManger.GetPatientByHospitalStateQueryableAsync(centerid);
                return new ServiceMessage<EmplyeeByStaBisOutPut[]>(result);

            });

        }
        //
        /// <summary>
        /// 统计近年患者增长情况
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetPatientsByDateList/{centerid}/{Year}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<LineOutPut>> GetPatientByDateQueryableAsync(string centerid, int Year)
        {
            return Task.Run(async () =>
            {
                var result = await _patientManger.GetPatientByDateQueryableAsync(centerid, Year);
                return new ServiceMessage<LineOutPut>(result);

            });

        }

        #region 病历
        /// <summary>
        /// 门诊血液净化治疗病历首页
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetMedicalRecords/{PatientID}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        // [LogRecord("患者病历", LogType.DataAccess)]
        public virtual Task<ServiceMessage<MedicalRecordsOutPut>> GetMedicalRecords(string PatientID)
        {
            return Task.Run(async () =>
            {
                var result = await _patientManger.GetMedicalRecords(PatientID);
                return new ServiceMessage<MedicalRecordsOutPut>(result);

            });

        }

        /// <summary>
        /// 血液净化专用病历
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpGet("FirstOutpatientRecord/{PatientID}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<OutpatientRecordOutPut>> FirstOutpatientRecord(string PatientID)
        {
            return Task.Run(async () =>
            {
                var result = await _patientManger.GetFirstOutpatientRecord(PatientID);
                return new ServiceMessage<OutpatientRecordOutPut>(result);

            });
        }

        //
        /// <summary>
        /// 门诊专用病历
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetOutpatientMedicalRecord/{PatientID}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<POutpatientMedicalRecordOutPut>> GetOutpatientMedicalRecord(string PatientID)
        {
            return Task.Run(async () =>
            {
                var result = await _patientManger.GetOutpatientMedicalRecord(PatientID);
                return new ServiceMessage<POutpatientMedicalRecordOutPut>(result);

            });
        }

        #endregion


        #region 透析充分性
        //
        /// <summary>
        ///  患者透析充分性查询
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpPost("GetDialysisAdequacyRecord")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<DialysisAdequacyRecord[]>> GetDialysisAdequacyRecord([FromBody] DialysisAdequacyQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _patientManger.GetDialysisAdequacyRecord(inPut);
                return new ServiceMessage<DialysisAdequacyRecord[]>(result);

            });
        }



        /// <summary>
        ///   获取所有检验项目分类配置
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpPost("GetLaboratoryCategorySetting")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<LaboratoryCategorySetting[]>> GetAllLaboratoryCategorySetting()
        {
            return Task.Run(async () =>
            {
                var result = await _patientManger.GetAllLaboratoryCategorySetting();
                return new ServiceMessage<LaboratoryCategorySetting[]>(result);

            });
        }


        // public Task<LaboratoryItemSetting[]> GetLaboratoryItemSettingByCategoryId(string categoryId)
        /// <summary>
        ///  根据检验项目分类查询检验项目配置
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetLaboratoryCategorySetting/{categoryId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<LaboratoryItemSetting[]>> GetLaboratoryItemSettingByCategoryId([FromRoute] string categoryId)
        {
            return Task.Run(async () =>
            {
                var result = await _patientManger.GetLaboratoryItemSettingByCategoryId(categoryId);
                return new ServiceMessage<LaboratoryItemSetting[]>(result);

            });
        }
        // public Task<List<Dictionary<string, string>>> GetInspResultRecordsByPatientIdAndCheckType(PatientInspQueryInPut inPut)

        /// <summary>
        /// 检查结果获取
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("GetInspResultRecords")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<List<Dictionary<string, string>>>> GetInspResultRecordsByPatientIdAndCheckType([FromBody] PatientInspQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _patientManger.GetInspResultRecordsByPatientIdAndCheckType(inPut);
                return new ServiceMessage<List<Dictionary<string, string>>>(result);

            });
        } 

        /// <summary>
        /// 感控检查类别获取
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSensingDataGroupByProjectName")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<List<SensingDataProjectName>>> GetSensingDataGroupByProjectName()
        {
            return Task.Run(() =>
            {

                var result = _patientManger.GetSensingDataGroupByProjectName();
                return new ServiceMessage<List<SensingDataProjectName>>(result);
            });

        }

        /// <summary>
        ///感控结果获取
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("GetSensingDataByProjectName")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<List<Dictionary<string, string>>>> GetSensingDataByProjectName([FromBody] SensingDataQueryInPut inPut)
        {
            return Task.Run(() =>
            {
                var result = _patientManger.GetSensingDataByProjectName(inPut);
                return new ServiceMessage<List<Dictionary<string, string>>>(result);
            }); 
        }


        #endregion

    }
}
