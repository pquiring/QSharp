//include Qt Headers

#undef int64
#define int64 _w_int64

#include <QtCore/qmath.h>
#include <QtCore/QVector>
#include <QtCore/QList>
#include <QtCore/QMap>
#include <QtCore/QCoreApplication>
#include <QtCore/QFile>
#include <QtCore/QDir>
#include <QtCore/QMutex>
#include <QtCore/QWaitCondition>
#include <QtCore/QTimer>
#include <QtCore/QThread>
#include <QtCore/QDate>
#include <QtCore/QTime>
#include <QtCore/QDateTime>
#include <QtCore/QUrl>
#include <QtCore/QByteArray>
#include <QtCore/QBuffer>
#include <QtCore/QLibrary>
#include <QtCore/QCryptographicHash>
#include <QtCore/QProcess>

#include <QtGui/QGuiApplication>
#include <QtGui/QOpenGLWindow>
#include <QtGui/QOpenGLContext>
#include <QtGui/QOpenGLFunctions>
#include <QtGui/QKeyEvent>
#include <QtGui/QMouseEvent>
#include <QtGui/QMatrix4x4>
#include <QtGui/QVector2D>
#include <QtGui/QVector3D>
#include <QtGui/QVector4D>
#include <QtGui/QOpenGLShaderProgram>
#include <QtGui/QScreen>
#include <QtGui/QImage>
#include <QtGui/QPixmap>
#include <QtGui/QIcon>
#include <QtGui/QPainter>
#include <QtGui/QFont>

#include <QtWidgets/QApplication>
#include <QtWidgets/QWidget>
#include <QtWidgets/QMainWindow>
#include <QtWidgets/QAbstractButton>
#include <QtWidgets/QPushButton>
#include <QtWidgets/QCheckBox>
#include <QtWidgets/QRadioButton>
#include <QtWidgets/QAbstractSlider>
#include <QtWidgets/QDial>
#include <QtWidgets/QSlider>
#include <QtWidgets/QScrollBar>
#include <QtWidgets/QFrame>
#include <QtWidgets/QLabel>
#include <QtWidgets/QGroupBox>
#include <QtWidgets/QComboBox>
#include <QtWidgets/QDialog>
#include <QtWidgets/QFileDialog>
#include <QtWidgets/QColorDialog>
#include <QtWidgets/QFontDialog>
#include <QtWidgets/QProgressBar>
#include <QtWidgets/QListWidget>
#include <QtWidgets/QHeaderView>
#include <QtWidgets/QTableWidget>
#include <QtWidgets/QTreeWidget>
#include <QtWidgets/QSplitter>
#include <QtWidgets/QAction>
#include <QtWidgets/QMenu>
#include <QtWidgets/QMenuBar>
#include <QtWidgets/QSystemTrayIcon>
#include <QtWidgets/QLineEdit>
#include <QtWidgets/QTextEdit>
#include <QtWidgets/QToolBar>
#include <QtWidgets/QMessageBox>
#include <QtWidgets/QOpenGLWidget>
#include <QtWidgets/QSpinBox>

#include <QtWidgets/QLayout>
#include <QtWidgets/QBoxLayout>
#include <QtWidgets/QHBoxLayout>
#include <QtWidgets/QVBoxLayout>
#include <QtWidgets/QFormLayout>
#include <QtWidgets/QGridLayout>
#include <QtWidgets/QStackedLayout>

#include <QtNetwork/QTcpSocket>
#include <QtNetwork/QUdpSocket>
#include <QtNetwork/QTcpServer>
#include <QtNetwork/QSslSocket>
#include <QtNetwork/QNetworkAccessManager>
#include <QtNetwork/QNetworkRequest>
#include <QtNetwork/QNetworkReply>

#include <QtWebSockets/QWebSocket>
#include <QtWebSockets/QWebSocketServer>

#include <QtXml/QDomNode>
#include <QtXml/QDomElement>
#include <QtXml/QDomDocument>
#include <QtXml/QDomAttr>

#include <QtMultimedia/QAudioDecoder>
#include <QtMultimedia/QAudioFormat>
#include <QtMultimedia/QAudioBuffer>
#include <QtMultimedia/QAudioInput>
#include <QtMultimedia/QAudioOutput>
#include <QtMultimedia/QAudioDeviceInfo>

//include Quazip Headers
#include <quazip/quazip.h>
#include <quazip/quazipfile.h>
#include <quazip/quaziodevice.h>

//include AES
#include <AES.hpp>

#undef int64
#define int64 signed long long

namespace Qt { namespace Core {
  extern int g_argc;
  extern const char **g_argv;
  struct Object;
  struct String;
  struct Thread;
  struct $QThread;
}}

namespace Qt { namespace Gui {
  struct $EventFilter;
  struct $QOpenGLWidget;
  struct $QOpenGLWindow;
}}

namespace Qt { namespace Network {
  struct $QSslServer;
  struct $QWebReply;
}}

namespace Qt { namespace QSharp {
  template<typename T>
  struct Property {
    T $value;
    std::function<T(void)> _get;
    std::function<void(T)> _set;
    void Init(std::function<T()> get, std::function<void(T)> set) {
      _get = get;
      _set = set;
    }
    Property<T>& operator=(T t) {_set(t); return *this;}
    operator T() {return _get();}
    bool operator==(T t) {return _get() == t;}
    bool operator!=(T t) {return _get() != t;}
    T operator->() {return _get();}
    void Get(std::function<T()> get) {
      _get = get;
    }
    void Set(std::function<void(T)> set) {
      _set = set;
    }
  };
}}

extern void $npe();  //NullPointerException
extern void $abe();  //ArrayBoundsException
extern void $abe(int idx, int size);  //ArrayBoundsException

template<typename T>
T* $check(T* in) {
  if (in == nullptr) $npe();
  return in;
}

template<typename T>
T* $check(T* in, int idx, int length) {
  if (in == nullptr) $npe();
  return in;
}

namespace std {
  //qt_ptr is a relaxed version of std::unique_ptr
  template<typename T>
  struct qt_ptr {
    T* t;
    qt_ptr() {t = nullptr;}
    qt_ptr(T *t) {this->t = t;}
    ~qt_ptr() {if (t != nullptr) delete t;}
    T* get() const {return t;}
    qt_ptr& operator=(T *t) {this->t = t; return *this;}
    T* operator->() const {return t;}
    T& operator*() const {return *t;}
    bool operator==(const qt_ptr<T> &other) const { return t == other.t; }
    bool operator!=(const qt_ptr<T> &other) const { return t != other.t; }
    bool operator==(nullptr_t np) const { return t == nullptr; }
    bool operator!=(nullptr_t np) const { return t != nullptr; }
  };
}

namespace Qt { namespace QSharp {
  template<typename T>
  struct FixedData {
    T *t;
    int length;
    bool alloced;
    FixedData(int size) {
      t = new T[size];
      if (sizeof(T) <= 8) {
        //clear primative data types only
        std::memset(t, 0, size * sizeof(T));
      }
      length = size;
      alloced = true;
    }
    FixedData(T *buf, int size) {
      t = buf;
      length = size;
      alloced = false;	
    }
    FixedData(T *buf, int size, bool copy) {
      if (copy) {
        t = new T[size];
        std::memcpy(t, buf, size * sizeof(T));
        length = size;
        alloced = true;
      } else {
        t = buf;
        length = size;
        alloced = false;	
      }
    }
    FixedData(std::initializer_list<T> list) {
      length = (int)list.size();
      t = new T[length];
      alloced = true;
      const T *src = list.begin();
      for(int a=0;a<length;a++) {
        t[a] = src[a];
      }
    }
    T& operator[](int idx) {
      return t[idx];
    }
    ~FixedData() {
      if (alloced) {
        delete[] t;
        t = nullptr;
        alloced = false;
      }
    }
  };
}}

//reflection data
struct $field {
  $field(const char *name) {
    this->name = name;
  }
  const char* name;
};

struct $method {
  $method(const char *name) {
    this->name = name;
  }
  const char *name;
};

struct $class {
  $class(bool isInterface, const char*name, $class *base, std::initializer_list<$class*> ifaceList, std::initializer_list<$field*> fieldList, std::initializer_list<$method*> methodList, std::function<Qt::Core::Object*()> newInstance) {
    this->iface = isInterface;
    this->name = name;
    this->base = base;
    this->ifaces = new QVector<$class*>(ifaceList);
    this->fields = new QVector<$field*>(fieldList);
    this->methods = new QVector<$method*>(methodList);
    this->newInstance = newInstance;
  }
  bool isDerivedFrom($class *cls) {
    if (std::strcmp(name, cls->name) == 0) return true;
    if (base != nullptr) {
      return (base->isDerivedFrom(cls));
    }
    return false;
  }
  bool iface;
  const char *name;
  $class *base;
  QVector<$class*> *ifaces;
  QVector<$field*> *fields;
  QVector<$method*> *methods;
  std::function<Qt::Core::Object*()> newInstance;
};

struct $QMutexHolder {
  $QMutexHolder(std::qt_ptr<std::unique_lock<std::mutex>> lock) {this->lock = lock; lock->lock();}
  std::qt_ptr<std::unique_lock<std::mutex>> lock;
  bool signal = true;
  bool Condition() {return signal;}
  void Signal() {signal = false;}
  ~$QMutexHolder() {lock->unlock();}
};

inline int $hash(Qt::Core::Object *obj) {
  union {Qt::Core::Object* ptr; int hash;} u;
  u.ptr = obj;
  return u.hash;
}

//$mod
template<typename T1, typename T2>
inline int $modi(T1 x, T2 y) {return x % y;}

template<typename T1, typename T2>
inline int64 $modl(T1 x, T2 y) {return x % y;}

inline float $modf(float x, float y) {return std::fmod(x, y);}

inline double $modd(double x, double y) {return std::fmod(x, y);}

//$add
extern Qt::Core::String* $addstr(Qt::Core::String* s1, Qt::Core::String* s2);

extern Qt::Core::String* $addstr(Qt::Core::String* s1, Qt::Core::Object* y);
extern Qt::Core::String* $addstr(Qt::Core::String* s1, char16 y);
extern Qt::Core::String* $addstr(Qt::Core::String* s1, int32 y);
extern Qt::Core::String* $addstr(Qt::Core::String* s1, int64 y);
extern Qt::Core::String* $addstr(Qt::Core::String* s1, uint32 y);
extern Qt::Core::String* $addstr(Qt::Core::String* s1, uint64 y);
extern Qt::Core::String* $addstr(Qt::Core::String* s1, float y);
extern Qt::Core::String* $addstr(Qt::Core::String* s1, double y);

extern Qt::Core::String* $addstr(Qt::Core::Object* x, Qt::Core::String* s2);
extern Qt::Core::String* $addstr(int32 x, Qt::Core::String* s2);
extern Qt::Core::String* $addstr(int64 x, Qt::Core::String* s2);
extern Qt::Core::String* $addstr(uint32 x, Qt::Core::String* s2);
extern Qt::Core::String* $addstr(uint64 x, Qt::Core::String* s2);
extern Qt::Core::String* $addstr(float x, Qt::Core::String* s2);
extern Qt::Core::String* $addstr(double x, Qt::Core::String* s2);

template<typename T1, typename T2>
T1 $addnum(T1 x,T2 y) {return x + y;}

template<typename T1, typename T2>
T1 $addnum(Qt::QSharp::Property<T1> x,T2 y) {return x.$value + y;}

template<typename T1, typename T2>
T1 $addnum(T1 x,Qt::QSharp::Property<T2> y) {return x + y.$value;}

template<typename T1, typename T2>
T1 $addnum(Qt::QSharp::Property<T1> x,Qt::QSharp::Property<T2> y) {return x.$value + y.$value;}

namespace std {

struct MemoryPool {
  MemoryPool();
  ~MemoryPool();
  MemoryPool *org = nullptr;
  Qt::Core::Object *head = nullptr;
  Qt::Core::Object *tail = nullptr;
};

extern thread_local MemoryPool *pool;

void attach_object(Qt::Core::Object *);

void detach_object(Qt::Core::Object *);

template <class T>
class Vector {
  private:
    std::vector<T> data;
  public:
    Vector();
    Vector(int size);
    Vector(const Vector & other);  //copy ctor
    Vector(std::initializer_list<T> list);
    ~Vector();
    T get(int idx);
    void set(int idx, T value);
    void add(T value);
    void add(int idx, T value);
    void add(std::initializer_list<T> list);
    T removeAt(int idx);
    void remove(T value);
    void clear();
    int size();
    boolean isEmpty();
    boolean contains(T value);
    int indexOf(T value);
    int lastIndexOf(T value);
    T& operator[] (int idx);
    T* get();  //returns backing array
};

template <class T>
Vector<T>::Vector() {
}

template <class T>
Vector<T>::Vector(int size) : data(size) {
}

template <class T>
Vector<T>::Vector(const Vector &copy) : data(copy) {
}

template <class T>
Vector<T>::Vector(std::initializer_list<T> list) : data(list) {
}

template <class T>
Vector<T>::~Vector() {
}

template <class T>
T Vector<T>::get(int idx) {
  if (idx < 0 || idx >= data.size()) $abe(idx, (int)data.size());
  return data[idx];
}

template <class T>
void Vector<T>::set(int idx, T value) {
  if (idx < 0 || idx >= data.size()) return;
  data[idx] = value;
}

template <class T>
void Vector<T>::add(T value) {
  data.push_back(value);
}

template <class T>
void Vector<T>::add(int idx, T value) {
  data.insert(data.begin() + idx, value);
}

template <class T>
T Vector<T>::removeAt(int idx) {
  T ret = data[idx];
  data.remove(data.begin() + idx);
  return ret;
}

template <class T>
void Vector<T>::remove(T value) {
  int idx = indexOf(value);
  if (idx == -1) return;
  removeAt(idx);
}

template <class T>
void Vector<T>::clear() {
  data.clear();
}

template <class T>
int Vector<T>::size() {
  return (int)data.size();
}

template <class T>
boolean Vector<T>::isEmpty() {
  return data.size() == 0;
}

template <class T>
boolean Vector<T>::contains(T value) {
  T *t = data.data();
  int size = data.size();
  for(int idx=0;idx<size;idx++) {
    if (t[idx] == value) return true;
  }
  return false;
}

template <class T>
int Vector<T>::indexOf(T value) {
  T *t = data.data();
  int size = data.size();
  for(int idx=0;idx<size;idx++) {
    if (t[idx] == value) return idx;
  }
  return -1;
}

template <class T>
int Vector<T>::lastIndexOf(T value) {
  T *t = data.data();
  int size = data.size();
  for(int idx=size-1;idx>=0;idx--) {
    if (t[idx] == value) return idx;
  }
  return -1;
}

template <class T>
T& Vector<T>::operator[] (int idx) {
  if (idx < 0 || idx >= data.size()) $abe(idx, (int)data.size());
  return data[idx];
}

template <class T>
T* Vector<T>::get() {
  return data.data();
}

}
