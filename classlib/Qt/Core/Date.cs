using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: std::unique_ptr<QDate> $q;" +
        "public: QDate* $date() {return $q.get();}"
    )]
    public class Date {
        public Date() {
            CPP.Add("$q = std::make_unique<QDate>();");
        }
        [CPPReplaceArgs("const QDate date")]
        private Date(NativeArg1 arg1) {
            CPP.Add("$q = std::make_unique<QDate>(date);");
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
            return CPP.ReturnString("String::$new($q->toString($check(format)->qstring()))");
        }

        public static Date GetCurrentDate() {
            return (Date)CPP.ReturnObject("Date::$new(QDate::currentDate())");
        }
    }
}
