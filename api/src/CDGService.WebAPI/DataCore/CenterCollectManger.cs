using CDGService.Data.Datas;
using CDGService.Data.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.DataCore
{
    public class CenterCollectManger
    {
        private readonly IGetUserInfo _getUserInfo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _logManager;

        private IRepository<Log> LogStore => _unitOfWork.GetStore<Log>();
        /// <summary>
        /// 中心端采集对接
        /// </summary>
        public CenterCollectManger(IUnitOfWork unitOfWork, LogManager logManager, IGetUserInfo getUserInfo)
        {

            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
            _logManager = logManager;

        }

        #region 员工


        #endregion

        #region


        #endregion 档案
    }
}
