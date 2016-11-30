using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomReservation.Server {
    class ServerClient {
        private const string serverDomain = "http://10.16.10.68/";
        private const string makeReservationPage = "classroom.php";
        private const string getReservationPage = "user_data.php";

        public void MakeReservation(ReservationItem reservation) {
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
                "&passsword=" + reservation.password;

            connect(url, dataStr);
        }

        public List<ReservationItem> GetReservation(DateTime date, int classTime, int classroom) {
            List<ReservationItem> items = new List<ReservationItem>();
            string url = serverDomain + getReservationPage;

            string dataStr =
                "date=" + date.ToString("yyyy-MM-dd") +
                "&class=" + classTime +
                "&classroom=" + classroom;

            string result = connect(url, dataStr);

            return items;
        }

        public string connect(string url, string dataStr) {
            byte[] data = UTF8Encoding.UTF8.GetBytes(dataStr);

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentLength = data.Length;

            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            Stream respPostStream = httpResponse.GetResponseStream();
            StreamReader readerPost = new StreamReader(respPostStream, Encoding.GetEncoding("UTF-8"), true);

            return readerPost.ReadToEnd();
        }
    }
}
