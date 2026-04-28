﻿﻿using AutoMapper;
using Castle.Core.Internal;
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
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CDGService.Data.Helper;
namespace CDGService.WebAPI.DataCore
{
    /// <summary>
    /// 文档及信息管理类
    /// </summary>
    public class DocumentManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Data.DocumentSetting _documentSetting;
        private readonly LogManager _logManager;
        private readonly IGetUserInfo _getUserInfo;
        private readonly DictionaryCode _dictionaryCode;
        private readonly IMapper _mapper;
        /// <summary>
        /// 
        /// </summary>
        public DocumentManager(IUnitOfWork unitOfWork, IOptions<Data.DocumentSetting> documentSetting, LogManager logManager, IGetUserInfo getUserInfo, EmployeeManger employeeManger, IOptions<DictionaryCode> dictionaryCode, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _documentSetting = documentSetting.Value;
            _logManager = logManager;
            _getUserInfo = getUserInfo;
            _dictionaryCode = dictionaryCode.Value;
            _mapper = mapper;
        }

        private IRepository<MedicalUnit> MedicalUnitStore => _unitOfWork.GetStore<MedicalUnit>();
        private IRepository<Employee> EmployeeStore => _unitOfWork.GetStore<Employee>();
        private IRepository<WarehouseCatalog> WarehouseCatalogStore => _unitOfWork.GetStore<WarehouseCatalog>();
        private IRepository<CenterDialysis> CenterDialysisStore => _unitOfWork.GetStore<CenterDialysis>();

        private IRepository<MedicalItemRecord> MedicalItemRecordStore => _unitOfWork.GetStore<MedicalItemRecord>();
        private IRepository<MedicalDrugExtension> MedicalDrugExtensionStore => _unitOfWork.GetStore<MedicalDrugExtension>();
        private IRepository<SystemDictionary> SystemDictionaryStore => _unitOfWork.GetStore<SystemDictionary>();
        private IRepository<Document> DocumentStore => _unitOfWork.GetStore<Document>();
        private IRepository<SysRegion> SysRegionStore => _unitOfWork.GetStore<SysRegion>();
        private IRepository<Supplier> SupplierStore => _unitOfWork.GetStore<Supplier>();
        private IRepository<DosageForm> DosageFormStore => _unitOfWork.GetStore<DosageForm>();
        private IRepository<TempTab> TempTabStore => _unitOfWork.GetStore<TempTab>();
        /// <summary>
        /// 文档-新增
        /// </summary>
        public Task<bool> CreateDocAsync(DocumentInput input)
        {
            if (input == null)
                throw new Exception(MessageFormater.PrameterNeedProvider("文件上传信息"));
            return Task.Run(async () =>
            {
                var userId = await _getUserInfo.GetCurrentUserIdAsync();
                var data = _mapper.Map<Document>(input);
                data.EntryTime = DateTime.Now;
                data.LastModifyTime = data.EntryTime;
                data.PublishPersonId = userId;
                data.PublishTime = DateTime.Now;
                data.FileUrl = Path.Combine(_documentSetting.DocumentRoot, input.Catalog.ToChinese(), input.FileName);
                data.Id = Guid.NewGuid().tostring32();
                data.DataState = 1;
                DocumentStore.Insert(data);
                _unitOfWork.SaveChanges();
                await _logManager.WriteLogAsync(LogType.DataAdded, "文档", $"编号为{data.Id}");
                return true;
            });

        }


        /// <summary>
        /// 文档-列表
        /// </summary>
        public Task<PageData<DocumentOutput[]>> GetDocQueryableAsync(DocumentSearchInput input)
        {
            return Task.Run(async () =>
            {
                DocumentOutput[] result = null;
                int count = 0;
                try
                {
                    Expression<Func<Document, bool>> predicate = t => !t.IsPublish;
                    if (input == null)
                    {
                        var datas = await DocumentStore.Entities.Include(t => t.user).ThenInclude(t => t.Employee).Where(predicate).ToArrayAsync();
                        result = _mapper.Map<DocumentOutput[]>(datas);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(input.FileName))
                            predicate = predicate.And(t => t.Title.Contains(input.FileName));
                        if (input.Id + "" != "" && input.Id + "" != "0")
                            predicate = predicate.And(t => t.Id == input.Id);
                        if (input.Catalog > 0)
                            predicate = predicate.And(t => t.Catalog == input.Catalog);
                        if (input.knowledgeType > 0)
                            predicate = predicate.And(t => t.knowledgeType == input.knowledgeType);
                        var datas = DocumentStore.Entities.Where(predicate).Include(t => t.user).ThenInclude(t => t.Employee);
                        if (input.PageNum > 0 && input.PageSize > 0)
                        {

                            var persons = await PaginatedList<Document>.CreateAsync(datas, input.PageNum, input.PageSize);
                            result = _mapper.Map<DocumentOutput[]>(persons);
                            count = datas.Count();
                        }
                        else
                        {
                            result = _mapper.Map<DocumentOutput[]>(datas);
                            count = result.Length;

                        }

                        // result = _mapper.Map<DocumentOutput[]>(datas);
                    }
                    return new PageData<DocumentOutput[]>(result, count);
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

            });
        }


        public Task<DocKnowledgeOutPut[]> GetDocKnowledgeQueryableAsync(DocumentCatalog documentCatalog)
        {

            return Task.Run(async () =>
            {
                List<DocKnowledgeOutPut> result = new List<DocKnowledgeOutPut>();
                int count = 0;
                try
                {
                    Expression<Func<Document, bool>> predicate = t => !t.IsPublish;
                    predicate = predicate.And(t => t.Catalog == documentCatalog);
                    var datas = await DocumentStore.Entities.Where(predicate).ToArrayAsync();
                    var GroupData = datas.GroupBy(t => t.knowledgeType);

                    switch (documentCatalog)
                    {
                        case DocumentCatalog.KnowledgeFlie:
                            for (int i = 1; i < 4; i++)
                            {
                                result.Add(new DocKnowledgeOutPut() { count = datas.Where(t => t.knowledgeType == (KnowledgeType)i).Count(), value = (KnowledgeType)i, label = ((KnowledgeType)i).ToChinese() });
                            }

                            break;
                        case DocumentCatalog.InstitutionFile:
                            for (int i = 4; i < 7; i++)
                            {
                                result.Add(new DocKnowledgeOutPut() { count = datas.Where(t => t.knowledgeType == (KnowledgeType)i).Count(), value = (KnowledgeType)i, label = ((KnowledgeType)i).ToChinese() });
                            }
                            break;
                        case DocumentCatalog.QualificationFlie:
                            for (int i = 7; i < 10; i++)
                            {
                                result.Add(new DocKnowledgeOutPut() { count = datas.Where(t => t.knowledgeType == (KnowledgeType)i).Count(), value = (KnowledgeType)i, label = ((KnowledgeType)i).ToChinese() });
                            }
                            break;
                        //case DocumentCatalog.CultivateFlie:
                        //    for (int i = 1; i < 4; i++)
                        //    {
                        //        result.Add(new DocKnowledgeOutPut() { count = datas.Where(t => t.knowledgeType == (KnowledgeType)i).Count(), value = (KnowledgeType)i, label = ((KnowledgeType)i).ToChinese() });
                        //    }
                        //    break;

                        default:
                            break;
                    }
                    //foreach (KnowledgeType suit in Enum.GetValues(typeof(KnowledgeType)))
                    //{


                    //}
                    //foreach (var item in GroupData)
                    //{
                    //    result.Add(new DocKnowledgeOutPut() { count = item.Count(), value = item.Key, label = item.Key.ToChinese() });
                    //}


                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
                return result.ToArray();
            });

        }

        /// <summary>
        /// 文档-修改
        /// </summary>
        public Task<bool> ModifyDocAsync(DocumentModifyInput input)
        {
            if (input == null)
                throw new Exception(MessageFormater.PrameterNeedProvider("资料信息"));
            return Task.Run(async () =>
            {
                var oldData = await DocumentStore.GetFirstOrDefaultAsync(t => t.Id == input.Id);
                if (oldData == null)
                    throw new Exception(MessageFormater.PrameterNeedExist("id"));
                var oldConverUrl = oldData.CoverUrl;

                try
                {
                    if (input.download)
                        oldData.DownloadCount += 1;
                    if (input.Title + "" != "")
                        oldData.Title = input.Title;
                    if (input.Note + "" != "")
                        oldData.Note = input.Note;

                    oldData.LastModifyTime = DateTime.UtcNow;
                    DocumentStore.Update(oldData);
                    _unitOfWork.SaveChanges();
                    if (!input.download)
                        await _logManager.WriteLogAsync(LogType.DataUpdate, "文档", $"编号为{oldData.Id}");


                }
                catch (Exception ex)
                {
                    return false;
                }

                return true;
            });

        }

        /// <summary>
        /// 文档-删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> DelDocAsync(string id)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var data = await DocumentStore.GetFirstOrDefaultAsync(t => t.Id == id);
                    if (data == null)
                        return true;
                    data.IsDelete = true;
                    data.DataState = 3;
                    DocumentStore.Update(data);
                    _unitOfWork.SaveChanges();
                    await
                       _logManager.WriteLogAsync(LogType.DataDelete, "文档",
                           $"编号为{data.Id}");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }



        #region  导入Excel


        public Task<bool> UpLoadExcelImport(string FilePath, ImportCatalog import)
        {
            return Task.Run(async () =>
            {
                DataTable table;
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    switch (import)
                    {
                        case ImportCatalog.Emp:

                            table = ExcelHelper.ReadExcel(FilePath, 3);
                            var emp = TableToemp(table);
                            //   var data = (_mapper.Map<Employee[]>(emp)).ToList();  
                            foreach (var item in emp)
                            {
                                if (item.Name == "")
                                {
                                    continue;
                                }
                                Employee employee = _mapper.Map<Employee>(item);
                                employee.Id = Guid.NewGuid().tostring32();
                                employee.Founder = userId;
                                employee.FounderDate = DateTime.Now;
                                employee.Modifier = userId;
                                employee.ModifierDate = DateTime.Now;
                                //透析中心
                                var center = await CenterDialysisStore.GetFirstOrDefaultAsync(t => (t.DialysisName.Contains(item.CenterDialysisId.Substring(0, 2)) || t.ShortName.Contains(item.CenterDialysisId.Substring(0, 2))) && t.IsDelete == false);
                                if (center != null)
                                {
                                    employee.CenterDialysisId = center.Id;
                                    item.DepId = "无";
                                }
                                else
                                {

                                    if (item.CenterDialysisId.Contains("透析中心") || item.CenterDialysisId.Contains("透析门诊部") || item.CenterDialysisId.Contains("慈济中医院"))
                                    {
                                        string guid = Guid.NewGuid().tostring32();
                                        CenterDialysisStore.Insert(new CenterDialysis() { DialysisName = item.CenterDialysisId, DataState = 1, Id = guid });
                                        employee.CenterDialysisId = guid;
                                        item.DepId = "";
                                    }
                                    else
                                    {
                                        employee.CenterDialysisId = "";
                                    }
                                }

                                //部门   
                                item.DepId = item.DepId == "" ? "无" : item.DepId;
                                var eipid = await SystemDictionaryStore.GetFirstOrDefaultAsync(t => t.TypeId == _dictionaryCode.DepTypeId && t.Name == item.DepId);
                                if (eipid != null)
                                {
                                    employee.DepId = eipid.Id;
                                    // employee.DepId = "无";
                                }
                                else
                                {
                                    string guid = Guid.NewGuid().tostring32();
                                    SystemDictionaryStore.Insert(new SystemDictionary() { Name = item.DepId, DataState = 1, Id = guid, TypeId = _dictionaryCode.DepTypeId });
                                    employee.DepId = guid;
                                }
                                //学历
                                item.EduId = item.EduId == "" ? "无" : item.EduId;
                                var xueli = await SystemDictionaryStore.GetFirstOrDefaultAsync(t => t.TypeId == _dictionaryCode.EduTypeId && t.Name == item.EduId);
                                if (xueli != null)
                                {
                                    employee.EduId = xueli.Id;
                                    // employee.DepId = "无";
                                }
                                else
                                {
                                    string guid = Guid.NewGuid().tostring32();
                                    SystemDictionaryStore.Insert(new SystemDictionary() { Name = item.EduId, DataState = 1, Id = guid, TypeId = _dictionaryCode.EduTypeId });
                                    employee.EduId = guid;
                                }
                                //职称
                                item.JobTitleId = item.JobTitleId == "" || item.JobTitleId == "/" ? "无" : item.JobTitleId;
                                var JobTitle = await SystemDictionaryStore.GetFirstOrDefaultAsync(t => t.TypeId == _dictionaryCode.JobTitleTypeId && t.Name == item.JobTitleId);
                                if (JobTitle != null)
                                {
                                    employee.JobTitleId = JobTitle.Id;
                                    // employee.DepId = "无";
                                }
                                else
                                {
                                    string guid = Guid.NewGuid().tostring32();
                                    SystemDictionaryStore.Insert(new SystemDictionary() { Name = item.JobTitleId, DataState = 1, Id = guid, TypeId = _dictionaryCode.JobTitleTypeId });
                                    employee.JobTitleId = guid;
                                }
                                //职位
                                item.positionId = item.positionId == "" ? "无" : item.positionId;
                                var Professional = await SystemDictionaryStore.GetFirstOrDefaultAsync(t => t.TypeId == _dictionaryCode.ProfessionalTypeId && t.Name == item.positionId);
                                if (Professional != null)
                                {
                                    employee.positionId = Professional.Id;
                                    // employee.DepId = "无";
                                }
                                else
                                {
                                    string guid = Guid.NewGuid().tostring32();
                                    SystemDictionaryStore.Insert(new SystemDictionary() { Name = item.positionId, Id = guid, DataState = 1, TypeId = _dictionaryCode.ProfessionalTypeId });
                                    employee.positionId = guid;
                                }

                                //性别
                                item.Sex = item.Sex == "" ? "无" : item.Sex;
                                var Sex = await SystemDictionaryStore.GetFirstOrDefaultAsync(t => t.TypeId == _dictionaryCode.SexTypeId && t.Name == item.Sex);
                                if (Sex != null)
                                {
                                    employee.Sex = Sex.Id;
                                    // employee.DepId = "无";
                                }
                                else
                                {
                                    string guid = Guid.NewGuid().tostring32();
                                    SystemDictionaryStore.Insert(new SystemDictionary() { Name = item.Sex, Id = guid, DataState = 1, TypeId = _dictionaryCode.SexTypeId });
                                    employee.Sex = guid;
                                }
                                employee.WorkingState = SystemDictionaryStore.GetFirstOrDefaultAsync(t => t.TypeId == _dictionaryCode.WorkStateTypeId && t.Name == "在职").Result.Id;


                                EmployeeStore.Insert(employee);
                                _unitOfWork.SaveChanges();
                            }

                            break;
                        case ImportCatalog.Centher:
                            table = ExcelHelper.ReadExcel(FilePath, 2);
                            var dialysisInputs = TableToCenter(table);
                            foreach (var item in dialysisInputs)
                            {
                                if (item.DialysisName == "")
                                {
                                    continue;
                                }
                                string cenderid = Guid.NewGuid().tostring32();
                                CenterDialysis Newcenter = _mapper.Map<CenterDialysis>(item);
                                Newcenter.Id = cenderid;
                                // Newcenter.DialysisCode = ValueHelper.TimeNowRandom('C');
                                Newcenter.AddMan = userId;
                                Newcenter.AddTime = DateTime.Now;
                                Newcenter.ModifyMan = userId;
                                Newcenter.ModifyTime = DateTime.Now;
                                //透析中心
                                var center = await CenterDialysisStore.GetFirstOrDefaultAsync(t => t.DialysisName == item.DialysisName);

                                var main = await EmployeeStore.GetFirstOrDefaultAsync(t => t.Name == item.DialysisContactManID);
                                string mainId = "";
                                cenderid = center == null ? cenderid : center.Id;
                                var region = await SysRegionStore.GetFirstOrDefaultAsync(t => t.RegionName == item.DialysisRegionID);
                                if (item.DialysisContactManID != "")
                                {
                                    if (main == null)
                                    {
                                        mainId = Guid.NewGuid().tostring32();
                                        EmployeeStore.Insert(new Employee() { Id = mainId, Name = item.DialysisContactManID });
                                    }
                                    else
                                        mainId = main.Id;
                                }
                                if (center != null)
                                {
                                    EntityHelper.CoptyProperty(Newcenter, center);
                                    center.DialysisContactManID = mainId;
                                    center.ModifyMan = userId;
                                    center.ModifyTime = DateTime.Now;
                                    center.DialysisRegionID = region.Id;
                                    CenterDialysisStore.Update(center);
                                }
                                else
                                {
                                    Newcenter.DialysisContactManID = mainId;
                                    Newcenter.DialysisRegionID = region.Id;
                                    bool flag = true;
                                    while (flag)
                                    {
                                        Newcenter.DialysisCode = ValueHelper.TimeNowRandom('C');
                                        var Codedata = await CenterDialysisStore.GetFirstOrDefaultAsync(t => t.DialysisCode == Newcenter.DialysisCode);
                                        if (Codedata == null)
                                            flag = false;
                                    }
                                    CenterDialysisStore.Insert(Newcenter);
                                }
                                _unitOfWork.SaveChanges();

                            }
                            break;
                        case ImportCatalog.Spplier:



                            await TableToSupplier(FilePath);


                            break;
                        case ImportCatalog.Material:
                            await MedicalsItem(FilePath);
                            break;
                        case ImportCatalog.Drug:
                            await DrugsItem(FilePath);
                            break;
                        case ImportCatalog.Diagnosis:
                            await DiagnosisItem(FilePath);

                            break;
                        case ImportCatalog.FixedAssets:
                            await FixedAssetsItem(FilePath);
                            break;
                        case ImportCatalog.Low://低值
                            await LowsItem(FilePath);
                            break;

                        default:


                            break;
                    }


                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return true;
            });

        }






        /// <summary>
        /// 员工
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private List<EmployeeInput> TableToemp(DataTable table)
        {

            List<EmployeeInput> employees = new List<EmployeeInput>();
            try
            {
                foreach (var item in table.Rows.Cast<DataRow>())
                {
                    EmployeeInput employee = new EmployeeInput
                    {
                        Age = string.IsNullOrEmpty(item["年龄"].ToString()) ? 0 : Convert.ToInt32(item["年龄"].ToString()),
                        AssignmentStatus = item["外派合同情况"].ToString(),
                        Birthday = string.IsNullOrEmpty(item["出生日期"].ToString()) || item["出生日期"].ToString() + "" == "/" ? null : (DateTime?)DateTime.Parse(item["出生日期"].ToString()),
                        CenterDialysisId = item["部门"].ToString(),//所在机构
                        ContactAddress = item["现居住地"].ToString(),
                        DataState = 1,
                        DepId = item["部门"].ToString(),//部门
                        EducationBackground = item["最高学历"].ToString(),//最高学历
                        EduId = item["学历"].ToString(),//学历
                        EmergencyPhone = item["紧急联系电话"].ToString(),
                        Ethnic = item["民族"].ToString(),
                        ExperienceJob = item["工作经历1"].ToString(),
                        ExperienceJobTwo = item["工作经历2"].ToString(),
                        expireDate = string.IsNullOrEmpty(item["合同到期时间"].ToString()) || item["合同到期时间"].ToString() + "" == "/" ? null : (DateTime?)DateTime.Parse(item["合同到期时间"].ToString()),
                        GraduateSchool = item["毕业院校（现有毕业证）"].ToString(),
                        GraduationDate = string.IsNullOrEmpty(item["毕业时间"].ToString()) || item["毕业时间"].ToString() + "" == "/" ? null : (DateTime?)DateTime.Parse(item["毕业时间"].ToString()),
                        highestSchool = item["毕业院校（最高）"].ToString(),
                        hiredate = string.IsNullOrEmpty(item["入职日期"].ToString()) || item["入职日期"].ToString() + "" == "/" ? null : (DateTime?)DateTime.Parse(item["入职日期"].ToString()),
                        HjAddress = item["户口所在地"].ToString(),
                        HouseholdRegister = item["户口性质"].ToString(),
                        Idcard = item["身份证号码"].ToString(),
                        // IsDelete = false,
                        JobYear = string.IsNullOrEmpty(item["血透经验年限"].ToString()) ? 0 : Convert.ToDecimal(item["血透经验年限"].ToString()),
                        JobTitleId = item["职称"].ToString(),//职称ID
                        LaborContract = item["劳动合同关系"].ToString(),
                        Major = item["专业"].ToString(),
                        Name = item["姓名"].ToString(),
                        Phone = item["联系方式"].ToString(),
                        positionId = item["职务"].ToString(),//职位 
                        positiveDate = string.IsNullOrEmpty(item["转正日期"].ToString()) || item["转正日期"].ToString() + "" == "/" ? null : (DateTime?)DateTime.Parse(item["转正日期"].ToString()),
                        Recommendations = item["举荐人"].ToString(),
                        RecommendationsPhone = item["联系电话"].ToString(),
                        Sex = item["性别"].ToString(),
                        signDate = string.IsNullOrEmpty(item["合同签订时间"].ToString()) || item["合同签订时间"].ToString() + "" == "/" ? null : (DateTime?)DateTime.Parse(item["合同签订时间"].ToString()),
                        startSchool = item["毕业院校（起点）"].ToString(),
                        //Founder = userId.Result,
                        //FounderDate = DateTime.Now,
                        //Modifier = userId.Result,
                        //ModifierDate = DateTime.Now,
                    };
                    employees.Add(employee);

                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }

            return employees;
        }
        /// <summary>
        /// 透析中心
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private List<CenterDialysisInput> TableToCenter(DataTable table)
        {
            List<CenterDialysisInput> employees = new List<CenterDialysisInput>();
            try
            {
                foreach (var item in table.Rows.Cast<DataRow>())
                {
                    CenterDialysisInput employee = new CenterDialysisInput
                    {
                        DataState = 1,
                        DialysisAddress = item["详细地址"].ToString(),
                        DialysisCode = ValueHelper.TimeNowRandom('C'),
                        DialysisDetails = item["机构简介"].ToString(),
                        DialysisName = item["机构名称"].ToString(),
                        DialysisPhone = item["联系电话"].ToString(),
                        RunDate = string.IsNullOrEmpty(item["运行时间"].ToString()) ? null : (DateTime?)DateTime.Parse(item["运行时间"].ToString()),
                        SetUpDate = string.IsNullOrEmpty(item["成立时间"].ToString()) ? null : (DateTime?)DateTime.Parse(item["成立时间"].ToString()),
                        DialysisContactManID = item["负责人"].ToString(),
                        DialysisRegionID = item["所在地区"].ToString(),

                    };
                    employees.Add(employee);

                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }

            return employees;
        }

        /// <summary>
        /// 供应商
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private Task<bool> TableToSupplier(string FilePath)
        {
            return Task.Run(async () =>
             {
                 //  List<Supplier> employees = new List<Supplier>();
                 try
                 {

                     string UserId = _getUserInfo.GetUserAsync().Result.Id;
                     var table = ExcelHelper.ReadExcel(FilePath, 1, 0);
                     foreach (var item in table.Rows.Cast<DataRow>())
                     {
                         var data = await SupplierStore.GetFirstOrDefaultAsync(t => t.Name == item["供应商名称"].ToString() && t.IsDelete == false);
                         if (data != null)
                         {
                             data.DataState = 1;
                             data.Address = item["联系地址"].ToString();
                             data.SupCode = item["供应商编码"].ToString();
                             data.Name = item["供应商名称"].ToString();
                             data.LegalPerson = item["法人代表"].ToString();
                             data.MainBusiness = item["主营业务"].ToString();
                             data.LinkMan = item["联系人"].ToString();
                             data.Phone = item["联系电话"].ToString();
                             data.Remark = item["备注"].ToString();
                             data.Modifier = UserId;
                             data.ModifierDate = DateTime.Now;
                             SupplierStore.Update(data);
                         }
                         else
                         {
                             Supplier employee = new Supplier
                             {
                                 Id = Guid.NewGuid().tostring32(),
                                 DataState = 1,
                                 Address = item["联系地址"].ToString(),
                                 SupCode = item["供应商编码"].ToString(),
                                 Name = item["供应商名称"].ToString(),
                                 LegalPerson = item["法人代表"].ToString(),
                                 MainBusiness = item["主营业务"].ToString(),
                                 LinkMan = item["联系人"].ToString(),
                                 Phone = item["联系电话"].ToString(),
                                 Remark = item["备注"].ToString(),
                                 // MainCategories = item["主营类别"].ToString().Split(','),
                                 Founder = UserId,
                                 FounderDate = DateTime.Now,
                                 Modifier = UserId,
                                 ModifierDate = DateTime.Now,
                                 SysCode = ValueHelper.TimeNowRandom('S')

                             };
                             SupplierStore.Insert(employee);
                         }
                     }

                     await _unitOfWork.SaveChangesAsync();
                 }
                 catch (Exception ex)
                 {

                     throw new Exception(ex.Message, ex);
                 }

                 return true;
             });


            //  return employees;

        }


        /// <summary>
        /// 诊疗项目
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private List<MedicalItemRecordInput> TableToDiagnosis(DataTable table)
        {
            List<MedicalItemRecordInput> medicalItemRecordInputs = new List<MedicalItemRecordInput>();
            try
            {
                foreach (var item in table.Rows.Cast<DataRow>())
                {

                    if (!string.IsNullOrWhiteSpace
                        (item["打包项目名称"].ToString()) && medicalItemRecordInputs.FindAll(t => t.MedicalItemName == item["打包项目名称"].ToString()).Count <= 0)
                    {
                        if (medicalItemRecordInputs.FirstOrDefault(T => T.MedicalItemName == item["打包项目名称"].ToString()) != null)
                        { continue; }
                        MedicalItemRecordInput zlxm = new MedicalItemRecordInput
                        {
                            Id = Guid.NewGuid().tostring32(),
                            DataState = 1,
                            ParentId = "0",
                            MedicalItemName = item["打包项目名称"].ToString(),
                            WareHouseId = item["诊疗类别"].ToString(),
                            MedicalItemWorkCode = item["项目编码"].ToString(),
                            Mnemonic = item["助记码"].ToString(),
                            MinDose = (string.IsNullOrEmpty(item["打包价格"].ToString()) ? 0 : Convert.ToDecimal(item["打包价格"].ToString())) + "元/" + item["单位"].ToString(),
                            Specifications = item["单位"].ToString(),
                            FeeTypeId = item["费用类别"].ToString(),
                            Usage = item["内容"].ToString(),
                            MonthDosage = string.IsNullOrEmpty(item["患者单次可用次数"].ToString()) ? 0 : Convert.ToInt32(item["患者单次可用次数"].ToString()),//item["患者单次可用次数"].ToString(),
                            Remark = item["备注"].ToString(),
                            MedicalDrugExtension = new MedicalDrugExtensionInput() { Id = Guid.NewGuid().tostring32(), CenterId = "0", AgreementPrice = 0, GocGuidePrice = 0, IsCurrentUse = true, PurchasingPrice = 0, ReferencePrice = 0, RetailPrice = string.IsNullOrEmpty(item["打包价格"].ToString()) ? 0 : Convert.ToDecimal(item["打包价格"].ToString()), SocialSecurityPrice = 0 }

                        };
                        medicalItemRecordInputs.Add(zlxm);
                    }


                    MedicalItemRecordInput employee = new MedicalItemRecordInput
                    {
                        Id = Guid.NewGuid().tostring32(),
                        DataState = 1,
                        ParentId = item["打包项目名称"].ToString(),
                        MedicalItemName = item["项目名称"].ToString(),
                        WareHouseId = item["诊疗类别"].ToString(),
                        MedicalItemWorkCode = item["项目编码"].ToString(),
                        Mnemonic = item["助记码"].ToString(),
                        MinDose = (string.IsNullOrEmpty(item["单价"].ToString()) ? 0 : Convert.ToDecimal(item["单价"].ToString())) + "元/" + item["单位"].ToString(),
                        Specifications = item["单位"].ToString(),
                        FeeTypeId = item["费用类别"].ToString(),
                        Usage = item["内容"].ToString(),
                        MonthDosage = string.IsNullOrEmpty(item["患者单次可用次数"].ToString()) ? 0 : Convert.ToInt32(item["患者单次可用次数"].ToString()),//item["患者单次可用次数"].ToString(),
                        Remark = item["备注"].ToString(),
                        MedicalDrugExtension = new MedicalDrugExtensionInput() { Id = Guid.NewGuid().tostring32(), CenterId = "0", AgreementPrice = 0, GocGuidePrice = 0, IsCurrentUse = true, PurchasingPrice = 0, ReferencePrice = 0, RetailPrice = string.IsNullOrEmpty(item["单价"].ToString()) ? 0 : Convert.ToDecimal(item["单价"].ToString()), SocialSecurityPrice = 0 }

                    };
                    medicalItemRecordInputs.Add(employee);

                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }

            return medicalItemRecordInputs;
        }


        /// <summary>
        ///药品
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private List<MedicalItemRecordInput> TableToDrugs(DataTable table)
        {
            List<MedicalItemRecordInput> medicalItemRecordInputs = new List<MedicalItemRecordInput>();
            try
            {
                foreach (var item in table.Rows.Cast<DataRow>())
                {
                    MedicalItemRecordInput Drug = new MedicalItemRecordInput
                    {
                        DataState = 1,
                        MedicalItemName = item["药品名称"].ToString(),
                        GoodsName = item["商品名称"].ToString(),
                        AliasName = item["别名"].ToString(),
                        Brand = item["品牌/型号"].ToString(),
                        EnglishName = item["英文名称"].ToString(),
                        //WareHouseId = item["诊疗类别"].ToString(),
                        MedicalItemWorkCode = item["药品编码"].ToString(),
                        Mnemonic = item["助记码"].ToString(),
                        medicalItemTypeEnum = MedicalItemTypeEnum.D,
                        Manufacturer = item["生产厂家"].ToString(),
                        SupplierId = item["供货商"].ToString(),
                        Forms = item["剂型"].ToString(),
                        MinDose = item["规格"].ToString(),
                        Specifications = item["规格单位"].ToString(),
                        PackageUnit = item["包装单位"].ToString(),
                        Packaging = item["包装规格"].ToString(),
                        ProcurementPackage = item["包装规格"].ToString(),
                        //item["采购包装"].ToString(),
                        ProcurementUnit = item["包装单位"].ToString(),
                        //item["采购单位"].ToString(),
                        PackageSpecifications = 1,
                        DoseMin = string.IsNullOrEmpty(item["最小剂量"].ToString()) ? 1 : Convert.ToDecimal(item["最小剂量"].ToString()),
                        DoseUnit = item["剂量单位"].ToString(),
                        SpecificationsQuantity = string.IsNullOrEmpty(item["包装拆分规则"].ToString()) && Convert.ToInt32(item["包装拆分规则"].ToString()) <= 1 ? 1 : Convert.ToInt32(item["包装拆分规则"].ToString()),
                        FeeTypeId = item["费用类别"].ToString(),
                        Usage = item["用法用量"].ToString(),
                        MonthDosage = string.IsNullOrEmpty(item["患者自然月可用量"].ToString()) ? 0 : Convert.ToInt32(item["患者自然月可用量"].ToString()),//item["患者单次可用次数"].ToString(),
                        Remark = item["备注"].ToString(),
                        IsSalesReturn = true,
                        IsSplited = (string.IsNullOrEmpty(item["包装拆分规则"].ToString()) && Convert.ToInt32(item["包装拆分规则"].ToString()) <= 1) ? false : true,



                        MedicalDrugExtension = new MedicalDrugExtensionInput() { Id = Guid.NewGuid().tostring32(), AgreementPrice = 0, GocGuidePrice = 0, IsCurrentUse = true, PurchasingPrice = string.IsNullOrEmpty(item["采购价"].ToString()) ? 0 : Convert.ToDecimal(item["采购价"].ToString()), ReferencePrice = 0, RetailPrice = string.IsNullOrEmpty(item["销售价"].ToString()) ? 0 : Convert.ToDecimal(item["销售价"].ToString()), SocialSecurityPrice = string.IsNullOrEmpty(item["社保指导价"].ToString()) ? 0 : Convert.ToDecimal(item["社保指导价"].ToString()) }

                    };
                    medicalItemRecordInputs.Add(Drug);

                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }




            return medicalItemRecordInputs;


        }


        /// <summary>
        ///卫材
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private List<MedicalItemRecordInput> TableToMaterial(DataTable table)
        {
            List<MedicalItemRecordInput> medicalItemRecordInputs = new List<MedicalItemRecordInput>();
            try
            {
                foreach (var item in table.Rows.Cast<DataRow>())
                {
                    MedicalItemRecordInput Material = new MedicalItemRecordInput
                    {
                        DataState = 1,
                        MedicalItemName = item["耗材名称"].ToString(),
                        // GoodsName = item["商品名称"].ToString(),
                        //AliasName = item["别名"].ToString(),
                        Brand = item["品牌/型号"].ToString(),
                        EnglishName = item["英文名称"].ToString(),
                        //WareHouseId = item["诊疗类别"].ToString(),
                        MedicalItemWorkCode = item["耗材编码"].ToString(),
                        Mnemonic = item["助记码"].ToString(),
                        medicalItemTypeEnum = MedicalItemTypeEnum.U,
                        Manufacturer = item["生产厂家"].ToString(),
                        SupplierId = item["供货商"].ToString(),
                        //  Forms = item["剂型"].ToString(),
                        MinDose = item["规格"].ToString(),
                        Specifications = item["规格单位"].ToString(),
                        PackageUnit = item["包装单位"].ToString(),
                        Packaging = item["包装规格"].ToString(),
                        // DoseMin = string.IsNullOrEmpty(item["最小剂量"].ToString()) ? 1 : Convert.ToDecimal(item["最小剂量"].ToString()),
                        // DoseUnit = item["剂量单位"].ToString(),
                        SpecificationsQuantity = string.IsNullOrEmpty(item["包装拆分规则"].ToString()) && Convert.ToInt32(item["包装拆分规则"].ToString()) <= 1 ? 1 : Convert.ToInt32(item["包装拆分规则"].ToString()),
                        FeeTypeId = item["费用类别"].ToString(),
                        //Usage = item["用法用量"].ToString(),
                        // MonthDosage = string.IsNullOrEmpty(item["患者自然月可用量"].ToString()) ? 0 : Convert.ToInt32(item["患者自然月可用量"].ToString()),//item["患者单次可用次数"].ToString(),
                        Remark = item["备注"].ToString(),
                        IsSalesReturn = true,
                        IsSplited = (string.IsNullOrEmpty(item["包装拆分规则"].ToString()) && Convert.ToInt32(item["包装拆分规则"].ToString()) <= 1) ? false : true,

                        MedicalDrugExtension = new MedicalDrugExtensionInput() { Id = Guid.NewGuid().tostring32(), AgreementPrice = 0, GocGuidePrice = 0, IsCurrentUse = true, PurchasingPrice = string.IsNullOrEmpty(item["采购价"].ToString()) ? 0 : Convert.ToDecimal(item["采购价"].ToString()), ReferencePrice = 0, RetailPrice = string.IsNullOrEmpty(item["销售价"].ToString()) ? 0 : Convert.ToDecimal(item["销售价"].ToString()), SocialSecurityPrice = string.IsNullOrEmpty(item["社保指导价"].ToString()) ? 0 : Convert.ToDecimal(item["社保指导价"].ToString()) }

                    };
                    medicalItemRecordInputs.Add(Material);

                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }




            return medicalItemRecordInputs;


        }

        /// <summary>
        ///固定资产
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private List<MedicalItemRecordInput> TableToFixedAssets(DataTable table)
        {
            List<MedicalItemRecordInput> medicalItemRecordInputs = new List<MedicalItemRecordInput>();
            try
            {
                foreach (var item in table.Rows.Cast<DataRow>())
                {
                    MedicalItemRecordInput Material = new MedicalItemRecordInput
                    {
                        DataState = 1,
                        MedicalItemName = item["资产名称"].ToString(),
                        // GoodsName = item["商品名称"].ToString(),
                        //AliasName = item["别名"].ToString(),
                        Brand = item["品牌/型号"].ToString(),
                        //  EnglishName = item["英文名称"].ToString(),
                        //WareHouseId = item["诊疗类别"].ToString(),
                        MedicalItemWorkCode = item["资产编码"].ToString(),
                        Mnemonic = item["助记码"].ToString(),
                        medicalItemTypeEnum = MedicalItemTypeEnum.G,
                        Manufacturer = item["生产厂家"].ToString(),
                        SupplierId = item["供货商"].ToString(),
                        //  Forms = item["剂型"].ToString(),
                        //MinDose = item["规格"].ToString(),
                        //Specifications = item["规格单位"].ToString(),
                        PackageUnit = item["包装单位"].ToString(),
                        // Packaging = item["包装规格"].ToString(),
                        // DoseMin = string.IsNullOrEmpty(item["最小剂量"].ToString()) ? 1 : Convert.ToDecimal(item["最小剂量"].ToString()),
                        // DoseUnit = item["剂量单位"].ToString(),
                        //SpecificationsQuantity = string.IsNullOrEmpty(item["包装拆分规则"].ToString()) ? 1 : Convert.ToInt32(item["包装拆分规则"].ToString()),
                        //FeeTypeId = item["费用类别"].ToString(),
                        //Usage = item["用法用量"].ToString(),
                        // MonthDosage = string.IsNullOrEmpty(item["患者自然月可用量"].ToString()) ? 0 : Convert.ToInt32(item["患者自然月可用量"].ToString()),//item["患者单次可用次数"].ToString(),
                        Remark = item["备注"].ToString(),
                        IsSalesReturn = true,
                        //  IsSplited = (string.IsNullOrEmpty(item["包装拆分规则"].ToString()) ? 1 : Convert.ToInt32(item["包装拆分规则"].ToString())) == 1 ? false : true,



                        MedicalDrugExtension = new MedicalDrugExtensionInput() { Id = Guid.NewGuid().tostring32(), AgreementPrice = 0, GocGuidePrice = 0, IsCurrentUse = true, PurchasingPrice = string.IsNullOrEmpty(item["采购价"].ToString()) ? 0 : Convert.ToDecimal(item["采购价"].ToString()), ReferencePrice = 0, RetailPrice = 0, SocialSecurityPrice = 0 }

                    };
                    medicalItemRecordInputs.Add(Material);

                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }




            return medicalItemRecordInputs;


        }

        /// <summary>
        ///低值
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private List<MedicalItemRecordInput> TableToLow(DataTable table)
        {
            DataRow row = null;
            int i = 0;
            List<MedicalItemRecordInput> medicalItemRecordInputs = new List<MedicalItemRecordInput>();
            try
            {
                foreach (var item in table.Rows.Cast<DataRow>())
                {
                    i++;
                    row = item;
                    if (item[0] + "" == "")
                        continue;
                    MedicalItemRecordInput Material = new MedicalItemRecordInput
                    {
                        DataState = 1,
                        MedicalItemName = item["物品名称"].ToString(),
                        // GoodsName = item["商品名称"].ToString(),
                        //AliasName = item["别名"].ToString(),
                        Brand = item["品牌/型号"].ToString(),
                        EnglishName = item["英文名称"].ToString(),
                        //WareHouseId = item["诊疗类别"].ToString(),
                        MedicalItemWorkCode = item["物品编码"].ToString(),
                        Mnemonic = item["助记码"].ToString(),
                        medicalItemTypeEnum = MedicalItemTypeEnum.U,
                        Manufacturer = item["生产厂家"].ToString(),
                        SupplierId = item["供货商"].ToString(),
                        //  Forms = item["剂型"].ToString(),
                        MinDose = item["规格"].ToString(),
                        Specifications = item["规格单位"].ToString(),
                        PackageUnit = item["包装单位"].ToString(),
                        Packaging = item["包装规格"].ToString(),
                        // DoseMin = string.IsNullOrEmpty(item["最小剂量"].ToString()) ? 1 : Convert.ToDecimal(item["最小剂量"].ToString()),
                        // DoseUnit = item["剂量单位"].ToString(),
                        SpecificationsQuantity = string.IsNullOrEmpty(item["包装拆分规则"].ToString()) && Convert.ToInt32(item["包装拆分规则"].ToString()) <= 1 ? 1 : Convert.ToInt32(item["包装拆分规则"].ToString()),
                        FeeTypeId = item["费用类别"].ToString(),
                        //Usage = item["用法用量"].ToString(),
                        // MonthDosage = string.IsNullOrEmpty(item["患者自然月可用量"].ToString()) ? 0 : Convert.ToInt32(item["患者自然月可用量"].ToString()),//item["患者单次可用次数"].ToString(),
                        Remark = item["备注"].ToString(),
                        IsSalesReturn = true,
                        IsSplited = (string.IsNullOrEmpty(item["包装拆分规则"].ToString()) && Convert.ToInt32(item["包装拆分规则"].ToString()) <= 1) ? false : true,

                        MedicalDrugExtension = new MedicalDrugExtensionInput() { Id = Guid.NewGuid().tostring32(), AgreementPrice = 0, GocGuidePrice = 0, IsCurrentUse = true, PurchasingPrice = string.IsNullOrEmpty(item["采购价"].ToString()) ? 0 : Convert.ToDecimal(item["采购价"].ToString()), ReferencePrice = 0, RetailPrice = string.IsNullOrEmpty(item["销售价"].ToString()) ? 0 : Convert.ToDecimal(item["销售价"].ToString()), SocialSecurityPrice = 0 }

                    };
                    medicalItemRecordInputs.Add(Material);

                }
            }
            catch (Exception ex)
            {
                var data = row;
                int k = i;
                throw new Exception(ex.Message, ex);
            }




            return medicalItemRecordInputs;


        }

        /// <summary>
        /// 导入诊疗项目
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        private Task<bool> DiagnosisItem(string FilePath)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    var table = ExcelHelper.ReadExcel(FilePath, 3);
                    var ItemInputs = TableToDiagnosis(table);

                    var lodMed = await MedicalItemRecordStore.Entities.Where(t => t.MedicalItemType == 5 && t.DataState == 1 && t.IsDelete == false).ToListAsync();
                    foreach (var item in ItemInputs)
                    {
                        if (item.MedicalItemName == "")
                        {
                            continue;
                        }


                        var temMed = lodMed.Where(t => t.MedicalItemName == item.MedicalItemName).FirstOrDefault();
                        if (temMed != null)
                        {
                            temMed.ParentId = "0";
                            var Mainmed = lodMed.Where(t => t.MedicalItemName == item.ParentId).FirstOrDefault();
                            if (Mainmed != null)
                            {
                                temMed.ParentId = Mainmed.Id;
                            }
                            else if (item.ParentId != "" && item.ParentId != "0")
                            {
                                temMed.ParentId = ItemInputs.First(t => t.MedicalItemName == item.ParentId).Id;
                            }
                            MedicalItemRecordStore.Update(temMed);
                            _unitOfWork.SaveChanges();
                            continue;
                        }

                        //  item.Id = Itemid;
                        MedicalItemRecord NewItem = _mapper.Map<MedicalItemRecord>(item);
                        // NewItem.Id = Itemid;
                        // Newcenter.DialysisCode = ValueHelper.TimeNowRandom('C');

                        if (item.ParentId != "" && item.ParentId != "0")
                        {
                            NewItem.ParentId = ItemInputs.First(t => t.MedicalItemName == item.ParentId).Id;
                        }
                        NewItem.Founder = userId;
                        NewItem.FounderDate = DateTime.Now;
                        NewItem.Modifier = userId;
                        NewItem.ModifierDate = DateTime.Now;
                        NewItem.SpecificationsQuantity = 1;
                        List<string> Main = new List<string>();
                        string WareHouseIds = WarehouseCatalogStore.Entities.Where(t => t.Name == item.WareHouseId).FirstOrDefault().Id;
                        GetManId(WareHouseIds, Main);
                        NewItem.WareHouseIds = WareHouseIds;
                        NewItem.WareHouseParentIds = string.Join(",", Main.ToArray());
                        NewItem.MedicalItemType = 5;
                        // 

                        //单位  
                        item.Specifications = item.Specifications == "" ? "无" : item.Specifications;
                        var unitdata = await MedicalUnitStore.GetFirstOrDefaultAsync(t => t.SpeUnitCHS == item.Specifications);

                        if (unitdata != null)
                        {
                            NewItem.SpecificationsUnit = unitdata.Id;
                            // employee.DepId = "无";
                        }
                        else
                        {
                            string guid = Guid.NewGuid().tostring32();
                            MedicalUnitStore.Insert(new MedicalUnit() { SpeUnitCHS = item.Specifications, DataState = 1, Id = guid, IsDelete = false, UnitType = 4, FounderDate = DateTime.Now, ModifierDate = DateTime.Now, Founder = userId, Modifier = userId });
                            NewItem.SpecificationsUnit = guid;
                        }

                        //费用类别                         
                        item.FeeTypeId = item.FeeTypeId == "" ? "无" : item.FeeTypeId;
                        var FeeTypeId = await SystemDictionaryStore.GetFirstOrDefaultAsync(t => t.TypeId == _dictionaryCode.FeeTypeId && t.Name == item.FeeTypeId);
                        if (FeeTypeId != null)
                        {
                            NewItem.FeeTypeId = FeeTypeId.Id;
                            // employee.DepId = "无";
                        }
                        else
                        {
                            string guid = Guid.NewGuid().tostring32();
                            SystemDictionaryStore.Insert(new SystemDictionary() { Name = item.FeeTypeId, DataState = 1, Id = guid, TypeId = _dictionaryCode.FeeTypeId, FounderDate = DateTime.Now, ModifierDate = DateTime.Now, Founder = userId, Modifier = userId });
                            NewItem.FeeTypeId = guid;
                        }
                        NewItem.Specifications = item.MinDose;
                        // var Item = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.MedicalItemName == item.MedicalItemName && t.MedicalItemType == 5); 
                        NewItem.Id = item.Id;// Item == null ? Itemid : Item.Id;

                        //if (Item != null)
                        //{
                        //    var Specifications = Item.Specifications;
                        //    EntityHelper.CoptyProperty(NewItem, Item);
                        //    Item.SpecificationsUnit = Item.PackageUnit = item.ProcurementUnit = NewItem.SpecificationsUnit;
                        //    Item.Specifications = Item.ProcurementPackage = Item.Packaging = item.MinDose;
                        //    MedicalItemRecordStore.Update(Item);
                        //    var _MedicalDrugExtensione = await MedicalDrugExtensionStore.GetFirstOrDefaultAsync(t => t.MedicalId == Item.Id && t.IsCurrentUse);
                        //    if (_MedicalDrugExtensione != null
                        //    && _MedicalDrugExtensione.PurchasingPrice == item.MedicalDrugExtension.PurchasingPrice
                        //    && _MedicalDrugExtensione.AgreementPrice == item.MedicalDrugExtension.AgreementPrice
                        //    && _MedicalDrugExtensione.RetailPrice == item.MedicalDrugExtension.RetailPrice
                        //    && _MedicalDrugExtensione.ReferencePrice == item.MedicalDrugExtension.ReferencePrice
                        //    && _MedicalDrugExtensione.GocGuidePrice == item.MedicalDrugExtension.GocGuidePrice
                        //    && _MedicalDrugExtensione.SocialSecurityPrice == item.MedicalDrugExtension.SocialSecurityPrice
                        //    && _MedicalDrugExtensione.PurchasingPrice == item.MedicalDrugExtension.PurchasingPrice)
                        //    {

                        //    }
                        //    else
                        //    {
                        //        if (_MedicalDrugExtensione != null)
                        //        {
                        //            _MedicalDrugExtensione.IsCurrentUse = false;
                        //            MedicalDrugExtensionStore.Update(_MedicalDrugExtensione);
                        //        }

                        //        MedicalDrugExtension medicalDrugExtension = _mapper.Map<MedicalDrugExtension>(item.MedicalDrugExtension);
                        //        medicalDrugExtension.Id = Guid.NewGuid().tostring32();
                        //        medicalDrugExtension.DataState = 1;
                        //        medicalDrugExtension.IsCurrentUse = true;
                        //        medicalDrugExtension.MedicalId = Item.Id;
                        //        medicalDrugExtension.FounderDate = DateTime.Now;
                        //        medicalDrugExtension.ModifierDate = DateTime.Now;
                        //        medicalDrugExtension.Founder = userId;
                        //        medicalDrugExtension.Modifier = userId;
                        //        MedicalDrugExtensionStore.Insert(medicalDrugExtension);

                        //    }
                        //}
                        //else
                        {

                            bool flag = true;
                            while (flag)
                            {
                                
                                NewItem.MedicalItemCode = ValueHelper.TimeNowRandom('T');
                                var Codedata = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.MedicalItemCode == NewItem.MedicalItemCode);
                                if (Codedata == null)
                                    flag = false;
                            }
                            NewItem.Id = item.Id;
                            NewItem.Mnemonic = NewItem.Mnemonic == "" ? CDGService.Data.Helper.Pinyin.GetInitials(NewItem.MedicalItemName) : NewItem.Mnemonic;
                            NewItem.PackageUnit = NewItem.ProcurementUnit = NewItem.SpecificationsUnit;
                            NewItem.Packaging = NewItem.ProcurementPackage = NewItem.Specifications;
                            MedicalItemRecordStore.Insert(NewItem);
                            MedicalDrugExtension medicalDrugExtension = _mapper.Map<MedicalDrugExtension>(item.MedicalDrugExtension);
                            medicalDrugExtension.Id = Guid.NewGuid().tostring32();
                            medicalDrugExtension.DataState = 1;
                            medicalDrugExtension.IsCurrentUse = true;
                            medicalDrugExtension.MedicalId = NewItem.Id;
                            medicalDrugExtension.FounderDate = DateTime.Now;
                            medicalDrugExtension.ModifierDate = DateTime.Now;
                            medicalDrugExtension.Founder = userId;
                            medicalDrugExtension.Modifier = userId;

                            MedicalDrugExtensionStore.Insert(medicalDrugExtension);
                        }
                        _unitOfWork.SaveChanges();

                    }
                }
                catch (Exception exp)
                {


                }
                return true;
            });
        }

        /// <summary>
        /// 导入药品
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        private Task<bool> DrugsItem(string FilePath)
        {

            return Task.Run(async () =>
            {
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();

                    foreach (DrugsTypeEnum myCode in Enum.GetValues(typeof(DrugsTypeEnum)))
                    {
                        string WareHouseIds = WarehouseCatalogStore.Entities.Where(t => t.Name == myCode.ToChinese()).FirstOrDefault().Id;
                        List<string> Main = new List<string>();
                        GetManId(WareHouseIds, Main);
                        var table = ExcelHelper.ReadExcel(FilePath, 3, (int)myCode);
                        var ItemInputs = TableToDrugs(table);

                        foreach (var item in ItemInputs)
                        {

                            if (item.MedicalItemName == "")
                            {
                                continue;
                            }
                            string Itemid = Guid.NewGuid().tostring32();
                            // item.Id = Itemid;
                            MedicalItemRecord NewItem = _mapper.Map<MedicalItemRecord>(item);
                            // NewItem.Id = Itemid;
                            // Newcenter.DialysisCode = ValueHelper.TimeNowRandom('C');
                            NewItem.Founder = userId;
                            NewItem.FounderDate = DateTime.Now;
                            NewItem.Modifier = userId;
                            NewItem.ModifierDate = DateTime.Now;
                            // NewItem.SpecificationsQuantity = 1;                            
                            NewItem.WareHouseIds = WareHouseIds;
                            NewItem.WareHouseParentIds = string.Join(",", Main.ToArray());
                            NewItem.MedicalItemType = 1;
                            //NewItem.IsSplited = NewItem.p
                            // 

                            //规格单位  
                            item.Specifications = item.Specifications == "" ? "无" : item.Specifications;
                            var unitdata = await MedicalUnitStore.GetFirstOrDefaultAsync(t => t.SpeUnitCHS == item.Specifications && t.UnitType == 1);

                            if (unitdata != null)
                            {
                                NewItem.SpecificationsUnit = unitdata.Id;
                                // employee.DepId = "无";
                            }
                            else
                            {
                                string guid = Guid.NewGuid().tostring32();
                                MedicalUnitStore.Insert(new MedicalUnit() { SpeUnitCHS = item.Specifications, DataState = 1, Id = guid, IsDelete = false, UnitType = 1, FounderDate = DateTime.Now, ModifierDate = DateTime.Now, Founder = userId, Modifier = userId });
                                NewItem.SpecificationsUnit = guid;
                            }

                            //包装单位  
                            item.PackageUnit = item.PackageUnit == "" ? "无" : item.PackageUnit;
                            var PackageUnit = await MedicalUnitStore.GetFirstOrDefaultAsync(t => t.SpeUnitCHS == item.PackageUnit && t.UnitType == 2);

                            if (PackageUnit != null)
                            {
                                NewItem.PackageUnit = PackageUnit.Id;
                                // employee.DepId = "无";
                            }
                            else
                            {
                                string guid = Guid.NewGuid().tostring32();
                                MedicalUnitStore.Insert(new MedicalUnit() { SpeUnitCHS = item.PackageUnit, DataState = 1, Id = guid, IsDelete = false, UnitType = 2, FounderDate = DateTime.Now, ModifierDate = DateTime.Now, Founder = userId, Modifier = userId });
                                NewItem.PackageUnit = guid;
                            }
                            if (NewItem.IsSplited)
                            {
                                NewItem.ProcurementPackage = NewItem.Specifications;
                                NewItem.ProcurementUnit = NewItem.SpecificationsUnit;
                            }
                            else
                            {
                                NewItem.ProcurementPackage = NewItem.Packaging;
                                NewItem.ProcurementUnit = NewItem.PackageUnit;

                            }

                            //剂量单位   
                            item.DoseUnit = item.DoseUnit == "" ? "无" : item.DoseUnit;
                            var DoseUnit = await MedicalUnitStore.GetFirstOrDefaultAsync(t => t.SpeUnitUS == item.DoseUnit && t.UnitType == 3);

                            if (DoseUnit != null)
                            {
                                NewItem.DoseUnit = DoseUnit.Id;
                                // employee.DepId = "无";
                            }
                            else
                            {
                                string guid = Guid.NewGuid().tostring32();
                                MedicalUnitStore.Insert(new MedicalUnit() { SpeUnitUS = item.DoseUnit, DataState = 1, Id = guid, IsDelete = false, UnitType = 3, FounderDate = DateTime.Now, ModifierDate = DateTime.Now, Founder = userId, Modifier = userId });
                                NewItem.DoseUnit = guid;
                            }

                            //剂型
                            item.Forms = item.Forms == "" ? "无" : item.Forms;
                            var Forms = await DosageFormStore.GetFirstOrDefaultAsync(t => t.TypeName == item.Forms);

                            if (Forms != null)
                            {
                                NewItem.Form = Forms.Id;
                                // employee.DepId = "无";
                            }
                            else
                            {
                                string guid = Guid.NewGuid().tostring32();
                                DosageFormStore.Insert(new DosageForm() { ParentId = "0", Id = guid, IsDelete = false, TypeName = item.Forms.ToString(), FounderDate = DateTime.Now, ModifierDate = DateTime.Now, Founder = userId, Modifier = userId });
                                NewItem.Form = guid;
                            }

                            //供货商 

                            item.SupplierId = item.SupplierId == "" ? "无" : item.SupplierId;
                            var SupplierId = await SupplierStore.GetFirstOrDefaultAsync(t => t.Name == item.SupplierId);

                            if (SupplierId != null)
                            {
                                NewItem.SupplierId = SupplierId.Id;
                                // employee.DepId = "无";
                            }
                            else
                            {
                                string guid = Guid.NewGuid().tostring32();
                                SupplierStore.Insert(new Supplier() { Name = item.SupplierId, DataState = 1, Id = guid, IsDelete = false, FounderDate = DateTime.Now, ModifierDate = DateTime.Now, Founder = userId, Modifier = userId });
                                NewItem.DoseUnit = guid;
                            }

                            //费用类别  
                            item.FeeTypeId = item.FeeTypeId == "" ? "无" : item.FeeTypeId;
                            var FeeTypeId = await SystemDictionaryStore.GetFirstOrDefaultAsync(t => t.TypeId == _dictionaryCode.FeeTypeId && t.Name == item.FeeTypeId);
                            if (FeeTypeId != null)
                            {
                                NewItem.FeeTypeId = FeeTypeId.Id;
                                // employee.DepId = "无";
                            }
                            else
                            {
                                string guid = Guid.NewGuid().tostring32();
                                SystemDictionaryStore.Insert(new SystemDictionary() { Name = item.FeeTypeId, DataState = 1, Id = guid, TypeId = _dictionaryCode.FeeTypeId, FounderDate = DateTime.Now, ModifierDate = DateTime.Now, Founder = userId, Modifier = userId });
                                NewItem.FeeTypeId = guid;
                            }
                            NewItem.Specifications = item.MinDose;

                            var Item = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.MedicalItemName == item.MedicalItemName && t.GoodsName == item.GoodsName && t.Manufacturer == item.Manufacturer && t.PackageUnit == item.PackageUnit);

                            NewItem.Id = Item == null ? Itemid : Item.Id;

                            if (Item != null)
                            {
                                var Specifications = Item.Specifications;
                                EntityHelper.CoptyProperty(NewItem, Item);
                                Item.SpecificationsUnit = NewItem.Specifications;
                                Item.Specifications = item.MinDose;
                                MedicalItemRecordStore.Update(Item);
                                var _MedicalDrugExtensione = await MedicalDrugExtensionStore.GetFirstOrDefaultAsync(t => t.MedicalId == Item.Id && t.IsCurrentUse);
                                if (_MedicalDrugExtensione != null
                                && _MedicalDrugExtensione.PurchasingPrice == item.MedicalDrugExtension.PurchasingPrice
                                && _MedicalDrugExtensione.AgreementPrice == item.MedicalDrugExtension.AgreementPrice
                                && _MedicalDrugExtensione.RetailPrice == item.MedicalDrugExtension.RetailPrice
                                && _MedicalDrugExtensione.ReferencePrice == item.MedicalDrugExtension.ReferencePrice
                                && _MedicalDrugExtensione.GocGuidePrice == item.MedicalDrugExtension.GocGuidePrice
                                && _MedicalDrugExtensione.SocialSecurityPrice == item.MedicalDrugExtension.SocialSecurityPrice
                                && _MedicalDrugExtensione.PurchasingPrice == item.MedicalDrugExtension.PurchasingPrice)
                                {

                                }
                                else
                                {
                                    if (_MedicalDrugExtensione != null)
                                    {
                                        _MedicalDrugExtensione.IsCurrentUse = false;
                                        MedicalDrugExtensionStore.Update(_MedicalDrugExtensione);
                                    }

                                    MedicalDrugExtension medicalDrugExtension = _mapper.Map<MedicalDrugExtension>(item.MedicalDrugExtension);
                                    medicalDrugExtension.Id = Guid.NewGuid().tostring32();
                                    medicalDrugExtension.DataState = 1;
                                    medicalDrugExtension.IsCurrentUse = true;
                                    medicalDrugExtension.MedicalId = Item.Id;
                                    medicalDrugExtension.FounderDate = DateTime.Now;
                                    medicalDrugExtension.ModifierDate = DateTime.Now;
                                    medicalDrugExtension.Founder = userId;
                                    medicalDrugExtension.Modifier = userId;
                                    MedicalDrugExtensionStore.Insert(medicalDrugExtension);

                                }
                            }
                            else
                            {
                                bool flag = true;
                                while (flag)
                                {
                                    NewItem.MedicalItemCode = ValueHelper.TimeNowRandom('D');
                                    var Codedata = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.MedicalItemCode == NewItem.MedicalItemCode);
                                    if (Codedata == null)
                                        flag = false;
                                }
                                NewItem.Id = Itemid;
                                NewItem.Mnemonic = NewItem.Mnemonic == "" ? CDGService.Data.Helper.Pinyin.GetInitials(NewItem.MedicalItemName) : NewItem.Mnemonic;
                                MedicalItemRecordStore.Insert(NewItem);

                                MedicalDrugExtension medicalDrugExtension = _mapper.Map<MedicalDrugExtension>(item.MedicalDrugExtension);
                                medicalDrugExtension.Id = Guid.NewGuid().tostring32();
                                medicalDrugExtension.DataState = 1;
                                medicalDrugExtension.IsCurrentUse = true;
                                medicalDrugExtension.MedicalId = NewItem.Id;
                                medicalDrugExtension.FounderDate = DateTime.Now;
                                medicalDrugExtension.ModifierDate = DateTime.Now;
                                medicalDrugExtension.Founder = userId;
                                medicalDrugExtension.Modifier = userId;
                                MedicalDrugExtensionStore.Insert(medicalDrugExtension);
                            }
                            _unitOfWork.SaveChanges();
                        }


                    }


                }
                catch (Exception exp)
                {

                }
                return true;
            });

        }

        /// <summary>
        /// 导入耗材
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        private Task<bool> MedicalsItem(string FilePath)
        {

            return Task.Run(async () =>
            {
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();

                    foreach (MaterialTypeEnum myCode in Enum.GetValues(typeof(MaterialTypeEnum)))
                    {
                        string WareHouseIds = WarehouseCatalogStore.Entities.Where(t => t.Name == myCode.ToChinese()).FirstOrDefault().Id;
                        List<string> Main = new List<string>();
                        GetManId(WareHouseIds, Main);
                        var table = ExcelHelper.ReadExcel(FilePath, 3, (int)myCode);
                        var ItemInputs = TableToMaterial(table);

                        foreach (var item in ItemInputs)
                        {

                            if (item.MedicalItemName == "")
                            {
                                continue;
                            }
                            string Itemid = Guid.NewGuid().tostring32();
                            // item.Id = Itemid;
                            MedicalItemRecord NewItem = _mapper.Map<MedicalItemRecord>(item);
                            // NewItem.Id = Itemid;
                            // Newcenter.DialysisCode = ValueHelper.TimeNowRandom('C');
                            NewItem.Founder = userId;
                            NewItem.FounderDate = DateTime.Now;
                            NewItem.Modifier = userId;
                            NewItem.ModifierDate = DateTime.Now;
                            // NewItem.SpecificationsQuantity = 1;  
                            NewItem.IsSplited = NewItem.SpecificationsQuantity == 1 ? false : true;
                            NewItem.WareHouseIds = WareHouseIds;
                            NewItem.WareHouseParentIds = string.Join(",", Main.ToArray());
                            NewItem.MedicalItemType = 2;
                            // 
                            // NewItem.IsSplited = 
                            //规格单位  
                            item.Specifications = item.Specifications == "" ? "无" : item.Specifications;
                            var unitdata = await MedicalUnitStore.GetFirstOrDefaultAsync(t => t.SpeUnitCHS == item.Specifications && t.UnitType == 1);

                            if (unitdata != null)
                            {
                                NewItem.SpecificationsUnit = unitdata.Id;
                                // employee.DepId = "无";
                            }
                            else
                            {
                                string guid = Guid.NewGuid().tostring32();
                                MedicalUnitStore.Insert(new MedicalUnit() { SpeUnitCHS = item.Specifications, DataState = 1, Id = guid, IsDelete = false, UnitType = 1, FounderDate = DateTime.Now, ModifierDate = DateTime.Now, Founder = userId, Modifier = userId });
                                NewItem.SpecificationsUnit = guid;
                            }

                            //包装单位  
                            item.PackageUnit = item.PackageUnit == "" ? "无" : item.PackageUnit;
                            var PackageUnit = await MedicalUnitStore.GetFirstOrDefaultAsync(t => t.SpeUnitCHS == item.PackageUnit && t.UnitType == 2);

                            if (PackageUnit != null)
                            {
                                NewItem.PackageUnit = PackageUnit.Id;
                                // employee.DepId = "无";
                            }
                            else
                            {
                                string guid = Guid.NewGuid().tostring32();
                                MedicalUnitStore.Insert(new MedicalUnit() { SpeUnitCHS = item.PackageUnit, DataState = 1, Id = guid, IsDelete = false, UnitType = 2, FounderDate = DateTime.Now, ModifierDate = DateTime.Now, Founder = userId, Modifier = userId });
                                NewItem.PackageUnit = guid;
                            }

                            if (NewItem.IsSplited)
                            {
                                NewItem.ProcurementPackage = NewItem.Specifications;
                                NewItem.ProcurementUnit = NewItem.SpecificationsUnit;
                            }
                            else
                            {
                                NewItem.ProcurementPackage = NewItem.Packaging;
                                NewItem.ProcurementUnit = NewItem.PackageUnit;

                            }


                            //供货商

                            item.SupplierId = item.SupplierId == "" ? "无" : item.SupplierId;
                            var SupplierId = await SupplierStore.GetFirstOrDefaultAsync(t => t.Name == item.SupplierId);

                            if (SupplierId != null)
                            {
                                NewItem.SupplierId = SupplierId.Id;
                                // employee.DepId = "无";
                            }
                            else
                            {
                                string guid = Guid.NewGuid().tostring32();
                                SupplierStore.Insert(new Supplier() { Name = item.SupplierId, DataState = 1, Id = guid, IsDelete = false, FounderDate = DateTime.Now, ModifierDate = DateTime.Now, Founder = userId, Modifier = userId });
                                NewItem.DoseUnit = guid;
                            }

                            //费用类别                         
                            item.FeeTypeId = "材料费";
                            var FeeTypeId = await SystemDictionaryStore.GetFirstOrDefaultAsync(t => t.TypeId == _dictionaryCode.FeeTypeId && t.Name == item.FeeTypeId);
                            if (FeeTypeId != null)
                            {
                                NewItem.FeeTypeId = FeeTypeId.Id;
                                // employee.DepId = "无";
                            }
                            else
                            {
                                string guid = Guid.NewGuid().tostring32();
                                SystemDictionaryStore.Insert(new SystemDictionary() { Name = item.FeeTypeId, DataState = 1, Id = guid, TypeId = _dictionaryCode.FeeTypeId, FounderDate = DateTime.Now, ModifierDate = DateTime.Now, Founder = userId, Modifier = userId });
                                NewItem.FeeTypeId = guid;
                            }
                            NewItem.Specifications = item.MinDose;
                            var Item = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.MedicalItemName == item.MedicalItemName && t.GoodsName == item.GoodsName && t.Manufacturer == item.Manufacturer && t.Specifications == item.MinDose);//&& t.PackageUnit == item.PackageUnit

                            NewItem.Id = Item == null ? Itemid : Item.Id;

                            if (Item != null)
                            {
                                var Specifications = Item.Specifications;
                                EntityHelper.CoptyProperty(NewItem, Item);
                                //Item.SpecificationsUnit = NewItem.Specifications;
                                //Item.Specifications = item.MinDose;
                                MedicalItemRecordStore.Update(Item);
                                var _MedicalDrugExtensione = await MedicalDrugExtensionStore.GetFirstOrDefaultAsync(t => t.MedicalId == Item.Id && t.IsCurrentUse);
                                if (_MedicalDrugExtensione != null
                                && _MedicalDrugExtensione.PurchasingPrice == item.MedicalDrugExtension.PurchasingPrice
                                && _MedicalDrugExtensione.AgreementPrice == item.MedicalDrugExtension.AgreementPrice
                                && _MedicalDrugExtensione.RetailPrice == item.MedicalDrugExtension.RetailPrice
                                && _MedicalDrugExtensione.ReferencePrice == item.MedicalDrugExtension.ReferencePrice
                                && _MedicalDrugExtensione.GocGuidePrice == item.MedicalDrugExtension.GocGuidePrice
                                && _MedicalDrugExtensione.SocialSecurityPrice == item.MedicalDrugExtension.SocialSecurityPrice
                                && _MedicalDrugExtensione.PurchasingPrice == item.MedicalDrugExtension.PurchasingPrice)
                                {

                                }
                                else
                                {
                                    if (_MedicalDrugExtensione != null)
                                    {
                                        _MedicalDrugExtensione.IsCurrentUse = false;
                                        MedicalDrugExtensionStore.Update(_MedicalDrugExtensione);
                                    }

                                    MedicalDrugExtension medicalDrugExtension = _mapper.Map<MedicalDrugExtension>(item.MedicalDrugExtension);
                                    medicalDrugExtension.Id = Guid.NewGuid().tostring32();
                                    medicalDrugExtension.DataState = 1;
                                    medicalDrugExtension.IsCurrentUse = true;
                                    medicalDrugExtension.MedicalId = Item.Id;
                                    medicalDrugExtension.FounderDate = DateTime.Now;
                                    medicalDrugExtension.ModifierDate = DateTime.Now;
                                    medicalDrugExtension.Founder = userId;
                                    medicalDrugExtension.Modifier = userId;
                                    MedicalDrugExtensionStore.Insert(medicalDrugExtension);

                                }
                            }
                            else
                            {
                                bool flag = true;
                                while (flag)
                                {
                                    NewItem.MedicalItemCode = ValueHelper.TimeNowRandom('U');
                                    var Codedata = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.MedicalItemCode == NewItem.MedicalItemCode);
                                    if (Codedata == null)
                                        flag = false;
                                }
                                NewItem.Id = Itemid;
                                NewItem.Mnemonic = NewItem.Mnemonic == "" ? CDGService.Data.Helper.Pinyin.GetInitials(NewItem.MedicalItemName) : NewItem.Mnemonic;
                                MedicalItemRecordStore.Insert(NewItem);

                                MedicalDrugExtension medicalDrugExtension = _mapper.Map<MedicalDrugExtension>(item.MedicalDrugExtension);
                                medicalDrugExtension.Id = Guid.NewGuid().tostring32();
                                medicalDrugExtension.DataState = 1;
                                medicalDrugExtension.IsCurrentUse = true;
                                medicalDrugExtension.MedicalId = NewItem.Id;
                                medicalDrugExtension.FounderDate = DateTime.Now;
                                medicalDrugExtension.ModifierDate = DateTime.Now;
                                medicalDrugExtension.Founder = userId;
                                medicalDrugExtension.Modifier = userId;
                                MedicalDrugExtensionStore.Insert(medicalDrugExtension);
                            }
                            _unitOfWork.SaveChanges();
                        }


                    }


                }
                catch (Exception exp)
                {

                }
                return true;
            });

        }

        /// <summary>
        /// 导入固定资产
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        private Task<bool> FixedAssetsItem(string FilePath)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();

                    foreach (AssetsItemEnum myCode in Enum.GetValues(typeof(AssetsItemEnum)))
                    {
                        string WareHouseIds = WarehouseCatalogStore.Entities.Where(t => t.Name == myCode.ToChinese()).FirstOrDefault().Id;
                        List<string> Main = new List<string>();
                        GetManId(WareHouseIds, Main);
                        var table = ExcelHelper.ReadExcel(FilePath, 3, (int)myCode);
                        var ItemInputs = TableToFixedAssets(table);

                        foreach (var item in ItemInputs)
                        {

                            if (item.MedicalItemName == "")
                            {
                                continue;
                            }
                            string Itemid = Guid.NewGuid().tostring32();
                            // item.Id = Itemid;
                            MedicalItemRecord NewItem = _mapper.Map<MedicalItemRecord>(item);
                            // NewItem.Id = Itemid;
                            // Newcenter.DialysisCode = ValueHelper.TimeNowRandom('C');
                            NewItem.Founder = userId;
                            NewItem.FounderDate = DateTime.Now;
                            NewItem.Modifier = userId;
                            NewItem.ModifierDate = DateTime.Now;
                            // NewItem.SpecificationsQuantity = 1;                            
                            NewItem.WareHouseIds = WareHouseIds;
                            NewItem.WareHouseParentIds = string.Join(",", Main.ToArray());
                            NewItem.MedicalItemType = 3;
                            //  
                            //包装单位  
                            item.PackageUnit = item.PackageUnit == "" ? "无" : item.PackageUnit;
                            var PackageUnit = await MedicalUnitStore.GetFirstOrDefaultAsync(t => t.SpeUnitCHS == item.PackageUnit && t.UnitType == 2);

                            if (PackageUnit != null)
                            {
                                NewItem.PackageUnit = PackageUnit.Id;
                                // employee.DepId = "无";
                            }
                            else
                            {
                                string guid = Guid.NewGuid().tostring32();
                                MedicalUnitStore.Insert(new MedicalUnit() { SpeUnitCHS = item.PackageUnit, DataState = 1, Id = guid, IsDelete = false, UnitType = 2, FounderDate = DateTime.Now, ModifierDate = DateTime.Now, Founder = userId, Modifier = userId });
                                NewItem.PackageUnit = guid;
                            }


                            //供货商

                            item.SupplierId = item.SupplierId == "" ? "无" : item.SupplierId;
                            var SupplierId = await SupplierStore.GetFirstOrDefaultAsync(t => t.Name == item.SupplierId);

                            if (SupplierId != null)
                            {
                                NewItem.SupplierId = SupplierId.Id;
                                // employee.DepId = "无";
                            }
                            else
                                NewItem.SupplierId = "";
                            //else
                            //{
                            //    string guid = Guid.NewGuid().tostring32();
                            //    SupplierStore.Insert(new Supplier() { Name = item.SupplierId, DataState = 1, Id = guid, IsDelete = false, FounderDate = DateTime.Now, ModifierDate = DateTime.Now, Founder = userId, Modifier = userId });
                            //    NewItem.DoseUnit = guid;
                            //}

                            ////费用类别                         
                            //item.FeeTypeId = item.FeeTypeId == "" ? "无" : item.FeeTypeId;
                            //var FeeTypeId = await SystemDictionaryStore.GetFirstOrDefaultAsync(t => t.TypeId == _dictionaryCode.FeeTypeId && t.Name == item.FeeTypeId);
                            //if (FeeTypeId != null)
                            //{
                            //    NewItem.FeeTypeId = FeeTypeId.Id;
                            //    // employee.DepId = "无";
                            //}
                            //else
                            //{
                            //    string guid = Guid.NewGuid().tostring32();
                            //    SystemDictionaryStore.Insert(new SystemDictionary() { Name = item.FeeTypeId, DataState = 1, Id = guid, TypeId = _dictionaryCode.FeeTypeId, FounderDate = DateTime.Now, ModifierDate = DateTime.Now, Founder = userId, Modifier = userId });
                            //    NewItem.FeeTypeId = guid;
                            //}
                            //NewItem.Specifications = item.MinDose;
                            var Item = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.MedicalItemName == item.MedicalItemName && t.GoodsName == item.GoodsName && t.Manufacturer == item.Manufacturer && t.PackageUnit == item.PackageUnit);

                            NewItem.Id = Item == null ? Itemid : Item.Id;

                            if (Item != null)
                            {
                                var Specifications = Item.Specifications;
                                EntityHelper.CoptyProperty(NewItem, Item);
                                //Item.SpecificationsUnit = NewItem.Specifications;
                                //Item.Specifications = item.MinDose;
                                MedicalItemRecordStore.Update(Item);
                                var _MedicalDrugExtensione = await MedicalDrugExtensionStore.GetFirstOrDefaultAsync(t => t.MedicalId == Item.Id && t.IsCurrentUse);
                                if (_MedicalDrugExtensione != null
                                && _MedicalDrugExtensione.PurchasingPrice == item.MedicalDrugExtension.PurchasingPrice
                                && _MedicalDrugExtensione.AgreementPrice == item.MedicalDrugExtension.AgreementPrice
                                && _MedicalDrugExtensione.RetailPrice == item.MedicalDrugExtension.RetailPrice
                                && _MedicalDrugExtensione.ReferencePrice == item.MedicalDrugExtension.ReferencePrice
                                && _MedicalDrugExtensione.GocGuidePrice == item.MedicalDrugExtension.GocGuidePrice
                                && _MedicalDrugExtensione.SocialSecurityPrice == item.MedicalDrugExtension.SocialSecurityPrice
                                && _MedicalDrugExtensione.PurchasingPrice == item.MedicalDrugExtension.PurchasingPrice)
                                {

                                }
                                else
                                {
                                    if (_MedicalDrugExtensione != null)
                                    {
                                        _MedicalDrugExtensione.IsCurrentUse = false;
                                        MedicalDrugExtensionStore.Update(_MedicalDrugExtensione);
                                    }

                                    MedicalDrugExtension medicalDrugExtension = _mapper.Map<MedicalDrugExtension>(item.MedicalDrugExtension);
                                    medicalDrugExtension.Id = Guid.NewGuid().tostring32();
                                    medicalDrugExtension.DataState = 1;
                                    medicalDrugExtension.IsCurrentUse = true;
                                    medicalDrugExtension.MedicalId = Item.Id;
                                    medicalDrugExtension.FounderDate = DateTime.Now;
                                    medicalDrugExtension.ModifierDate = DateTime.Now;
                                    medicalDrugExtension.Founder = userId;
                                    medicalDrugExtension.Modifier = userId;
                                    MedicalDrugExtensionStore.Insert(medicalDrugExtension);

                                }
                            }
                            else
                            {
                                bool flag = true;
                                while (flag)
                                {
                                    NewItem.MedicalItemCode = ValueHelper.TimeNowRandom('G');
                                    var Codedata = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.MedicalItemCode == NewItem.MedicalItemCode);
                                    if (Codedata == null)
                                        flag = false;
                                }
                                NewItem.Id = Itemid;
                                NewItem.Mnemonic = NewItem.Mnemonic == "" ? CDGService.Data.Helper.Pinyin.GetInitials(NewItem.MedicalItemName) : NewItem.Mnemonic;
                                MedicalItemRecordStore.Insert(NewItem);

                                MedicalDrugExtension medicalDrugExtension = _mapper.Map<MedicalDrugExtension>(item.MedicalDrugExtension);
                                medicalDrugExtension.Id = Guid.NewGuid().tostring32();
                                medicalDrugExtension.DataState = 1;
                                medicalDrugExtension.IsCurrentUse = true;
                                medicalDrugExtension.MedicalId = NewItem.Id;
                                medicalDrugExtension.FounderDate = DateTime.Now;
                                medicalDrugExtension.ModifierDate = DateTime.Now;
                                medicalDrugExtension.Founder = userId;
                                medicalDrugExtension.Modifier = userId;
                                MedicalDrugExtensionStore.Insert(medicalDrugExtension);
                            }
                            _unitOfWork.SaveChanges();
                        }
                    }
                }
                catch (Exception exp)
                {

                }
                return true;
            });

        }


        /// <summary>
        /// 导入低值
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        private Task<bool> LowsItem(string FilePath)
        {

            return Task.Run(async () =>
            {
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();

                    foreach (LowItemEnum myCode in Enum.GetValues(typeof(LowItemEnum)))
                    {
                        string WareHouseIds = WarehouseCatalogStore.Entities.Where(t => t.Name == myCode.ToChinese()).FirstOrDefault().Id;
                        List<string> Main = new List<string>();
                        GetManId(WareHouseIds, Main);
                        var table = ExcelHelper.ReadExcel(FilePath, 3, (int)myCode);
                        var ItemInputs = TableToLow(table);

                        foreach (var item in ItemInputs)
                        {

                            if (item.MedicalItemName == "")
                            {
                                continue;
                            }
                            string Itemid = Guid.NewGuid().tostring32();
                            // item.Id = Itemid;
                            MedicalItemRecord NewItem = _mapper.Map<MedicalItemRecord>(item);
                            // NewItem.Id = Itemid;
                            // Newcenter.DialysisCode = ValueHelper.TimeNowRandom('C');
                            NewItem.Founder = userId;
                            NewItem.FounderDate = DateTime.Now;
                            NewItem.Modifier = userId;
                            NewItem.ModifierDate = DateTime.Now;
                            // NewItem.SpecificationsQuantity = 1;  
                            NewItem.IsSplited = NewItem.SpecificationsQuantity == 1 ? false : true;
                            NewItem.WareHouseIds = WareHouseIds;
                            NewItem.WareHouseParentIds = string.Join(",", Main.ToArray());
                            NewItem.MedicalItemType = 4;
                            // 
                            // NewItem.IsSplited = 
                            //规格单位  
                            item.Specifications = item.Specifications == "" ? "无" : item.Specifications;
                            var unitdata = await MedicalUnitStore.GetFirstOrDefaultAsync(t => t.SpeUnitCHS == item.Specifications && t.UnitType == 1);

                            if (unitdata != null)
                            {
                                NewItem.SpecificationsUnit = unitdata.Id;
                                // employee.DepId = "无";
                            }
                            else
                            {
                                string guid = Guid.NewGuid().tostring32();
                                MedicalUnitStore.Insert(new MedicalUnit() { SpeUnitCHS = item.Specifications, DataState = 1, Id = guid, IsDelete = false, UnitType = 1, FounderDate = DateTime.Now, ModifierDate = DateTime.Now, Founder = userId, Modifier = userId });
                                NewItem.SpecificationsUnit = guid;
                            }

                            //包装单位  
                            item.PackageUnit = item.PackageUnit == "" ? "无" : item.PackageUnit;
                            var PackageUnit = await MedicalUnitStore.GetFirstOrDefaultAsync(t => t.SpeUnitCHS == item.PackageUnit && t.UnitType == 2);

                            if (PackageUnit != null)
                            {
                                NewItem.PackageUnit = PackageUnit.Id;
                                // employee.DepId = "无";
                            }
                            else
                            {
                                string guid = Guid.NewGuid().tostring32();
                                MedicalUnitStore.Insert(new MedicalUnit() { SpeUnitCHS = item.PackageUnit, DataState = 1, Id = guid, IsDelete = false, UnitType = 2, FounderDate = DateTime.Now, ModifierDate = DateTime.Now, Founder = userId, Modifier = userId });
                                NewItem.PackageUnit = guid;
                            }
                            if (NewItem.IsSplited)
                            {
                                NewItem.ProcurementPackage = NewItem.Specifications;
                                NewItem.ProcurementUnit = NewItem.SpecificationsUnit;
                            }
                            else
                            {
                                NewItem.ProcurementPackage = NewItem.Packaging;
                                NewItem.ProcurementUnit = NewItem.PackageUnit;

                            }


                            //供货商

                            item.SupplierId = item.SupplierId == "" ? "无" : item.SupplierId;
                            var SupplierId = await SupplierStore.GetFirstOrDefaultAsync(t => t.Name == item.SupplierId);

                            if (SupplierId != null)
                            {
                                NewItem.SupplierId = SupplierId.Id;
                                // employee.DepId = "无";
                            }
                            else
                            {
                                string guid = Guid.NewGuid().tostring32();
                                SupplierStore.Insert(new Supplier() { Name = item.SupplierId, DataState = 1, Id = guid, IsDelete = false, FounderDate = DateTime.Now, ModifierDate = DateTime.Now, Founder = userId, Modifier = userId });
                                NewItem.DoseUnit = guid;
                            }

                            //费用类别                         
                            item.FeeTypeId = "材料费";
                            var FeeTypeId = await SystemDictionaryStore.GetFirstOrDefaultAsync(t => t.TypeId == _dictionaryCode.FeeTypeId && t.Name == item.FeeTypeId);
                            if (FeeTypeId != null)
                            {
                                NewItem.FeeTypeId = FeeTypeId.Id;
                                // employee.DepId = "无";
                            }
                            else
                            {
                                string guid = Guid.NewGuid().tostring32();
                                SystemDictionaryStore.Insert(new SystemDictionary() { Name = item.FeeTypeId, DataState = 1, Id = guid, TypeId = _dictionaryCode.FeeTypeId, FounderDate = DateTime.Now, ModifierDate = DateTime.Now, Founder = userId, Modifier = userId });
                                NewItem.FeeTypeId = guid;
                            }
                            NewItem.Specifications = item.MinDose;
                            var Item = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.MedicalItemName == item.MedicalItemName && t.GoodsName == item.GoodsName && t.Manufacturer == item.Manufacturer && t.Specifications == item.MinDose && t.Brand == item.Brand);//&& t.PackageUnit == item.PackageUnit

                            NewItem.Id = Item == null ? Itemid : Item.Id;

                            if (Item != null)
                            {
                                var Specifications = Item.Specifications;
                                EntityHelper.CoptyProperty(NewItem, Item);
                                //Item.SpecificationsUnit = NewItem.Specifications;
                                //Item.Specifications = item.MinDose;
                                MedicalItemRecordStore.Update(Item);
                                var _MedicalDrugExtensione = await MedicalDrugExtensionStore.GetFirstOrDefaultAsync(t => t.MedicalId == Item.Id && t.IsCurrentUse);
                                if (_MedicalDrugExtensione != null
                                && _MedicalDrugExtensione.PurchasingPrice == item.MedicalDrugExtension.PurchasingPrice
                                && _MedicalDrugExtensione.AgreementPrice == item.MedicalDrugExtension.AgreementPrice
                                && _MedicalDrugExtensione.RetailPrice == item.MedicalDrugExtension.RetailPrice
                                && _MedicalDrugExtensione.ReferencePrice == item.MedicalDrugExtension.ReferencePrice
                                && _MedicalDrugExtensione.GocGuidePrice == item.MedicalDrugExtension.GocGuidePrice
                                && _MedicalDrugExtensione.SocialSecurityPrice == item.MedicalDrugExtension.SocialSecurityPrice
                                && _MedicalDrugExtensione.PurchasingPrice == item.MedicalDrugExtension.PurchasingPrice)
                                {

                                }
                                else
                                {
                                    if (_MedicalDrugExtensione != null)
                                    {
                                        _MedicalDrugExtensione.IsCurrentUse = false;
                                        MedicalDrugExtensionStore.Update(_MedicalDrugExtensione);
                                    }

                                    MedicalDrugExtension medicalDrugExtension = _mapper.Map<MedicalDrugExtension>(item.MedicalDrugExtension);
                                    medicalDrugExtension.Id = Guid.NewGuid().tostring32();
                                    medicalDrugExtension.DataState = 1;
                                    medicalDrugExtension.IsCurrentUse = true;
                                    medicalDrugExtension.MedicalId = Item.Id;
                                    medicalDrugExtension.FounderDate = DateTime.Now;
                                    medicalDrugExtension.ModifierDate = DateTime.Now;
                                    medicalDrugExtension.Founder = userId;
                                    medicalDrugExtension.Modifier = userId;
                                    MedicalDrugExtensionStore.Insert(medicalDrugExtension);

                                }
                            }
                            else
                            {
                                bool flag = true;
                                while (flag)
                                {
                                    NewItem.MedicalItemCode = ValueHelper.TimeNowRandom('L');
                                    var Codedata = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.MedicalItemCode == NewItem.MedicalItemCode);
                                    if (Codedata == null)
                                        flag = false;
                                }
                                NewItem.Id = Itemid;
                                NewItem.Mnemonic = NewItem.Mnemonic == "" ? CDGService.Data.Helper.Pinyin.GetInitials(NewItem.MedicalItemName) : NewItem.Mnemonic;
                                MedicalItemRecordStore.Insert(NewItem);

                                MedicalDrugExtension medicalDrugExtension = _mapper.Map<MedicalDrugExtension>(item.MedicalDrugExtension);
                                medicalDrugExtension.Id = Guid.NewGuid().tostring32();
                                medicalDrugExtension.DataState = 1;
                                medicalDrugExtension.IsCurrentUse = true;
                                medicalDrugExtension.MedicalId = NewItem.Id;
                                medicalDrugExtension.FounderDate = DateTime.Now;
                                medicalDrugExtension.ModifierDate = DateTime.Now;
                                medicalDrugExtension.Founder = userId;
                                medicalDrugExtension.Modifier = userId;
                                MedicalDrugExtensionStore.Insert(medicalDrugExtension);
                            }
                            _unitOfWork.SaveChanges();
                        }


                    }


                }
                catch (Exception exp)
                {

                }
                return true;
            });

        }




        #endregion


        #region 导出Excel




        #endregion

        public void GetManId(string id, List<string> listId)
        {
            try
            {
                var data = WarehouseCatalogStore.Entities.Where(t => t.Id == id).FirstOrDefault();
                if (data != null)
                {
                    listId.Insert(0, data.Id);
                    if (data.ParentId + "" != "")
                    {
                        GetManId(data.ParentId, listId);
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 导入低值
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public Task<bool> TempTableTo(string FilePath)
        {

            return Task.Run(async () =>
            {
                try
                {
                    FilePath = @"C:\Users\Administrator\Desktop\审计资料\中联自查\已核对\筠连康达 - 副本.xls";
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();

                    var table = ExcelHelper.ReadExcel(FilePath, 2);
                    var ItemInputs = TempTableToFixedAssets(table);

                    var temp = ItemInputs.GroupBy(a => new { a.CardNum, a.CreateDate });
                    int i = 1;
                    List<TempTab> NewTable = new List<TempTab>();
                    foreach (var item in temp)
                    {
                        TempTab tab = new TempTab()
                        {
                            BalanceNo = "JLJS" + i,
                            CreateDate = item.First().CreateDate,
                            Birthday = item.First().Birthday,
                            CardNum = item.First().CardNum,
                            ContactAddress = item.First().ContactAddress,
                            CostMoney = item.Sum(A => A.CostMoney),
                            DataState = 1,
                            DataType = 2,
                            Name = item.First().Name,
                            ReceivableMoney = item.Sum(A => A.ReceivableMoney),
                            CenterId = item.First().CenterId,
                        };
                        NewTable.Add(tab);
                        i++;
                    }
                    var tmoney = NewTable.Sum(t => t.CostMoney);
                    var tmoney1 = NewTable.Sum(t => t.ReceivableMoney);
                    TempTabStore.Insert(NewTable);

                    _unitOfWork.SaveChanges();

                }
                catch (Exception exp)
                {

                }
                return true;
            });

        }


        private List<TempTab> TempTableToFixedAssets(DataTable table)
        {
            List<TempTab> TempTabs = new List<TempTab>();
            DataRow row;
            try
            {
                foreach (var item in table.Rows.Cast<DataRow>())
                {
                    row = item;
                    string sfz = item["身份证号码"].ToString();
                    if (sfz == "5125321194111123014")
                        sfz = "512531197410012991";
                    if (item["姓名"].ToString() == "张浩")
                        sfz = "511526199112074411";
                    if (sfz.Length < 14)
                    {

                    }

                    if (string.IsNullOrWhiteSpace(item["金额"].ToString()))
                        continue;
                    var bsd = DateTime.ParseExact(sfz.Substring(6, 8), "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces); ;
                    TempTab Material = new TempTab
                    {
                        DataState = 1,
                        DataType = 2,
                        CenterId = "eea94f2837244315b30d3095b9f4dc83",
                        BalanceNo = "",
                        Birthday = bsd,
                        CardNum = sfz,
                        ContactAddress = item["地址"].ToString(),
                        CostMoney = Convert.ToDecimal(item["金额"].ToString()),
                        ReceivableMoney = Convert.ToDecimal(item["金额"].ToString()),
                        CreateDate = Convert.ToDateTime(item["费用时间"].ToString()),
                        Name = item["姓名"].ToString(),
                    };
                    TempTabs.Add(Material);

                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }




            return TempTabs;


        }
    }



}
