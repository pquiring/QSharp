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
  C++ toolset (C++14 required for GCC, C++17 required for MSVC)
  QT5 libraries (Qt 5.4+ required)
  CMake (3.6 for GCC, 3.10 for MSVC)
  Platform make tool (make for GCC, nmake for MSVC)
  ffmpeg/3.0+

ffmpeg:
  Download ffmpeg extract to include folder and run 'bash configure --disable-yasm'
  You'll need C++ compiler in path (use gcc for cygwin/mingw).
  The pre-built shared binaries can be downloaded from ffmpeg.org

Notes:
 - uses std::shared_ptr<> to implement memory management
 - NullPointerExceptions are checked
 - classlib is a work in progress

C# features that differ:
 - lock () {} only works with Qt.Core.ThreadLock objects
 - typeof(Class) must be enclosed in "new Type()" or "Type.Convert()" since it returns System.Type which needs to be converted to Qt.Core.Type

C# features not supported (yet):
 - reflection (is as)
 - operators
 - events
 - goto switch case label

Compiling:

First compile the compiler:

  cd cs2cpp
  setup
  build

Then build the classlib or any test:

  cd classlib | test...
  setup
  build
  cmake {cmake options}
  make | nmake

cmake options (depends on platform):
  to specify build type (Release recommended for faster compiling):
    -D CMAKE_BUILD_TYPE=Release | Debug
  to build with MSVC++
    -G "NMake Makefiles"

To compile with MSVC++
  cs2cpp required options : --cxx=17 --msvc
  Requires CMake/3.10
  When using VS Build Tools installer for VC++ make sure to install the .Net Core 1.1 toolset even if you have .NET 2.0 Core installed or you'll get strange errors that will keep you up at night ;)

To compile under cygwin/mingw define these environment variables before calling cmake:
  set CC=/usr/bin/x86_64-w64-mingw32-gcc.exe
  set CXX=/usr/bin/x86_64-w64-mingw32-gcc.exe

Under cygwin you should also define:
  set CMAKE_LEGACY_CYGWIN_WIN32=0
to avoid cmake warnings.

WebSite : github.com/pquiring/qsharp

Author : Peter Quiring (pquiring@gmail.com)

Version 0.8

Released Dec 15, 2017
