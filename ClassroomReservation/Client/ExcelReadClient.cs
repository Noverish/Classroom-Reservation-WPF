using ClassroomReservation.Server;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Excel = Microsoft.Office.Interop.Excel;

namespace ClassroomReservation.Resource {
    public delegate void onFileSelected(string fileName);

    class ExcelReadClient {
        public static onFileSelected onFileSelected { private get; set; }

        public static List<LectureItem> readExcel() {
            List<LectureItem> items = new List<LectureItem>();

            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true) {
                onFileSelected?.Invoke(dlg.FileName);

                // Open document 
                Excel.Application excelApp = null;
                Excel.Workbook wb = null;
                Excel.Worksheet ws = null;


                excelApp = new Excel.Application();

                //파일 열기
                wb = excelApp.Workbooks.Open(dlg.FileName);

                //첫 번째 worksheet 선택
                ws = wb.Worksheets.get_Item(1);

                //현재 worksheet에서 사용된 셀 전체를 선택
                Excel.Range rng = ws.UsedRange;

                //range에 있는 data를 이중배열로 받아옴
                object[,] data = rng.Value;

                bool validRow = true;
                for (int row = 1; row <= data.GetLength(0); row++) {
                    string[] rowArray = new string[data.GetLength(1)];

                    for (int col = 1; col <= data.GetLength(1); col++) {
                        if (data[row, col] == null || (data[row, col].ToString().Trim().Equals("") && col != 7)) {
                            validRow = false;
                            break;
                        }

                        rowArray[col - 1] = data[row, col].ToString();
                    }

                    if (validRow) {
                        var item = ProcessRow(rowArray);
                        if (item != null)
                            items.Add(item);
                    }
                }

                wb.Close(true);
                excelApp.Quit();

                ReleaseExcelObject(ws);
                ReleaseExcelObject(wb);
                ReleaseExcelObject(excelApp);

                return items;
            } else {
                return null;
            }
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

        private static LectureItem ProcessRow(string[] row) {
            try {
                int year = Int32.Parse(row[0]);
                int semester = Int32.Parse(new Regex("[^0-9]").Replace(row[1], ""));
                string code = row[2] + "(" + Int32.Parse(row[3]).ToString("D2") + ")";
                string name = row[4];
                string professor = row[5];
                string contact = row[6];
                string tmp = (new Regex("\\s+").Replace(row[7], " "));
                string dayOfWeekList = "", classtimeList = "", classroomList = "";

                List<Match> matches = new List<Match>();
                foreach (Match m in Regex.Matches(tmp, "\\S[(]\\d(-\\d)?[)]"))
                    if (m.Success)
                        matches.Add(m);
                    else
                        Console.WriteLine("{0} ", m.Index);

                for (int i = 0; i < matches.Count; i++) {
                    string one;

                    if (i != matches.Count - 1) {
                        one = tmp.Substring(matches[i].Index, matches[i + 1].Index);
                    } else {
                        one = tmp.Substring(matches[i].Index);
                    }

                    one = one.Trim();

                    string day = one.ToCharArray()[0].ToString();
                    string times = Regex.Match(matches[i].Value, "\\d(-\\d)?").Value;
                    string classroom = new Regex(" ").Replace(one.Remove(0, matches[i].Value.Length + 1), ":");

                    if (Database.getInstance().GetRowByClassroom(classroom) < 0)
                        continue;

                    times = (times.Contains("-")) ? times : times + "-" + times;

                    if (i == 0) {
                        dayOfWeekList = day;
                        classtimeList = times;
                        classroomList = classroom;
                    } else {
                        dayOfWeekList += ";" + day;
                        classtimeList += ";" + times;
                        classroomList += ";" + classroom;
                    }
                }

                if (classroomList.Equals(""))
                    return null;

                return new LectureItem(year, semester, dayOfWeekList, classtimeList, classroomList, professor, contact, code, name);
            } catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }
    }
}
