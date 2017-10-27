#ifndef CS2CPP_HPP
#define CS2CPP_HPP

//include C headers
#include <math.h>

//include C++ headers
#include <cstddef>
#include <memory>
#include <atomic>
#include <functional>
#include <initializer_list>

//include Qt Headers
#include <QtGui/QGuiApplication>
#include <QtGui/QOpenGLWindow>
#include <QtGui/QOpenGLFunctions>
#include <QtGui/QMatrix4x4>
#include <QtGui/QOpenGLShaderProgram>
#include <QtGui/QScreen>
#include <QtCore/qmath.h>

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

extern void $npe();

template<typename T> T* $deref(std::shared_ptr<T> x) {
  if (x == nullptr) $npe();
  return x.get();
}

#endif
