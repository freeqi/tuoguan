using CDGService.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Application.Service.Net
{
    public static class WebApiHelper
    {
        /// <summary>
        /// Post请求数据，并从Json字符串返回结果对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        public static async Task<ServiceMessage<T>> SendPostAsync<T>(this HttpClient client, string url, dynamic pas = null)
        {
            if (pas == null)
                pas = new Dictionary<string, string>();

            var stringcontent = new StringContent(JsonConvert.SerializeObject(pas), Encoding.UTF8, "application/json");
            /*
             
             */
            return await SendPostContentAsync<T>(client, url, stringcontent);
        }

        public static async Task<ServiceMessage<T>> SendPostContentAsync<T>(this HttpClient client, string url, StringContent stringcontent)
        {
            var servicemessage = await client.PostAsync(url, stringcontent);

            servicemessage.EnsureSuccessStatusCode();
            var content = await servicemessage.Content.ReadAsStringAsync();
            // Debug.Write(content);

            var result = JsonConvert.DeserializeObject<ServiceMessage<T>>(content);
            return result;
        }

        /// <summary>
        /// 采用Get请求数据，并从Json字符串返回结果对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Client"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<ServiceMessage<T>> SendGetAsync<T>(this HttpClient Client, string url)
        {
            string contents = "";
            try
            {
                
                var servicemessage = await Client.GetAsync(url);
                servicemessage.EnsureSuccessStatusCode();
                contents = await servicemessage.Content.ReadAsStringAsync();

                if(contents.First() != '{') { 
                contents = System.Text.RegularExpressions.Regex.Unescape(contents);

                //if (contents.Contains("\\\\n"))
                //    contents = contents.Replace("\\\\n", "");
                //if (contents.Contains("\\\\r"))
                //    contents = contents.Replace("\\\\r", "");
                //contents = contents.Replace("\\", "&_&");

                contents = contents.Remove(0, 1);
                contents = contents.Remove(contents.Length - 1, 1);
                contents = contents.Replace("'", "’");
                contents = contents.Replace("\"", "'");
                    //contents = contents.Replace("&_&", "'");
                    // StringEscapeUtils.unescapeJavaScript(str1);
                    //var result33 = JsonConvert.DeserializeObject<ServiceMessage>(content);
                    //var result1 = JsonConvert.DeserializeObject<object>(content);
                    //var result2 = JsonConvert.DeserializeObject<ServiceMessage<object>>(content);
                }
                var result = JsonConvert.DeserializeObject<ServiceMessage<T>>(contents);
                return result;
            }
            catch (Exception exp)
            {
                CDGService.Utils.FileHelper.WriteLog("", $"采集数据异常:{contents}:{url}；{exp.Message}");
                return new ServiceMessage<T>() { Code = 500 };

            }
        }


        //public static async void Tstt(test d, HttpClient client)
        //{
        //    try
        //    {


        //        client = new HttpClient();
        //        var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };

        //        client = new HttpClient(handler);
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        string url = "http://192.168.30.244:816/SwsMesWeb.ashx";



        //        var test = new test()
        //        {
        //            methodName = "SWSManage.Func_getfour_zj",
        //            only_cx = "yes",
        //            parameters = "1|#|2020-11-26",
        //            u_login = "13312341234",
        //            u_pass = "123"
        //        };
        //        string ste = test.parameters.Replace("|#|", "%7C%23%7C");
        //        url += $"?methodName={test.methodName}&parameters={test.parameters}&only_cx={test.only_cx}&u_login={test.u_login}&u_pass={test.u_pass}";
        //        var servicemessage = client.GetAsync(url).Result;
        //        servicemessage.EnsureSuccessStatusCode();
        //        var contents = await servicemessage.Content.ReadAsStringAsync();
        //        contents = System.Text.RegularExpressions.Regex.Unescape(contents);
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}






        /// <summary>
        /// PUT请求数据，并从Json字符串返回结果对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        //public static async Task<ServiceMessage<T>> SendPutAsync<T>(this HttpClient client, string url, string Psa)
        //{
        //    //if (pas == null)
        //    //    pas = new Dictionary<string, string>();

        //    var stringcontent = new  StringContent(Psa, Encoding.UTF8, "application/json");
        //   // return await SendPutContentAsync<T>(client, url, stringcontent);
        //}

        public static async Task<ServiceMessage<T>> SendPutContentAsync<T>(this HttpClient client, string url, string json)
        {

            HttpContent content;
            string contents = "";
            try
            {

                using (var formData = new MultipartFormDataContent())
                {

                    content = new StringContent(json);
                    formData.Add(content);
                    var servicemessage = await client.PutAsync(url, formData);
                    servicemessage.EnsureSuccessStatusCode();
                    contents = await servicemessage.Content.ReadAsStringAsync();
                    // Debug.Write(content);
                    if (contents.Contains("\\\\n"))
                        contents = contents.Replace("\\\\n", "");
                    if (contents.Contains("\\\\r"))
                        contents = contents.Replace("\\\\r", "");
                    contents = contents.Replace("\\", "&_&");
                    contents = contents.Replace("\"", "");
                    contents = contents.Replace("&_&", "'");
                    var result = JsonConvert.DeserializeObject<ServiceMessage<T>>(contents);
                    return result;

                }
            }
            catch (Exception exp)
            {
                CDGService.Utils.FileHelper.WriteLog("", $"put异常:{contents}:{url};{exp.Message}{json}");
                return new ServiceMessage<T>() { Code = 1024 };
            }
        }


        public static async Task<ServiceMessage<T>> SendPOSTContentAsync<T>(this HttpClient client, string url, string json)
        {
            try
            {

                HttpContent content;
                using (var formData = new MultipartFormDataContent())
                {

                    content = new StringContent(json);
                    formData.Add(content);
                    var servicemessage = await client.PostAsync(url, formData);
                    servicemessage.EnsureSuccessStatusCode();
                    var contents = await servicemessage.Content.ReadAsStringAsync();
                    // Debug.Write(content);
                    if (contents.Contains("\\\\n"))
                        contents = contents.Replace("\\\\n", "");                 
                    if (contents.Contains("\\\\r"))
                        contents = contents.Replace("\\\\r", "");
                    contents = contents.Replace("\\", "&_&");
                    contents = contents.Replace("\"", "");
                    contents = contents.Replace("&_&", "'");
                    var result = JsonConvert.DeserializeObject<ServiceMessage<T>>(contents);
                    return result;

                }
            }
            catch (Exception exp)
            {
                return new ServiceMessage<T>() { Code = 500 };
            }
        }

        public static async Task<string > HISSendPOSTContentAsync(this HttpClient client, string url, string json)
        {
            try
            {

                HttpContent content;
                using (var formData = new MultipartFormDataContent())
                {

                    content = new StringContent(json);
                    formData.Add(content);
                    var servicemessage = await client.PostAsync(url, formData);
                    servicemessage.EnsureSuccessStatusCode();
                    var contents = await servicemessage.Content.ReadAsStringAsync();
                   
                    
                    return contents;

                }
            }
            catch (Exception exp)
            {
                return "";
            }
        }

        public static async Task<ServiceMessage<T>> SendDelContentAsync<T>(this HttpClient client, string url, string json)
        {
            try
            {

                HttpContent content;

                var servicemessage = await client.DeleteAsync($"{url}?data={json}");
                servicemessage.EnsureSuccessStatusCode();
                var contents = await servicemessage.Content.ReadAsStringAsync();
                // Debug.Write(content);
                if (contents.Contains("\\\\n"))
                    contents = contents.Replace("\\\\n", "");
                if (contents.Contains("\\\\r"))
                    contents = contents.Replace("\\\\r", "");
                contents = contents.Replace("\\", "&_&");
                contents = contents.Replace("\"", "");
                contents = contents.Replace("&_&", "'");
                var result = JsonConvert.DeserializeObject<ServiceMessage<T>>(contents);
                return result;


            }
            catch (Exception exp)
            {

                return new ServiceMessage<T>() { Code = 500 };
            }

        }

        public static void tt(string url, string postString)
        {
            //    string postString = "arg1=a&arg2=b";//这里即为传递的参数，可以用工具抓包分析，也可以自己分析，主要是form里面每一个name都要加进来  
            byte[] postData = Encoding.UTF8.GetBytes(postString);//编码，尤其是汉字，事先要看下抓取网页的编码方式  

            WebClient webClient = new WebClient();
            //  webClient.Headers.Add("Content-Type", "application/json");//采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可  

            byte[] responseData = webClient.UploadData(url, "PUT", postData);//得到返回字符流  
            string srcString = Encoding.UTF8.GetString(responseData);//解码  
        }


        /// <summary>
        ///   替换部分字符串
        /// </summary>
        /// <param name="sPassed">需要替换的字符串</param>
        /// <returns></returns>
        public static string ReplaceString(string JsonString)
        {
            if (JsonString == null) { return JsonString; }
            if (JsonString.Contains("\\"))
            {
                JsonString = JsonString.Replace("\\", "\\\\");
            }
            if (JsonString.Contains("\'"))
            {
                JsonString = JsonString.Replace("\'", "\\\'");
            }
            if (JsonString.Contains("\""))
            {
                JsonString = JsonString.Replace("\"", "\\\"");
            }
            //去掉字符串的回车换行符
            JsonString = System.Text.RegularExpressions.Regex.Replace(JsonString, @"[\n\r]", "");
            JsonString = JsonString.Trim();
            return JsonString;
        }

    }


    public class test
    {


        public string methodName { get; set; }
        public string parameters { get; set; }
        public string only_cx { get; set; }
        public string u_login { get; set; }
        public string u_pass { get; set; }

    }
}
