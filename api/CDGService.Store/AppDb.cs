using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using CDGService.Data.Datas;
using CDGService.Data;
using System.Linq;

namespace CDGService.Store
{
    /// <summary>
    /// 系统用到的数据库
    /// </summary>
    public class AppDb : DbContext
    {
        static AppDb()
        {

        }

        public AppDb(DbContextOptions<AppDb> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SI_CheckPriceInfo>().HasKey(t => t.rid);
            modelBuilder.Entity<SI_FWXM>().HasKey(t => t.LYMLBM);
            modelBuilder.Entity<SI_HC>().HasKey(t => t.YLMLBM);
            modelBuilder.Entity<SI_YP>().HasKey(t => t.YLMLBM);
            modelBuilder.Entity<SI_YBcatalog>().HasKey(t => t.Id);
            modelBuilder.Entity<SI_OwnExpenseInfo>().HasKey(t => t.ID);
            modelBuilder.Entity<Employee>().HasIndex(t => t.Name).IsUnique();
            //  modelBuilder.Entity<Employee>().hasre
            modelBuilder.Entity<User>().HasIndex(t => t.UserName).IsUnique();
            modelBuilder.Entity<UserToken>().HasIndex(t => t.Token).IsUnique();
            modelBuilder.Entity<SysParameter>().HasIndex(t => t.ParamName).IsUnique();
            //modelBuilder.Entity<Employee>().HasAlternateKey(l => l.position).(p => p.PrimaryContactFor);

            //取消联级删除
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                                 .SelectMany(t => t.GetForeignKeys());
            // .Where(fk => !fk.IsOwnership);
            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }


            //modelBuilder.Entity<PrescriptionDetail>().RemoeForeignKey("PrescriptionId");
            //modelBuilder.Entity<PrescriptionDetail>().RemoeForeignKey("OutboundId");

            //modelBuilder.Entity<PrescriptionDetail>().RemoeForeignKey("PrescriptionId");
            //modelBuilder.Entity<PrescriptionDetail>().RemoeForeignKey("OutboundId");

            // modelBuilder.Entity<PrescriptionDetail>().RemoeForeignKey("PrescriptionId");
            //modelBuilder.Entity<PrescriptionDetail>().RemoeForeignKey("OutboundId");



        }

        #region  运行管理

        public DbSet<CenterDialysis> CenterDialysiss { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public DbSet<EmployeeTransfer> EmployeeTransfers { get; set; }
        public DbSet<PerEmpTransfer> PerEmpTransfers { get; set; }

        public DbSet<Patient> Patient { get; set; }

        public DbSet<TransferRecord> TransferRecord { get; set; }


        public DbSet<EquipmentInfo> EquipmentInfo { get; set; }

        public DbSet<EquipmentType> EquipmentType { get; set; }
        public DbSet<EquipmentSupplier> EquipmentSupplier { get; set; }
        public DbSet<EquipmentModel> EquipmentModel { get; set; }
        public DbSet<EquipmentManufacturers> EquipmentManufacturers { get; set; }
        public DbSet<EquipmentBedConfigur> EquipmentBedConfigur { get; set; }
        public DbSet<MaintenanceRecord> MaintenanceRecord { get; set; }
        public DbSet<EquipmentKeepRecord> EquipmentKeepRecord { get; set; }
        public DbSet<BiochemicalTest> BiochemicalTests { get; set; }

        public DbSet<MedicalHistoryFirstPage> MedicalHistoryFirstPage { get; set; }
        public DbSet<FirstPageItemCurrentDiagnosis> FirstPageItemCurrentDiagnosis { get; set; }
        public DbSet<VascularAccessRecord> VascularAccessRecord { get; set; }
        public DbSet<InfectiousDiseaseRegister> InfectiousDiseaseRegister { get; set; }
        public DbSet<AllergyRegister> AllergyRegister { get; set; }
        public DbSet<TumorRegister> TumorRegister { get; set; }

        public DbSet<FirstOutpatientRecord> FirstOutpatientRecord { get; set; }
        //float OutpatientMedicalRecords

        public DbSet<OutpatientMedicalRecord> OutpatientMedicalRecord { get; set; }
        public DbSet<HypertensiveRecord> HypertensiveRecords { get; set; }
        public DbSet<ComplicationsHistoryRecord> ComplicationsHistoryRecords { get; set; }
        public DbSet<GeneticHistoryRecord> GeneticHistoryRecords { get; set; }
        public DbSet<FallHistoryRecord> FallHistoryRecords { get; set; }
        public DbSet<PastHistoryRecord> PastHistoryRecords { get; set; }
        public DbSet<DrugTakingRecord> DrugTakingRecords { get; set; }
        public DbSet<GoutRecord> GoutRecords { get; set; }
        public DbSet<DrugAbuseRecord> DrugAbuseRecords { get; set; }
        public DbSet<KidneyTransplantRecord> KidneyTransplantRecords { get; set; }

        public DbSet<DialysisAdequacyRecord> DialysisAdequacyRecord { get; set; }
        public DbSet<NursingAssessmentRecord> NursingAssessmentRecord { get; set; }
        public DbSet<FallAssessmentAndNursingPlan> FallAssessmentAndNursingPlan { get; set; }
        public DbSet<PatientNutritionAssessmentRecord> PatientNutritionAssessmentRecord { get; set; }

        public DbSet<PatientHealthEducationRecord> PatientHealthEducationRecords { get; set; }

        public DbSet<LaboratoryItemSetting> LaboratoryItemSetting { get; set; }
        public DbSet<LaboratoryItemRecord> LaboratoryItemRecord { get; set; }
        public DbSet<LaboratoryCategorySetting> LaboratoryCategorySetting { get; set; }




        #endregion


        #region  账户角色权限

        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuButton> MenuButtons { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserToken> UserTokens { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<RoleUser> RoleUsers { get; set; }

        #endregion
        #region 基础数据表    

        /// <summary>
        /// 日志信息表
        /// </summary>
        public DbSet<Log> Logs { get; set; }
        /// <summary>
        /// 行政区划
        /// </summary>
        public DbSet<SysRegion> SysRegions { get; set; }
        /// <summary>
        ///字典类型
        /// </summary>

        public DbSet<DictionaryType> DictionaryTypes { get; set; }

        public DbSet<SystemDictionary> SystemDictionarys { get; set; }

        public DbSet<WarehouseCatalog> CatalogueItems { get; set; }
        #endregion
        #region 文档、制度

        public DbSet<Document> Documents { get; set; }
        #endregion
        #region 配置相关

        /// <summary>
        /// 系统参数
        /// </summary>
        public DbSet<SysParameter> SysParameters { get; set; }
        public DbSet<DataUpdate> dataUpdates { get; set; }
        #endregion

        public DbSet<TempTab> TempTab { get; set; }
        //统计
        public DbSet<PatientStatis> PatientStatiss { get; set; }

        public DbSet<MedicalItemRecord> MedicalItemRecords { get; set; }
        public DbSet<MedicalRelevancy> MedicalRelevancy { get; set; }
        public DbSet<MedMatchCode> MedMatchCode { get; set; }

        public DbSet<MedMonthAudit> MedMonthAudit { get; set; }
        public DbSet<MedicalDrugExtension> MedicalDrugExtensions { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<DosageForm> DosageForms { get; set; }


        public DbSet<MedicalUnit> MedicalUnits { get; set; }
        public DbSet<UseWay> UseWays { get; set; }


        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<PurchaseDetail> PurchaseDetails { get; set; }
        public DbSet<GroupMergePurchase> GroupMergePurchase { get; set; }
        public DbSet<PurchaseApprove> PurchaseApproves { get; set; }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<OrderApprove> OrderApproves { get; set; }
        public DbSet<PurchaseRequestsOrder> PurchaseRequestsOrders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ApprovalConfig> ApprovalConfig { get; set; }


        public DbSet<OutpatientDetailsLog> OutpatientDetailsLogs { get; set; }
        //public DbSet<OutpatientLog> OutpatientLogs { get; set; }

        public DbSet<DisinfectionRoom> DisinfectionRooms { get; set; }

        public DbSet<InspectionWaterPollution> InspectionWaterPollutions { get; set; }

        public DbSet<PatientInfectiousCheck> PatientInfectiousChecks { get; set; }

        public DbSet<SI_YPMLS> SI_YPML { get; set; }
        public DbSet<SI_ZLXMS> SI_ZLXM { get; set; }
        public DbSet<SI_BZML> SI_BZML { get; set; }



        //质控
        #region 检验结果
        public DbSet<RoutineBloodRecord> RoutineBloodRecords { get; set; }
        public DbSet<SevenElectrolyteTermsRecord> SevenElectrolyteTermsRecords { get; set; }

        public DbSet<RenalFunctionRecord> RenalFunctionRecords { get; set; }

        public DbSet<LiverFunctionBloodFatBoodSugarRecord> LiverFunctionBloodFatBoodSugarRecords { get; set; }


        public DbSet<MineralAndBoneMetabolismRecord> MineralAndBoneMetabolismRecords { get; set; }

        public DbSet<InspectionOtherRecord> InspectionOtherRecords { get; set; }


        public DbSet<IronParametersRecord> IronParametersRecords { get; set; }
        public DbSet<AnemiaThreeRecord> AnemiaThreeRecords { get; set; }
        public DbSet<CoagulationFiveRecord> CoagulationFiveRecords { get; set; }
        public DbSet<HypertensionFourRecord> HypertensionFourRecords { get; set; }
        public DbSet<InfectiousDiseasesRecord> InfectiousDiseasesRecords { get; set; }
        public DbSet<MyocardialEnzymeFiveRecord> MyocardialEnzymeFiveRecords { get; set; }
        #endregion
        //StageSummary
        public DbSet<StageSummary> StageSummary { get; set; }

        //StageSummary
        public DbSet<DryWeightSet> DryWeightSet { get; set; }


        public DbSet<PreTreatMessage> PreTreatMessage { get; set; }

        public DbSet<CurrentDialysisProgram> CurrentDialysisProgram { get; set; }

        public DbSet<MedicalIndexesMonth> MedicalIndexesMonths { get; set; }
        public DbSet<MedicalIndexesYear> MedicalIndexesYears { get; set; }
        public DbSet<DayItemIncome> DayItemIncomes { get; set; }

        public DbSet<CurePatternCheck> CurePatternChecks { get; set; }
        public DbSet<HistoryDialysisRecords> HistoryDialysisRecords { get; set; }
        public DbSet<PatientCycleScheduling> PatientCycleScheduling { get; set; }
        public DbSet<ObserveRecord> ObserveRecord { get; set; }
        public DbSet<PostPreTreatMessage> PostPreTreatMessage { get; set; }
        public DbSet<VascularMaintainRecord> VascularMaintainRecord { get; set; }

        //收费结算 医保相关
        public DbSet<BalanceMain> BalanceMain { get; set; }
        public DbSet<BalanceDetails> BalanceDetails { get; set; }
        public DbSet<BalancePayType> BalancePayType { get; set; }
        public DbSet<DayBalanceDetail> DayBalanceDetail { get; set; }
        public DbSet<DayBalanceMain> DayBalanceMain { get; set; }
        public DbSet<SI_JSMX> SI_JSMX { get; set; }
        public DbSet<SI_JZDJMX> SI_JZDJMX { get; set; }
        public DbSet<SICompare> SICompare { get; set; }

        public DbSet<SI_CFMX> SI_CFMX { get; set; }

        public DbSet<DialysisProgram> DialysisProgram { get; set; }
        //财务报表相关表
        public DbSet<BatchNoSecondaryStoreroom> BatchNoSecondaryStoreroom { get; set; }
        public DbSet<BatchNoSecondaryStoreroomDetail> BatchNoSecondaryStoreroomDetail { get; set; }
        public DbSet<MaterialOutbound> MaterialOutbound { get; set; }
        public DbSet<MaterialOutboundDetail> MaterialOutboundDetail { get; set; }
        public DbSet<Prescription> Prescription { get; set; }
        public DbSet<PrescriptionDetail> PrescriptionDetail { get; set; }

        public DbSet<PrescriptionDetailRefund> PrescriptionDetailRefund { get; set; }
        public DbSet<PutInStorageBatchNoDetails> PutInStorageBatchNoDetails { get; set; }
        public DbSet<PutInStorageBill> PutInStorageBill { get; set; }
        public DbSet<PutInStorageBillDetails> PutInStorageBillDetails { get; set; }
        public DbSet<PutOutFlow> PutOutFlow { get; set; }
        public DbSet<ReturnDetails> ReturnDetails { get; set; }
        public DbSet<ReturnRequest> ReturnRequest { get; set; }

        public DbSet<WareHouseCatalogs> WareHouseCatalog { get; set; }
        public DbSet<MaterialTypeWarehouseConfig> MaterialTypeWarehouseConfig { get; set; }
        public DbSet<AssetSecondaryStoreList> AssetSecondaryStoreList { get; set; }

        public DbSet<WaterFuel> WaterFuels { get; set; }

        //消息中心
        public DbSet<UsersMsg> UsersMsgs { get; set; }
        public DbSet<Information> Informations { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<FeedbackReply> FeedbackReplys { get; set; }

        public DbSet<SysPublishInfo> SysPublishInfo { get; set; }
        public DbSet<CenterPublishLog> CenterPublishLog { get; set; }

        public DbSet<AppPublishInfo> AppPublishInfo { get; set; }
        public DbSet<NoticeMedical> NoticeMedicals { get; set; }


        //新医保对照
        public DbSet<SI_YP> SI_YP { get; set; }
        public DbSet<SI_HC> SI_HC { get; set; }
        public DbSet<SI_FWXM> SI_FWXM { get; set; }
        public DbSet<SI_OwnExpenseInfo> SI_OwnExpenseInfo { get; set; }
        public DbSet<SI_CheckPriceInfo> SI_CheckPriceInfo { get; set; }
        public DbSet<SI_YBcatalog> SI_YBcatalog { get; set; }
        public DbSet<MaterialApplyApprove> MaterialApplyApprove { get; set; }
        public DbSet<MaterialApplyApproveDetail> MaterialApplyApproveDetail { get; set; }

        public DbSet<MaterialsWarningApplyList> MaterialsWarningApplyList { get; set; }
        public DbSet<MaterialsWarningApplyDetailList> MaterialsWarningApplyDetailList { get; set; }
    }
}
