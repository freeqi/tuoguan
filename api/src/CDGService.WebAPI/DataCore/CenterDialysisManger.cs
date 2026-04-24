﻿﻿using AutoMapper;
using CDGService.Data.Datas;
using CDGService.Data.Enums;
using CDGService.Data.Store;
using CDGService.Utils;
using CDGService.WebAPI.Datas;
using CDGService.Data.Helper;
using CDGService.WebAPI.Dto;
using CDGService.WebAPI.Extenstions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace CDGService.WebAPI.DataCore
{
    /// <summary>
    /// 透析中心
    /// </summary>
    public class CenterDialysisManger
    {
        private readonly Data.DocumentSetting _documentSetting;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _logManager;
        private readonly IGetUserInfo _getUserInfo;
        private readonly IMapper _mapper;
        public CenterDialysisManger(IUnitOfWork unitOfWork, LogManager logManager, IGetUserInfo getUserInfo, IOptions<Data.DocumentSetting> documentSetting, IMapper mapper)
        {

            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _documentSetting = documentSetting.Value;
            _mapper = mapper;
        }
        private IRepository<CenterDialysis> DialysisStore => _unitOfWork.GetStore<CenterDialysis>();
        private IRepository<SysRegion> SysRegionStore => _unitOfWork.GetStore<SysRegion>();
        private IRepository<Employee> EmployeeStore => _unitOfWork.GetStore<Employee>();
        private IRepository<EquipmentInfo> EquipmentInfoStore => _unitOfWork.GetStore<EquipmentInfo>();
        private IRepository<Patient> PatientStore => _unitOfWork.GetStore<Patient>();



        /// <summary>
        ///  新增/修改血透中心
        ///  Id为0.视为新增
        /// </summary>
        public Task<bool> CreateUpdateDialysisAsync(CenterDialysisInput input)
        {
            return Task.Run(async () =>
            {
                bool result = false;
                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("透析中心信息"));
                if (string.IsNullOrEmpty(input.DialysisName))
                    throw new Exception(MessageFormater.PrameterNeedProvider("透析中心名称"));

                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    CenterDialysis data = null;
                    if (input.Id + "" != "" && input.Id + "" != "0")
                    {
                        data = await DialysisStore.GetFirstOrDefaultAsync(t => t.Id == input.Id);
                        if (data == null)
                            throw new Exception(MessageFormater.PrameterNeedExist("id"));
                        EntityHelper.CoptyProperty(input, data);
                        data.ModifyMan = userId;
                        data.ModifyTime = DateTime.Now;
                        data.DialysisImg = "";
                        data.DialysisImg1 = "";
                        data.DialysisImg2 = "";
                        data.DialysisImg3 = "";

                        data.DialysisImg4 = "";
                        int i = 0;
                        foreach (var item in input.DialysisImg)
                        {
                            string ImagePath = Path.Combine(_documentSetting.ImageRoot, "透析中心", Guid.NewGuid().tostring16() + i + ".jpg");
                            switch (i)
                            {
                                case 0:
                                    data.DialysisImg = ImageHelper.Base64StringToImage1(item, ImagePath);
                                    break;
                                case 1:
                                    data.DialysisImg1 = ImageHelper.Base64StringToImage1(item, ImagePath);
                                    break;
                                case 2:
                                    data.DialysisImg2 = ImageHelper.Base64StringToImage1(item, ImagePath);
                                    break;
                                case 3:
                                    data.DialysisImg3 = ImageHelper.Base64StringToImage1(item, ImagePath);
                                    break;
                                case 4:
                                    data.DialysisImg4 = ImageHelper.Base64StringToImage1(item, ImagePath);
                                    break;
                                default:
                                    break;
                            }
                            i++;

                        }

                        DialysisStore.Update(data);
                    }
                    else
                    {

                        data = _mapper.Map<CenterDialysis>(input);
                        data.Id = Guid.NewGuid().tostring32();
                        data.AddMan = userId;
                        bool flag = true;
                        while (flag)
                        {
                            data.DialysisCode = ValueHelper.TimeNowRandom('C');
                            var Codedata = await DialysisStore.GetFirstOrDefaultAsync(t => t.DialysisCode == data.DialysisCode);
                            if (Codedata == null)
                                flag = false;
                        }

                        data.AddTime = DateTime.Now;
                        data.ModifyMan = userId;
                        data.ModifyTime = DateTime.Now;
                        int i = 0;
                        foreach (var item in input.DialysisImg)
                        {
                            string ImagePath = Path.Combine(_documentSetting.ImageRoot, "透析中心", Guid.NewGuid().tostring16() + i + ".jpg");
                            switch (i)
                            {
                                case 0:
                                    data.DialysisImg = ImageHelper.Base64StringToImage1(item, ImagePath);
                                    break;
                                case 1:
                                    data.DialysisImg1 = ImageHelper.Base64StringToImage1(item, ImagePath);
                                    break;
                                case 2:
                                    data.DialysisImg2 = ImageHelper.Base64StringToImage1(item, ImagePath);
                                    break;
                                case 3:
                                    data.DialysisImg3 = ImageHelper.Base64StringToImage1(item, ImagePath);
                                    break;
                                case 4:
                                    data.DialysisImg4 = ImageHelper.Base64StringToImage1(item, ImagePath);
                                    break;
                                default:
                                    break;
                            }
                            i++;

                        }
                        DialysisStore.Insert(data);
                    }
                    _unitOfWork.SaveChanges();
                    result = true;
                    await
                        _logManager.WriteLogAsync(input.Id + "" != "" && input.Id + "" != "0" ? LogType.DataUpdate : LogType.DataAdded, "透析中心",
                            $"编号为{data.Id}");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return result;
            });
        }

        /// <summary>
        ///根据ID删除血透中心-软删
        /// </summary>
        /// <param name="id">机构ID</param>
        /// <returns></returns>
        public Task<bool> DeleteDialysisAsync(string id)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var data = await DialysisStore.GetFirstOrDefaultAsync(t => t.Id == id);
                    if (data == null)
                        return true;
                    if (data.CenterUrl + "" != "")
                        throw new Exception("当前机构正式运行中，不可删除");
                    data.IsDelete = true;
                    DialysisStore.Update(data);
                    _unitOfWork.SaveChanges();
                    await
                        _logManager.WriteLogAsync(LogType.DataDelete, "透析中心",
                            data.ShortName);

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }


        /// <summary>
        /// 获取透析中心列表
        /// </summary>
        public Task<PageData<CenterDialysisOutPut[]>> GetDialysisQueryableAsync(DialysisSearchInput input = null)
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                CenterDialysisOutPut[] result = null;
                int count = 0;

                try
                {
                    var UserData = await EmployeeStore.Entities.ToListAsync();
                    if (input == null || input.PageNum <= 0 || input.PageSize <= 0)
                    {
                        var datas = await DialysisStore.Entities.Include(t => t.DialysisRegions).Include(t => t.DialysisContactMan).Where(t => t.IsDelete == IsDel).OrderBy(t => t.SortNnm).ToArrayAsync();

                        result = _mapper.Map<CenterDialysisOutPut[]>(datas);
                        count = result.Length;
                        int i = 0;
                        foreach (var item in result)
                        {
                            item.DialysisRegionID.Clear();
                            GetManId(datas[i].DialysisRegionID, item.DialysisRegionID);
                            if (item.HeadNurseId + "" != "")
                                item.HeadNurseMan = UserData.FirstOrDefault(t => t.Id == item.HeadNurseId).Name;

                            if (!string.IsNullOrEmpty(item.CenterWebURL))
                            {
                                item.CenterWebuser = "swsgroup".Encrypt();
                                item.CenterWebpwd = StringHelper.GetRandomPwd(item.DialysisCode).Encrypt();
                            }
                            i++;
                        }
                    }
                    else
                    {
                        Expression<Func<CenterDialysis, bool>> predicate = t => t.IsDelete == IsDel;
                        if (!string.IsNullOrEmpty(input.DialysisName))
                            predicate = predicate.And(t => t.DialysisName.Contains(input.DialysisName));


                        if (input.PageNum > 0 && input.PageSize > 0)
                        {
                            var Sum = DialysisStore.Entities.Include(t => t.DialysisRegions).Include(t => t.DialysisContactMan).OrderBy(t => t.SortNnm).Where(predicate);
                            var Dialysis = await PaginatedList<CenterDialysis>.CreateAsync(Sum, input.PageNum, input.PageSize);
                            // result = _mapper.Map<CenterDialysisOutPut[]>(Dialysis);
                            count = Sum.Count();

                            result = _mapper.Map<CenterDialysisOutPut[]>(Dialysis);
                            int i = 0;
                            foreach (var item in result)
                            {
                                item.DialysisRegionID.Clear();
                                GetManId(Dialysis[i].DialysisRegionID, item.DialysisRegionID);
                                if (item.HeadNurseId + "" != "")
                                    item.HeadNurseMan = UserData.FirstOrDefault(t => t.Id == item.HeadNurseId).Name;
                                if (!string.IsNullOrEmpty(item.CenterWebURL))
                                {
                                    item.CenterWebuser = "swsgroup".Encrypt();
                                    item.CenterWebpwd = StringHelper.GetRandomPwd(item.DialysisCode).Encrypt();
                                }
                                i++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return new PageData<CenterDialysisOutPut[]>(result, count);
            });
        }

        /// <summary>
        /// 获取透析中心列表
        /// </summary>
        public Task<CenterDialysis[]> GetDialysisQueryableByIdsAsync(List<string> Ids)
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                CenterDialysis[] datas = null;
                try
                {
                    datas = await DialysisStore.Entities.Where(t => t.IsDelete == IsDel && Ids.Contains(t.Id)).OrderBy(t => t.SortNnm).ToArrayAsync();
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
                return datas;
            });
        }

        /// <summary>
        /// 获取透析中心下拉列表
        /// </summary>
        /// <returns></returns>
        public Task<CenterListOutPut[]> GetCenterListOutPuts()
        {
            return Task.Run(async () =>
            {
                var data = await DialysisStore.Entities.Where(t => t.IsDelete == false).ToArrayAsync();

                return _mapper.Map<CenterListOutPut[]>(data);
            });
        }



        /// <summary>
        /// 获取透析中心地图标记列表
        /// </summary>
        public Task<DialysisMapOutPut[]> GetDialysisQueryableAsync()
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                DialysisMapOutPut[] result = null;

                try
                {
                    var datas = await DialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToArrayAsync();
                    result = _mapper.Map<DialysisMapOutPut[]>(datas);


                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return result;
            });
        }


        /// <summary>
        /// 根据ID获取透析中心详情
        /// </summary>
        public Task<CenterDialysisOutPut[]> GetDialysisByiIdAsync(string id)
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                CenterDialysisOutPut[] result = null;

                try
                {
                    if (id == "")
                    {
                        throw new Exception(MessageFormater.PrameterNeedProvider("id"));
                    }
                    else
                    {
                        Expression<Func<CenterDialysis, bool>> predicate = t => t.IsDelete == IsDel;
                        if (id + "" != "")
                            predicate = predicate.And(t => t.Id == id);
                        var data = await DialysisStore.Entities.Include(t => t.DialysisRegions).Include(t => t.DialysisContactMan).Where(predicate).ToArrayAsync();
                        result = _mapper.Map<CenterDialysisOutPut[]>(data);
                        int i = 0;
                        foreach (var item in result)
                        {
                            item.DialysisRegionID.Clear();
                            GetManId(data[i].DialysisRegionID, item.DialysisRegionID);

                            i++;
                        }

                        if (result == null || result.Length <= 0)
                        {
                            throw new Exception(MessageFormater.PrameterResultsIsNull("查询透析中心详情"));
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return result;
            });
        }
        public void GetManId(string id, List<string> listId)
        {
            try
            {
                var data = SysRegionStore.Entities.Where(t => t.Id == id).FirstOrDefault();
                if (data != null)
                {
                    listId.Insert(0, data.Id);
                    if (data.RegionParentId + "" != "" && data.RegionParentId + "" != "0")
                    {
                        GetManId(data.RegionParentId, listId);
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }
        }


        /// <summary>
        /// 统计-地区
        /// </summary> 
        /// <returns></returns>
        public Task<DialysisStatisticalByCityOutPut[]> DialysisStatisticalByCityAsync()
        {
            return Task.Run(async () =>
            {
                bool IsDel = false;
                List<DialysisStatisticalByCityOutPut> result1 = new List<DialysisStatisticalByCityOutPut>();
                List<DialysisStatisticalByCityOutPut> result = new List<DialysisStatisticalByCityOutPut>();
                try
                {
                    var datas = await DialysisStore.Entities.Include(t => t.DialysisRegions).Where(t => t.IsDelete == IsDel).ToArrayAsync();
                    var GroupData = datas.GroupBy(t => t.DialysisRegionID);
                    List<string> list = new List<string>();
                    foreach (var item in GroupData)
                    {
                        if (item.Key == null)
                            continue;
                        GetManId(item.Key, list);
                        string name = SysRegionStore.Entities.Where(t => t.Id == list[0]).FirstOrDefault().RegionName;

                        double? nmi = ValueHelper.ToRound((1.0 * item.Count()) / (1.0 * datas.Count()) * 100, 1);
                        result.Add(new DialysisStatisticalByCityOutPut() { DialysisCount = item.Count(), RegionId = list[0], RetionName = name, Percentage = nmi + "%" });
                    }
                    var GroupData1 = result.GroupBy(t => t.RegionId);

                    foreach (var item in GroupData1)
                    {
                        double? nmi = ValueHelper.ToRound((1.0 * item.Sum(t => t.DialysisCount)) / (1.0 * datas.Count()) * 100, 1);
                        result1.Add(new DialysisStatisticalByCityOutPut() { DialysisCount = item.Sum(t => t.DialysisCount), RegionId = item.Key, RetionName = item.FirstOrDefault().RetionName, Percentage = nmi + "%" });
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

                return result1.ToArray();
            });
        }


        /// <summary>
        /// 统计-运行时间  
        /// </summary> 
        /// <returns></returns>
        public Task<DialysisStatisticalByYearOutPut[]> DialysisStatisticalByYearAsync()
        {
            return Task.Run(async () =>
            {
                bool IsDel = false;
                List<DialysisStatisticalByYearOutPut> result = new List<DialysisStatisticalByYearOutPut>();
                try
                {
                    var datas = await DialysisStore.Entities.Where(t => t.IsDelete == IsDel && t.SetUpDate.HasValue).ToArrayAsync();
                    var GroupData = datas.GroupBy(t => t.SetUpDate.Value.Year).OrderBy(t => t.Key);
                    int count = 0;
                    foreach (var item in GroupData)
                    {
                        //if (item.Key == null)
                        //    continue;
                        result.Add(new DialysisStatisticalByYearOutPut() { DialysisCount = item.Count() + count, Year = item.Key, growth = item.Count() });
                        count += item.Count();
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
                return result.ToArray();
            });
        }

        public Task<AllTotal> GetTotalAsync()
        {
            return Task.Run(async () =>
             {
                 bool Flag = false;
                 string workState = "e81a52d7e88e49a28dd536548ca8fe8a";//离职
                 string eqType = "285b4938d09d425d9bf2f5b0c6c657c1";//透析设备
                 string hState = "3f4eaafc1d134659bc323bd251041a74";//在院/转入
                 AllTotal allTotal = new AllTotal()
                 {
                     CenterCount = await DialysisStore.Entities.CountAsync(t => t.IsDelete == Flag),
                     EmpCount = await EmployeeStore.Entities.CountAsync(t => t.IsDelete == Flag && t.WorkingState != workState),
                     MachineCount = await EquipmentInfoStore.Entities.CountAsync(t => t.IsDelete == Flag && t.EquipType == eqType),
                     patientCount = await PatientStore.Entities.CountAsync(t =>  t.HospitalState == hState),

                 };

                 return allTotal;
             });

        }


        //中心端获取机构详细信息（采集）

        public Task<OrganizationInfo> GetDialysisAsync(string id)
        {
            return Task.Run(async () =>
            {
                OrganizationInfo Info = new OrganizationInfo();

                var data = await DialysisStore.Entities.Include(t => t.DialysisContactMan).Where(t => t.Id == id).FirstOrDefaultAsync();
                if (data != null)
                {
                    Info = new OrganizationInfo()
                    {
                        Id = data.Id,
                        Address = data.DialysisAddress,
                        AddrX = data.DialysisLng + "",
                        AddrY = data.DialysisLat + "",
                        Code = data.DialysisCode,
                        DataState = 1,
                        LxTel = data.DialysisPhone,
                        Name = data.DialysisName,
                        PersonLiable = data.DialysisContactMan != null ? data.DialysisContactMan.Name : "",
                        Remark = data.DialysisDetails,
                        NationalCode = data.NationalCode,
                        NationalName = data.NationalName
                    };
                }
                return Info;
            });
        }

    }
}
