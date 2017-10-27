Q Sharp
-------

Q# is a new experiment to convert C# to C++ using the Qt library as the classlib.

C# + C++ + Qt = Q#

folder layout:
/cs2cpp - C# .NET Core program to convert C# to C++ using Roslyn code analyzer
/classlib - Qt wrapper classes
/tests/helloworld - simple Console.WriteLine() example
/tests/testgl1 - simple OpenGL app using QOpenGLWindow

Build Tools:
.Net Core 2.0 + Roslyn
cygwin/mingw gcc toolset (C++11 required)
cygwin/mingw qt5 libraries
cygwin/cmake
cygwin/make

Notes:
 - uses std::shared_ptr<> to implement memory management
 - NullPointerExceptions are checked
 - not all C# features are supported (reflection, properties, etc.)
 - classlib is a work in progress

WebSite : github.com/pquiring/qsharp

Author : Peter Quiring (pquiring@gmail.com)

Version 0.1

Released Oct 27, 2017
