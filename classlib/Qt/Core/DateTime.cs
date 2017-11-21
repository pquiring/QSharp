using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "public: std::shared_ptr<QDateTime> $q;" +
        "public: DateTime() {$q = std::make_shared<QDateTime>();}" +
        "public: DateTime(QDate date, QTime time) {$q = std::make_shared<QDateTime>(date, time);}" +
        "public: DateTime(QDateTime datetime) {$q = std::make_shared<QDateTime>((const QDateTime&)datetime);}"
    )]
    public class DateTime {
        public Date GetDate() {
            return (Date)CPP.ReturnObject("std::make_shared<Date>($q->date())");
        }
        public void SetDate(Date date) {
            CPP.Add("$q->setDate(*date->$q);");
        }
        public Time GetTime() {
            return (Time)CPP.ReturnObject("std::make_shared<Time>($q->time())");
        }
        public void SetTime(Time time) {
            CPP.Add("$q->setTime(*time->$q);");
        }

        public DateTime AddYears(int years) {
            return (DateTime)CPP.ReturnObject("std::make_shared<DateTime>($q->addYears(years))");
        }
        public DateTime AddMonths(int months) {
            return (DateTime)CPP.ReturnObject("std::make_shared<DateTime>($q->addMonths(months))");
        }
        public DateTime AddDays(int days) {
            return (DateTime)CPP.ReturnObject("std::make_shared<DateTime>($q->addDays(days))");
        }
        public DateTime AddSeconds(int seconds) {
            return (DateTime)CPP.ReturnObject("std::make_shared<DateTime>($q->addSecs(seconds))");
        }
        public DateTime AddMilliSeconds(int ms) {
            return (DateTime)CPP.ReturnObject("std::make_shared<DateTime>($q->addMSecs(ms))");
        }

        public String ToString(String format) {
            return CPP.ReturnString("std::make_shared<String>($q->toString(format->qstring()))");
        }

        public static DateTime GetCurrentDateTime() {
            return (DateTime)CPP.ReturnObject("std::make_shared<DateTime>(QDateTime::currentDateTime())");
        }
        public static long GetMiliSecondsSinceEpoch() {
            return CPP.ReturnLong("QDateTime::currentMSecsSinceEpoch()");
        }
    }
}
