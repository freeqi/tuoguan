using AutoMapper;
using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CDGService.Data.Helper;
using CDGService.WebAPI.Extenstions;
using Microsoft.EntityFrameworkCore;
using CDGService.WebAPI.Datas;
using System.Linq.Expressions;
using CDGService.Utils;
using CDGService.Data.Enums;

namespace CDGService.WebAPI.DataCore
{
    public class WaterFuelManager
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IGetUserInfo _getUserInfo;
        private readonly LogManager _logManager;
        private IRepository<WaterFuel> WaterFuelStore => _unitOfWork.GetStore<WaterFuel>();

        private IRepository<CenterDialysis> CenterDialysisStore => _unitOfWork.GetStore<CenterDialysis>();
        public WaterFuelManager(IUnitOfWork unitOfWork, IGetUserInfo getUserInfo, LogManager logManager, MenuManager menuManager)
        {
            _unitOfWork = unitOfWork;
            _getUserInfo = getUserInfo;
            _logManager = logManager;
        }


        public Task<bool> AddUpdateWaterFuel(WaterFuelInPut inPut)
        {
            return Task.Run(async () =>
            {
                bool Flag = true;
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    if (inPut.Id + "" == "")
                    {
                        string MainId = Guid.NewGuid().tostring32();
                        foreach (var item in inPut.itemDetails)
                        {
                            WaterFuel data = new WaterFuel();
                            //data = _mapper.Map<WaterFuel>(inPut);
                            data.CenterId = inPut.CenterId;
                            data.ItemType = (BusinessTypeEnum)Enum.Parse(typeof(BusinessTypeEnum), item.ItemType); //item.ItemType;
                            data.AmountPriec = item.AmountPriec;
                            data.Month = inPut.Month;
                            data.Remarks = inPut.Remarks;
                            data.Founder = userId;
                            data.FounderDate = DateTime.Now;
                            data.Modifier = userId;
                            data.ModifierDate = DateTime.Now;
                            data.DataState = 1;
                            data.Id = Guid.NewGuid().tostring32();
                            data.MainId = MainId;
                            WaterFuelStore.Insert(data);
                        }

                    }
                    else
                    {
                        var datas = await WaterFuelStore.Entities.Where(t => t.MainId == inPut.Id).ToListAsync();

                        foreach (var item in inPut.itemDetails)
                        {
                            var temp = (BusinessTypeEnum)Enum.Parse(typeof(BusinessTypeEnum), item.ItemType);
                            datas.Where(t => t.ItemType == temp).FirstOrDefault().AmountPriec = item.AmountPriec;
                            //WaterFuel data = new WaterFuel();
                            ////data = _mapper.Map<WaterFuel>(inPut);
                            //data.CenterId = inPut.CenterId;
                            //data.ItemType = item.ItemType;
                            //data.AmountPriec = item.AmountPriec;
                            //data.Month = inPut.Month;
                            //data.Remarks = inPut.Remarks;
                            //data.Founder = userId;
                            //data.FounderDate = DateTime.Now;
                            //data.Modifier = userId;
                            //data.ModifierDate = DateTime.Now;
                            //data.DataState = 1;
                            //data.Id = Guid.NewGuid().tostring32();

                        }
                        WaterFuelStore.Update(datas);
                    }
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }


                return Flag;

            });

        }


        public Task<bool> DelWaterFuel(string Id)
        {
            return Task.Run(async () =>
            {
                bool Flag = true;
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    var OldData = await WaterFuelStore.Entities.Where(t => t.MainId == Id).ToListAsync();
                    OldData.ForEach(t =>
                    {
                        t.Modifier = userId;
                        t.ModifierDate = DateTime.Now;
                        t.IsDelete = true;
                    });

                    WaterFuelStore.Update(OldData);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return Flag;

            });

        }



        public Task<PageData<WaterFuelOutPut[]>> GetWaterFuelListAsync(WaterFuelQueryInPut input)
        {
            return Task.Run(async () =>
            {
                bool IsDel = false;
                List<WaterFuelOutPut> result = new List<WaterFuelOutPut>();
                int count = 0;

                try
                {
                    {
                        Expression<Func<WaterFuel, bool>> predicate = t => t.IsDelete == IsDel;
                        if (input.CenterId + "" != "" && input.CenterId + "" != "0")
                            predicate = predicate.And(t => t.CenterId == input.CenterId);
                        
                        if (input.BeginTime.HasValue && input.EndTime.HasValue)
                        {
                            DateTime btime = new DateTime(input.BeginTime.Value.Year, input.BeginTime.Value.Month, input.BeginTime.Value.Day, 0, 0, 0);
                            DateTime etime = new DateTime(input.EndTime.Value.Year, input.EndTime.Value.Month, input.EndTime.Value.Day, 23, 59, 59);
                            predicate = predicate.And(t => t.Month > btime && t.Month <= etime);
                        }
                        if (input.PageNum > 0 && input.PageSize > 0)
                        {
                            var Sum = await WaterFuelStore.Entities.Include(t => t.centerDialysis).Where(predicate).ToListAsync();
                            var data = Sum.GroupBy(t => t.MainId);
                            var PageData = data.Skip((input.PageNum - 1) * input.PageSize).Take(input.PageSize);
                            foreach (var item in PageData)
                            {
                                WaterFuelOutPut waterFuelOutPut = new WaterFuelOutPut();
                                List<WaterFuel> waterFuels = item.ToList();
                                foreach (var items in waterFuels)
                                {
                                    // string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(item.ShortName).ToLower();

                                    waterFuelOutPut.GetType().GetProperties().Where(t => t.Name == items.ItemType.ToString()).First().SetValue(waterFuelOutPut, items.AmountPriec);
                                    waterFuelOutPut.Remarks = items.Remarks;
                                    waterFuelOutPut.CenterName = items.centerDialysis.ShortName;
                                    waterFuelOutPut.TotalAmountPriec += items.AmountPriec;
                                    waterFuelOutPut.Id = items.MainId;
                                    waterFuelOutPut.CenterId = items.CenterId;
                                    waterFuelOutPut.Month = items.Month;
                                    waterFuelOutPut.DataState = items.DataState;
                                }

                                result.Add(waterFuelOutPut);
                            }

                            count = data.Count();

                        }
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
                return new PageData<WaterFuelOutPut[]>(result.ToArray(), count);
            });
        }

        #region  统计

        public Task<WaterFuelStatisOutPut[]> GetWaterFuelStatissAsync(WaterFuelQueryInPut input)
        {
            return Task.Run(async () =>
             {
                 List<WaterFuelStatisOutPut> outPuts = new List<WaterFuelStatisOutPut>();

                 //Expression<Func<WaterFuel, bool>> predicate = t => t.IsDelete == false;
                 //if (input.BeginTime.HasValue && input.EndTime.HasValue)
                 //    predicate = predicate.And(t => t.Month.Value > input.BeginTime.Value && t.Month <= input.EndTime.Value);

                 //var Sum = await WaterFuelStore.Entities.Include(t => t.centerDialysis).Where(predicate).ToListAsync();
                 //var centerDta = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.DataState == 1).ToListAsync();
                 //foreach (var item in centerDta)
                 //{
                 //    // Sum.GroupBy(t => t.ItemType);
                 //    WaterFuelStatisOutPut statis = new WaterFuelStatisOutPut()
                 //    {
                 //        CenterName = item.ShortName,
                 //        WaterSum = Sum.Where(t => t.CenterId == item.Id && t.ItemType == 1).Sum(t => t.Dosage),
                 //        WaterPrice = Sum.Where(t => t.CenterId == item.Id && t.ItemType == 1).Sum(t => t.AmountPriec),
                 //        FuelPrice = Sum.Where(t => t.CenterId == item.Id && t.ItemType == 2).Sum(t => t.AmountPriec),
                 //        FuelSum = Sum.Where(t => t.CenterId == item.Id && t.ItemType == 2).Sum(t => t.AmountPriec),
                 //        SumPrice = Sum.Where(t => t.CenterId == item.Id).Sum(t => t.AmountPriec),
                 //    };
                 //    outPuts.Add(statis);
                 //}


                 return outPuts.OrderByDescending(t => t.SumPrice).ToArray();
             });
        }




        #endregion
    }
}
