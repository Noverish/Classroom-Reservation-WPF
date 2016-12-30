using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomReservation.Server {
    class DummyServerClient {
        public static void encode(string str) {
            try {
                string url = "http://noverish.me/api/cs_reservation/password_add.php";

                byte[] data = UTF8Encoding.UTF8.GetBytes("password=" + str);

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = data.Length;
                httpWebRequest.Timeout = 5000;

                Stream requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                Stream respPostStream = httpResponse.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, Encoding.GetEncoding("UTF-8"), true);

                string resultStr = readerPost.ReadToEnd();
            } catch (Exception e) {

            }
        }
    }
}
