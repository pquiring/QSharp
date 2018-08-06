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
    }  };
}}

extern void $npe();  //NullPointerException
extern void $abe();  //ArrayBoundsException

namespace std {

  template<typename T>
  struct gc_ptr;

  //gc_ptr is a simple smart pointer
  template<typename T>
  struct gc_ptr {
    T *ptr = nullptr;

    gc_ptr() {
    }
    gc_ptr(T *t) {
      ptr = t;
    }
    gc_ptr(const gc_ptr &other) {
      ptr = other.ptr;
    }
    gc_ptr(gc_ptr &&other) {
      ptr = other.ptr;
      other.ptr = nullptr;
    }
    template<typename O>
    gc_ptr(const gc_ptr<O> &other) {
      ptr = other.ptr;
    }

    ~gc_ptr() {
    }

    gc_ptr& operator=(const gc_ptr &other) { ptr = other.ptr; return *this; }
    gc_ptr& operator=(gc_ptr &&other) { ptr = other.ptr; other.ptr = nullptr; return *this; }
    template<typename O>
    gc_ptr& operator=(gc_ptr<O> &other) { ptr = other.ptr; other.ptr = nullptr; return *this; }
    gc_ptr& operator=(T *t) { ptr = t; return *this; }

    bool operator==(gc_ptr &other) const { return ptr == other.ptr; }
    bool operator==(const gc_ptr &other) const { return ptr == other.ptr; }
    bool operator==(T *other) const { return ptr == other; }
    bool operator==(nullptr_t np) const { return ptr == nullptr; }
    bool operator!=(gc_ptr &other) const { return ptr != other.ptr; }
    bool operator!=(const gc_ptr &other) const { return ptr != other.ptr; }
    bool operator!=(T *other) const { return ptr != other; }
    bool operator!=(nullptr_t np) const { return ptr != nullptr; }

    //TODO : FIXME : need to use references not pointers (see Qt/Core/String.cs) (these are used by QMap)
    bool operator<(const gc_ptr &other) const { return get()->operator<(other.get()); }
    bool operator>(const gc_ptr &other) const { return get()->operator>(other.get()); }

    template<typename O>
    bool operator==(const gc_ptr<O> &other) const { return other.ptr == ptr;}
    T* operator->() { T *t = dynamic_cast<T*>(ptr); if (t == nullptr) $npe(); return t; }
    const T& operator*() { T *t = dynamic_cast<T*>(ptr); if (t == nullptr) $npe(); return *t; }
    T* get() const { T *t = dynamic_cast<T*>(ptr); if (t == nullptr) $npe(); return t; }
    T& value() const { T *t = dynamic_cast<T*>(ptr); if (t == nullptr) $npe(); return *t; }
    operator T*() {T *t = dynamic_cast<T*>(ptr); if (t == nullptr) $npe(); return t;}
    operator const T*() { const T *t = dynamic_cast<const T*>(ptr); if (t == nullptr) $npe(); return t;}
    auto& operator[](int idx) { T*t = dynamic_cast<T*>(ptr); if (t == nullptr) $npe(); return t->operator[](idx); }
  };

  //same as std::dynamic_pointer_cast but for gc_ptr
  template<typename T, typename U>
  gc_ptr<T> gc_dynamic_pointer_cast(std::gc_ptr<U> ptr) {
    return gc_ptr<T>(dynamic_cast<T*>(ptr.get()));
  }

  //same as std::make_shared but for gc_ptr
  template<typename T, typename... Args>
  gc_ptr<T> make_gc(Args&&... args) {
    return gc_ptr<T>(new T(args...));
  }

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
    std::gc_ptr<Qt::Core::Object> objRef;
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
    FixedData(std::gc_ptr<Qt::Core::Object> obj, T *buf, int size) {
      objRef = obj;
      t = buf;
      length = size;
      alloced = false;	
    }
    FixedData(std::gc_ptr<Qt::Core::Object> obj, T *buf, int size, bool copy) {
      objRef = obj;
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
  $class(bool isInterface, const char*name, $class *base, std::initializer_list<$class*> ifaceList, std::initializer_list<$field*> fieldList, std::initializer_list<$method*> methodList, std::function<std::gc_ptr<Qt::Core::Object>()> newInstance) {
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
  std::function<std::gc_ptr<Qt::Core::Object>()> newInstance;
};

struct $QMutexHolder {
  $QMutexHolder(std::qt_ptr<std::unique_lock<std::mutex>> lock) {this->lock = lock; lock->lock();}
  std::qt_ptr<std::unique_lock<std::mutex>> lock;
  bool signal = true;
  bool Condition() {return signal;}
  void Signal() {signal = false;}
  ~$QMutexHolder() {lock->unlock();}
};

template<typename T>
inline std::gc_ptr<T>& $check(std::gc_ptr<T> &sptr) {
  if (sptr == nullptr) $npe();
  return sptr;
}

template<typename T>
inline std::function<T> $check(std::function<T> func) {
  if (!func) $npe();
  return func;
}

template<typename T>
inline Qt::QSharp::Property<T> $check(Qt::QSharp::Property<T> pptr) {
  if (pptr == nullptr) $npe();
  return pptr;
}

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
extern std::gc_ptr<Qt::Core::String> $addstr(std::gc_ptr<Qt::Core::String> s1, std::gc_ptr<Qt::Core::String> s2);

extern std::gc_ptr<Qt::Core::String> $addstr(std::gc_ptr<Qt::Core::String> s1, std::gc_ptr<Qt::Core::Object> y);
extern std::gc_ptr<Qt::Core::String> $addstr(std::gc_ptr<Qt::Core::String> s1, char16 y);
extern std::gc_ptr<Qt::Core::String> $addstr(std::gc_ptr<Qt::Core::String> s1, int32 y);
extern std::gc_ptr<Qt::Core::String> $addstr(std::gc_ptr<Qt::Core::String> s1, int64 y);
extern std::gc_ptr<Qt::Core::String> $addstr(std::gc_ptr<Qt::Core::String> s1, uint32 y);
extern std::gc_ptr<Qt::Core::String> $addstr(std::gc_ptr<Qt::Core::String> s1, uint64 y);
extern std::gc_ptr<Qt::Core::String> $addstr(std::gc_ptr<Qt::Core::String> s1, float y);
extern std::gc_ptr<Qt::Core::String> $addstr(std::gc_ptr<Qt::Core::String> s1, double y);

extern std::gc_ptr<Qt::Core::String> $addstr(std::gc_ptr<Qt::Core::Object> x, std::gc_ptr<Qt::Core::String> s2);
extern std::gc_ptr<Qt::Core::String> $addstr(int32 x, std::gc_ptr<Qt::Core::String> s2);
extern std::gc_ptr<Qt::Core::String> $addstr(int64 x, std::gc_ptr<Qt::Core::String> s2);
extern std::gc_ptr<Qt::Core::String> $addstr(uint32 x, std::gc_ptr<Qt::Core::String> s2);
extern std::gc_ptr<Qt::Core::String> $addstr(uint64 x, std::gc_ptr<Qt::Core::String> s2);
extern std::gc_ptr<Qt::Core::String> $addstr(float x, std::gc_ptr<Qt::Core::String> s2);
extern std::gc_ptr<Qt::Core::String> $addstr(double x, std::gc_ptr<Qt::Core::String> s2);

template<typename T1, typename T2>
T1 $addnum(T1 x,T2 y) {return x + y;}

template<typename T1, typename T2>
T1 $addnum(Qt::QSharp::Property<T1> x,T2 y) {return x.$value + y;}

template<typename T1, typename T2>
T1 $addnum(T1 x,Qt::QSharp::Property<T2> y) {return x + y.$value;}

template<typename T1, typename T2>
T1 $addnum(Qt::QSharp::Property<T1> x,Qt::QSharp::Property<T2> y) {return x.$value + y.$value;}

