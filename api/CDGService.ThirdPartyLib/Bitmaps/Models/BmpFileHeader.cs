namespace CDGService.ThirdPartyLib.Bitmaps.Models
{
    /// <summary>
    /// Bmp文件头
    /// </summary>
    public class BmpFileHeader
    {
        /// <summary>
        /// 文件标识
        /// </summary>
        public string FileFlag { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public int FileLength { get; set; }

        /// <summary>
        /// 数据区起始位置
        /// </summary>
        public int DataBlockIndex { get; set; }
    }
}
