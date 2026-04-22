using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Application.BisStatistical
{
    public class PatientStatistical
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IGetUserInfo _getUserInfo;
        public PatientStatistical(IUnitOfWork unitOfWork, IGetUserInfo getUserInfo)
        {
            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
            //  PatientStatisticalInDataAsync();
        }
        private IRepository<Patient> PatientStore => _unitOfWork.GetStore<Patient>();
        private IRepository<SystemDictionary> DictionaryStore => _unitOfWork.GetStore<SystemDictionary>();
        private IRepository<CenterDialysis> DialysisStore => _unitOfWork.GetStore<CenterDialysis>();
        private IRepository<PatientStatis> PatientStatisStore => _unitOfWork.GetStore<PatientStatis>();

        /// <summary>
        /// 统计患者月增长状况并入库
        /// </summary>
        /// <returns></returns>
        private Task<bool> PatientStatisticalInDataAsync()
        {

            return Task.Run(async () =>
            {
                List<PatientStatis> ListPatientStatis = new List<PatientStatis>();
                bool IsDel = false;
                var CenterData = await DialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToArrayAsync();

                var Pcount = await PatientStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();

                foreach (var item in CenterData)
                {
                    var CenterPData = Pcount.Where(t => t.CenterId == item.Id).ToList();
                    DateTime BeginTime = CenterPData.Min(t => t.ReceiveDate.Value);
                    for (DateTime i = BeginTime; i < DateTime.Now; i.AddMonths(+1))
                    {
                        int ManCount = CenterPData.Where(t => t.Sex == "男" && t.ReceiveDate.Value.Date >= i.Date && t.ReceiveDate.Value.Date <= i.AddMonths(+1).Date).Count();
                        int WomanCount = CenterPData.Where(t => t.Sex == "女" && t.ReceiveDate.Value.Date >= i.Date && t.ReceiveDate.Value.Date <= i.AddMonths(+1).Date).Count();

                        ListPatientStatis.Add(new PatientStatis() { CenterId = item.Id, ManCount = ManCount, WomanCount = WomanCount, PatientDate = i.Date, SumCount = ManCount + WomanCount });
                    }

                }
                PatientStatisStore.Remove(t => t.Id !="" || t.Id !="0");
                PatientStatisStore.Insert(ListPatientStatis.ToArray());

                _unitOfWork.SaveChanges();


                return true;
            });
        }

    }
}
