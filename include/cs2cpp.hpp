#ifndef CS2CPP_HPP
#define CS2CPP_HPP

//include C++ headers
#include <cstddef>
#include <cstdio>
#include <cstring>
#include <cmath>
#include <memory>
#include <atomic>
#include <functional>
#include <initializer_list>
#include <new>

//include Qt Headers
#include <QtCore/qmath.h>
#include <QtCore/QCoreApplication>
#include <QtCore/QFile>
#include <QtCore/QDir>
#include <QtCore/QMutex>
#include <QtCore/QTimer>
#include <QtCore/QThread>
#include <QtCore/QDate>
#include <QtCore/QTime>
#include <QtCore/QDateTime>
#include <QtCore/QUrl>
#include <QtCore/QByteArray>
#include <QtCore/QBuffer>
#include <QtCore/QLibrary>

#include <QtGui/QGuiApplication>
#include <QtGui/QOpenGLWindow>
#include <QtGui/QOpenGLFunctions>
#include <QtGui/QKeyEvent>
#include <QtGui/QMouseEvent>
#include <QtGui/QMatrix4x4>
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
#include <QtWidgets/QProgressBar>
#include <QtWidgets/QListWidget>
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

#include <QtXml/QDomNode>
#include <QtXml/QDomElement>
#include <QtXml/QDomDocument>
#include <QtXml/QDomAttr>

//include Quazip Headers
#include <quazip/quazip.h>
#include <quazip/quazipfile.h>

namespace Qt::Core {
  extern int g_argc;
  extern const char **g_argv;
  class Object;
}

namespace Qt::Media {
  struct FFContext;
}

//macro definitions

#define int8 signed char
#define int16 signed short
#define int32 signed int
#undef int64
#define int64 signed long long

#define uint8 unsigned char
#define uint16 unsigned short
#define uint32 unsigned int
#define uint64 unsigned long long

#define char8 char
#define char16 short

#define true 1
#define false 0

extern void $npe();  //NullPointerException
extern void $abe();  //ArrayBoundsException

//fixed arrays
template<typename T>
class QSharpArray {
public:
  T *t;
  int Length;
  bool alloced;
  std::shared_ptr<Qt::Core::Object> ref;
  int $get_Length() {return Length;}
  QSharpArray(int size) {if (size < 0) $abe(); t = (T*)new T[size]; Length = size; alloced = true; }
  QSharpArray(void *buf, int size) {t = (T*)buf; Length = size; alloced = false;}
  QSharpArray(std::shared_ptr<Qt::Core::Object> ref, void *buf, int size) {t = (T*)buf; Length = size; alloced = false; this->ref = ref;}
  QSharpArray(std::initializer_list<T> list) {int size = list.size(); t = (T*)new T[size]; Length = size; T* ptr = (T*)list.begin(); for(int idx=0;idx<size;idx++) {t[idx] = ptr[idx];} alloced = true; }
  ~QSharpArray() {if (alloced) delete[] t;}
  T& operator[](int pos) {if (pos < 0 || pos > Length) $abe(); return t[pos];}
  T* data() {return t;}  //deprecated
  T& at(int pos) {if (pos < 0 || pos > Length) $abe(); return t[pos];}  //deprecated
};

template<typename T>
inline T* $deref(std::shared_ptr<T> x) {
  T* ptr = x.get();
  if (ptr == nullptr) $npe();
  return ptr;
}

namespace Qt::Core {
class String;
class Object;
}

template<typename T>
std::function<T> $checkDelegate(std::function<T> func) {
  if (!func) $npe();
  return func;
}

inline int $hash(Qt::Core::Object *obj) {
  union {Qt::Core::Object* ptr; int hash;} u;
  u.ptr = obj;
  return u.hash;
}

template<typename T>
void $checkArray(std::shared_ptr<QSharpArray<T>> array, int offset, int length) {
  if (offset + length > array->Length) $abe();
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

#endif
