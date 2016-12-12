using ClassroomReservation.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomReservation.Resource {
    class Database {
        private static Database instance;
        public static Database getInstance() {
            if (instance == null)
                instance = new Database();
            return instance;
        }

        private const string CLASS_TIME_PATH = @"..\..\Resource\ClassTime.txt";
        private const string CLASSROOM_PATH = @"..\..\Resource\Classroom.txt";

        public Hashtable classTimeTable { get; private set; }
        public Hashtable classroomTable { get; private set; }
        
        private string[] defaultClassTime = new string[]{
                    "09:00 AM ~ 10:15 AM",
                    "10:15 AM ~ 11:45 AM",
                    "12:00 PM ~ 12:50 PM",
                    "01:00 PM ~ 01:50 PM",
                    "02:00 PM ~ 03:15 PM",
                    "03:30 PM ~ 04:45 PM",
                    "05:00 PM ~ 05:50 PM",
                    "06:00 PM ~ 06:50 PM",
                    "07:00 PM ~ 07:50 PM",
                    "08:00 PM ~ 08:50 PM"
                };
        private string[] defaultClassroom = new string[]{
                    "이학별관:107호",
                    "정보관:B102호",
                    "정보관:B103호",
                    "정보관:B104호",
                    "정보관:201호",
                    "정보관:202호",
                    "정보관:205호",
                    "정보관:206호",
                    "정보관:208호",
                    "과도관:611호",
                    "과도관:614A호",
                    "과도관:615호"
                };

        private Database() {
            readClassTime();
            readClassroom();
        }

        private void readClassTime() {
            string[] lines;

            try {
                lines = File.ReadAllLines(CLASS_TIME_PATH);
            } catch (Exception ex) {
                lines = defaultClassTime;
                File.WriteAllLines(CLASS_TIME_PATH, lines);
            }

            classTimeTable = new Hashtable();

            for (int i = 0; i < lines.Length; i++) {
                classTimeTable.Add(i, lines[i]);
            }
        }

        private void readClassroom() {
            string[] lines = ServerClient.GetClassroomList();
            
            classroomTable = new Hashtable();

            for (int i = 0; i < lines.Length; i++) {
                string[] tmp = lines[i].Split(':');

                if (tmp.Length == 2) {
                    classroomTable.Add(i, new ClassroomItem(tmp[0], tmp[1]));
                } else {
                    Console.WriteLine("ERROR : " + lines[i]);
                }
            }
        }

        public int GetRowByClassroom(string fullName) {
            if (classroomTable != null) {
                for (int i = 0; i < classroomTable.Count; i++) {
                    string buildingName = (classroomTable[i] as ClassroomItem).building;
                    string classroomName= (classroomTable[i] as ClassroomItem).classroom;

                    if (fullName.Equals(buildingName + ":" + classroomName))
                        return i;
                }
                return -1;
            } else {
                return -2;
            }
        }
    }

    public class ClassroomItem {
        public string building { get; private set; }
        public string classroom { get; private set; }

        public ClassroomItem(string building, string classroom) {
            this.building = building;
            this.classroom = classroom;
        }
    }
}
