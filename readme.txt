Q Sharp
-------

Q# is a new experiment to convert C# to C++ using the Qt library as the classlib.

C# + C++ + Qt = Q#

The project has no relation to Microsofts new Q# (Quantum) programming language.

folder layout:
  /cs2cpp - C# .NET Core program to convert C# to C++ using Roslyn code analyzer
  /classlib - Qt wrapper classes
  /tests/helloworld - simple Console.WriteLine() example
  /tests/test... - various test apps

Build Tools:
  .Net Core 2.0 + Roslyn
  C++14 toolset (gcc or MSVC)
  Qt5.4+ libraries
  CMake/3.6+
  Platform make tool (make for GCC, nmake for MSVC)
  ffmpeg/3.0+

ffmpeg:
  Download ffmpeg, extract to include folder and run 'bash configure --disable-yasm'
  You'll need C++ compiler in path (use gcc for cygwin/mingw).
  The pre-built shared binaries can be downloaded from ffmpeg.org

Notes:
 - uses std::shared_ptr<> to implement memory management
 - NullPointerExceptions are checked
 - classlib is a work in progress

C# features that differ:
 - lock () {} only works with Qt.Core.ThreadLock or Qt.Core.ThreadSignal objects

C# features not supported (yet):
 - reflection (partially supported)
 - operators
 - events
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
  to specify build type (Release recommended for faster compiling):
    -D CMAKE_BUILD_TYPE=Release | Debug
  to build with MSVC++
    -G "NMake Makefiles"
  there are scripts in /bin for each platform (gcc and msvc)

To compile with MSVC++
  Install either VS BuildTools or VS IDE with VC++ toolchain.
  cs2cpp required options : --msvc

VS Code Bug
  If you use VS Code and have either VS Build Tools or VS IDE installed
  you MUST also install the .Net Core that comes with those installers.
  Otherwise the OmniSharp extension in VS Code tries to use the wrong MSBuild environment.
  See bug reports
      https://github.com/Microsoft/vscode/issues/40721
      https://github.com/OmniSharp/omnisharp-vscode/issues/1941
      https://github.com/OmniSharp/omnisharp-roslyn/issues/1094
  VS BuildTools : install .Net Core 1.1 - you must install all components.
  VS IDE : install .Net Core 2.0 which is a little outdated.
  Temporary work around to avoid installing older .Net SDKs:
    Rename "C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild" to MSBuild.disabled
      or
    Rename "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild" to MSBuild.disabled

To compile under cygwin/mingw define these environment variables before calling cmake:
  set CC=/usr/bin/x86_64-w64-mingw32-gcc.exe
  set CXX=/usr/bin/x86_64-w64-mingw32-gcc.exe

Under cygwin you should also define:
  set CMAKE_LEGACY_CYGWIN_WIN32=0
to avoid cmake warnings.

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

Version 0.14

Released Mar 16, 2018
