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

//include Qt Headers
#include <QtCore/qmath.h>
#include <QtCore/QCoreApplication>
#include <QtCore/QFile>
#include <QtCore/QDir>
#include <QtCore/QMutex>
#include <QtCore/QTimer>
#include <QtCore/QThread>

#include <QtGui/QGuiApplication>
#include <QtGui/QOpenGLWindow>
#include <QtGui/QOpenGLFunctions>
#include <QtGui/QKeyEvent>
#include <QtGui/QMouseEvent>
#include <QtGui/QMatrix4x4>
#include <QtGui/QOpenGLShaderProgram>
#include <QtGui/QScreen>

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

#include <QtWidgets/QLayout>
#include <QtWidgets/QBoxLayout>
#include <QtWidgets/QHBoxLayout>
#include <QtWidgets/QVBoxLayout>
#include <QtWidgets/QFormLayout>
#include <QtWidgets/QGridLayout>
#include <QtWidgets/QStackedLayout>

#include <QtNetwork/QTcpSocket>
#include <QtNetwork/QUdpSocket>

//include Quazip Headers
#include <quazip/quazip.h>
#include <quazip/quazipfile.h>

namespace Qt::Core {
  extern int g_argc;
  extern const char **g_argv;
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

#define char8 unsigned char
#define char16 unsigned short

#define true 1
#define false 0

extern void $npe();  //NullPointerException
extern void $abe();  //ArrayBoundsException

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
void $checkArray(std::shared_ptr<QVector<T>> array, int offset, int length) {
  int arrayLength = array->size();
  if (offset + length > arrayLength) $abe();
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
