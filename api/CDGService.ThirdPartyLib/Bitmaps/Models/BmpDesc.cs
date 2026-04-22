namespace CDGService.ThirdPartyLib.Bitmaps.Models
{
    /// <summary>
    /// 图像描述
    /// </summary>
    public class BmpDesc
    {
        /// <summary>
        /// 图像描述块大小
        /// </summary>
        public int BmpDescBlockLength { get; set; }

        /// <summary>
        /// 图像宽度
        /// </summary>
        public int ImgWidth { get; set; }

        /// <summary>
        /// 图像高度
        /// </summary>
        public int ImgHeight { get; set; }

        /// <summary>
        /// 图像的Plane总数（恒为1）
        /// </summary>
        public int PlaneSum { get; set; }

        /// <summary>
        /// 记录像素的位数
        /// </summary>
        public int PixelRecordBitLength { get; set; }

        /// <summary>
        /// 数据压缩方式（数值位0：不压缩；1：8位压缩；2：4位压缩）
        /// </summary>
        public int CompressMode { get; set; }

        /// <summary>
        /// 图像区数据块大小
        /// </summary>
        public int ImgBlockDataLength { get; set; }

        /// <summary>
        /// 水平每米有多少像素
        /// </summary>
        public int HorizontalPixelPerMeter { get; set; }

        /// <summary>
        /// 垂直每米有多少像素
        /// </summary>
        public int VerticalPixelPerMeter { get; set; }

        /// <summary>
        /// 此图像所用的颜色数，如值为0，表示所有颜色一样重要
        /// </summary>
        public int ColorCount { get; set; }

        /// <summary>
        /// 重要的颜色数
        /// </summary>
        public int ImportantColorCount { get; set; }

    }
}
