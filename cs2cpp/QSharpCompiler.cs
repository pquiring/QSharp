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
        public static string cppFile;
        public static string hppFile;
        public ArrayList files = new ArrayList();
        public static bool debug = false;
        public static bool debugToString = false;
        public static bool debugTokens = false;
        public static string version = "0.5";
        public static bool library;
        public static bool classlib;
        public static string main;
        public static List<string> refs = new List<string>();
        public static List<string> libs = new List<string>();

        public static CSharpCompilation compiler;

        static void Main(string[] args)
        {
            if (args.Length < 3) {
                Console.WriteLine("Q# Compiler/" + version);
                Console.WriteLine("Usage : cs2cpp cs_in_folder src_out_file.cpp header_out_file.hpp [--library | --main=class] [--ref=dll ...] [--debug[=tokens,tostring,all]]");
                return;
            }
            if (args.Length > 3) {
                for(int a=3;a<args.Length;a++) {
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
                    if (arg == "--classlib") {
                        classlib = true;
                    }
                    if (arg == "--main") {
                        main = value.Replace(".", "::");
                    }
                    if (arg == "--ref") {
                        refs.Add(value);
                        int i1 = value.LastIndexOf("\\");
                        if (i1 != -1) {
                            value = value.Substring(i1+1);
                        }
                        int i2 = value.IndexOf(".");
                        value = value.Substring(0, i2);
                        libs.Add(value);
                    }
                    if (arg == "--debug") {
                        debug = true;
                        switch (value) {
                            case "all": debugToString = true; debugTokens = true; break;
                            case "tokens": debugTokens = true; break;
                            case "tostring": debugToString = true; break;
                        }
                    }
                }
            }
            new Program().process(args[0], args[1], args[2]);
            Console.WriteLine("cs2cpp generated " + args[1]);
        }

        void process(string cs, string cpp, string hpp)
        {
            csFolder = cs;
            cppFile = cpp;
            hppFile = hpp;
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
            addFolder(cs);
            foreach(Source node in files)
            {
                node.model = compiler.GetSemanticModel(node.tree);
                if (debug) {
                    SyntaxNode root = node.tree.GetRoot();
                    printNodes(node, root.ChildNodes(), 0);
                }
            }
            try {
                new Generate().generate(files, cppFile);
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
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
//            Console.WriteLine("AddFile:" + file);
            string src = System.IO.File.ReadAllText(file);
            Source node = new Source();
            node.csFile = file;
            int idx = csFolder.Length;
            int len = file.Length - idx - 3;
            file = file.Substring(idx, len);
            node.src = src;
            node.tree = CSharpSyntaxTree.ParseText(src);
            files.Add(node);
            compiler = compiler.AddSyntaxTrees(node.tree);
        }

        void printNodes(Source file, IEnumerable<SyntaxNode> nodes, int lvl)
        {
            int idx = 0;
            foreach(var node in nodes) {
                printNode(file, node, lvl, idx);
                if (debugTokens) printTokens(file, node.ChildTokens(), lvl);
                printNodes(file, node.ChildNodes(), lvl+1);
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
                ln += ",DeclSymbol=" + decl.Name;
            } else {
                ln += ",DeclSymbol=null";
            }
            ISymbol symbol = file.model.GetSymbolInfo(node).Symbol;
            if (symbol != null) {
                ln += ",Symbol=" + symbol.ToString();
                ln += ",Symbol.Kind=" + symbol.Kind;
                ln += ",Symbol.Name=" + symbol.Name;
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
            foreach(var diag in file.model.GetDiagnostics()) {
                ln += ",diag=" + diag.ToString();
            }
            foreach(var diag in file.model.GetSyntaxDiagnostics()) {
                ln += ",syntaxdiag=" + diag.ToString();
            }
            foreach(var diag in file.model.GetDeclarationDiagnostics()) {
                ln += ",decldiag=" + diag.ToString();
            }
            foreach(var diag in file.model.GetMethodBodyDiagnostics()) {
                ln += ",methoddiag=" + diag.ToString();
            }
            Object value = file.model.GetConstantValue(node).Value;
            if (value != null) {
                ln += ",Constant=" + value.ToString().Replace("\r", "").Replace("\n", "");
            }
            if (debugToString) {
                ln += ",ToString=" + node.ToString().Replace("\r", "").Replace("\n", "");
            }
            Console.WriteLine(ln);
        }

        void printTokens(Source file, IEnumerable<SyntaxToken> tokens, int lvl)
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
        public string src;
        public SyntaxTree tree;
        public SemanticModel model;
    }

    class Generate
    {
        public static Source file;

        private FileStream fs;
        private string Namespace = "";
        private List<Class> clss = new List<Class>();
        private List<string> usings = new List<string>();
        private static Class cls;
        private Class NoClass = new Class();  //for classless delegates
        private Method method;
        private Field field;

        public void generate(ArrayList sources, string cppFile)
        {
            if (Program.debug) {
                Console.WriteLine();
            }
            usings.Add("Qt::Core");
            foreach(Source file in sources) {
                generate(file);
            }
            openOutput(Program.hppFile);
            writeForward();
            while (sortClasses()) {};
            writeNoClassTypes();
            writeClasses();
            closeOutput();
            openOutput(Program.cppFile);
            writeIncludes();
            writeStaticFields();
            writeStaticFieldsInit();
            writeMethods();
            if (Program.main != null) writeMain();
            closeOutput();
        }

        private void generate(Source file)
        {
            SyntaxNode root = file.tree.GetRoot();
            if (Program.debug) {
                Console.WriteLine("Compiling:" + file.csFile);
            }
            outputFile(file);
        }

        private void openOutput(string filename) {
            fs = System.IO.File.Open(filename, FileMode.Create);
            byte[] bytes = new UTF8Encoding().GetBytes("//cs2cpp : Machine generated code : Do not edit!\r\n");
            fs.Write(bytes, 0, bytes.Length);
        }

        private void writeForward() {
            StringBuilder sb = new StringBuilder();
            sb.Append("#include <cs2cpp.hpp>\r\n");
            if (Program.cppFile != "classlib.cpp") {
                sb.Append("#include <classlib.hpp>\r\n");
            }
            foreach(var use in usings) {
                sb.Append("namespace " + use + " {}\r\n");
                sb.Append("using namespace " + use + ";\r\n");
            }
            foreach(var cls in clss) {
                if (cls.Namespace == "Qt::QSharp" && cls.name.StartsWith("CPP")) continue;
                if (cls.Namespace != "") sb.Append("namespace " + cls.Namespace + "{\r\n");
                sb.Append(cls.GetForwardDeclaration());
                if (cls.Namespace != "") sb.Append("}\r\n");
                if (!cls.Interface && cls.bases.Count == 0 && (!(cls.Namespace == "Qt::Core" && cls.name == "Object"))) {
                    cls.bases.Add("Qt::Core::Object");
                    cls.addUsage("Object");
                    if (cls.Namespace.StartsWith("Qt")) cls.addUsage("Derived");  //temp fix
                }
            }
            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        /** In C++ you can not use an undefined class, so they must be sorted by usage. */
        private bool sortClasses() {
            int cnt = clss.Count;
            Class tmp;
            for(int idx=0;idx<cnt;idx++) {
                Class cls = clss[idx];
                int ucnt = cls.uses.Count;
                for(int uidx=0;uidx<ucnt;uidx++) {
                    string use = cls.uses[uidx];
                    for(int i=idx+1;i<cnt;i++) {
                        if (clss[i].name == use) {
                            //need to move i before idx
//                            Console.WriteLine("Moving " + clss[i].name + " before " + clss[idx].name);
                            tmp = clss[i];
                            clss.RemoveAt(i);
                            clss.Insert(0, tmp);
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

        private void writeClasses() {
            StringBuilder sb = new StringBuilder();
            foreach(var cls in clss) {
                if (cls.Namespace == "Qt::QSharp" && cls.name.StartsWith("CPP")) continue;
                if (!cls.hasctor && !cls.Interface && !cls.omitConstructors) {
                    Generate.cls = cls;
                    ctorNode(null);
                }
                if (cls.Namespace != "") sb.Append("namespace " + cls.Namespace + "{\r\n");
                sb.Append(cls.GetDeclaration());
                if (cls.Namespace != "") sb.Append("}\r\n");
                if (cls.nonClassHPP != null) sb.Append(cls.nonClassHPP);
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
            foreach(var cls in clss) {
                if (cls.Namespace != "") sb.Append("namespace " + cls.Namespace + "{\r\n");
                sb.Append(cls.GetStaticFields());
                if (cls.Namespace != "") sb.Append("}\r\n");
            }
            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        private void writeStaticFieldsInit() {
            StringBuilder sb = new StringBuilder();
            String libraryName = Program.cppFile;
            int i1 = libraryName.LastIndexOf("\\");
            if (i1 != -1) {
                libraryName = libraryName.Substring(i1+1);
            }
            int i2 = libraryName.IndexOf(".");
            libraryName = libraryName.Substring(0, i2);
            sb.Append("void $" + libraryName + "_ctor() {\r\n");
            foreach(var cls in clss) {
                sb.Append(cls.GetStaticFieldsInit());
            }
            sb.Append("};\r\n");
            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        private void writeMethods() {
            StringBuilder sb = new StringBuilder();
            foreach(var cls in clss) {
                if (cls.Generic) continue;
                if (cls.Namespace == "Qt::QSharp" && cls.name.StartsWith("CPP")) continue;
                string hppfile = "src/" + cls.name + ".hpp";
                if (File.Exists(hppfile)) sb.Append("#include \"" + hppfile + "\"\r\n");
                if (cls.Namespace != "") sb.Append("namespace " + cls.Namespace + "{\r\n");
                sb.Append(cls.GetMethodsDefinitions());
                if (cls.Namespace != "") sb.Append("}\r\n");
                if (cls.nonClassCPP != null) sb.Append(cls.nonClassCPP);
                string cppfile = "src/" + cls.name + ".cpp";
                if (File.Exists(cppfile)) sb.Append("#include \"" + cppfile + "\"\r\n");
            }
            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        private void writeMain() {
            StringBuilder sb = new StringBuilder();

            foreach(var lib in Program.libs) {
                sb.Append("extern void $" + lib + "_ctor();\r\n");
            }

            sb.Append("namespace Qt::Core {\r\n");;
            sb.Append("int g_argc;\r\n");
            sb.Append("const char **g_argv;\r\n");
            sb.Append("}\r\n");
            sb.Append("int main(int argc, const char **argv) {\r\n");
            sb.Append("std::shared_ptr<QSharpArray<std::shared_ptr<Qt::Core::String>>> args = std::make_shared<QSharpArray<std::shared_ptr<Qt::Core::String>>>(argc-1);\r\n");
            foreach(var lib in Program.libs) {
                sb.Append("$" + lib + "_ctor();\r\n");
            }
            sb.Append("for(int a=1;a<argc;a++) {args->at(a) = std::make_shared<Qt::Core::String>(argv[a]);}\r\n");
            sb.Append(Program.main + "::Main(args);\r\n");
            sb.Append("return 0;}\r\n");

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
                    clss.Add(topcls);
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

        private void classNode(SyntaxNode node, Class inner, Class otter, bool Interface) {
            cls = inner;
            cls.fullname = otter.fullname;
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
                init.type.type = "void";
                init.type.primative = true;
                init.type.Private = true;
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
                }
            }
        }

        private void typeParameterListNode(SyntaxNode node) {
            cls.Generic = true;
            foreach(var child in node.ChildNodes()) {
                switch (child.Kind()) {
                    case SyntaxKind.TypeParameter:
                        cls.GenericArgs.Add(new Type(child));
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
                        String baseName = null;
                        switch (baseNode.Kind()) {
                            case SyntaxKind.IdentifierName:
                            case SyntaxKind.QualifiedName:
                                baseName = baseNode.ToString().Replace(".", "::");
                                break;
                            case SyntaxKind.GenericName:
                                baseName = GetSymbol(baseNode);
                                SyntaxNode argList = GetChildNode(baseNode);
                                baseName += "<";
                                bool first = true;
                                foreach(var arg in argList.ChildNodes()) {
                                    if (!first) baseName += ","; else first = false;
                                    Type type = new Type(arg);
                                    baseName += type.GetTypeDeclaration();
                                }
                                baseName += ">";
                                break;
                            default:
                                Console.WriteLine("unknown baseList node:" + baseNode.Kind());
                                break;
                        }
                        if (baseName == "System::Exception") continue;
                        if (baseName == "System::Attribute") continue;
                        if (isClass(baseNode))
                            cls.bases.Add(baseName);
                        else
                            cls.ifaces.Add(baseName);
                        int idx = baseName.IndexOf("<");
                        if (idx != -1) {
                            baseName = baseName.Substring(0, idx);
                        }
                        cls.addUsage(baseName);
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
                arrayInitNode(equalsChild, v, field.type, field.arrays);
            } else {
                expressionNode(equalsChild, v);
            }
            v.Append(";\r\n");
        }

        private void propertyNode(SyntaxNode node) {
            // type, AccessorList -> {GetAccessorDeclaration, SetAccessorDeclaration}
            field = new Field();
            field.Property = true;
            Variable v = new Variable();
            v.type = field;
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
                                    method.type = v.type;
                                    method.type.typekind = field.typekind;
                                    method.type.setTypes();
                                    field.get_Property = true;
                                    break;
                                case SyntaxKind.SetAccessorDeclaration:
                                    methodNode(_etter, false, false, "$set_" + v.name);
                                    Variable arg = new Variable();
                                    arg.type = v.type;
                                    arg.name = "value";
                                    method.args.Add(arg);
                                    method.type.type = "void";
                                    method.type.setTypes();
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
                                    case "Qt::QSharp::CPPVersion":
                                        SyntaxNode arg = GetChildNode(attrArgList);  //AttributeArgument
                                        SyntaxNode str = GetChildNode(arg);  //StringLiteralExpression
                                        String value = file.model.GetConstantValue(str).Value.ToString();
                                        method.version = value;
                                        break;
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
                                    case "Qt::QSharp::CPPOmitConstructors": {
                                        cls.omitConstructors = true;
                                        break;
                                    }
                                }
                                break;
                        }
                        break;
                }
            }
        }

        //return EqualsClause if present
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
                            type.GenericArgs.Add(new Type(arg));
                        }
                        type.type = GetSymbol(child);
                        type.setTypes();
                        break;
                    case SyntaxKind.PredefinedType:
                    case SyntaxKind.IdentifierName:
                    case SyntaxKind.QualifiedName:
                        ITypeSymbol typesym = file.model.GetTypeInfo(child).Type;
                        if (typesym != null) {
                            type.type = typesym.ToString().Replace(".", "::");
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

        private void ctorNode(SyntaxNode node) {
            method = new Method();
            cls.methods.Add(method);
            method.cls = cls;
            method.name = "$ctor";  //C++ = cls.name;
            method.type.type = "void";
            method.type.primative = true;
            method.ctor = true;
            cls.hasctor = true;
            if (node != null) {
                getFlags(method.type, file.model.GetDeclaredSymbol(node));
                IEnumerable<SyntaxNode> nodes = node.ChildNodes();
                //nodes : parameter list, [baseCtor], block
                foreach(var child in nodes) {
                    switch (child.Kind()) {
                        case SyntaxKind.ParameterList:
                            parameterListNode(child);
                            break;
                        case SyntaxKind.BaseConstructorInitializer:
                            SyntaxNode argList = GetChildNode(child);
                            method.Append(cls.bases[0]);
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
                if (cls.bases.Count > 0) {
                    method.type.Public = true;
                    method.Append("{");
                    method.Append("std::shared_ptr<" + cls.GetTypeDeclaration() + "> $this = this->$this.lock();\r\n");
                    method.Append(cls.bases[0]);
                    method.Append("::$ctor();\r\n");
                    method.Append("}");
                }
            }
            method.type.setTypes();
            createNewMethod(cls, method.args);
        }

        private void createNewMethod(Class cls, List<Variable> args) {
            Method method = new Method();
            method.type.Public = true;
            method.type.Static = true;
            method.name = "$new";
            method.type.type = cls.name;
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
            method.Append("$this->$this = $this;\r\n");
            method.Append("$this->$init();\r\n");
            method.Append("$this->$ctor(");
            if (args != null) {
                bool first = true;
                foreach(var arg in args) {
                    if (!first) method.Append(","); else first = false;
                    method.Append(arg.name);
                    method.args.Add(arg);
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
            if (dtor) {
                method.type.Virtual = true;
                method.type.Public = true;
            } else if (isDelegate) {
                method.isDelegate = true;
                method.Namespace = Namespace;
            }
            if (name != null) {
                method.name = name;
            } else {
                if (dtor) {
                    method.name = "~" + cls.name;
                    method.type.type = "";
                } else {
                    method.name = file.model.GetDeclaredSymbol(node).Name;
                }
            }
            getFlags(method.type, file.model.GetDeclaredSymbol(node));
            if (dtor) {
                method.type.Protected = false;
            }
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            //nodes : [return type], parameter list, block
            foreach(var child in nodes) {
                switch (child.Kind()) {
                    case SyntaxKind.PredefinedType:
                    case SyntaxKind.IdentifierName:
                    case SyntaxKind.QualifiedName:
                        Type type = new Type(child);
                        ISymbol symbol = file.model.GetSymbolInfo(child).Symbol;
                        ISymbol declsymbol = file.model.GetDeclaredSymbol(child);
                        if (symbol == null && declsymbol == null) {
                            Console.WriteLine("Error:Symbol not found for:" + child);
                            Environment.Exit(0);
                        }
                        if (symbol != null) {
                            method.type.type = symbol.ToString().Replace(".", "::");
                        } else {
                            method.type.type = declsymbol.ToString().Replace(".", "::");
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
                            method.type.GenericArgs.Add(new Type(arg));
                        }
                        method.type.type = GetSymbol(child);
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
                            type.type = "auto";
                        }
                        Variable variable = new Variable();
                        variable.name = file.model.GetDeclaredSymbol(param).Name.Replace(".", "::");
                        variable.type = type;
                        method.args.Add(variable);
                        SyntaxNode equals = GetChildNode(param, 2);
                        if (equals != null && equals.Kind() == SyntaxKind.EqualsValueClause) {
                            expressionNode(GetChildNode(equals), variable);
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
                        type.type = symbol.ToString().Replace(".", "::");
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
                        type.GenericArgs.Add(new Type(arg));
                    }
                    type.type = GetSymbol(node);
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
                    method.Append("std::shared_ptr<" + cls.GetTypeDeclaration() + "> $this = this->$this.lock();\r\n");
                }
                if (method.basector != null) method.Append(method.basector);
            }
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach(var child in nodes) {
                statementNode(child);
            }
            if (throwFinally) {
                method.Append("throw std::make_shared<FinallyException>();");
            }
            method.Append("}\r\n");
        }

        private void statementNode(SyntaxNode node) {
            switch (node.Kind()) {
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
                    expressionNode(GetChildNode(node), method);
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
                    //for(expression;expression;expression...) statement
                    int Count = GetChildCount(node);
                    method.Append("for(");
                    expressionNode(GetChildNode(node, 1), method);
                    method.Append(";");
                    expressionNode(GetChildNode(node, 2), method);
                    method.Append(";");
                    for(int idx=3;idx<Count;idx++) {
                        if (idx > 3) method.Append(",");
                        expressionNode(GetChildNode(node, idx), method);
                    }
                    method.Append(")");
                    statementNode(GetChildNode(node, Count));
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
                                SyntaxNode catchDecl = GetChildNode(child, 1);
                                method.Append(" catch(std::shared_ptr<");
                                expressionNode(GetChildNode(catchDecl), method);  //exception type
                                method.Append("> ");
                                method.Append(file.model.GetDeclaredSymbol(catchDecl).Name);  //exception variable name
                                method.Append(")");
                                SyntaxNode catchBlock = GetChildNode(child, 2);
                                if (catchBlock.Kind() == SyntaxKind.Block) {
                                    blockNode(catchBlock, false, hasFinally);
                                } else {
                                    statementNode(catchBlock);
                                }
                                break;
                            case SyntaxKind.FinallyClause:
                                method.Append("} catch(std::shared_ptr<FinallyException> $finally" + cls.finallyCnt++ + ") ");
                                statementNode(GetChildNode(child));
                                break;
                        }
                    }
                    break;
                case SyntaxKind.ThrowStatement:
                    method.Append("throw ");
                    expressionNode(GetChildNode(node), method);
                    method.Append(";");
                    break;
                case SyntaxKind.Block:
                    blockNode(node);
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
                                            arrayInitNode(equalsChild, method, type.type, type.arrays);
                                        } else {
                                            expressionNode(equalsChild, method);
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
                    if (lockIdName != "Qt::Core::Mutex") {
                        Console.WriteLine("Error:lock {} must use Qt.Core.Mutex");
                        Environment.Exit(0);
                    }
                    SyntaxNode lockBlock = GetChildNode(node, 2);
                    string holder = "$lock" + cls.lockCnt++;
                    method.Append("for(MutexHolder " + holder + "(");
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

        private void expressionNode(SyntaxNode node, OutputBuffer ob, bool lvalue = false, bool argument = false, bool invoke = false, bool assignment = false) {
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            Type type;
            switch (node.Kind()) {
                case SyntaxKind.IdentifierName:
                case SyntaxKind.PredefinedType:
                case SyntaxKind.QualifiedName:
                    type = new Type(node);
                    if (isProperty(node)) {
                        if (lvalue)
                            ob.Append("$set_" + type.type);
                        else
                            ob.Append("$get_" + type.type + "()");
                        break;
                    } else {
                        if ((argument || assignment) && type.isSymbolMethod()) {
                            ob.Append("std::bind(&");
                        }
                        ob.Append(type.GetTypeType());
                        if ((argument || assignment) && type.isSymbolMethod()) {
                            ob.Append(", this");
                            //add std::placeholders::_1, ... for # of arguments to delegate
                            int numArgs = GetNumArgs(node);
                            for(int a=0;a<numArgs;a++) {
                                ob.Append(", std::placeholders::_" + (a+1));
                            }
                            ob.Append(")");
                        }
                    }
                    break;
                case SyntaxKind.GenericName:
                    type = new Type(node);
                    SyntaxNode typeList = GetChildNode(node);
                    foreach(var arg in typeList.ChildNodes()) {
                        type.GenericArgs.Add(new Type(arg));
                    }
                    ob.Append(type.GetTypeType());
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
                                arrayInitNode(equalsChild, method, type.type, type.arrays);
                            } else {
                                expressionNode(equalsChild, method);
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
                    ob.Append(file.model.GetConstantValue(node).Value.ToString());
                    break;
                case SyntaxKind.TrueLiteralExpression:
                    ob.Append("true");
                    break;
                case SyntaxKind.FalseLiteralExpression:
                    ob.Append("false");
                    break;
                case SyntaxKind.StringLiteralExpression:
                    ob.Append("std::make_shared<String>(");
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
                        break;
                    }
                    if (isStatic(right) || left.Kind() == SyntaxKind.BaseExpression || isEnum(left) || (isNamedType(left) && isNamedType(right))) {
                        expressionNode(left, ob, lvalue);
                        ob.Append("::");
                        expressionNode(right, ob, lvalue);
                    } else {
                        ob.Append("$deref(");  //NPE check
                        expressionNode(left, ob, lvalue);
                        ob.Append(")->");
                        expressionNode(right, ob, lvalue);
                    }
                    break;
                case SyntaxKind.BaseExpression:
                    ob.Append(cls.name);
                    break;
                case SyntaxKind.ObjectCreationExpression:
                    invokeNode(node, ob, true);
                    break;
                case SyntaxKind.CastExpression:
                    castNode(node, ob);
                    break;
                case SyntaxKind.ElementAccessExpression:
                    //IdentifierNode, BracketedArgumentList -> {Argument, ...}
                    SyntaxNode array = GetChildNode(node, 1);
                    SyntaxNode index = GetChildNode(node, 2);
                    ob.Append("(*$deref(");  //NPE check
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
                    equalsNode(node, ob);
                    break;
                case SyntaxKind.NotEqualsExpression:
                    binaryNode(node, ob, "!=");
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
                                    ptype.type = "auto";
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
                    ob.Append("std::make_shared<QSharpArray<");
                else
                    ob.Append("std::shared_ptr<QSharpArray<");
            }
            ob.Append(type);
            for(int a=0;a<dims;a++) {
                ob.Append(">>");
            }
            ob.Append("(std::initializer_list<");
            for(int a=1;a<dims;a++) {
                ob.Append("std::shared_ptr<QSharpArray<");
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
            Console.WriteLine("Error:Symbol not found for:" + node.ToString());
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

        private void binaryNode(SyntaxNode node, OutputBuffer ob, string op) {
            expressionNode(GetChildNode(node, 1), ob);
            ob.Append(op);
            expressionNode(GetChildNode(node, 2), ob);
        }

        private void equalsNode(SyntaxNode node, OutputBuffer ob) {
            SyntaxNode left = GetChildNode(node, 1);
            SyntaxNode right = GetChildNode(node, 2);
            if (GetTypeName(left) == "Qt::Core::String" && GetTypeName(right) == "Qt::Core::String") {
                //string comparison : invoke left.Equals(right) [NOTE: Use Object.ReferenceEquals() to compare pointers]
                expressionNode(left, ob);
                ob.Append("->Equals(");
                expressionNode(right, ob);
                ob.Append(")");
            } else {
                binaryNode(node, ob, "==");
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

        private static bool CStyleCast = true;  //C++ style not working

        private void castNode(SyntaxNode node, OutputBuffer ob) {
            SyntaxNode castType = GetChildNode(node, 1);
            SyntaxNode value = GetChildNode(node, 2);
            //cast value to type
            //C# (type)value
            //C++ std::static_pointer_cast<type>(value)
            Type type = new Type(castType);
            if (!CStyleCast) if (type.shared) ob.Append("std::static_pointer_cast<"); else ob.Append("static_cast<");
            if (CStyleCast) ob.Append("(");
            ob.Append(type.GetTypeDeclaration());
            if (CStyleCast) ob.Append(")");
            if (!CStyleCast) ob.Append(">");
            if (!CStyleCast) ob.Append("(");
            expressionNode(value, ob);
            if (!CStyleCast) ob.Append(")");
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
                arrayInitNode(initList, ob, typeNode.ToString(), dims);
                return;
            }
            if (typeNode == null || sizeNode == null) {
                Console.WriteLine("Error:Invalid ArrayCreationExpression : " + typeNode + " : " + sizeNode);
                return;
            }
            for(int a=0;a<dims;a++) {
              ob.Append("std::make_shared<QSharpArray<");
            }
            Type type = new Type(typeNode);
            ob.Append(type.GetTypeType());
            for(int a=0;a<dims;a++) {
                ob.Append(">>");
            }
            ob.Append("(");
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

        private void invokeNode(SyntaxNode node, OutputBuffer ob, bool New = false) {
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
                ob.Append("$checkDelegate(");
            }
            expressionNode(id, ob, true, false, true);
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
                    sb.Append(" virtual");
                }
            }
            if (Virtual) sb.Append(" virtual");
            if (Extern) sb.Append(" extern");
            return sb.ToString();
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
        public List<string> bases = new List<string>();
        public List<string> cppbases = new List<string>();
        public List<string> ifaces = new List<string>();
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
        public bool omitFields, omitMethods, omitConstructors;
        //uses are used to sort classes
        public List<string> uses = new List<string>();
        public void addUsage(string cls) {
            int idx = cls.IndexOf("::");
            if (idx != -1) cls = cls.Substring(idx+1);
            if (cls == name) return;
            if (!uses.Contains(cls)) {
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
                sb.Append(">");
            }
            sb.Append("class " + name);
            sb.Append(";\r\n");
            return sb.ToString();
        }
        public string GetDeclaration() {
            StringBuilder sb = new StringBuilder();
            if (Generic) {
                sb.Append("template< ");
                bool first = true;
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
                bool first = true;
                foreach(var basecls in bases) {
                    if (!first) sb.Append(","); else first = false;
                    sb.Append("public ");
                    sb.Append(basecls);
                }
                foreach(var cppcls in cppbases) {
                    if (!first) sb.Append(","); else first = false;
                    sb.Append("public ");
                    sb.Append(cppcls);
                }
                foreach(var iface in ifaces) {
                    if (!first) sb.Append(","); else first = false;
                    sb.Append("public ");
                    sb.Append(iface);
                }
            }
            sb.Append("{\r\n");
            foreach(var inner in inners) {
                sb.Append(inner.GetDeclaration());
            }
            if (cpp != null) sb.Append(cpp);
            if (!Interface) {
                sb.Append("private: std::weak_ptr<" + name);
                if (Generic) {
                    sb.Append("<");
                    bool first = true;
                    foreach(var arg in GenericArgs) {
                        if (!first) sb.Append(","); else first = false;
                        sb.Append(arg.GetTypeDeclaration());
                    }
                    sb.Append(">");
                }
                sb.Append("> $this;\r\n");
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
                if (method.isDelegate) continue;
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
        public TypeKind typekind;
        public SymbolKind symbolkind;
        public SyntaxNode node;
        public bool primative;
        public bool numeric;
        public bool weakRef;
        public bool array;
        public int arrays;  //# of dimensions
        public bool shared;
        public bool ptr;  //unsafe pointer
        public List<Type> GenericArgs = new List<Type>();

        public virtual bool isField() {return false;}
        public virtual bool isMethod() {return false;}

        public Type() {}
        public Type(SyntaxNode node) {
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
            ISymbol symbol = Generate.file.model.GetSymbolInfo(node).Symbol;
            if (symbol != null) {
                //for PredefinedType ISymbol.Name == Boxed type
                if (node.Kind() == SyntaxKind.PredefinedType)
                    type = symbol.ToString().Replace(".", "::");
                else
                    type = symbol.Name.Replace(".", "::");
                symbolkind = symbol.Kind;
            } else {
                symbol = Generate.file.model.GetDeclaredSymbol(node);
                if (symbol != null) {
                    type = symbol.Name.Replace(".", "::");
                } else {
                    type = node.ToString().Replace(".", "::");
                }
            }
            ITypeSymbol typesymbol = Generate.file.model.GetTypeInfo(node).Type;
            if (typesymbol != null) {
                typekind = typesymbol.TypeKind;
            }
            setTypes();
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
                case "string": return "String";
                case "System::string":
                case "Qt::Core::string":
                    return "Qt::Core::String";
                case "object": return "Object";
                case "System::object":
                case "Qt::Core::object":
                    return "Qt::Core::Object";
                default: return type;
            }
        }
        public string GetTypeType() {
            StringBuilder sb = new StringBuilder();
            sb.Append(ConvertType());
            if (GenericArgs.Count > 0) {
                sb.Append("<");
                bool first = true;
                foreach(var arg in GenericArgs) {
                    if (!first) sb.Append(","); else first = false;
                    sb.Append(arg.GetTypeDeclaration());
                }
                sb.Append(">");
            }
            return sb.ToString();
        }
        public string GetTypeDeclaration() {
            StringBuilder sb = new StringBuilder();
            for(int a=0;a<arrays;a++) {
                sb.Append("std::shared_ptr<QSharpArray<");
            }
            if (shared) {
                if (weakRef)
                    sb.Append("std::weak_ptr<");
                else
                    sb.Append("std::shared_ptr<");
            }
            sb.Append(GetTypeType());
            if (ptr) sb.Append("*");
            if (shared) {
                sb.Append(">");
            }
            for(int a=0;a<arrays;a++) {
                sb.Append(">>");
            }
            return sb.ToString();
        }
    }

    class Variable : OutputBuffer {
        public Type type = new Type();
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
                for(int a=0;a<arrays;a++) {
                    sb.Append("std::shared_ptr<QSharpArray<");
                }
                sb.Append(GetTypeDeclaration());
                for(int a=0;a<arrays;a++) {
                    sb.Append(">>");
                }
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

        public bool ctor;
        public bool isDelegate;
        public string Namespace;  //if classless delegate only
        public String basector;
        public List<Variable> args = new List<Variable>();
        public Class cls;
        public bool inFixedBlock;
        public String version;
        public string GetArgs(bool decl) {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            bool first = true;
            foreach(var arg in args) {
                if (!first) sb.Append(","); else first = false;
                sb.Append(arg.type.GetTypeDeclaration());
                sb.Append(" ");
                sb.Append(arg.name);
                if (decl && arg.src.Length > 0) {
                    sb.Append(" = " );
                    sb.Append(arg.src.ToString());
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
