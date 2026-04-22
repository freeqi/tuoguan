namespace CDGService.Store.Extensions
{
    /// <summary>
    /// xml文件中sql语句格式化参数
    /// </summary>
    public sealed class XmlSqlParameter
    {
        /// <summary>
        /// 业务模块类（class）名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        ///  业务模块类备注说明（业务模块类描述）
        /// </summary>
        //public string ClassNameRemark { get; set; }

        /// <summary>
        /// 执行sql语句的方法名称
        /// </summary>
        public string SqlFunName { get; set; }
        /// <summary>
        /// 执行sql语句的方法备注说明（业务功能描述）
        /// </summary>
        public string SqlFunRemark { get; set; }
    }
}
