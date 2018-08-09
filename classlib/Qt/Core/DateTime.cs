using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "std::qt_ptr<QDateTime> $q;"
    )]
    public class DateTime {
        public DateTime() {
            CPP.Add("$q = new QDateTime();");
        }
        [CPPReplaceArgs("const QDateTime dt")]
        private DateTime(NativeArg1 arg) {
            CPP.Add("$q = new QDateTime(dt);");
        }
        [CPPReplaceArgs("QDate date,QTime time")]
        private DateTime(NativeArg2 arg) {
            CPP.Add("$q = new QDateTime(date,time);");
        }
        public Date GetDate() {
            return (Date)CPP.ReturnObject("new Date($q->date())");
        }
        public void SetDate(Date date) {
            CPP.Add("$q->setDate(*$check(date)->$date());");
        }
        public Time GetTime() {
            return (Time)CPP.ReturnObject("new Time($q->time())");
        }
        public void SetTime(Time time) {
            CPP.Add("$q->setTime(*$check(time)->$time());");
        }

        public DateTime AddYears(int years) {
            return (DateTime)CPP.ReturnObject("new DateTime($q->addYears(years))");
        }
        public DateTime AddMonths(int months) {
            return (DateTime)CPP.ReturnObject("new DateTime($q->addMonths(months))");
        }
        public DateTime AddDays(long days) {
            return (DateTime)CPP.ReturnObject("new DateTime($q->addDays(days))");
        }
        public DateTime AddSeconds(long seconds) {
            return (DateTime)CPP.ReturnObject("new DateTime($q->addSecs(seconds))");
        }
        public DateTime AddMilliSeconds(long ms) {
            return (DateTime)CPP.ReturnObject("new DateTime($q->addMSecs(ms))");
        }

        public String ToString(String format) {
            return CPP.ReturnString("new Qt::Core::String($q->toString($check(format)->qstring()))");
        }

        public static DateTime GetCurrentDateTime() {
            return (DateTime)CPP.ReturnObject("new DateTime(QDateTime::currentDateTime())");
        }
        public static long GetMilliSecondsSinceEpoch() {
            return CPP.ReturnLong("QDateTime::currentMSecsSinceEpoch()");
        }
    }
}
