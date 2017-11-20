using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "public: std::shared_ptr<QTime> $q;" +
        "public: Time() {$q = std::make_shared<QTime>();}" +
        "public: Time(QTime time) {$q = std::make_shared<QTime>(time);}"
    )]
    public class Time {
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
            return CPP.ReturnString("std::make_shared<String>($q->toString(format->qstring()))");
        }

        public static Time GetCurrentTime() {
            return (Time)CPP.ReturnObject("std::make_shared<Time>(QTime::currentTime())");
        }

    }
}
