using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "std::shared_ptr<$QThread> $q;"
    )]
    public class Thread {
        public Thread() {
            CPP.Add("$q = std::make_shared<$QThread>();");
            CPP.Add("$q->bind([=] () {Run();}, $this);");
        }
        public static void Sleep(int ms) {
            CPP.Add("QThread::msleep(ms);");
        }
        public void Start() {
            CPP.Add("$q->start();");
        }
        public virtual void Run() {}
        public void Join() {
            CPP.Add("$q->wait();");
        }
        public bool IsRunning() {
            return CPP.ReturnBool("$q->isRunning()");
        }

        //the following are EventLoop related

        public void Exit(int exitCode = 0) {
            CPP.Add("$q->exit(exitCode);");
        }
        /** Processes Events until Exit() or Quit() is called. */
        public void Exec() {
            CPP.Add("$q->exec();");
        }
        public void Quit() {
            CPP.Add("$q->quit();");
        }

    }
}
