using System.Collections.Generic;
using System.Drawing;

namespace BlockSms.Core.Excel
{
    /// <summary>
    /// 描 述：Excel导入导出列设置模型
    /// </summary>
    public class ColumnModel
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string Column { get; set; }
        /// <summary>
        /// Excel列名
        /// </summary>
        public string ExcelColumn { get; set; }
        /// <summary>
        /// Excel列宽(最大256)
        /// </summary>
        public int? Width { get; set; }
        /// <summary>
        /// 列类型
        /// </summary>
        public ColType ColumnType { get; set; } = ColType.String;
        /// <summary>
        /// 前景色
        /// </summary>
        public Color ForeColor { get; set; }
        /// <summary>
        /// 背景色
        /// </summary>
        public Color Background { get; set; }
        /// <summary>
        /// 字体
        /// </summary>
        public string Font { get; set; } = "微软雅黑";
        /// <summary>
        /// 字号
        /// </summary>
        public short Point { get; set; } = 10;
        /// <summary>
        ///对齐方式
        ///left 左
        ///center 中间
        ///right 右
        ///fill 填充
        ///justify 两端对齐
        ///centerselection 跨行居中
        ///distributed
        /// </summary>
        public string Alignment { get; set; }

        public Dictionary<int, string> ColumnEnum { get; set; }

        public enum ColType
        {
            String = 1,
            DateTime = 2,
            Boolean = 3,
            Int = 4,
            Byte = 5,
            Decimal = 6,
            Double = 7,
            DBNull = 8,
            Enum = 9
        }
    }

}
