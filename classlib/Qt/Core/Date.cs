using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "std::qt_ptr<QDate> $q;" +
        "QDate* $date() {return $q.get();}"
    )]
    public class Date {
        public Date() {
            CPP.Add("$q = new QDate();");
        }
        [CPPReplaceArgs("const QDate date")]
        private Date(NativeArg1 arg1) {
            CPP.Add("$q = new QDate(date);");
        }
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
            return CPP.ReturnString("new Qt::Core::String($q->toString($check(format)->qstring()))");
        }

        public static Date GetCurrentDate() {
            return (Date)CPP.ReturnObject("new Date(QDate::currentDate())");
        }
    }
}
