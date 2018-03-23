namespace Qt { namespace Core {

typedef std::function<void()> $thread_run;

struct $QThread : public QThread {
  $thread_run $run_ptr;
  void bind($thread_run run_ptr) {
    $run_ptr = run_ptr;
  }
  void run() {$run_ptr();}
  void exec() {QThread::exec();}  //QThread::exec() is protected
};

} }  //namespace Qt::Core
