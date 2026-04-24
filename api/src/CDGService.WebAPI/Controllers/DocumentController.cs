using Castle.Core.Internal;
using CDGService.Data.Enums;
using CDGService.Data.Helper;
using CDGService.Utils;
using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Dto;
using CDGService.WebAPI.Extenstions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;

namespace CDGService.WebAPI.Controllers
{
    /// <summary>
    /// 文件
    /// </summary>
    [EnableCors("any")] //启用跨域
    [Route("api/[controller]")]
    public class DocumentController : Controller
    {
        private readonly DocumentManager _documentManager;
        private readonly Data.DocumentSetting _documentSetting;
        private readonly PatientsManager _patientsManager;
        private readonly BusinessTargetStatisticalManager _businessTargetStatisticalManager;

        private readonly CenterDialysisManger _centerDialysisManger;


        //  Application.APPConversionToPDF.ConversionMain _conversionMain;
        public DocumentController(DocumentManager documentManager, PatientsManager patientsManager, IOptions<Data.DocumentSetting> documentSetting, BusinessTargetStatisticalManager businessTargetStatisticalManager, CenterDialysisManger centerDialysisManger)
        {
            _documentManager = documentManager;
            _patientsManager = patientsManager;
            //  _conversionMain = conversionMain;
            _businessTargetStatisticalManager = businessTargetStatisticalManager;
            _centerDialysisManger = centerDialysisManger;
            _documentSetting = documentSetting.Value;
        }
        /// <summary>
        /// 文档-新增
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        [HttpPost("doc/new")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CreateDocAsync([FromBody] DocumentInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _documentManager.CreateDocAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }



        /// <summary>
        /// 获取文档列表。
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        [HttpPost("doc/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]

        public virtual Task<ServiceMessage<DocumentOutput[]>> GetDocQueryableAsync([FromBody] DocumentSearchInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _documentManager.GetDocQueryableAsync(input);
                return new ServiceMessage<DocumentOutput[]>(result.Result, result.Count);
            });
        }




        /// <summary>
        /// 获取知识库分类列表。
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        [HttpPost("doc/DocKnowledgeList/{documentCatalog}")]
        [CheckLogin]
        [ServiceMessageTryCatch]

        public virtual Task<ServiceMessage<DocKnowledgeOutPut[]>> GetDocKnowledgeQueryableAsync([FromRoute] DocumentCatalog documentCatalog)
        {
            return Task.Run(async () =>
            {
                var result = await _documentManager.GetDocKnowledgeQueryableAsync(documentCatalog);
                return new ServiceMessage<DocKnowledgeOutPut[]>(result);
            });
        }
        /// <summary>
        /// 文档-修改
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        [HttpPost("doc/update")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> UpdateDocAsync([FromBody] DocumentModifyInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _documentManager.ModifyDocAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="catalog">模块</param>       
        /// <returns></returns>
        [HttpPost("fileup/{catalog}/{fileName}")]
        [ServiceMessageTryCatch]
        [DisableRequestSizeLimit]  //取消大小的限制
        public virtual Task<ServiceMessage<string>> UpLoadFile([FromRoute] string fileName, [FromRoute] DocumentCatalog catalog)
        {
            string catalogName = catalog.ToChinese();
            if (catalogName.IsNullOrEmpty())
                throw new Exception($"文档目录不能为空");

            string dir = Path.Combine(_documentSetting.DocumentRoot, catalog.ToChinese());
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (Request.Form.Files == null || Request.Form.Files.Count <= 0)
            {
                throw new Exception($"未能获取到上传文件信息");
            }
            try
            {
                var file = Request.Form.Files[0];
                string d = Path.GetExtension(fileName);//扩展名 “.docx” 
                fileName = Guid.NewGuid().tostring16() + d;
                string savePath = Path.Combine(dir, fileName);
                if (System.IO.File.Exists(savePath))
                    throw new Exception($"文件{fileName}已经存在");
                var size = file.Length;
                using (FileStream fs = System.IO.File.Create(savePath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                //转PDF
                //_conversionMain.FilePath = "";
                //_conversionMain.FileValue = savePath;

                //  _conversionMain.Start();
            }
            catch (Exception ex)
            {
                throw new Exception(ex + "", ex);
            }
            return Task.FromResult(new ServiceMessage<string>(fileName));
        }

        /// <summary>
        /// 下载文件(通常用于知识库等文件库管理)
        /// </summary>
        /// <param name="Id">文件ID</param>        
        /// <returns></returns>
        [HttpGet("filedownload/{Id}")]

        [CheckLogin]
        public virtual FileResult GetFile([FromRoute] string Id)
        {

            try
            {


                var task = Task.Run(async () =>
                 {
                     return await _documentManager.GetDocQueryableAsync(new DocumentSearchInput() { Id = Id });
                 });

                var result = task.Result.Result;
                if (result == null || result.Length == 0)
                {
                    throw new Exception("未能获取到文件");
                }
                //   var result = await _documentManager.GetDocQueryableAsync(input);
                var catalogName = result[0].catalog.ToChinese();
                string fileFullPath = Path.Combine(_documentSetting.DocumentRoot, catalogName, result[0].FileName);
                Request.Path = new Microsoft.AspNetCore.Http.PathString($"/api/Document/filedownload2/{(int)result[0].catalog}/{result[0].FileName}");
                if (!System.IO.File.Exists(fileFullPath))
                    throw new Exception($"文件{fileFullPath}不存在");
                var fileResult = new PhysicalFileResult(fileFullPath, "application/x-zip-compressed");
                fileResult.FileDownloadName = result[0].FileName;
                //修改文件下载次数
                var bools = Task.Run(async () =>
                  {
                      return await _documentManager.ModifyDocAsync(new DocumentModifyInput() { Id = Id, download = true });

                  });
                if (bools.Result)
                    return fileResult;
                else
                    throw new Exception("下载文件失败，请联系技术人员");
            }
            catch (Exception exp)
            {

                throw new Exception(exp.Message, exp);
            }
        }


        /// <summary>
		/// 下载文件（用于下载指定文件）
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="catalog"></param>
		/// <returns></returns>
		[HttpGet("filedownload/{catalog}/{fileName}")]
        [ServiceMessageTryCatch]
        [CheckLogin]
        public FileResult GetFile([FromRoute] DocumentCatalog catalog, [FromRoute] string fileName)
        {
            var catalogName = catalog.ToChinese();
            string fileFullPath = string.Empty;
            fileFullPath = Path.Combine(_documentSetting.DocumentRoot, catalogName, fileName);
            Request.Path = new Microsoft.AspNetCore.Http.PathString($"/api/Document/filedownload2/{(int)catalog}/{fileName}");
            if (!System.IO.File.Exists(fileFullPath))
                throw new Exception($"文件{fileFullPath}不存在");
            var fileResult = new PhysicalFileResult(fileFullPath, "application/x-zip-compressed");
            fileResult.FileDownloadName = fileName;
            return fileResult;
        }




        /// <summary>
        /// 下载文件(通常用于知识库等文件库管理)
        /// </summary>
        /// <param name="filec"></param>
        /// <returns></returns>
        [HttpPost("filedownload2")]
        [CheckLogin]

        public virtual FileResult GetFile2([FromBody] FileC filec)
        {

            var task = Task.Run(async () =>
            {
                return await _documentManager.GetDocQueryableAsync(new DocumentSearchInput() { Id = filec.Id });
            });

            var result = task.Result.Result;
            if (result == null || result.Length == 0)
            {
                throw new Exception("未能获取到文件");
            }
            //   var result = await _documentManager.GetDocQueryableAsync(input);
            var catalogName = result[0].catalog.ToChinese();
            string fileFullPath = Path.Combine(_documentSetting.DocumentRoot, catalogName, result[0].FileName);
            Request.Path = new Microsoft.AspNetCore.Http.PathString($"/api/Document/filedownload2/{(int)result[0].catalog}/{result[0].FileName}");
            if (!System.IO.File.Exists(fileFullPath))
                throw new Exception($"文件{fileFullPath}不存在");
            var fileResult = new PhysicalFileResult(fileFullPath, "application/x-zip-compressed");
            fileResult.FileDownloadName = result[0].FileName;
            //修改文件下载次数
            var bools = Task.Run(async () =>
            {
                return await _documentManager.ModifyDocAsync(new DocumentModifyInput() { Id = filec.Id, download = true });

            });
            if (bools.Result)
                return fileResult;
            else
                throw new Exception("下载文件失败，请联系技术人员");
        }


        /// <summary>
        /// 在线预览(通常用于知识库等文件库管理)
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>

        [HttpGet("fileview/{Id}")]
        [CheckLogin]

        public virtual FileResult GetFileView([FromRoute] string Id)
        {

            var task = Task.Run(async () =>
            {
                return await _documentManager.GetDocQueryableAsync(new DocumentSearchInput() { Id = Id });
            });

            var result = task.Result.Result;
            if (result == null || result.Length == 0)
            {
                throw new Exception("未能获取到文件");
            }
            //   var result = await _documentManager.GetDocQueryableAsync(input);
            var catalogName = result[0].catalog.ToChinese();

            string fileName = System.IO.Path.GetFileNameWithoutExtension(result[0].FileName) + ".pdf";
            string fileFullPath = Path.Combine(_documentSetting.PdfRoot, fileName);
            Request.Path = new Microsoft.AspNetCore.Http.PathString($"/api/Document/fileview/{fileName}");
            if (!System.IO.File.Exists(fileFullPath))
                throw new Exception($"文件{fileFullPath}不存在");
            var fileResult = new PhysicalFileResult(fileFullPath, "application/pdf");

            return fileResult;
            //else
            //    throw new Exception("下载文件失败，请联系技术人员");
        }

        /// <summary>
        /// 获取患者病历
        /// </summary>
        /// <param name="Id">患者ID</param>
        /// <param name="records">模块分类：1、病历首页 2、专用病历 3、门诊病历/透析记录单 4、透析方案调整  5、阶段小结 6、护理评估记录 7、健康宣教    8、营养评估   </param>
        /// <returns></returns>
        [HttpGet("Patientsfileview/{Id}/{records}")]
        [CheckLogin]

        public virtual FileResult GetPaintFileView([FromRoute] string Id, [FromRoute] PatientRecords records)
        {

            var data = _patientsManager.GetPatientsQueryableAsync(new PatientQueryInPut() { id = Id, PageNum = 1, PageSize = 9999 }).Result;

            if (data.Result != null && data.Result.Length > 0)
            {
                var patient = data.Result.Where(t => t.Id == Id).FirstOrDefault();
                if (patient == null)
                    throw new Exception($"未能获取到患者信息");
                string fileName = records.ToChinese() + ".pdf";
                // string fileName = patient.Name + "20190715.pdf";//System.IO.Path.GetFileNameWithoutExtension(result[0].FileName) + ".pdf";

                string fileFullPath = Path.Combine(_documentSetting.PdfRoot, "PatientRecords", patient.Name, fileName);
                Request.Path = new Microsoft.AspNetCore.Http.PathString($"/api/Document/fileview/{fileName}");
                if (!System.IO.File.Exists(fileFullPath))
                    throw new Exception($"文件{fileFullPath}不存在");
                var fileResult = new PhysicalFileResult(fileFullPath, "application/pdf");

                return fileResult;
            }
            else
            {
                throw new Exception($"未能获取到患者信息");
            }
            //else
            //    throw new Exception("下载文件失败，请联系技术人员");
        }
        #region  下载

        /*

             #region 文件下载
             /// <summary>
             /// 下载文件
             /// </summary>
             /// <returns></returns>
             [HttpGet("DownloadFile3")]
             public async Task<FileResult> DownloadFile3(string data)
             {
                 //if (string.IsNullOrWhiteSpace(data))
                 //    return NotFound();

                 //var json =   JsonConvert.DeserializeObject<dynamic>(data);
                 //  string fileName = json.FileName;
                 //if (string.IsNullOrWhiteSpace(fileName))
                 //    return NotFound();

                 //var path = _hostingEnvironment.WebRootPath + fileName;
                 //var stream = new FileStream(path, FileMode.Open);
                 //return  File(stream, "application/octet-stream", Path.GetFileName(fileName));//文件流方式，指定文件流对应的ContenType。
                 string fileFullPath = @"D:\FileManager\DocumentManager\培训记录\信息技术部月工作计划表201810.DOC";
               //  string fileName = "编码规范.pdf";
               //  string fileFullPath =
                // fileFullPath = Path.Combine(_documentSetting.DocumentRoot, fileName);// _hostingEnvironment.WebRootPath + fileName;
                 //Request.Path = new Microsoft.AspNetCore.Http.PathString($"/api/Document/filedownload2/{(int)catalog}/{fileName}");
                 if (!System.IO.File.Exists(fileFullPath))
                     throw new Exception($"文件{fileFullPath}不存在");
                 var fileResult = new PhysicalFileResult(fileFullPath, "application/x-zip-compressed");
                 fileResult.FileDownloadName = Path.GetFileName("信息技术部月工作计划表201810.DOC");
                 return fileResult;
             }

             /// <summary>
             /// 下载文件
             /// </summary>
             /// <returns></returns>
             [HttpGet]
             public async Task<IActionResult> DownloadFile1(string data)
             {
                 if (string.IsNullOrWhiteSpace(data))
                     return NotFound();

               //  var json = js.JsonToT<dynamic>(data);
               //  string fileName = json.FileName;
                 //if (string.IsNullOrWhiteSpace(fileName))
                 //    return NotFound();

               //  var path = _hostingEnvironment.WebRootPath + fileName;
                 var memoryStream = new MemoryStream();
                 string fileFullPath = @"D:\FileManager\DocumentManager\培训记录\信息技术部月工作计划表201810.DOC";
                 using (var stream = new FileStream(fileFullPath, FileMode.Open))
                 {
                     await stream.CopyToAsync(memoryStream);
                 }
                 memoryStream.Seek(0, SeekOrigin.Begin);
                 //文件名必须编码，否则会有特殊字符(如中文)无法在此下载。
                 string encodeFilename = System.Web.HttpUtility.UrlEncode(fileFullPath, Encoding.GetEncoding("UTF-8"));
                 Response.Headers.Add("Content-Disposition", "attachment; filename=" + encodeFilename);
                 return new FileStreamResult(memoryStream, "application/octet-stream");//文件流方式，指定文件流对应的ContenType。
             }



             ///// <summary>
             ///// 文件流的方式输出        
             ///// </summary>
             ///// <param name="file"></param>
             ///// <returns></returns>
             [HttpGet("DownloadFile11")]
             public async Task<IActionResult> DownloadFile2(string file)
             {
                 if (string.IsNullOrWhiteSpace(file))
                     return NotFound();

                 var json = JsonConvert.DeserializeObject<dynamic>(file);
                 string fileName = json.FileName;
                 if (string.IsNullOrWhiteSpace(fileName))
                     return NotFound();

                 string fileFullPath = _documentSetting.DocumentRoot+ "//培训记录//" + fileName;
                 string fName = Path.GetFileName(fileFullPath); //xxx.xxx格式，文件全名（带后缀）
                 string suffix = Path.GetExtension(fileName);

                 if (string.IsNullOrWhiteSpace(suffix))
                     return NotFound();

                 Console.OutputEncoding = Encoding.UTF8;
                 var provider = new FileExtensionContentTypeProvider();
                 var memi = provider.Mappings[suffix]; //映射MIME类型
                 var fs = System.IO.File.OpenRead(fileFullPath); //读取文件流
                 return await Task.Run(() => File(fs, memi, System.Web.HttpUtility.UrlEncode(fName))); //页面url解码即可显示中文名称
             }


             ///// <summary>
             /////  用于下载大文件，循环读取大文件的内容到服务器内存，然后发送给客户端浏览器
             ///// </summary>
             ///// <param name="file">文件相对路径</param>
             ///// <returns></returns>
             [HttpGet("DownloadFile123")]
             public async Task<IActionResult> DownloadBigFile(string file)
             {
                 if (string.IsNullOrWhiteSpace(file))
                     return NotFound();

                 var json = JsonConvert.DeserializeObject<dynamic>(file);
                 string fileName = json.FileName;
                 string fileFullPath = _documentSetting.DocumentRoot + @"\培训记录\" + fileName;

                 //要下载的文件地址，这个文件会被分成片段，通过循环逐步读取到ASP.NET Core中，然后发送给客户端浏览器
                 var fName = Path.GetFileName(fileName);//测试文档.xlsx

                 //这就是ASP.NET Core循环读取下载文件的缓存大小，这里我们设置为了1024字节，也就是说ASP.NET Core每次会从下载文件中读取1024字节的内容到服务器内存中，然后发送到客户端浏览器，这样避免了一次将整个下载文件都加载到服务器内存中，导致服务器崩溃
                 int bufferSize = 1024;


                 string suffix = Path.GetExtension(fileName);

                 if (string.IsNullOrWhiteSpace(suffix))
                     return NotFound();
                 var provider = new FileExtensionContentTypeProvider();
                 var memi = provider.Mappings[suffix]; //映射MIME类型
                 Response.ContentType = memi; //"application/vnd.ms-excel";//由于我们下载的是一个Excel文件，所以设置ContentType为application/vnd.ms-excel

                 var contentDisposition = "attachment;" + "filename=" + System.Web.HttpUtility.UrlEncode(fName);//在Response的Header中设置下载文件的文件名，这样客户端浏览器才能正确显示下载的文件名，注意这里要用HttpUtility.UrlEncode编码文件名，否则有些浏览器可能会显示乱码文件名
                 Response.Headers.Add("Content-Disposition", new string[] { contentDisposition });

                 //使用FileStream开始循环读取要下载文件的内容
                 using (FileStream fs = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read))
                 {
                     using (Response.Body)//调用Response.Body.Dispose()并不会关闭客户端浏览器到ASP.NET Core服务器的连接，之后还可以继续往Response.Body中写入数据
                     {
                         long contentLength = fs.Length;//获取下载文件的大小
                         Response.ContentLength = contentLength;//在Response的Header中设置下载文件的大小，这样客户端浏览器才能正确显示下载的进度

                         byte[] buffer;
                         long hasRead = 0;//变量hasRead用于记录已经发送了多少字节的数据到客户端浏览器

                         //如果hasRead小于contentLength，说明下载文件还没读取完毕，继续循环读取下载文件的内容，并发送到客户端浏览器
                         while (hasRead < contentLength)
                         {
                             //HttpContext.RequestAborted.IsCancellationRequested可用于检测客户端浏览器和ASP.NET Core服务器之间的连接状态，如果HttpContext.RequestAborted.IsCancellationRequested返回true，说明客户端浏览器中断了连接
                             if (HttpContext.RequestAborted.IsCancellationRequested)
                             {
                                 //如果客户端浏览器中断了到ASP.NET Core服务器的连接，这里应该立刻break，取消下载文件的读取和发送，避免服务器耗费资源
                                 break;
                             }

                             buffer = new byte[bufferSize];
                             int currentRead = fs.Read(buffer, 0, bufferSize);//从下载文件中读取bufferSize(1024字节)大小的内容到服务器内存中

                             Response.Body.Write(buffer, 0, currentRead);//发送读取的内容数据到客户端浏览器
                             Response.Body.Flush();//注意每次Write后，要及时调用Flush方法，及时释放服务器内存空间
                             hasRead += currentRead;//更新已经发送到客户端浏览器的字节数
                         }
                     }
                 }
                 return await Task.Run(() => new EmptyResult());
             }


             //      public virtual FileResult GetFile([FromRoute]string Id )
             //      {

             //          var task = Task.Run(async () =>
             //          {
             //              return await _documentManager.GetDocQueryableAsync(new DocumentSearchInput() { Id = Id });
             //          });

             //          var result = task.Result.Result;
             //          if (result == null || result.Length == 0)
             //          {
             //              throw new Exception("未能获取到文件");
             //          }
             //          //   var result = await _documentManager.GetDocQueryableAsync(input);
             //          var catalogName = result[0].catalog.ToChinese();
             //          string fileFullPath = Path.Combine(_hostingEnvironment.DocumentRoot, catalogName, result[0].FileName);
             //          Request.Path = new Microsoft.AspNetCore.Http.PathString($"/api/Document/filedownload2/{(int)result[0].catalog}/{result[0].FileName}");
             //          if (!System.IO.File.Exists(fileFullPath))
             //              throw new Exception($"文件{fileFullPath}不存在");
             //          var fileResult = new PhysicalFileResult(fileFullPath, "application/x-zip-compressed");
             //          fileResult.FileDownloadName = result[0].FileName;
             //          //修改文件下载次数
             //          var bools = Task.Run(async () =>
             //          {
             //              return await _hostingEnvironment.ModifyDocAsync(new DocumentModifyInput() { Id = Id, download = true });

             //          });
             //          if (bools.Result)
             //              return fileResult;
             //          else
             //              throw new Exception("下载文件失败，请联系技术人员");
             //      }
             //      /// <summary>
             ///// 下载文件（用于下载指定文件）
             ///// </summary>
             ///// <param name="fileName"></param>
             ///// <param name="catalog"></param>
             ///// <returns></returns>
             //[HttpGet("filedownload/{catalog}/{fileName}")]
             //      public FileResult GetFile([FromRoute]DocumentCatalog catalog, [FromRoute]string fileName)
             //      {
             //          var catalogName = catalog.ToChinese();
             //          string fileFullPath = string.Empty;
             //          fileFullPath = Path.Combine(_hostingEnvironment.DocumentRoot, catalogName, fileName);
             //          Request.Path = new Microsoft.AspNetCore.Http.PathString($"/api/Document/filedownload2/{(int)catalog}/{fileName}");
             //          if (!System.IO.File.Exists(fileFullPath))
             //              throw new Exception($"文件{fileFullPath}不存在");
             //          var fileResult = new PhysicalFileResult(fileFullPath, "application/x-zip-compressed");
             //          fileResult.FileDownloadName = fileName;
             //          return fileResult;
             //      }


             #endregion
     */

        #endregion

        ///// <summary>
        ///// 其他库
        ///// </summary>
        //[ChineseEnum("上传Excel")]
        // ImportExcel = 7

        /// <summary>
        /// 导入Excel文件
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="import">模块 1员工 2机构 3 供应商 4 物料</param>        
        /// <returns></returns>
        [HttpPost("ImportExcel/{import}/{fileName}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<string>> UpLoadExcelImport([FromRoute] string fileName, [FromRoute] ImportCatalog import)
        {
            string catalogName = "Excel导入";// catalog.ToChinese();
            string d = Path.GetExtension(fileName);//扩展名 “.docx” 
            if (catalogName.IsNullOrEmpty())
                throw new Exception($"文档目录不能为空");
            if (d != ".xlsx" && d != ".xls")
                throw new Exception($"请导入excle文件");
            string dir = Path.Combine(_documentSetting.DocumentRoot, catalogName);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (Request.Form.Files == null || Request.Form.Files.Count <= 0)
            {
                throw new Exception($"未能获取到上传文件信息");
            }
            try
            {
                var file = Request.Form.Files[0];
                //55Path.GetFileNameWithoutExtension
                fileName = Path.GetFileNameWithoutExtension(fileName) + "" + Guid.NewGuid().tostring16() + d;
                string savePath = Path.Combine(dir, fileName);


                if (System.IO.File.Exists(savePath))
                    throw new Exception($"文件{fileName}已经存在");
                var size = file.Length;
                using (FileStream fs = System.IO.File.Create(savePath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                //导入
                return Task.Run(async () =>
                {
                    bool ret = await _documentManager.UpLoadExcelImport(savePath, import);
                    return new ServiceMessage<string>(fileName);
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex + "", ex);
            }
            return Task.FromResult(new ServiceMessage<string>(fileName));
        }

        //
        /// <summary>
        /// 导入Excel文件
        /// </summary>      
        /// <returns></returns>
        [HttpGet("ImportExcels")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<string>> UpLoadExcelImport1()
        {

            //导入
            return Task.Run(async () =>
            {
                bool ret = await _documentManager.TempTableTo("");
                return new ServiceMessage<string>("");
            });

            return Task.FromResult(new ServiceMessage<string>(""));
        }


        /// <summary>
        /// 下载文件
        /// </summary>
        /// <returns></returns>
        [HttpGet("ImportExcel/data")]
        public IActionResult DownLoad(string data)
        {


            var path = @"E:\Work\血液透析中心智能管理系统\code\SwsXytxManagSystem\XytxApi\wwwroot\Files\Patient\111.txt";
            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                stream.CopyToAsync(memoryStream);

            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            //文件名必须编码，否则会有特殊字符(如中文)无法在此下载。
            string encodeFileName = System.Web.HttpUtility.UrlEncode(path, Encoding.GetEncoding("UTF-8"));
            Response.Headers.Add("Content-Disposition", "attachment; filename=" + Path.GetFileName(encodeFileName));
            return new FileStreamResult(memoryStream, "application/octet-stream");//文件流方式，指定文件流对应的ContenType。
        }









        #region  ExportExcel Excel导出
        /// <summary>
        /// 报表Excel导出
        /// </summary>
        /// <returns></returns>
        [HttpPost("ExportExcel/filedownload")]
        [CheckLogin]

        public virtual FileResult ExportExcelFile([FromBody] exportExcleInPut InPut)
        {
            try
            {
                string fileFullPath = _documentSetting.ExportExcel + "收入日报表.xlsx";
                ALLDayIncomeOutPut data = new ALLDayIncomeOutPut();
                var centerDatas = _centerDialysisManger.GetDialysisQueryableByIdsAsync(InPut.CenterId).Result;
                List<DayIncomeOutPut> dayIncomeOutPuts = new List<DayIncomeOutPut>();

                var excelcolumn = new List<ExcelColumnSetting>();

                excelcolumn.Add(new ExcelColumnSetting("no", "序号") { IsSeqColumn = true });


                if (InPut.CenterId.Contains("0"))
                {
                    excelcolumn.Add(new ExcelColumnSetting("itemName", "项目"));
                    data = _businessTargetStatisticalManager.GetBusinessTargetsAsync(new IncomeSummaryQueryInPut() { BeginTime = InPut.BeginTime, CenterId = InPut.CenterId, EndTime = InPut.EndTime, Model = InPut.Model }).Result as ALLDayIncomeOutPut;
                    excelcolumn.Add(new ExcelColumnSetting("kmtxzx", "康美"));
                    excelcolumn.Add(new ExcelColumnSetting("xstxzx", "秀山"));
                    excelcolumn.Add(new ExcelColumnSetting("dzsmzb", "弹子石"));
                    excelcolumn.Add(new ExcelColumnSetting("jlptxzx", "石桥铺"));
                    excelcolumn.Add(new ExcelColumnSetting("total", "合计"));
                    ExcelHelper.Save<CenterDayIncomeOutPut>(data.CenterDayIncomeOutPuts, excelcolumn, fileFullPath, "收入日报表", "重庆山外山血液透析中心收费日报表", $"时间：{InPut.BeginTime.ToString("yyyy年MM月dd日 00:00:00")} 至 {InPut.EndTime.ToString("yyyy年MM月dd日 23:59:59")}");
                }
                else if (!InPut.CenterId.Contains("0") && InPut.CenterId.Count > 1)
                {
                    excelcolumn.Add(new ExcelColumnSetting("itemName", "项目"));
                    data = _businessTargetStatisticalManager.GetBusinessTargetsAsync(new IncomeSummaryQueryInPut() { BeginTime = InPut.BeginTime, CenterId = InPut.CenterId, EndTime = InPut.EndTime, Model = InPut.Model }).Result as ALLDayIncomeOutPut;
                    if (centerDatas.Where(t => t.ShortName.Contains("康美")).ToArray().Length > 0)
                        excelcolumn.Add(new ExcelColumnSetting("kmtxzx", "康美"));
                    if (centerDatas.Where(t => t.ShortName.Contains("秀山")).ToArray().Length > 0)
                        excelcolumn.Add(new ExcelColumnSetting("xstxzx", "秀山"));
                    if (centerDatas.Where(t => t.ShortName.Contains("弹子石")).ToArray().Length > 0)
                        excelcolumn.Add(new ExcelColumnSetting("dzsmzb", "弹子石"));
                    if (centerDatas.Where(t => t.ShortName.Contains("九龙坡")).ToArray().Length > 0)
                        excelcolumn.Add(new ExcelColumnSetting("jlptxzx", "石桥铺"));
                    excelcolumn.Add(new ExcelColumnSetting("total", "合计"));
                    ExcelHelper.Save<CenterDayIncomeOutPut>(data.CenterDayIncomeOutPuts, excelcolumn, fileFullPath, "收入日报表", "重庆山外山血液透析中心收费日报表", $"时间：{InPut.BeginTime.ToString("yyyy年MM月dd日 00:00:00")} 至 {InPut.EndTime.ToString("yyyy年MM月dd日 23:59:59")}");
                }
                else
                {
                    excelcolumn.Add(new ExcelColumnSetting("ItemName", "项目"));
                    dayIncomeOutPuts = _businessTargetStatisticalManager.GetBusinessTargetsAsync(new IncomeSummaryQueryInPut() { BeginTime = InPut.BeginTime, CenterId = InPut.CenterId, EndTime = InPut.EndTime, Model = InPut.Model }).Result as List<DayIncomeOutPut>;
                    excelcolumn.Add(new ExcelColumnSetting("TotalMoney", "合计"));
                    ExcelHelper.Save<DayIncomeOutPut>(dayIncomeOutPuts, excelcolumn, fileFullPath, "收入日报表", $"{centerDatas.First().DialysisName}收费日报表", $"时间：{InPut.BeginTime.ToString("yyyy年MM月dd日 00:00:00")} 至 {InPut.EndTime.ToString("yyyy年MM月dd日 23:59:59")}");

                }


                DayReportOutPut dayReportOutPut = _businessTargetStatisticalManager.GetDayReportAsync(new IncomeSummaryQueryInPut() { BeginTime = InPut.BeginTime, CenterId = InPut.CenterId, EndTime = InPut.EndTime, Model = InPut.Model }).Result as DayReportOutPut;


                ExcelHelper.AddDayDayReport<DayReportOutPut>(dayReportOutPut, fileFullPath);

                if (!System.IO.File.Exists(fileFullPath))
                    throw new Exception($"文件{fileFullPath}不存在");
                var fileResult = new PhysicalFileResult(fileFullPath, "application/x-zip-compressed");

                return fileResult;


            }
            catch (Exception exp)
            {

                throw new Exception("下载文件失败，请联系技术人员");
            }

        }



        /// <summary>
        /// 报表Excel导出
        /// </summary>
        /// <returns></returns>
        [HttpPost("ExportExcel/ykbfiledownload")]
        [CheckLogin]

        public virtual FileResult YKBExportExcelFile([FromBody] PatientDialysisInput InPut)
        {
            try
            {
                string fileFullPath = _documentSetting.ExportExcel + "渝快保赔付明细表.xlsx";

                var centerDatas = _businessTargetStatisticalManager.GetChargeHifesPayAsync(InPut).Result;
                var excelcolumn = new List<ExcelColumnSetting>();
                excelcolumn.Add(new ExcelColumnSetting("no", "序号") { IsSeqColumn = true });
                excelcolumn.Add(new ExcelColumnSetting("ShortName", "机构名称"));
                excelcolumn.Add(new ExcelColumnSetting("mdtrt_id", "就诊流水号"));
                excelcolumn.Add(new ExcelColumnSetting("JSJYLSH", "医保结算流水号"));
                excelcolumn.Add(new ExcelColumnSetting("XM", "姓名"));
                excelcolumn.Add(new ExcelColumnSetting("CardNum", "证件号码"));
                excelcolumn.Add(new ExcelColumnSetting("StrCYRQ", "出院日期"));
                excelcolumn.Add(new ExcelColumnSetting("StrJSRQ", "结算时间"));
                excelcolumn.Add(new ExcelColumnSetting("RYZDMC", "确诊疾病"));
                excelcolumn.Add(new ExcelColumnSetting("ZJE", "医疗费用总额"));
                excelcolumn.Add(new ExcelColumnSetting("hifes_pay", "渝快保赔付金额"));
                ExcelHelper.Save<ChargeHifesPayOutPut>(centerDatas, excelcolumn, fileFullPath, "渝快保赔付明细表", "重庆山外山血液透析中心渝快保赔付明细表", $"时间：{InPut.BeginTime.ToString("yyyy年MM月dd日 00:00:00")} 至 {InPut.EndTime.ToString("yyyy年MM月dd日 23:59:59")}");


                if (!System.IO.File.Exists(fileFullPath))
                    throw new Exception($"文件{fileFullPath}不存在");
                var fileResult = new PhysicalFileResult(fileFullPath, "application/x-zip-compressed");

                return fileResult;


            }
            catch (Exception exp)
            {

                throw new Exception("下载文件失败，请联系技术人员");
            }

        }





        #endregion



    }

    public class FileC
    {
        public string Id { get; set; }
    }

}
