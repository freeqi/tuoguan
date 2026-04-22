using System;
using System.Drawing.Imaging;
using CDGService.ThirdPartyLib.Binaries;
using CDGService.ThirdPartyLib.Bitmaps.Models;

namespace CDGService.ThirdPartyLib.Bitmaps
{
    /// <summary>
    /// Bitmap图片帮助类（8位颜色值，即256种颜色）
    /// </summary>
    public static class BitmapHelper
    {
        #region 公共方法

        /// <summary>
        /// 获取位图结构
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>位图结构</returns>
        public static BmpModel GetBmpModel(string filename)
        {
            var result = new BmpModel();
            var btData = BinaryHelper.GetOriginByteDatas(filename);
            result.FileHeader = GetFileHeader(btData);
            result.BmpDesc = GetBmpDesc(btData);
            //颜色表
            var colorBlockLength = result.FileHeader.DataBlockIndex - 14 - 40;
            var btColor = new byte[colorBlockLength];
            Array.ConstrainedCopy(btData, 54, btColor, 0, colorBlockLength);
            //数据区
            var dataBlockLength = result.FileHeader.FileLength - result.FileHeader.DataBlockIndex;
            var btDb = new byte[dataBlockLength];
            Array.ConstrainedCopy(btData, result.FileHeader.DataBlockIndex, btDb, 0, dataBlockLength);
            return result;
        }

        /// <summary>
        /// 获取图像二进制流
        /// </summary>
        /// <param name="btImgDataBlock">图像数据区</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>图像二进制流</returns>
        public static byte[] GetBitmapBytesFromDataBytes(byte[] btImgDataBlock, int width, int height)
        {
            var totalLength = GetByteTotalLength(width, height);
            var btData = new byte[totalLength];
            SetFileHeaderBytes(ref btData, width, height);
            SetImgDescBytes(ref btData, width, height);
            SetColorTable(ref btData);
            SetImageDataBlock(ref btData, btImgDataBlock, width, height);
            return btData;
        }

        /// <summary>
        /// 将二进制数据保存为Bitmap图片
        /// </summary>
        /// <param name="byteArrayIn">二进制流</param>
        /// <param name="filename">文件路径</param>
        /// <param name="format">图片格式</param>
        public static void SaveByteArrayToImage(byte[] byteArrayIn, string filename, ImageFormat format)
        {
            if (byteArrayIn == null)
                return;
            System.Drawing.Image image = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArrayIn))
            {
                image = System.Drawing.Image.FromStream(ms);
                image.Save(filename, format);
            }
        }

        /// <summary>  
        /// byte[]转换成Image  
        /// </summary>  
        /// <param name="byteArrayIn">二进制图片流</param>  
        /// <returns>Image</returns>  
        public static System.Drawing.Image ByteArrayToImage(byte[] byteArrayIn, int width, int height)
        {
            if (byteArrayIn == null)
                return null;
            System.Drawing.Image returnImage = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArrayIn))
            {
                returnImage = System.Drawing.Image.FromStream(ms);
                return returnImage;
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取文件头
        /// </summary>
        /// <param name="btData">二进制数据源</param>
        /// <returns>文件头</returns>
        private static BmpFileHeader GetFileHeader(byte[] btData)
        {
            var result = new BmpFileHeader();
            result.FileFlag = btData.ReadString(0, 2);
            result.FileLength = btData.ReadInt32(2);
            result.DataBlockIndex = btData.ReadInt32(0x0a);
            return result;
        }

        /// <summary>
        /// 获取图像描述信息
        /// </summary>
        /// <param name="btData">二进制数据源</param>
        /// <returns>图像描述信息</returns>
        private static BmpDesc GetBmpDesc(byte[] btData)
        {
            var result = new BmpDesc();
            result.BmpDescBlockLength = btData.ReadInt16(0x0e);
            result.ImgWidth = btData.ReadInt32(0x12);
            result.ImgHeight = btData.ReadInt32(0x16);
            result.PlaneSum = btData.ReadInt16(0x1a);
            result.PixelRecordBitLength = btData.ReadInt16(0x1c);
            result.CompressMode = btData.ReadInt16(0x1e);
            result.ImgBlockDataLength = btData.ReadInt32(0x22);
            result.HorizontalPixelPerMeter = btData.ReadInt32(0x26);
            result.VerticalPixelPerMeter = btData.ReadInt32(0x2a);
            result.ColorCount = btData.ReadInt16(0x2e);
            result.ImportantColorCount = btData.ReadInt32(0x32);
            return result;
        }

        /// <summary>
        /// 获取总长度
        /// </summary>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <returns>总长度</returns>
        private static int GetByteTotalLength(int width, int height)
        {
            int trueWidth = width + (width%4 == 0 ? 0 : 4 - width%4);
            var totalLength = 14 + 40 + 1024 + trueWidth*height;
            return totalLength;
        }

        /// <summary>
        /// 设置文件头二进制流
        /// </summary>
        /// <returns></returns>
        private static void SetFileHeaderBytes(ref byte[] btData, int width, int height)
        {
            //文件标识：BM
            btData.WriteString(0, "BM");
            //文件大小
            int totalLength = GetByteTotalLength(width, height);
            btData.WriteInt32(2, totalLength);
            //图像数据区起始位置
            btData.WriteInt32(0x0a, 14 + 40 + 1024);
        }

        /// <summary>
        /// 设置图像描述
        /// </summary>
        /// <param name="btData"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private static void SetImgDescBytes(ref byte[] btData, int width, int height)
        {
            //图像描述信息块的大小
            btData.WriteInt16(0x0e, 40);
            //图像宽度
            btData.WriteInt32(0x12, width);
            //图像高度
            btData.WriteInt32(0x16, height);
            //图像的plane总数（恒为1）
            btData.WriteInt16(0x1a, 1);
            //记录像素的位数，此时为8
            btData.WriteInt16(0x1c, 8);
            //压缩方式（不压缩）
            btData.WriteInt16(0x1e, 0);
        }

        /// <summary>
        /// 设置颜色表
        /// </summary>
        /// <param name="btData">二进制数据源</param>
        private static void SetColorTable(ref byte[] btData)
        {
            int startIndex = 54;
            for (int colorIndex = 0;colorIndex<=255; colorIndex++)
            {
                if (colorIndex > 255)
                {
                    break;
                }
                var btColorIndex = (byte) colorIndex;
                var tpColor = ConvertToColor(btColorIndex);
                var tmpColor = new byte[] {btColorIndex, btColorIndex, btColorIndex, 0};
                Array.ConstrainedCopy(tmpColor, 0, btData, startIndex + 4*colorIndex, 4);
            }
        }

        /// <summary>
        /// 将灰度转换为伪彩色
        /// </summary>
        /// <param name="gray">灰度</param>
        /// <returns></returns>
        private static Tuple<byte, byte, byte> ConvertToColor(byte gray)
        {
            var result = new Tuple<byte, byte, byte>(0, 0, 0);
            int temp = 0;
            if (gray <= 51)
            {
                result = new Tuple<byte, byte, byte>(0, (byte)(gray*5), 255);
            }
            else if (gray <= 102)
            {
                gray -= 51;
                result = new Tuple<byte, byte, byte>(0, 255, (byte)(255-gray*5));
            }
            else if (gray <= 153)
            {
                gray -= 102;
                result = new Tuple<byte, byte, byte>((byte) (gray*5), 255, 0);
            }
            else if (gray <= 204)
            {
                gray -= 153;
                temp = (int)(255 - gray*128/51 - 0.5);
                if (temp > 255)
                {
                    temp = 255;
                }
                result = new Tuple<byte, byte, byte>(255, (byte)temp, 0);
            }
            else
            {
                gray -= 204;
                temp = (int) (127 - 127.0*gray/51.0 + 0.5);
                if (temp > 255)
                {
                    temp = 255;
                }
                result = new Tuple<byte, byte, byte>(255, (byte) temp, 0);
            }
            return result;
        }

        /// <summary>
        /// 设置图片数据区
        /// </summary>
        /// <param name="btData">二进制数据</param>
        /// <param name="btImageBlock">图像存储数据</param>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片高度</param>
        private static void SetImageDataBlock(ref byte[] btData, byte[] btImageBlock, int width, int height)
        {
            int startIndex = 14 + 40 + 1024;
            int trueWidth = width + (width % 4 == 0 ? 0 : 4 - width % 4);
            for (int row = 1; row <= height; row++)
            {
                for (int col = 1; col <= width; col++)
                {
                    var indexData = startIndex + (row - 1)*trueWidth + (col - 1);
                    var indexImage = (row - 1)*width + (col - 1);
                    btData[indexData] = btImageBlock[indexImage];
                }
            }
        }

        #endregion
    }
}
