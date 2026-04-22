﻿using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;
using NPOI.XSSF.UserModel;

namespace CDGService.Utils
{
    /// <summary>
    /// excle帮助类
    /// </summary>
    public static class ExcelHelper
    {
        #region 公共方法

        /// <summary>
        /// 保存数据列表到Excel
        /// </summary>
        /// <param name="data">数据列表</param>
        /// <param name="columnSettings">列定义</param>
        /// <param name="filePath">Excel文件</param>
        /// <param name="sheetName">sheet名称</param>
        public static void Save<T>(IEnumerable<T> data, List<ExcelColumnSetting> columnSettings, string filePath,
            string sheetName, string Title, string remorks) where T : class
        {
            if (data == null || string.IsNullOrEmpty(filePath) || columnSettings == null || !columnSettings.Any())
            {
                return;
            }
            if (filePath.EndsWith(".xls")) //2003格式
            {
                WriteExcel2003(data, columnSettings, filePath, sheetName);
            }
            else if (filePath.EndsWith(".xlsx")) //2007及以上版本
            {
                SaveToExcel2007OrAbove<T>(data, columnSettings, filePath, sheetName, Title, remorks);
            }
        }


        /// <summary>
        /// 动态导出模板
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="fileName"></param>
        /// <param name="filePath"></param>
        /// <param name="sheetName"></param>
        /// <param name="attenceTtile">必填字段，背景加红</param>
        /// <returns></returns>
        public static bool ExportExcel(OrderedDictionary dict, string fileName, string filePath, string attenceTtile, string sheetName = "sheet1")
        {
            FileStream fs = null;
            HSSFWorkbook workbook = null;
            try
            {
                workbook = new HSSFWorkbook();//从流内容创建Workbook对象
                ISheet sheet = ((HSSFWorkbook)workbook).CreateSheet(sheetName);//创建工作表
                HSSFSheet SheetOne = (HSSFSheet)workbook.GetSheet(sheetName);
                SheetOne.CreateRow(0);


                HSSFRow SheetRow = (HSSFRow)SheetOne.GetRow(0);
                HSSFCell[] SheetCell = new HSSFCell[dict.Keys.Count];
                int tmp = 0;
                ICellStyle s = workbook.CreateCellStyle();
                s.FillForegroundColor = HSSFColor.Red.Index;
                s.FillPattern = FillPattern.SolidForeground;
                List<string> attenceTtileList = new List<string>();
                if (!string.IsNullOrEmpty(attenceTtile))
                {
                    attenceTtileList = attenceTtile.Split(',').ToList();
                }
                foreach (object key in dict.Keys)
                {
                    string strkey = key.ToString();
                    List<string> list = (List<string>)dict[key];
                    SheetCell[tmp] = (HSSFCell)SheetRow.CreateCell(tmp);
                    SheetCell[tmp].SetCellValue(strkey);
                    if (attenceTtileList.Contains(strkey))
                    {
                        SheetCell[tmp].CellStyle = s;
                    }
                    if (list != null && list.Count > 0)
                    {
                        var cellRegions = new CellRangeAddressList(0, 65535, tmp, tmp);
                        //设置 下拉框内容
                        DVConstraint constraint = DVConstraint.CreateExplicitListConstraint(list.ToArray());
                        //绑定下拉框和作用区域，并设置错误提示信息
                        HSSFDataValidation dataValidate = new HSSFDataValidation(cellRegions, constraint);
                        dataValidate.CreateErrorBox("输入不合法", "请输入下拉列表中的值。");
                        dataValidate.ShowPromptBox = true;
                        sheet.AddValidationData(dataValidate);
                    }
                    tmp++;
                }
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
                string filefullPath = Path.Combine(filePath, fileName);
                fs = new FileStream(filefullPath, FileMode.Create);
                workbook.Write(fs);
                return true;
            }
            catch (Exception eex)
            {
                return false;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 写入Excel
        /// </summary>
        /// <param name="dataList">数据源</param>
        /// <param name="columnSettings">列设置</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="sheetName">SHEET名称</param>
        private static void WriteExcel2003<T>(IEnumerable<T> dataList, List<ExcelColumnSetting> columnSettings, string filePath, string sheetName) where T : class
        {
            if (!string.IsNullOrEmpty(filePath) && null != dataList && columnSettings != null && columnSettings.Any())
            {
                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                NPOI.SS.UserModel.ISheet sheet = book.CreateSheet(sheetName);

                NPOI.SS.UserModel.IRow row = sheet.CreateRow(0);
                var colIndex = 0;
                var ltProperty = new List<Tuple<PropertyInfo, ExcelColumnSetting>>();
                foreach (var item in columnSettings)
                {
                    PropertyInfo property = null;
                    if (item.IsSeqColumn)
                    {
                        item.Header = item.Header ?? "序号";
                    }
                    else
                    {
                        property = typeof(T).GetProperty(item.PropertyName);
                        if (property == null)
                        {
                            continue;
                        }
                    }
                    row.CreateCell(colIndex).SetCellValue(item.Header);
                    colIndex++;
                    ltProperty.Add(new Tuple<PropertyInfo, ExcelColumnSetting>(property, item));
                }
                int columnCount = ltProperty.Count;
                if (columnCount > 0)
                {
                    var arData = dataList.ToArray();
                    int rowCount = dataList.Count();
                    for (var i = 0; i < rowCount; i++)
                    {
                        NPOI.SS.UserModel.IRow row2 = sheet.CreateRow(i + 1);
                        var item = arData[i];
                        for (var j = 0; j < columnCount; j++)
                        {
                            var columnSetting = ltProperty[j].Item2;
                            var content = columnSetting.IsSeqColumn ? (i + 1).ToString() : ltProperty[j].Item1.ToExcelValue(item, columnSetting);
                            var cell = row2.CreateCell(j);
                            cell.SetCellValue(content);
                            SetCellFromat2003(cell, columnSetting);
                        }
                    }
                }
                // 写入到客户端  
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    book.Write(ms);
                    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        byte[] data = ms.ToArray();
                        fs.Write(data, 0, data.Length);
                        fs.Flush();
                    }
                    book = null;
                }
            }
        }

        /// <summary>
        /// 保存数据列表到Excel（2007及以上版本）
        /// </summary>
        /// <param name="data">数据列表</param>
        /// <param name="columnSettings">列设置条件</param>
        /// <param name="fileName">Excel文件</param>
        /// <param name="sheetName">sheet名称</param>
        private static void SaveToExcel2007OrAbove<T>(IEnumerable<T> data, List<ExcelColumnSetting> columnSettings, string fileName, string sheetName, string Title, string remorks) where T : class
        {
            if (data == null || !columnSettings.Any())
            {
                return;
            }
            if (File.Exists(fileName))
                File.Delete(fileName);
            FileInfo file = new FileInfo(fileName);
            try
            {


                using (ExcelPackage ep = new ExcelPackage(file))
                {
                    int rowIndex = 1;
                    var ltProperty = new List<Tuple<PropertyInfo, ExcelColumnSetting>>();
                    int columnCount = columnSettings.Count;
                    ExcelWorksheet ws = ep.Workbook.Worksheets.Add(sheetName);
                    //标题 //时间
                    if (Title != "")
                    {
                        int[,] mergeRowIndexs = { { 1, 1, columnCount }, { 1, 1, columnCount } };
                        MergeRowCells(ws, 0, mergeRowIndexs);
                        ws.Cells[1, 1].Value = Title;
                        ws.Cells[1, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        SetFontSty(ws.Cells[1, 1], "微软雅黑", 20.5f, true);
                        rowIndex++;
                    }
                    if (remorks != "")
                    {
                        int[,] mergeRowIndexs = { { 1, 1, columnCount }, { 1, 1, columnCount } };
                        MergeRowCells(ws, 1, mergeRowIndexs);
                        ws.Cells[2, 1].Value = remorks;
                        ws.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        rowIndex++;
                    }
                    //


                    //列头
                    var colIndex = 0;

                    foreach (var item in columnSettings)
                    {
                        PropertyInfo property = null;
                        if (item.IsSeqColumn)
                        {
                            item.Header = item.Header ?? "序号";
                        }
                        else
                        {
                            property = typeof(T).GetProperty(item.PropertyName);
                            if (property == null)
                            {
                                continue;
                            }
                        }
                        colIndex++;
                        ws.Cells[rowIndex, colIndex].Value = item.Header;
                        ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        ltProperty.Add(new Tuple<PropertyInfo, ExcelColumnSetting>(property, item));
                    }

                    //内容
                    if (columnCount > 0)
                    {
                        var arData = data.ToArray();
                        int rowCount = data.Count();
                        for (var i = 0; i < rowCount; i++)
                        {
                            var item = arData[i];
                            for (var j = 0; j < columnCount; j++)
                            {
                                var columnSetting = ltProperty[j].Item2;
                                var content = columnSetting.IsSeqColumn
                                    ? (i + 1).ToString()
                                    : ltProperty[j].Item1.ToExcelValue(item, columnSetting);
                                var range = ws.Cells[rowIndex + 1 + i, j + 1];
                                range.Value = content;
                                SetCellFormat2007OrAbove(range, columnSetting);
                            }
                        }
                    }
                    ep.Save();
                }
            }
            catch (InvalidOperationException ex)
            {
                // Console.WriteLine(ex.Message);
            }
        }


        public static void AddDayDayReport<T>(T data, string fileName)
        {
            if (data == null)
            {
                return;
            }
            //dayIncomeOutPut.GetType().GetProperties()
            FileInfo file = new FileInfo(fileName);

            using (ExcelPackage ep = new ExcelPackage(file))
            {

                ExcelWorksheet ws = ep.Workbook.Worksheets[1];

                //var range = ws.Cells[20, 1];
                //range.Value = "收费情况";
                //SetCellFormat2007OrAbove(range, new ExcelColumnSetting() { IsAutoFitWidth = true });

                //var range1 = ws.Cells[20, 2];
                //var num = Convert.ToDecimal(data.GetType().GetProperties().Where(t => t.Name == "total").First().GetValue(data));
                //range1.Value = $"合计:￥{num}/大写：{num.NumtoChinese()}";
                //SetCellFormat2007OrAbove(range1, new ExcelColumnSetting() { IsAutoFitWidth = true });
                /*
                个人帐户 民政补助 大额统筹 现金 医院超标 医保基金 票据收退 号码范围 实际票号
             */

                int[,] mergeRowIndexs = { { 1, 1, 2 }, { 1, 1, 2 } };
                MergeRowCells(ws, 18, mergeRowIndexs);
                ws.Cells[19, 1].Value = ws.Cells[2, 1].Value;
                ws.Cells[19, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                var pro = data.GetType().GetProperties();
                int rowindex = 20;
                //ws.Cells[rowindex, 1].Value = "时间：";
                //SetCellFormat2007OrAbove(ws.Cells[20, 1], new ExcelColumnSetting() { IsAutoFitWidth = true });
                //ws.Cells[rowindex, 2].Value = $"合计:￥{prices}/大写：{prices.NumtoChinese()}";
                //SetCellFormat2007OrAbove(ws.Cells[rowindex, 2], new ExcelColumnSetting() { IsAutoFitWidth = true });
                foreach (var item in pro)
                {
                    var prices = Convert.ToDecimal(data.GetType().GetProperties().Where(t => t.Name == item.Name).First().GetValue(data)).ToFloorRound();
                    switch (item.Name)
                    {
                        case "total":
                            ws.Cells[rowindex, 1].Value = "收费情况";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 1], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 2].Value = $"合计:￥{prices}/大写：{prices.NumtoChinese()}";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 2], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            break;
                        case "xj":
                            ws.Cells[rowindex, 1].Value = "现金";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 1], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 2].Value = $"收:￥{prices}/大写：{prices.NumtoChinese()}";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 2], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            break;
                        case "Individual":
                            var zgpircesum = Convert.ToDecimal(data.GetType().GetProperties().Where(t => t.Name == "Individual").First().GetValue(data)).ToFloorRound() + Convert.ToDecimal(data.GetType().GetProperties().Where(t => t.Name == "mzbz").First().GetValue(data)).ToFloorRound() +
                                  Convert.ToDecimal(data.GetType().GetProperties().Where(t => t.Name == "detc").First().GetValue(data)).ToFloorRound() +
                                     Convert.ToDecimal(data.GetType().GetProperties().Where(t => t.Name == "ybjj").First().GetValue(data)).ToFloorRound();
                            ws.Cells[rowindex, 1].Value = "职工医保合计";
                            ws.Cells[rowindex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 1], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 2].Value = $"收:￥{zgpircesum}/大写：{zgpircesum.NumtoChinese()}";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 2], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            rowindex++;

                            ws.Cells[rowindex, 1].Value = "个人帐户";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 1], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 2].Value = $"收:￥{prices}/大写：{prices.NumtoChinese()}";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 2], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            break;
                        case "mzbz":
                            ws.Cells[rowindex, 1].Value = "民政补助";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 1], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 2].Value = $"收:￥{prices}/大写：{prices.NumtoChinese()}";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 2], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            break;
                        case "detc":
                            ws.Cells[rowindex, 1].Value = "大额统筹";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 1], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 2].Value = $"收:￥{prices}/大写：{prices.NumtoChinese()}";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 2], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            break;
                        case "ybjj":
                            ws.Cells[rowindex, 1].Value = "医保基金";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 1], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 2].Value = $"收:￥{prices}/大写：{prices.NumtoChinese()}";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 2], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            break;

                        case "jmindividual":

                            var jmpircesum = Convert.ToDecimal(data.GetType().GetProperties().Where(t => t.Name == "jmindividual").First().GetValue(data)).ToFloorRound() + Convert.ToDecimal(data.GetType().GetProperties().Where(t => t.Name == "jmmzbz").First().GetValue(data)).ToFloorRound() +
                              Convert.ToDecimal(data.GetType().GetProperties().Where(t => t.Name == "jmdetc").First().GetValue(data)).ToFloorRound() +
                                 Convert.ToDecimal(data.GetType().GetProperties().Where(t => t.Name == "jmybjj").First().GetValue(data)).ToFloorRound();
                            ws.Cells[rowindex, 1].Value = "居民医保合计";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 1], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            ws.Cells[rowindex, 2].Value = $"收:￥{jmpircesum}/大写：{jmpircesum.NumtoChinese()}";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 2], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            rowindex++;

                            ws.Cells[rowindex, 1].Value = "个人帐户";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 1], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 2].Value = $"收:￥{prices}/大写：{prices.NumtoChinese()}";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 2], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            break;
                        case "jmmzbz":
                            ws.Cells[rowindex, 1].Value = "民政补助";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 1], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 2].Value = $"收:￥{prices}/大写：{prices.NumtoChinese()}";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 2], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            break;
                        case "jmdetc":
                            ws.Cells[rowindex, 1].Value = "大额统筹";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 1], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 2].Value = $"收:￥{prices}/大写：{prices.NumtoChinese()}";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 2], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            break;
                        case "jmybjj":
                            ws.Cells[rowindex, 1].Value = "医保基金";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 1], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 2].Value = $"收:￥{prices}/大写：{prices.NumtoChinese()}";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 2], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            break;


                        case "yycb":
                            ws.Cells[rowindex, 1].Value = "医院超标";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 1], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 2].Value = $"收:￥{prices}/大写：{prices.NumtoChinese()}";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 2], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            break;
                        case "pjst":
                            ws.Cells[rowindex, 1].Value = "票据收退";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 1], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 2].Value = $"暂无";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 2], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            break;
                        case "hmfw":
                            ws.Cells[rowindex, 1].Value = "号码范围";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 1], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 2].Value = $"暂无";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 2], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            break;
                        case "sjph":
                            ws.Cells[rowindex, 1].Value = "实际票号";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 1], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            ws.Cells[rowindex, 2].Value = $"暂无";
                            SetCellFormat2007OrAbove(ws.Cells[rowindex, 2], new ExcelColumnSetting() { IsAutoFitWidth = true });
                            break;
                        default:
                            break;
                    }
                    rowindex++;
                }
                ws.Column(1).Width = 14;//设置列宽
                ws.Column(2).Width = 70;
                ep.Save();
            }

        }



        /// <summary>
        /// 将对象的数据属性转换为EXCEL值
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="property">属性</param>
        /// <param name="model">对象</param>
        /// <param name="columnSetting">列设置</param>
        private static string ToExcelValue<T>(this PropertyInfo property, T model, ExcelColumnSetting columnSetting) where T : class
        {
            var result = string.Empty;
            if (model == null || property == null || columnSetting == null)
            {
                return null;
            }
            var midValue = property.GetValue(model);
            if (midValue == null)
            {
                if (columnSetting.BoolChineseDesc != null)
                {
                    result = columnSetting.BoolChineseDesc.NullValue;
                }
                else
                {
                    result = null;
                }
                return result;
            }
            if (midValue is Enum)
            {
                result = ((Enum)midValue).ToChinese();
            }
            else if (midValue is bool)
            {
                if (columnSetting.BoolChineseDesc != null)
                {
                    result = (bool)midValue
                        ? columnSetting.BoolChineseDesc.TrueValue : columnSetting.BoolChineseDesc.FalseValue;

                }
                else
                {
                    result = midValue.ToString();
                }
            }
            else if (midValue is DateTime)
            {
                result = string.IsNullOrEmpty(columnSetting.DateTimeFromat)
                    ? midValue.ToString() : ((DateTime)midValue).ToString(columnSetting.DateTimeFromat);
            }
            else
            {
                result = midValue.ToString();
            }
            return result;
        }

        /// <summary>
        /// 设置单元格格式（2003版本）
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="columnSetting"></param>
        private static void SetCellFromat2003(ICell cell, ExcelColumnSetting columnSetting)
        {
            cell.CellStyle.WrapText = columnSetting.WrapText;
        }

        /// <summary>
        /// 设置单元格格式（2007及以上版本）
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="columnSetting"></param>
        private static void SetCellFormat2007OrAbove(ExcelRange cell, ExcelColumnSetting columnSetting)
        {
            if (columnSetting.IsAutoFitWidth)
            {
                cell.AutoFitColumns();
            }
            cell.Style.WrapText = columnSetting.WrapText;
            cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        }
        /// <summary>
        /// 合并行
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="startRowIndex"></param>
        /// <param name="mergeRowIndexs">合并行的行数，起始位置，终止位置</param>
        public static void MergeRowCells(ExcelWorksheet sheet, int startRowIndex, int[,] mergeRowIndexs)
        {
            try
            {
                for (int i = 0; i < mergeRowIndexs.Rank; i++)
                {
                    sheet.Cells[mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 1], mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 2]].Merge = true;
                    sheet.Cells[mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 1], mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 2]].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    sheet.Cells[mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 1], mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 2]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    sheet.Cells[mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 1], mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 2]].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                }
            }
            catch (Exception exp)
            {

            }

        }
        private static void SetFontSty(ExcelRange cells, string fontName, float fontSize, bool Bold)
        {
            cells.Style.Font.Bold = Bold;//字体为粗体
                                         // cells.Style.Font.Color.SetColor(Color.White);//字体颜色
            cells.Style.Font.Name = fontName;//字体
            cells.Style.Font.Size = fontSize;//字体大小

        }


        #endregion

        #region import
        /// <summary>
        /// 从Excel将数据导入到DataTable中
        /// </summary>
        /// <param name="fileName">文件完整路径</param>
        /// <returns></returns>
        public static DataTable ReadExcel(string fileName, int rowIndex, int SheetIndex = 0)
        {

            /*
             
             */
            //E:\Work\血透中心集团管理信息系统\文档\xlsx\透析中心模版.xlsx   E:\Work\血透中心集团管理信息系统\文档\xlsx\康美佳人员信息表.xls
            DataTable table = new DataTable();
            // fileName = @"E:\Work\血透中心集团管理信息系统\文档\xlsx\档案模版\康美员工信息表模版.xlsx";
            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                IWorkbook workbook = WorkbookFactory.Create(fs);// WorkbookFactory.Create(fs);

                ISheet sheet = workbook.GetSheetAt(SheetIndex);
                IRow heaherRow = sheet.GetRow(rowIndex); //标题行
                if (heaherRow != null)
                {
                    int cellCount = heaherRow.LastCellNum; //列数  
                                                           //遍历标题
                    for (int i = heaherRow.FirstCellNum; i < cellCount; i++)
                    {
                        string title = heaherRow.GetCell(i).StringCellValue.Replace("\n", "").Trim();
                        DataColumn column = new DataColumn(title);
                        table.Columns.Add(column);
                    }

                    int rowCount = sheet.LastRowNum + 1; //行数
                    ICell cell = null;
                    //遍历行数
                    for (int i = rowIndex + 1; i < rowCount; i++)
                    {
                        IRow row = sheet.GetRow(i); //得到数据行
                        if (row != null)
                        {
                            DataRow dr = table.NewRow();
                            for (int j = heaherRow.FirstCellNum; j < cellCount; j++)
                            {
                                cell = row.GetCell(j);
                                if (cell != null)
                                {
                                    //读取Excel格式，根据格式读取数据类型
                                    switch (cell.CellType)
                                    {
                                        //
                                        case CellType.Blank: //空数据类型处理
                                            dr[j] = "";
                                            break;
                                        case CellType.String: //字符串类型
                                            dr[j] = cell.StringCellValue;
                                            break;
                                        case CellType.Numeric: //数字类型                                   
                                            if (HSSFDateUtil.IsCellDateFormatted(cell))
                                            {
                                                dr[j] = cell.DateCellValue;
                                            }
                                            else
                                            {
                                                dr[j] = cell.NumericCellValue;
                                            }
                                            break;
                                        case CellType.Formula:
                                            //HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(workbook);
                                            //dr[j] = e.Evaluate(cell).StringValue;
                                            if (cell.CachedFormulaResultType == CellType.Numeric)
                                                dr[j] = cell.NumericCellValue;
                                            if (cell.CachedFormulaResultType == CellType.String)
                                                dr[j] = cell.StringCellValue;

                                            break;
                                        default:
                                            dr[j] = "";
                                            break;
                                    }
                                }
                            }
                            table.Rows.Add(dr);
                        }
                    }
                }
                sheet = null;
                workbook = null;
            }
            catch (Exception exp)
            {
                if (fs != null)
                    fs.Close();
                throw exp;
            }

            return table;
        }
        #endregion
    }
}

