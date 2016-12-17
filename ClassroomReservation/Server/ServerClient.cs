using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using Newtonsoft.Json.Converters;

namespace ClassroomReservation.Server {
    class ServerClient {
        private const string serverDomain = "http://192.168.0.13/";
        private const string makeReservationPage = "reserv_make_one.php";
        private const string makeLecturePage = "lecture_add.php";
        private const string getDayReservationPage = "reserv_get_day.php";
        private const string deleteReservationOnePage = "reserv_delete_one.php";
        private const string getClassroomListPage = "classroom_list.php";

        public static void MakeReservation(ReservationItem reservation) {
            try {
                string url = serverDomain + makeReservationPage;

                string dataStr =
                    "startDate=" + reservation.startDate.ToString("yyyy-MM-dd") +
                    "&endDate=" + reservation.endDate.ToString("yyyy-MM-dd") +
                    "&startClass=" + reservation.startClass +
                    "&endClass=" + reservation.endClass +
                    "&classroom=" + reservation.classroom +
                    "&userName=" + reservation.userName +
                    "&contact=" + reservation.contact +
                    "&content=" + reservation.content +
                    "&password=" + reservation.password;

                //Console.WriteLine(dataStr);

                connect(url, dataStr);
            } catch (ServerException e) {
                throw e;
            }
        }

        public static void MakeLecture(LectureItem lecture, DateTime semesterStartDate) {
            try {
                string url = serverDomain + makeLecturePage;

                DateTime semesterEndDate = semesterStartDate.AddDays((7 * 16) - 1);

                string dataStr =
                    "year=" + lecture.year +
                    "&semester=" + lecture.semester +
                    "&dayOfWeek=" + lecture.dayOfWeek +
                    "&classtime=" + lecture.classtime +
                    "&classroom=" + lecture.classroom +
                    "&professor=" + lecture.professor +
                    "&contact=" + lecture.contact +
                    "&code=" + lecture.code +
                    "&name=" + lecture.name +
                    "&startDate=" + semesterStartDate.ToString("yyyy-MM-dd") +
                    "&endDate=" + semesterEndDate.ToString("yyyy-MM-dd");

                connect(url, dataStr);
            } catch (ServerException e) {
                throw e;
            }
        }

        public static bool DeleteReservation(int reservID, string password) {
            try {
                string url = serverDomain + deleteReservationOnePage;
                string dataStr = "reservID=" + reservID + "&password=" + password;

                string result = connect(url, dataStr);

                return result.Equals("1");
            } catch (ServerException e) {
                throw e;
            }
        }

        public static List<StatusItem> GetDayReservation(DateTime datePara) {
            try {
                List<StatusItem> items = new List<StatusItem>();
                string url = serverDomain + getDayReservationPage;

                string dataStr =
                    "date=" + datePara.ToString("yyyy-MM-dd");

                string result = connect(url, dataStr);

                //Console.WriteLine(date.ToString("yyyy-MM-dd") + " - " + result);

                dynamic json = JsonConvert.DeserializeObject<ExpandoObject>(result, new ExpandoObjectConverter());

                List<dynamic> array = json.data;
                
                for (int i = 0; i < array.Count; i++) {
                    int reservID = Int32.Parse(array[i].ReservID);
                    int type = Int32.Parse(array[i].Type);
                    DateTime date = Convert.ToDateTime(array[i].Date);
                    int classtime = Int32.Parse(array[i].Classtime);
                    string classroom = array[i].Classroom;
                    string userName = array[i].UserName;
                    string contact = array[i].Contact;
                    string content = array[i].Content;

                    StatusItem item = new StatusItem(reservID, type, date, classtime, classroom, userName, contact, content);

                    items.Add(item);
                }

                return items;
            } catch (ServerException e) {
                throw e;
            }
        }

        public static string[] GetClassroomList() {
            try {
                return connect(serverDomain + getClassroomListPage, "").Split('\n');
            } catch (ServerException e) {
                throw e;
            }
        }

        public static string connect(string url, string dataStr) {
            try {
                byte[] data = UTF8Encoding.UTF8.GetBytes(dataStr);

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = data.Length;
                httpWebRequest.Timeout = 1000;

                Stream requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                Stream respPostStream = httpResponse.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, Encoding.GetEncoding("UTF-8"), true);

                string result = readerPost.ReadToEnd();

                Console.WriteLine("result : " + result);

                return result;
            } catch (Exception e) {
                throw new ServerException(e);
            }
        }
    }

    public class ServerException : Exception {
        public int res { get; private set; }
        public string msg { get; private set; }
        public string query { get; private set; }
        public Exception exception { get; private set; }

        public ServerException() { }

        public ServerException(Exception ex) {
            this.exception = ex;
        }

        public ServerException(int res, string msg, string query) {
            this.res = res;
            this.msg = msg;
            this.query = query;
        }
    }
}
