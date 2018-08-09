using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "std::qt_ptr<QTime> $q;" +
        "QTime* $time() {return $q.get();}"
    )]
    public class Time {
        public Time() {
            CPP.Add("$q = new QTime();");
        }
        [CPPReplaceArgs("QTime time")]
        private Time(NativeArg1 arg) {
            CPP.Add("$q = new QTime(time);");
        }
        public int GetHour() {
            return CPP.ReturnInt("$q->hour()");
        }
        public int GetMinute() {
            return CPP.ReturnInt("$q->minute()");
        }
        public int GetSecond() {
            return CPP.ReturnInt("$q->second()");
        }
        public int GetMilliSecond() {
            return CPP.ReturnInt("$q->msec()");
        }
        public void SetTime(int hour, int minute, int second, int milli = 0) {
            CPP.Add("$q->setHMS(hour, minute, second, milli);");
        }
        public String ToString(String format) {
            return CPP.ReturnString("new Qt::Core::String($q->toString($check(format)->qstring()))");
        }

        public static Time GetCurrentTime() {
            return (Time)CPP.ReturnObject("new Time(QTime::currentTime())");
        }

    }
}
