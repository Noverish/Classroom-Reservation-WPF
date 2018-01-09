using ClassroomReservation.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace ClassroomReservation.Resource {
    public delegate void onFileSelected(string fileName);

    class ExcelReadClient {
        public static onFileSelected onFileSelected { private get; set; }

        public static List<LectureItem> readExcel() {
            List<LectureItem> items = new List<LectureItem>();
            
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".xls";
            dlg.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
 
            if (dlg.ShowDialog() != true) {
                return null;
            }

            onFileSelected?.Invoke(dlg.FileName);

            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook wb = excelApp.Workbooks.Open(dlg.FileName);
            Excel.Worksheet ws = wb.Worksheets.get_Item(1);
            Excel.Range rng = ws.UsedRange;

            object[,] data = rng.Value.Clone() as object[,];

            wb.Close(true);
            excelApp.Quit();
            ReleaseExcelObject(ws);
            ReleaseExcelObject(wb);
            ReleaseExcelObject(excelApp);

            for (int row = 1; row <= data.GetLength(0); row++) {
                string[] rowArray = new string[data.GetLength(1)];

                for (int col = 1; col <= data.GetLength(1); col++) {
                    object tmp = data[row, col];

                    if (tmp == null)
                        tmp = "";

                    rowArray[col - 1] = tmp.ToString();
                }

                LectureItem item = null;

                try {
                    item = ProcessRow(row, rowArray);
                } catch (Exception ex) {
                    return null;
                }

                if (item != null)
                    items.Add(item);
            }

            return items;
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

        private static LectureItem ProcessRow(int index, string[] row) {
            int year;
            try {
                year = Int32.Parse(row[0]);
            } catch (Exception _) {
                string msg = "A" + index + "셀에 연도를 숫자로 입력해 주시기 바랍니다. (예: 2016)";
                MessageBox.Show(msg, "에러", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new Exception();
            }

            int semester;
            try {
                semester = Int32.Parse(new Regex("[^0-9]").Replace(row[1], ""));
            } catch (Exception _) {
                string msg = "B" + index + "셀에 학기를 입력해 주시기 바랍니다. (예: 1학기, 2학기)";
                MessageBox.Show(msg, "에러", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new Exception();
            }

            string code;
            try {
                if (row[3].Equals(""))
                    row[3] = "0";

                string lectureCode = row[2];
                string classCode = Int32.Parse(row[3]).ToString("D2");
                code = row[2] + "(" + classCode + ")";
            } catch (Exception _) {
                string msg = "D" + index + "셀에 분반을 숫자로 입력해 주시기 바랍니다. (예: 0, 00, 01, 1)";
                MessageBox.Show(msg, "에러", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new Exception();
            }

            string name = row[4];
            string professor = row[5];
            string contact = row[6];

            if (row[7].Trim().Equals("")) {
                string msg = "H" + index + "셀에 강의실 내용을 입력해 주시기 바랍니다. (예: 월(2-3) 과도관 202호)";
                MessageBox.Show(msg, "에러", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new Exception();
            }

            string dayOfWeekList = "", classtimeList = "", classroomList = "";
            try {
                foreach (string l in row[7].Trim().Split('\n')) {
                    string line = l.Trim();

                    //월화수목금토일
                    string day = line.ToCharArray()[0].ToString();

                    //(5) 또는 (2-3)
                    string times = Regex.Match(line, "[(]\\d+(-\\d+)?[)]").Value;

                    //과도관:202호
                    string classroom = line.Replace(day, "").Replace(times, "").Trim().Replace(" ", ":");

                    //모르는 강의실이 있으면 우리랑 상관 없으므로 패스
                    int classroowRow = ServerClient.getInstance().GetRowByClassroom(classroom);
                    if (classroowRow < 0) {
                        continue;
                    }

                    //괄호 삭제
                    times = times.Replace("(", "").Replace(")", "");

                    //시간이 5 이렇게 하나면 5-5 이렇게 바꿈
                    if (!times.Contains("-")) {
                        times = times + "-" + times;
                    }

                    dayOfWeekList += day + ";";
                    classtimeList += times + ";";
                    classroomList += classroowRow + ";";
                }
            } catch (Exception ex) {
                string msg = "H" + index + "셀에 알 수 없는 내용이 있습니다.";
                MessageBox.Show(msg, "에러", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new Exception();
            }

            if (dayOfWeekList.Length == 0)
                return null;

            dayOfWeekList = dayOfWeekList.Substring(0, dayOfWeekList.Length - 1);
            classtimeList = classtimeList.Substring(0, classtimeList.Length - 1);
            classroomList = classroomList.Substring(0, classroomList.Length - 1);

            return new LectureItem(year, semester, dayOfWeekList, classtimeList, classroomList, professor, contact, code, name);
        }
    }
}
