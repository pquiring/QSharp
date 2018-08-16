using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "std::qt_ptr<$QThread> $q;"
    )]
    public class Thread {
        public Thread() {
            CPP.Add("$q = new $QThread();");
            CPP.Add("$q->bind([=] () {Run();});");
        }
        public static void Sleep(int ms) {
            CPP.Add("QThread::msleep(ms);");
        }
        public void Start() {
            CPP.Add("$q->start();");
        }
        public virtual void Run() {
            Console.WriteLine("Error:Qt.Core.Thread.Run() executed!");
        }
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
