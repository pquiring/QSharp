using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        public static string version = "0.12";
        public static bool library;
        public static bool shared;
        public static bool service;
        public static String serviceName;
        public static string target;
        public static bool classlib;
        public static int headidx = 0;
        public static string main;
        public static string home = ".";
        public static bool single = false;  //generate monolithic cpp source file
        public static string cxx = "14";  //C++ version to use
        public static bool msvc = false;
        public static bool debug = false;
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
                Console.WriteLine("  --shared");
                Console.WriteLine("  --main=class");
                Console.WriteLine("  --service=name");
                Console.WriteLine("  --ref=dll");
                Console.WriteLine("  --home=folder");
                Console.WriteLine("  --print[=tokens,tostring,all]");
                Console.WriteLine("  --single | --multi");
                Console.WriteLine("  --cxx=version");
                Console.WriteLine("  --msvc");
                Console.WriteLine("  --debug");
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
                if (arg == "--cxx") {
                    if (value.Length == 0) {
                        Console.WriteLine("Error:--cxx requires a version number (14 or 17)");
                        return;
                    }
                    cxx = value;
                }
                if (arg == "--msvc") {
                    msvc = true;
                }
                if (arg == "--debug") {
                    debug = true;
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
            node.cppFile = "cpp\\$head.cpp";
            node.tree = CSharpSyntaxTree.ParseText(node.src);
            compiler = compiler.AddSyntaxTrees(node.tree);
            files.Add(node);
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
            } else {
                ln += ",DeclSymbol=null";
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
            } else {
                ln += ",symbol=null";
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
            } else {
                ln += ",Type=null";
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

        private FileStream fs;
        private string Namespace = "";
//        private List<string> usings = new List<string>();
        public static Class cls;
        private Class NoClass = new Class();  //for classless delegates
        private Method method;
        private Field field;

        public void generate()
        {
            if (Program.printTree) {
                Console.WriteLine();
            }
//            usings.Add("Qt::Core");
            foreach(Source file in Program.files) {
                generate(file);
            }
            Directory.CreateDirectory("cpp");
            openOutput("cpp/" + Program.hppFile);
            writeForward();
            /** In C++ you can not use an undefined class, so they must be sorted by usage. */
            if (Program.classlib) {
                sortClasslib();
            }
            while (sortClasses()) {};
            while (sortFiles()) {};
            writeNoClassTypes();
            writeClasses();
            writeEndIf();
            closeOutput();
            if (Program.single) openOutput("cpp/" + Program.target + ".cpp");
            if (Program.single) writeIncludes();
            foreach(Source file in Program.files) {
                if (!Program.single && file.csTimestamp <= file.cppTimestamp) continue;
                Generate.file = file;
                if (!Program.single) openOutput(file.cppFile);
                if (!Program.single) writeIncludes();
                writeStaticFields();
                writeMethods();
                if (!Program.single) closeOutput();
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
//            foreach(var use in usings) {
//                sb.Append("namespace " + use + " {}\r\n");
//                sb.Append("using namespace " + use + ";\r\n");
//            }
            foreach(var file in Program.files) {
                foreach(var cls in file.clss) {
                    if (cls.Namespace == "Qt::QSharp" && cls.name.StartsWith("CPP")) continue;
                    sb.Append(cls.GetReflectionExtern());
                    if (cls.Namespace != "") sb.Append("namespace " + cls.Namespace + "{\r\n");
                    sb.Append(cls.GetForwardDeclaration());
                    if (cls.Namespace != "") sb.Append("}\r\n");
                    if (!cls.Interface && cls.bases.Count == 0 && (!(cls.Namespace == "Qt::Core" && cls.name == "Object"))) {
                        cls.bases.Add(new Type("Qt::Core::Object"));
                        cls.addUsage("Object");
                        if (cls.Namespace.StartsWith("Qt")) cls.addUsage("QSharpDerived");  //temp fix
                    }
                }
            }
            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        private void writeEndIf() {
            byte[] bytes = new UTF8Encoding().GetBytes("#endif\r\n");
            fs.Write(bytes, 0, bytes.Length);
        }

        private void MoveClass(String mvFile, String mvCls) {
            foreach(var file in Program.files) {
                if (file.csFile != mvFile) continue;
                foreach(var cls in file.clss) {
                    if (cls.name == mvCls) {
//                        Console.WriteLine("Moving:" + mvFile + ":" + mvCls);
                        file.clss.Remove(cls);
                        Program.files[0].clss.Insert(0, cls);
                        return;
                    }
                }
            }
            Console.WriteLine("Error:Can not find:" + mvFile + ":" + mvCls);
        }

        private void sortClasslib() {
            //certain classes MUST come first
            MoveClass("Qt\\Core\\String.cs", "String");
            MoveClass("Qt\\QSharp\\FixedArray.cs", "FixedArrayEnumerator");
            MoveClass("Qt\\QSharp\\FixedArray.cs", "FixedArray");
            MoveClass("Qt\\Core\\IEnumerable.cs", "IEnumerable");
            MoveClass("Qt\\Core\\IEnumerator.cs", "IEnumerator");
            MoveClass("Qt\\Core\\Object.cs", "Object");
        }

        private bool sortClasses() {
            int fcnt = Program.files.Count;
            for(int fidx = 0;fidx<fcnt;fidx++) {
                Source file = Program.files[fidx];
                List<Class> clss = file.clss;
                int cnt = clss.Count;
                for(int idx=0;idx<cnt;idx++) {
                    Class cls = clss[idx];
                    int ucnt = cls.uses.Count;
                    for(int uidx=0;uidx<ucnt;uidx++) {
                        string use = cls.uses[uidx];
                        for(int idx2=idx+1;idx2<cnt;idx2++) {
                            if (clss[idx2].name == use) {
                                //need to move idx2 before idx
                                Class tmp = clss[idx2];
//                                Console.WriteLine("move:" + tmp.name + " before " + cls.name);
                                clss.RemoveAt(idx2);
                                clss.Insert(0, tmp);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool sortFiles() {
            int fcnt = Program.files.Count;
            for(int fidx = 0;fidx<fcnt;fidx++) {
                Source file = Program.files[fidx];
                List<Class> clss = file.clss;
                int cnt = clss.Count;
                for(int idx=0;idx<cnt;idx++) {
                    Class cls = clss[idx];
                    int ucnt = cls.uses.Count;
                    for(int uidx=0;uidx<ucnt;uidx++) {
                        string use = cls.uses[uidx];
                        for(int fidx2=fidx+1;fidx2<fcnt;fidx2++) {
                            Source file2 = Program.files[fidx2];
                            List<Class> clss2 = file2.clss;
                            int cnt2 = clss2.Count;
                            for(int idx2=0;idx2<cnt2;idx2++) {
                                Class cls2 = clss2[idx2];
                                if (use == cls2.name) {
                                    //need to move idx2 before idx
                                    Source tmp = Program.files[fidx2];
//                                    Console.WriteLine("move:" + tmp.csFile + " before " + file.csFile);
                                    Program.files.RemoveAt(fidx2);
                                    Program.files.Insert(Program.headidx, tmp);
                                    return true;
                                }
                            }
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
                    sb.Append("namespace ");
                    sb.Append(dgate.Namespace);
                    sb.Append("{\r\n");
                }
                sb.Append(dgate.GetDeclaration());
                sb.Append(";\r\n");
                if (dgate.Namespace.Length > 0) sb.Append("}\r\n");
            }
            foreach(var e in NoClass.enums) {
                if (e.Namespace.Length > 0) {
                    sb.Append("namespace ");
                    sb.Append(e.Namespace);
                    sb.Append("{\r\n");
                }
                sb.Append(e.src);
                sb.Append(";\r\n");
                if (e.Namespace.Length > 0) sb.Append("}\r\n");
            }
            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        /** Create default ctor if class has no ctors. */
        private void createDefaultCtor(Class cls) {
            if (!cls.hasctor && !cls.Interface && !cls.omitConstructors) {
                Generate.cls = cls;
                ctorNode(null);
            }
            foreach(var inner in cls.inners) {
                createDefaultCtor(inner);
            }
        }

        private void writeClasses() {
            StringBuilder sb = new StringBuilder();
            foreach(var file in Program.files) {
                foreach(var cls in file.clss) {
                    if (cls.Namespace == "Qt::QSharp" && cls.name.StartsWith("CPP")) continue;
                    createDefaultCtor(cls);
                    if (cls.Namespace != "") sb.Append("namespace " + cls.Namespace + "{\r\n");
                    sb.Append(cls.GetDeclaration());
                    if (cls.Namespace != "") sb.Append("}\r\n");
                    if (cls.nonClassHPP != null) sb.Append(cls.nonClassHPP);
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
                if (cls.Namespace != "") sb.Append("namespace " + cls.Namespace + "{\r\n");
                sb.Append(cls.GetStaticFields());
                if (cls.Namespace != "") sb.Append("}\r\n");
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
                    sb.Append("class " + cls.forward + ";\r\n");
                }
                string hppfile = "src/" + cls.name + ".hpp";
                if (File.Exists(hppfile)) sb.Append("#include \"../" + hppfile + "\"\r\n");
                sb.Append(cls.GetReflectionData());
                if (cls.Namespace != "") sb.Append("namespace " + cls.Namespace + "{\r\n");
                if (!cls.Generic && !cls.Interface) {
                    sb.Append(cls.GetMethodsDefinitions());
                }
                if (cls.Namespace != "") sb.Append("}\r\n");
                if (cls.nonClassCPP != null) sb.Append(cls.nonClassCPP);
                string cppfile = "src/" + cls.name + ".cpp";
                if (File.Exists(cppfile)) sb.Append("#include \"../" + cppfile + "\"\r\n");
            }
            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        private void writeMain() {
            StringBuilder sb = new StringBuilder();

            foreach(var lib in Program.libs) {
//                sb.Append("#include <" + lib + ".hpp>\r\n");
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

            sb.Append("namespace Qt::Core {\r\n");;
            sb.Append("int g_argc;\r\n");
            sb.Append("const char **g_argv;\r\n");
            sb.Append("}\r\n");
            sb.Append("int main(int argc, const char **argv) {\r\n");
            sb.Append("Qt::Core::g_argc = argc;\r\n");
            sb.Append("Qt::Core::g_argv = argv;\r\n");
            foreach(var lib in Program.libs) {
                sb.Append("$" + lib + "_ctor();\r\n");
            }
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
            sb.Append("std::shared_ptr<Qt::QSharp::FixedArray<std::shared_ptr<Qt::Core::String>>> args = Qt::QSharp::FixedArray<std::shared_ptr<Qt::Core::String>>::$new(argc-1);\r\n");
            sb.Append("for(int a=1;a<argc;a++) {args->at(a-1) = Qt::Core::String::$new(argv[a]);}\r\n");
            sb.Append("try {\r\n");
            sb.Append(Program.main + "::" + name + "(args);\r\n");
            sb.Append("} catch (std::shared_ptr<Qt::Core::Exception> ex) {Qt::Core::Console::WriteLine($add(Qt::Core::String::$new(\"Exception caught:\"), ex->ToString()));}\r\n");
            sb.Append("catch (std::shared_ptr<Qt::Core::NullPointerException> ex) {Qt::Core::Console::WriteLine($add(Qt::Core::String::$new(\"Exception caught:\"), ex->ToString()));}\r\n");
            sb.Append("catch (std::shared_ptr<Qt::Core::ArrayBoundsException> ex) {Qt::Core::Console::WriteLine($add(Qt::Core::String::$new(\"Exception caught:\"), ex->ToString()));}\r\n");
            sb.Append("catch (...) {Qt::Core::Console::WriteLine(Qt::Core::String::$new(\"Unknown exception thrown\"));}\r\n");
            return sb.ToString();
        }

        private void writeLibraryMain() {
            StringBuilder sb = new StringBuilder();

            if (!Program.single) sb.Append("#include \"" + Program.target + ".hpp\"\r\n");

            sb.Append("namespace Qt::Core {\r\n");;
            sb.Append("int g_argc;\r\n");
            sb.Append("const char **g_argv;\r\n");
            sb.Append("}\r\n");
            sb.Append("extern \"C\" {\r\n");
            sb.Append("__declspec(dllexport)");
            sb.Append("void LibraryMain(std::shared_ptr<Qt::Core::Object> obj) {\r\n");
            sb.Append(Program.main + "::LibraryMain(obj);}\r\n");
            sb.Append("}\r\n");

            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        private void writeCMakeLists() {
            StringBuilder sb = new StringBuilder();

            if (Program.cxx == "17")
                sb.Append("cmake_minimum_required(VERSION 3.10)\r\n");
            else
                sb.Append("cmake_minimum_required(VERSION 3.6)\r\n");
            sb.Append("set(CMAKE_CXX_STANDARD " + Program.cxx + ")\r\n");
            if (Program.classlib) {
                sb.Append("add_definitions(-DCLASSLIB)\r\n");
            }
            sb.Append("include_directories(/usr/include/qt5)\r\n");
            sb.Append("include_directories(/usr/include/ffmpeg)\r\n");
            sb.Append("include_directories(" + Program.home + "/include)\r\n");
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
                sb.Append(" Qt5Core Qt5Gui Qt5Network Qt5Widgets Qt5Xml Qt5WebSockets Qt5Multimedia");
                if (Program.msvc) {
                    sb.Append(" msvcrt");
                    if (Program.debug) sb.Append("d");
                } else {
                    sb.Append(" stdc++ z");
                }
                sb.Append(")\r\n");
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
/*
                    IEnumerable<SyntaxNode> ids = node.DescendantNodes();
                    string Using = "";
                    foreach(var id in ids) {
                        if (id.Kind() == SyntaxKind.IdentifierName) {
                            if (Using.Length != 0) Using += "::";
                            Using += id.ToString();
                        }
                    }
                    if (Using == "System") break;  //not needed
                    if (Using == "System::Reflection") break;  //not supported yet
                    if (!usings.Contains(Using)) {
                        usings.Add(Using);
                    }
*/
                    break;
                case SyntaxKind.DelegateDeclaration:
                    cls = NoClass;
                    methodNode(node, false, true, null);
                    break;
                case SyntaxKind.EnumDeclaration:
                    cls = NoClass;
                    cls.enums.Add(new Enum(node.ToString().Replace("public", ""), Namespace));
                    break;
            }
        }

        private void classNode(SyntaxNode node, Class inner, Class outter, bool Interface) {
            cls = inner;
            cls.fullname = outter.fullname;
            if (cls.fullname.Length > 0) {
                cls.fullname += "::";
            }
            cls.name = file.model.GetDeclaredSymbol(node).Name;
            cls.fullname += cls.name;
            cls.Namespace = Namespace;
            cls.Interface = Interface;
            if (!Interface) {
                Method init = new Method();
                init.cls = cls;
                init.type.set("void");
                init.type.cls = cls;
                init.type.primative = true;
                init.type.Public = true;
                init.name = "$init";
                init.type.setTypes();
                cls.methods.Add(init);
            }
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
                        cls.enums.Add(new Enum(child.ToString().Replace("public", ""), null));
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
                        //TODO
                        break;
                    default:
                        Console.WriteLine("Unknown Declaration:" + child.ToString());
                        Environment.Exit(1);
                        break;
                }
            }
        }

        private void typeParameterListNode(SyntaxNode node) {
            cls.Generic = true;
            foreach(var child in node.ChildNodes()) {
                switch (child.Kind()) {
                    case SyntaxKind.TypeParameter:
                        cls.GenericArgs.Add(new Type(child, true));
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
                                baseType = new Type(baseNode, true);
                                break;
                            case SyntaxKind.GenericName:
                                baseType = new Type(baseNode, true);
                                SyntaxNode argList = GetChildNode(baseNode);
                                baseType.SetGenericArgs(argList);
                                break;
                            default:
                                Console.WriteLine("unknown baseList node:" + baseNode.Kind());
                                break;
                        }
                        String baseName = baseType.GetSymbolFull();
                        if (baseName == "System::Exception") continue;
                        if (baseName == "System::Attribute") continue;
                        if (isClass(baseNode))
                            cls.bases.Add(baseType);
                        else
                            cls.ifaces.Add(baseType);
                        cls.addUsage(baseType.GetSymbolShort());
                        break;
                }
            }
        }

        private void getFlags(Flags flags, ISymbol symbol) {
            if (symbol == null) return;
            flags.Static = symbol.IsStatic;
            flags.Abstract = symbol.IsAbstract;
            flags.Virtual = symbol.IsVirtual;
            flags.Extern = symbol.IsExtern;
            flags.Override = symbol.IsOverride;
            flags.Definition = symbol.IsDefinition;
            flags.Sealed = symbol.IsSealed;
            Accessibility access = symbol.DeclaredAccessibility;
            if (access == Accessibility.Private) flags.Private = true;
            if (access == Accessibility.Protected) flags.Protected = true;
            if (access == Accessibility.Public) flags.Public = true;
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
                        field.variables = variableDeclaration(child, field);
                        foreach(var v in field.variables) {
                            if (v.equals != null) fieldEquals(v);
                        }
                        break;
                }
            }
            cls.fields.Add(field);
        }

        private void fieldEquals(Variable v) {
            if (field.Static) {
                if (cls.Namespace.Length > 0) {
                    v.Append(cls.Namespace);
                    v.Append("::");
                }
            }
            v.Append(cls.fullname);
            v.Append("::");
            v.Append(v.name);
            v.Append(" = ");
            SyntaxNode equalsChild = GetChildNode(v.equals);
            if (equalsChild.Kind() == SyntaxKind.ArrayInitializerExpression) {
                arrayInitNode(equalsChild, v, field.GetTypeDeclaration(false), field.arrays);
            } else {
                expressionNode(equalsChild, v, false, false, false, true);
            }
            v.Append(";\r\n");
        }

        private void propertyNode(SyntaxNode node) {
            // type, AccessorList -> {GetAccessorDeclaration, SetAccessorDeclaration}
            field = new Field();
            field.cls = cls;
            field.Property = true;
            Variable v = new Variable();
            field.variables.Add(v);  //property decl can only have 1 variable
            ISymbol symbol = file.model.GetDeclaredSymbol(node);
            v.name = symbol.Name;
            field.Public = true;
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
                                    method.type.Virtual = true;
                                    if (cls.Abstract) method.type.Abstract = true;
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
                                    method.type.Virtual = true;
                                    if (cls.Abstract) method.type.Abstract = true;
                                    field.set_Property = true;
                                    break;
                            }
                        }
                        break;
                }
            }
            cls.fields.Add(field);
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
                                    case "Qt::Core::weak":
                                        type.weakRef = true;
                                        break;
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
                                        method.type.Public = true;
                                        method.type.Private = false;
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
                                        SyntaxNode arg = GetChildNode(attrArgList);  //AttributeArgument
                                        SyntaxNode str = GetChildNode(arg);  //StringLiteralExpression
                                        String value = file.model.GetConstantValue(str).Value.ToString();
                                        cls.addUsage(value);
                                        break;
                                    }
                                }
                                break;
                        }
                        break;
                }
            }
        }

        private List<Variable> variableDeclaration(SyntaxNode node, Type type) {
            List<Variable> vars = new List<Variable>();
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach(var child in nodes) {
                switch (child.Kind()) {
                    case SyntaxKind.ArrayType:
                        type.array = true;
                        variableDeclaration(child, type);
                        break;
                    case SyntaxKind.ArrayRankSpecifier:
                        //assume it's OmittedArraySizeExpression
                        type.arrays++;
                        break;
                    case SyntaxKind.PointerType:
                        type.ptr = true;
                        variableDeclaration(child, type);
                        break;
                    case SyntaxKind.GenericName:
                        SyntaxNode typeList = GetChildNode(child);
                        foreach(var arg in typeList.ChildNodes()) {
                            type.GenericArgs.Add(new Type(arg, true));
                        }
                        type.set(GetSymbolFull(child));
                        type.setTypes();
                        break;
                    case SyntaxKind.PredefinedType:
                    case SyntaxKind.IdentifierName:
                    case SyntaxKind.QualifiedName:
                        ITypeSymbol typesym = file.model.GetTypeInfo(child).Type;
                        if (typesym != null) {
                            type.set(typesym.ToString().Replace(".", "::"));
                            type.typekind = typesym.TypeKind;
                        }
                        type.setTypes();
                        break;
                    case SyntaxKind.VariableDeclarator:
                        getFlags(type, file.model.GetDeclaredSymbol(child));
                        ISymbol symbol2 = file.model.GetDeclaredSymbol(child);
                        Variable var = new Variable();
                        vars.Add(var);
                        if (symbol2 != null) {
                            var.name = symbol2.Name;
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
            method.name = "$ctor";  //C++ = cls.name;
            method.type.set("void");
            method.type.cls = cls;
            method.type.primative = true;
            method.ctor = true;
            cls.hasctor = true;
            if (cls.bases.Count > 0) {
                method.type.Public = true;
                method.basector = cls.bases[0].GetSymbolShort() + "::$ctor();\r\n";
            }
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
                            method.Append(cls.bases[0].GetTypeType());
                            method.Append("::$ctor(");
                            outArgList(argList, method);
                            method.Append(");\r\n");
                            method.basector = method.src.ToString();
                            method.src.Length = 0;
                            break;
                        case SyntaxKind.Block:
                            blockNode(child, true);
                            break;
                    }
                }
            } else {
                method.src.Append("{");
                if (method.basector != null) method.src.Append(method.basector);
                method.src.Append("}\r\n");
            }
            method.type.setTypes();
            createNewMethod(cls, method.args, method.replaceArgs);
        }

        private void createNewMethod(Class cls, List<Argument> args, String replaceArgs) {
            Method method = new Method();
            method.type.Public = true;
            method.type.Static = true;
            method.name = "$new";
            method.type.set(cls.name);
            method.type.cls = cls;
            method.cls = cls;
            method.type.GenericArgs = cls.GenericArgs;
            method.Append("{\r\n");
            method.Append("std::shared_ptr<" + cls.GetTypeDeclaration() + ">$this = std::make_shared<" + cls.name);
            if (cls.Generic) {
                method.Append("<");
                bool first = true;
                foreach(var arg in cls.GenericArgs) {
                    if (!first) method.Append(","); else first = false;
                    method.Append(arg.GetTypeDeclaration());
                }
                method.Append(">");
            }
            method.Append(">(" + cls.ctorArgs + ");\r\n");
            method.Append("$this->$weak_this = $this;\r\n");
            method.Append("$this->$init();\r\n");
            method.Append("$this->$ctor(");
            if (replaceArgs == null) {
                if (args != null) {
                    bool first = true;
                    foreach(var arg in args) {
                        if (!first) method.Append(","); else first = false;
                        method.Append(arg.name.name);
                        method.args.Add(arg);
                    }
                }
            } else {
                method.replaceArgs = replaceArgs;
                String[] repArgs = replaceArgs.Split(",");
                bool first = true;
                foreach(var arg in repArgs) {
                    if (!first) method.Append(","); else first = false;
                    int idx = arg.LastIndexOf("*");
                    if (idx == -1) {
                      idx = arg.LastIndexOf(" ");
                    }
                    method.Append(arg.Substring(idx+1));
                }
            }
            method.Append(");\r\n");
            method.Append("return $this;\r\n");
            method.Append("}\r\n");
            method.type.setTypes();
            cls.methods.Add(method);
        }

        private void methodNode(SyntaxNode node, bool dtor, bool isDelegate, String name) {
            method = new Method();
            method.cls = cls;
            method.type.cls = cls;
            if (isDelegate) {
                method.isDelegate = true;
                method.Namespace = Namespace;
            }
            if (name != null) {
                method.name = name;
            } else {
                if (dtor) {
                    method.name = "~" + cls.name;
                    method.type.set("");
                } else {
                    method.name = file.model.GetDeclaredSymbol(node).Name;
                }
            }
            getFlags(method.type, file.model.GetDeclaredSymbol(node));
            if (dtor) {
                method.type.Protected = false;
                method.type.Virtual = true;
                method.type.Public = true;
            }
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            //nodes : [return type], parameter list, block
            foreach(var child in nodes) {
                switch (child.Kind()) {
                    case SyntaxKind.PredefinedType:
                    case SyntaxKind.IdentifierName:
                    case SyntaxKind.QualifiedName:
                        Type type = new Type(child, child.Kind() == SyntaxKind.PredefinedType);
                        ISymbol symbol = file.model.GetSymbolInfo(child).Symbol;
                        ISymbol declsymbol = file.model.GetDeclaredSymbol(child);
                        if (symbol == null && declsymbol == null) {
                            Console.WriteLine("Error:methodNode():Symbol not found for:" + child);
                            Environment.Exit(0);
                        }
                        if (symbol != null) {
                            method.type.set(symbol.ToString().Replace(".", "::"));
                        } else {
                            method.type.set(declsymbol.ToString().Replace(".", "::"));
                        }
                        ITypeSymbol typesym = file.model.GetTypeInfo(child).Type;
                        if (typesym != null) {
                            method.type.typekind = typesym.TypeKind;
                        }
                        method.type.node = child;
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
                        method.type.array = true;
                        foreach(var arrayType in child.ChildNodes()) {
                            parameterNode(arrayType, method.type);
                        }
                        break;
                    case SyntaxKind.GenericName:
                        SyntaxNode typeList = GetChildNode(child);
                        foreach(var arg in typeList.ChildNodes()) {
                            method.type.GenericArgs.Add(new Type(arg, true));
                        }
                        method.type.set(GetSymbolFull(child));
                        method.type.setTypes();
                        break;
                }
            }
            method.type.setTypes();
            cls.methods.Add(method);
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
                        arg.name.name = file.model.GetDeclaredSymbol(param).Name.Replace(".", "::");
                        arg.type = type;
                        method.args.Add(arg);
                        SyntaxNode equals = GetChildNode(param, 2);
                        if (equals != null && equals.Kind() == SyntaxKind.EqualsValueClause) {
                            expressionNode(GetChildNode(equals), arg.name);
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
                    ISymbol symbol = file.model.GetSymbolInfo(node).Symbol;
                    if (symbol != null) {
                        type.set(symbol.ToString().Replace(".", "::"));
                    }
                    ITypeSymbol typesym = file.model.GetTypeInfo(node).Type;
                    if (typesym != null) {
                        type.typekind = typesym.TypeKind;
                    }
                    type.setTypes();
                    break;
                case SyntaxKind.ArrayType:
                    type.array = true;
                    parameterNode(GetChildNode(node), type);
                    IEnumerable<SyntaxNode> ranks = node.DescendantNodes();
                    foreach(var rank in ranks) {
                        if (rank.Kind() == SyntaxKind.ArrayRankSpecifier) {
                            type.arrays++;
                        }
                    }
                    break;
                case SyntaxKind.ArrayRankSpecifier:
                    type.arrays++;
                    break;
                case SyntaxKind.GenericName:
                    SyntaxNode typeList = GetChildNode(node);
                    foreach(var arg in typeList.ChildNodes()) {
                        type.GenericArgs.Add(new Type(arg, true));
                    }
                    type.set(GetSymbolFull(node));
                    type.setTypes();
                    break;
                default:
                    Console.WriteLine("Unknown arg type:" + node.Kind());
                    break;
            }
        }

        private void blockNode(SyntaxNode node, bool top = false, bool throwFinally = false) {
            method.Append("{\r\n");
            if (top) {
                if (!method.type.Static) {
                    method.Append("std::shared_ptr<" + cls.GetTypeDeclaration() + "> $this = std::dynamic_pointer_cast<" + cls.GetTypeDeclaration() + ">(this->$weak_this.lock());\r\n");
                }
                if (method.basector != null) method.Append(method.basector);
            }
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach(var child in nodes) {
                statementNode(child);
            }
            if (throwFinally) {
                method.Append("throw Qt::QSharp::FinallyException::$new();");
            }
            method.Append("}\r\n");
        }

        private void statementNode(SyntaxNode node, bool top = false, bool throwFinally = false) {
            switch (node.Kind()) {
                case SyntaxKind.Block:
                    blockNode(node, top, throwFinally);
                    break;
                case SyntaxKind.ExpressionStatement:
                    expressionNode(GetChildNode(node), method);
                    method.Append(";\r\n");
                    break;
                case SyntaxKind.LocalDeclarationStatement:
                    expressionNode(GetChildNode(node), method);
                    method.Append(";\r\n");
                    break;
                case SyntaxKind.ReturnStatement:
                    method.Append("return ");
                    SyntaxNode returnValue = GetChildNode(node);
                    if (returnValue != null) expressionNode(returnValue , method);
                    method.Append(";\r\n");
                    break;
                case SyntaxKind.WhileStatement:
                    //while (expression) statement
                    method.Append("while (");
                    expressionNode(GetChildNode(node, 1), method);
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
                        expressionNode(GetChildNode(node, pos++), method);
                    }
                    method.Append(";");
                    if (HasCond) {
                        expressionNode(GetChildNode(node, pos++), method);
                    }
                    method.Append(";");
                    for(int idx=0;idx<IncreCount;idx++) {
                        if (idx > 0) method.Append(",");
                        expressionNode(GetChildNode(node, pos++), method);
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
                    Type foreachType = new Type(foreachTypeNode, false);
                    SyntaxNode foreachItems = GetChildNode(node, 2);
                    SyntaxNode foreachBlock = GetChildNode(node, 3);
                    String enumID = "$enum_" + cls.enumCnt++;
                    method.Append("{");
                    method.Append(foreachType.GetTypeDeclaration());  //var type
                    method.Append(" ");
                    method.Append(foreachName);  //var name : item
                    method.Append(";\r\n");
                    method.Append("std::shared_ptr<IEnumerator<");
                    method.Append(foreachType.GetTypeDeclaration());  //var type
                    method.Append(">> " + enumID + " = ");
                    expressionNode(foreachItems, method);  //items
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
                    expressionNode(GetChildNode(node, 2), method);
                    method.Append(");\r\n");
                    break;
                case SyntaxKind.IfStatement:
                    //if (expression) statement [else statement]
                    method.Append("if (");
                    expressionNode(GetChildNode(node, 1), method);
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
                                    method.Append(" catch(std::shared_ptr<");
                                    expressionNode(GetChildNode(catchDecl), method);  //exception type
                                    method.Append("> ");
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
                                method.Append("} catch(std::shared_ptr<Qt::QSharp::FinallyException> $finally" + cls.finallyCnt++ + ") ");
                                statementNode(GetChildNode(child));
                                break;
                        }
                    }
                    break;
                case SyntaxKind.ThrowStatement:
                    int tc = GetChildCount(node);
                    if (tc == 1) {
                        method.Append("throw ");
                        expressionNode(GetChildNode(node), method);
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
                                            arrayInitNode(equalsChild, method, type.GetTypeDeclaration(false), type.arrays);
                                        } else {
                                            expressionNode(equalsChild, method, false, false, false, true);
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
                        Console.WriteLine("Error:lock {} must use Qt.Core.ThreadLock");
                        Environment.Exit(0);
                    }
                    SyntaxNode lockBlock = GetChildNode(node, 2);
                    string holder = "$lock" + cls.lockCnt++;
                    method.Append("for($ThreadLockHolder " + holder + "(");
                    expressionNode(lockId, method);
                    method.Append(");" + holder + ".Condition();" + holder + ".Signal())");
                    blockNode(lockBlock);
                    break;
                case SyntaxKind.SwitchStatement:
                    // var, [SwitchSection...]
                    SyntaxNode var = GetChildNode(node);
                    if (GetTypeName(var) == "Qt::Core::String") {
                        switchString(node);
                        break;
                    }
                    method.Append("switch (");
                    expressionNode(var, method);
                    method.Append(") {\r\n");
                    foreach(var section in node.ChildNodes()) {
                        if (section.Kind() != SyntaxKind.SwitchSection) continue;
                        foreach(var child in section.ChildNodes()) {
                            switch (child.Kind()) {
                                case SyntaxKind.CaseSwitchLabel:
                                    method.Append("case ");
                                    expressionNode(GetChildNode(child), method);
                                    method.Append(":");
                                    break;
                                case SyntaxKind.DefaultSwitchLabel:
                                    method.Append("default:");
                                    break;
                                default:
                                    statementNode(child);
                                    break;
                            }
                        }
                    }
                    method.Append("}\r\n");
                    break;
                case SyntaxKind.BreakStatement:
                    method.Append("break;\r\n");
                    break;
                case SyntaxKind.ContinueStatement:
                    method.Append("continue;\r\n");
                    break;
                default:
                    Console.WriteLine("Error:Statement not supported:" + node.Kind());
                    Environment.Exit(0);
                    break;
            }
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
                        expressionNode(var, method);
                        method.Append("->Equals(");
                        expressionNode(GetChildNode(child), method);
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

        private void expressionNode(SyntaxNode node, OutputBuffer ob, bool lvalue = false, bool argument = false, bool invoke = false, bool assignment = false, bool first = false) {
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            Type type;
            switch (node.Kind()) {
                case SyntaxKind.IdentifierName:
                case SyntaxKind.PredefinedType:
                case SyntaxKind.QualifiedName:
                    type = new Type(node, node.Kind() == SyntaxKind.PredefinedType || first);
                    if (isProperty(node)) {
                        if (lvalue)
                            ob.Append("$set_" + type.type);
                        else
                            ob.Append("$get_" + type.type + "()");
                        break;
                    } else {
                        if ((argument || assignment) && isMethod(node)) {
                            if (isStatic(node)) {
                                ob.Append("&");
                                ob.Append(type.GetTypeType(first));
                            } else {
                                type = new Type(node, true);
                                ob.Append("std::bind(&");
                                ob.Append(type.GetTypeType(true));
                                ob.Append(", $this");
                                //add std::placeholders::_1, ... for # of arguments to delegate
                                int numArgs = GetNumArgs(node);
                                for(int a=0;a<numArgs;a++) {
                                    ob.Append(", std::placeholders::_" + (a+1));
                                }
                                ob.Append(")");
                            }
                        } else {
                            ob.Append(type.GetTypeType(first));
                        }
                    }
                    break;
                case SyntaxKind.GenericName:
                    type = new Type(node, true);
                    SyntaxNode typeList = GetChildNode(node);
                    foreach(var arg in typeList.ChildNodes()) {
                        type.GenericArgs.Add(new Type(arg, true));
                    }
                    ob.Append(type.GetTypeType(true));
                    break;
                case SyntaxKind.VariableDeclaration:
                    //local variable
                    type = new Type();
                    List<Variable> vars = variableDeclaration(node, type);
                    method.Append(type.GetTypeDeclaration());
                    method.Append(" ");
                    bool firstLocal = true;
                    foreach(var variable in vars) {
                        if (!firstLocal) method.Append(","); else firstLocal = false;
                        method.Append(variable.name);
                        SyntaxNode equals = variable.equals;
                        if (equals != null) {
                            method.Append(" = ");
                            SyntaxNode equalsChild = GetChildNode(equals);
                            if (equalsChild.Kind() == SyntaxKind.ArrayInitializerExpression) {
                                arrayInitNode(equalsChild, method, type.GetTypeDeclaration(false), type.arrays);
                            } else {
                                expressionNode(equalsChild, method, false, false, false, true);
                            }
                        }
                    }
                    break;
                case SyntaxKind.SimpleAssignmentExpression:
                    assignNode(node);
                    break;
                case SyntaxKind.InvocationExpression:
                    invokeNode(node, ob);
                    break;
                case SyntaxKind.ArrayCreationExpression:
                    newArrayNode(node, ob);
                    break;
                case SyntaxKind.NullLiteralExpression:
                    ob.Append("nullptr");
                    break;
                case SyntaxKind.NumericLiteralExpression:
                    String value = file.model.GetConstantValue(node).Value.ToString();
                    String valueType = GetTypeName(node);
                    switch (valueType) {
                        case "float":
                            if (value.IndexOf(".") == -1) value += ".0";
                            value += "f";
                            break;
                        case "double":
                            if (value.IndexOf(".") == -1) value += ".0";
                            break;
                    }
                    ob.Append(value);
                    break;
                case SyntaxKind.TrueLiteralExpression:
                    ob.Append("true");
                    break;
                case SyntaxKind.FalseLiteralExpression:
                    ob.Append("false");
                    break;
                case SyntaxKind.StringLiteralExpression:
                    ob.Append("Qt::Core::String::$new(");
                    ob.Append("\"");
                    outString(file.model.GetConstantValue(node).Value.ToString(), ob);
                    ob.Append("\"");
                    ob.Append(")");
                    break;
                case SyntaxKind.CharacterLiteralExpression:
                    ob.Append("(char16)\'");
                    outString(file.model.GetConstantValue(node).Value.ToString(), ob);
                    ob.Append("\'");
                    break;
                case SyntaxKind.SimpleMemberAccessExpression:
                    SyntaxNode left = GetChildNode(node, 1);
                    SyntaxNode right = GetChildNode(node, 2);
                    if ((!invoke || assignment) && isMethod(right)) {
                        //delegate method
                        if (isStatic(right)) {
                            ob.Append("&");
                            expressionNode(left, ob);
                            ob.Append("::");
                            expressionNode(right, ob);
                        } else {
                            int numArgs = GetNumArgs(right);
                            ob.Append("std::bind(&");
                            ITypeSymbol dtype = file.model.GetTypeInfo(left).Type;
                            ob.Append(dtype.ToString().Replace(".", "::"));
                            ob.Append("::");
                            expressionNode(right, ob);
                            ob.Append(",");
                            expressionNode(left, ob);  //'this'
                            for(int a=0;a<numArgs;a++) {
                                ob.Append(", std::placeholders::_" + (a+1));
                            }
                            ob.Append(")");
                        }
                        break;
                    }
                    if (isStatic(right) || left.Kind() == SyntaxKind.BaseExpression || isEnum(left) || isNamespace(left) || (isNamedType(left) && isNamedType(right))) {
                        expressionNode(left, ob, lvalue, false, false, false, true);
                        ob.Append("::");
                        expressionNode(right, ob, lvalue);
                    } else {
                        ob.Append("$check(");  //NPE check
                        expressionNode(left, ob, lvalue);
                        ob.Append(")->");
                        expressionNode(right, ob, lvalue);
                    }
                    break;
                case SyntaxKind.BaseExpression:
                    ob.Append(cls.bases[0].full);
                    break;
                case SyntaxKind.ObjectCreationExpression:
                    invokeNode(node, ob, true, true);
                    break;
                case SyntaxKind.CastExpression:
                    castNode(node, ob);
                    break;
                case SyntaxKind.ElementAccessExpression:
                    //IdentifierNode, BracketedArgumentList -> {Argument, ...}
                    SyntaxNode array = GetChildNode(node, 1);
                    SyntaxNode index = GetChildNode(node, 2);
                    ob.Append("(*$check(");  //NPE check
                    expressionNode(array, ob, lvalue);
                    ob.Append("))[");
                    expressionNode(index, ob, lvalue);
                    ob.Append("]");
                    break;
                case SyntaxKind.BracketedArgumentList:
                    foreach(var child in nodes) {
                        expressionNode(child, ob, lvalue);
                    }
                    break;
                case SyntaxKind.Argument:
                    expressionNode(GetChildNode(node), ob, lvalue);
                    break;
                case SyntaxKind.AddExpression:
                    ob.Append("$add(");
                    expressionNode(GetChildNode(node, 1), ob, lvalue);
                    ob.Append(",");
                    expressionNode(GetChildNode(node, 2), ob, lvalue);
                    ob.Append(")");
                    break;
                case SyntaxKind.SubtractExpression:
                    binaryNode(node, ob, "-");
                    break;
                case SyntaxKind.MultiplyExpression:
                    binaryNode(node, ob, "*");
                    break;
                case SyntaxKind.DivideExpression:
                    binaryNode(node, ob, "/");
                    break;
                case SyntaxKind.ModuloExpression:
                    modNode(node, ob, "%");
                    break;
                case SyntaxKind.ModuloAssignmentExpression:
                    modAssignNode(node, ob, "%");
                    break;
                case SyntaxKind.LessThanExpression:
                    binaryNode(node, ob, "<");
                    break;
                case SyntaxKind.LessThanOrEqualExpression:
                    binaryNode(node, ob, "<=");
                    break;
                case SyntaxKind.GreaterThanExpression:
                    binaryNode(node, ob, ">");
                    break;
                case SyntaxKind.GreaterThanOrEqualExpression:
                    binaryNode(node, ob, ">=");
                    break;
                case SyntaxKind.EqualsExpression:
                    equalsNode(node, ob, "==");
                    break;
                case SyntaxKind.NotEqualsExpression:
                    equalsNode(node, ob, "!=");
                    break;
                case SyntaxKind.LeftShiftExpression:
                    binaryNode(node, ob, "<<");
                    break;
                case SyntaxKind.RightShiftExpression:
                    binaryNode(node, ob, ">>");
                    break;
                case SyntaxKind.AddAssignmentExpression:
                    expressionNode(GetChildNode(node, 1), ob, true);
                    ob.Append("= $add(");
                    expressionNode(GetChildNode(node, 1), ob);
                    ob.Append(",");
                    expressionNode(GetChildNode(node, 2), ob);
                    ob.Append(")");
                    break;
                case SyntaxKind.SubtractAssignmentExpression:
                    binaryAssignNode(node, ob, "-");
                    break;
                case SyntaxKind.MultiplyAssignmentExpression:
                    binaryAssignNode(node, ob, "*");
                    break;
                case SyntaxKind.DivideAssignmentExpression:
                    binaryAssignNode(node, ob, "/");
                    break;
                case SyntaxKind.OrAssignmentExpression:
                    binaryAssignNode(node, ob, "|");
                    break;
                case SyntaxKind.AndAssignmentExpression:
                    binaryAssignNode(node, ob, "&");
                    break;
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                    binaryAssignNode(node, ob, "^");
                    break;
                case SyntaxKind.ExclusiveOrExpression:
                    binaryNode(node, ob, "^");
                    break;
                case SyntaxKind.LogicalNotExpression:
                    ob.Append("!");
                    expressionNode(GetChildNode(node), ob);
                    break;
                case SyntaxKind.LogicalOrExpression:
                    binaryNode(node, ob, "||");
                    break;
                case SyntaxKind.LogicalAndExpression:
                    binaryNode(node, ob, "&&");
                    break;
                case SyntaxKind.BitwiseOrExpression:
                    if (isEnum(GetChildNode(node))) {
                        //enums convert to int which must be type casted back to enum type
                        ob.Append("(");
                        ob.Append(GetTypeName(GetChildNode(node)));
                        ob.Append(")");
                    }
                    ob.Append("(");
                    binaryNode(node, ob, "|");
                    ob.Append(")");
                    break;
                case SyntaxKind.BitwiseAndExpression:
                    binaryNode(node, ob, "&");
                    break;
                case SyntaxKind.BitwiseNotExpression:
                    ob.Append("!");
                    expressionNode(GetChildNode(node), ob);
                    break;
                case SyntaxKind.LeftShiftAssignmentExpression:
                    binaryAssignNode(node, ob, "<<");
                    break;
                case SyntaxKind.RightShiftAssignmentExpression:
                    binaryAssignNode(node, ob, ">>");
                    break;
                case SyntaxKind.ParenthesizedExpression:
                    ob.Append("(");
                    expressionNode(GetChildNode(node), ob);
                    ob.Append(")");
                    break;
                case SyntaxKind.PostIncrementExpression:
                    expressionNode(GetChildNode(node), ob);
                    ob.Append("++");
                    break;
                case SyntaxKind.PreIncrementExpression:
                    ob.Append("++");
                    expressionNode(GetChildNode(node), ob);
                    break;
                case SyntaxKind.PostDecrementExpression:
                    expressionNode(GetChildNode(node), ob, true);
                    ob.Append("--");
                    break;
                case SyntaxKind.PreDecrementExpression:
                    ob.Append("--");
                    expressionNode(GetChildNode(node), ob, true);
                    break;
                case SyntaxKind.UnaryMinusExpression:
                    ob.Append("-");
                    expressionNode(GetChildNode(node), ob);
                    break;
                case SyntaxKind.UnaryPlusExpression:
                    ob.Append("+");
                    expressionNode(GetChildNode(node), ob);
                    break;
                case SyntaxKind.ThisExpression:
                    ob.Append("$this");
                    break;
                case SyntaxKind.PointerIndirectionExpression:
                    ob.Append("*");
                    expressionNode(GetChildNode(node), ob);
                    break;
                case SyntaxKind.PointerMemberAccessExpression:
                    SyntaxNode ptrleft = GetChildNode(node, 1);
                    SyntaxNode ptrright = GetChildNode(node, 2);
                    expressionNode(ptrleft, ob);
                    ob.Append("->");
                    expressionNode(ptrright, ob);
                    break;
                case SyntaxKind.ParenthesizedLambdaExpression:
                    // ParameterList, Block
                    SyntaxNode plist = GetChildNode(node, 1);
                    SyntaxNode pblock = GetChildNode(node, 2);
                    ob.Append("[&]");
                    //output parameter list
                    bool firstLambda = true;
                    ob.Append("(");
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
                                if (!firstLambda) ob.Append(","); else firstLambda = false;
                                ob.Append(ptype.GetTypeDeclaration());
                                ob.Append(" ");
                                ob.Append(pvar.name);
                                break;
                        }
                    }
                    ob.Append(")");
                    blockNode(pblock);
                    break;
                case SyntaxKind.DefaultExpression:
                    expressionNode(GetChildNode(node), ob);
                    ob.Append("()");
                    break;
                case SyntaxKind.TypeOfExpression:
                    SyntaxNode typeOf = GetChildNode(node);
                    Type typeSymbol = new Type(typeOf, true);
                    ob.Append("Type::$new(&$class_" + typeSymbol.full_name + ")");
                    break;
                case SyntaxKind.IsExpression:
                    SyntaxNode isObj = GetChildNode(node, 1);
                    SyntaxNode isType = GetChildNode(node, 2);
                    Type isTypeType = new Type(isType, false);
                    ob.Append("$check(");
                    expressionNode(isObj, ob);
                    ob.Append(")->GetType()");
                    ob.Append("->IsDerivedFrom(");
                    ob.Append("Type::$new(&$class_" + isTypeType.full_name + "))");
                    break;
                case SyntaxKind.AsExpression:
                    SyntaxNode asObj = GetChildNode(node, 1);
                    SyntaxNode asType = GetChildNode(node, 2);
                    Type asTypeType = new Type(asType, false);
                    ob.Append("(Type::$new(&$class_" + asTypeType.full_name + ")");
                    ob.Append("->IsDerivedFrom($check(");
                    expressionNode(asObj, ob);
                    ob.Append(")->GetType()) ? std::dynamic_pointer_cast<" + asTypeType.full_name + ">(");
                    expressionNode(asObj, ob);
                    ob.Append(") : nullptr)");
                    break;
                default:
                    Console.WriteLine("Error:Unsupported expression:" + node.Kind());
                    Environment.Exit(0);
                    break;
            }
        }

        private void outString(String ch, OutputBuffer ob) {
            ob.Append(ch.Replace("\0", "\\0").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t"));
        }

        private void arrayInitNode(SyntaxNode node, OutputBuffer ob, String type, int dims) {
            IEnumerable<SyntaxNode> list = node.ChildNodes();
            bool first = true;
            for(int a=0;a<dims;a++) {
                if (a == 0)
                    ob.Append("Qt::QSharp::FixedArray<");
                else
                    ob.Append("std::shared_ptr<Qt::QSharp::FixedArray<");
            }
            ob.Append(type);
            for(int a=0;a<dims;a++) {
                if (a == 0)
                    ob.Append(">");
                else
                    ob.Append(">>");
            }
            ob.Append("::$new(std::initializer_list<");
            for(int a=1;a<dims;a++) {
                ob.Append("std::shared_ptr<Qt::QSharp::FixedArray<");
            }
            ob.Append(type);
            for(int a=1;a<dims;a++) {
                ob.Append(">>");
            }
            ob.Append(">{");
            foreach(var elem in list) {
                if (!first) ob.Append(","); else first = false;
                expressionNode(elem, ob);
            }
            ob.Append("})");
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
            Environment.Exit(0);
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
            String name = symbol.Name;
            if (method != null) {
                if (method.name == "$set_" + name) return false;
                if (method.name == "$get_" + name) return false;
            }
            return (symbol.Kind == SymbolKind.Property);
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

        private void binaryNode(SyntaxNode node, OutputBuffer ob, string op) {
            expressionNode(GetChildNode(node, 1), ob);
            ob.Append(op);
            expressionNode(GetChildNode(node, 2), ob);
        }

        private void equalsNode(SyntaxNode node, OutputBuffer ob, string op) {
            SyntaxNode left = GetChildNode(node, 1);
            SyntaxNode right = GetChildNode(node, 2);
            bool useEquals = false;
            bool leftString = false;
            bool rightString = false;
            if (GetTypeName(left) == "Qt::Core::String") leftString = true;
            else if (GetTypeName(left) == "string") leftString = true;
            if (GetTypeName(right) == "Qt::Core::String") rightString = true;
            else if (GetTypeName(right) == "string") rightString = true;
            if (leftString && rightString) {
                useEquals = true;
            }
            if (GetTypeName(left) == "Qt::Core::Type" && GetTypeName(right) == "Qt::Core::Type") {
                useEquals = true;
            }
            if (useEquals) {
                if (op == "!=") ob.Append("!");
                ob.Append("$check(");
                expressionNode(left, ob);
                ob.Append(")->Equals(");
                expressionNode(right, ob);
                ob.Append(")");
            } else {
                binaryNode(node, ob, op);
            }
        }

        private void binaryAssignNode(SyntaxNode node, OutputBuffer ob, string op) {
            expressionNode(GetChildNode(node, 1), ob, true);
            ob.Append(" = ");
            expressionNode(GetChildNode(node, 1), ob);
            ob.Append(op);
            expressionNode(GetChildNode(node, 2), ob);
        }

        //C++ does not support float % -- must use a special function
        private void modNode(SyntaxNode node, OutputBuffer ob, string op) {
            ob.Append("$mod(");
            expressionNode(GetChildNode(node, 1), ob);
            ob.Append(",");
            expressionNode(GetChildNode(node, 2), ob);
            ob.Append(")");
        }

        //C++ does not support float % -- must use a special function
        private void modAssignNode(SyntaxNode node, OutputBuffer ob, string op) {
            expressionNode(GetChildNode(node, 1), ob, true);
            ob.Append("= $mod(");
            expressionNode(GetChildNode(node, 1), ob);
            ob.Append(",");
            expressionNode(GetChildNode(node, 2), ob);
            ob.Append(")");
        }

        private void castNode(SyntaxNode node, OutputBuffer ob) {
            SyntaxNode castType = GetChildNode(node, 1);
            SyntaxNode value = GetChildNode(node, 2);
            //cast value to type
            //C# (type)value
            //C++ std::dynamic_pointer_cast<type>(value)
            Type type = new Type(castType, true);
            if (type.shared) {
                ob.Append("std::dynamic_pointer_cast<");
                String typestr = type.GetTypeDeclaration();
                typestr = typestr.Substring(16, typestr.Length - 17);  //remove outter std::shared_ptr< ... >
                ob.Append(typestr);
                ob.Append(">");
                ob.Append("(");
                expressionNode(value, ob);
                ob.Append(")");
            } else {
                ob.Append("static_cast<");
                ob.Append(type.GetTypeDeclaration());
                ob.Append(">");
                ob.Append("(");
                expressionNode(value, ob);
                ob.Append(")");
            }
        }

        private void newArrayNode(SyntaxNode node, OutputBuffer ob) {
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
                                if (sizeNode != null) Console.WriteLine("Error:multiple sizes for ArrayCreationExpression");
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
                //ob.Append("=");
                Type dataType = new Type(typeNode, true);
                arrayInitNode(initList, ob, dataType.GetTypeDeclaration(false), dims);
                return;
            }
            if (typeNode == null || sizeNode == null) {
                Console.WriteLine("Error:Invalid ArrayCreationExpression : " + typeNode + " : " + sizeNode);
                return;
            }
            for(int a=0;a<dims;a++) {
              ob.Append("Qt::QSharp::FixedArray<");
            }
            Type type = new Type(typeNode, true);
            ob.Append(type.GetTypeDeclaration());
            for(int a=0;a<dims;a++) {
                ob.Append(">");
            }
            ob.Append("::$new(");
            expressionNode(sizeNode, ob);
            ob.Append(")");
        }

        private void assignNode(SyntaxNode node) {
            //lvalue = rvalue
            IEnumerator<SyntaxNode> nodes = node.ChildNodes().GetEnumerator();
            nodes.MoveNext();
            SyntaxNode left = nodes.Current;
            nodes.MoveNext();
            SyntaxNode right = nodes.Current;
            if (isProperty(left)) {
                expressionNode(left, method, true);
                method.Append("(");
                expressionNode(right, method, false, false, false, true);
                method.Append(")");
            } else {
                expressionNode(left, method);
                method.Append(" = ");
                expressionNode(right, method, false, false, false, true);
            }
        }

        private void invokeNode(SyntaxNode node, OutputBuffer ob, bool New = false, bool first = false) {
            //IdentifierName/SimpleMemberAccessExpression/QualifiedName, ArgumentList
            SyntaxNode id = GetChildNode(node, 1);
            SyntaxNode args = GetChildNode(node, 2);
            if (isCPP(id)) {
                SyntaxNode arg = GetChildNode(args);  //Argument
                SyntaxNode str = GetChildNode(arg);  //StringLiteralExpression
                ob.Append(file.model.GetConstantValue(str).Value.ToString());
                return;
            }
            if (isDelegate(id)) {
                ob.Append("$check(");
            }
            expressionNode(id, ob, true, false, true, false, first);
            if (isDelegate(id)) {
                ob.Append(")");
            }
            if (New) {
                ob.Append("::$new");
            }
            ob.Append("(");
            outArgList(args, ob);
            ob.Append(")");
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
        private void outArgList(SyntaxNode node, OutputBuffer ob) {
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            bool first = true;
            foreach(var child in nodes) {
                if (!first) ob.Append(","); else first = false;
                switch (child.Kind()) {
                    case SyntaxKind.Argument:
                        expressionNode(GetChildNode(child), ob, false, true);
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

        public static string GetSymbolFull(SyntaxNode node) {
            ISymbol symbol = file.model.GetSymbolInfo(node).Symbol;
            if (symbol != null) return symbol.ToString().Replace(".", "::");
            return null;
        }

        private string GetDeclaredSymbol(SyntaxNode node) {
            ISymbol symbol = file.model.GetDeclaredSymbol(node);
            if (symbol != null) return symbol.Name.Replace(".", "::");
            return null;
        }

        private string GetTypeName(SyntaxNode node) {
            ITypeSymbol type = file.model.GetTypeInfo(node).Type;
            if (type == null) return "";
            return type.ToString().Replace(".", "::");
        }

        private TypeKind GetTypeKind(SyntaxNode node) {
            ITypeSymbol type = file.model.GetTypeInfo(node).Type;
            if (type == null) return TypeKind.Error;
            return type.TypeKind;
        }
    }

    interface OutputBuffer {
        void Append(string s);
        void Insert(int idx, string s);
        int Length();
        bool isField();
        bool isMethod();
    }

    class Flags
    {
        public bool Public;
        public bool Private;
        public bool Protected;
        public bool Static;
        public bool Abstract;
        public bool Virtual;
        public bool Extern;
        public bool Override;
        public bool Definition;
        public bool Sealed;
        public string GetFlags(bool cls) {
            StringBuilder sb = new StringBuilder();
            if (Public) sb.Append("public:");
            if (Private) sb.Append("private:");
            if (Protected) sb.Append("protected:");
            if (Static) sb.Append(" static");
            if (Abstract) {
                if (!cls) {
                    if (!Virtual) {
                        sb.Append(" virtual");
                    }
                }
            }
            if (Virtual) sb.Append(" virtual");
            if (Extern) sb.Append(" extern");
            return sb.ToString();
        }
        public void CopyFlags(Flags src) {
            Public = src.Public;
            Private = src.Private;
            Protected = src.Protected;
            Static = src.Static;
            Abstract = src.Abstract;
            Virtual = src.Virtual;
            Extern = src.Extern;
            Override = src.Override;
            Definition = src.Definition;
            Sealed = src.Sealed;
        }
    }

    class Enum {
        public Enum(string src, string Namespace) {
            src = src.Substring(src.IndexOf("enum"));  //remove public, private, etc.
            this.src = src;
            this.Namespace = Namespace;
        }
        public string src;
        public string Namespace;
    }

    class Class : Flags
    {
        public string name = "";
        public string fullname = "";  //inner classes
        public string Namespace = "";
        public bool hasctor;
        public bool Interface;
        public List<Type> bases = new List<Type>();
        public List<string> cppbases = new List<string>();
        public List<Type> ifaces = new List<Type>();
        public List<Field> fields = new List<Field>();
        public List<Method> methods = new List<Method>();
        public List<Enum> enums = new List<Enum>();
        public List<Class> inners = new List<Class>();
        public Class outter;
        public int lockCnt;
        public int finallyCnt;
        public int enumCnt;
        public int switchStringCnt;
        public bool Generic;
        public List<Type> GenericArgs = new List<Type>();
        public string cpp, ctorArgs = "", nonClassCPP, nonClassHPP;
        public bool omitFields, omitMethods, omitConstructors, omitBodies;
        public string forward;
        //uses are used to sort classes
        public List<string> uses = new List<string>();
        public void addUsage(string cls) {
            int idx = cls.LastIndexOf("::");
            if (idx != -1) cls = cls.Substring(idx+2);
            idx = cls.LastIndexOf("<");
            if (idx != -1) cls = cls.Substring(0, idx);
            if (cls == name) return;
            if (!uses.Contains(cls)) {
//                Console.WriteLine("Class.AddUsage:" + name + " uses " + cls);
                uses.Add(cls);
            }
        }
        public string GetTypeDeclaration() {
            StringBuilder sb = new StringBuilder();
            sb.Append(name);
            if (Generic) {
                sb.Append("<");
                bool first = true;
                foreach(var arg in GenericArgs) {
                    if (!first) sb.Append(","); else first = false;
                    sb.Append(arg.type);
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
            if (Generic) {
                sb.Append("template<");
                bool first = true;
                foreach(var arg in GenericArgs) {
                    if (!first) sb.Append(","); else first = false;
                    sb.Append("typename ");
                    sb.Append(arg.GetTypeDeclaration());
                }
                sb.Append(">\r\n");
            }
            sb.Append("class " + name);
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
                sb.Append("$method $method_" + full_name + "_" + name + (idx++) + "(\"" + m.name + "\");\r\n");
            }
            //reflection data : class
            sb.Append("$class $class_" + full_name + "(");
            if (Interface) {
                sb.Append("true,");
            } else {
                sb.Append("false,");
            }
            sb.Append("\"" + name + "\",");
            if (bases.Count == 0) {
                sb.Append("nullptr,");
            } else {
                sb.Append("&$class_" + bases[0].full_name + ",");
            }
            //relection data : interfaces list
            sb.Append("{");
            first = true;
            foreach(var i in ifaces) {
                if (!first) sb.Append(","); else first = false;
                sb.Append("&$class_" + i.full_name);
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
            sb.Append(");\r\n");
            foreach(var inner in inners) {
                sb.Append(inner.GetReflectionData());
            }
            return sb.ToString();
        }
        public string GetDeclaration() {
            StringBuilder sb = new StringBuilder();
            bool first;
            String full_name = FullName(Namespace, fullname);
            if (Generic) {
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
            sb.Append(" class " + name);
            if (bases.Count > 0 || cppbases.Count > 0 || ifaces.Count > 0) {
                sb.Append(":");
                first = true;
                foreach(var basecls in bases) {
                    if (!first) sb.Append(","); else first = false;
                    sb.Append("public ");
                    sb.Append(basecls.GetTypeType(true));
                }
                foreach(var cppcls in cppbases) {
                    if (!first) sb.Append(","); else first = false;
                    sb.Append("public ");
                    sb.Append(cppcls);
                }
                foreach(var iface in ifaces) {
                    if (!first) sb.Append(","); else first = false;
                    sb.Append("public ");
                    sb.Append(iface.GetTypeType(true));
                }
            }
            sb.Append("{\r\n");
            foreach(var inner in inners) {
                sb.Append(inner.GetDeclaration());
            }
            if (cpp != null) sb.Append(cpp);
            if (!Interface) {
                sb.Append("public: virtual $class* $getType() {return &$class_" + full_name + ";}\r\n");
            }
            foreach(var field in fields) {
                sb.Append(field.GetDeclaration());
            }
            foreach(var method in methods) {
                if (method.version != null) {
                    sb.Append("#if QT_VERSION >= " + method.version + "\r\n");
                }
                sb.Append(method.GetDeclaration());
                if (Generic) {
                    if (method.name == "$init") {
                        sb.Append("{\r\n");
                        if (method.cls.bases.Count > 0) {
                            sb.Append(method.cls.bases[0].GetSymbolShort() + "::$init();\r\n");
                        }
                        foreach(var field in fields) {
                            foreach(var v in field.variables) {
                                if (v.Length() > 0 && !field.Static) {
                                    sb.Append(v.src);
                                }
                            }
                        }
                        sb.Append("}\r\n");
                    } else {
                        sb.Append(method.src);
                    }
                }
                sb.Append(";\r\n");
                if (method.version != null) {
                    sb.Append("#endif\r\n");
                }
            }
            foreach(var e in enums) {
                sb.Append(e.src);
                sb.Append(";\r\n");
            }
            sb.Append("};\r\n");
            return sb.ToString();
        }
        public void GetInnerStaticFields(StringBuilder sb) {
            foreach(var inner in inners) {
                foreach(var field in inner.fields) {
                    if (!field.Static) continue;
                    foreach(var v in field.variables) {
                        sb.Append(field.GetTypeDeclaration() + " " + inner.fullname + "::" + v.name);
                        if (field.IsNumeric()) {
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
                    if (!field.Static) continue;
                    foreach(var v in field.variables) {
                        sb.Append(v.src);
                    }
                }
                inner.GetInnerStaticFields(sb);
            }
        }
        public string GetStaticFields() {
            StringBuilder sb = new StringBuilder();
            foreach(var field in fields) {
                if (!field.Static) continue;
                foreach(var v in field.variables) {
                    sb.Append(field.GetTypeDeclaration() + " " + name + "::" + v.name);
                    if (field.IsNumeric()) {
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
                if (!field.Static) continue;
                foreach(var v in field.variables) {
                    sb.Append(v.src);
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
                if (method.version != null) {
                    sb.Append("#if QT_VERSION >= " + method.version + "\r\n");
                }
                sb.Append(method.type.GetTypeDeclaration());
                sb.Append(" ");
                sb.Append(method.cls.fullname);
                sb.Append("::");
                sb.Append(method.name);
                sb.Append(method.GetArgs(false));
                if (method.Length() == 0) method.Append("{}\r\n");
                if (method.name == "$init") {
                    sb.Append("{\r\n");
                    if (method.cls.bases.Count > 0) {
                        sb.Append(method.cls.bases[0].GetSymbolShort() + "::$init();\r\n");
                    }
                    foreach(var field in method.cls.fields) {
                        foreach(var v in field.variables) {
                            if (v.Length() > 0 && !field.Static) {
                                sb.Append(v.src);
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
        public string type;
        public string full;
        public string full_name;
        public TypeKind typekind;
        public SymbolKind symbolkind;
        public SyntaxNode node;
        public ISymbol symbol, declSymbol;
        public ITypeSymbol typeSymbol;
        public bool primative;
        public bool numeric;
        public bool weakRef;
        public bool array;
        public int arrays;  //# of dimensions
        public bool shared;
        public bool ptr;  //unsafe pointer
        public List<Type> GenericArgs = new List<Type>();
        public Class cls;

        public virtual bool isField() {return false;}
        public virtual bool isMethod() {return false;}

        public Type() {}
        public Type(SyntaxNode node, bool get_full_name) {
            this.node = node;
            while (node.Kind() == SyntaxKind.ArrayType) {
                array = true;
                foreach(var child in node.ChildNodes()) {
                    if (child.Kind() == SyntaxKind.ArrayRankSpecifier) {
                        arrays++;
                    }
                }
                node = Generate.GetChildNode(node);
            }
            symbol = Generate.file.model.GetSymbolInfo(node).Symbol;
            if (symbol != null) {
                if (get_full_name)
                    set(symbol.ToString().Replace(".", "::"));
                else
                    set(symbol.Name.Replace(".", "::"));
                symbolkind = symbol.Kind;
            } else {
                declSymbol = Generate.file.model.GetDeclaredSymbol(node);
                if (declSymbol != null) {
                    set(declSymbol.ToString().Replace(".", "::"));
                } else {
                    set(node.ToString().Replace(".", "::"));
                }
            }
            typeSymbol = Generate.file.model.GetTypeInfo(node).Type;
            if (typeSymbol != null) {
                typekind = typeSymbol.TypeKind;
            }
            setTypes();
        }
        public Type(String sym) {
            set(sym);
            setTypes();
        }

/*
        public string type;
        public string full;
        public string full_name;
        public TypeKind typekind;
        public SymbolKind symbolkind;
        public SyntaxNode node;
        public ISymbol symbol, declSymbol;
        public ITypeSymbol typeSymbol;
        public bool primative;
        public bool numeric;
        public bool weakRef;
        public bool array;
        public int arrays;  //# of dimensions
        public bool shared;
        public bool ptr;  //unsafe pointer
        public List<Type> GenericArgs = new List<Type>();
        public Class cls;
 */
        public void CopyType(Type src) {
            type = src.type;
            full = src.full;
            full_name = src.full_name;
            typekind = src.typekind;
            symbolkind = src.symbolkind;
            node = src.node;
            symbol = src.symbol;
            declSymbol = src.declSymbol;
            typeSymbol = src.typeSymbol;
            primative = src.primative;
            numeric = src.numeric;
            weakRef = src.weakRef;
            array = src.array;
            arrays = src.arrays;
            shared = src.shared;
            ptr = src.ptr;
            GenericArgs = src.GenericArgs;
            cls = src.cls;
        }
        public String GetSymbolFull() {
            return full;
        }
        public String GetSymbolShort() {
            return type;
        }
        public void set(String sym) {
            int idx;
            full = sym;
            idx = full.IndexOf("<");
            if (idx != -1) full = full.Substring(0, idx);
            idx = full.IndexOf("(");
            if (idx != -1) full = full.Substring(0, idx);
            full_name = full.Replace("::", "_");
//            full = "::" + full;
            idx = full.LastIndexOf("::");
            if (idx == -1) {
                type = full;
            } else {
                type = full.Substring(idx+2);
            }
        }
        public void setTypes() {
            switch (type) {
                case "":
                case "void":
                    primative = true;
                    numeric = false;
                    shared = false;
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
                    primative = true;
                    numeric = true;
                    shared = false;
                    break;
                default:
                    primative = false;
                    shared = true;
                    switch (typekind) {
                        case TypeKind.Delegate: shared = false; break;
                        case TypeKind.Enum: shared = false; break;
                        case TypeKind.TypeParameter: shared = false; break;
                    }
                    if (node != null) {
                        switch (node.Kind()) {
                            case SyntaxKind.TypeParameter: shared = false; break;
                        }
                    }
                    break;
            }
/*
            switch (typekind) {
                case TypeKind.Delegate:
                    isTypeDelegate = true;
                    if (symbolkind == SymbolKind.NamedType) {
                        int idx = type.LastIndexOf("::");
                        if (idx == -1) {
                            type = "$delegate" + type;
                        } else {
                            type = type.Substring(0, idx+2) + "$delegate" + type.Substring(idx + 2);
                        }
                    }
                    break;
            }
*/
        }
        public void SetGenericArgs(SyntaxNode argList) {
            GenericArgs = new List<Type>();
            foreach(var arg in argList.ChildNodes()) {
                GenericArgs.Add(new Type(arg, true));
            }
        }
        public bool IsNumeric() {
            return numeric;
        }
        public bool isPrimative() {
            return primative;
        }
        public bool isDelegate() {
            return typekind == TypeKind.Delegate;
        }
        public bool isSymbolMethod() {
            return symbolkind == SymbolKind.Method;
        }
        public string ConvertType(bool use_full = false) {
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
                case "string": return "Qt::Core::String";
                case "System::string":
                case "Qt::Core::string":
                    return "Qt::Core::String";
                case "object": return "Qt::Core::Object";
                case "System::object":
                case "Qt::Core::object":
                    return "Qt::Core::Object";
                default: return use_full ? full : type;
            }
        }
        public string GetTypeType(bool use_full = false) {
            StringBuilder sb = new StringBuilder();
            sb.Append(ConvertType(use_full));
            sb.Append(GetGenericArgs());
            return sb.ToString();
        }
        public string GetTypeDeclaration(bool inc_arrays = true) {
            StringBuilder sb = new StringBuilder();
            if (inc_arrays) {
                for(int a=0;a<arrays;a++) {
                    sb.Append("std::shared_ptr<Qt::QSharp::FixedArray<");
                }
            }
            if (shared) {
                if (weakRef)
                    sb.Append("std::weak_ptr<");
                else
                    sb.Append("std::shared_ptr<");
                if (cls != null && cls.outter != null) {
                    sb.Append(cls.outter.fullname);
                    sb.Append("::");
                }
            }
            sb.Append(GetTypeType(true));
            if (ptr) sb.Append("*");
            if (shared) {
                sb.Append(">");
            }
            if (inc_arrays) {
                for(int a=0;a<arrays;a++) {
                    sb.Append(">>");
                }
            }
            return sb.ToString();
        }
        public string GetGenericArgs() {
            if (GenericArgs == null || GenericArgs.Count == 0) return "";
            StringBuilder sb = new StringBuilder();
            sb.Append("<");
            bool first = true;
            foreach(var arg in GenericArgs) {
                if (!first) sb.Append(","); else first = false;
                sb.Append(arg.GetTypeDeclaration());
            }
            sb.Append(">");
            return sb.ToString();
        }
    }

    class Variable : OutputBuffer {
        public string name;
        public SyntaxNode equals;

        public StringBuilder src = new StringBuilder();
        public void Append(string s) {
            src.Append(s);
        }
        public void Insert(int idx, string s) {
            src.Insert(idx, s);
        }
        public int Length() {return src.Length;}
        public virtual bool isField() {return false;}
        public virtual bool isMethod() {return false;}
    }

    class Argument {
        public Type type;
        public Variable name = new Variable();
    }

    class Field : Type
    {
        public override bool isField() {return true;}
        public List<Variable> variables = new List<Variable>();
        public bool Property;
        public bool get_Property;
        public bool set_Property;

        public string GetDeclaration() {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetFlags(false));
            if (array) {
                sb.Append(" ");
                sb.Append(GetTypeDeclaration());
                sb.Append(" ");
                bool first = true;
                foreach(var v in variables) {
                    if (!first) sb.Append(","); else first = false;
                    sb.Append(v.name);
                }
                sb.Append(";\r\n");
            } else {
                sb.Append(" ");
                sb.Append(GetTypeDeclaration());
                sb.Append(" ");
                bool first = true;
                foreach(var v in variables) {
                    if (!first) sb.Append(","); else first = false;
                    sb.Append(v.name);
                }
                sb.Append(";\r\n");
            }
            return sb.ToString();
        }
    }

    class Method : Variable
    {
        public override bool isMethod() {return true;}
        public Type type = new Type();

        public bool ctor;
        public bool isDelegate;
        public string Namespace;  //if classless delegate only
        public String basector;
        public List<Argument> args = new List<Argument>();
        public Class cls;
        public bool inFixedBlock;
        public String version;
        public String replaceArgs;
        public bool omitBody;
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
                    if (decl && arg.name.src.Length > 0) {
                        sb.Append(" = " );
                        sb.Append(arg.name.src.ToString());
                    }
                }
            }
            sb.Append(")");
            return sb.ToString();
        }
        public string GetDeclaration() {
            StringBuilder sb = new StringBuilder();
            if (!isDelegate) sb.Append(type.GetFlags(false));
            sb.Append(" ");
            if (isDelegate) sb.Append("typedef std::function<");
            sb.Append(type.GetTypeDeclaration());
            sb.Append(" ");
            if (!isDelegate) sb.Append(name);
            sb.Append(GetArgs(true));
            if (isDelegate) {
                sb.Append(">");  //$delegate");
                sb.Append(name);
            }
            if (type.Abstract) sb.Append("=0" + ";\r\n");
            return sb.ToString();
        }
    }
}
