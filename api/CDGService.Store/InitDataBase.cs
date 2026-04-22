using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CDGService.Data.Datas;
using CDGService.Data.Helper;
using System.Threading.Tasks;
using System.Linq.Expressions;
using CDGService.Utils;

namespace CDGService.Store
{
    /// <summary>
    /// 实始化数据库的方法
    /// </summary>
    public static class InitDataBase
    {


        /// <summary>
        /// 初始化员工信息
        /// </summary>
        /// <param name="dbContext"></param>
        public static void InitEmployeeInfo(this DbContext dbContext)
        {
            //dbContext.SaveChanges();
           
            if (dbContext.Set<Employee>().Any())
                return;

            var Employee = new Employee()
            {
                Id = Guid.NewGuid().tostring32(),
                Name = "系统内部人员",
                Age = 25,
                ContactAddress = "重庆渝北",
            };
            dbContext.Set<Employee>().Add(Employee);

            dbContext.SaveChanges();

        }


        /// <summary>
        /// 初始化用户信息
        /// </summary>
        /// <param name="dbContext"></param>
        public static void InitUserInfo(this DbContext dbContext)
        {
            //dbContext.SaveChanges();

            if (dbContext.Set<User>().Any())
                return;

            var user = new User()
            {
                Id = Guid.NewGuid().tostring32(),
                UserName = "admin",
                Pwd = "123456".MD5(),
                Name = "内置测试用户",

                IsActive = true
            };
            dbContext.Set<User>().Add(user);
            user = new User()
            {
                Id = Guid.NewGuid().tostring32(),
                UserName = "superadmin",
                Pwd = "1234567".MD5(),
                Name = "内置测试用户",

                IsActive = true
            };
            dbContext.Set<User>().Add(user);
            dbContext.SaveChanges();
            var ustore = dbContext.Set<UserToken>();
            // ustore.RemoveRange(ustore.ToArray());
            ustore.Add(new UserToken() { Token = "987654321123456789", User = user, Id = Guid.NewGuid().tostring32() });
            dbContext.SaveChanges();

        }


        /// <summary>
        /// 清理过期的token
        /// </summary>
        /// <param name="db"></param>
        public static void ClearExpireTokens(this DbContext db)
        {
            //Task.Run(async () =>
            //{
            bool IsCollectToken = false;
            try
            {
                Expression<Func<UserToken, bool>> predicate = t => t.IsCollectToken == IsCollectToken;
                predicate = predicate.And(t => t.LastUpdateTime < DateTime.Now.AddDays(-1));
                var data = db.Set<UserToken>().Where(predicate).ToArray();

                db.Set<UserToken>().RemoveRange(data);
                db.SaveChanges();
            }
            catch (Exception exp)
            {

            }
            // var tokens = db.Set<UserToken>();

            //});
        }

    }
}