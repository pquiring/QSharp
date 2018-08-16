using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace QSharpCompiler
{
    class Program
    {
        public static string csFolder;
        public static string cppFolder;
        public static string hppFile;
        public static List<Source> files = new List<Source>();
        public static bool printTree = false;
        public static bool printToString = false;
        public static bool printTokens = false;
        public static string version = "0.20";
        public static bool library;
        public static bool shared;
        public static bool service;
        public static String serviceName;
        public static string target;
        public static bool classlib;
        public static int headidx = 0;
        public static string main;
        public static string home = ".";
        public static string cxx = "17";
        public static bool single = false;  //generate monolithic cpp source file
        public static bool msvc = false;
        public static bool debug = false;
        public static bool gc = false;
        public static bool no_npe_checks = false;
        public static List<string> refs = new List<string>();
        public static List<string> libs = new List<string>();

        public static CSharpCompilation compiler;

        static void Main(string[] args)
        {
            if (args.Length < 2) {
                Console.WriteLine("Q# Compiler/" + version);
                Console.WriteLine("Usage : cs2cpp cs_folder project_name [options]");
                Console.WriteLine("options:");
                Console.WriteLine("  --library");
                Console.WriteLine("      create a library");
                Console.WriteLine("  --shared");
                Console.WriteLine("      create a shared library (dll so)");
                Console.WriteLine("  --main=class");
                Console.WriteLine("      define class that contains static void Main(String[] args)");
                Console.WriteLine("  --service=name");
                Console.WriteLine("      create a Win32 Service");
                Console.WriteLine("  --ref=dll");
                Console.WriteLine("      add a reference to a library");
                Console.WriteLine("  --home=folder");
                Console.WriteLine("      define where Q# home folder is (to find include and lib folders)");
                Console.WriteLine("  --print[=tokens,tostring,all]");
                Console.WriteLine("      print out C# Roslyn SyntaxNode trees");
                Console.WriteLine("  --single | --multi");
                Console.WriteLine("      generate a single monolithic C++ file or multiple files (default = single)");
                Console.WriteLine("  --gcc | --msvc");
                Console.WriteLine("      use gcc or msvc C++ compiler (default = gcc)");
                Console.WriteLine("  --release | --debug");
                Console.WriteLine("      generate release or debug type");
                Console.WriteLine("  --gc");
                Console.WriteLine("      enable experimental support for 'Boehm-Demers-Weiser' Garbage Collector");
                Console.WriteLine("  --no-npe-checks");
                Console.WriteLine("      disable NPE checks");
                return;
            }

            for(int a=2;a<args.Length;a++) {
                int idx = args[a].IndexOf("=");
                String arg = "";
                String value = "";
                if (idx == -1) {
                    arg = args[a];
                    value = "";
                } else {
                    arg = args[a].Substring(0, idx);
                    value = args[a].Substring(idx + 1);
                }
                if (arg == "--library") {
                    library = true;
                }
                if (arg == "--shared") {
                    shared = true;
                }
                if (arg == "--service") {
                    service = true;
                    if (value.Length == 0) {
                        Console.WriteLine("Error:--service requires a name");
                        return;
                    }
                    serviceName = value;
                }
                if (arg == "--classlib") {
                    classlib = true;
                }
                if (arg == "--main") {
                    if (value.Length == 0) {
                        Console.WriteLine("Error:--main requires a class");
                        return;
                    }
                    main = value.Replace(".", "::");
                }
                if (arg == "--cxx") {
                    cxx = value;
                }
                if (arg == "--gc") {
                    gc = true;
                }
                if (arg == "--no-npe-checks") {
                    no_npe_checks = true;
                }
                if (arg == "--ref") {
                    if (value.Length == 0) {
                        Console.WriteLine("Error:--ref requires a file");
                        return;
                    }
                    refs.Add(value);
                    int i1 = value.LastIndexOf("\\");
                    if (i1 != -1) {
                        value = value.Substring(i1+1);
                    }
                    int i2 = value.IndexOf(".");
                    value = value.Substring(0, i2);
                    libs.Add(value);
                }
                if (arg == "--home") {
                    if (value.Length == 0) {
                        Console.WriteLine("Error:--home requires a path");
                        return;
                    }
                    home = value.Replace("\\", "/");
                }
                if (arg == "--print") {
                    printTree = true;
                    switch (value) {
                        case "all": printToString = true; printTokens = true; break;
                        case "tokens": printTokens = true; break;
                        case "tostring": printToString = true; break;
                    }
                }
                if (arg == "--single") {
                    single = true;
                }
                if (arg == "--multi") {
                    single = false;
                }
                if (arg == "--msvc") {
                    msvc = true;
                }
                if (arg == "--gcc") {
                    msvc = false;
                }
                if (arg == "--debug") {
                    debug = true;
                }
                if (arg == "--release") {
                    debug = false;
                }
            }
            if (shared && !library) {
                Console.WriteLine("Error:--shared requires --library");
                return;
            }
            if (classlib && !library) {
                Console.WriteLine("Error:--classlib requires --library");
                return;
            }
            if (shared && main == null) {
                Console.WriteLine("Error:--shared requires --main");
                return;
            }
            if (service && library) {
                Console.WriteLine("Error:--service can not be a --library");
                return;
            }
            if (service && main == null) {
                Console.WriteLine("Error:--service requires --main");
                return;
            }
            if (!library && main == null) {
                Console.WriteLine("Error:application requires --main");
                return;
            }
            csFolder = args[0];
            target = args[1];
            cppFolder = "cpp";
            hppFile = target + ".hpp";
            new Program().process();
            Console.WriteLine("cs2cpp generated " + args[1]);
        }

        void process()
        {
            compiler = CSharpCompilation.Create("C#");
            var corelibs = ((String)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
            foreach(var lib in corelibs) {
//                if (lib.Contains("System")) {
//                    Console.WriteLine("Adding Corelib Reference:" + lib);
                    compiler = compiler.AddReferences(MetadataReference.CreateFromFile(lib));
//                }
            }
            foreach(var lib in refs) {
                Console.WriteLine("Adding Reference:" + lib);
                compiler = compiler.AddReferences(MetadataReference.CreateFromFile(lib));
            }
            if (classlib) addHead();
            addFolder(csFolder);
            foreach(Source node in files)
            {
                node.model = compiler.GetSemanticModel(node.tree);
                if (printTree) {
                    printNodes(node, node.tree.GetRoot().ChildNodes(), 0, true);
                }
            }
            try {
                new Generate().generate();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        void addHead() {
            headidx = 1;
            Source node = new Source();
            node.clss = new List<Class>();
            node.src = "//head";
            node.cppFile = "cpp/$head.cpp";
            node.tree = CSharpSyntaxTree.ParseText(node.src);
            compiler = compiler.AddSyntaxTrees(node.tree);
//            files.Add(node);
        }

        void addFolder(string folder)
        {
            string[] files = Directory.GetFiles(folder);
            foreach(var file in files) {
                if (file.EndsWith(".cs")) addFile(file);
            }
            string[] folders = Directory.GetDirectories(folder);
            foreach(var subfolder in folders) {
                addFolder(subfolder);
            }
        }

        void addFile(string file)
        {
            if (file.IndexOf("AssemblyInfo") != -1) return;
            Source node = new Source();
            node.src = System.IO.File.ReadAllText(file);
            node.csFile = file;
            node.cppFile = cppFolder + "/" + node.csFile.Replace(".", "_").Replace("\\", "_") + ".cpp";
            node.csTimestamp = timestamp(node.csFile);
            node.cppTimestamp = timestamp(node.cppFile);
            node.clss = new List<Class>();
            int idx = csFolder.Length;
            int len = file.Length - idx - 3;
            file = file.Substring(idx, len);
            node.tree = CSharpSyntaxTree.ParseText(node.src);
            compiler = compiler.AddSyntaxTrees(node.tree);
            files.Add(node);
        }

        DateTime now = DateTime.Now;

        long timestamp(string filename) {
            if (!File.Exists(filename)) return -1;
            DateTime dt = File.GetLastWriteTimeUtc(filename);
            TimeSpan offset = now - dt;
            return (long)offset.TotalMilliseconds;
        }

        void printNodes(Source file, IEnumerable<SyntaxNode> nodes, int lvl, bool showDiag)
        {
            int idx = 0;
            if (showDiag) {
                String diags = "";
                foreach(var diag in file.model.GetDiagnostics()) {
                    diags += ",diag=" + diag.ToString();
                }
                foreach(var diag in file.model.GetSyntaxDiagnostics()) {
                    diags += ",syntaxdiag=" + diag.ToString();
                }
                foreach(var diag in file.model.GetDeclarationDiagnostics()) {
                    diags += ",decldiag=" + diag.ToString();
                }
                foreach(var diag in file.model.GetMethodBodyDiagnostics()) {
                    diags += ",methoddiag=" + diag.ToString();
                }
                if (diags.Length > 0) {
                    Console.WriteLine("Errors in file:" + file.csFile);
                    Console.WriteLine(diags);
                }
            }
            foreach(var node in nodes) {
                printNode(file, node, lvl, idx);
                if (printTokens) PrintTokens(file, node.ChildTokens(), lvl);
                printNodes(file, node.ChildNodes(), lvl+1, false);
                idx++;
            }
        }

        public static void printNode(Source file, SyntaxNode node, int lvl, int idx) {
            for(int a=0;a<lvl;a++) {
                Console.Write("  ");
            }
            String ln = "node[" + lvl + "][" + idx + "]=" + node.Kind();
            ISymbol decl = file.model.GetDeclaredSymbol(node);
            if (decl != null) {
                ln += ",DeclSymbol=" + decl.ToString();
                ln += ",DeclSymbol.Name=" + decl.Name;
                ln += ",DeclSymbol.Kind=" + decl.Kind;
                ln += ",DeclSymbol.IsStatic=" + decl.IsStatic;
                ln += ",DeclSymbol.IsAbstract=" + decl.IsAbstract;
                ITypeSymbol containing = decl.ContainingType;
                if (containing != null) {
                    ln += ",DeclSymbol.ContainingType.TypeKind=" + containing.TypeKind;
                }
            }
            ISymbol symbol = file.model.GetSymbolInfo(node).Symbol;
            if (symbol != null) {
                ln += ",Symbol=" + symbol.ToString();
                ln += ",Symbol.Name=" + symbol.Name;
                ln += ",Symbol.Kind=" + symbol.Kind;
                ln += ",Symbol.IsStatic=" + symbol.IsStatic;
                ITypeSymbol containing = symbol.ContainingType;
                if (containing != null) {
                    ln += ",Symbol.ContainingType.TypeKind=" + containing.TypeKind;
                }
            }
            ITypeSymbol type = file.model.GetTypeInfo(node).Type;
            if (type != null) {
                ln += ",Type=" + type.ToString();
                ln += ",Type.IsStatic=" + type.IsStatic;
                ln += ",Type.Kind=" + type.Kind;
                ln += ",Type.TypeKind=" + type.TypeKind;
                ln += ",Type.OrgDef=" + type.OriginalDefinition;
                ln += ",Type.SpecialType=" + type.SpecialType;
                INamedTypeSymbol baseType = type.BaseType;
                if (baseType != null) {
                    ln += ",Type.BaseType=" + baseType;
                    ln += ",Type.BaseType.Name=" + baseType.Name;
                    ln += ",Type.BaseType.TypeKind=" + baseType.TypeKind;
                    ln += ",Type.BaseType.OrgDef=" + baseType.OriginalDefinition;
                    ln += ",Type.BaseType.SpecialType=" + baseType.SpecialType;
                }
            }
            Object value = file.model.GetConstantValue(node).Value;
            if (value != null) {
                ln += ",Constant=" + value.ToString().Replace("\r", "").Replace("\n", "");
            }
            if (printToString) {
                ln += ",ToString=" + node.ToString().Replace("\r", "").Replace("\n", "");
            }
            Console.WriteLine(ln);
        }

        void PrintTokens(Source file, IEnumerable<SyntaxToken> tokens, int lvl)
        {
            int idx = 0;
            foreach(var token in tokens) {
                for(int a=0;a<lvl+2;a++) {
                  Console.Write("  ");
                }
                Console.WriteLine("token[" + lvl + "][" + idx + "]=" + token + ":" + token.Kind());
                idx++;
            }
        }
    }

    class Source
    {
        public string csFile;
        public long csTimestamp;
        public string cppFile;
        public long cppTimestamp;
        public string src;
        public SyntaxTree tree;
        public SemanticModel model;
        public List<Class> clss;
    }

    class Generate
    {
        public static Source file;
        public static int errors = 0;

        private FileStream fs;
        private string Namespace = "";
        private Class NoClass = new Class();  //for classless delegates
        private Class OpClass = new Class();  //for operators
        private List<Class> clss = new List<Class>();

        public static Class cls;
        public Method method;
        public Method init;
        public Field field;

        public void generate()
        {
            if (Program.printTree) {
                Console.WriteLine();
            }
            foreach(Source file in Program.files) {
                generate(file);
            }
            if (errors > 0) {
                Console.WriteLine("Errors:" + errors);
                Environment.Exit(1);
            }
            Directory.CreateDirectory("cpp");
            openOutput("cpp/" + Program.hppFile);
            writeForward();
            /** In C++ you can not use an undefined class, so they must be sorted by usage. */
            buildClasses();
            checkClasses();
            if (errors > 0) {
                Console.WriteLine("Errors:" + errors);
                Environment.Exit(1);
            }
            if (Program.classlib) {
                //move some base classes to the top
                sortClasslib();
            }
            while (sortClasses()) {};
            //TODO : sort inner classes
            writeNoClassTypes();
            writeClasses();
            writeOperators();
            writeEndIf();
            closeOutput();
            if (Program.single) openOutput("cpp/" + Program.target + ".cpp");
            if (Program.single) writeIncludes();
            foreach(Source file in Program.files) {
                //if (!Program.single && file.csTimestamp <= file.cppTimestamp) continue;
                Generate.file = file;
                if (!Program.single) openOutput(file.cppFile);
                if (!Program.single) writeIncludes();
                writeStaticFields();
                writeMethods();
                if (!Program.single) closeOutput();
            }
            if (Program.library) {
                if (File.Exists("library.cpp")) {
                  writeLibrary();
                }
            }
            if (!Program.single) openOutput("cpp/ctor.cpp");
            if (!Program.single) writeIncludes();
            writeStaticFieldsInit();
            if (!Program.single) closeOutput();
            if (Program.main != null) {
                if (!Program.single) openOutput("cpp/main.cpp");
                if (!Program.library) {
                    writeMain();
                } else {
                    writeLibraryMain();
                }
                if (!Program.single) closeOutput();
            }
            if (Program.single) closeOutput();
            openOutput("CMakeLists.txt");
            writeCMakeLists();
            closeOutput();
        }

        private void generate(Source file)
        {
            SyntaxNode root = file.tree.GetRoot();
            if (Program.printTree) {
                Console.WriteLine("Compiling:" + file.csFile);
            }
            outputFile(file);
        }

        private void openOutput(string filename) {
            fs = System.IO.File.Open(filename, FileMode.Create);
            byte[] bytes;
            if (filename.EndsWith(".txt"))
                bytes = new UTF8Encoding().GetBytes("# cs2cpp : Machine generated code : Do not edit!\r\n");
            else
                bytes = new UTF8Encoding().GetBytes("// cs2cpp : Machine generated code : Do not edit!\r\n");
            fs.Write(bytes, 0, bytes.Length);
        }

        private void writeForward() {
            StringBuilder sb = new StringBuilder();
            sb.Append("#ifndef __" + Program.target + "__\r\n");
            sb.Append("#define __" + Program.target + "__\r\n");
            sb.Append("#include <cs2cpp.hpp>\r\n");
            if (Program.library) {
                if (File.Exists("library.hpp")) {
                    sb.Append(System.IO.File.ReadAllText("library.hpp"));
                }
            }
            foreach(var lib in Program.libs) {
                sb.Append("#include <" + lib + ".hpp>\r\n");
            }
            foreach(var file in Program.files) {
                foreach(var cls in file.clss) {
                    if (cls.Namespace == "Qt::QSharp" && cls.name.StartsWith("CPP")) continue;
                    sb.Append(cls.GetReflectionExtern());
                    if (cls.Namespace != "") sb.Append(OpenNamespace(cls.Namespace));
                    sb.Append(cls.GetForwardDeclaration());
                    if (cls.Namespace != "") sb.Append(CloseNamespace(cls.Namespace));
                }
            }
            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        private void writeEndIf() {
            byte[] bytes = new UTF8Encoding().GetBytes("#endif\r\n");
            fs.Write(bytes, 0, bytes.Length);
        }

        public static string OpenNamespace(String Namespace) {
            StringBuilder sb = new StringBuilder();
            String[] strs = Namespace.Split("::");
            for(int a=0;a<strs.Length;a++) {
                sb.Append("namespace " + strs[a] + "{");
            }
            sb.Append("\r\n");
            return sb.ToString();
        }

        public static string CloseNamespace(String Namespace) {
            StringBuilder sb = new StringBuilder();
            String[] strs = Namespace.Split("::");
            for(int a=0;a<strs.Length;a++) {
                sb.Append("}");
            }
            sb.Append("\r\n");
            return sb.ToString();
        }

        private void buildClasses() {
            int fcnt = Program.files.Count;
            for(int fidx = 0;fidx<fcnt;fidx++) {
                Source file = Program.files[fidx];
                List<Class> fclss = file.clss;
                int cnt = fclss.Count;
                for(int idx=0;idx<cnt;idx++) {
                    clss.Add(fclss[idx]);
                }
            }
        }

        /** Check classes for a cross references. */
        private void checkClasses() {
            int cnt = clss.Count;
            for(int idx=0;idx<cnt;idx++) {
                Class cls1 = clss[idx];
                string clsfull = cls1.nsfullname;
                int ucnt1 = cls1.uses.Count;
                for(int uidx1=0;uidx1<ucnt1;uidx1++) {
                    string use = cls1.uses[uidx1];
                    for(int idx2=0;idx2<cnt;idx2++) {
                        if (clss[idx2].nsfullname == use) {
                            Class cls2 = clss[idx2];
                            int ucnt2 = cls2.uses.Count;
                            for(int uidx2=0;uidx2<ucnt2;uidx2++) {
                                if (cls2.uses[uidx2] == clsfull) {
                                    Console.WriteLine("Error:Cross reference detected:" + cls1.nsfullname.Replace("::", ".") + " with " + cls2.nsfullname.Replace("::", "."));
                                    errors++;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void sortClasslib() {
            int cnt = clss.Count;
            for(int idx1=0;idx1<cnt;idx1++) {
                Class cls1 = clss[idx1];
                if (cls1.nsfullname == "Qt::Core::ThreadLock") {
                    clss.RemoveAt(idx1);
                    clss.Insert(0, cls1);
                    break;
                }
            }
            for(int idx1=0;idx1<cnt;idx1++) {
                Class cls1 = clss[idx1];
                if (cls1.nsfullname.StartsWith("Qt::QSharp::FixedArray") && cls1.nsfullname.Contains("Enumerator")) {
                    clss.RemoveAt(idx1);
                    clss.Insert(0, cls1);
                }
            }
            for(int idx1=0;idx1<cnt;idx1++) {
                Class cls1 = clss[idx1];
                if (cls1.nsfullname.StartsWith("Qt::QSharp::FixedArray") && !cls1.nsfullname.Contains("Enumerator")) {
                    clss.RemoveAt(idx1);
                    clss.Insert(0, cls1);
                }
            }
        }

        private bool sortClasses() {
            int cnt = clss.Count;
            for(int idx1=0;idx1<cnt;idx1++) {
                Class cls1 = clss[idx1];
                int ucnt1 = cls1.uses.Count;
                for(int uidx1=0;uidx1<ucnt1;uidx1++) {
                    string use = cls1.uses[uidx1];
                    for(int idx2=idx1+1;idx2<cnt;idx2++) {
                        if (clss[idx2].nsfullname == use) {
                            //need to move idx2 before idx
                            Class tmp = clss[idx2];
                            clss.RemoveAt(idx2);
                            clss.Insert(idx1, tmp);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void writeNoClassTypes() {
            StringBuilder sb = new StringBuilder();
            foreach(var dgate in NoClass.methods) {
                if (dgate.Namespace.Length > 0) {
                    sb.Append(OpenNamespace(dgate.Namespace));
                }
                sb.Append(dgate.GetMethodDeclaration());
                sb.Append(";\r\n");
                if (dgate.Namespace.Length > 0) sb.Append(CloseNamespace(dgate.Namespace));
            }
            foreach(var e in NoClass.enums) {
                if (e.Namespace.Length > 0) {
                    sb.Append(OpenNamespace(e.Namespace));
                }
                sb.Append(GetEnumStruct(e) + e.name + ";\r\n");
                if (e.Namespace.Length > 0) sb.Append(CloseNamespace(e.Namespace));
            }
            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        private void writeOperators() {
            StringBuilder sb = new StringBuilder();
            foreach(var op in OpClass.methods) {
                sb.Append(op.GetMethodDeclaration());
                sb.Append(";\r\n");
            }
            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        /** Create default ctor if class has no ctors. */
        private void createDefaultCtor(Class cls) {
            if (!cls.hasctor && !cls.isInterface && !cls.omitConstructors) {
                Generate.cls = cls;
                ctorNode(null);
            }
            foreach(var inner in cls.inners) {
                createDefaultCtor(inner);
            }
        }

        private void writeClasses() {
            StringBuilder sb = new StringBuilder();
            foreach(var cls in clss) {
                if (cls.Namespace == "Qt::QSharp" && cls.name.StartsWith("CPP")) continue;
                createDefaultCtor(cls);
                if (cls.Namespace != "") sb.Append(OpenNamespace(cls.Namespace));
                sb.Append(cls.GetClassDeclaration());
                if (cls.Namespace != "") sb.Append(CloseNamespace(cls.Namespace));
                if (cls.nonClassHPP != null) sb.Append(cls.nonClassHPP);
                string hppfile = "src/" + cls.name + ".hpp";
                if (File.Exists(hppfile)) {
                    String hpp = File.ReadAllText(hppfile);
                    sb.Append(hpp);
                }
            }
            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        private void writeIncludes() {
            StringBuilder sb = new StringBuilder();
            sb.Append("#include \"" + Program.hppFile + "\"\r\n");
            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        private void writeStaticFields() {
            StringBuilder sb = new StringBuilder();
            foreach(var cls in file.clss) {
                if (cls.Namespace != "") sb.Append(OpenNamespace(cls.Namespace));
                sb.Append(cls.GetStaticFields());
                if (cls.Namespace != "") sb.Append(CloseNamespace(cls.Namespace));
            }
            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        private void writeStaticFieldsInit() {
            StringBuilder sb = new StringBuilder();
            sb.Append("void $" + Program.target + "_ctor() {\r\n");
            foreach(var file in Program.files) {
                foreach(var cls in file.clss) {
                    sb.Append(cls.GetStaticFieldsInit());
                }
            }
            sb.Append("};\r\n");
            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        private void writeMethods() {
            StringBuilder sb = new StringBuilder();
            foreach(var cls in file.clss) {
                if (cls.Namespace == "Qt::QSharp" && cls.name.StartsWith("CPP")) continue;
                if (cls.forward != null) {
                    sb.Append("struct " + cls.forward + ";\r\n");
                }
                sb.Append(cls.GetReflectionData());
                if (cls.Namespace != "") sb.Append(OpenNamespace(cls.Namespace));
                if (!cls.isGeneric) {
                    sb.Append(cls.GetMethodsDefinitions());
                }
                if (cls.Namespace != "") sb.Append(CloseNamespace(cls.Namespace));
                if (cls.nonClassCPP != null) sb.Append(cls.nonClassCPP);
                string cppfile = "src/" + cls.name + ".cpp";
                if (File.Exists(cppfile)) {
                    String cpp = File.ReadAllText(cppfile);
                    sb.Append(cpp);
                }
            }
            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        private void writeLibrary() {
            StringBuilder sb = new StringBuilder();
            sb.Append(System.IO.File.ReadAllText("library.cpp"));
            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        private void writeMain() {
            StringBuilder sb = new StringBuilder();

            foreach(var lib in Program.libs) {
                sb.Append("extern void $" + lib + "_ctor();\r\n");
            }

            if (!Program.single) sb.Append("#include \"" + Program.target + ".hpp\"\r\n");
            if (Program.service) {
                sb.Append("#include <windows.h>\r\n");
            }

            if (Program.service) {
                sb.Append("SERVICE_STATUS_HANDLE ServiceHandle;\r\n");

                sb.Append("void ServiceStatus(int state) {\r\n");
                sb.Append("  SERVICE_STATUS ss;\r\n");
                sb.Append("  ss.dwServiceType = SERVICE_WIN32;\r\n");
                sb.Append("  ss.dwWin32ExitCode = 0;\r\n");
                sb.Append("  ss.dwCurrentState = state;\r\n");
                sb.Append("  ss.dwControlsAccepted = SERVICE_ACCEPT_STOP;\r\n");
                sb.Append("  ss.dwWin32ExitCode = 0;\r\n");
                sb.Append("  ss.dwServiceSpecificExitCode = 0;\r\n");
                sb.Append("  ss.dwCheckPoint = 0;\r\n");
                sb.Append("  ss.dwWaitHint = 0;\r\n");
                sb.Append("  SetServiceStatus(ServiceHandle, &ss);\r\n");
                sb.Append("}\r\n");

                sb.Append("void __stdcall ServiceControl(int OpCode) {\r\n");
                sb.Append("  switch (OpCode) {\r\n");
                sb.Append("    case SERVICE_CONTROL_STOP:\r\n");
                sb.Append("      ServiceStatus(SERVICE_STOPPED);\r\n");
                sb.Append("      " + Program.main + "::ServiceStop();\r\n");
                sb.Append("      break;\r\n");
                sb.Append("  }\r\n");
                sb.Append("}\r\n");

                sb.Append("void __stdcall ServiceMain(int argc, char **argv) {\r\n");
                sb.Append("  ServiceHandle = RegisterServiceCtrlHandler(\"" + Program.serviceName + "\", (void (__stdcall *)(unsigned long))ServiceControl);\r\n");
                sb.Append("  ServiceStatus(SERVICE_RUNNING);\r\n");
                sb.Append(writeInvokeMain("ServiceStart"));
                sb.Append("}\r\n");
            }

            sb.Append("namespace Qt { namespace Core {\r\n");;
            sb.Append("int g_argc;\r\n");
            sb.Append("const char **g_argv;\r\n");
            sb.Append("}}\r\n");
            sb.Append("int main(int argc, const char **argv) {\r\n");
            sb.Append("Qt::Core::g_argc = argc;\r\n");
            sb.Append("Qt::Core::g_argv = argv;\r\n");
            foreach(var lib in Program.libs) {
                sb.Append("$" + lib + "_ctor();\r\n");
            }
            sb.Append("$" + Program.target + "_ctor();\r\n");
            if (!Program.service) {
                sb.Append(writeInvokeMain("Main"));
            } else {
                sb.Append("void *ServiceTable[4];\r\n");
                sb.Append("ServiceTable[0] = (void*)\"" + Program.serviceName + "\";\r\n");
                sb.Append("ServiceTable[1] = (void*)ServiceMain;\r\n");
                sb.Append("ServiceTable[2] = nullptr;\r\n");
                sb.Append("ServiceTable[3] = nullptr;\r\n");
                sb.Append("StartServiceCtrlDispatcher((LPSERVICE_TABLE_ENTRY)&ServiceTable);\r\n");  //does not return until service has been stopped
            }
            sb.Append("return 0;\r\n");
            sb.Append("}\r\n");

            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        private String writeInvokeMain(String name) {
            StringBuilder sb = new StringBuilder();
            sb.Append("Qt::QSharp::FixedArray<Qt::Core::String*> *args = new Qt::QSharp::FixedArray<Qt::Core::String*>(argc-1);\r\n");
            sb.Append("for(int a=1;a<argc;a++) {args->at(a-1) = new Qt::Core::String(argv[a]);}\r\n");
            if (!Program.debug) {
                sb.Append("try {\r\n");
            }
            sb.Append(Program.main + "::" + name + "(args);\r\n");
            if (!Program.debug) {
                sb.Append("} catch (Qt::Core::Exception *ex) {Qt::Core::Console::WriteLine($addstr(new Qt::Core::String(\"Exception caught:\"), ex->ToString()));}\r\n");
                sb.Append("catch (...) {Qt::Core::Console::WriteLine(new Qt::Core::String(\"Unknown exception thrown\"));}\r\n");
            }
            return sb.ToString();
        }

        private void writeLibraryMain() {
            StringBuilder sb = new StringBuilder();

            if (!Program.single) sb.Append("#include \"" + Program.target + ".hpp\"\r\n");

            sb.Append("namespace Qt { namespace Core {\r\n");;
            sb.Append("int g_argc;\r\n");
            sb.Append("const char **g_argv;\r\n");
            sb.Append("}}\r\n");
            sb.Append("extern \"C\" {\r\n");
            sb.Append("__declspec(dllexport)");
            sb.Append("void LibraryMain(Qt::Core::Object *obj) {\r\n");
            sb.Append(Program.main + "::LibraryMain(obj);}\r\n");
            sb.Append("}\r\n");

            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        private void writeCMakeLists() {
            StringBuilder sb = new StringBuilder();

            sb.Append("cmake_minimum_required(VERSION 3.8)\r\n");
            sb.Append("set(CMAKE_CXX_STANDARD " + Program.cxx + ")\r\n");
            if (Program.classlib) {
                sb.Append("add_definitions(-DCLASSLIB)\r\n");
            }
            if (Program.gc) {
                sb.Append("add_definitions(-DQSHARP_GC)\r\n");
            }
            if (Program.no_npe_checks) {
                sb.Append("add_definitions(-DQSHARP_NO_NPE_CHECKS)\r\n");
            }
            sb.Append("include_directories(/usr/include/qt5)\r\n");
            sb.Append("include_directories(" + Program.home + "/include)\r\n");
            sb.Append("include_directories(src)\r\n");
            if (Program.classlib) {
                sb.Append("include_directories(" + Program.home + "/include/quazip)\r\n");
            }
            sb.Append("link_directories(" + Program.home + "/lib)\r\n");
            if (Program.library) {
                sb.Append("add_library(" + Program.target + "\r\n");
                if (Program.shared) {
                    sb.Append("SHARED\r\n");
                }
                if (Program.classlib) {
                    sb.Append("quazip/qioapi.cpp quazip/quaadler32.cpp quazip/quacrc32.cpp quazip/quagzipfile.cpp quazip/quaziodevice.cpp quazip/quazip.cpp quazip/quazipdir.cpp quazip/quazipfile.cpp quazip/quazipfileinfo.cpp quazip/quazipnewinfo.cpp\r\n");
                    sb.Append("quazip/release/moc_quagzipfile.cpp quazip/release/moc_quaziodevice.cpp quazip/release/moc_quazipfile.cpp\r\n");
                    sb.Append("quazip/unzip.c quazip/zip.c\r\n");
                }
                if (!Program.single) {
                    sb.Append("cpp/ctor.cpp");
                    foreach(Source file in Program.files) {
                        sb.Append(" ");
                        sb.Append(file.cppFile);
                    }
                } else {
                    sb.Append(" cpp/" + Program.target + ".cpp");
                }
                sb.Append(")\r\n");
                sb.Append("set_property(TARGET " + Program.target + " PROPERTY POSITION_INDEPENDENT_CODE ON)\r\n");
            } else {
                sb.Append("add_executable(" + Program.target + "\r\n");
                if (!Program.single) {
                    sb.Append("cpp/main.cpp cpp/ctor.cpp");
                    foreach(Source file in Program.files) {
                        sb.Append(" ");
                        sb.Append(file.cppFile);
                    }
                } else {
                    sb.Append(" cpp/" + Program.target + ".cpp");
                }
                sb.Append(")\r\n");
            }
            if (!Program.library || Program.shared) {
                sb.Append("target_link_libraries(" + Program.target + "\r\n");
                foreach(var lib in Program.libs) {
                    sb.Append(" ");
                    sb.Append(lib);
                }
                if (Program.debug) {
                    sb.Append(" Qt5Cored Qt5Guid Qt5Networkd Qt5Widgetsd Qt5Xmld Qt5WebSocketsd Qt5Multimediad");
                } else {
                    sb.Append(" Qt5Core Qt5Gui Qt5Network Qt5Widgets Qt5Xml Qt5WebSockets Qt5Multimedia");
                }
                if (Program.msvc) {
//                    sb.Append(" msvcrt");
//                    if (Program.debug) sb.Append("d");
                } else {
                    sb.Append(" stdc++ z");
                }
                if (Program.gc) {
                    sb.Append(" gcmt-lib");  //atomic_ops ???
                }
                sb.Append(")\r\n");
            }
            if (Program.msvc) {
                sb.Append("target_compile_options(" + Program.target + " PRIVATE /bigobj)\r\n");
                sb.Append("target_compile_options(" + Program.target + " PRIVATE /wd4102)\r\n");  //$case labels not used
                if (!Program.debug) {
                    sb.Append("target_compile_options(" + Program.target + " PRIVATE /O2)\r\n");
                }
            } else {
                if (!Program.debug) {
                    sb.Append("target_compile_options(" + Program.target + " PRIVATE -O3)\r\n");
                }
            }
            if (Program.library && !Program.shared) {
                sb.Append("add_custom_command(TARGET " + Program.target + " POST_BUILD COMMAND ${CMAKE_COMMAND} -E copy cpp/" + Program.target + ".hpp " + Program.home + "/include)\r\n");
                sb.Append("add_custom_command(TARGET " + Program.target + " POST_BUILD COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_FILE:" + Program.target + "> " + Program.home + "/lib)\r\n");
            }

            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        private void closeOutput() {
            fs.Close();
        }

        private string nodeToString(SyntaxNode node) {
            if (node == null) return "null";
            return node.Kind().ToString() + ":hash=" + node.GetHashCode();
        }

        private string typeToString(INamedTypeSymbol type) {
            if (type == null) return "null";
            return type.ToString() + ":hash=" + type.GetHashCode();
        }

        private void outputFile(Source file)
        {
            Generate.file = file;
            SyntaxNode root = file.tree.GetRoot();
            IEnumerable<SyntaxNode> nodes = root.ChildNodes();
            foreach(var child in nodes) {
                topLevelNode(child);
            }
        }

        private void topLevelNode(SyntaxNode node) {
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            switch (node.Kind()) {
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                    Class topcls = new Class();
                    file.clss.Add(topcls);
                    classNode(node, topcls, NoClass, node.Kind() == SyntaxKind.InterfaceDeclaration);
                    cls = NoClass;
                    break;
                case SyntaxKind.NamespaceDeclaration:
                    string name = GetChildNode(node).ToString().Replace(".", "::");  //IdentifierName or QualifiedName
                    string org = Namespace;
                    if (Namespace.Length > 0) Namespace += "::";
                    Namespace += name.ToString().Replace(".", "::");
                    foreach(var child in nodes) {
                        topLevelNode(child);
                    }
                    Namespace = org;
                    break;
                case SyntaxKind.UsingDirective:
                    break;
                case SyntaxKind.DelegateDeclaration:
                    cls = NoClass;
                    methodNode(node, false, true, null);
                    break;
                case SyntaxKind.EnumDeclaration:
                    cls = NoClass;
                    Enum e = new Enum(GetDeclaredSymbol(node), Namespace);
                    SyntaxNode attrList = GetChildNode(node);
                    if (attrList != null && attrList.Kind() == SyntaxKind.AttributeList) {
                        attributeListNode(attrList, e);
                    }
                    cls.enums.Add(e);
                    break;
            }
        }

        private static StringBuilder convert = new StringBuilder();

        //convert reserved C++ names
        public static String ConvertName(String name) {
            if (name.Contains("::")) {
                string[] parts = name.Split("::");
                convert.Clear();
                for(int a=0;a<parts.Length;a++) {
                    if (a > 0) convert.Append("::");
                    convert.Append(ConvertName(parts[a]));
                }
                return convert.ToString();
            }
            switch (name) {
                case "near": return "$near";
                case "far": return "$far";
                case "delete": return "$delete";
                case "slots": return "$slots";
                case "BUFSIZ": return "$BUFSIZ";
                case "TRUE": return "$TRUE";
                case "FALSE": return "$FALSE";
            }
            return name;
        }

        public static String ConvertOperatorName(String name) {
            return "$" + name;
/*
            switch (name) {
                case "op_LessThan": return "<";
                case "op_GreaterThan": return ">";
                case "op_LessThanOrEqual": return "<=";
                case "op_GreaterThanOrEqual": return ">=";
                case "op_Equality": return "==";
            }
            return "??";
*/
        }

        private void classNode(SyntaxNode node, Class inner, Class outter, bool Interface) {
            cls = inner;
            cls.node = node;
            cls.model = file.model;
            cls.fullname = outter.fullname;
            if (cls.fullname.Length > 0) {
                cls.fullname += "::";
            }
            cls.name = ConvertName(file.model.GetDeclaredSymbol(node).Name);
            cls.fullname += cls.name;
            cls.Namespace = Namespace;
            if (cls.Namespace.Length > 0) {
                cls.nsfullname = cls.Namespace + "::";
            }
            cls.nsfullname += cls.fullname;
            cls.isInterface = Interface;
            init = new Method();
            init.cls = cls;
            init.type.set("void");
            init.type.cls = cls;
            init.type.isPrimative = true;
            init.type.isPublic = true;
            init.name = "$init";
            init.type.setTypes();
            cls.methods.Add(init);
            getFlags(cls, file.model.GetDeclaredSymbol(node));
            foreach(var child in node.ChildNodes()) {
                switch (child.Kind()) {
                    case SyntaxKind.FieldDeclaration:
                        if (!cls.omitFields) fieldNode(child);
                        break;
                    case SyntaxKind.PropertyDeclaration:
                        if (!cls.omitFields) propertyNode(child);
                        break;
                    case SyntaxKind.ConstructorDeclaration:
                        if (!cls.omitConstructors) ctorNode(child);
                        break;
                    case SyntaxKind.DestructorDeclaration:
                        if (!cls.omitMethods) methodNode(child, true, false, null);
                        break;
                    case SyntaxKind.MethodDeclaration:
                        if (!cls.omitMethods) methodNode(child, false, false, null);
                        break;
                    case SyntaxKind.DelegateDeclaration:
                        if (!cls.omitMethods) methodNode(child, false, true, null);
                        break;
                    case SyntaxKind.BaseList:
                        baseListNode(child);
                        break;
                    case SyntaxKind.AttributeList:
                        attributeListNode(child, cls);
                        break;
                    case SyntaxKind.EnumDeclaration:
                        Enum e = new Enum(GetDeclaredSymbol(child), cls.Namespace);
                        SyntaxNode attrList = GetChildNode(child);
                        if (attrList != null && attrList.Kind() == SyntaxKind.AttributeList) {
                            attributeListNode(attrList, e);
                        }
                        cls.enums.Add(e);
                        break;
                    case SyntaxKind.TypeParameterList:
                        typeParameterListNode(child);
                        break;
                    case SyntaxKind.InterfaceDeclaration:
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.StructDeclaration:
                        Class _otter = cls;
                        Class _inner = new Class();
                        _otter.inners.Add(_inner);
                        _inner.outter = _otter;
                        classNode(child, _inner, _otter, node.Kind() == SyntaxKind.InterfaceDeclaration);
                        cls = _otter;
                        break;
                    case SyntaxKind.ConversionOperatorDeclaration:
                        //TODO
                        break;
                    case SyntaxKind.OperatorDeclaration:
                        if (!cls.omitMethods) methodNode(child, false, false, null, true);
                        break;
                    default:
                        Console.WriteLine("Unknown Declaration:" + child.ToString());
                        Environment.Exit(1);
                        break;
                }
            }
            if (cls.nsfullname != "Qt::Core::Object") {
                if (cls.bases.Count == 0 && !cls.isInterface) {
                    cls.bases.Add(new Type(null, "Qt::Core::Object"));
                }
                cls.addUsage("Qt::Core::Object");
            }
        }

        private void typeParameterListNode(SyntaxNode node) {
            cls.isGeneric = true;
            foreach(var child in node.ChildNodes()) {
                switch (child.Kind()) {
                    case SyntaxKind.TypeParameter:
                        cls.GenericArgs.Add(new Type(child, GetDeclaredSymbol(child)));
                        break;
                }
            }
        }

        private void baseListNode(SyntaxNode node) {
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach(var child in nodes) {
                switch (child.Kind()) {
                    case SyntaxKind.SimpleBaseType:
                        SyntaxNode baseNode = GetChildNode(child);
                        Type baseType = null;
                        switch (baseNode.Kind()) {
                            case SyntaxKind.IdentifierName:
                            case SyntaxKind.QualifiedName:
                            case SyntaxKind.GenericName:
                                baseType = new Type(baseNode);
                                break;
                            default:
                                Console.WriteLine("unknown baseList node:" + baseNode.Kind());
                                break;
                        }
                        String baseName = baseType.GetSymbol();
                        if (baseName == "System::Exception") continue;
                        if (baseName == "System::Attribute") continue;
                        if (isClass(baseNode))
                            cls.bases.Add(baseType);
                        else
                            cls.ifaces.Add(baseType);
                        cls.addUsage(baseType.GetSymbol());
                        break;
                }
            }
        }

        private void getFlags(Flags flags, ISymbol symbol) {
            if (symbol == null) return;
            flags.isStatic = symbol.IsStatic;
            flags.isAbstract = symbol.IsAbstract;
            flags.isVirtual = symbol.IsVirtual;
            flags.isExtern = symbol.IsExtern;
            flags.isOverride = symbol.IsOverride;
            flags.isDefinition = symbol.IsDefinition;
            flags.isSealed = symbol.IsSealed;
            Accessibility access = symbol.DeclaredAccessibility;
            if (access == Accessibility.Private) flags.isPrivate = true;
            if (access == Accessibility.Protected) flags.isProtected = true;
            if (access == Accessibility.Public) flags.isPublic = true;
        }

        private void fieldNode(SyntaxNode node) {
            field = new Field();
            field.cls = cls;
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach(var child in nodes) {
                switch (child.Kind()) {
                    case SyntaxKind.AttributeList:
                        if (attributeListNode(child, field)) return;
                        break;
                    case SyntaxKind.VariableDeclaration:
                        field.variables = variableDeclaration(child, field, true);
                        foreach(var v in field.variables) {
                            if (v.equals != null) fieldEquals(v);
                        }
                        break;
                }
            }
            cls.fields.Add(field);
        }

        private void fieldEquals(Variable v) {
            method = v.method;
            if (field.isStatic) {
                if (cls.Namespace.Length > 0) {
                    method.Append(cls.Namespace);
                    method.Append("::");
                }
            }
            method.Append(cls.fullname);
            method.Append("::");
            method.Append(v.name);
            method.Append(" = ");
            SyntaxNode equalsChild = GetChildNode(v.equals);
            if (equalsChild.Kind() == SyntaxKind.ArrayInitializerExpression) {
                arrayInitNode(equalsChild, field.GetTypeDeclaration(false), field.arrays);
            } else {
                expressionNode(equalsChild);
            }
            method.Append(";\r\n");
        }

        private void propertyNode(SyntaxNode node) {
            // type, AccessorList -> {GetAccessorDeclaration, SetAccessorDeclaration}
            // type, ExplicitInterfaceSpecifier -> {...}
            field = new Field();
            field.cls = cls;
            field.isProperty = true;
            Variable v = new Variable();
            field.variables.Add(v);  //property decl can only have 1 variable
            ISymbol symbol = file.model.GetDeclaredSymbol(node);
            v.name = symbol.Name;
            field.isPublic = true;
            variableDeclaration(node, field);
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach(var child in nodes) {
                switch (child.Kind()) {
                    case SyntaxKind.AccessorList:
                        foreach(var _etter in child.ChildNodes()) {
                            switch (_etter.Kind()) {
                                case SyntaxKind.GetAccessorDeclaration:
                                    methodNode(_etter, false, false, "$get_" + v.name);
                                    method.type.CopyType(field);
                                    method.type.CopyFlags(field);
                                    method.type.setTypes();
                                    method.type.isVirtual = true;
                                    if (cls.isAbstract) method.type.isAbstract = true;
                                    if (method.src.Length == 0 && !cls.isAbstract) {
                                        method.Append("{return " + v.name + ".$value;}");
                                    }
                                    field.get_Property = true;
                                    break;
                                case SyntaxKind.SetAccessorDeclaration:
                                    methodNode(_etter, false, false, "$set_" + v.name);
                                    Argument arg = new Argument();
                                    arg.type = field;
                                    arg.name.name = "value";
                                    method.args.Add(arg);
                                    method.type.set("void");
                                    method.type.setTypes();
                                    method.type.isVirtual = true;
                                    if (cls.isAbstract) method.type.isAbstract = true;
                                    if (method.src.Length == 0 && !cls.isAbstract) {
                                        method.Append("{" + v.name + ".$value = value;}");
                                    }
                                    field.set_Property = true;
                                    break;
                            }
                        }
                        break;
                    case SyntaxKind.ExplicitInterfaceSpecifier:
                        //TODO
                        break;
                }
            }
            cls.fields.Add(field);
            //add default getter/setter
            if (!field.get_Property) {
                method = new Method();
                method.cls = cls;
                method.name = "$get_" + v.name;
                method.type.CopyType(field);
                method.type.CopyFlags(field);
                method.type.setTypes();
                method.type.isVirtual = true;
                if (cls.isAbstract) method.type.isAbstract = true;
                method.Append("{return " + v.name + ".$value;}");
                cls.methods.Add(method);
            }
            if (!field.set_Property) {
                method = new Method();
                method.cls = cls;
                method.name = "$set_" + v.name;
                Argument arg = new Argument();
                arg.type = field;
                arg.name.name = "value";
                method.args.Add(arg);
                method.type.set("void");
                method.type.setTypes();
                method.type.isVirtual = true;
                if (cls.isAbstract) method.type.isAbstract = true;
                method.Append("{" + v.name + ".$value = value;}");
                cls.methods.Add(method);
            }
            v.method.Append(v.name + ".Get([=] () {return $get_" + v.name + "();});\r\n");
            v.method.Append(v.name + ".Set([=] (" + field.GetTypeDeclaration() + " t) {$set_" + v.name + "(t);});\r\n");
        }

        //method or field attributes (returns true if ommited)
        private bool attributeListNode(SyntaxNode node, Type type) {
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach(var child in nodes) {
                switch (child.Kind()) {
                    case SyntaxKind.Attribute:
                        SyntaxNode attrNode = GetChildNode(child);
                        SyntaxNode attrArgList = GetChildNode(child, 2);  //AttributeArgumentList
                        switch (attrNode.Kind()) {
                            case SyntaxKind.IdentifierName:
                            case SyntaxKind.QualifiedName:
                                ITypeSymbol symbol = file.model.GetTypeInfo(attrNode).Type;
                                if (symbol == null) break;
                                String name = symbol.ToString().Replace(".","::");
                                switch (name) {
                                    case "Qt::QSharp::CPPOmitField":
                                    case "Qt::QSharp::CPPOmitMethod":
                                    case "Qt::QSharp::CPPOmitConstructor":
                                        return true;
                                    case "Qt::QSharp::CPPOmitBody":
                                        method.omitBody = true;
                                        break;
                                    case "Qt::QSharp::CPPVersion": {
                                        SyntaxNode arg = GetChildNode(attrArgList);  //AttributeArgument
                                        SyntaxNode str = GetChildNode(arg);  //StringLiteralExpression
                                        String value = file.model.GetConstantValue(str).Value.ToString();
                                        method.version = value;
                                        break;
                                    }
                                    case "Qt::QSharp::CPPReplaceArgs": {
                                        SyntaxNode arg = GetChildNode(attrArgList);  //AttributeArgument
                                        SyntaxNode str = GetChildNode(arg);  //StringLiteralExpression
                                        String value = file.model.GetConstantValue(str).Value.ToString();
                                        method.replaceArgs = value;
                                        method.type.isPublic = true;
                                        method.type.isPrivate = false;
                                        break;
                                    }
                                    case "Qt::Core::AutoMemoryPool": {
                                        method.automemorypool = true;
                                        if (Program.gc) {
                                            Console.WriteLine("Error:[AutoMemoryPool] can not be used with --gc option");
                                            Environment.Exit(1);
                                        }
                                        break;
                                    }
                                }
                                break;
                        }
                        break;
                }
            }
            return false;
        }

        //class attributes
        private void attributeListNode(SyntaxNode node, Class cls) {
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach(var child in nodes) {
                switch (child.Kind()) {
                    case SyntaxKind.Attribute:
                        SyntaxNode attrNode = GetChildNode(child);
                        switch (attrNode.Kind()) {
                            case SyntaxKind.IdentifierName:
                            case SyntaxKind.QualifiedName:
                                ITypeSymbol symbol = file.model.GetTypeInfo(attrNode).Type;
                                if (symbol == null) break;
                                String name = symbol.ToString().Replace(".","::");
                                SyntaxNode attrArgList = GetChildNode(child, 2);  //AttributeArgumentList
                                switch (name) {
                                    case "Qt::QSharp::CPPClass": {
                                        SyntaxNode arg = GetChildNode(attrArgList);  //AttributeArgument
                                        SyntaxNode str = GetChildNode(arg);  //StringLiteralExpression
                                        String value = file.model.GetConstantValue(str).Value.ToString();
                                        cls.cpp = value + "\r\n";
                                        break;
                                    }
                                    case "Qt::QSharp::CPPExtends": {
                                        IEnumerable<SyntaxNode> args = attrArgList.DescendantNodes();
                                        foreach(var arg in args) {
                                            if (arg.Kind() != SyntaxKind.StringLiteralExpression) continue;
                                            String value = file.model.GetConstantValue(arg).Value.ToString();
                                            cls.cppbases.Add(value);
                                        }
                                        break;
                                    }
                                    case "Qt::QSharp::CPPForward": {
                                        IEnumerable<SyntaxNode> args = attrArgList.DescendantNodes();
                                        foreach(var arg in args) {
                                            if (arg.Kind() != SyntaxKind.StringLiteralExpression) continue;
                                            String value = file.model.GetConstantValue(arg).Value.ToString();
                                            cls.forward = value;
                                        }
                                        break;
                                    }
                                    case "Qt::QSharp::CPPConstructorArgs": {
                                        SyntaxNode arg = GetChildNode(attrArgList);  //AttributeArgument
                                        SyntaxNode str = GetChildNode(arg);  //StringLiteralExpression
                                        String value = file.model.GetConstantValue(str).Value.ToString();
                                        cls.ctorArgs = value;
                                        break;
                                    }
                                    case "Qt::QSharp::CPPNonClassCPP": {
                                        SyntaxNode arg = GetChildNode(attrArgList);  //AttributeArgument
                                        SyntaxNode str = GetChildNode(arg);  //StringLiteralExpression
                                        String value = file.model.GetConstantValue(str).Value.ToString();
                                        cls.nonClassCPP = value;
                                        break;
                                    }
                                    case "Qt::QSharp::CPPNonClassHPP": {
                                        SyntaxNode arg = GetChildNode(attrArgList);  //AttributeArgument
                                        SyntaxNode str = GetChildNode(arg);  //StringLiteralExpression
                                        String value = file.model.GetConstantValue(str).Value.ToString();
                                        cls.nonClassHPP = value;
                                        break;
                                    }
                                    case "Qt::QSharp::CPPOmitFields": {
                                        cls.omitFields = true;
                                        break;
                                    }
                                    case "Qt::QSharp::CPPOmitMethods": {
                                        cls.omitMethods = true;
                                        break;
                                    }
                                    case "Qt::QSharp::CPPOmitBodies": {
                                        cls.omitBodies = true;
                                        break;
                                    }
                                    case "Qt::QSharp::CPPOmitConstructors": {
                                        cls.omitConstructors = true;
                                        break;
                                    }
                                    case "Qt::QSharp::CPPAddUsage": {
                                        IEnumerable<SyntaxNode> args = attrArgList.DescendantNodes();
                                        foreach(var arg in args) {
                                            if (arg.Kind() != SyntaxKind.StringLiteralExpression) continue;
                                            String value = file.model.GetConstantValue(arg).Value.ToString();
                                            cls.addUsage(value);
                                        }
                                        break;
                                    }
                                }
                                break;
                        }
                        break;
                }
            }
        }

        //enum attributes
        private void attributeListNode(SyntaxNode node, Enum e) {
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach(var child in nodes) {
                switch (child.Kind()) {
                    case SyntaxKind.Attribute:
                        SyntaxNode attrNode = GetChildNode(child);
                        switch (attrNode.Kind()) {
                            case SyntaxKind.IdentifierName:
                            case SyntaxKind.QualifiedName:
                                ITypeSymbol symbol = file.model.GetTypeInfo(attrNode).Type;
                                if (symbol == null) break;
                                String name = symbol.ToString().Replace(".","::");
                                SyntaxNode attrArgList = GetChildNode(child, 2);  //AttributeArgumentList
                                switch (name) {
                                    case "Qt::QSharp::CPPEnum": {
                                        IEnumerable<SyntaxNode> args = attrArgList.DescendantNodes();
                                        foreach(var arg in args) {
                                            if (arg.Kind() != SyntaxKind.StringLiteralExpression) continue;
                                            String value = file.model.GetConstantValue(arg).Value.ToString();
                                            e.qtType.Add(value);
                                        }
                                        break;
                                    }
                                }
                                break;
                        }
                        break;
                }
            }
        }

        private List<Variable> variableDeclaration(SyntaxNode node, Type type, bool field = false) {
            List<Variable> vars = new List<Variable>();
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach(var child in nodes) {
                switch (child.Kind()) {
                    case SyntaxKind.ArrayType:
                        type.isArray = true;
                        variableDeclaration(child, type);
                        break;
                    case SyntaxKind.ArrayRankSpecifier:
                        //assume it's OmittedArraySizeExpression
                        type.arrays++;
                        if (type.arrays > 3) {
                            Console.WriteLine("Error:Array Dimensions not supported:" + type.arrays);
                            WriteFileLine(node);
                            errors++;
                        }
                        break;
                    case SyntaxKind.PointerType:
                        type.isPtr = true;
                        variableDeclaration(child, type);
                        break;
                    case SyntaxKind.PredefinedType:
                    case SyntaxKind.IdentifierName:
                    case SyntaxKind.QualifiedName:
                    case SyntaxKind.GenericName:
                        type.set(child);
                        type.setTypes();
                        break;
                    case SyntaxKind.VariableDeclarator:
                        getFlags(type, file.model.GetDeclaredSymbol(child));
                        ISymbol symbol2 = file.model.GetDeclaredSymbol(child);
                        Variable var = new Variable();
                        vars.Add(var);
                        if (symbol2 != null) {
                            var.name = ConvertName(symbol2.Name);
                        }
                        var.equals = GetChildNode(child);
                        break;
                }
            }
            return vars;
        }

        public void ctorNode(SyntaxNode node) {
            method = new Method();
            cls.methods.Add(method);
            method.cls = cls;
            method.name = cls.name;
            method.type.set("");
            method.type.cls = cls;
            method.type.isPrimative = true;
            method.ctor = true;
            cls.hasctor = true;
            if (node != null) {
                getFlags(method.type, file.model.GetDeclaredSymbol(node));
                IEnumerable<SyntaxNode> nodes = node.ChildNodes();
                //nodes : parameter list, [baseCtor], block
                foreach(var child in nodes) {
                    switch (child.Kind()) {
                        case SyntaxKind.AttributeList:
                            if (attributeListNode(child, method.type)) return;
                            break;
                        case SyntaxKind.ParameterList:
                            parameterListNode(child);
                            break;
                        case SyntaxKind.BaseConstructorInitializer:
                            SyntaxNode argList = GetChildNode(child);
                            method.Append(cls.bases[0].GetCPPType());
                            method.Append("(");
                            outArgList(argList);
                            method.Append(")\r\n");
                            method.basector = method.src.ToString();
                            method.src.Length = 0;
                            break;
                        case SyntaxKind.Block:
                            blockNode(child, true, false, true);
                            break;
                    }
                }
            } else {
                method.src.Append("{$init();}");
            }
            method.type.setTypes();
            if (method.cls.isAbstract) return;
        }

        private void methodNode(SyntaxNode node, bool dtor, bool isDelegate, String name, bool isOperator = false) {
            method = new Method();
            method.cls = cls;
            method.type.cls = cls;
            method.isOperator = isOperator;
            if (isDelegate) {
                method.isDelegate = true;
                method.Namespace = Namespace;
            }
            if (name != null) {
                method.name = name;
            } else {
                method.node = node;
                method.symbol = file.model.GetDeclaredSymbol(node);
                if (dtor) {
                    method.name = "~" + cls.name;
                    method.type.set("");
                } else {
                    if (isOperator) {
                        method.name = ConvertOperatorName(method.symbol.Name);
                    } else {
                        method.name = ConvertName(method.symbol.Name);
                    }
                }
            }
            getFlags(method.type, file.model.GetDeclaredSymbol(node));
            if (dtor) {
                method.type.isProtected = false;
                method.type.isVirtual = true;
                method.type.isPublic = true;
            }
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            //nodes : [return type], parameter list, block
            foreach(var child in nodes) {
                switch (child.Kind()) {
                    case SyntaxKind.PredefinedType:
                    case SyntaxKind.IdentifierName:
                    case SyntaxKind.QualifiedName:
                    case SyntaxKind.GenericName:
                        method.type.set(child);
                        method.type.setTypes();
                        break;
                    case SyntaxKind.ParameterList:
                        parameterListNode(child);
                        break;
                    case SyntaxKind.Block:
                        blockNode(child, true);
                        break;
                    case SyntaxKind.AttributeList:
                        if (attributeListNode(child, method.type)) return;
                        break;
                    case SyntaxKind.ArrayType:
                        method.type.isArray = true;
                        foreach(var arrayType in child.ChildNodes()) {
                            parameterNode(arrayType, method.type);
                        }
                        break;
                }
            }
            method.type.setTypes();
            if (isOperator) {
                OpClass.methods.Add(method);
            } else {
                cls.methods.Add(method);
            }
        }

        private void parameterListNode(SyntaxNode node) {
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach(var param in nodes) {
                switch (param.Kind()) {
                    case SyntaxKind.Parameter:
                        SyntaxNode par = GetChildNode(param, 1);
                        Type type = new Type();
                        if (par != null) {
                            parameterNode(par, type);
                        } else {
                            type.set("auto");
                        }
                        Argument arg = new Argument();
                        arg.name.name = ConvertName(file.model.GetDeclaredSymbol(param).Name.Replace(".", "::"));
                        arg.type = type;
                        method.args.Add(arg);
                        SyntaxNode equals = GetChildNode(param, 2);
                        if (equals != null && equals.Kind() == SyntaxKind.EqualsValueClause) {
                            Method orgmethod = method;
                            method = arg.name.method;
                            expressionNode(GetChildNode(equals));
                            method = orgmethod;
                        }
                        break;
                }
            }
        }

        private void parameterNode(SyntaxNode node, Type type) {
            switch (node.Kind()) {
                case SyntaxKind.PredefinedType:
                case SyntaxKind.IdentifierName:
                case SyntaxKind.QualifiedName:
                case SyntaxKind.GenericName:
                    type.set(node);
                    type.setTypes();
                    break;
                case SyntaxKind.ArrayType:
                    type.isArray = true;
                    parameterNode(GetChildNode(node), type);
                    IEnumerable<SyntaxNode> ranks = node.DescendantNodes();
                    foreach(var rank in ranks) {
                        if (rank.Kind() == SyntaxKind.ArrayRankSpecifier) {
                            type.arrays++;
                            if (type.arrays > 3) {
                                Console.WriteLine("Error:Array Dimensions not supported:" + type.arrays);
                                WriteFileLine(node);
                                errors++;
                            }
                        }
                    }
                    break;
                case SyntaxKind.ArrayRankSpecifier:
                    type.arrays++;
                    if (type.arrays > 3) {
                        Console.WriteLine("Error:Array Dimensions not supported:" + type.arrays);
                        WriteFileLine(node);
                        errors++;
                    }
                    break;
                default:
                    Console.WriteLine("Unknown arg type:" + node.Kind());
                    break;
            }
        }

        private void blockNode(SyntaxNode node, bool top = false, bool throwFinally = false, bool ctor = false) {
            method.Append("{\r\n");
            if (ctor) {
                method.Append("$init();\r\n");
            }
            if (top && method.automemorypool) {
                method.Append("std::MemoryPool $pool;\r\n");
                if (method.type.isObject) {
                    method.Append(method.type.GetTypeDeclaration());
                    method.Append(" $ret;\r\n");
                }
            }
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach(var child in nodes) {
                statementNode(child);
            }
            if (throwFinally) {
                method.Append("throw new Qt::QSharp::FinallyException();");
            }
            method.Append("}\r\n");
        }

        private void statementNode(SyntaxNode node, bool top = false, bool throwFinally = false) {
            switch (node.Kind()) {
                case SyntaxKind.Block:
                    blockNode(node, top, throwFinally);
                    break;
                case SyntaxKind.ExpressionStatement:
                    expressionNode(GetChildNode(node));
                    method.Append(";\r\n");
                    break;
                case SyntaxKind.LocalDeclarationStatement:
                    expressionNode(GetChildNode(node));
                    method.Append(";\r\n");
                    break;
                case SyntaxKind.ReturnStatement:
                    SyntaxNode returnValue = GetChildNode(node);
                    bool useValue = method.type.isObject && method.automemorypool && returnValue != null;
                    if (useValue) {
                        method.Append("$ret = ");
                    } else {
                        method.Append("return ");
                    }
                    if (returnValue != null) expressionNode(returnValue);
                    method.Append(";\r\n");
                    if (useValue) {
                        method.Append("$ret->Detach();return $ret;\r\n");
                    }
                    break;
                case SyntaxKind.WhileStatement:
                    //while (expression) statement
                    method.Append("while (");
                    expressionNode(GetChildNode(node, 1));
                    method.Append(")");
                    statementNode(GetChildNode(node, 2));
                    break;
                case SyntaxKind.ForStatement:
                    //for(initializers;condition;incrementors) statement
                    ForStatementSyntax ffs = (ForStatementSyntax)node;
                    bool HasDecl = ffs.Declaration != null;
                    int InitCount = ffs.Initializers.Count;
                    if (HasDecl) InitCount++;
                    bool HasCond = ffs.Condition != null;
                    int IncreCount = ffs.Incrementors.Count;
                    method.Append("for(");
                    int pos = 1;
                    for(int idx=0;idx<InitCount;idx++) {
                        if (idx > 0) method.Append(",");
                        expressionNode(GetChildNode(node, pos++));
                    }
                    method.Append(";");
                    if (HasCond) {
                        expressionNode(GetChildNode(node, pos++));
                    }
                    method.Append(";");
                    for(int idx=0;idx<IncreCount;idx++) {
                        if (idx > 0) method.Append(",");
                        expressionNode(GetChildNode(node, pos++));
                    }
                    method.Append(")");
                    statementNode(GetChildNode(node, pos));
                    break;
                case SyntaxKind.ForEachStatement:
                    //foreach(var item in items) {}
                    //node(item) -> type, items, block {}
                    SyntaxNode foreachItem = node;
                    String foreachName = file.model.GetDeclaredSymbol(foreachItem).ToString();
                    SyntaxNode foreachTypeNode = GetChildNode(node, 1);
                    ISymbol foreachTypeNodeSymbol = file.model.GetSymbolInfo(foreachTypeNode).Symbol;
                    Type foreachType = new Type(foreachTypeNode);
                    SyntaxNode foreachItems = GetChildNode(node, 2);
                    SyntaxNode foreachBlock = GetChildNode(node, 3);
                    String enumID = "$enum_" + cls.enumCnt++;
                    method.Append("{");
                    method.Append(foreachType.GetTypeDeclaration());  //var type
                    method.Append(" ");
                    method.Append(foreachName);  //var name : item
                    method.Append(";\r\n");
                    method.Append("Qt::Core::IEnumerator");
                    method.Append(foreachType.GetTypeDeclaration());  //var type
                    method.Append("> *" + enumID + " = ");
                    expressionNode(foreachItems);  //items
                    method.Append("->GetEnumerator();\r\n");
                    method.Append("while (");
                    method.Append(enumID + "->MoveNext()) {\r\n");
                    method.Append(foreachName + " = ");  //var name : item =
                    method.Append(enumID + "->$get_Current();\r\n");
                    statementNode(foreachBlock);
                    method.Append("}}\r\n");
                    break;
                case SyntaxKind.DoStatement:
                    //do statement/block while (expression)
                    method.Append("do ");
                    statementNode(GetChildNode(node, 1));
                    method.Append(" while (");
                    expressionNode(GetChildNode(node, 2));
                    method.Append(");\r\n");
                    break;
                case SyntaxKind.IfStatement:
                    //if (expression) statement [else statement]
                    method.Append("if (");
                    expressionNode(GetChildNode(node, 1));
                    method.Append(")");
                    statementNode(GetChildNode(node, 2));
                    SyntaxNode elseClause = GetChildNode(node, 3);
                    if (elseClause != null && elseClause.Kind() == SyntaxKind.ElseClause) {
                        method.Append(" else ");
                        statementNode(GetChildNode(elseClause, 1));
                    }
                    break;
                case SyntaxKind.TryStatement:
                    //statement CatchClause ... FinallyClause
                    int cnt = GetChildCount(node);
                    bool hasFinally = false;
                    for(int a=2;a<=cnt;a++) {
                        SyntaxNode child = GetChildNode(node, a);
                        if (child.Kind() == SyntaxKind.FinallyClause) {
                            hasFinally = true;
                        }
                    }
                    if (hasFinally) {
                        method.Append("try {");
                    }
                    method.Append("try ");
                    SyntaxNode tryBlock = GetChildNode(node, 1);
                    if (tryBlock.Kind() == SyntaxKind.Block) {
                        blockNode(tryBlock, false, hasFinally);
                    } else {
                        statementNode(tryBlock);
                    }
                    for(int a=2;a<=cnt;a++) {
                        SyntaxNode child = GetChildNode(node, a);
                        switch (child.Kind()) {
                            case SyntaxKind.CatchClause:
                                int cc = GetChildCount(child);
                                if (cc == 2) {
                                    //catch (Exception ?)
                                    SyntaxNode catchDecl = GetChildNode(child, 1);
                                    method.Append(" catch(");
                                    expressionNode(GetChildNode(catchDecl));  //exception type
                                    method.Append(" *");
                                    method.Append(file.model.GetDeclaredSymbol(catchDecl).Name);  //exception variable name
                                    method.Append(")");
                                    SyntaxNode catchBlock = GetChildNode(child, 2);
                                    statementNode(catchBlock, false, hasFinally);
                                } else {
                                    //catch all
                                    method.Append(" catch (...)");
                                    SyntaxNode catchBlock = GetChildNode(child, 1);
                                    statementNode(catchBlock, false, hasFinally);
                                }
                                break;
                            case SyntaxKind.FinallyClause:
                                method.Append("} catch(Qt::QSharp::FinallyException *$finally" + cls.finallyCnt++ + ") ");
                                statementNode(GetChildNode(child));
                                break;
                        }
                    }
                    break;
                case SyntaxKind.ThrowStatement:
                    int tc = GetChildCount(node);
                    if (tc == 1) {
                        method.Append("throw ");
                        expressionNode(GetChildNode(node));
                    } else {
                        method.Append("std::rethrow_exception(std::current_exception())");
                    }
                    method.Append(";");
                    break;
                case SyntaxKind.FixedStatement:
                    method.inFixedBlock = true;
                    method.Append("{\r\n");
                    foreach(var child in node.ChildNodes()) {
                        switch (child.Kind()) {
                            case SyntaxKind.VariableDeclaration:
                                Type type = new Type();
                                List<Variable> vars = variableDeclaration(child, type);
                                method.Append(type.GetTypeDeclaration());
                                method.Append(" ");
                                foreach(var variable in vars) {
                                    method.Append(variable.name);
                                    SyntaxNode equals = variable.equals;
                                    if (equals != null) {
                                        method.Append(" = ");
                                        SyntaxNode equalsChild = GetChildNode(equals);
                                        if (equalsChild.Kind() == SyntaxKind.ArrayInitializerExpression) {
                                            arrayInitNode(equalsChild, type.GetTypeDeclaration(false), type.arrays);
                                        } else {
                                            expressionNode(equalsChild);
                                        }
                                    }
                                    method.Append(".get()->data();\r\n");
                                }
                                break;
                            case SyntaxKind.Block:
                                blockNode(child);
                                break;
                        }
                    }
                    method.Append("}\r\n");
                    method.inFixedBlock = false;
                    break;
                case SyntaxKind.LockStatement:
                    //lock, block
                    SyntaxNode lockId = GetChildNode(node, 1);
                    string lockIdName = GetTypeName(lockId);
                    if (lockIdName != "Qt::Core::ThreadLock") {
                        Console.WriteLine("Error:lock {} must use Qt.Core.ThreadLock (Type=" + lockIdName + " id=" + GetSymbol(lockId) + ")");
                        WriteFileLine(lockId);
                        errors++;
                        break;
                    }
                    SyntaxNode lockBlock = GetChildNode(node, 2);
                    string holder = "$lock" + cls.lockCnt++;
                    method.Append("{$LockHolder " + holder + "(");
                    expressionNode(lockId);
                    method.Append(");"); // + holder + ".Condition();" + holder + ".Signal())");
                    blockNode(lockBlock);
                    method.Append("}\r\n");
                    break;
                case SyntaxKind.SwitchStatement:
                    // var, [SwitchSection...]
                    SyntaxNode var = GetChildNode(node);
                    if (isString(var)) {
                        switchString(node);
                        break;
                    }
                    method.Append("switch (");
                    expressionNode(var);
                    method.Append(") {\r\n");
                    method.currentSwitch++;
                    method.switchIDs[method.currentSwitch] = method.nextSwitchID++;
                    int caseIdx = 0;
                    bool block = false;
                    foreach(var section in node.ChildNodes()) {
                        if (section.Kind() != SyntaxKind.SwitchSection) continue;
                        foreach(var child in section.ChildNodes()) {
                            switch (child.Kind()) {
                                case SyntaxKind.CaseSwitchLabel:
                                    method.Append("case ");
                                    method.Append(constantNode(GetChildNode(child), false));
                                    method.Append(":\r\n");
                                    method.Append("$case_" + method.switchIDs[method.currentSwitch] + "_" + caseIdx++);
                                    method.Append(":\r\n");
                                    break;
                                case SyntaxKind.DefaultSwitchLabel:
                                    method.Append("default:\r\n");
                                    method.Append("$default_" + method.switchIDs[method.currentSwitch]);
                                    method.Append(":\r\n");
                                    break;
                                default:
                                    if (!block) {
                                        method.Append("{\r\n");
                                        block = true;
                                    }
                                    statementNode(child);
                                    break;
                            }
                        }
                        method.Append("}\r\n");
                        block = false;
                    }
                    method.currentSwitch--;
                    method.Append("}\r\n");
                    break;
                case SyntaxKind.BreakStatement:
                    method.Append("break;\r\n");
                    break;
                case SyntaxKind.ContinueStatement:
                    method.Append("continue;\r\n");
                    break;
                case SyntaxKind.GotoCaseStatement:
                    String value = file.model.GetConstantValue(GetChildNode(node)).Value.ToString();
                    String index = FindCase(node, value);
                    method.Append("goto $case_" + + method.switchIDs[method.currentSwitch] + "_" + index + ";\r\n");
                    break;
                case SyntaxKind.GotoDefaultStatement:
                    method.Append("goto $default_" + method.switchIDs[method.currentSwitch] + ";\r\n");
                    break;
                default:
                    Console.WriteLine("Error:Statement not supported:" + node.Kind());
                    WriteFileLine(node);
                    Environment.Exit(0);
                    break;
            }
        }

        public String FindCase(SyntaxNode node, String value) {
            //first find parent SwitchStatement
            while (node.Kind() != SyntaxKind.SwitchStatement) {
                node = node.Parent;
            }
            int caseIdx = 0;
            //interate over SwitchSection/CaseSwitchLabel to find matching value
            foreach(var section in node.ChildNodes()) {
                if (section.Kind() != SyntaxKind.SwitchSection) continue;
                SyntaxNode caseSwitch = GetChildNode(section);
                if (caseSwitch.Kind() != SyntaxKind.CaseSwitchLabel) continue;
                SyntaxNode caseValue = GetChildNode(caseSwitch);
                String caseConst = file.model.GetConstantValue(caseValue).Value.ToString();
                if (caseConst == value) return "" + caseIdx;
                caseIdx++;
            }
            Console.WriteLine("Failed to find goto case target");
            Environment.Exit(0);
            return null;
        }

        public void switchString(SyntaxNode node) {
            //SwitchStatement -> [SwitchSection -> [CaseSwitchLabel, [Default] ...] [Statements...] ...]
            SyntaxNode var = GetChildNode(node);
            String ssid = "$ss_" + cls.switchStringCnt++;
            method.Append("bool " + ssid + " = false;\r\n");  //set to true if case block used (else default is used)
            method.Append("while (true) {\r\n");
            SyntaxNode defaultSection = null;
            foreach(var section in node.ChildNodes()) {
                if (section.Kind() != SyntaxKind.SwitchSection) continue;
                if (hasDefault(section)) {
                    defaultSection = section;
                } else {
                    switchStringSection(var, section, ssid);
                }
            }
            if (defaultSection != null) {
                switchStringSection(var, defaultSection, ssid);
            }
            method.Append("}\r\n");  //end of while loop
        }

        public bool hasDefault(SyntaxNode section) {
            foreach(var child in section.ChildNodes()) {
                if (child.Kind() == SyntaxKind.DefaultSwitchLabel) return true;
            }
            return false;
        }

        public void switchStringSection(SyntaxNode var, SyntaxNode section, String ssid) {
            bool first = true;
            bool statements = false;
            foreach(var child in section.ChildNodes()) {
                switch (child.Kind()) {
                    case SyntaxKind.CaseSwitchLabel:
                        if (first) {
                            method.Append("if (");
                            first = false;
                        } else {
                            method.Append("||");
                        }
                        method.Append("(");
                        expressionNode(var);
                        method.Append("->Equals(");
                        expressionNode(GetChildNode(child));
                        method.Append("))");
                        break;
                    case SyntaxKind.DefaultSwitchLabel:
                        if (first) {
                            method.Append("if (");
                            first = false;
                        } else {
                            method.Append("||");
                        }
                        method.Append("(!" + ssid + ")");
                        break;
                    default:
                        //statements
                        if (!statements) {
                            method.Append(") {\r\n");
                            method.Append(ssid + " = true;\r\n");
                            statements = true;
                        }
                        statementNode(child);
                        break;
                }
            }
            method.Append("}\r\n");  //end of statements
        }

        private int GetNumArgs(SyntaxNode node) {
            ISymbol symbol = file.model.GetSymbolInfo(node).Symbol;
            String str = symbol.ToString();  //delegate(args...)
            if (str.EndsWith("()")) return 0;
            String[] args = str.Split(",");
            return args.Length;
        }

        private static String ConvertChar(String value) {
            if (value == "\\") return "'\\\\'";
            if (value == "\t") return "'\\t'";
            if (value == "\r") return "'\\r'";
            if (value == "\n") return "'\\n'";
            return "'" + value + "'";
        }

        public static String constantNode(SyntaxNode node, bool typeCastEnum = true) {
            Object obj = file.model.GetConstantValue(node).Value;
            if (obj == null) return null;
            String value = obj.ToString();
            String valueType = GetTypeName(node);
            switch (valueType) {
                case "char":
                    value = ConvertChar(value);
                    break;
                case "float":
                    if (value.IndexOf(".") == -1) value += ".0";
                    value += "f";
                    break;
                case "double":
                    if (value.IndexOf(".") == -1) value += ".0";
                    break;
                case "long":
                    value += "LL";
                    break;
                case "string":
                    value = "\"" + value.Replace("\0", "\\0").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t") + "\"";
                    break;
            }
            if (!typeCastEnum) return value;
            ISymbol symbol = file.model.GetSymbolInfo(node).Symbol;
            ITypeSymbol type = file.model.GetTypeInfo(node).Type;
            if (type.TypeKind == TypeKind.Enum) {
                value = "(" + type.ToString().Replace(".", "::") + ")" + value;
            }
            return value;
        }

        private void expressionNode(SyntaxNode node, bool useName = false) {
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            Type type;
            String constValue = constantNode(node);
            switch (node.Kind()) {
                case SyntaxKind.IdentifierName:
                case SyntaxKind.PredefinedType:
                case SyntaxKind.QualifiedName:
                case SyntaxKind.GenericName:
                    if (constValue != null) {
                        method.Append(constValue);
                        return;
                    }
                    type = new Type(node, useName);
                    method.Append(type.GetCPPType());
                    if (isProperty(node)) {
                        method.Append(".$value");
                    }
                    break;
                case SyntaxKind.VariableDeclaration:
                    //local variable
                    type = new Type();
                    List<Variable> vars = variableDeclaration(node, type);
                    foreach(var variable in vars) {
                        method.Append(type.GetTypeDeclaration());
                        method.Append(" ");
                        method.Append(variable.name);
                        SyntaxNode equals = variable.equals;
                        if (equals != null) {
                            method.Append(" = ");
                            SyntaxNode equalsChild = GetChildNode(equals);
                            if (equalsChild.Kind() == SyntaxKind.ArrayInitializerExpression) {
                                arrayInitNode(equalsChild, type.GetTypeDeclaration(false), type.arrays);
                            } else {
                                expressionNode(equalsChild);
                            }
                        }
                        if (vars.Count > 1) method.Append(";");
                    }
                    break;
                case SyntaxKind.SimpleAssignmentExpression:
                    assignNode(node);
                    break;
                case SyntaxKind.InvocationExpression:
                    invokeNode(node);
                    break;
                case SyntaxKind.ArrayCreationExpression:
                    newArrayNode(node);
                    break;
                case SyntaxKind.NullLiteralExpression:
                    method.Append("nullptr");
                    break;
                case SyntaxKind.NumericLiteralExpression:
                    method.Append(constantNode(node));
                    break;
                case SyntaxKind.TrueLiteralExpression:
                    method.Append("true");
                    break;
                case SyntaxKind.FalseLiteralExpression:
                    method.Append("false");
                    break;
                case SyntaxKind.StringLiteralExpression:
                    method.Append("new Qt::Core::String(");
                    method.Append(constantNode(node));
                    method.Append(")");
                    break;
                case SyntaxKind.CharacterLiteralExpression:
                    method.Append("(char16)");
                    method.Append(constantNode(node));
                    break;
                case SyntaxKind.SimpleMemberAccessExpression:
                    if (constValue != null) {
                        method.Append(constValue);
                        return;
                    }
                    SyntaxNode left = GetChildNode(node, 1);
                    SyntaxNode right = GetChildNode(node, 2);
                    if (isStatic(right) || left.Kind() == SyntaxKind.BaseExpression || isEnum(left) || isNamespace(left) || (isNamedType(left) && isNamedType(right))) {
                        expressionNode(left);
                        method.Append("::");
                        expressionNode(right, true);
                    } else {
                        method.Append("$check(");
                        expressionNode(left);
                        method.Append(")->");
                        expressionNode(right, true);
                    }
                    break;
                case SyntaxKind.BaseExpression:
                    method.Append(cls.bases[0].GetCPPType());
                    break;
                case SyntaxKind.ObjectCreationExpression:
                    invokeNode(node, true);
                    break;
                case SyntaxKind.CastExpression:
                    castNode(node);
                    break;
                case SyntaxKind.ElementAccessExpression:
                    //IdentifierNode, BracketedArgumentList -> {Argument, ...}
                    SyntaxNode array = GetChildNode(node, 1);
                    SyntaxNode index = GetChildNode(node, 2);
                    expressionNode(array);
                    method.Append("->at(");
                    expressionNode(index);
                    method.Append(")");
                    break;
                case SyntaxKind.BracketedArgumentList:
                    foreach(var child in nodes) {
                        expressionNode(child);
                    }
                    break;
                case SyntaxKind.Argument:
                    expressionNode(GetChildNode(node));
                    break;
                case SyntaxKind.AddExpression:
                    SyntaxNode addleft = GetChildNode(node, 1);
                    SyntaxNode addright = GetChildNode(node, 2);
                    if (isString(addleft) || isString(addright)) {
                        method.Append("$addstr(");
                    } else {
                        method.Append("$addnum(");
                    }
                    expressionNode(addleft);
                    method.Append(",");
                    switch (GetTypeName(addright)) {
                        case "char": method.Append("(char16)"); break;
                        case "short": method.Append("(int32)"); break;
                        case "ushort": method.Append("(uint32)"); break;
                        case "byte": method.Append("(uint32)"); break;
                        case "sbyte": method.Append("(int32)"); break;
                    }
                    expressionNode(addright);
                    method.Append(")");
                    break;
                case SyntaxKind.SubtractExpression:
                    binaryNode(node, "-");
                    break;
                case SyntaxKind.MultiplyExpression:
                    binaryNode(node, "*");
                    break;
                case SyntaxKind.DivideExpression:
                    binaryNode(node, "/");
                    break;
                case SyntaxKind.ModuloExpression:
                    modNode(node, "%");
                    break;
                case SyntaxKind.ModuloAssignmentExpression:
                    modAssignNode(node, "%");
                    break;
                case SyntaxKind.LessThanExpression:
                    binaryNode(node, "<");
                    break;
                case SyntaxKind.LessThanOrEqualExpression:
                    binaryNode(node, "<=");
                    break;
                case SyntaxKind.GreaterThanExpression:
                    binaryNode(node, ">");
                    break;
                case SyntaxKind.GreaterThanOrEqualExpression:
                    binaryNode(node, ">=");
                    break;
                case SyntaxKind.EqualsExpression:
                    equalsNode(node, "==");
                    break;
                case SyntaxKind.NotEqualsExpression:
                    equalsNode(node, "!=");
                    break;
                case SyntaxKind.LeftShiftExpression:
                    binaryNode(node, "<<");
                    break;
                case SyntaxKind.RightShiftExpression:
                    binaryNode(node, ">>");
                    break;
                case SyntaxKind.AddAssignmentExpression:
                    SyntaxNode addassignleft = GetChildNode(node, 1);
                    SyntaxNode addassignright = GetChildNode(node, 2);
                    expressionNode(addassignleft, true);
                    if (isString(addassignleft) || isString(addassignright)) {
                        method.Append("= $addstr(");
                    } else {
                        method.Append("= $addnum(");
                    }
                    expressionNode(addassignleft);
                    method.Append(",");
                    switch (GetTypeName(addassignright)) {
                        case "char": method.Append("(char16)"); break;
                        case "short": method.Append("(int32)"); break;
                        case "ushort": method.Append("(uint32)"); break;
                        case "byte": method.Append("(uint32)"); break;
                        case "sbyte": method.Append("(int32)"); break;
                    }
                    expressionNode(addassignright);
                    method.Append(")");
                    break;
                case SyntaxKind.SubtractAssignmentExpression:
                    binaryAssignNode(node, "-");
                    break;
                case SyntaxKind.MultiplyAssignmentExpression:
                    binaryAssignNode(node, "*");
                    break;
                case SyntaxKind.DivideAssignmentExpression:
                    binaryAssignNode(node, "/");
                    break;
                case SyntaxKind.OrAssignmentExpression:
                    binaryAssignNode(node, "|");
                    break;
                case SyntaxKind.AndAssignmentExpression:
                    binaryAssignNode(node, "&");
                    break;
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                    binaryAssignNode(node, "^");
                    break;
                case SyntaxKind.ExclusiveOrExpression:
                    binaryNode(node, "^");
                    break;
                case SyntaxKind.LogicalNotExpression:
                    method.Append("!");
                    expressionNode(GetChildNode(node));
                    break;
                case SyntaxKind.LogicalOrExpression:
                    binaryNode(node, "||");
                    break;
                case SyntaxKind.LogicalAndExpression:
                    binaryNode(node, "&&");
                    break;
                case SyntaxKind.BitwiseOrExpression:
                    if (isEnum(GetChildNode(node))) {
                        //enums convert to int which must be type casted back to enum type
                        method.Append("(");
                        method.Append(GetTypeName(GetChildNode(node)));
                        method.Append(")");
                    }
                    method.Append("(");
                    binaryNode(node, "|");
                    method.Append(")");
                    break;
                case SyntaxKind.BitwiseAndExpression:
                    binaryNode(node, "&");
                    break;
                case SyntaxKind.BitwiseNotExpression:
                    method.Append("!");
                    expressionNode(GetChildNode(node));
                    break;
                case SyntaxKind.LeftShiftAssignmentExpression:
                    binaryAssignNode(node, "<<");
                    break;
                case SyntaxKind.RightShiftAssignmentExpression:
                    binaryAssignNode(node, ">>");
                    break;
                case SyntaxKind.ParenthesizedExpression:
                    method.Append("(");
                    expressionNode(GetChildNode(node));
                    method.Append(")");
                    break;
                case SyntaxKind.PostIncrementExpression:
                    expressionNode(GetChildNode(node));
                    method.Append("++");
                    break;
                case SyntaxKind.PreIncrementExpression:
                    method.Append("++");
                    expressionNode(GetChildNode(node));
                    break;
                case SyntaxKind.PostDecrementExpression:
                    expressionNode(GetChildNode(node), true);
                    method.Append("--");
                    break;
                case SyntaxKind.PreDecrementExpression:
                    method.Append("--");
                    expressionNode(GetChildNode(node), true);
                    break;
                case SyntaxKind.UnaryMinusExpression:
                    method.Append("-");
                    expressionNode(GetChildNode(node));
                    break;
                case SyntaxKind.UnaryPlusExpression:
                    method.Append("+");
                    expressionNode(GetChildNode(node));
                    break;
                case SyntaxKind.ThisExpression:
                    method.Append("this");
                    break;
                case SyntaxKind.PointerIndirectionExpression:
                    method.Append("*");
                    expressionNode(GetChildNode(node));
                    break;
                case SyntaxKind.PointerMemberAccessExpression:
                    SyntaxNode ptrleft = GetChildNode(node, 1);
                    SyntaxNode ptrright = GetChildNode(node, 2);
                    expressionNode(ptrleft);
                    method.Append("->");
                    expressionNode(ptrright);
                    break;
                case SyntaxKind.ParenthesizedLambdaExpression:
                    // ParameterList, Block
                    SyntaxNode plist = GetChildNode(node, 1);
                    SyntaxNode pblock = GetChildNode(node, 2);
                    method.Append("[&]");
                    //output parameter list
                    bool firstLambda = true;
                    method.Append("(");
                    foreach(var param in plist.ChildNodes()) {
                        switch (param.Kind()) {
                            case SyntaxKind.Parameter:
                                SyntaxNode par = GetChildNode(param);
                                Type ptype = new Type();
                                if (par != null) {
                                    parameterNode(par, ptype);
                                } else {
                                    //lambda without arg types
                                    ptype.set("auto");
                                }
                                Variable pvar = new Variable();
                                pvar.name = file.model.GetDeclaredSymbol(param).Name.Replace(".", "::");
                                if (!firstLambda) method.Append(","); else firstLambda = false;
                                method.Append(ptype.GetTypeDeclaration());
                                method.Append(" ");
                                method.Append(pvar.name);
                                break;
                        }
                    }
                    method.Append(")");
                    blockNode(pblock);
                    break;
                case SyntaxKind.DefaultExpression:
                    SyntaxNode defType = GetChildNode(node);
                    Type defSymbol = new Type(defType);
                    if (defSymbol.isObject)
                        method.Append("nullptr");
                    else
                        method.Append("0");
                    break;
                case SyntaxKind.TypeOfExpression:
                    SyntaxNode typeOf = GetChildNode(node);
                    Type typeSymbol = new Type(typeOf);
                    method.Append("new Qt::Core::Type(&$class_" + typeSymbol.Get_Symbol() + ")");
                    break;
                case SyntaxKind.IsExpression:
                    SyntaxNode isObj = GetChildNode(node, 1);
                    SyntaxNode isType = GetChildNode(node, 2);
                    Type isTypeType = new Type(isType);
                    expressionNode(isObj);
                    method.Append("->GetType()");
                    method.Append("->IsDerivedFrom(");
                    method.Append("new Qt::Core::Type(&$class_" + isTypeType.Get_Symbol() + "))");
                    break;
                case SyntaxKind.AsExpression:
                    SyntaxNode asObj = GetChildNode(node, 1);
                    SyntaxNode asType = GetChildNode(node, 2);
                    Type asTypeType = new Type(asType);
                    method.Append("(new Qt::Core::Type(&$class_" + asTypeType.Get_Symbol() + ")");
                    method.Append("->IsDerivedFrom(");
                    expressionNode(asObj);
                    method.Append("->GetType()) ? dynamic_cast<" + asTypeType.Get_Symbol() + ">(");
                    expressionNode(asObj);
                    method.Append(") : nullptr)");
                    break;
                case SyntaxKind.ConditionalExpression:
                    // (cond ? val1 : val2)
                    method.Append("(");
                    expressionNode(GetChildNode(node, 1));
                    method.Append("?");
                    expressionNode(GetChildNode(node, 2));
                    method.Append(":");
                    expressionNode(GetChildNode(node, 3));
                    method.Append(")");
                    break;
                default:
                    Console.WriteLine("Error:Unsupported expression:" + node.Kind());
                    WriteFileLine(node);
                    Environment.Exit(0);
                    break;
            }
        }

        private void arrayInitNode(SyntaxNode node, String type, int dims) {
            IEnumerable<SyntaxNode> list = node.ChildNodes();
            method.Append(" new ");
            for(int a=0;a<dims;a++) {
                method.Append("Qt::QSharp::FixedArray<");
            }
            method.Append(type);
            for(int a=0;a<dims;a++) {
                if (a > 0) method.Append("*");
                method.Append(">");
            }
            method.Append("(std::initializer_list<");
            dims--;
            for(int a=0;a<dims;a++) {
                method.Append("Qt::QSharp::FixedArray<");
            }
            method.Append(type);
            for(int a=0;a<dims;a++) {
                if (a > 0) method.Append("*");
                method.Append(">");
            }
            if (dims > 0) {
                method.Append("*");
            }
            method.Append(">{");
            bool first = true;
            foreach(var elem in list) {
                if (!first) method.Append(","); else first = false;
                expressionNode(elem);
            }
            method.Append("})");
        }

        private bool isStatic(SyntaxNode node) {
            ISymbol symbol = file.model.GetSymbolInfo(node).Symbol;
            if (symbol != null) {
                return symbol.IsStatic;
            }
            ISymbol declsymbol = file.model.GetDeclaredSymbol(node);
            if (declsymbol != null) {
                return declsymbol.IsStatic;
            }
            ISymbol type = file.model.GetTypeInfo(node).Type;
            if (type != null) {
                return type.IsStatic;
            }
            Console.WriteLine("Error:isStatic():Symbol not found for:" + node.ToString());
            WriteFileLine(node);
            errors++;
            return true;
        }

        private bool isClass(SyntaxNode node) {
            ITypeSymbol type = file.model.GetTypeInfo(node).Type;
            if (type == null) return false;
            return (type.TypeKind == TypeKind.Class);
        }

        private bool isEnum(SyntaxNode node) {
            ITypeSymbol type = file.model.GetTypeInfo(node).Type;
            if (type == null) return false;
            if (type.TypeKind == TypeKind.Enum) return true;
            return false;
        }

        private bool isProperty(SyntaxNode node) {
            ISymbol symbol = file.model.GetSymbolInfo(node).Symbol;
            if (symbol == null) return false;
            if (symbol.Kind != SymbolKind.Property) return false;
            String name = symbol.Name;
            if (method != null) {
                if (method.name == "$set_" + name) return true;
                if (method.name == "$get_" + name) return true;
            }
            return false;
        }

        private bool isDelegate(SyntaxNode node) {
            ITypeSymbol type = file.model.GetTypeInfo(node).Type;
            if (type == null) {
                return false;
            }
            return type.TypeKind == TypeKind.Delegate;
        }

        private bool isMethod(SyntaxNode node) {
            ISymbol symbol = Generate.file.model.GetSymbolInfo(node).Symbol;
            if (symbol == null) return false;
            return symbol.Kind == SymbolKind.Method;
        }

        private bool isNamedType(SyntaxNode node) {
            ISymbol symbol = Generate.file.model.GetSymbolInfo(node).Symbol;
            if (symbol == null) return false;
            return symbol.Kind == SymbolKind.NamedType;
        }

        private bool isNamespace(SyntaxNode node) {
            ISymbol symbol = Generate.file.model.GetSymbolInfo(node).Symbol;
            if (symbol == null) return false;
            return symbol.Kind == SymbolKind.Namespace;
        }

        private bool isString(SyntaxNode node) {
            ITypeSymbol type = file.model.GetTypeInfo(node).Type;
            if (type == null) return false;
            switch (type.ToString()) {
                case "string": return true;
                case "System.String": return true;
                case "Qt.Core.String": return true;
            }
            return false;
        }

        private bool isFloat(SyntaxNode node) {
            ITypeSymbol type = file.model.GetTypeInfo(node).Type;
            if (type == null) return false;
            return type.ToString() == "float";
        }

        private bool isDouble(SyntaxNode node) {
            ITypeSymbol type = file.model.GetTypeInfo(node).Type;
            if (type == null) return false;
            return type.ToString() == "double";
        }

        private bool isLong(SyntaxNode node) {
            ITypeSymbol type = file.model.GetTypeInfo(node).Type;
            if (type == null) return false;
            return type.ToString() == "long";
        }

        private bool isChar(SyntaxNode node) {
            ITypeSymbol type = file.model.GetTypeInfo(node).Type;
            if (type == null) return false;
            return type.ToString() == "char";
        }

        private bool isShort(SyntaxNode node) {
            ITypeSymbol type = file.model.GetTypeInfo(node).Type;
            if (type == null) return false;
            return type.ToString() == "short";
        }

        private bool isUShort(SyntaxNode node) {
            ITypeSymbol type = file.model.GetTypeInfo(node).Type;
            if (type == null) return false;
            return type.ToString() == "ushort";
        }

        //returns true if member hides base class member with same name
        public static bool isNew(Class cls, String member) {
            INamedTypeSymbol type = (INamedTypeSymbol)cls.model.GetDeclaredSymbol(cls.node);
            if (type == null) {
                return false;
            }
            type = type.BaseType;
            while (type != null) {
                ImmutableArray<ISymbol> members = type.GetMembers();
                foreach(var m in members) {
                    if (m.Name.Equals(member)) {
                        return true;
                    }
                }
                type = type.BaseType;
            }
            return false;
        }

        private void binaryNode(SyntaxNode node, string op) {
            expressionNode(GetChildNode(node, 1));
            method.Append(op);
            expressionNode(GetChildNode(node, 2));
        }

        private void equalsNode(SyntaxNode node, string op) {
            SyntaxNode left = GetChildNode(node, 1);
            SyntaxNode right = GetChildNode(node, 2);
            bool useEquals = false;
            bool leftString = false;
            bool rightString = false;
            if (isString(left)) leftString = true;
            if (isString(right)) rightString = true;
            if (leftString && rightString) {
                useEquals = true;
            }
            if (GetTypeName(left) == "Qt::Core::Type" && GetTypeName(right) == "Qt::Core::Type") {
                useEquals = true;
            }
            if (useEquals) {
                if (op == "!=") method.Append("!");
                expressionNode(left);
                method.Append("->Equals(");
                expressionNode(right);
                method.Append(")");
            } else {
                binaryNode(node, op);
            }
        }

        private void binaryAssignNode(SyntaxNode node, string op) {
            expressionNode(GetChildNode(node, 1), true);
            method.Append(" = ");
            expressionNode(GetChildNode(node, 1));
            method.Append(op);
            expressionNode(GetChildNode(node, 2));
        }

        private String getModType(SyntaxNode left, SyntaxNode right) {
            if (isDouble(left)) return "d";
            if (isDouble(right)) return "d";
            if (isFloat(left)) return "f";
            if (isFloat(right)) return "f";
            if (isLong(left)) return "l";
            if (isLong(right)) return "l";
            return "i";
        }

        //C++ does not support float % -- must use a special function
        private void modNode(SyntaxNode node, string op) {
            SyntaxNode left = GetChildNode(node, 1);
            SyntaxNode right = GetChildNode(node, 2);
            method.Append("$mod");
            method.Append(getModType(left, right));
            method.Append("(");
            expressionNode(left);
            method.Append(",");
            expressionNode(right);
            method.Append(")");
        }

        //C++ does not support float % -- must use a special function
        private void modAssignNode(SyntaxNode node, string op) {
            SyntaxNode left = GetChildNode(node, 1);
            SyntaxNode right = GetChildNode(node, 2);
            expressionNode(left, true);
            method.Append("= $mod");
            method.Append(getModType(left, right));
            method.Append("(");
            expressionNode(left);
            method.Append(",");
            expressionNode(right);
            method.Append(")");
        }

        private void castNode(SyntaxNode node) {
            SyntaxNode castType = GetChildNode(node, 1);
            SyntaxNode value = GetChildNode(node, 2);
            //cast value to type
            //C# (type)value
            //C++ dynamic_cast<type>(value)
            Type type = new Type(castType);
            String typestr = type.GetTypeDeclaration();
            if (type.isObject) {
                method.Append("dynamic_cast<");
                method.Append(typestr);
                method.Append(">");
                method.Append("(");
                expressionNode(value);
                method.Append(")");
            } else {
                method.Append("static_cast<");
                method.Append(type.GetTypeDeclaration());
                method.Append(">");
                method.Append("(");
                expressionNode(value);
                method.Append(")");
            }
        }

        private void newArrayNode(SyntaxNode node) {
            //node = ArrayCreationExpression -> {ArrayType -> {type, [[rank -> size] ...]} [, ArrayInitializerExpression -> {...}]}
            SyntaxNode arrayType = GetChildNode(node);
            IEnumerable<SyntaxNode> nodes = arrayType.ChildNodes();
            SyntaxNode typeNode = null;
            SyntaxNode sizeNode = null;
            int dims = 0;
            foreach(var child in nodes) {
                switch (child.Kind()) {
                    case SyntaxKind.ArrayRankSpecifier:
                        SyntaxNode rank = GetChildNode(child);
                        dims++;
                        switch (rank.Kind()) {
                            case SyntaxKind.OmittedArraySizeExpression:
                                break;
                            default:
                                if (sizeNode != null) {
                                    Console.WriteLine("Error:multiple sizes for ArrayCreationExpression");
                                    WriteFileLine(node);
                                    errors++;
                                }
                                sizeNode = rank;  //*Expression
                                break;
                        }
                        break;
                    default:  //PredefinedType or IdentifierName
                        typeNode = child;
                        break;
                }
            }
            SyntaxNode initList = GetChildNode(node, 2);
            if (initList != null && initList.Kind() == SyntaxKind.ArrayInitializerExpression) {
                //method.Append("=");
                Type dataType = new Type(typeNode);
                arrayInitNode(initList, dataType.GetTypeDeclaration(false), dims);
                return;
            }
            if (typeNode == null || sizeNode == null || dims == 0) {
                Console.WriteLine("Error:Invalid ArrayCreationExpression : " + typeNode + " : " + sizeNode);
                WriteFileLine(node);
                errors++;
                return;
            }
            method.Append(" new ");
            for(int a=0;a<dims;a++) {
                method.Append("Qt::QSharp::FixedArray<");
            }
            Type type = new Type(typeNode);
            method.Append(type.GetTypeDeclaration());
            for(int a=0;a<dims;a++) {
                if (a > 0) method.Append("*");
                method.Append(">");
            }
            method.Append("(");
            expressionNode(sizeNode);
            method.Append(")");
        }

        private void assignNode(SyntaxNode node) {
            //lvalue = rvalue
            SyntaxNode left = GetChildNode(node, 1);
            SyntaxNode right = GetChildNode(node, 2);
            expressionNode(left);
            method.Append(" = ");
            if (false && isMethod(right)) {
                //assign method to delegate
                if (isStatic(node)) {
                    method.Append("&");
                    expressionNode(right);
                } else {
                    Type type = new Type(right);
                    method.Append("std::bind(&");
                    method.Append(type.GetCPPType());
                    method.Append(", this");
                    //add std::placeholders::_1, ... for # of arguments to delegate
                    int numArgs = GetNumArgs(right);
                    for(int a=0;a<numArgs;a++) {
                        method.Append(", std::placeholders::_" + (a+1));
                    }
                    method.Append(")");
                }
            } else {
                expressionNode(right);
            }
        }

        private void invokeNode(SyntaxNode node, bool New = false) {
            //IdentifierName/SimpleMemberAccessExpression/QualifiedName, ArgumentList
            SyntaxNode id = GetChildNode(node, 1);
            SyntaxNode args = GetChildNode(node, 2);
            if (isCPP(id)) {
                SyntaxNode arg = GetChildNode(args);  //Argument
                SyntaxNode str = GetChildNode(arg);  //StringLiteralExpression
                method.Append(file.model.GetConstantValue(str).Value.ToString());
                return;
            }
            if (New) {
                method.Append("(new ");
            }
            expressionNode(id, !New);
            method.Append("(");
            outArgList(args);
            method.Append(")");
            if (New) {
                method.Append(")");
            }
        }

        /* Determine if id is from the System.QSharp.CPP class. */
        private bool isCPP(SyntaxNode id) {
            if (id.Kind() != SyntaxKind.SimpleMemberAccessExpression) return false;
            ISymbol symbol = file.model.GetSymbolInfo(id).Symbol;
            if (symbol == null) return false;
            String name = symbol.ToString().Replace(".", "::");
            return name.StartsWith("Qt::QSharp::CPP::");
        }

        //ArgumentList
        private void outArgList(SyntaxNode node) {
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            bool first = true;
            foreach(var child in nodes) {
                if (!first) method.Append(","); else first = false;
                switch (child.Kind()) {
                    case SyntaxKind.Argument:
                        expressionNode(GetChildNode(child));
                        break;
                }
            }
        }

        public static SyntaxNode GetChildNode(SyntaxNode node, int idx = 1) {
            IEnumerator<SyntaxNode> e = node.ChildNodes().GetEnumerator();
            for(int a=0;a<idx;a++) {
                e.MoveNext();
            }
            return e.Current;
        }

        private int GetChildCount(SyntaxNode node) {
            IEnumerable<SyntaxNode> childs = node.ChildNodes();
            int cnt = 0;
            foreach(var child in childs) {
                cnt++;
            }
            return cnt;
        }

        private string GetSymbol(SyntaxNode node) {
            ISymbol symbol = file.model.GetSymbolInfo(node).Symbol;
            if (symbol != null) return symbol.Name.Replace(".", "::");
            return null;
        }

        private string GetDeclaredSymbol(SyntaxNode node) {
            ISymbol symbol = file.model.GetDeclaredSymbol(node);
            if (symbol != null) return symbol.Name.Replace(".", "::");
            return null;
        }

        private static string GetTypeName(SyntaxNode node) {
            ITypeSymbol type = file.model.GetTypeInfo(node).Type;
            if (type == null) return "";
            return type.ToString().Replace(".", "::");
        }

        private TypeKind GetTypeKind(SyntaxNode node) {
            ITypeSymbol type = file.model.GetTypeInfo(node).Type;
            if (type == null) return TypeKind.Error;
            return type.TypeKind;
        }

        public static void WriteFileLine(SyntaxNode node) {
            FileLinePositionSpan span = file.tree.GetLineSpan(node.Span);
            Console.WriteLine("  in " + file.csFile + " @ " + (span.StartLinePosition.Line + 1));
        }

        public static String GetEnumStruct(Enum e) {
            StringBuilder str = new StringBuilder();
            str.Append("typedef struct " + e.name + "{\r\n");
            str.Append("int value;\r\n");
            str.Append(e.name + "() {value = 0;}\r\n");
            str.Append(e.name + "(int initValue) {value = initValue;}\r\n");
            str.Append("void operator=(int newValue) {value=newValue;}\r\n");
            str.Append("operator int() {return value;}\r\n");
            foreach(var qt in e.qtType) {
                str.Append("operator " + qt + "() {return (" + qt + ")value;}");
            }
            str.Append("bool operator==(int other) {return value!=other;}\r\n");
            str.Append("bool operator!=(int other) {return value!=other;}\r\n");
            str.Append("} ");
            return str.ToString();
        }
    }

    class Flags
    {
        public bool isPublic;
        public bool isPrivate;
        public bool isProtected;
        public bool isStatic;
        public bool isAbstract;
        public bool isVirtual;
        public bool isExtern;
        public bool isOverride;
        public bool isDefinition;
        public bool isSealed;
        public string GetFlags(bool cls) {
            StringBuilder sb = new StringBuilder();
            //if (Public) sb.Append("public:");
            //if (Private) sb.Append("private:");
            //if (Protected) sb.Append("protected:");
            sb.Append("public:");
            if (isStatic) sb.Append(" static");
            if (isAbstract) {
                if (!cls) {
                    if (!isVirtual) {
                        sb.Append(" virtual");
                    }
                }
            }
            if (isVirtual) sb.Append(" virtual");
            if (isExtern) sb.Append(" extern");
            return sb.ToString();
        }
        public void CopyFlags(Flags src) {
            isPublic = src.isPublic;
            isPrivate = src.isPrivate;
            isProtected = src.isProtected;
            isStatic = src.isStatic;
            isAbstract = src.isAbstract;
            isVirtual = src.isVirtual;
            isExtern = src.isExtern;
            isOverride = src.isOverride;
            isDefinition = src.isDefinition;
            isSealed = src.isSealed;
        }
    }

    class Enum {
        public Enum(string name, string Namespace) {
            this.name = name;
            this.Namespace = Namespace;
        }
        public string name;
        public string Namespace;
        public List<string> qtType = new List<string>();
    }

    class Class : Flags
    {
        public string name = "";
        public string fullname = "";  //inner classes
        public string Namespace = "";
        public string nsfullname = "";  //namespace + fullname
        public bool hasctor;
        public bool isInterface;
        public SyntaxNode node;
        public SemanticModel model;
        public List<Type> bases = new List<Type>();
        public List<string> cppbases = new List<string>();
        public List<Type> ifaces = new List<Type>();
        public List<Field> fields = new List<Field>();
        public List<Method> methods = new List<Method>();
        public List<Enum> enums = new List<Enum>();
        public List<Class> inners = new List<Class>();
        public List<string> usingBaseMembers = new List<string>();
        public Class outter;
        public int lockCnt;
        public int finallyCnt;
        public int enumCnt;
        public int switchStringCnt;
        public bool isGeneric;
        public List<Type> GenericArgs = new List<Type>();
        public string cpp, ctorArgs = "", nonClassCPP, nonClassHPP;
        public bool omitFields, omitMethods, omitConstructors, omitBodies;
        public string forward;
        //uses are used to sort classes
        public List<string> uses = new List<string>();
        public void addUsage(string cls) {
            int idx;
            idx = cls.IndexOf("<");
            if (idx != -1) cls = cls.Substring(0, idx);
            if (cls == nsfullname) return;  //do not add ref to this
            if (!uses.Contains(cls)) {
                uses.Add(cls);
            }
        }
        public string GetTypeDeclaration() {
            StringBuilder sb = new StringBuilder();
            sb.Append(name);
            if (isGeneric) {
                sb.Append("<");
                bool first = true;
                foreach(var arg in GenericArgs) {
                    if (!first) sb.Append(","); else first = false;
                    sb.Append(arg.GetCPPType());
                }
                sb.Append(">");
            }
            return sb.ToString();
        }
        public string FullName(String NameSpace, String name) {
            String ret = NameSpace.Replace("::", "_");
            if (ret.Length > 0) ret += "_";
            ret += name.Replace("::", "_");
            return ret;
        }
        public string GetReflectionExtern() {
            StringBuilder sb = new StringBuilder();
            String full_name = FullName(Namespace, fullname);
            sb.Append("extern $class $class_" + full_name + ";\r\n");
            foreach(var inner in inners) {
                sb.Append(inner.GetReflectionExtern());
            }
            return sb.ToString();
        }
        public string GetForwardDeclaration() {
            StringBuilder sb = new StringBuilder();
            if (isGeneric) {
                sb.Append("template<");
                bool first = true;
                foreach(var arg in GenericArgs) {
                    if (!first) sb.Append(","); else first = false;
                    sb.Append("typename ");
                    sb.Append(arg.GetTypeDeclaration());
                }
                sb.Append(">\r\n");
            }
            sb.Append("struct " + name);
            sb.Append(";\r\n");
            return sb.ToString();
        }
        public string GetReflectionData() {
            StringBuilder sb = new StringBuilder();
            bool first;
            int idx;
            String full_name = FullName(Namespace, fullname);
            //reflection data : fields
            foreach(Field i in fields) {
                foreach(Variable v in i.variables) {
                    sb.Append("static $field $field_" + full_name + "_" + v.name + "(\"" + v.name + "\");\r\n");
                }
            }
            //reflection data : methods
            idx = 0;
            foreach(Method m in methods) {
                if (m.name.StartsWith("$")) continue;
                String name = m.name;
                if (name.StartsWith("~")) {
                    name = name.Replace("~", "$");
                }
                sb.Append("static $method $method_" + full_name + "_" + name + (idx++) + "(\"" + m.name + "\");\r\n");
            }
            //reflection data : class
            sb.Append("$class $class_" + full_name + "(");
            if (isInterface) {
                sb.Append("true,");
            } else {
                sb.Append("false,");
            }
            sb.Append("\"" + name + "\",");
            if (bases.Count == 0) {
                sb.Append("nullptr,");
            } else {
                sb.Append("&$class_" + bases[0].Get_Symbol() + ",");
            }
            //relection data : interfaces list
            sb.Append("{");
            first = true;
            foreach(var i in ifaces) {
                if (!first) sb.Append(","); else first = false;
                sb.Append("&$class_" + i.Get_Symbol());
            }
            sb.Append("},");
            //reflection data : fields lists
            sb.Append("{");
            first = true;
            foreach(Field i in fields) {
                foreach(Variable v in i.variables) {
                    if (!first) sb.Append(","); else first = false;
                    sb.Append("&$field_" + full_name + "_" + v.name);
                }
            }
            sb.Append("},");
            //reflection data : methods list
            sb.Append("{");
            first = true;
            idx = 0;
            foreach(Method m in methods) {
                if (m.name.StartsWith("$")) continue;
                if (!first) sb.Append(","); else first = false;
                String name = m.name;
                if (name.StartsWith("~")) {
                    name = name.Replace("~", "$");
                }
                sb.Append("&$method_" + full_name + "_" + name + (idx++));
            }
            sb.Append("}");
            //newInstance
            Method _new = null;
            if (!isGeneric) {
                foreach(Method m in methods) {
                    if (m.replaceArgs != null) continue;
                    if (m.name == m.cls.name) {
                        if (m.args.Count == 0) {
                            _new = m;
                            break;
                        }
                    }
                }
            }
            if (_new == null || isAbstract) {
                sb.Append(",[] () {return nullptr;}");
            } else {
                sb.Append(",[] () {return new " + this.Namespace + "::" + this.fullname + "();}");
            }
            sb.Append(");\r\n");
            foreach(var inner in inners) {
                sb.Append(inner.GetReflectionData());
            }
            return sb.ToString();
        }
        public string GetClassDeclaration() {
            StringBuilder sb = new StringBuilder();
            bool first;
            String full_name = FullName(Namespace, fullname);
            if (isGeneric) {
                sb.Append("template< ");
                first = true;
                foreach(var arg in GenericArgs) {
                    if (!first) sb.Append(","); else first = false;
                    sb.Append("typename ");
                    sb.Append(arg.GetTypeDeclaration());
                }
                sb.Append(">");
            }
            if (name != fullname) sb.Append(GetFlags(true));  //inner class
            sb.Append(" struct " + name);
            if (bases.Count > 0 || cppbases.Count > 0 || ifaces.Count > 0) {
                sb.Append(":");
                first = true;
                foreach(var basecls in bases) {
                    if (!first) sb.Append(","); else first = false;
                    sb.Append("public ");
                    sb.Append(basecls.GetCPPType());
                }
                foreach(var cppcls in cppbases) {
                    if (!first) sb.Append(","); else first = false;
                    sb.Append("public ");
                    sb.Append(cppcls);
                }
                foreach(var iface in ifaces) {
                    if (!first) sb.Append(","); else first = false;
                    sb.Append("public ");
                    sb.Append(iface.GetCPPType());
                }
            }
            sb.Append("{\r\n");
            foreach(var inner in inners) {
                sb.Append(inner.GetClassDeclaration());
            }
            if (cpp != null) sb.Append(cpp);
            if (!isInterface) {
                sb.Append("public: virtual $class* $getType() {return &$class_" + full_name + ";}\r\n");
            }
            foreach(var e in enums) {
                sb.Append(Generate.GetEnumStruct(e) + e.name + ";\r\n");
            }
            foreach(var method in methods) {
                if (!method.isDelegate) continue;
                if (method.version != null) {
                    sb.Append("#if QT_VERSION >= " + method.version + "\r\n");
                }
                sb.Append(method.GetMethodDeclaration());
                if (isGeneric) {
                    if (method.name == "$init") {
                        sb.Append("{\r\n");
                        if (method.cls.bases.Count > 0) {
                            sb.Append(method.cls.bases[0].GetSymbol() + "::$init();\r\n");
                        }
                        foreach(var field in fields) {
                            foreach(var v in field.variables) {
                                if (v.method.Length() > 0 && !field.isStatic) {
                                    sb.Append(v.method.src);
                                }
                            }
                        }
                        sb.Append("}\r\n");
                    } else {
                        if (!method.type.isAbstract) sb.Append(method.src);
                    }
                }
                sb.Append(";\r\n");
                if (method.version != null) {
                    sb.Append("#endif\r\n");
                }
            }
            foreach(var field in fields) {
                sb.Append(field.GetFieldDeclaration());
            }
            foreach(var method in methods) {
                if (method.isDelegate) continue;
                if (bases.Count > 0) {
                    if (method.type.isOverride || Generate.isNew(this, method.name) ) {
                        if (!usingBaseMembers.Contains(method.name)) {
                            usingBaseMembers.Add(method.name);
                            //need to add using base::name so it does not hide base versions
                            sb.Append("using ");
                            sb.Append(bases[0].GetCPPType());
                            sb.Append("::");
                            sb.Append(method.name);
                            sb.Append(";\r\n");
                        }
                    }
                }
                if (method.version != null) {
                    sb.Append("#if QT_VERSION >= " + method.version + "\r\n");
                }
                sb.Append(method.GetMethodDeclaration());
                if (isGeneric) {
                    if (method.name == "$init") {
                        sb.Append("{\r\n");
                        if (method.cls.bases.Count > 0) {
                            sb.Append(method.cls.bases[0].GetSymbol() + "::$init();\r\n");
                        }
                        foreach(var field in fields) {
                            foreach(var v in field.variables) {
                                if (v.method.Length() > 0 && !field.isStatic) {
                                    sb.Append(v.method.src);
                                }
                            }
                        }
                        sb.Append("}\r\n");
                    } else {
                        if (!method.type.isAbstract) sb.Append(method.src);
                    }
                }
                sb.Append(";\r\n");
                if (method.version != null) {
                    sb.Append("#endif\r\n");
                }
            }
            sb.Append("};\r\n");
            return sb.ToString();
        }
        public void GetInnerStaticFields(StringBuilder sb) {
            foreach(var inner in inners) {
                foreach(var field in inner.fields) {
                    if (!field.isStatic) continue;
                    foreach(var v in field.variables) {
                        sb.Append(field.GetTypeDeclaration() + " " + inner.fullname + "::" + v.name);
                        if (field.isNumeric) {
                            sb.Append("= 0;\r\n");
                        }  else {
                            sb.Append(";\r\n");
                        }
                    }
                }
                inner.GetInnerStaticFields(sb);
            }
        }
        public void GetInnerStaticFieldsInit(StringBuilder sb) {
            foreach(var inner in inners) {
                foreach(var field in inner.fields) {
                    if (!field.isStatic) continue;
                    foreach(var v in field.variables) {
                        sb.Append(v.method.src);
                    }
                }
                inner.GetInnerStaticFields(sb);
            }
        }
        public string GetStaticFields() {
            StringBuilder sb = new StringBuilder();
            foreach(var field in fields) {
                if (!field.isStatic) continue;
                foreach(var v in field.variables) {
                    sb.Append(field.GetTypeDeclaration() + " " + name + "::" + v.name);
                    if (field.isNumeric) {
                        sb.Append("= 0;\r\n");
                    }  else {
                        sb.Append(";\r\n");
                    }
                }
            }
            GetInnerStaticFields(sb);
            return sb.ToString();
        }
        public string GetStaticFieldsInit() {
            StringBuilder sb = new StringBuilder();
            foreach(var field in fields) {
                if (!field.isStatic) continue;
                foreach(var v in field.variables) {
                    sb.Append(v.method.src);
                }
            }
            GetInnerStaticFieldsInit(sb);
            return sb.ToString();
        }
        public string GetMethodsDefinitions() {
            StringBuilder sb = new StringBuilder();
            foreach(var method in methods) {
                if (omitBodies) continue;
                if (method.isDelegate) continue;
                if (method.omitBody) continue;
                if (method.type.isAbstract) {
                    //C++ allows abstract methods to be defined and can be called
                    //Need to invoke virtual function without scope specifier (BUG : Need to remove scope specifier in the first place)
                    if (method.type.GetCSType() == "void") {
                        method.Append("{" + method.name + method.GetArgsNames() + ";}");
                    } else if (method.type.isPrimative) {
                        method.Append("{return " + method.name + method.GetArgsNames() + ";}");
                    } else {
                        method.Append("{return " + method.name + method.GetArgsNames() + ";}");
                    }
                }
                if (method.version != null) {
                    sb.Append("#if QT_VERSION >= " + method.version + "\r\n");
                }
                sb.Append(method.type.GetTypeDeclaration());
                sb.Append(" ");
                sb.Append(method.cls.fullname);
                sb.Append("::");
                sb.Append(method.name);
                sb.Append(method.GetArgs(false));
                if (method.ctor) {
                    if (method.basector != null) {
                        sb.Append(":");
                        sb.Append(method.basector);
                    }
                }
                if (method.Length() == 0) method.Append("{}\r\n");
                if (method.name == "$init") {
                    sb.Append("{\r\n");
                    foreach(var basecls in method.cls.bases) {
                        sb.Append(basecls.GetCPPType());
                        sb.Append("::$init();\r\n");
                    }
                    foreach(var iface in method.cls.ifaces) {
                        sb.Append(iface.GetCPPType());
                        sb.Append("::$init();\r\n");
                    }
                    foreach(var field in method.cls.fields) {
                        foreach(var v in field.variables) {
                            if (v.method.Length() > 0 && !field.isStatic) {
                                sb.Append(v.method.src);
                            }
                        }
                    }
                    sb.Append("}\r\n");
                } else {
                    sb.Append(method.src);
                }
                if (method.version != null) {
                    sb.Append("#endif\r\n");
                }
            }
            foreach(var inner in inners) {
                sb.Append(inner.GetMethodsDefinitions());
            }
            return sb.ToString();
        }
    }

    class Type : Flags {
        private string type;
        public TypeKind typekind;
        public SymbolKind symbolkind;
        public SyntaxNode node;
        public ISymbol symbol, declSymbol;
        public ITypeSymbol typeSymbol;
        public bool isPrimative;
        public bool isNumeric;
        public bool isArray;
        public int arrays;  //# of dimensions
        public bool isObject;
        public bool isDelegate;
        public bool isPtr;  //unsafe pointer
        public Class cls;

        public virtual bool isField() {return false;}
        public virtual bool isMethod() {return false;}

        public Type() {}
        public Type(SyntaxNode node, bool useName = false) {
            set(node, useName);
            setTypes();
        }
        public Type(SyntaxNode node, String sym) {
            this.node = node;
            set(sym);
            setTypes();
        }
        public void CopyType(Type src) {
            type = src.type;
            typekind = src.typekind;
            symbolkind = src.symbolkind;
            node = src.node;
            symbol = src.symbol;
            declSymbol = src.declSymbol;
            typeSymbol = src.typeSymbol;
            isPrimative = src.isPrimative;
            isNumeric = src.isNumeric;
            isArray = src.isArray;
            arrays = src.arrays;
            isObject = src.isObject;
            isPtr = src.isPtr;
            cls = src.cls;
        }
        public void set(String sym) {
            int idx = sym.IndexOf("(");
            if (idx != -1) {
                sym = sym.Substring(0, idx);
            }
            type = sym.Replace(".", "::");
        }
        public void set(SyntaxNode node, bool useName = false) {
            this.node = node;
            while (node.Kind() == SyntaxKind.ArrayType) {
                isArray = true;
                foreach(var child in node.ChildNodes()) {
                    if (child.Kind() == SyntaxKind.ArrayRankSpecifier) {
                        arrays++;
                        if (arrays > 3) {
                            Console.WriteLine("Error:Array Dimensions not supported:" + arrays);
                            Generate.WriteFileLine(node);
                            Generate.errors++;
                        }
                    }
                }
                node = Generate.GetChildNode(node);
            }
            symbol = Generate.file.model.GetSymbolInfo(node).Symbol;
            if (symbol == null) {
                Console.WriteLine("Error:symbol==null:" + node.Kind().ToString());
                Generate.WriteFileLine(node);
                Generate.errors++;
                return;
            }
            switch (symbol.Kind) {
                case SymbolKind.Parameter:
                    useName = true;
                    break;
            }
            symbolkind = symbol.Kind;
            typeSymbol = Generate.file.model.GetTypeInfo(node).Type;
            if (typeSymbol != null) {
                typekind = typeSymbol.TypeKind;
                switch (typekind) {
                    case TypeKind.Delegate:
                        if (Generate.cls.isGeneric) useName = true;  //for MSVC bug
                        break;
                }
            }
            String value = Generate.constantNode(node);
            if (value != null) {
                set(value);
            } else {
                if (useName)
                    set(symbol.Name.Replace(".", "::"));
                else
                    set(symbol.ToString().Replace(".", "::"));
            }
            if (node.Kind() == SyntaxKind.GenericName) {
                //need to replace generic args
                SyntaxNode typeArgList = Generate.GetChildNode(node);
                int idx1 = type.IndexOf('<');
                int idx2 = type.LastIndexOf(">");
                if (idx1 != -1 && idx2 != -1) {
                    type = type.Substring(0, idx1+1) + type.Substring(idx2);
                    String args = "";
                    bool first = true;
                    foreach(var child in typeArgList.ChildNodes()) {
                        Type t = new Type(child);
                        if (first) {first = false;} else {args += ",";}
                        args += t.GetTypeDeclaration();
                    }
                    type = type.Insert(idx1+1, args);
                }
            }
        }
        public void setTypes() {
            isDelegate = typekind == TypeKind.Delegate;
            switch (type) {
                case "":
                case "void":
                    isPrimative = true;
                    isNumeric = false;
                    isObject = false;
                    break;
                case "bool":
                case "byte":
                case "sbyte":
                case "short":
                case "ushort":
                case "int":
                case "uint":
                case "long":
                case "ulong":
                case "char":
                case "float":
                case "double":
                    isPrimative = true;
                    isNumeric = true;
                    isObject = false;
                    break;
                default:
                    isPrimative = false;
                    isObject = true;
                    switch (typekind) {
                        case TypeKind.Delegate: isObject = false; break;
                        case TypeKind.Enum: isObject = false; break;
                        case TypeKind.TypeParameter: isObject = false; break;
                    }
                    if (node != null) {
                        switch (node.Kind()) {
                            case SyntaxKind.TypeParameter: isObject = false; break;
                        }
                    }
                    break;
            }
        }
        public bool isSymbolMethod() {
            return symbolkind == SymbolKind.Method;
        }
        public string ConvertType() {
            switch (type) {
                case "byte": return "uint8";
                case "sbyte": return "int8";
                case "short": return "int16";
                case "ushort": return "uint16";
                case "int": return "int32";
                case "uint": return "uint32";
                case "long": return "int64";
                case "ulong": return "uint64";
                case "char": return "char16";
                case "string":
                case "System::string":
                case "Qt::Core::string":
                    return "Qt::Core::String";
                case "object":
                case "System::object":
                case "Qt::Core::object":
                    return "Qt::Core::Object";
                default: return Generate.ConvertName(type);
            }
        }
        public String GetSymbol() {
            String sym = ConvertType();
            int idx = sym.IndexOf("<");
            if (idx != -1) {
                return sym.Substring(0, idx);
            }
            return sym;
        }
        public String Get_Symbol() {
            return GetSymbol().Replace("::", "_");
        }
        public String GetName() {
            String sym = GetSymbol();
            int idx = type.LastIndexOf("::");
            if (idx != -1) {
                return type.Substring(idx+2);
            }
            return type;
        }
        public string GetCSType() {
            return type;
        }
        public string GetCPPType() {
            return ConvertType();
        }
        public string GetTypeDeclaration(bool inc_arrays = true) {
            StringBuilder sb = new StringBuilder();
            if (inc_arrays && arrays > 0) {
                for(int a=0;a<arrays;a++) {
                    sb.Append("Qt::QSharp::FixedArray<");
                }
            }
            sb.Append(GetCPPType());
            if (isPtr || isObject) sb.Append("*");
            if (inc_arrays && arrays > 0) {
                for(int a=0;a<arrays;a++) {
                    if (a > 0) sb.Append("*");
                    sb.Append(">");
                }
                sb.Append("*");
            }
            return sb.ToString();
        }
    }

    class Variable {
        public string name;
        public SyntaxNode equals;
        public Method method = new Method();  //equals code
    }

    class Argument {
        public Type type;
        public Variable name = new Variable();
    }

    class Field : Type
    {
        public override bool isField() {return true;}
        public List<Variable> variables = new List<Variable>();
        public bool isProperty;
        public bool get_Property;
        public bool set_Property;

        public string GetFieldDeclaration() {
            StringBuilder sb = new StringBuilder();
            foreach(var v in variables) {
                sb.Append(GetFlags(false));
                if (isProperty) {
                    sb.Append(" Qt::QSharp::Property<");
                }
                if (isArray) {
                    sb.Append(" ");
                    sb.Append(GetTypeDeclaration());
                    sb.Append(" ");
                    if (isProperty) {
                        sb.Append(">");
                    }
                    sb.Append(v.name);
                } else {
                    sb.Append(" ");
                    sb.Append(GetTypeDeclaration());
                    sb.Append(" ");
                    if (isProperty) {
                        sb.Append(">");
                    }
                    sb.Append(v.name);
                }
                if (!isStatic && !isProperty && !isDelegate) {
                    if (isObject) {
                        sb.Append(" = nullptr");
                    } else {
                        sb.Append(" = 0");
                    }
                }
                sb.Append(";\r\n");
            }
            return sb.ToString();
        }
    }

    class Method
    {
        public Type type = new Type();
        public StringBuilder src = new StringBuilder();
        public int Length() {return src.Length;}
        public void Append(String str) {src.Append(str);}
        public String name;
        public SyntaxNode node;
        public ISymbol symbol;

        public bool ctor;
        public bool isDelegate;
        public bool isOperator;
        public string Namespace;  //if classless delegate only
        public String basector;
        public List<Argument> args = new List<Argument>();
        public Class cls;
        public bool inFixedBlock;
        public String version;
        public String replaceArgs;
        public bool omitBody;
        public bool automemorypool;
        public int[] switchIDs = new int[32];  //up to 32 nested switch statements
        public int currentSwitch = -1;
        public int nextSwitchID = 0;

        public string GetArgs(bool decl) {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            if (replaceArgs != null) {
                sb.Append(replaceArgs);
            } else {
                bool first = true;
                foreach(var arg in args) {
                    if (!first) sb.Append(","); else first = false;
                    sb.Append(arg.type.GetTypeDeclaration());
                    sb.Append(" ");
                    sb.Append(arg.name.name);
                    if (decl && arg.name.method.src.Length > 0) {
                        sb.Append(" = " );
                        sb.Append(arg.name.method.src.ToString());
                    }
                }
            }
            sb.Append(")");
            return sb.ToString();
        }
        public string GetArgsNames() {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            bool first = true;
            foreach(var arg in args) {
                if (!first) sb.Append(","); else first = false;
                sb.Append(arg.name.name);
            }
            sb.Append(")");
            return sb.ToString();
        }
        public string GetMethodDeclaration() {
            StringBuilder sb = new StringBuilder();
            if (isOperator) sb.Append("inline");
            if (!isDelegate && !isOperator) sb.Append(type.GetFlags(false));
            sb.Append(" ");
            if (isDelegate) sb.Append("typedef std::function<");
            sb.Append(type.GetTypeDeclaration());
            sb.Append(" ");
            if (isOperator) sb.Append(" operator");
            if (!isDelegate) sb.Append(name);
            sb.Append(GetArgs(true));
            if (isDelegate) {
                sb.Append(">");  //$delegate");
                sb.Append(name);
            }
            if (type.isAbstract) sb.Append("=0" + ";\r\n");
            if (isOperator) {
                sb.Append(src);
            }
            return sb.ToString();
        }
    }
}
