using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrackingPackages.Models
{
    public class ExcelFile
    {
        public string Path { get; set; }
        public _Application Excel { get; set; } = new Application();

        public Workbook Wb{ get; set; }
        public Worksheet Ws{ get; set; }
        public Range Range { get; set; }

        public int RowCount { get; set; }
        public int ColCount { get; set; }

        public ExcelFile(){ }
        public ExcelFile(string path, int sheet)
        {
            Path = path;
            Wb = Excel.Workbooks.Open(path);
            Ws = Wb.Worksheets[sheet];
            Range = Ws.UsedRange;
            RowCount = Range.Rows.Count;
            ColCount = Range.Columns.Count;
        }

        public string ReadCell(int i, int j)
        {
            if(Ws.Cells[i,j].Value2 != null) return Ws.Cells[i, j].Value2;
            return "";
        }
    }
}