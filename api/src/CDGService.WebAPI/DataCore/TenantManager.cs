using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.DataCore
{
    public class TenantManager
    {
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<UserTenant> _userTenantRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TenantManager(IRepository<Tenant> tenantRepository, IRepository<UserTenant> userTenantRepository, IRepository<User> userRepository, IUnitOfWork unitOfWork)
        {
            _tenantRepository = tenantRepository;
            _userTenantRepository = userTenantRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 获取租户列表
        /// </summary>
        public async Task<List<TenantOutput>> GetTenantsAsync()
        {
            var tenants = await _tenantRepository.GetAllAsync();
            return tenants.Select(t => new TenantOutput
            {
                Id = t.Id,
                TenantName = t.TenantName,
                TenantCode = t.TenantCode,
                ContactPhone = t.ContactPhone,
                ContactPerson = t.ContactPerson,
                Address = t.Address,
                Status = t.Status,
                StatusText = t.Status == 1 ? "启用" : "停用",
                Remark = t.Remark,
                Founder = t.Founder,
                FounderDate = t.FounderDate,
                Modifier = t.Modifier,
                ModifierDate = t.ModifierDate
            }).ToList();
        }

        /// <summary>
        /// 根据ID获取租户
        /// </summary>
        public async Task<TenantOutput> GetTenantByIdAsync(string id)
        {
            var tenant = await _tenantRepository.GetByIdAsync(id);
            if (tenant == null) return null;

            return new TenantOutput
            {
                Id = tenant.Id,
                TenantName = tenant.TenantName,
                TenantCode = tenant.TenantCode,
                ContactPhone = tenant.ContactPhone,
                ContactPerson = tenant.ContactPerson,
                Address = tenant.Address,
                Status = tenant.Status,
                StatusText = tenant.Status == 1 ? "启用" : "停用",
                Remark = tenant.Remark,
                Founder = tenant.Founder,
                FounderDate = tenant.FounderDate,
                Modifier = tenant.Modifier,
                ModifierDate = tenant.ModifierDate
            };
        }

        /// <summary>
        /// 添加租户
        /// </summary>
        public async Task<bool> AddTenantAsync(TenantInput input)
        {
            var tenant = new Tenant
            {
                Id = Guid.NewGuid().ToString(),
                TenantName = input.TenantName,
                TenantCode = input.TenantCode,
                ContactPhone = input.ContactPhone,
                ContactPerson = input.ContactPerson,
                Address = input.Address,
                Status = input.Status,
                Remark = input.Remark,
                Founder = input.Founder,
                FounderDate = DateTime.Now
            };

            await _tenantRepository.AddAsync(tenant);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新租户
        /// </summary>
        public async Task<bool> UpdateTenantAsync(TenantInput input)
        {
            var tenant = await _tenantRepository.GetByIdAsync(input.Id);
            if (tenant == null) return false;

            tenant.TenantName = input.TenantName;
            tenant.TenantCode = input.TenantCode;
            tenant.ContactPhone = input.ContactPhone;
            tenant.ContactPerson = input.ContactPerson;
            tenant.Address = input.Address;
            tenant.Status = input.Status;
            tenant.Remark = input.Remark;
            tenant.Modifier = input.Modifier;
            tenant.ModifierDate = DateTime.Now;

            _tenantRepository.Update(tenant);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 删除租户
        /// </summary>
        public async Task<bool> DeleteTenantAsync(string id)
        {
            var tenant = await _tenantRepository.GetByIdAsync(id);
            if (tenant == null) return false;

            tenant.IsDelete = true;
            _tenantRepository.Update(tenant);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 分配用户到租户
        /// </summary>
        public async Task<bool> AssignUserToTenantAsync(UserTenantInput input)
        {
            var existing = await _userTenantRepository.GetAllAsync(ut => ut.UserId == input.UserId && ut.TenantId == input.TenantId);
            if (existing.Any()) return false;

            var userTenant = new UserTenant
            {
                Id = Guid.NewGuid().ToString(),
                UserId = input.UserId,
                TenantId = input.TenantId
            };

            await _userTenantRepository.AddAsync(userTenant);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 从租户中移除用户
        /// </summary>
        public async Task<bool> RemoveUserFromTenantAsync(UserTenantInput input)
        {
            var userTenant = await _userTenantRepository.GetAllAsync(ut => ut.UserId == input.UserId && ut.TenantId == input.TenantId);
            if (!userTenant.Any()) return false;

            _userTenantRepository.Delete(userTenant.First());
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 获取租户下的用户
        /// </summary>
        public async Task<List<UserTenantOutput>> GetUsersByTenantIdAsync(string tenantId)
        {
            var userTenants = await _userTenantRepository.GetAllAsync(ut => ut.TenantId == tenantId);
            var userIds = userTenants.Select(ut => ut.UserId).ToList();
            var users = await _userRepository.GetAllAsync(u => userIds.Contains(u.Id));

            return users.Select(u => new UserTenantOutput
            {
                UserId = u.Id,
                UserName = u.UserName,
                RealName = u.RealName,
                TenantId = tenantId
            }).ToList();
        }
    }
}
