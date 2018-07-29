#define SLOT_FREE ((Qt::Core::Object*)-1)

#define MARK_MAX (2147483647)  //2^31-1
#define MARK_NEW 0

namespace std {

  bool gc_active = true;

  void gc_run() {
    while (gc_active) {
      Qt::Core::Thread::Sleep(5000);
      std::gc();
    }
  }

  thread_local Heap<Qt::Core::Object> *heap;  //thread
  Heap<Qt::Core::Object> *global_heap;  //static/main
  Heap<Qt::Core::Thread> *threads;
  int mark = 1;  //gc mark

  std::thread *gc_thread;

  void gc_init() {
    global_heap = heap;
    threads = new std::Heap<Qt::Core::Thread>();
    gc_thread = new std::thread(&gc_run);
  }

  static std::mutex *gc_mutex;

  void gc_lock() {
    if (gc_mutex == nullptr) {
      gc_mutex = new std::mutex();
    }
    gc_mutex->lock();
  }

  void gc_unlock() {
    gc_mutex->unlock();
  }

  void gc_add_object(Qt::Core::Object *obj) {
    if (heap == nullptr) {
      heap = new Heap<Qt::Core::Object>();
    }
    heap->add(obj);
  }

  void gc_add_thread(Qt::Core::Thread *thread) {
    if (threads == nullptr) {
      threads = new Heap<Qt::Core::Thread>();
    }
    threads->add(thread);
  }

  void gc() {
    printf("std::gc()\n");
    gc_lock();
    if (mark == MARK_MAX)
      mark = 1;
    else
      mark++;
    //stage one : mark
    {
      Qt::Core::Thread *thread = threads->head;
      while (thread != nullptr) {
  //      thread->printHeap();
        int size = thread->heap.size;
        Qt::Core::Object **refs = thread->heap.refs;
        for(int a=0;a<size;a++) {
          Qt::Core::Object *obj = refs[a];
          if (obj == nullptr) continue;
          if (obj == SLOT_FREE) continue;
          obj->mark = mark;
        }
        thread = thread->next;
      }
    }
    //stage two : sweep
    {
      Qt::Core::Thread *thread = threads->head;
      while (thread != nullptr) {
        Qt::Core::Object *prev = nullptr;
        Qt::Core::Object *obj = thread->heap.head;
        while (obj != nullptr) {
          Qt::Core::Object *next = obj->next;
          if ((obj->mark != MARK_NEW) && (obj->mark != mark)) {
            printf("delete obj:%p\n", obj);
            Qt::Core::Thread *delthread = dynamic_cast<Qt::Core::Thread*>(obj);
            if (delthread != nullptr) {
              //this is a thread - take special steps
              delthread->deleteme = true;
            } else {
              thread->heap.remove(obj, prev);
              delete obj;
            }
          } else {
            prev = obj;
          }
          obj = next;
        }
        thread = thread->next;
      }
    }
    //delete threads flagged for deletion
    {
      Qt::Core::Thread *thread = threads->head;
      Qt::Core::Thread *prev = nullptr;
      while (thread != nullptr) {
        if (thread->deleteme) {
          //copy it's Heap to the global heap
          global_heap->add(thread->heap.head->next);  //do NOT add Thread which will be 1st Object on it's Heap
          //remove from thread list
          prev->next = thread->next;
          //finally delete it
          delete thread;
          thread = prev->next;
        } else {
          thread = thread->next;
        }
      }
    }
    gc_unlock();
  }

}
