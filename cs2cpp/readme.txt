cs2cpp
======

Desc : Converts C# to C++ designed for Qt library.

Code Name : Q#

Usage : cs2cpp cs_in_folder cpp_out_file hpp_out_file [--library | --main=Class]

Notes:
 - uses automatic reference counting using std::shared_ptr
 - use [weak] attribute for weak references to avoid circular references
 - uses Roslyn to analyze code
 - C++ wrapper classes are used to bridge between C# and C++ worlds

Install:
 - download and install .NET Core 2.0 (http://microsoft.com/net/core)
 - run setup.bat to install dependancies

Building:
 - dotnet build

Technical Overview:
 - to prevent shared_ptr fragmentation each class has a $this member which is a weak_ptr<> reference to itself
 - the new operator is really complex, the generated code looks like this:
   C#  Object obj = new Object();
   C++ shared_ptr<Object> obj = Object::$new(args...);
   - there are three methods declared for contructors
     $new(args...)  //basically creates the shared_ptr<> $this pointer, invokes $init()ialize to set field values, invokes $ctor and then returns the $this pointer
     $init()  //init all field values
     $ctor(args...)  //the ctor defined in the C# source
   - thereforce the C++ objects don't have any 'real' ctors defined

Some features are NOT supported yet:
 - reflection (partially)
 - plus many more...
These will be implemented eventually.
