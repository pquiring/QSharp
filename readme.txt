Q Sharp
-------

Q# is a new experiment to convert C# to C++ using the Qt library as the classlib.

C# + C++ + Qt = Q#

This project has no relation to Microsofts new Q# (Quantum) programming language.
  (but I released my project first!)

Folder Layout:
  /cs2cpp - C# .NET Core program to convert C# to C++ using Roslyn code analyzer
  /classlib - Qt wrapper classes
  /tests/helloworld - simple Console.WriteLine() example
  /tests/test... - various test apps

Build Tools:
  .Net Core 2.0 + Roslyn
  C++17 toolset (gcc or msvc)
  Qt/5.4+ libraries
  CMake/3.8+
  Platform make tool (make for gcc, nmake for msvc)

Notes:
 - Supports either self-managed memory management like in C++ (see Object.Delete()) or the 'Boehm-Demers-Weiser' Garbage Collector
   - Garbage Collectors and Reference Counting often create performance issues
   - attribute [AutoMemoryPool] can be used on methods to automatically free all objects created during method when method returns
     - return value will not be freed
     - Object.Detach() can be used to "detach" an object from the auto release memory pool
     - this is usefull in methods that create many untrackable objects such as string + operators
 - NullPointerExceptions are checked
 - classlib is a work in progress
 - there are still some memory leaks, especially in the widget elements since ownership is often ambiguous
 - currently cygwin/mingw support is not working until cygwin cmake is upgraded to at least 3.8
   - don't recommend mingw anyways, it's generated code is very slow compared to msvc

Garbage Collector:
 - please go to https://pquiring.github.io/QSharp/ to download source and pre-built library files.
 - cs2cpp required option : --gc

C# features that differ:
 - lock () {} only works with Qt.Core.ThreadLock

C# features not supported (yet):
 - reflection (partially supported)
 - operators (ConversionOperatorDeclaration, OperatorDeclaration)
 - events
 - multiple Property inheritance (ExplicitInterfaceSpecifier)
 - linq
 - etc.

Environment setup:
  set MSBuildSDKsPath=C:\Program Files\dotnet\sdk\2.x.x\Sdks
  set QSHARP={cs2cpp options}

Compiling:

First compile the compiler:

  cd cs2cpp
  #run setup only once
  setup
  build

Then build the classlib or any test:

  cd classlib | test...
  #run setup only once
  setup
  build
  cmake {cmake options}
  make | nmake

cmake options (depends on platform):
  to specify build type:
    -D CMAKE_BUILD_TYPE=Release | Debug
  to build with msvc
    -G "NMake Makefiles"
  there are scripts in /bin for each platform (gcc and msvc)

To compile with msvc:
  Install either VS BuildTools or VS IDE with VC++ toolchain.
  cs2cpp required options : --msvc

VS Code Bug
  If you use VS Code and have either VS Build Tools or VS IDE installed
  and have the .Net Core standalone installed
  you MUST also install the .Net Core that comes with those installers.
  Otherwise the OmniSharp extension in VS Code tries to use the wrong MSBuild environment.
  See bug reports
      https://github.com/Microsoft/vscode/issues/40721
      https://github.com/OmniSharp/omnisharp-vscode/issues/1941
      https://github.com/OmniSharp/omnisharp-roslyn/issues/1094
  VS BuildTools : install .Net Core 1.1 - you must install all components.
  VS IDE : install .Net Core 2.0 which is a little outdated.
  Temporary work around to avoid installing older .Net Core SDKs:
    Rename "C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild" to MSBuild.disabled
      or
    Rename "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild" to MSBuild.disabled

To compile under cygwin/mingw define these environment variables before calling cmake:
  set CC=/usr/bin/x86_64-w64-mingw32-gcc.exe
  set CXX=/usr/bin/x86_64-w64-mingw32-gcc.exe

Under cygwin you should also define:
  set CMAKE_LEGACY_CYGWIN_WIN32=0
to avoid cmake warnings.

Under cygwin/mingw you may need to setup a link to mingw headers files:
  bash
  ln -s /usr/x86_64-w64-mingw32/sys-root/mingw/include/qt5 /usr/include/qt5

To create a new application:
  dotnet new console

To create a new library:
  dotnet new library

To add reference to another library:
  dotnet add reference ..\library\library.csproj

To test C# compilation (not really necessary) (the /clp:NoSummary option avoids outputting errors/warnings twice)
  dotnet build /clp:NoSummary

WebSite : github.com/pquiring/qsharp

Author : Peter Quiring (pquiring@gmail.com)

Version 0.17

Released Aug 10, 2018
