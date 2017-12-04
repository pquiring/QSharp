Q Sharp
-------

Q# is a new experiment to convert C# to C++ using the Qt library as the classlib.

C# + C++ + Qt = Q#

folder layout:
  /cs2cpp - C# .NET Core program to convert C# to C++ using Roslyn code analyzer
  /classlib - Qt wrapper classes
  /tests/helloworld - simple Console.WriteLine() example
  /tests/test... - various test apps

Build Tools:
  .Net Core 2.0 + Roslyn
  cygwin/mingw gcc toolset (C++11 required)
  cygwin/mingw qt5 libraries (Qt 5.4+ required)
  cygwin/cmake
  cygwin/make

Notes:
 - uses std::shared_ptr<> to implement memory management
 - NullPointerExceptions are checked
 - classlib is a work in progress

C# features that differ:
 - lock () {} only work with Qt.Core.ThreadLock objects

C# features not supported (yet):
 - reflection
 - operators
 - events
 - goto switch case label

Compiling:

First compile the compiler:

  cd cs2cpp
  setup
  build
  release

Then build the classlib:

  cd classlib
  setup
  build

To build any test:

  cd test...
  setup
  build

To compile under cygwin/mingw define these environment variables before calling cmake:
  set CC=/usr/bin/x86_64-w64-mingw32-gcc.exe
  set CXX=/usr/bin/x86_64-w64-mingw32-gcc.exe
If using cygwin/mingw you need to create a symlink /usr/include/qt5 to /usr/x86_64-w64-mingw32/sys-root/mingw/include/qt5
  cd \cygwin
  ln -s /usr/x86_64-w64-mingw32/sys-root/mingw/include/qt5 /usr/include/qt5
Do not install the cygwin version of qt5 since it's version is different and can cause unresolved linker errors.

Under cygwin you should also define:
  set CMAKE_LEGACY_CYGWIN_WIN32=0
to avoid cmake warnings.

WebSite : github.com/pquiring/qsharp

Author : Peter Quiring (pquiring@gmail.com)

Version 0.6

Released Dec 1, 2017
