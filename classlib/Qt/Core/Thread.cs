using Qt.QSharp;

namespace Qt.Core {
    [CPPExtends("QThread")]
    [CPPClass(
        "public: void run() {Run();}"
    )]
    public class Thread {
        public static void Sleep(int ms) {
            CPP.Add("msleep(ms);");
        }
        public void Start() {
            CPP.Add("start();");
        }
        public virtual void Run() {}
        public void Join() {
            CPP.Add("wait();");
        }
        public bool IsRunning() {
            return CPP.ReturnBool("isRunning();");
        }
 
        //the following are EventLoop related

        public void Exit(int exitCode = 0) {
            CPP.Add("exit(exitCode);");
        }
        public void Exec() {
            CPP.Add("exec();");
        }
        public void Quit() {
            CPP.Add("quit();");
        }

    }
}
