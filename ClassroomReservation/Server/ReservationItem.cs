using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomReservation.Server {
    class ReservationItem {
        public DateTime startDate;
        public DateTime endDate;
        public int startClass;
        public int endClass;
        public string classroom;
        public string userName;
        public string contact;
        public string content;
        public string password;

        public ReservationItem(DateTime startDate, DateTime endDate, int startClass, int endClass, string classroom, string userName, string contact, string content, string password) {
            this.startDate = startDate;
            this.endDate = endDate;
            this.startClass = startClass;
            this.endClass = endClass;
            this.classroom = classroom;
            this.userName = userName;
            this.contact = contact;
            this.content = content;
            this.password = password;
        }
    }
}
