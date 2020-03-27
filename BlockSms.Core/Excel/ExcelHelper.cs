using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;

using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace BlockSms.Core.Excel
{
    /// <summary>
    /// 描 述：Net Core NPOI Excel 操作类
    /// </summary>
    public static class ExcelHelper
    {
        #region 导出Excel

        /// <summary>
        /// List导出到Excel文件 Export()
        /// </summary>
        /// <param name="dtSource">DataTable数据源</param>
        /// <param name="excelConfig">导出设置包含文件名、标题、列设置</param>
        public static IWorkbook ToExcelWorkbook<T>(this List<T> dtSource, ExcelConfig excelConfig)
        {
            return ExportWorkbook<T>(dtSource, excelConfig);
        }

        /// <summary>
        /// List导出到Excel文件 Export()
        /// </summary>
        /// <param name="dtSource">DataTable数据源</param>
        /// <param name="excelConfig">导出设置包含文件名、标题、列设置</param>
        public static void ToExcelFile<T>(this List<T> dtSource, ExcelConfig excelConfig)
        {
            var workbook = ExportWorkbook<T>(dtSource, excelConfig);
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                using (FileStream fs = new FileStream(@"D:\" + excelConfig.FileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
                ms.Flush();
            }
        }

        /// <summary>
        /// List导出到Excel文件 Export()
        /// </summary>
        /// <param name="dtSource">DataTable数据源</param>
        /// <param name="excelConfig">导出设置包含文件名、标题、列设置</param>
        public static byte[] ToExcelByte<T>(this List<T> dtSource, ExcelConfig excelConfig)
        {
            byte[] buffer = new byte[1024 * 10];
            var workbook = ExportWorkbook<T>(dtSource, excelConfig);
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                buffer = ms.ToArray();
                ms.Close();
            }
            return buffer;
        }


        #region 导出设置项
        /// <summary>
        /// List<T>导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">List<T>数据源</param>
        /// <param name="excelConfig">导出设置包含文件名、标题、列设置</param>
        private static IWorkbook ExportWorkbook<T>(List<T> dtSource, ExcelConfig excelConfig)
        {
            if (dtSource.Count == 0) return null;

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();

            //workbook.DocumentSummaryInformation = GetDSI();
            //workbook.SummaryInformation = GetSI();

            ICellStyle headStyle = GetHeadStyle(excelConfig, workbook); // 设置表头样式
            ICellStyle bHeadStyle = GetBHeadStyle(excelConfig, workbook); // 制表详情样式
            ICellStyle cHeadStyle = GetCHeadStyle(excelConfig, workbook); // 列头及样式
            ICellStyle[] arryColumStyle = GetArryColStyle(excelConfig, workbook);// 设置内容单元格样式 

            int rowIndex = 0;
            rowIndex = CreateTitleRow(excelConfig, sheet, headStyle, rowIndex); // 设置表头高度以及内容，绑定样式
            rowIndex = CreateDescribeRow(excelConfig, sheet, bHeadStyle, rowIndex); // 设置制表详情高度以及内容，绑定样式
            rowIndex = CreateHeadRow(excelConfig, sheet, cHeadStyle, rowIndex); // 设置列头高度宽度以及内容，绑定样式

            #region 填充列头以及数据

            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd HH:MM:ss");

            var cols = excelConfig.Columns.ToList();

            Type entityType = dtSource[0].GetType();
            var properties = entityType.GetProperties();

            try
            {
                foreach (var item in dtSource)
                {
                    #region 新建表，填充表头，填充列头，样式
                    if (rowIndex % 65535 == 0)
                    {
                        sheet = workbook.CreateSheet();
                        rowIndex = 0;
                        rowIndex = CreateTitleRow(excelConfig, sheet, headStyle, rowIndex); // 设置表头高度以及内容，绑定样式
                        rowIndex = CreateDescribeRow(excelConfig, sheet, bHeadStyle, rowIndex); // 设置制表详情高度以及内容，绑定样式 
                        rowIndex = CreateHeadRow(excelConfig, sheet, cHeadStyle, rowIndex); // 设置列头高度宽度以及内容，绑定样式
                    }
                    #endregion

                    #region 填充内容
                    IRow dataRow = sheet.CreateRow(rowIndex);
                    for (int j = 0; j < cols.Count; j++)
                    {
                        ICell newCell = dataRow.CreateCell(j);
                        newCell.CellStyle = arryColumStyle[j];
                        string drValue = properties.FirstOrDefault(s => s.Name == cols[j].Column).GetValue(item)?.ToString();
                        SetCell(newCell, dateStyle, cols[j], drValue);
                    }
                    #endregion
                    rowIndex++;
                }
            }
            catch (Exception err)
            {
                var msg = err.Message;
            }

            #endregion

            return workbook;
        }

        /// <summary>
        /// 右击文件 属性信息
        /// </summary>
        private static DocumentSummaryInformation GetDSI()
        {
            var dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "西安丝路智慧旅游";
            return dsi;
        }

        /// <summary>
        /// 右击文件 属性信息
        /// </summary>
        private static SummaryInformation GetSI()
        {
            var si = PropertySetFactory.CreateSummaryInformation();
            si.Author = "武盼锋";
            si.ApplicationName = "西安丝路智慧旅游";
            si.LastAuthor = "武盼锋";
            si.Comments = "武盼锋";
            si.Title = "西安丝路智慧旅游";
            si.Subject = "西安丝路智慧旅游";
            si.CreateDateTime = System.DateTime.Now;
            return si;
        }

        /// <summary>
        /// 设置表头样式
        /// </summary>
        private static ICellStyle GetHeadStyle(ExcelConfig excelConfig, IWorkbook workbook)
        {
            ICellStyle headStyle = workbook.CreateCellStyle();
            headStyle.Alignment = HorizontalAlignment.Center;
            if (excelConfig.Background != new Color())//背景色
            {
                headStyle.FillPattern = FillPattern.SolidForeground;
                headStyle.FillForegroundColor = GetXLColour(excelConfig.Background);
            }
            IFont font = workbook.CreateFont();
            if (excelConfig.ForeColor != new Color())//前景色
                font.Color = GetXLColour(excelConfig.ForeColor);
            font.FontHeightInPoints = excelConfig.TitlePoint;//字号
            font.FontName = excelConfig.TitleFont;//字体
            headStyle.SetFont(font);
            return headStyle;
        }

        /// <summary>
        /// 制表详情样式
        /// </summary>
        private static ICellStyle GetBHeadStyle(ExcelConfig excelConfig, IWorkbook workbook)
        {
            ICellStyle bHeadStyle = workbook.CreateCellStyle();
            bHeadStyle.Alignment = HorizontalAlignment.Right;
            bHeadStyle.VerticalAlignment = VerticalAlignment.Center;
            IFont bfont = workbook.CreateFont();
            bfont.FontHeightInPoints = excelConfig.HeadPoint;//字号
            bfont.FontName = excelConfig.HeadFont;//字体
            bHeadStyle.SetFont(bfont);
            return bHeadStyle;
        }

        /// <summary>
        /// 列头及样式
        /// </summary>
        private static ICellStyle GetCHeadStyle(ExcelConfig excelConfig, IWorkbook workbook)
        {
            ICellStyle cHeadStyle = workbook.CreateCellStyle();
            cHeadStyle.Alignment = HorizontalAlignment.Center;
            cHeadStyle.VerticalAlignment = VerticalAlignment.Center;
            IFont cfont = workbook.CreateFont();
            cfont.FontHeightInPoints = excelConfig.HeadPoint;//字号
            cfont.FontName = excelConfig.HeadFont;//字体
            cHeadStyle.SetFont(cfont);
            return cHeadStyle;
        }

        /// <summary>
        /// 设置内容单元格样式
        /// </summary>
        private static ICellStyle[] GetArryColStyle(ExcelConfig excelConfig, IWorkbook workbook)
        {
            ICellStyle[] arryColumStyle = new ICellStyle[excelConfig.Columns.Count];//样式表
            foreach (ColumnModel item in excelConfig.Columns)
            {
                ICellStyle columnStyle = workbook.CreateCellStyle();
                columnStyle.Alignment = HorizontalAlignment.Center;

                int columnentityIndex = excelConfig.Columns.FindIndex(t => t.Column == item.Column);
                if (item.Background != new Color())
                {
                    columnStyle.FillPattern = FillPattern.SolidForeground;
                    columnStyle.FillForegroundColor = GetXLColour(item.Background);
                }

                IFont columnFont = workbook.CreateFont();
                if (item.Font != null) columnFont.FontName = item.Font;
                if (item.ForeColor != new Color()) columnFont.Color = GetXLColour(item.ForeColor);
                columnFont.FontHeightInPoints = item.Point;
                columnStyle.SetFont(columnFont);
                columnStyle.Alignment = getAlignment(item.Alignment);
                arryColumStyle[columnentityIndex] = columnStyle;
            }

            return arryColumStyle;
        }

        /// <summary>
        /// 设置表头高度以及内容，绑定样式
        /// </summary>
        private static int CreateTitleRow(ExcelConfig excelConfig, ISheet sheet, ICellStyle headStyle, int rowIndex)
        {
            if (excelConfig.Title != null)
            {
                IRow headerRow = sheet.CreateRow(rowIndex);
                headerRow.HeightInPoints = excelConfig.TitleHeight;//高度
                headerRow.CreateCell(0).SetCellValue(excelConfig.Title);
                headerRow.GetCell(0).CellStyle = headStyle;
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, excelConfig.Columns.Count - 1));
                rowIndex++;
            }
            return rowIndex;
        }

        /// <summary>
        /// 设置制表详情高度以及内容，绑定样式
        /// </summary>
        /// <returns></returns>
        private static int CreateDescribeRow(ExcelConfig excelConfig, ISheet sheet, ICellStyle bHeadStyle, int rowIndex)
        {
            if (excelConfig.Title != null)
            {
                IRow headerRow = sheet.CreateRow(rowIndex);
                headerRow.HeightInPoints = excelConfig.HeadHeight;//高度
                headerRow.CreateCell(0).SetCellValue($"制表时间：{DateTime.Now.ToString("yyyy年MM月dd日")}");
                headerRow.GetCell(0).CellStyle = bHeadStyle;
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 0, excelConfig.Columns.Count - 1));
                rowIndex++;
            }

            return rowIndex;
        }

        /// <summary>
        /// 设置列头高度宽度以及内容，绑定样式
        /// </summary>
        private static int CreateHeadRow(ExcelConfig excelConfig, ISheet sheet, ICellStyle cHeadStyle, int rowIndex)
        {
            IRow headerRow = sheet.CreateRow(rowIndex);
            headerRow.HeightInPoints = excelConfig.HeadHeight;//高度

            for (int i = 0; i < excelConfig.Columns.Count; i++)
            {
                headerRow.CreateCell(i).SetCellValue(excelConfig.Columns[i].ExcelColumn);
                headerRow.GetCell(i).CellStyle = cHeadStyle;
                int colWidth = (excelConfig.Columns[i].Width ?? (excelConfig.Columns[i].ExcelColumn.Length + 1)) * 256; //设置列宽
                if (colWidth < 255 * 256) sheet.SetColumnWidth(i, colWidth < 3000 ? 3000 : colWidth);
            }
            rowIndex++;
            return rowIndex;
        }

        #endregion 
        #endregion

        #region 导入Excel
        /// <summary>
        /// 读取excel ,默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public static DataTable ExcelImport(string strFileName)
        {
            DataTable dt = new DataTable();

            ISheet sheet = null;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                if (strFileName.IndexOf(".xlsx") == -1)//2003
                {
                    HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);
                    sheet = hssfworkbook.GetSheetAt(0);
                }
                else//2007
                {
                    XSSFWorkbook xssfworkbook = new XSSFWorkbook(file);
                    sheet = xssfworkbook.GetSheetAt(0);
                }
            }

            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                dt.Rows.Add(dataRow);
            }
            return dt;
        }
        /// <summary>
        /// 读取excel ,默认第一行为标头 
        /// </summary>
        /// <param name="fileStream">文件数据流</param>
        /// <returns></returns>
        public static DataTable ExcelImport(Stream fileStream, string flieType)
        {
            DataTable dt = new DataTable();
            ISheet sheet = null;
            if (flieType == ".xls")
            {
                HSSFWorkbook hssfworkbook = new HSSFWorkbook(fileStream);
                sheet = hssfworkbook.GetSheetAt(0);
            }
            else
            {
                XSSFWorkbook xssfworkbook = new XSSFWorkbook(fileStream);
                sheet = xssfworkbook.GetSheetAt(0);
            }
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                dt.Rows.Add(dataRow);
            }
            return dt;
        }
        #endregion

        #region 设置单元格内容
        private static void SetCell(ICell newCell, ICellStyle dateStyle, ColumnModel colModel, string drValue)
        {
            string dataType = colModel.ColumnType.ToString();
            switch (dataType.ToString())
            {
                case "String":
                    newCell.SetCellValue(drValue);
                    break;
                case "DateTime":
                    DateTime dateV;
                    if (DateTime.TryParse(drValue, out dateV)) newCell.SetCellValue(dateV);
                    else newCell.SetCellValue("");
                    newCell.CellStyle = dateStyle;
                    break;
                case "Boolean":
                    bool boolV = false;
                    bool.TryParse(drValue, out boolV);
                    newCell.SetCellValue(boolV);
                    break;
                case "Int":
                case "Byte":
                    int intV = 0;
                    int.TryParse(drValue, out intV);
                    newCell.SetCellValue(intV);
                    break;
                case "Decimal":
                case "Double":
                    double doubV = 0;
                    double.TryParse(drValue, out doubV);
                    newCell.SetCellValue(doubV);
                    break;
                case "DBNull":
                    newCell.SetCellValue("");
                    break;
                case "Enum":
                    int intTemp = 0;
                    string stringV = "";
                    if (int.TryParse(drValue, out intTemp) && colModel.ColumnEnum.ContainsKey(intTemp))
                        stringV = colModel.ColumnEnum.FirstOrDefault(s => s.Key == intTemp).Value;
                    newCell.SetCellValue(stringV);
                    break;
                default:
                    newCell.SetCellValue("");
                    break;
            }
        }
        #endregion

        #region RGB颜色转NPOI颜色
        private static short GetXLColour(Color SystemColour)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            short s = 0;
            HSSFPalette XlPalette = workbook.GetCustomPalette();
            NPOI.HSSF.Util.HSSFColor XlColour = XlPalette.FindColor(SystemColour.R, SystemColour.G, SystemColour.B);
            if (XlColour == null)
            {
                if (NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE < 255)
                {
                    XlColour = XlPalette.FindSimilarColor(SystemColour.R, SystemColour.G, SystemColour.B);
                    s = XlColour.Indexed;
                }

            }
            else s = XlColour.Indexed;
            return s;
        }
        #endregion

        #region 设置列的对齐方式
        private static HorizontalAlignment getAlignment(string style)
        {
            switch (style)
            {
                case "center":
                    return HorizontalAlignment.Center;
                case "left":
                    return HorizontalAlignment.Left;
                case "right":
                    return HorizontalAlignment.Right;
                case "fill":
                    return HorizontalAlignment.Fill;
                case "justify":
                    return HorizontalAlignment.Justify;
                case "centerselection":
                    return HorizontalAlignment.CenterSelection;
                case "distributed":
                    return HorizontalAlignment.Distributed;
            }
            return HorizontalAlignment.General;


        }
        #endregion
    }
}
