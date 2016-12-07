using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace ClassroomReservation.Server {
    class ServerClient {
        private const string serverDomain = "http://10.4.0.250/";
        private const string makeReservationPage = "reserv_make_one.php";
        private const string getReservationPage = "reserv_get_one.php";
        private const string getDayReservationPage = "reserv_get_day.php";

        public static void MakeReservation(ReservationItem reservation) {
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

            Console.WriteLine(dataStr);

            connect(url, dataStr);
        }

        public static List<ReservationItem> GetReservation(DateTime date, int classTime, int classroom) {
            try {
                List<ReservationItem> items = new List<ReservationItem>();
                string url = serverDomain + getReservationPage;

                string dataStr =
                    "date=" + date.ToString("yyyy-MM-dd") +
                    "&class=" + classTime +
                    "&classroom=" + classroom;

                string result = connect(url, dataStr);

                return items;
            } catch (ServerException e) {
                throw e;
            }
        }

        public static List<ReservationItem> GetDayReservation(DateTime date) {
            try {
                List<ReservationItem> items = new List<ReservationItem>();
                string url = serverDomain + getDayReservationPage;

                string dataStr =
                    "date=" + date.ToString("yyyy-MM-dd");

                string result = connect(url, dataStr);

                Console.WriteLine(date.ToString("yyyy-MM-dd") + " - " + result);

                List<dynamic> array = JsonConvert.DeserializeObject<List<dynamic>>(result);

                for (int i = 0; i < array.Count; i++) {
                    DateTime startDate = Convert.ToDateTime(array[i].StartDate);
                    DateTime endDate = Convert.ToDateTime(array[i].EndDate);
                    int startClass = array[i].StartClass;
                    int endClass = array[i].EndClass;
                    string classroom = array[i].Classroom;
                    string userName = array[i].UserName;
                    string contact = array[i].Contact;
                    string content = array[i].Content;
                    string password = array[i].Password;

                    ReservationItem item = new ReservationItem(startDate, endDate, startClass, endClass, classroom, userName, contact, content, password);

                    items.Add(item);
                }

                return items;
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

                //Console.WriteLine(result);

                return result;
            } catch (Exception e) {
                Other.AlertWindow alert = new Other.AlertWindow(e.Message);
                //alert.ShowDialog();

                throw new ServerException();
            }
        }
    }

    class ServerException : Exception {

    }
}
