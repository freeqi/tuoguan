using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Utils
{
    public class ImageHelper
    {
        //图片 转为    base64编码的文本
        public static string ImgToBase64String(string Imagefilename)
        {
            System.IO.MemoryStream ms = null;
            string base64 = null;
            Image img = null;
            try
            {
                if (!File.Exists(Imagefilename))
                    return null;
                img = Image.FromFile(Imagefilename);
                BinaryFormatter binFormatter = new BinaryFormatter();
                ms = new MemoryStream();
                binFormatter.Serialize(ms, img);
                byte[] bytes = ms.GetBuffer();
                base64 = Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (img != null)
                    img.Dispose();
                if (ms != null)
                    ms.Close();
            }
            return base64;
        }
        public static string ImgToBase64String1(string Imagefilename)
        {
            try
            {
                if (!File.Exists(Imagefilename))
                    return null;
                Bitmap bmp = new Bitmap(Imagefilename);

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 将BASE64编码转换保存为图片
        /// </summary>
        /// <param name="base64Str">base64编码</param>
        /// <param name="filename">图片文件名</param>
        public static void Base64StringToImage(string base64Str, string filename)
        {
            try
            {
                //将base64编码转换为图片
                byte[] bytes = Convert.FromBase64String(base64Str);
                MemoryStream msSource = new MemoryStream(bytes);
                BinaryFormatter binFormatter = new BinaryFormatter();
                System.Drawing.Image imgSource = (System.Drawing.Image)binFormatter.Deserialize(msSource);
                imgSource.Save(filename);
            }
            catch (Exception exp)
            {

                throw;
            }

        }

        public static string Base64StringToImage1(string base64Str, string filename)

        {

            var base64 = "";
            base64 = base64Str.Replace("data:image/png;base64,", "").Replace("data:image/jgp;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "");//将base64头部信息替换
            try
            {
                if (base64 != "")
                {
                    byte[] bytes = Convert.FromBase64String(base64);
                    MemoryStream memStream = new MemoryStream(bytes);
                    Image mImage = Image.FromStream(memStream);
                    Bitmap bp = new Bitmap(mImage);
                    MemoryStream ms = new MemoryStream();
                    var dir = new FileInfo(filename).Directory;
                    if (dir == null)
                    {
                        return "";
                    }
                    if (!dir.Exists)
                    {
                        dir.Create();
                    }

                    bp.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);//注意保存路径
                    return filename;
                }
                else
                    return "";
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
    }
}
