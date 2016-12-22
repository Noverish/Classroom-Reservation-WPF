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
using System.Collections;

namespace ClassroomReservation.Server {
    class ServerClient {
        private const string serverDomain = "http://10.4.0.250/api/";

        private const string reservationListDayUrl = "reservation_list_day.php";
        private const string reservationAddUrl = "reservation_add.php";
        private const string reservationDeleteOneUrl = "reservation_delete_one.php";
        private const string reservationDeletePeriodUrl = "reservation_delete_period.php";
        private const string reservationModifyUrl = "reservation_modify.php";
        
        private const string lectureAddUrl = "lecture_add.php";

        private const string classroomListUrl = "classroom_list.php";
        private const string classroomAddUrl = "classroom_add.php";
        private const string classroomDeleteUrl = "classroom_delete.php";

        private const string classtimeListUrl = "classtime_list.php";



        public static List<StatusItem> reservationListDay(DateTime datePara) {
            try {
                List<StatusItem> items = new List<StatusItem>();
                string url = serverDomain + reservationListDayUrl;

                string dataStr =
                    "date=" + datePara.ToString("yyyy-MM-dd");

                ServerResult result = connect(url, dataStr);
                
                List<dynamic> array = result.data;
                
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
            } catch (ServerResult e) {
                throw e;
            }
        }

        public static void reservationAdd(ReservationItem reservation) {
            try {
                string url = serverDomain + reservationAddUrl;

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

                connect(url, dataStr);
            } catch (ServerResult e) {
                throw e;
            }
        }
        
        public static bool reservationDeleteOne(int reservID, string password) {
            try {
                string url = serverDomain + reservationDeleteOneUrl;
                string dataStr = "reservID=" + reservID + "&password=" + password;

                ServerResult result = connect(url, dataStr);

                return result.res == 1;
            } catch (ServerResult e) {
                throw e;
            }
        }

        public static void reservationDeletePeriod(DateTime startDate, DateTime endDate, bool deleteLecture) {
            try {
                string url = serverDomain + reservationDeletePeriodUrl;
                string dataStr = 
                    "startDate=" + startDate.ToString("yyyy-MM-dd") + 
                    "&endDate=" + endDate.ToString("yyyy-MM-dd") +
                    "&deleteLecture=" + ((deleteLecture) ? 1 : 0);

                connect(url, dataStr);
            } catch (ServerResult e) {
                throw e;
            }
        }

        public static bool reservationModify(int reservID, string password, string userName, string contact, string content) {
            try {
                string url = serverDomain + reservationModifyUrl;
                string dataStr =
                    "reservID=" + reservID +
                    "&password=" + password +
                    "&userName=" + userName +
                    "&contact=" + contact +
                    "&content=" + content;

                ServerResult result = connect(url, dataStr);

                return result.res == 1;
            } catch (ServerResult e) {
                throw e;
            }
        }


        public static void lectureAdd(LectureItem lecture, DateTime semesterStartDate) {
            try {
                string url = serverDomain + lectureAddUrl;

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
            } catch (ServerResult e) {
                throw e;
            }
        }


        public static string[] classroomList() {
            try {
                ServerResult result = connect(serverDomain + classroomListUrl, "");

                List<dynamic> data = result.data;

                string[] classrooms = new string[data.Count];

                for (int i = 0; i < data.Count; i++)
                    classrooms[i] = data[i].Classroom;

                return classrooms;
            } catch (ServerResult e) {
                throw e;
            }
        }

        public static void classroomAdd(string classroom) {
            try {
                string url = serverDomain + classroomAddUrl;

                string dataStr = "classroom=" + classroom;

                connect(url, dataStr);
            } catch (ServerResult e) {
                throw e;
            }
        }

        public static void classroomDelete(string classroom) {
            try {
                string url = serverDomain + classroomDeleteUrl;

                string dataStr = "classroom=" + classroom;

                connect(url, dataStr);
            } catch (ServerResult e) {
                throw e;
            }
        }


        public static Hashtable classtimeList() {
            try {
                Hashtable table = new Hashtable();

                ServerResult result = connect(serverDomain + classtimeListUrl, "");

                List<dynamic> data = result.data;

                for (int i = 0; i < data.Count; i++)
                    table.Add(i + 1, data[i].Detail);

                return table;
            } catch (ServerResult e) {
                throw e;
            }
        }
        

        public static ServerResult connect(string url, string dataStr) {
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

                string resultStr = readerPost.ReadToEnd();

                Console.WriteLine("result : " + resultStr);

                ServerResult result = new ServerResult(resultStr);

                if (result.res == 0)
                    throw result;
                else
                    return result;
            } catch (Exception e) {
                throw new ServerResult(e);
            }
        }
    }

    public class ServerResult : Exception {
        public int res { get; private set; }
        public string msg { get; private set; }
        public string query { get; private set; }
        public Exception exception { get; private set; }
        public List<dynamic> data { get; private set; }

        public ServerResult() { }

        public ServerResult(string result) {
            dynamic json = JsonConvert.DeserializeObject<ExpandoObject>(result, new ExpandoObjectConverter());

            this.res = Int32.Parse(json.res);
            this.msg = json.msg;
            this.query = json.query;
            this.data = json.data;
        }

        public ServerResult(Exception ex) {
            this.exception = ex;
        }

        public ServerResult(int res, string msg, string query) {
            this.res = res;
            this.msg = msg;
            this.query = query;
        }
    }
}
