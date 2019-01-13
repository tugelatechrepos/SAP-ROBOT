using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPAutomationJob
{
    public class ExportRequest
    {
        public ICollection<Payment> PaymentList { get; set; }
        public Dictionary<string, Type> ColumnNamesList { get; set; }
    }

    public class ExportPaymentListProcessor
    {
        #region Declarations

        private ExcelPackage _ExcelPackage;
        private ExcelWorksheet _Worksheet;
        private List<string> _WorkSheetNameList;
        private Dictionary<string, Type> _ColumnNamesList;
        private DataTable _PaymentListDataTable;
        private ICollection<Payment> _PaymentList;
        private object _startFromRow;
        private ExcelRangeBase _DataRange;
        private ExportRequest _Request;

        #endregion Declarations

        public void Export(ExportRequest Request)
        {
            _Request = Request;
            _PaymentList = Request.PaymentList;
            _ColumnNamesList = Request.ColumnNamesList;
            _WorkSheetNameList = new List<string> { "PaymentList" };
            _startFromRow = 3;
            getWorkSheets();
        }

        private void getWorkSheets()
        {
            using (_ExcelPackage = new ExcelPackage())
            {
                ExcelWorksheet ws = _ExcelPackage.Workbook.Worksheets.Add(_WorkSheetNameList[0]);
                ws.Name = _WorkSheetNameList[0];
                ws.Cells.Style.Font.Size = 11;
                ws.Cells.Style.Font.Name = "Calibri";
                ws.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells.Style.Fill.BackgroundColor.SetColor(Color.White);

                _Worksheet = ws;
                assignPaymentListDataTable();
                assignRows();
                assignData();
                formatData();
                setBackgroundColorOfColumns();
                assignFormattedData();
            }
        }

        private void assignPaymentListDataTable()
        {
            _PaymentListDataTable = new DataTable();
            foreach (var field in _ColumnNamesList)
            {
                var dataColumn = new DataColumn(field.Key, field.Value);
                _PaymentListDataTable.Columns.Add(dataColumn);
            }
        }

        private void assignRows()
        {
            foreach (var payment in _PaymentList)
            {
                var row = _PaymentListDataTable.NewRow();
                foreach (var column in _PaymentListDataTable.Columns)
                {
                    var columnName = Convert.ToString(column);
                    row[columnName] = getValueFromColumnName(columnName, payment);
                }
                _PaymentListDataTable.Rows.Add(row);
            }
        }

        private object getValueFromColumnName(string columnName, Payment payment)
        {
            object value = null;

            switch (columnName)
            {
                case "AccountNumber":
                    value = payment.AccountNumber;
                    break;
                case "ServiceId":
                    value = payment.ServiceId;
                    break;
                case "PaymentDate":
                    value = payment.PaymentDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture); ;
                    break;
                case "PaymentType":
                    value = payment.PaymentType;
                    break;
                case "Amount":
                    value = payment.Amount;
                    break;
                case "Currency":
                    value = payment.Currency;
                    break;
                case "Description":
                    value = payment.Description;
                    break;

            }
            return value;
        }

        private void assignData()
        {
            _DataRange = _Worksheet.Cells[string.Format("A{0}", _startFromRow)].LoadFromDataTable(
                 _PaymentListDataTable, true, OfficeOpenXml.Table.TableStyles.Medium2);

            _Worksheet.Tables[0].ShowFilter = false;

        }

        private void formatData()
        {
            var modelTable = _Worksheet.Cells[_DataRange.Address];
            modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            modelTable.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            var AmountColumn = _Worksheet.Tables[0].Columns.Any(x => x.Name == "Amount") ?
                _Worksheet.Tables[0].Columns.FirstOrDefault(x => x.Name == "Amount").Id : 0;

            var amountColumnLetter = AmountColumn == 0 ? string.Empty : ColumnIndexToColumnLetter(AmountColumn);

            ExcelCellAddress start = _Worksheet.Tables[0].Address.Start;
            ExcelCellAddress end = _Worksheet.Tables[0].Address.End;

            _Worksheet.Cells[$"{amountColumnLetter}{start.Row + 1}:{amountColumnLetter}{end.Row}"].Style.Numberformat.Format = "#,##0.00";

            _Worksheet.Cells[_Worksheet.Dimension.Address].AutoFitColumns();
        }

        private void setBackgroundColorOfColumns()
        {

            var excelTable = _Worksheet.Tables[0];
            var endColumn = excelTable.Address.End.Column;
            var endColumnLetter = ColumnIndexToColumnLetter(endColumn);

            _Worksheet.Cells[string.Format("A{0}:{1}{0}", _startFromRow, endColumnLetter)].Style.Font.Bold = true;
            _Worksheet.Cells[string.Format("A{0}:{1}{0}", _startFromRow, endColumnLetter)].Style.Fill.PatternType = ExcelFillStyle.Solid;
            _Worksheet.Cells[string.Format("A{0}:{1}{0}", _startFromRow, endColumnLetter)].Style.Fill.BackgroundColor.SetColor(Color.Gray);
        }

        private string ColumnIndexToColumnLetter(int colIndex)
        {
            int div = colIndex;
            string colLetter = String.Empty;
            int mod = 0;

            while (div > 0)
            {
                mod = (div - 1) % 26;
                colLetter = (char)(65 + mod) + colLetter;
                div = (int)((div - mod) / 26);
            }
            return colLetter;
        }

        private void assignFormattedData()
        {
            byte[] data = _ExcelPackage.GetAsByteArray();
            var directoryPath = ConfigurationManager.AppSettings["Outputfolder"];
            var file = new FileInfo(directoryPath);
            file.Directory.Create();
            var fileName = $"PaymentList_{DateTime.Now.ToString("dd-MM-yyyy")}.xlsx";
            string path = $"{directoryPath}{ fileName }";
            File.WriteAllBytes(path, data);
        }
    }
}

