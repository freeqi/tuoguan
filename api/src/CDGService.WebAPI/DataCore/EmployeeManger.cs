﻿﻿using AutoMapper;
using CDGService.Data;
using CDGService.Data.Datas;
using CDGService.Data.Enums;
using CDGService.Data.Store;
using CDGService.Utils;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Dto;
using CDGService.WebAPI.Extenstions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CDGService.Data.Helper;
namespace CDGService.WebAPI.DataCore
{
    /// <summary>
    /// 员工
    /// </summary>
    public class EmployeeManger
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _logManager;
        private readonly IGetUserInfo _getUserInfo;
        private readonly DictionaryCode _dictionaryCode;
        private readonly IMapper _mapper;
        public EmployeeManger(IUnitOfWork unitOfWork, LogManager logManager, IGetUserInfo getUserInfo, IOptions<DictionaryCode> dictionaryCode, IMapper mapper)
        {
            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _dictionaryCode = dictionaryCode.Value;
            _mapper = mapper;
        }
        private IRepository<Employee> EmployeeStore => _unitOfWork.GetStore<Employee>();
        private IRepository<EmployeeTransfer> EmployeeTransferStore => _unitOfWork.GetStore<EmployeeTransfer>();

        private IRepository<PerEmpTransfer> PerEmpTransferStore => _unitOfWork.GetStore<PerEmpTransfer>();


        private IRepository<Patient> PatientStore => _unitOfWork.GetStore<Patient>();
        private IRepository<CenterDialysis> CenterDialysisStore => _unitOfWork.GetStore<CenterDialysis>();

        private IRepository<SystemDictionary> DictionaryStore => _unitOfWork.GetStore<SystemDictionary>();

        private IRepository<EquipmentInfo> EquipmentStore => _unitOfWork.GetStore<EquipmentInfo>();

        /// <summary>
        /// 获取员工列表
        /// </summary>
        public Task<PageData<EmployeeOutPut[]>> GetDialysisQueryableAsync(EmployeeQueryInput input = null)
        {
            return Task.Run(async () =>
            {

                bool IsDel = false;
                EmployeeOutPut[] result = null;
                int count = 0;
                Expression<Func<Employee, bool>> predicate = t => t.IsDelete == IsDel && t.Name != "系统内部人员";
                if (input.DialysisId + "" != "" && input.DialysisId + "" != "0" && input.DialysisId + "" != "1")
                    predicate = predicate.And(t => t.CenterDialysisId == input.DialysisId);

                if (input.DialysisId + "" != "" && input.DialysisId + "" == "1")
                    predicate = predicate.And(t => t.CenterDialysisId + "" == "");
                if (input.EmpName + "" != "")
                    predicate = predicate.And(t => t.Name.Contains(input.EmpName));
                switch (input.IsQureydoctor)
                {

                    case 1: //医生：
                        List<string> doctorDic = new List<string>()
                        { "130e8797c49c4563af7dba1b0cde9d97",
                          "1f02d9d9f9404fb19c3cfdafecbb3ef3",
                          "d8c2c0dfa0e4458e9cedff7f0b08ed22",
                          "21677ce204f8m6c7ad0d8c3a96c3b48e"};
                        predicate = predicate.And(t => doctorDic.Contains(t.positionId));
                        break;

                    case 2:
                        List<string> careDic = new List<string>()
                        { "21677ce204f847c7ad228c3a96c3b48e",
                          "d7ecafb7409b4104855ab9fb0f262bc8",};
                        predicate = predicate.And(t => careDic.Contains(t.positionId));
                        break;

                    case 3:
                        List<string> doctorcareDic = new List<string>()
                        { "130e8797c49c4563af7dba1b0cde9d97",
                          "1f02d9d9f9404fb19c3cfdafecbb3ef3",
                          "d8c2c0dfa0e4458e9cedff7f0b08ed22",
                          "21677ce204f8m6c7ad0d8c3a96c3b48e",
                            "21677ce204f847c7ad228c3a96c3b48e",
                          "d7ecafb7409b4104855ab9fb0f262bc8",};
                        predicate = predicate.And(t => doctorcareDic.Contains(t.positionId));
                        break;
                }

                try
                {
                    if (input.PageNum > 0 && input.pageSize > 0)
                    {
                        var Sum = EmployeeStore.Entities.Include(t => t.CenterDialysis).Include(t => t.Sposition).Include(t => t.SJobTitle).Include(t => t.Sdepartment).Include(t => t.SEducation).Include(t => t.DicWorkingState).Where(predicate).OrderBy(t => t.WorkingState).ThenBy(t => t.CenterDialysisId).ThenByDescending(t => t.NationDoctCode);
                        var persons = await PaginatedList<Employee>.CreateAsync(Sum, input.PageNum, input.pageSize);
                        result = _mapper.Map<EmployeeOutPut[]>(persons);
                        count = persons.Count;
                    }
                    else
                    {
                        var datas = await EmployeeStore.Entities.Include(t => t.CenterDialysis).Include(t => t.Sposition).Include(t => t.SJobTitle).Include(t => t.Sdepartment).Include(t => t.SEducation).Include(t => t.DicWorkingState).Where(predicate).OrderBy(t => t.WorkingState).ThenBy(t => t.CenterDialysisId).ThenByDescending(t=>t.NationDoctCode).ToArrayAsync();
                        result = _mapper.Map<EmployeeOutPut[]>(datas);
                        count = result.Length;

                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

                return new PageData<EmployeeOutPut[]>(result, count);
            });
        }


        /// <summary>
        /// 职工 调动记录
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public Task<EmployeeTransferOutPut[]> GetEmployeeTransfersAsync(string empId)
        {
            return Task.Run(async () =>
            {
                //await AutoEmpTransferAsync();
                List<EmployeeTransferOutPut> result = new List<EmployeeTransferOutPut>();
                var data = await EmployeeTransferStore.Entities.Include(t => t.nowcenterDialysis).Include(t => t.NowPosition).Where(t => t.EmpId == empId).OrderBy(t => t.FounderDate).ToListAsync();
                if (data != null && data.Count > 0)
                {
                    for (int i = 0; i < data.Count - 1; i++)
                    {
                        var item = data[i];
                        var item1 = data[i + 1];
                        string itcen = item.nowcenterDialysis == null ? "集团总部" : item.nowcenterDialysis.ShortName;

                        result.Add(new EmployeeTransferOutPut()
                        {
                            strEmployeeTransfer = $"{item.FounderDate.Value.ToString("yyyy-MM-dd")}至{item1.FounderDate.Value.ToString("yyyy-MM-dd")}{itcen}({item.NowPosition.Name})"
                        });
                    }
                    var LastData = data.LastOrDefault();
                    if (LastData != null)
                    {
                        string itcen1 = LastData.nowcenterDialysis == null ? "集团总部" : LastData.nowcenterDialysis.ShortName;
                        result.Add(new EmployeeTransferOutPut()
                        {
                            strEmployeeTransfer = $"{LastData.FounderDate.Value.ToString("yyyy-MM-dd")}至今{itcen1}({LastData.NowPosition.Name})"
                        });
                    }
                }
                if (data == null || data.Count <= 0)
                {
                    var emp = await EmployeeStore.Entities.Include(t => t.CenterDialysis).Include(t => t.Sposition).Where(t => t.Id == empId).FirstOrDefaultAsync();

                    if (emp != null)
                    {
                        string cen = emp.CenterDialysis == null ? "集团总部" : emp.CenterDialysis.ShortName;
                        DateTime time = emp.hiredate.HasValue ? emp.hiredate.Value : emp.FounderDate.Value;
                        result.Add(new EmployeeTransferOutPut()
                        {
                            strEmployeeTransfer = $"{time.ToString("yyyy-MM-dd")}至今{cen}({emp.Sposition.Name})"
                        });
                    }
                }
                return result.ToArray();
            });
        }




        public Task<PageData<Employee[]>> GetEmpQueryableAsync(EmployeeQueryInput input = null)
        {
            return Task.Run(async () =>
            {

                bool IsDel = false;
                Employee[] result = null;
                int count = 0;
                Expression<Func<Employee, bool>> predicate = t => t.IsDelete == IsDel;
                if (input.DialysisId + "" != "" && input.DialysisId + "" != "0")
                    predicate = predicate.And(t => t.CenterDialysisId == input.DialysisId);
                if (input.EmpName + "" != "")
                    predicate = predicate.And(t => t.Name.Contains(input.EmpName));
                try
                {
                    if (input.PageNum > 0 && input.pageSize > 0)
                    {
                        var Sum = EmployeeStore.Entities.Where(predicate);
                        var persons = await PaginatedList<Employee>.CreateAsync(Sum, input.PageNum, input.pageSize);
                        result = (persons.ToArray());
                        count = Sum.Count();
                    }
                    else
                    {
                        var datas = await EmployeeStore.Entities.Where(predicate).ToArrayAsync();
                        result = (datas);
                        count = result.Length;

                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

                return new PageData<Employee[]>(result, count);
            });
        }
        /// <summary>
        ///  新增/修改员工信息
        ///  Id为0.视为新增
        /// </summary>
        public Task<bool> CreateUpdateEmployeeAsync(EmployeeInput input)
        {
            return Task.Run(async () =>
            {
                bool result = false;
                try
                {
                    //string rolesId = "1"; //只有管理员可以修改
                    // 
                     var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    //var user = await _getUserInfo.GetUserAsync();
                    //var roleIds = user.RoleUsers.Select(y => y.RoleId).ToList();
                    //var subData = roleIds.Where(t => t == rolesId).FirstOrDefault();
                    //if (subData + "" == "") 
                    //    throw new Exception("您暂时没有处理员工基本信息的权限");
                    // 
                    if (input == null)
                        throw new Exception(MessageFormater.PrameterNeedProvider("input"));
                    if (string.IsNullOrEmpty(input.Name))
                        throw new Exception(MessageFormater.PrameterNeedProvider("input.Name"));

                    Employee data = null;
                    if (input.Id + "" != "")
                    {
                        data = await EmployeeStore.GetFirstOrDefaultAsync(t => t.Id == input.Id);
                        if (data == null)
                            throw new Exception(MessageFormater.PrameterNeedExist("id"));
                        if (input.positionId != data.positionId || input.CenterDialysisId != data.CenterDialysisId)
                        {
                            EmployeeTransferStore.Insert(new EmployeeTransfer() { Id = Guid.NewGuid().tostring32(), DataState = 1, EmpId = data.Id, Founder = userId, FounderDate = DateTime.Now, NowCenterId = input.CenterDialysisId, NowPositionId = input.positionId, OriginalCenterId = data.CenterDialysisId, OriginalPositionId = data.positionId, });

                        }
                        //
                        EntityHelper.CoptyPropertys(input, data);
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        if (data.CenterDialysisId == "1" || data.CenterDialysisId == "0")
                            data.CenterDialysisId = "";
                        EmployeeStore.Update(data);

                    }
                    else
                    {
                        data = _mapper.Map<Employee>(input);
                        data.Id = Guid.NewGuid().tostring32();
                        data.DataState = 1;
                        data.Founder = userId;
                        data.FounderDate = DateTime.Now;
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        if (data.CenterDialysisId == "1" || data.CenterDialysisId == "0")
                            data.CenterDialysisId = "";
                        EmployeeStore.Insert(data);
                        EmployeeTransferStore.Insert(new EmployeeTransfer() { Id = Guid.NewGuid().tostring32(), DataState = 1, EmpId = data.Id, Founder = userId, FounderDate = DateTime.Now, NowCenterId = data.CenterDialysisId, NowPositionId = data.positionId, OriginalCenterId = "", OriginalPositionId = "", });

                    }
                    _unitOfWork.SaveChanges();
                    result = true;
                    await
                        _logManager.WriteLogAsync(input.Id + "" != "" ? LogType.DataUpdate : LogType.DataAdded, "员工信息",
                            $"{data.Name},编号为{data.Id}");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return result;
            });
        }
        //
        /// <summary>
        ///根据ID删除员工信息-软删 
        /// </summary>
        /// <param name="id">机构ID</param>
        /// <returns></returns>
        public Task<bool> DeleteDialysisAsync(string id)
        {
            return Task.Run(async () =>
            {
                try
                {
                    string rolesId = "1"; //只有管理员可以修改

                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    var user = await _getUserInfo.GetUserAsync();
                    var roleIds = user.RoleUsers.Select(y => y.RoleId).ToList();
                    var subData = roleIds.Where(t => t == rolesId).FirstOrDefault();
                    if (subData + "" == "")
                        throw new Exception("您暂时没有删除员工基本信息的权限");

                    var data = await EmployeeStore.GetFirstOrDefaultAsync(t => t.Id == id);
                    if (data == null)
                        return true;
                    data.IsDelete = true;
                    data.DataState = 3;
                    data.ModifierDate = DateTime.Now;
                    EmployeeStore.Update(data);
                    _unitOfWork.SaveChanges();
                    await
                       _logManager.WriteLogAsync(LogType.DataDelete, "员工",
                           data.Name);

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }


        private void Test()
        {
            var EmpData = EmployeeStore.GetFirstOrDefault();
            foreach (var item in CenterDialysisStore.Entities.ToList())
            {

                for (int i = 0; i < 500; i++)
                {
                    Employee employee = new Employee();
                    EntityHelper.CoptyProperty(EmpData, employee);
                    employee.Id = Guid.NewGuid().tostring32();
                    employee.Founder = "爸爸";
                    employee.CenterDialysisId = item.Id;
                    employee.FounderDate = new DateTime(2019, 4, 20);
                    EmployeeStore.Insert(employee);
                }
                _unitOfWork.SaveChanges();
            }
            //  _unitOfWork.SaveChanges();
        }



        #region  统计



        /// <summary>
        /// 根据机构统计员工数量
        /// </summary>
        /// <returns></returns>
        public Task<EmplyeeByDiaOutPut[]> GetEmplyeeByDialysisQueryableAsync(int empType)
        {
            return Task.Run(async () =>
            {

                bool IsDel = false;
                List<EmplyeeByDiaOutPut> result = new List<EmplyeeByDiaOutPut>();

                try
                {
                    Expression<Func<Employee, bool>> predicate = t => t.IsDelete == IsDel && t.WorkingState == "90ae6d2b647b4a22b3f8481a1b6c078b";
                    var dialList = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false).OrderBy(t => t.SortNnm).ToListAsync();
                    var emps = await EmployeeStore.Entities.Where(predicate).ToListAsync();
                    var patients = await PatientStore.Entities.Where(t => t.HospitalState == "3f4eaafc1d134659bc323bd251041a74").ToListAsync();
                    var Equipments = await EquipmentStore.Entities.ToListAsync();
                    result.Add(new EmplyeeByDiaOutPut() { DialysisId = "0", DialysisName = "全部", EmlyeeCount = emps.Count(), PatientCount = patients.Count(), EquipCount = Equipments.Count() });
                    if (empType == 1 &&   emps.Where(t => t.CenterDialysisId + "" == "").Count()>0)
                        result.Add(new EmplyeeByDiaOutPut() { DialysisId = "1", DialysisName = "集团总部", EmlyeeCount = emps.Where(t => t.CenterDialysisId + "" == "").Count(), PatientCount = patients.Count(), EquipCount = Equipments.Count() });
                    foreach (var item in dialList)
                    {
                        int empDataCount = emps.Count(t => t.CenterDialysisId == item.Id);
                        var patientscount = patients.Count(t => t.CenterId == item.Id);
                        var Equipmentcount = Equipments.Count(t => t.CenterId == item.Id);
                        result.Add(new EmplyeeByDiaOutPut() { DialysisId = item.Id, DialysisName = item.ShortName, EmlyeeCount = empDataCount, PatientCount = patientscount, EquipCount = Equipmentcount });
                    }

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

                return result.ToArray();
            });
        }

        /// <summary>
        /// 根据职业统计员工
        /// </summary>
        /// <returns></returns>

        public Task<EmplyeeByStaBisOutPut[]> GetEmplyeeByProfessionQueryableAsync(string CenterId)
        {
            return Task.Run(async () =>
            {

                bool IsDel = false;
                List<EmplyeeByStaBisOutPut> result = new List<EmplyeeByStaBisOutPut>();

                try
                {

                    var JobTitleData = await DictionaryStore.Entities.Where(t => t.IsDelete == IsDel && t.TypeId == _dictionaryCode.ProfessionalTypeId).OrderBy(t => t.ShowSortNo).ToListAsync();
                    foreach (var item in JobTitleData)
                    {
                        Expression<Func<Employee, bool>> predicate = t => t.IsDelete == IsDel && t.positionId == item.Id && t.WorkingState == "90ae6d2b647b4a22b3f8481a1b6c078b";
                        if (CenterId != "" && CenterId != "0" && CenterId != "1")
                        {
                            predicate = predicate.And(t => t.CenterDialysisId == CenterId);
                        }
                        if (CenterId != "" && CenterId == "1")
                        {
                            predicate = predicate.And(t => t.CenterDialysisId + "" == "");
                        }
                        int count = await EmployeeStore.Entities.Where(predicate).CountAsync();
                        result.Add(new EmplyeeByStaBisOutPut() { name = item.Name, value = count });
                    }


                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

                return result.ToArray();
            });


        }

        /// <summary>
        /// 性别统计员工
        /// </summary>
        /// <returns></returns>
        public Task<EmplyeeByStaBisOutPut[]> GetEmplyeeBySexQueryableAsync(string CenterId)
        {
            return Task.Run(async () =>
            {

                bool IsDel = false;
                List<EmplyeeByStaBisOutPut> result = new List<EmplyeeByStaBisOutPut>();

                try
                {

                    var JobTitleData = await DictionaryStore.Entities.Where(t => t.IsDelete == IsDel && t.TypeId == _dictionaryCode.SexTypeId).OrderBy(t => t.ShowSortNo).ToListAsync();
                    foreach (var item in JobTitleData)
                    {
                        Expression<Func<Employee, bool>> predicate = t => t.IsDelete == IsDel && t.Sex == item.Id && t.WorkingState == "90ae6d2b647b4a22b3f8481a1b6c078b";
                        if (CenterId != "" && CenterId != "0" && CenterId != "1")
                        {
                            predicate = predicate.And(t => t.CenterDialysisId == CenterId);
                        }
                        if (CenterId != "" && CenterId == "1")
                        {
                            predicate = predicate.And(t => t.CenterDialysisId + "" == "");
                        }
                        int count = await EmployeeStore.Entities.Where(predicate).CountAsync();
                        result.Add(new EmplyeeByStaBisOutPut() { name = item.Name, value = count });
                    }


                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

                return result.ToArray();
            });


        }

        /// <summary>
        /// 职称统计员工
        /// </summary>
        /// <returns></returns>
        public Task<EmplyeeByStaBisOutPut[]> GetEmplyeeByTitlexQueryableAsync(string CenterId)
        {
            return Task.Run(async () =>
            {

                bool IsDel = false;
                List<EmplyeeByStaBisOutPut> result = new List<EmplyeeByStaBisOutPut>();

                try
                {

                    var JobTitleData = await DictionaryStore.Entities.Where(t => t.IsDelete == IsDel && t.TypeId == _dictionaryCode.JobTitleTypeId).OrderBy(t => t.ShowSortNo).ToListAsync();
                    foreach (var item in JobTitleData)
                    {
                        Expression<Func<Employee, bool>> predicate = t => t.IsDelete == IsDel && t.JobTitleId == item.Id && t.WorkingState == "90ae6d2b647b4a22b3f8481a1b6c078b";
                        if (CenterId != "" && CenterId != "0" && CenterId != "1")
                        {
                            predicate = predicate.And(t => t.CenterDialysisId == CenterId);
                        }
                        if (CenterId != "" && CenterId == "1")
                        {
                            predicate = predicate.And(t => t.CenterDialysisId + "" == "");
                        }
                        int count = await EmployeeStore.Entities.Where(predicate).CountAsync();
                        result.Add(new EmplyeeByStaBisOutPut() { name = item.Name, value = count });
                    }


                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

                return result.ToArray();
            });


        }

        /// <summary>
        /// 学历统计员工
        /// </summary>
        /// <returns></returns>
        public Task<EmplyeeByStaBisOutPut[]> GetEmplyeeByEducationxQueryableAsync(string CenterId)
        {
            return Task.Run(async () =>
            {
                // Test();
                bool IsDel = false;
                List<EmplyeeByStaBisOutPut> result = new List<EmplyeeByStaBisOutPut>();

                try
                { 
                    var JobTitleData = await DictionaryStore.Entities.Where(t => t.IsDelete == IsDel && t.TypeId == _dictionaryCode.EduTypeId).OrderBy(t => t.ShowSortNo).ToListAsync();
                    foreach (var item in JobTitleData)
                    {
                        Expression<Func<Employee, bool>> predicate = t => t.IsDelete == IsDel && t.EduId == item.Id && t.WorkingState == "90ae6d2b647b4a22b3f8481a1b6c078b";
                        if (CenterId != "" && CenterId != "0" && CenterId != "1")
                        {
                            predicate = predicate.And(t => t.CenterDialysisId == CenterId);
                        }
                        if (CenterId != "" && CenterId == "1")
                        {
                            predicate = predicate.And(t => t.CenterDialysisId + "" == "");
                        }
                        int count = await EmployeeStore.Entities.Where(predicate).CountAsync();
                        result.Add(new EmplyeeByStaBisOutPut() { name = item.Name, value = count });
                    }


                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

                return result.ToArray();
            });

        }

        #endregion

        #region 预调动

        public Task<PageData<PerEmpTransferOutPut[]>> GetPerEmpTransferQueryableAsync(PerQuerInput input = null)
        {
            return Task.Run(async () =>
            {

                bool IsDel = false;
                PerEmpTransferOutPut[] result = null;
                int count = 0;
                Expression<Func<PerEmpTransfer, bool>> predicate = t => t.DataState != 3;
                if (input.EmpName + "" != "")
                    predicate = predicate.And(t => t.employee.Name.Contains(input.EmpName));
                try
                {
                    if (input.PageNum > 0 && input.pageSize > 0)
                    {
                        var Sum = PerEmpTransferStore.Entities.Include(t => t.employee).Include(t => t.Center).Include(t => t.PosDic).Include(t => t.depDic).Include(t => t.OriginalCenter).Include(t => t.OriginalDep).Include(t => t.OriginalPosition).Where(predicate).OrderByDescending(t => t.TransferDate);
                        var persons = await PaginatedList<PerEmpTransfer>.CreateAsync(Sum, input.PageNum, input.pageSize);
                        result = _mapper.Map<PerEmpTransferOutPut[]>(persons);
                        count = Sum.Count();
                    }
                    else
                    {
                        var datas = await PerEmpTransferStore.Entities.Include(t => t.employee).Include(t => t.Center).Include(t => t.PosDic).Include(t => t.depDic).Include(t => t.OriginalCenter).Include(t => t.OriginalDep).Include(t => t.OriginalPosition).Where(predicate).OrderByDescending(t => t.TransferDate).ToArrayAsync();
                        result = _mapper.Map<PerEmpTransferOutPut[]>(datas);
                        count = result.Length;

                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

                return new PageData<PerEmpTransferOutPut[]>(result, count);
            });
        }

        public Task<bool> AddUpdatePerEmpTransferAsync(PerEmpTransferInput input)
        {

            return Task.Run(async () =>
            {
                bool result = false;
                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input"));
                //if (string.IsNullOrEmpty(input.Name))
                //    throw new Exception(MessageFormater.PrameterNeedProvider("input.Name"));

                try
                {

                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    var empData = await EmployeeStore.GetFirstOrDefaultAsync(t => t.Id == input.EmpId);
                    if (empData == null)
                        throw new Exception("不存在调动人员信息");
                    PerEmpTransfer data = null;
                    input.BackDate = new DateTime(input.BackDate.Year, input.BackDate.Month, input.BackDate.Day, 1, 0, 0);
                    input.TransferDate = new DateTime(input.TransferDate.Year, input.TransferDate.Month, input.TransferDate.Day, 1, 0, 0);
                    if (input.Id + "" != "")
                    {
                        data = await PerEmpTransferStore.GetFirstOrDefaultAsync(t => t.Id == input.Id);
                        if (data == null)
                            throw new Exception(MessageFormater.PrameterNeedExist("id"));

                        EntityHelper.CoptyProperty(input, data);
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        PerEmpTransferStore.Update(data);

                    }
                    else
                    {
                        data = _mapper.Map<PerEmpTransfer>(input);
                        data.Id = Guid.NewGuid().tostring32();
                        data.OriginalCenterId = empData.CenterDialysisId;
                        data.OriginalDepId = empData.DepId;
                        data.OriginalPositionId = empData.positionId;
                        data.DataState = 1;
                        data.Founder = userId;
                        data.FounderDate = DateTime.Now;
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        //if (data.BackDate.Value != data.TransferDate)
                        //    data.BackDate = data.TransferDate;
                        PerEmpTransferStore.Insert(data);

                    }
                    _unitOfWork.SaveChanges();

                    //    await AutoEmpTransferAsync();
                    result = true;
                    await
                        _logManager.WriteLogAsync(input.Id + "" != "" ? LogType.DataUpdate : LogType.DataAdded, "员工调动信息",
                            $"{empData.Name},编号为{data.Id}");

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return result;
            });
        }

        /// <summary>
        /// 取消调动或者删除调动记录
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="dataState"></param>
        /// <returns></returns>
        public Task<bool> cancelPerEmpTransferAsync(string Id, int dataState)
        {

            return Task.Run(async () =>
            {
                bool result = false;
                if (Id == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("Id"));
                //if (string.IsNullOrEmpty(input.Name))
                //    throw new Exception(MessageFormater.PrameterNeedProvider("input.Name"));

                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();

                    PerEmpTransfer data = null;

                    data = await PerEmpTransferStore.GetFirstOrDefaultAsync(t => t.Id == Id);
                    if (data == null)
                        throw new Exception(MessageFormater.PrameterNeedExist("id"));

                    if (data.IsTransfer)
                        throw new Exception("该员工已经执行完调动了， 不可撤销或删除");

                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    data.DataState = dataState;
                    PerEmpTransferStore.Update(data);
                    _unitOfWork.SaveChanges();
                    result = true;

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return result;
            });
        }


        /// <summary>
        /// 自动执行预调动信息
        /// </summary>
        /// <returns></returns>
        public Task<bool> AutoEmpTransferAsync()
        {
            return Task.Run(async () =>
            {
                bool Flag = true;
                //764dafb2c1aa59482f583812b07e4234b2a488fa890fa0b5
                await _getUserInfo.SetUserInfo("764dafb2c1aa59482f583812b07e4234b2a488fa890fa0b5");
                DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 1, 0, 0);
                //查询有效且没调动且调动日期为今日的数据
                var EmpTransferData = await PerEmpTransferStore.Entities.Include(t => t.employee).Where(t => t.IsTransfer == false && t.DataState == 1 && t.TransferDate == dateTime).ToListAsync();
                //调出
                foreach (var item in EmpTransferData)
                {
                    //    EntityHelper.CoptyPropertys(input, data); 
                    EmployeeInput employee = new EmployeeInput();
                    EntityHelper.CoptyPropertys(item.employee, employee);
                    employee.DepId = item.PresentDepId;
                    employee.positionId = item.PresentPositionId;
                    employee.CenterDialysisId = item.PresentCenterId;
                    await CreateUpdateEmployeeAsync(employee);
                    item.IsTransfer = true;
                    item.DataState = 1;
                    PerEmpTransferStore.Update(item);
                    await _unitOfWork.SaveChangesAsync();
                }
                //调回
                var BackEmpTransferData = await PerEmpTransferStore.Entities.Include(t => t.employee).Where(t => t.DataState == 1 && t.BackDate > t.TransferDate && t.BackDate == dateTime && t.IsTransfer == true).ToListAsync();

                foreach (var item in BackEmpTransferData)
                {
                    //    EntityHelper.CoptyPropertys(input, data); 
                    EmployeeInput employee = new EmployeeInput();
                    EntityHelper.CoptyPropertys(item.employee, employee);
                    employee.DepId = item.OriginalDepId;
                    employee.positionId = item.OriginalPositionId;
                    employee.CenterDialysisId = item.OriginalCenterId;
                    await CreateUpdateEmployeeAsync(employee);
                    //item.IsTransfer = true;
                    //item.DataState = 1;
                    //PerEmpTransferStore.Update(item);
                    //await _unitOfWork.SaveChangesAsync();
                }
                await _getUserInfo.SetUserInfo("");
                return Flag;

            });
        }


        #endregion


    }
}
