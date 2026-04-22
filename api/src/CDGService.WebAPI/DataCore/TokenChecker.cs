using System;
using System.Linq;
using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Extenstions;

namespace CDGService.WebAPI.DataCore
{
    /// <summary>
    /// 令牌验证
    /// </summary>
    public class TokenChecker : IDependency
    {
        private readonly IUnitOfWork _uw;

        public TokenChecker(IUnitOfWork uw)
        {
            _uw = uw;
        }
        /// <summary>
        /// 令牌验证，判断令牌是否过期/账号是否被禁用
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string IsValid(string token)
        {
            var store = _uw.GetStore<UserToken>();

            var s = store.GetFirstOrDefault(t => t.Token == token);
            if (s == null) return "";
            //var tokenData = store.Entities.Where(t => t.UserId == s.UserId).OrderByDescending(t => t.LogingTime).First();
            //if (tokenData.Token != token)
            //    return false;
            s.LastUpdateTime = DateTime.Now;

            _uw.SaveChanges();
            return s.UserId;
        }
    }
}