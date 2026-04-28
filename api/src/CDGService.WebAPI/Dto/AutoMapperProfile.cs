using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CDGService.Data;
using CDGService.Data.Datas;
using CDGService.Data.Enums;
using CDGService.Data.Store;
using CDGService.ThirdPartyLib.Binaries;
using CDGService.Utils;
using CDGService.WebAPI.Dto.AutoCenterPut;
using CDGService.WebAPI.Extenstions;

namespace CDGService.WebAPI.Dto
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //Source->Destination  //这一个映射是做什么？
            //CreateMap<JobStationOutput, JobStation>(); 

            //人员
            CreateMap<Employee, EmployeeOutPut>().AfterMap((from, to) =>
            {
                if (from.CenterDialysis != null && from.CenterDialysis.DialysisName != "")
                {
                    to.CenterDialysisName = from.CenterDialysis.DialysisName;
                }
                else
                {
                    to.CenterDialysisName = "康美佳集团总部";
                }
                if (from.positiveDate.HasValue)
                    to.positiveDate = from.positiveDate.Value.ToString("yyyy-MM-dd");
                if (from.expireDate.HasValue)
                    to.expireDate = from.expireDate.Value.ToString("yyyy-MM-dd");
                if (from.GraduationDate.HasValue)
                    to.GraduationDate = from.GraduationDate.Value.ToString("yyyy-MM-dd");
                if (from.signDate.HasValue)
                    to.signDate = from.signDate.Value.ToString("yyyy-MM-dd");
                if (from.hiredate.HasValue)
                    to.hiredate = from.hiredate.Value.ToString("yyyy-MM-dd");
                if (from.Birthday.HasValue)
                    to.Birthday = from.Birthday.Value.ToString("yyyy-MM-dd");
                if (from.Sposition != null)
                {
                    to.position = from.Sposition.Name;
                    to.positionId = from.Sposition.Id;
                }
                if (from.SEducation != null)
                {
                    to.Education = from.SEducation.Name;
                    to.EduId = from.SEducation.Id;
                }
                if (from.Sdepartment != null)
                {
                    to.department = from.Sdepartment.Name;
                    to.DepId = from.Sdepartment.Id;
                }
                if (from.SJobTitle != null)
                {
                    to.JobTitleId = from.SJobTitle.Id;
                    to.JobTitle = from.SJobTitle.Name;
                }
                //StrWorkingState
                if (from.DicWorkingState != null)
                {
                    to.WorkingState = from.DicWorkingState.Id;
                    to.StrWorkingState = from.DicWorkingState.Name;
                }
                if (to.ExperienceJob + "" != "")
                    to.ExperienceJobs.Add(to.ExperienceJob);
                if (to.ExperienceJobTwo + "" != "")
                    to.ExperienceJobs.Add(to.ExperienceJobTwo);
                if (from.Birthday.HasValue)
                    to.Age = DateTime.Now.Year - from.Birthday.Value.Year;


            });


            CreateMap<EmployeeInput, Employee>();


            //人员预调动

            CreateMap<PerEmpTransfer, PerEmpTransferOutPut>().AfterMap((from, to) =>
            {
                to.OriginalCenter = from.OriginalCenter != null ? from.OriginalCenter.DialysisName : "集团总部";
                to.OriginalDep = from.OriginalDep != null ? from.OriginalDep.Name : "无";
                to.OriginalPosition = from.OriginalPosition != null ? from.OriginalPosition.Name : "无";
                to.PresentCenter = from.Center != null ? from.Center.DialysisName : "集团总部";
                to.PresentDep = from.depDic != null ? from.depDic.Name : "无";
                to.PresentPosition = from.PosDic != null ? from.PosDic.Name : "无";

            });

            CreateMap<PerEmpTransferInput, PerEmpTransfer>();



            CreateMap<SysRegion, SysRegionOutput>();

            //账户
            CreateMap<User, UserOutput>().AfterMap((from, to) =>
            {
                to.Name = from.Employee != null ? from.Employee.Name : "";
                to.position = (from.Employee != null && from.Employee.Sposition != null) ? from.Employee.Sposition.Name : "";
                to.department = (from.Employee != null && from.Employee.Sdepartment != null) ? from.Employee.Sdepartment.Name : "";
                to.CenterDialysisName = (from.Employee != null && from.Employee.CenterDialysis != null) ? from.Employee.CenterDialysis.DialysisName : "康美佳集团总部";

            });
            CreateMap<UserNewInput, User>();
            CreateMap<UserInPut, User>();
            //机构
            //机构
            CreateMap<CenterDialysis, CenterListOutPut>().AfterMap((from, to) =>
            {
                to.DialysisName = from.ShortName;
            }
            );
            CreateMap<CenterDialysis, CenterDialysisOutPut>().ForMember(d => d.DialysisRegionID, a => a.Ignore()).AfterMap((from, to) =>
            {
                if (from.DialysisRegions != null)
                    to.DialysisRegion = from.DialysisRegions.RegionName;
                if (from.DialysisContactMan != null)
                    to.DialysisContactMan = from.DialysisContactMan.Name;
                //if (from.HeadNurseUser != null)
                //    to.HeadNurseMan = from.HeadNurseUser.Name;
                if (from.SetUpDate != null)
                    to.SetUpDate = from.SetUpDate.Value.ToString("yyyy-MM-dd");
                if (from.RunDate != null)
                    to.RunDate = from.RunDate.Value.ToString("yyyy-MM-dd");
                if (from.ModifyTime != null)
                    to.ModifyTime = from.ModifyTime.Value.ToString("yyyy-MM-dd");
                if (from.DialysisImg + "" != "" && System.IO.File.Exists(from.DialysisImg))

                    to.DialysisImgs.Add("data:image/jpeg;base64," + ImageHelper.ImgToBase64String1(from.DialysisImg));
                if (from.DialysisImg1 + "" != "" && System.IO.File.Exists(from.DialysisImg1))

                    to.DialysisImgs.Add("data:image/jpeg;base64," + ImageHelper.ImgToBase64String1(from.DialysisImg1));
                if (from.DialysisImg2 + "" != "" && System.IO.File.Exists(from.DialysisImg2))

                    to.DialysisImgs.Add("data:image/jpeg;base64," + ImageHelper.ImgToBase64String1(from.DialysisImg2));
                if (from.DialysisImg3 + "" != "" && System.IO.File.Exists(from.DialysisImg3))

                    to.DialysisImgs.Add("data:image/jpeg;base64," + ImageHelper.ImgToBase64String1(from.DialysisImg3));
                if (from.DialysisImg4 + "" != "" && System.IO.File.Exists(from.DialysisImg4))

                    to.DialysisImgs.Add("data:image/jpeg;base64," + ImageHelper.ImgToBase64String1(from.DialysisImg4));

            });
            CreateMap<CenterDialysis, DialysisMapOutPut>().AfterMap((from, to) =>
            {
                to.position = new float[] { from.DialysisLng, from.DialysisLat };
            });
            CreateMap<CenterDialysisInput, CenterDialysis>();

            //患者

            CreateMap<Patient, PatientOutPut>().AfterMap((from, to) =>
            {
                try
                {
                    to.Age = DateTime.Now.Year - from.Birthday.Value.Year;
                    if (from.Dialysis != null && from.Dialysis.DialysisName != "")
                    {
                        to.Dialysis = from.Dialysis.DialysisName;
                    }
                    if (from.Birthday.HasValue)
                        to.Birthday = from.Birthday.Value.ToString("yyyy-MM-dd");
                    if (from.SBloodBorneDisease != null)
                    {
                        to.SBloodBorneDisease = from.SBloodBorneDisease.Name;

                    }
                    if (from.SHospitalState != null)
                    {
                        to.SHospitalState = from.SHospitalState.Name;

                    }
                    if (from.SSIInsuredType != null)
                    {
                        to.SSIInsuredType = from.SSIInsuredType.Name;

                    }
                    else
                    {
                        to.SSIInsuredType = from.SIInsuredType;
                    }
                    if (from.SRHBloodType != null)
                    {

                        to.SRHBloodType = from.SRHBloodType.Name;
                    }
                    if (from.SABOBloodType != null)
                        to.SABOBloodType = from.SABOBloodType.Name;
                    if (from.SEducationBackground != null && from.SEducationBackground.Name != null)
                        to.SEducationBackground = from.SEducationBackground.Name;
                    if (from.SMarital != null)
                        to.SMarital = from.SMarital.Name;
                    if (from.ReceiveDate.HasValue)
                        to.ReceiveDate = DateTimeHelper.ShortDateString(from.ReceiveDate.Value);
                    if (from.FirsthHematodialysisDate.HasValue)
                        to.FirsthHematodialysisDate = DateTimeHelper.ShortDateString(from.FirsthHematodialysisDate.Value);
                    if (from.ChargeDoctorEmployee != null)
                        to.ReceiveDoctor = from.ChargeDoctorEmployee.Name;
                    // to.ChargeDoctor = from.ChargeDoctorEmployee.Name;
                }
                catch (Exception exp)
                {

                }

            });
            //病历
            CreateMap<MedicalHistoryFirstPage, MedicalHistoryFirstOutPut>().AfterMap((from, to) =>
            {
                to.Anticoagulants = from.Anticoagulants.Split('|').ToList();
                to.PrimaryDisease = from.PrimaryDisease.Split('|').ToList();
                to.TreatmentFrequency = from.TreatmentFrequency.Split('|').ToList();
                to.TreatmentMode = from.TreatmentMode.Split('|').ToList();

            });
            CreateMap<FirstPageItemCurrentDiagnosis, FirstPageItemCurrentDiagnosisOutPut>().AfterMap((from, to) =>
            {
                to.CurrentDiagnosisDate = DateTimeHelper.LongDateString(from.CurrentDiagnosisDate.Value);

            });
            CreateMap<VascularAccessRecord, VascularAccessRecordOutPut>().AfterMap((from, to) =>
            {
                to.RecRegistrationDate = DateTimeHelper.LongDateString(from.RecRegistrationDate.Value);

            });

            CreateMap<InfectiousDiseaseRegister, InfectiousDiseaseRegisterOutPut>().AfterMap((from, to) =>
            {
                to.RecRegistrationDate = DateTimeHelper.LongDateString(from.RecRegistrationDate.Value);

            });

            CreateMap<AllergyRegister, AllergyRegisterOutPut>().AfterMap((from, to) =>
            {
                to.RecRegistrationDate = DateTimeHelper.LongDateString(from.RecRegistrationDate.Value);

            });
            CreateMap<TumorRegister, TumorRegisterOutPut>()
                .AfterMap((from, to) =>
                {
                    to.RecRegistrationDate = DateTimeHelper.LongDateString(from.RecRegistrationDate.Value);

                });
            CreateMap<FirstOutpatientRecord, FirstOutpatientRecordOutPut>().AfterMap((from, to) =>
            {


            });
            //

            CreateMap<OutpatientMedicalRecord, OutpatientMedicalRecordOutPut>()
                 .AfterMap((from, to) =>
                 {
                     to.TreatmentDate = DateTimeHelper.LongDateString(from.TreatmentDate.Value);

                 });
            //设备
            CreateMap<EquipmentInfo, EquipmentOutPut>().AfterMap((from, to) =>
            {

                if (from.Dialysis != null && from.Dialysis.DialysisName != "")
                {
                    to.Center = from.Dialysis.DialysisName;
                }

                if (from.SBloodBorneDisease != null)
                {
                    to.BloodBorneDisease = from.SBloodBorneDisease.Name;

                }
                if (from.SName != null)
                {
                    to.Name = from.SName.Name;

                }
                to.validity = 0;
                //透析设备 	 开头 SWS-
                if (from.EquipType == "6f65d7538e0844c2bb9f8c92e650b493")
                {
                    if (from.Model.IndexOf("SWS-") != 0)
                        to.validity = 2;
                }
                //透析编码 	 开头 SWS-4000
                if (from.EquipType == "6f65d7538e0844c2bb9f8c92e650b493")
                {
                    if ((from.SerialNumber.Length < 9 || from.SerialNumber.Length > 10) || (from.Model.Length <= 4 || from.Model.Substring(4, 1) != from.SerialNumber.Substring(0, 1)))
                        to.validity = 1;
                }
                if (from.EquipType == "6f65d7538e0844c2bb9f8c92e650b493")
                {
                    if (from.Model.IndexOf("SWS-") != 0 && (from.SerialNumber.Length < 9 || from.SerialNumber.Length > 10 || (from.Model.Length <= 4 || from.Model.Substring(4, 1) != from.SerialNumber.Substring(0, 1))))
                        to.validity = 3;
                }
                if (from.SEquipType != null)
                {
                    to.EquipType = from.SEquipType.Name;

                }

                if (from.DicTreatmentRegion != null)
                {

                    to.TreatmentRegion = from.DicTreatmentRegion.Name;
                }


                List<string> vs = new List<string>();
                foreach (var item in from.TreatmentModels.Split(','))
                {

                    // (TreatmentModelEnum)Enum.Parse(typeof(TreatmentModelEnum), ("T" + item)
                    if (item != "")
                        vs.Add(((TreatmentModelEnum)Enum.Parse(typeof(TreatmentModelEnum), ("T" + item))).ToChinese());
                }
                to.TreatmentModelsK = vs.ToArray();
                //  to.SDataState = from.EquipmentState.ToChinese();
                to.PurchaseDate = from.PurchaseDate != null ? from.PurchaseDate.Value.ToString("yyyy-MM-dd") : "";
                to.MaintenanceDate = from.MaintenanceDate != null ? from.MaintenanceDate.Value.ToString("yyyy-MM-dd") : "";
                to.ProduceDate = from.ProduceDate != null ? from.ProduceDate.Value.ToString("yyyy-MM-dd") : "";

            });

            //设备维修
            CreateMap<MaintenanceRecord, MaintenanceRecordOutPut>().AfterMap((from, to) =>
            {


                to.MaintenanceDate = from.MaintenanceDate.Value.ToString("yyyy-MM-dd");


            });
            //设备维保
            CreateMap<EquipmentKeepRecord, EquipmentKeepRecordOutPut>().AfterMap((from, to) =>
            {

                if (from.MaintenanceDate.HasValue)
                    to.MaintenanceDate = from.MaintenanceDate.Value.ToString("yyyy-MM-dd");


            });

            //生化检测
            CreateMap<BiochemicalTest, BiochemicalTestOutPut>().AfterMap((from, to) =>
            {


                to.FiledDate = from.FiledDate.Value.ToString("yyyy-MM-dd");

                to.TestDate = from.TestDate.Value.ToString("yyyy-MM-dd");
            });

            //字典类型
            CreateMap<DictionaryTypeInput, DictionaryType>();
            CreateMap<DictionaryType, DictionaryTypeOutPut>();
            //字典
            CreateMap<SystemDictionaryInput, SystemDictionary>();
            CreateMap<SystemDictionary, SystemDictionaryOutPut>().AfterMap((from, to) =>
            {
                if (from.DictionaryType != null)
                    to.DictionaryTypeName = from.DictionaryType.Name;
            });

            //日志

            CreateMap<Log, LogOutPut>().AfterMap((from, to) =>
            {
                if (from.OperatorUser != null)
                {
                    to.OperatorUserName = from.OperatorUser.Name;
                    if (from.OperatorUser.Employee != null)
                        to.EmployeeName = from.OperatorUser.Employee.Name;
                }
                to.CreateTime = from.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
                to.LogType = to.LogCode.ToChinese();
            });

            //角色
            CreateMap<RoleInPut, Role>();
            CreateMap<Role, RoleOutPut>();
            //菜单
            CreateMap<MenuInput, Menu>();
            CreateMap<Menu, MenuOutPut>().AfterMap((from, to) =>
            {
                to.title = from.MenuName;

            });

            CreateMap<DocumentInput, Document>();
            CreateMap<Document, DocumentOutput>().AfterMap((from, to) =>
            {
                if (from.user != null && from.user.Employee != null)
                    to.EmployeeName = from.user.Employee.Name;
                to.StringCatalog = from.Catalog.ToChinese();
                to.StringKnowledgeType = from.knowledgeType.ToChinese();
                to.FileSize = ValueHelper.ToRound((from.FileSize / 1024.0));
                to.LastModifyTime = from.LastModifyTime.ToString("yyyy-MM-dd");
                to.PublishTime = from.PublishTime.Value.ToString("yyyy-MM-dd");
                to.EntryTime = from.EntryTime.ToString("yyyy-MM-dd");
                string temp = Path.GetExtension(from.FileUrl);
                switch (temp.Substring(1, temp.Length - 1).ToLower())
                {
                    case "doc":
                        to.FileType = FielType.Word;
                        break;
                    case "docx":
                        to.FileType = FielType.Word;
                        break;
                    case "xlsx":
                        to.FileType = FielType.excel;
                        break;
                    case "xls":

                        to.FileType = FielType.excel;
                        break;
                    case "ppt":
                        to.FileType = FielType.ppt;
                        break;

                    case "pptx":
                        to.FileType = FielType.ppt;
                        break;
                    case "txt":
                        to.FileType = FielType.text;
                        break;
                    case "jpg":
                        to.FileType = FielType.jpg;
                        break;
                    case "png":
                        to.FileType = FielType.jpg;
                        break;
                    case "jpeg":
                        to.FileType = FielType.jpg;
                        break;
                    case "gif":
                        to.FileType = FielType.jpg;
                        break;
                    case "pdf":
                        to.FileType = FielType.pdf;
                        break;
                    default:
                        to.FileType = FielType.other;
                        break;
                }


            });

            //按钮

            CreateMap<MenuButtonInput, MenuButton>();
            CreateMap<MenuButton, MenuButtonOutPut>();



            //库房目录

            CreateMap<WarehouseCatalog, WarehouseCatalogOutPut>();
            CreateMap<WarehouseCatalog, CenterWarehouseCatalogOutPut>();


            CreateMap<WarehouseCatalogInput, WarehouseCatalog>();
            //供应商

            CreateMap<Supplier, SupplierOutPut>().AfterMap((from, to) =>
            {
                //  to.MainCategories = from.MainCategories.Split(',').ToList(); 
                //
                List<string> vs = new List<string>();
                if (from.MainCategories + "" != "")
                {

                    var data = from.MainCategories.Split(',');
                    if (data != null && data.Length > 0)
                    {
                        foreach (var item in data)
                        {
                            vs.Add(((MaincategorieslEnum)Convert.ToInt32(item)).ToChinese());
                        }
                    }
                }

                to.MainCategories = string.Join(",", vs);
                if (from.Image1 + "" != "")
                    to.Image1 = "data:image/jpeg;base64," + ImageHelper.ImgToBase64String1(from.Image1);
                if (from.Image2 + "" != "")
                    to.Image2 = "data:image/jpeg;base64," + ImageHelper.ImgToBase64String1(from.Image2);
            });


            CreateMap<SupplierInPut, Supplier>();
            CreateMap<MedicalItemRecord, MedicalItemRecordOutPut>().AfterMap((from, to) =>
            {
                /*
                 
        public string Specifications  { get; set; }
        
        /// <summary>
        /// 规格单位
        /// 最小剂量+剂量单位*包装规格+计量单位/包装单位（显示为2g*100片/盒，规格自动生成，可手工调整）
        /// </summary>
        public string SpecificationsUnit { get; set; }

        /// <summary>
        /// 包装规格
        /// </summary>
        public string Packaging { get; set; }

        /// <summary>
        /// 包装单位
        /// </summary>
        public string PackageUnit { get; set; }
                 */

                to.supplierName = from.supplier == null ? "" : from.supplier.Name;
                //if (to.WareHouseId.Count <= 0) to.WareHouseId.Add(from.WareHouseIds);
                //to.MedicalDrugExtensions = from.MedicalDrugExtensions == null ? new MedicalDrugExtensionOutput() : from.MedicalDrugExtensions.Where(t => t.IsCurrentUse).FirstOrDefault();

                to.Specifications = from.SpecificationsUnit;
                //最小剂量+剂量单位*规格  2.5g*15粒*3板/盒 10.00ml*10支/盒	  0.13g*6粒*3袋/盒
                string minDose = "";
                //if (from.MedicalItemType == 1 || from.MedicalItemType == 2 || from.MedicalItemType ==4)//药品规格
                //{
                //    if (from.DoseMin.HasValue && from.doseUnits != null)
                //        minDose = $"{from.DoseMin}{from.doseUnits.SpeUnitUS}";
                //    if ((Convert.ToDouble(from.Specifications) * Convert.ToDouble(from.Packaging)) != 1)
                //    {
                //        minDose += minDose==""? (Convert.ToDouble(from.Specifications) * Convert.ToDouble(from.Packaging)) + "" : "*" + (Convert.ToDouble(from.Specifications) * Convert.ToDouble(from.Packaging)) + "";
                //    }
                //    if (from.SpecificationsUnits != null)
                //    {
                //        minDose += from.SpecificationsUnits.SpeUnitCHS;
                //    }
                //    //if (from.Packaging != "1")
                //    //{
                //    //    minDose += from.Packaging;
                //    //}
                //    if (from.PackageUnits != null)
                //    {
                //        minDose += "/" + from.PackageUnits.SpeUnitCHS;
                //    }
                //    to.MinDose = minDose;
                //}
                ////if (from.SpecificationsUnits != null && from.PackageUnits != null)
                ////    to.MinDose = $"{from.DoseMin}{from.doseUnits.SpeUnitUS}*{from.Packaging}{from.SpecificationsUnits.SpeUnitCHS}/{from.PackageUnits.SpeUnitCHS}"; //from.Specifications;
                //else
                to.MinDose = from.Specifications;
                to.WareHouseId = from.WareHouseParentIds + "" != "" ? from.WareHouseParentIds.Split(',').ToList() : new List<string>();
                to.FormValue = from.dosageForm == null ? "" : from.dosageForm.TypeName;
                to.PackageUnitValue = from.PackageUnits == null ? "" : from.PackageUnits.SpeUnitCHS + "" != "" ? from.PackageUnits.SpeUnitCHS : from.PackageUnits.SpeUnitUS;

                to.SpecificationsValue = from.SpecificationsUnits == null ? "" : from.SpecificationsUnits.SpeUnitCHS + "" != "" ? from.SpecificationsUnits.SpeUnitCHS : from.SpecificationsUnits.SpeUnitUS;
                to.DoseUnitValue = from.doseUnits == null ? "" : from.doseUnits.SpeUnitUS + "" != "" ? from.doseUnits.SpeUnitUS : from.doseUnits.SpeUnitCHS;
                to.ProcurementUnitValue = from.ProcurementUnits == null ? "" : from.ProcurementUnits.SpeUnitCHS + "" != "" ? from.ProcurementUnits.SpeUnitCHS : from.ProcurementUnits.SpeUnitUS;

                to.IsMedCanal = from.IsMedCanal.HasValue ? from.IsMedCanal.Value ? 1 : 0 : 0;
            });



            CreateMap<MedicalItemRecord, CenterMedicalItemRecordOutPut>().AfterMap((from, to) =>
            {
                to.supplierName = from.supplier == null ? "" : from.supplier.Name;

                to.Specifications = from.SpecificationsUnit;
                to.MinDose = from.Specifications;

            });
            CreateMap<MedicalItemRecordInput, MedicalItemRecord>().AfterMap((from, to) =>
            {
                to.WareHouseIds = from.WareHouseId;
                to.SpecificationsUnit = from.Specifications;
                to.Specifications = from.MinDose;


            });
            //
            CreateMap<MedicalDrugExtension, MedicalDrugExtensionOutput>().AfterMap((from, to) =>
            {
                to.medicalItemRecordName = from.medicalItemRecord.MedicalItemName;
                to.CenterName = from.centerDialysis == null ? "统一价" : from.centerDialysis.ShortName;

            });
            //MedicalDrugExtensionInput
            CreateMap<MedicalDrugExtensionInput, MedicalDrugExtension>();
            CreateMap<MedicalRelevancy, MRelevancyOutPut>();

            CreateMap<DosageFormInput, DosageForm>();
            CreateMap<DosageForm, DosageFormOutput>();

            CreateMap<DosageForm, DosageFormCenterOutput>();
            CreateMap<MedicalUnitInput, MedicalUnit>();
            CreateMap<MedicalUnit, MedicalUnitOutput>();
            CreateMap<UseWayInput, UseWay>();
            CreateMap<UseWay, UseWayOutput>();



            CreateMap<PurchaseRequest, PurchaseRequestOutPut>().AfterMap((from, to) =>
            {
                to.CenterName = from.centerDialysis != null ? from.centerDialysis.ShortName : "集团合并单";
                to.CenterId = from.CenterId;
                to.AuditDates = from.AuditDate.Value.ToString("yyyy-MM-dd HH:mm:ss");

                if (from.PurchaseRequestsOrders != null && from.PurchaseRequestsOrders.Count > 0)
                {
                    foreach (var item in from.PurchaseRequestsOrders)
                    {
                        if (item.IsDelete == false)
                            to.purchaseToOrder.Add(new PurchaseToOrder() { MedicaItemType = item.MedicalItemType, OrdreNO = item.OrderNo, supplierId = item.SupplierId });
                    }

                }
                if (from.WarehouseCatalog != null)
                    to.CatalogueName = from.WarehouseCatalog.Name;
                to.TotalQty = from.ApprovalTotalQty;


            });

            CreateMap<PurchaseDetail, PurchaseDetailOutPut>().AfterMap((from, to, context) =>
            {

                if (from.medicalItemRecord != null)
                {
                    to.medicalItemRecordOutPut = context.Mapper.Map<MedicalItemRecordOutPut>(from.medicalItemRecord);
                    if (from.medicalItemRecord.MedicalDrugExtensions != null)
                    {
                        to.medicalItemRecordOutPut.MedicalDrugExtension = context.Mapper.Map<MedicalDrugExtensionOutput>(from.medicalItemRecord.MedicalDrugExtensions.Where(t => t.IsCurrentUse == true).FirstOrDefault());
                        if (to.medicalItemRecordOutPut.MedicalDrugExtension != null && to.medicalItemRecordOutPut.HiCenterCode + "" != "")
                            to.SocialSecurityPrice = to.medicalItemRecordOutPut.MedicalDrugExtension.SocialSecurityPrice;
                    }
                }


                //if (!to.InPrice.HasValue || to.InPrice <= 0)
                //{
                //    if (to.medicalItemRecordOutPut.MedicalDrugExtension != null)
                //    {
                //        to.InPrice = to.medicalItemRecordOutPut.MedicalDrugExtension.PurchasingPrice;

                //    }
                //}

                if (to.medicalItemRecordOutPut != null && to.medicalItemRecordOutPut.MedicalItemType == 1 && to.InPrice.HasValue && to.InPrice > 0)
                {
                    /*
                     零售价≤13元时，采购价=零售价/（1+30%）；
13元＜零售价≤125元时，采购价=零售价/（1+25%）；
125元＜零售价≤360元时，采购价=零售价/（1+20%）；
360元＜零售价≤575元时，采购价=零售价/（1+15%）；
零售价＞575元时，采购价=零售价-75。
                     */

                    decimal price = to.InPrice.Value;
                    if (price <= 10)
                        to.DualPrice = price * Convert.ToDecimal(1.3);
                    if (price > 10 && price <= 100)
                        to.DualPrice = price * Convert.ToDecimal(1.25);
                    if (price > 100 && price <= 300)
                        to.DualPrice = price * Convert.ToDecimal(1.2);
                    if (price > 300 && price <= 500)
                        to.DualPrice = price * Convert.ToDecimal(1.15);
                    if (price > 500)
                        to.DualPrice = price + Convert.ToDecimal(75);
                }

                if (from.supplier != null)
                {
                    to.SupplierName = from.supplier.Name;
                }

                if (from.purchaseRequest != null && from.purchaseRequest.centerDialysis != null)
                    to.CenterName = from.purchaseRequest.centerDialysis.ShortName.Replace("透析中心", "").Replace("门诊部", "");
                to.CenterId = from.purchaseRequest.CenterId;
                if (from.purchaseRequest != null)
                    to.IsUpdate = from.purchaseRequest.GroupAuditStatus == "3" ? false : true;
                else
                    to.IsUpdate = true;

                if (from.purchaseOrder != null)
                    to.OrderNo = from.purchaseOrder.OrderNo;
                else
                    to.OrderNo = "";

                if (from.Remark == "手动调整供应商，价格")
                    to.Remark = "";

                //采购系数
                if (to.MonthAverage > 0)
                    to.coefficient = Math.Round(((to.ApprovalQty + to.CurrentInventory + 0) / to.MonthAverage).Value, 1);
                else
                    to.coefficient = 1;

            });
            //PurchaseDetail
            CreateMap<PurchaseDetailInPut, PurchaseDetail>();
            CreateMap<OrderInput, PurchaseOrder>();
            CreateMap<PurchaseOrder, OrderQueryOutPut>().AfterMap((from, to) =>
            {

                to.SupplierName = from.supplier != null ? from.supplier.Name : "";
                to.centerName = from.center != null ? from.center.ShortName : "";
                to.MedicalItemTypeName = from.warehouseCatalog != null ? from.warehouseCatalog.Name : "其他";
            });
            CreateMap<OrderDetail, OrderDetailData>().AfterMap((from, to) =>
        {

            to.CenterName = from.Center != null ? from.Center.ShortName : "采购合计";
            if (from.Founder == "0")
                to.CenterName = "采购总合计";
            if (from.Medical != null)
            {
                to.sFlag = (from.Medical.IsSplited || from.Medical.Packaging == "/") ? from.Medical.Specifications : from.Medical.Packaging;
                to.IsHIS = from.Medical.NationItemCode + "" == "" ? false : true;
                if (from.Medical.IsSplited)
                    to.Uint = from.Medical.SpecificationsUnits != null ? from.Medical.SpecificationsUnits.SpeUnitCHS : "";
                else
                    to.Uint = from.Medical.PackageUnits != null ? from.Medical.PackageUnits.SpeUnitCHS : "";

            }
            // to.Uint = from.Medical != null && from.Medical.PackageUnits != null ? from.Medical.PackageUnits.SpeUnitCHS : "";
            if (from.Medical != null)
            {
                if (from.Medical.Specifications == from.Medical.ProcurementPackage)
                    to.Specifications = from.Medical.Specifications;
                else if (from.Medical.ProcurementPackage == "/" || from.Medical.ProcurementPackage == @"\")
                    to.Specifications = from.Medical.Specifications;
                else
                {
                    if (from.Medical.MedicalItemType == 1)
                        to.Specifications = from.Medical != null ? from.Medical.ProcurementPackage : "";
                    else
                        to.Specifications = from.Medical != null ? from.Medical.ProcurementPackage + "(" + from.Medical.Specifications + ")" : "";
                }

            }
            else
            { to.Specifications = ""; }
            if (from.Medical != null && from.Medical.MedicalItemType == 3 && from.HurrySlowly + "" != "")
                to.Specifications = from.HurrySlowly;
            to.Manufacturer = from.Medical != null ? from.Medical.Manufacturer : "";
            // var name = string.IsNullOrWhiteSpace(_data.GoodsName) ? "" : "(" + _data.GoodsName + ")" + (string.IsNullOrWhiteSpace(_data.Brand) ? "" : "(" + _data.Brand + ")");
            // _data.MedicalItemName = _data.MedicalItemName + name;
            to.MedicalName = from.Medical != null ? from.Medical.MedicalItemName + (from.Medical.GoodsName + "" != "" ? "(" + from.Medical.GoodsName + ")" : "") : "";
            to.Ncode = from.Medical != null ? from.Medical.NationItemCode : "";
            to.ActualQty = to.ActualQty.HasValue ? to.ActualQty : 0;
            to.Remark = from.Remarks;
            // to.SupplierId = from.purchaseOrder != null && from.purchaseOrder.supplier != null ? from.purchaseOrder.supplier.Id : "";
            to.MedicalThan = from.Medical != null ? from.Medical.MedicalThan : "";
            to.SupplierName = from.supplier != null ? from.supplier.Name : "";
            to.IsSellFree = from.Medical != null ? from.Medical.IsSellFree : false;
            if (from.Medical != null && (from.Medical.MedicalItemType == 1 || from.Medical.MedicalItemType == 2) && to.PurchPrice.HasValue && to.PurchPrice > 0)
            {
                /*
                 零售价≤13元时，采购价=零售价/（1+30%）；
13元＜零售价≤125元时，采购价=零售价/（1+25%）；
125元＜零售价≤360元时，采购价=零售价/（1+20%）；
360元＜零售价≤575元时，采购价=零售价/（1+15%）；
零售价＞575元时，采购价=零售价-75。
                 */
                decimal price = to.PurchPrice.Value;
                if (from.Medical.MedicalItemType == 1)
                {
                    if (price <= 10)
                        to.DualPrice = (price * Convert.ToDecimal(1.3)).ToFloorRound();
                    if (price > 10 && price <= 100)
                        to.DualPrice = (price * Convert.ToDecimal(1.25)).ToFloorRound();
                    if (price > 100 && price <= 300)
                        to.DualPrice = (price * Convert.ToDecimal(1.2)).ToFloorRound();
                    if (price > 300 && price <= 500)
                        to.DualPrice = (price * Convert.ToDecimal(1.15)).ToFloorRound();
                    if (price > 500)
                        to.DualPrice = (price + Convert.ToDecimal(75)).ToFloorRound();
                    if (from.Medical.IsSellFree) to.DualPrice = 0;
                }
                if (from.Medical.MedicalItemType == 2)
                {

                    //to.DualPrice = (price * Convert.ToDecimal(1.15)).ToFloorRound();
                    //2025年6月27日16:01:47 修改为固定价格

                    to.DualPrice = from.Medical.MedicalDrugExtensions.Where(t => t.IsCurrentUse && t.CenterId == "0").First().RetailPrice;
                    if (from.Medical.IsSellFree) to.DualPrice = 0;

                }
            }
            if (from.Medical != null && from.Medical.NationItemCode + "" != "" && from.Medical.SI_CheckPriceInfoS != null)
            {
                //  to.SocialSecurityPrice = from.Medical.MedicalDrugExtensions.Where(t => t.IsCurrentUse).First().SocialSecurityPrice;
                to.SocialSecurityPrice = from.Medical.SI_CheckPriceInfoS.hilist_pric_uplmt_amt;
                if (to.SocialSecurityPrice == 0) to.SocialSecurityPrice = null;
            }

        });
            CreateMap<ReturnRequest, ReturnRequestOutPut>().AfterMap((from, to) =>
                      {

                          to.SupplierName = from.Supplier != null ? from.Supplier.Name : "";

                          to.CenterName = from.center != null ? from.center.ShortName : "";
                          to.Auditor = from.employee != null ? from.employee.Name : "";
                      });

            CreateMap<ReturnDetails, ReturnDetailsOutPut>().AfterMap((from, to) =>
                      {

                          to.MedicalItemName = from.MedicalItem != null ? from.MedicalItem.MedicalItemName : "";

                          to.ProcurementUnit = from.Unit != null ? from.Unit.SpeUnitCHS : "";
                          to.specifications = from.MedicalItem != null ? from.MedicalItem.Packaging + "(" + from.MedicalItem.Specifications + ")" : "";
                          to.manufacturer = from.MedicalItem != null ? from.MedicalItem.Manufacturer : "";
                      });
            //门诊日志

            CreateMap<OutpatientDetailsLog, OutpatientDetailsLogOutPut>()
        .AfterMap((from, to) =>
           {

               to.DiagnosisDate = from.DiagnosisDate.HasValue ? from.DiagnosisDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
               to.MorbidityDate = from.MorbidityDate.HasValue ? from.MorbidityDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
               to.ReportDate = from.ReportDate.HasValue ? from.ReportDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
               to.Sex = from.Patient != null ? from.Patient.Sex : "";
               to.Age = from.Patient != null ? from.Patient.Birthday.HasValue ? (DateTime.Now.Year - from.Patient.Birthday.Value.Year) : 0 : 0;
               to.Professional = from.Patient != null ? from.Patient.Professional : "";
               to.Address = from.Patient != null ? from.Patient.ContactAddress : "";
           });

            CreateMap<DisinfectionRoom, DisinfectionRoomOutPut>()
                      .AfterMap((from, to) =>
                         {

                             to.DisinfectionTime = from.DisinfectionTime.HasValue ? from.DisinfectionTime.Value.ToString("yyyy-MM-dd") : "";
                             to.CenterName = from.CenterDialysis.DialysisName;
                             to.PartitionName = from.Partition.Name;
                             to.IsQualified = from.IsQualified ? "合格" : "不合格";

                         });

            CreateMap<InspectionWaterPollution, InspectionWaterPollutionOutPut>()
                      .AfterMap((from, to) =>
                         {

                             to.DisinfectionTime = from.DisinfectionTime.HasValue ? from.DisinfectionTime.Value.ToString("yyyy-MM-dd") : "";
                             to.CenterName = from.CenterDialysis.DialysisName;

                             to.IsQualified = from.IsQualified ? "合格" : "不合格";
                         });


            CreateMap<PatientInfectiousCheck, PatientInfectiousCheckOutPut>()
                .AfterMap((from, to) =>
                {

                    to.DisinfectionTime = from.DisinfectionTime.HasValue ? from.DisinfectionTime.Value.ToString("yyyy-MM-dd") : "";
                    to.CenterName = from.CenterDialysis != null ? from.CenterDialysis.DialysisName : "";
                    to.PatentName = from.Patient != null ? from.Patient.Name : "";
                    to.PatentSex = from.Patient != null ? from.Patient.Sex : "";
                    to.InfectiousTypeName = from.DicInfectiousType != null ? from.DicInfectiousType.Name : "";
                    to.IsQualified = from.IsQualified ? "完成" : "未完成";
                });

            CreateMap<MedicalIndexesMonth, MedicalIndexesMonthOutPut>()
                .AfterMap((from, to) =>
                {

                    to.IndicatorsName = from.Indicators.ToChinese();
                    if (to.Indicators == MedicalIndicatorsMonth.CureSum || to.Indicators == MedicalIndicatorsMonth.Complications || to.Indicators == MedicalIndicatorsMonth.SeeDoctorSum)
                        //列表样式控制
                        to.cellClassName = new CellClassName() { IndicatorsName = "big-font-cell" };
                    else
                        to.cellClassName = new CellClassName() { IndicatorsName = "small-font-cell" };
                    //同比
                    int lasty = from.LastYear.HasValue ? from.LastYear.Value : 1;
                    lasty = lasty == 0 ? 1 : lasty;
                    to.SameCompared = $"({Convert.ToInt32(from.SameCompared)})   {Math.Round(((from.SameCompared / lasty) * 100).Value, 2)}%";
                    //环比
                    int lasmo = from.LastMonth == 0 ? 1 : from.LastMonth;
                    lasmo = lasmo == 0 ? 1 : lasmo;
                    to.Sequential = $"({Convert.ToInt32(from.Sequential)})   {Math.Round(((from.Sequential / lasmo) * 100).Value, 2)}%";

                });
            //AllCenterMedicalIndexesMonthOutPut
            CreateMap<MedicalIndexesMonth, AllCenterMedicalIndexesMonthOutPut>()
            .AfterMap((from, to) =>
            {

                to.IndicatorsName = from.Indicators.ToChinese();
                to.CenterName = from.centerDialysis.ShortName;
                //同比
                int lasty = from.LastYear.HasValue ? from.LastYear.Value : 1;
                lasty = lasty == 0 ? 1 : lasty;
                to.SameCompared = $"({Convert.ToInt32(from.SameCompared)})   {Math.Round(((from.SameCompared / lasty) * 100).Value, 2)}%";
                //环比
                int lasmo = from.LastMonth == 0 ? 1 : from.LastMonth;
                to.Sequential = $"({Convert.ToInt32(from.Sequential)})   {Math.Round(((from.Sequential / lasmo) * 100).Value, 2)}%";// Math.Round(((from.Sequential / lasmo) * 100).Value, 2) + "%";
            });
            //
            CreateMap<MedicalIndexesYear, MedicalIndexesYearOutPut>()
          .AfterMap((from, to) =>
          {
              to.IndicatorsName = from.Indicators.ToChinese();
          });

            CreateMap<MedicalIndexesYear, AllCenterMedicalIndexesYearOutPut>()
   .AfterMap((from, to) =>
   {
       to.IndicatorsName = from.Indicators.ToChinese();
       to.CenterName = from.centerDialysis.ShortName;
   });

            CreateMap<WaterFuelInPut, WaterFuel>();
            CreateMap<WaterFuel, WaterFuelOutPut>().AfterMap((from, to) =>
            {

                to.CenterName = from.centerDialysis.ShortName;
            });
            CreateMap<InformationInput, Information>();
            CreateMap<NoticeMedicalInput, NoticeMedical>();
            CreateMap<NoticeMedical, NoticeMedicalOutPut>();
            CreateMap<UsersMsg, UserMsgOutPut>();
            CreateMap<Information, InformationOutPut>().AfterMap((from, to, context) =>
            {
                to.SendTime = from.FounderDate;
                to.UserName = from.user.Employee.Name;
                to.noticeMedicalOutPuts = context.Mapper.Map<NoticeMedicalOutPut[]>(from.NoticeMedicals);
                to.UsersMsgs = context.Mapper.Map<UserMsgOutPut[]>(from.UsersMsgs);
            });
            CreateMap<FeedbackReply, FeedbackReplyOutPut>().AfterMap((from, to) =>
            {
                to.ReplyTime = from.ReplyTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            });
            CreateMap<Feedback, FeedbackOutPut>().AfterMap((from, to) =>
            {
                to.BackUserName = from.user.Name;
            });

            CreateMap<Feedback, FeedbackReplysOutPut>().AfterMap((from, to, context) =>
            {
                to.BackUserName = from.user.Name;
                to.BackTime = from.BackTime.Value.ToString("yyyy-MM-dd HH:mm:ss");

                to.feedbackReplyOutPuts = context.Mapper.Map<FeedbackReplyOutPut[]>(from.feedbackReplys);
            });


            CreateMap<SysPublishInfoInput, SysPublishInfo>();
            CreateMap<SysPublishInfo, SysPublishInfoOutPut>();

            CreateMap<AppPublishInfoInput, AppPublishInfo>();
            CreateMap<AppPublishInfo, AppPublishInfoOutPut>();

        }
    }

}
