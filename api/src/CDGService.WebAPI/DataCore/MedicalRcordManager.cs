using CDGService.Data;
using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CDGService.Data.Helper;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CDGService.Store;
using System.Data.SqlClient;

namespace CDGService.WebAPI.DataCore
{
    /// <summary>
    /// 病案管理
    /// </summary>
    public class MedicalRcordManager : XmlSql
    {
        private readonly string _ClassName;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _logManager;
        private readonly IGetUserInfo _getUserInfo;
        private IRepository<Prescription> PrescriptionStore => _unitOfWork.GetStore<Prescription>();
        private IRepository<PrescriptionDetail> PrescriptionDetailStore => _unitOfWork.GetStore<PrescriptionDetail>();
        public MedicalRcordManager(IUnitOfWork unitOfWork, LogManager logManager, IGetUserInfo getUserInfo)
        {
            _ClassName = GetType().Name;//当前类名称
            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
            _logManager = logManager;

        }

        /// <summary>
        ///患者处方列表
        /// </summary>
        public Task<PageData<PrescriptionViewModel[]>> GetPrescriptionByPatientIdAsync(PrescriptionInput model)
        {
            return Task.Run(() =>
            {
                int count = 0;
                var reult_list = new List<PrescriptionViewModel>();
                var sqlParameterList = new List<SqlParameter>();
                sqlParameterList.Add(new SqlParameter("@PatientId", model.PatientId));
                sqlParameterList.Add(new SqlParameter("@CenterId", model.CenterId));
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetAllPrescriptionByPatientId", "根据患者Id查询处方单列表");
                string Sql = GetQuerySql(xmlSqlParameter);
                Sql = string.Format(Sql, model.BeginTime.Value.Date, model.EndTime);
                var tuple = GetTupleByList<PrescriptionViewModel>(MsSqlHelper.GetSingleObj().GetDataTable(Sql, sqlParameterList.ToArray()));
                if (tuple.Item1)
                {
                    var pageData = tuple.Item2.Skip((model.PageIndex.Value - 1) * model.PageSize.Value).Take(model.PageSize.Value);
                    reult_list.AddRange(pageData);
                    count = tuple.Item2.Count;
                }
                return new PageData<PrescriptionViewModel[]>(reult_list.ToArray(), count);
            });
        }


        /// <summary>
        ///根据处方单id查询处方明细 
        /// </summary>
        public Task<PrescriptionDetailViewModel[]> GetPrescriptionDetailByPrescriptionIdAsync(PrescriptionInput model)
        {
            return Task.Run(() =>
            {
                var sqlParameterList = new List<SqlParameter>();
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetAllPrescriptionDetailByPrescriptionId", "根据处方单Id查询处方单明细列表");
                string sql = GetQuerySql(xmlSqlParameter);
                sqlParameterList.Add(new SqlParameter("@PrescriptionId", model.PrescriptionId));
                var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                var tuple = GetTupleByList<PrescriptionDetailViewModel>(dt);
                if (tuple.Item1)
                {
                    foreach (var item in tuple.Item2)
                    {
                        item.MedicalItemName = item.MaterialName;
                        if (item.MedicalItemType == 1)
                        {
                            var GoodsName = string.IsNullOrWhiteSpace(item.GoodsName) ? "" : "(" + item.GoodsName + ")";
                            item.MaterialName = item.MaterialName + GoodsName;
                        }
                        if (item.MedicalItemType == 2)
                        {
                            var Specifications = string.IsNullOrWhiteSpace(item.Specifications) ? "" : "(" + item.Specifications + ")";
                            item.MaterialName = item.MaterialName + Specifications;
                        }
                        item.RefundPrice = (item.RefundTotalQty ?? 0) * item.UnitPrice;
                    }
                }
                return tuple.Item2.ToArray();
            });
        }


        /// <summary>
        ///收费清单
        /// </summary>
        public Task<PageData<FeesforListingModel[]>> GetChargeListByPatientIdAsync(PrescriptionInput model)
        {
            return Task.Run(() =>
            {
                int count = 0;
                var reult_list = new List<FeesforListingModel>();
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetAllFeesForListingByPatientId", "根据患者Id查询收费清单");
                string Sql = GetQuerySql(xmlSqlParameter);
                Sql = string.Format(Sql,model.PatientId,model.CenterId, model.BeginTime.Value.Date, model.EndTime) ;
                var tuple = GetTupleByList<FeesforListingModel>(MsSqlHelper.GetSingleObj().GetDataTable(Sql));
                if (tuple.Item1)
                {
                    var pageData = tuple.Item2.Skip((model.PageIndex.Value - 1) * model.PageSize.Value).Take(model.PageSize.Value);
                    reult_list.AddRange(pageData);
                    count = tuple.Item2.Count;
                }
                return new PageData<FeesforListingModel[]>(reult_list.ToArray(), count);
            });
        }

        /// <summary>
        ///收费清单明细
        /// </summary>
        public Task<FeesforListingDetailModel[]> GetAllFeesForListingDetailByIdAsync(PrescriptionInput model)
        {
            return Task.Run(() =>
            {
                var sqlParameterList = new List<SqlParameter>();
                sqlParameterList.Add(new SqlParameter("@BalanceNo", model.BalanceNo));
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetAllFeesForListingDetailById", "根据Id查询收费清单明细");
                string Sql = GetQuerySql(xmlSqlParameter);
                Sql = string.Format(Sql, model.CenterId);
                var tuple = GetTupleByList<FeesforListingDetailModel>(MsSqlHelper.GetSingleObj().GetDataTable(Sql, sqlParameterList.ToArray()));
                return tuple.Item2.ToArray();
            });
        }
    }
}
