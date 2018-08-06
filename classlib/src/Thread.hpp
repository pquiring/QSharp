namespace Qt { namespace Core {

typedef std::function<void()> $thread_run;

struct Thread;

struct $QThread : public QThread {
  std::gc_ptr<Qt::Core::Thread> $thread;  //created in calling thread
  std::gc_ptr<Qt::Core::Thread> $self;  //created in this thread (to keep ref to running thread)
  $thread_run $run_ptr;
  void bind($thread_run run_ptr, std::gc_ptr<Qt::Core::Thread> thread) {
    $run_ptr = run_ptr;
    $thread = thread;
  }
  void run() {
    $self = $thread;
#ifdef QSHARP_GC
    std::heap = $thread->heap;
#endif
    $run_ptr();
    $self = nullptr;
    $thread = nullptr;
  }
  void exec() {QThread::exec();}  //QThread::exec() is protected
};

} }  //namespace Qt::Core
