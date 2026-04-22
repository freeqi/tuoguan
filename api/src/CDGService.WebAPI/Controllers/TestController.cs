using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CDGService.WebAPI.Datas;
using Microsoft.AspNetCore.Mvc; 

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CDGService.WebAPI.Controllers
{
    /// <summary>
    /// 测试Controller
    /// </summary>
    [Route("api/[controller]")]
    public class TestController : Controller
    {
       

        public TestController(
            )
        {
            
        }

        //public virtual Task<ServiceMessage<string>> Index()
        //{
        //    var startTime = new DateTime(2016, 12, 7, 1, 0, 0).ToUniversalTime();
        //    var endTime = new DateTime(2016, 12, 7, 10, 0, 0).ToUniversalTime();
        //    var stationNum = "D6021"; //百花公园
        //    var rainDataResult = _hourDataService.GetHourRainfallDataListByTimeSection(startTime, endTime);
        //    var generalDataResult = _hourDataService.GetHourGeneralDataListByTimeSectionAndStation(startTime, endTime,
        //        stationNum);
        //    return Task.FromResult(new ServiceMessage<string>("OK"));
        //}
    }
}
