using ClassroomReservation.Server;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace ClassroomReservation.Resource {
    class ExcelReadClient {
        public static List<ReservationItem> readExcel() {
            //var fileName = string.Format("{0}\\fileNameHere", Directory.GetCurrentDirectory());
            //var connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", fileName);

            //var adapter = new OleDbDataAdapter("SELECT * FROM [workSheetNameHere$]", connectionString);
            //var ds = new DataSet();

            //adapter.Fill(ds, "anyNameHere");

            //DataTable data = ds.Tables["anyNameHere"];

            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true) {
                // Open document 
                //fileOpenTextBox.Text = dlg.FileName;
                Excel.Application excelApp = null;
                Excel.Workbook wb = null;
                Excel.Worksheet ws = null;

                try {
                    excelApp = new Excel.Application();

                    //파일 열기
                    wb = excelApp.Workbooks.Open(dlg.FileName);

                    //첫 번째 worksheet 선택
                    ws = wb.Worksheets.get_Item(1);

                    //현재 worksheet에서 사용된 셀 전체를 선택
                    Excel.Range rng = ws.UsedRange;

                    //range에 있는 data를 이중배열로 받아옴
                    object[,] data = rng.Value;

                    for (int row = 1; row <= data.GetLength(0); row++) {
                        for (int col = 1; col <= data.GetLength(1); col++) {
                            if (data[row, col] != null) {
                                Console.WriteLine(data[row, col].ToString());
                            }
                        }
                    }

                    wb.Close(true);
                    excelApp.Quit();
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    ReleaseExcelObject(ws);
                    ReleaseExcelObject(wb);
                    ReleaseExcelObject(excelApp);
                }
            }

            return null;
        }

        private static void ReleaseExcelObject(object obj) {
            try {
                if (obj != null) {
                    Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            } catch (Exception ex) {
                obj = null;
                throw ex;
            } finally {
                GC.Collect();
            }
        }
    }
}
