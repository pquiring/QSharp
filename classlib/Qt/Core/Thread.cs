using Qt.QSharp;

namespace Qt.Core {
    [CPPExtends("QThread")]
    [CPPClass(
        "public: void run() {Run();}\r\n"
    )]
    public class Thread {
        public static void Sleep(int ms) {
            CPP.Add("msleep(ms);\r\n");
        }
        public void Start() {
            CPP.Add("start();\r\n");
        }
        public virtual void Run() {}
        public void Join() {
            CPP.Add("wait();\r\n");
        }
        public bool IsRunning() {
            return CPP.ReturnBool("isRunning();\r\n");
        }
 
        //the following are EventLoop related

        public void Exit(int exitCode = 0) {
            CPP.Add("exit(exitCode);\r\n");
        }
        public void Exec() {
            CPP.Add("exec();\r\n");
        }
        public void Quit() {
            CPP.Add("quit();\r\n");
        }

    }
}
