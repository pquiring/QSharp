namespace Qt { namespace Core {

typedef std::function<void()> $thread_run;

class $QThread : public QThread {
  private: $thread_run $run_ptr;
  public: void bind($thread_run run_ptr) {
    $run_ptr = run_ptr;
  }
  public: void run() {$run_ptr();}
  public: void exec() {QThread::exec();}  //QThread::exec() is protected
};

} }  //namespace Qt::Core
