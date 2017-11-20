using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "public: std::shared_ptr<QDate> $q;" +
        "public: Date() {$q = std::make_shared<QDate>();}" +
        "public: Date(QDate Date) {$q = std::make_shared<QDate>(Date);}"
    )]
    public class Date {
        public int GetDayOfMonth() {
            return CPP.ReturnInt("$q->day()");
        }
        public int GetDayOfWeek() {
            return CPP.ReturnInt("$q->dayOfWeek()");
        }
        public int GetDayOfYear() {
            return CPP.ReturnInt("$q->dayOfYear()");
        }
        public int GetDaysInMonth() {
            return CPP.ReturnInt("$q->daysInMonth()");
        }
        public int GetDaysInYear() {
            return CPP.ReturnInt("$q->daysInYear()");
        }
        public int GetMonth() {
            return CPP.ReturnInt("$q->month()");
        }
        public int GetYear() {
            return CPP.ReturnInt("$q->year()");
        }
        public void SetDate(int year, int month, int day) {
            CPP.Add("$q->setDate(year, month, day);");
        }
        public String ToString(String format) {
            return CPP.ReturnString("std::make_shared<String>($q->toString(format->qstring()))");
        }

        public static Date GetCurrentDate() {
            return (Date)CPP.ReturnObject("std::make_shared<Date>(QDate::currentDate())");
        }
    }
}
