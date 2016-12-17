using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomReservation.Server {
    class LectureItem {
        public int year { get; private set; }
        public int semester { get; private set; }
        public string dayOfWeek { get; private set; }
        public string classtime { get; private set; }
        public string classroom { get; private set; }
        public string professor { get; private set; }
        public string contact { get; private set; }
        public string code { get; private set; }
        public string name { get; private set; }

        public LectureItem(int year, int semester, string dayOfWeek, string classtime, string classroom, string professor, string contact, string code, string name) {
            this.year = year;
            this.semester = semester;
            this.dayOfWeek = dayOfWeek;
            this.classtime = classtime;
            this.classroom = classroom;
            this.professor = professor;
            this.contact = contact;
            this.code = code;
            this.name = name;
        }
    }
}
