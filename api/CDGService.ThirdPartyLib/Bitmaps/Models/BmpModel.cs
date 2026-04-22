namespace CDGService.ThirdPartyLib.Bitmaps.Models
{
    /// <summary>
    /// Bmp图像对象
    /// </summary>
    public class BmpModel
    {
        /// <summary>
        /// 文件头
        /// </summary>
        public BmpFileHeader FileHeader { get; set; }

        /// <summary>
        /// 图像描述
        /// </summary>
        public BmpDesc BmpDesc { get; set; }
    }
}
