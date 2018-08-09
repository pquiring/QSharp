#ifndef QSHARP_GC

namespace std {

inline thread_local MemoryPool *pool = nullptr;

MemoryPool::MemoryPool() {
  org = pool;
  pool = this;
}

MemoryPool::~MemoryPool() {
  //delete objects in pool
  Qt::Core::Object *obj = pool->head;
  while (obj != nullptr) {
    Qt::Core::Object *next = obj->next;
    delete obj;
    obj = next;
  }
  pool = org;
}

void attach_object(Qt::Core::Object *obj) {
  if (pool == nullptr) return;
  if (pool->head == nullptr) {
    pool->head = obj;
    pool->tail = obj;
  } else {
    pool->tail->next = obj;
    obj->prev = pool->tail;
    pool->tail = obj;
  }
}

void detach_object(Qt::Core::Object *obj) {
  if (pool == nullptr) return;
  if (obj->prev != nullptr) {
    obj->prev->next = obj->next;
  }
  if (obj->next != nullptr) {
    obj->next->prev = obj->prev;
  }
  obj->prev = nullptr;
  obj->next = nullptr;
}

}

#endif
