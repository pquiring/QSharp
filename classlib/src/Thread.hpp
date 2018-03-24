namespace Qt { namespace Core {

typedef std::function<void()> $thread_run;

struct Thread;

struct $ThreadReference : QObject {
  std::shared_ptr<Thread> ref;
};

struct $QThread : public QThread {
  $ThreadReference *ref;
  $thread_run $run_ptr;
  void bind($thread_run run_ptr, std::shared_ptr<Qt::Core::Thread> thread) {
    ref = new $ThreadReference();
    ref->ref = thread;
    QObject::connect(this, &QThread::finished, ref, &QObject::deleteLater);
    $run_ptr = run_ptr;
  }
  void run() {$run_ptr();}
  void exec() {QThread::exec();}  //QThread::exec() is protected
};

} }  //namespace Qt::Core
