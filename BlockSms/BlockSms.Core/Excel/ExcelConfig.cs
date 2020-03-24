using System;
using System.Collections.Generic;
using System.Drawing;

namespace BlockSms.Core.Excel
{
    /// <summary>
    /// 描 述：Excel导入导出设置
    /// </summary>
    public class ExcelConfig
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 标题前景色
        /// </summary>
        public Color ForeColor { get; set; }

        /// <summary>
        /// 标题背景色
        /// </summary>
        public Color Background { get; set; }

        /// <summary>
        /// 标题字体
        /// </summary>
        public string TitleFont { get; set; } = "微软雅黑";

        /// <summary>
        /// 标题字号
        /// </summary>
        public short TitlePoint { get; set; } = 20;

        /// <summary>
        /// 标题高度
        /// </summary>
        public short TitleHeight { get; set; } = 30;

        /// <summary>
        /// 列头字体
        /// </summary>
        public string HeadFont { get; set; } = "微软雅黑";

        /// <summary>
        /// 列头字号
        /// </summary>
        public short HeadPoint { get; set; } = 12;

        /// <summary>
        /// 列标题高度
        /// </summary>
        public short HeadHeight { get; set; } = 20;


        /// <summary>
        /// 是否按内容长度来适应表格宽度
        /// </summary>
        [Obsolete("导出数据量大，会导致列表宽度计算很慢，不建议使用")]
        public bool IsAllSizeColumn { get; set; }
        /// <summary>
        /// 列设置
        /// </summary>
        public List<ColumnModel> Columns { get; set; }


    }
}
