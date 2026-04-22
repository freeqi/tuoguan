 
using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Dto;
using CDGService.WebAPI.Extenstions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Controllers
{
    /// <summary>
    /// 大数据首页接口
    /// </summary>
    [EnableCors("any")] //启用跨域
    [Route("api/[controller]")]
   
    public class BigDataHomeController : Controller
    {
        //接入机构、收录病历、治疗模式、收录患者
        

        #region  人口学统计

        //机构分组患者人数 

        //患者转归人数统计
        //血源性疾病统计
        //性别统计
        //年龄段统计

        #endregion

        #region 进入实时数据监控

        #endregion

        #region  数据中心
        #endregion
    }

}
