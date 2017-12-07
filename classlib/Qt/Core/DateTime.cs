using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "public: std::shared_ptr<QDateTime> $q;"
    )]
    public class DateTime {
        public DateTime() {
            CPP.Add("$q = std::make_shared<QDateTime>();");
        }
        [CPPReplaceArgs("const QDateTime dt")]
        private DateTime(NativeArg1 arg) {
            CPP.Add("$q = std::make_shared<QDateTime>(dt);");
        }
        [CPPReplaceArgs("QDate date,QTime time")]
        private DateTime(NativeArg2 arg) {
            CPP.Add("$q = std::make_shared<QDateTime>(date,time);");
        }
        public Date GetDate() {
            return (Date)CPP.ReturnObject("Date::$new($q->date())");
        }
        public void SetDate(Date date) {
            CPP.Add("$q->setDate(*date->$q);");
        }
        public Time GetTime() {
            return (Time)CPP.ReturnObject("Time::$new($q->time())");
        }
        public void SetTime(Time time) {
            CPP.Add("$q->setTime(*time->$q);");
        }

        public DateTime AddYears(int years) {
            return (DateTime)CPP.ReturnObject("DateTime::$new($q->addYears(years))");
        }
        public DateTime AddMonths(int months) {
            return (DateTime)CPP.ReturnObject("DateTime::$new($q->addMonths(months))");
        }
        public DateTime AddDays(int days) {
            return (DateTime)CPP.ReturnObject("DateTime::$new($q->addDays(days))");
        }
        public DateTime AddSeconds(int seconds) {
            return (DateTime)CPP.ReturnObject("DateTime::$new($q->addSecs(seconds))");
        }
        public DateTime AddMilliSeconds(int ms) {
            return (DateTime)CPP.ReturnObject("DateTime::$new($q->addMSecs(ms))");
        }

        public String ToString(String format) {
            return CPP.ReturnString("String::$new($q->toString(format->qstring()))");
        }

        public static DateTime GetCurrentDateTime() {
            return (DateTime)CPP.ReturnObject("DateTime::$new(QDateTime::currentDateTime())");
        }
        public static long GetMiliSecondsSinceEpoch() {
            return CPP.ReturnLong("QDateTime::currentMSecsSinceEpoch()");
        }
    }
}
