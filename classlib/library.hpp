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

#undef int64
#define int64 signed long long

namespace Qt::Core {
  extern int g_argc;
  extern const char **g_argv;
  class Object;
  class String;
  class $QThread;
}

namespace Qt::Gui {
  class $EventFilter;
  class $QOpenGLWidget;
  class $QOpenGLWindow;
}

namespace Qt::Network {
  class $QSslServer;
  class $QWebReply;
}

namespace Qt::Media {
  struct FFContext;
}

extern void $npe();  //NullPointerException
extern void $abe();  //ArrayBoundsException

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
  $class(bool isInterface, const char*name, $class *base, std::initializer_list<$class*> ifaceList, std::initializer_list<$field*> fieldList, std::initializer_list<$method*> methodList) {
    this->iface = isInterface;
    this->name = name;
    this->base = base;
    this->ifaces = new QVector<$class*>(ifaceList);
    this->fields = new QVector<$field*>(fieldList);
    this->methods = new QVector<$method*>(methodList);
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
};

struct $QMutexHolder {
  $QMutexHolder(std::shared_ptr<QMutex> lock) {this->lock = lock; lock->lock();}
  std::shared_ptr<QMutex> lock;
  bool signal = true;
  bool Condition() {return signal;}
  void Signal() {signal = false;}
  ~$QMutexHolder() {lock->unlock();}
};

template<typename T>
inline std::shared_ptr<T> $check(std::shared_ptr<T> sptr) {
  if (sptr == nullptr) $npe();
  return sptr;
}

template<typename T>
inline std::shared_ptr<T> $check(std::weak_ptr<T> wptr) {
  std::shared_ptr<T> sptr = wptr.lock();
  if (sptr == nullptr) $npe();
  return sptr;
}

template<typename T>
std::function<T> $check(std::function<T> func) {
  if (!func) $npe();
  return func;
}

inline int $hash(Qt::Core::Object *obj) {
  union {Qt::Core::Object* ptr; int hash;} u;
  u.ptr = obj;
  return u.hash;
}

//$mod
inline int $mod(int x, int y) {return x % y;}
inline uint32 $mod(uint32 x, uint32 y) {return x % y;}
inline float $mod(float x, float y) {return std::fmod(x, y);}
inline double $mod(double x, double y) {return std::fmod(x, y);}

//$add
extern std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, std::shared_ptr<Qt::Core::String> s2);

extern std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, std::shared_ptr<Qt::Core::Object> y);
extern std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, int32 y);
extern std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, int64 y);
extern std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, uint32 y);
extern std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, uint64 y);
extern std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, float y);
extern std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, double y);

extern std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::Object> x, std::shared_ptr<Qt::Core::String> s2);
extern std::shared_ptr<Qt::Core::String> $add(int32 x, std::shared_ptr<Qt::Core::String> s2);
extern std::shared_ptr<Qt::Core::String> $add(int64 x, std::shared_ptr<Qt::Core::String> s2);
extern std::shared_ptr<Qt::Core::String> $add(uint32 x, std::shared_ptr<Qt::Core::String> s2);
extern std::shared_ptr<Qt::Core::String> $add(uint64 x, std::shared_ptr<Qt::Core::String> s2);
extern std::shared_ptr<Qt::Core::String> $add(float x, std::shared_ptr<Qt::Core::String> s2);
extern std::shared_ptr<Qt::Core::String> $add(double x, std::shared_ptr<Qt::Core::String> s2);

inline int8 $add(int8 x,int8 y) {return x + y;}
inline int16 $add(int16 x,int16 y) {return x + y;}
inline int32 $add(int32 x,int32 y) {return x + y;}
inline int64 $add(int64 x,int64 y) {return x + y;}

inline uint8 $add(uint8 x,uint8 y) {return x + y;}
inline uint16 $add(uint16 x,uint16 y) {return x + y;}
inline uint32 $add(uint32 x,uint32 y) {return x + y;}
inline uint64 $add(uint64 x,uint64 y) {return x + y;}

inline float $add(float x,float y) {return x + y;}
inline double $add(double x,double y) {return x + y;}

//char mixed
inline int16 $add(char16 x,int8 y) {return x + y;}
inline int32 $add(char16 x,int32 y) {return x + y;}
inline int64 $add(char16 x,int64 y) {return x + y;}

inline int16 $add(int8 x,char16 y) {return x + y;}
inline int32 $add(int32 x,char16 y) {return x + y;}
inline int64 $add(int64 x,char16 y) {return x + y;}

inline uint16 $add(char16 x,uint8 y) {return x + y;}
inline uint32 $add(char16 x,uint32 y) {return x + y;}
inline uint64 $add(char16 x,uint64 y) {return x + y;}

inline uint16 $add(uint8 x,char16 y) {return x + y;}
inline uint32 $add(uint32 x,char16 y) {return x + y;}
inline uint64 $add(uint64 x,char16 y) {return x + y;}

//mixed numericals
inline float $add(float x,int32 y) {return x + y;}
inline float $add(int32 x,float y) {return x + y;}
inline float $add(float x,int64 y) {return x + y;}
inline float $add(int64 x,float y) {return x + y;}

inline float $add(float x,uint32 y) {return x + y;}
inline float $add(uint32 x,float y) {return x + y;}
inline float $add(float x,uint64 y) {return x + y;}
inline float $add(uint64 x,float y) {return x + y;}

inline double $add(double x,int32 y) {return x + y;}
inline double $add(int32 x,double y) {return x + y;}
inline double $add(double x,int64 y) {return x + y;}
inline double $add(int64 x,double y) {return x + y;}

inline double $add(double x,uint32 y) {return x + y;}
inline double $add(uint32 x,double y) {return x + y;}
inline double $add(double x,uint64 y) {return x + y;}
inline double $add(uint64 x,double y) {return x + y;}

inline int64 $add(int64 x,int32 y) {return x + y;}
inline int64 $add(int32 x,int64 y) {return x + y;}

inline int64 $add(uint64 x,uint32 y) {return x + y;}
inline int64 $add(uint32 x,uint64 y) {return x + y;}

inline int32 $add(int32 x,int8 y) {return x + y;}
inline int32 $add(int8 x,int32 y) {return x + y;}

inline int64 $add(int64 x,int8 y) {return x + y;}
inline int64 $add(int8 x,int64 y) {return x + y;}

inline int32 $add(int32 x,uint8 y) {return x + y;}
inline int32 $add(uint8 x,int32 y) {return x + y;}

inline int64 $add(int64 x,uint8 y) {return x + y;}
inline int64 $add(uint8 x,int64 y) {return x + y;}
