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
        public static string version = "0.0.1";
        public static bool library;
        public static bool classlib;
        public static string main;
        public static List<string> refs = new List<string>();

        public static CSharpCompilation compiler;

        static void Main(string[] args)
        {
            if (args.Length < 3) {
                Console.WriteLine("Q# Compiler/" + version);
                Console.WriteLine("Usage : cs2cpp cs_in_folder src_out_file.cpp header_out_file.hpp [--library | --main=class] [--ref=dll ...]");
                return;
            }
            if (args.Length > 3) {
                for(int a=3;a<args.Length;a++) {
                    if (args[a] == "--library") {
                        library = true;
                    }
                    if (args[a] == "--classlib") {
                        classlib = true;
                    }
                    if (args[a].StartsWith("--main=")) {
                        main = args[a].Substring(7).Replace(".", "::");
                    }
                    if (args[a].StartsWith("--ref=")) {
                        refs.Add(args[a].Substring(6));
                    }
                    if (args[a] == "--debug") {
                        debug = true;
                    }
                }
            }
            new Program().process(args[0], args[1], args[2]);
        }

        void process(string cs, string cpp, string hpp)
        {
            csFolder = cs;
            cppFile = cpp;
            hppFile = hpp;
            compiler = CSharpCompilation.Create("C#");
//            if (classlib)
            {
                String lib = typeof(object).Assembly.Location;
                Console.WriteLine("Adding Reference:" + lib);
                compiler = compiler.AddReferences(MetadataReference.CreateFromFile(lib));
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
                for(int a=0;a<lvl;a++) {
                  Console.Write("  ");
                }
                ISymbol symbol = file.model.GetSymbolInfo(node).Symbol;
                ITypeSymbol type = file.model.GetTypeInfo(node).Type;
                Object value = file.model.GetConstantValue(node).Value;
                ISymbol decl = file.model.GetDeclaredSymbol(node);
                String ln = "node[" + lvl + "][" + idx + "]=" + node.Kind();
                if (value != null) {
                  ln += ",constant=" + value;
                }
                if (decl != null) {
                  ln += ",declsymbol=" + decl.Name;
                }
                if (symbol != null) {
                    ln += ",symbol=" + symbol;
                    ln += ",static=" + symbol.IsStatic;
                }
                if (type != null) {
                    ln += ",type=" + type.ToString();
                    ln += ",type.Kind=" + type.Kind;
                    ln += ",type.TypeKind=" + type.TypeKind;
                }
                ln += ",tostring=" + node.ToString().Replace("\r\n", "").Replace("\n", "");
                Console.WriteLine(ln);
                printTokens(file, node.ChildTokens(), lvl);
                printNodes(file, node.ChildNodes(), lvl+1);
                idx++;
            }
        }

        void printTokens(Source file, IEnumerable<SyntaxToken> tokens, int lvl)
        {
            int idx = 0;
            foreach(var token in tokens) {
                for(int a=0;a<lvl+2;a++) {
                  Console.Write("  ");
                }
                Console.WriteLine("token[" + lvl + "][" + idx + "]=" + token + ":" + token.Kind() + ":hash=" + token.GetHashCode());
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
        private FileStream fs;
        private string Namespace = "";
        private List<Class> clss = new List<Class>();
        private List<string> usings = new List<string>();
        private Source file;
        private Class cls;
        private Method method;
        private Field field;
        private int finallyCount = 0;

        public void generate(ArrayList sources, string cppFile)
        {
            usings.Add("Qt::Core");
            foreach(Source file in sources) {
                generate(file);
            }
            openOutput(Program.hppFile);
            writeForward();
            while (sortClasses()) {};
            writeClasses();
            closeOutput();
            openOutput(Program.cppFile);
            writeIncludes();
            writeStaticFields();
            writeMethods();
            if (Program.main != null) writeMain();
            closeOutput();
        }

        private void generate(Source file)
        {
            SyntaxNode root = file.tree.GetRoot();
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
            sb.Append("namespace Qt::Core {\r\n");;
            sb.Append("extern int g_argc;\r\n");
            sb.Append("extern const char **g_argv;\r\n");
            sb.Append("}\r\n");
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
                sb.Append("class " + cls.name + ";\r\n");
                if (cls.Namespace != "") sb.Append("}\r\n");
                if (cls.bases.Count == 0 && (!(cls.Namespace == "Qt::Core" && cls.name == "Object"))) {
                    cls.bases.Add("Qt::Core::Object");
                    cls.addUsage("Object");
                }
            }
            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        /** In C++ you can not use an undefined class, so they must be sorted by usage. */
        private bool sortClasses() {
            int cnt = clss.Count;
            Class tmp;
            bool moved = false;
            for(int idx=0;idx<cnt;idx++) {
                Class cls = clss[idx];
                int ucnt = cls.uses.Count;
                for(int uidx=0;uidx<ucnt;uidx++) {
                    string use = cls.uses[uidx];
                    for(int i=idx+1;i<cnt;i++) {
                        if (clss[i].name == use) {
                            //need to move i before idx
                            tmp = clss[i];
                            clss[i] = clss[idx];
                            clss[idx] = tmp;
                            moved = true;
                            break;
                        }
                    }
                }
            }
            return moved;
        }

        private void writeClasses() {
            StringBuilder sb = new StringBuilder();
            foreach(var cls in clss) {
                if (cls.Namespace == "Qt::QSharp" && cls.name.StartsWith("CPP")) continue;
                if (!cls.hasctor && !cls.Interface && !cls.omitConstructors) {
                    createNewMethod(cls, null);
                }
                if (cls.Namespace != "") sb.Append("namespace " + cls.Namespace + "{\r\n");
                sb.Append(cls.GetFlags(true));
                sb.Append(" class " + cls.name);
                if (cls.bases.Count > 0 || cls.ifaces.Count > 0) {
                    sb.Append(":");
                    bool first = true;
                    foreach(var basecls in cls.bases) {
                        if (!first) sb.Append(","); else first = false;
                        sb.Append("public ");
                        sb.Append(basecls);
                    }
                    foreach(var iface in cls.ifaces) {
                        if (!first) sb.Append(","); else first = false;
                        sb.Append("public ");
                        sb.Append(iface);
                    }
                }
                sb.Append("{\r\n");
                if (cls.cpp != null) sb.Append(cls.cpp);
                if (!cls.Interface) {
                    sb.Append("private: std::weak_ptr<" + cls.name + "> $this;\r\n");
                }
                foreach(var field in cls.fields) {
                    sb.Append(field.GetDeclaration());
                }
                foreach(var method in cls.methods) {
                    sb.Append(method.GetDeclaration());
                }
                sb.Append("};\r\n");
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
                StringBuilder sb2 = new StringBuilder();
                if (cls.Namespace != "") sb.Append("namespace " + cls.Namespace + "{\r\n");
                int cnt = 0;
                sb2.Append("__attribute__((constructor)) static void $init_" + cls.name + "() {\r\n");
                foreach(var field in cls.fields) {
                    if (!field.Static) continue;
                    sb.Append(field.GetTypeDecl() + " " + cls.name + "::" + field.name);
                    if (field.primative) {
                        sb.Append("= 0;\r\n");
                    }  else {
                        sb.Append(";\r\n");
                    }
                    if (field.src.Length > 0) {
                        sb2.Append(field.src);
                    }
                    cnt++;
                }
                sb2.Append("}");
                if (cnt > 0) sb.Append(sb2);
                if (cls.Namespace != "") sb.Append("}\r\n");
            }
            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        private void writeMethods() {
            StringBuilder sb = new StringBuilder();
            foreach(var cls in clss) {
                if (cls.Namespace == "Qt::QSharp" && cls.name.StartsWith("CPP")) continue;
                if (cls.Namespace != "") sb.Append("namespace " + cls.Namespace + "{\r\n");
                foreach(var method in cls.methods) {
//                    if (method.Virtual) Console.WriteLine("virtual:" + cls.name + "::" + method.name);
//                    if (method.Abstract) Console.WriteLine("abstract:" + cls.name + "::" + method.name);
//                    if (method.Override) Console.WriteLine("override:" + cls.name + "::" + method.name);
//                    if (method.Definition) Console.WriteLine("definition:" + cls.name + "::" + method.name);
//                    if (method.Sealed) Console.WriteLine("sealed:" + cls.name + "::" + method.name);
                    sb.Append(method.GetTypeDecl());
                    sb.Append(" ");
                    sb.Append(cls.name);
                    sb.Append("::");
                    sb.Append(method.name);
                    sb.Append(method.GetArgs());
                    if (method.Length() == 0) method.Append("{}\r\n");
                    if (method.name == "$init") {
                        sb.Append("{\r\n");
                        foreach(var field in cls.fields) {
                            if (field.Length() > 0 && !field.Static) {
                                sb.Append(field.src);
                            }
                        }
                        sb.Append("}\r\n");
                    } else {
                        sb.Append(method.src);
                    }
                }
                if (cls.Namespace != "") sb.Append("}\r\n");
                if (cls.nonClassCPP != null) sb.Append(cls.nonClassCPP);
            }
            byte[] bytes = new UTF8Encoding().GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }

        private void writeMain() {
            StringBuilder sb = new StringBuilder();

            sb.Append("namespace Qt::Core {\r\n");;
            sb.Append("int g_argc;\r\n");
            sb.Append("const char **g_argv;\r\n");
            sb.Append("}\r\n");
            sb.Append("int main(int argc, const char **argv) {\r\n");
            sb.Append("std::shared_ptr<QVector<std::shared_ptr<Qt::Core::String>>> args = std::make_shared<QVector<std::shared_ptr<Qt::Core::String>>>();\r\n");
            sb.Append("for(int a=1;a<argc;a++) {args->append(std::make_shared<Qt::Core::String>(argv[a]));}\r\n");
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
            this.file = file;
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
                    classNode(node, node.Kind() == SyntaxKind.InterfaceDeclaration);
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
                    if (Using == "Qt::QSharp") break;  //omit (internal usage only)
                    if (Using == "System") break;  //not needed
                    if (Using == "System::Reflection") break;  //not supported yet
                    if (!usings.Contains(Using)) {
                        usings.Add(Using);
                    }
                    break;
            }
        }

        private void classNode(SyntaxNode node, bool Interface) {
            cls = new Class();
            clss.Add(cls);
            cls.name = file.model.GetDeclaredSymbol(node).Name;
            cls.Namespace = Namespace;
            cls.Interface = Interface;
            if (!Interface) {
                Method init = new Method();
                init.cls = cls;
                init.type = "void";
                init.primative = true;
                init.Private = true;
                init.name = "$init";
                cls.methods.Add(init);
            }
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach(var child in nodes) {
                switch (child.Kind()) {
                    case SyntaxKind.FieldDeclaration:
                        if (!cls.omitFields) fieldNode(child);
                        break;
                    case SyntaxKind.ConstructorDeclaration:
                        if (!cls.omitConstructors) ctorNode(child);
                        break;
                    case SyntaxKind.DestructorDeclaration:
                        if (!cls.omitMethods) methodNode(child, true);
                        break;
                    case SyntaxKind.MethodDeclaration:
                        if (!cls.omitMethods) methodNode(child, false);
                        break;
                    case SyntaxKind.BaseList:
                        baseListNode(child);
                        break;
                    case SyntaxKind.AttributeList:
                        attributeListNode(child, cls);
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
                        String baseName = baseNode.ToString().Replace(".", "::");
                        if (baseName == "System::Exception") continue;
                        if (baseName == "System::Attribute") continue;
                        if (isClass(baseNode))
                            cls.bases.Add(baseName);
                        else
                            cls.ifaces.Add(baseName);
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
                        if (attributeListNode(child, field, field)) return;
                        break;
                    case SyntaxKind.VariableDeclaration:
                        SyntaxNode equals = variableDeclaration(child, field);
                        if (equals != null) {
                            field.Append(cls.name);
                            field.Append("::");
                            field.Append(field.name);
                            field.Append(" = ");
                            SyntaxNode equalsChild = GetChildNode(equals);
                            if (equalsChild.Kind() == SyntaxKind.ArrayInitializerExpression) {
                                arrayInitNode(equalsChild, field, field.type, field.arrays);
                            } else {
                                expressionNode(GetChildNode(equals), field);
                            }
                            field.Append(";\r\n");
                        }
                        break;
                }
            }
            cls.fields.Add(field);
        }

        //method or field attributes (returns true if ommited)
        private bool attributeListNode(SyntaxNode node, OutputBuffer ob, Type type) {
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
                                switch (name) {
                                    case "Qt::Core::weak":
                                        type.weakRef = true;
                                        break;
                                    case "Qt::QSharp::CPPOmitField":
                                    case "Qt::QSharp::CPPOmitMethod":
                                    case "Qt::QSharp::CPPOmitConstructor":
                                        return true;
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
                                            cls.bases.Add(value);
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
        private SyntaxNode variableDeclaration(SyntaxNode node, Type type) {
            SyntaxNode equals = null;
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
                    case SyntaxKind.PredefinedType:
                    case SyntaxKind.IdentifierName:
                    case SyntaxKind.QualifiedName:
                        ITypeSymbol symbol1 = file.model.GetTypeInfo(child).Type;
                        if (symbol1 != null) {
                            type.type = symbol1.ToString().Replace(".", "::");
                        }
                        type.setPrimative();
                        break;
                    case SyntaxKind.VariableDeclarator:
                        getFlags(type, file.model.GetDeclaredSymbol(child));
                        ISymbol symbol2 = file.model.GetDeclaredSymbol(child);
                        if (symbol2 != null) {
                            type.name = symbol2.Name;
                        }
                        equals = GetChildNode(child);
                        break;
                }
            }
            return equals;
        }

        private void ctorNode(SyntaxNode node) {
            method = new Method();
            cls.methods.Add(method);
            method.cls = cls;
            method.name = "$ctor";  //C++ = cls.name;
            method.type = "void";
            method.primative = true;
            method.ctor = true;
            cls.hasctor = true;
            getFlags(method, file.model.GetDeclaredSymbol(node));
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
                        blockNode(child, true, false);
                        break;
                }
            }
            createNewMethod(cls, method.args);
        }

        private void createNewMethod(Class cls, List<Type> args) {
            Method method = new Method();
            method.Public = true;
            method.Static = true;
            method.name = "$new";
            method.type = cls.name;
            method.Append("{\r\n");
            method.Append("std::shared_ptr<" + cls.name + ">$this = std::make_shared<" + cls.name + ">(" + cls.ctorArgs + ");\r\n");
            method.Append("$this->$this = $this;\r\n");
            method.Append("$this->$init();\r\n");
            if (args != null) {
                method.Append("$this->$ctor(");
                bool first = true;
                foreach(var arg in args) {
                    if (!first) method.Append(","); else first = false;
                    method.Append(arg.name);
                    method.args.Add(arg);
                }
                method.Append(");\r\n");
            }
            method.Append("return $this;\r\n");
            method.Append("}\r\n");
            cls.methods.Add(method);
        }

        private void methodNode(SyntaxNode node, bool dtor) {
            method = new Method();
            method.cls = cls;
            if (dtor) {
                method.name = "~" + cls.name;
                method.Virtual = true;
                method.Public = true;
            } else {
                method.name = file.model.GetDeclaredSymbol(node).Name;
            }
            getFlags(method, file.model.GetDeclaredSymbol(node));
            if (dtor) {
                method.Protected = false;
            }
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            //nodes : [return type], parameter list, block
            foreach(var child in nodes) {
                switch (child.Kind()) {
                    case SyntaxKind.PredefinedType:
                    case SyntaxKind.IdentifierName:
                    case SyntaxKind.QualifiedName:
                        ISymbol symbol = file.model.GetSymbolInfo(child).Symbol;
                        if (symbol == null) {
                            Console.WriteLine("Error:Symbol not found for:" + child);
                            Environment.Exit(0);
                        }
                        method.type = symbol.ToString().Replace(".", "::");
                        method.setPrimative();
                        break;
                    case SyntaxKind.ParameterList:
                        parameterListNode(child);
                        break;
                    case SyntaxKind.Block:
                        blockNode(child, true, false);
                        break;
                    case SyntaxKind.AttributeList:
                        if (attributeListNode(child, method, method)) return;
                        break;
                    case SyntaxKind.ArrayType:
                        method.array = true;
                        method.arrays++;
                        parameterNode(GetChildNode(child), method);
                        break;
                }
            }
            cls.methods.Add(method);
        }

        private void parameterListNode(SyntaxNode node) {
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach(var param in nodes) {
                switch (param.Kind()) {
                    case SyntaxKind.Parameter:
                        SyntaxNode par = GetChildNode(param);
                        Type type = new Type();
                        parameterNode(par, type);
                        type.name = file.model.GetDeclaredSymbol(param).Name.Replace(".", "::");
                        method.args.Add(type);
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
                    type.setPrimative();
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
                default:
                    Console.WriteLine("Unknown arg type:" + node.Kind());
                    break;
            }
        }

        private void blockNode(SyntaxNode node, bool top, bool throwFinally) {
            method.Append("{\r\n");
            if (top) {
                if (!method.Static) {
                    method.Append("std::shared_ptr<" + cls.name + "> $this = this->$this.lock();\r\n");
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
                                method.Append("} catch(std::shared_ptr<FinallyException> __finally" + finallyCount++ + ") ");
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
                    blockNode(node, false, false);
                    break;
            }
        }

        private void expressionNode(SyntaxNode node, OutputBuffer ob) {
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            Type type;
            switch (node.Kind()) {
                case SyntaxKind.IdentifierName:
                case SyntaxKind.PredefinedType:
                case SyntaxKind.QualifiedName:
                    type = new Type();
                    type.type = node.ToString().Replace(".", "::");
                    ISymbol symbol = file.model.GetSymbolInfo(node).Symbol;
                    if (symbol != null && symbol.ToString() == "System.Array.Length") {
                        ob.Append("size()");
                    } else {
                        ob.Append(type.ConvertType());
                        cls.addUsage(type.type);
                    }
                    break;
                case SyntaxKind.VariableDeclaration:
                    type = new Type();
                    SyntaxNode equals = variableDeclaration(node, type);
                    method.Append(type.GetTypeDecl());
                    method.Append(" ");
                    method.Append(type.name);
                    if (equals != null) {
                        method.Append(" = ");
                        SyntaxNode equalsChild = GetChildNode(equals);
                        if (equalsChild.Kind() == SyntaxKind.ArrayInitializerExpression) {
                            arrayInitNode(equalsChild, method, type.type, type.arrays);
                        } else {
                            expressionNode(equalsChild, method);
                        }
                    }
                    break;
                case SyntaxKind.SimpleAssignmentExpression:
                    assignNode(node);
                    break;
                case SyntaxKind.InvocationExpression:
                    invokeNode(node, ob, false);
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
                    ob.Append("std::make_shared<String>(\"");
                    ob.Append(file.model.GetConstantValue(node).Value.ToString().Replace("\r", "\\r").Replace("\n", "\\n"));
                    ob.Append("\")");
                    break;
                case SyntaxKind.CharacterLiteralExpression:
                    ob.Append("\'");
                    ob.Append(file.model.GetConstantValue(node).Value.ToString());
                    ob.Append("\'");
                    break;
                case SyntaxKind.SimpleMemberAccessExpression:
                    SyntaxNode left = GetChildNode(node, 1);
                    SyntaxNode right = GetChildNode(node, 2);
                    if (isStatic(right) || left.Kind() == SyntaxKind.BaseExpression) {
                        expressionNode(left, ob);
                        ob.Append("::");
                        expressionNode(right, ob);
                    } else {
                        ob.Append("$deref(");  //NPE check
                        expressionNode(left, ob);
                        ob.Append(")->");
                        expressionNode(right, ob);
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
                    expressionNode(array, ob);
                    ob.Append("))[");
                    expressionNode(index, ob);
                    ob.Append("]");
                    break;
                case SyntaxKind.BracketedArgumentList:
                    foreach(var child in nodes) {
                        expressionNode(child, ob);
                    }
                    break;
                case SyntaxKind.Argument:
                    expressionNode(GetChildNode(node), ob);
                    break;
                case SyntaxKind.AddExpression:
                    binaryNode(node, ob, "+");
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
                    modNode(node, ob, "%");  //C++ does not support float % -- must use a special function
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
                case SyntaxKind.LogicalNotExpression:
                    ob.Append("!");
                    expressionNode(GetChildNode(node), ob);
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
                    expressionNode(GetChildNode(node), ob);
                    ob.Append("--");
                    break;
                case SyntaxKind.PreDecrementExpression:
                    ob.Append("--");
                    expressionNode(GetChildNode(node), ob);
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
            }
        }

        private void arrayInitNode(SyntaxNode node, OutputBuffer ob, String type, int dims) {
            IEnumerable<SyntaxNode> list = node.ChildNodes();
            bool first = true;
            for(int a=0;a<dims;a++) {
                if (a == 0)
                    ob.Append("std::make_shared<QVector<");
                else
                    ob.Append("std::shared_ptr<QVector<");
            }
            ob.Append(type);
            for(int a=0;a<dims;a++) {
                ob.Append(">>");
            }
            ob.Append("(std::initializer_list<");
            for(int a=1;a<dims;a++) {
                ob.Append("std::shared_ptr<QVector<");
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
            return (type.TypeKind.ToString() == "Class");
        }

        private void binaryNode(SyntaxNode node, OutputBuffer ob, string op) {
            expressionNode(GetChildNode(node, 1), ob);
            ob.Append(op);
            expressionNode(GetChildNode(node, 2), ob);
        }

        private void modNode(SyntaxNode node, OutputBuffer ob, string op) {
            ob.Append("$mod(");
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
            //C++ std::static_pointer_cast<type>(value)
            Type type = new Type();
            type.type = castType.ToString();
            type.setPrimative();
            if (!type.primative) ob.Append("std::static_pointer_cast<"); else ob.Append("static_cast<");  //reinterpret_cast ?
            expressionNode(castType, ob);
            ob.Append(">");
            ob.Append("(");
            expressionNode(value, ob);
            ob.Append(")");
        }

        private void newArrayNode(SyntaxNode node, OutputBuffer ob) {
            //node = ArrayCreationExpression -> {ArrayType -> {type, [[rank -> size] ...]} [, ArrayInitializerExpression -> {...}]}
            SyntaxNode arrayType = GetChildNode(node);
            IEnumerable<SyntaxNode> nodes = arrayType.ChildNodes();
            SyntaxNode type = null;
            SyntaxNode size = null;
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
                                if (size != null) Console.WriteLine("Error:multiple sizes for ArrayCreationExpression");
                                size = rank;  //*Expression
                                break;
                        }
                        break;
                    default:  //PredefinedType or IdentifierName
                        type = child;
                        break;
                }
            }
            SyntaxNode initList = GetChildNode(node, 2);
            if (initList != null && initList.Kind() == SyntaxKind.ArrayInitializerExpression) {
                //ob.Append("=");
                arrayInitNode(initList, ob, type.ToString(), dims);
                return;
            }
            if (type == null || size == null) {
                Console.WriteLine("Error:Invalid ArrayCreationExpression : " + type + " : " + size);
                return;
            }
            for(int a=0;a<dims;a++) {
              ob.Append("std::make_shared<QVector<");
            }
            ob.Append(type.ToString());
            for(int a=0;a<dims;a++) {
                ob.Append(">>");
            }
            ob.Append("(");
            expressionNode(size, ob);
            ob.Append(")");
        }

        private void assignNode(SyntaxNode node) {
            //lvalue = rvalue
            IEnumerator<SyntaxNode> nodes = node.ChildNodes().GetEnumerator();
            nodes.MoveNext();
            SyntaxNode left = nodes.Current;
            nodes.MoveNext();
            SyntaxNode right = nodes.Current;
            expressionNode(left, method);
            method.Append(" = ");
            expressionNode(right, method);
        }

        private void invokeNode(SyntaxNode node, OutputBuffer ob, bool New) {
            //IdentifierName/SimpleMemberAccessExpression/QualifiedName, ArgumentList
            SyntaxNode id = GetChildNode(node, 1);
            SyntaxNode args = GetChildNode(node, 2);
            if (isCPP(id)) {
                SyntaxNode arg = GetChildNode(args);  //Argument
                SyntaxNode str = GetChildNode(arg);  //StringLiteralExpression
                ob.Append(file.model.GetConstantValue(str).Value.ToString());
                return;
            }
            expressionNode(id, ob);
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
                        expressionNode(GetChildNode(child), ob);
                        break;
                }
            }
        }

        private SyntaxNode GetChildNode(SyntaxNode node, int idx = 1) {
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

    class Class : Flags
    {
        public string name;
        public string Namespace = "";
        public bool hasctor;
        public bool Interface;
        public List<string> bases = new List<string>();
        public List<string> ifaces = new List<string>();
        public List<Field> fields = new List<Field>();
        public List<Method> methods = new List<Method>();
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
    }

    class Type : Flags {
        public string name;
        public string type;
        public bool primative;
        public bool weakRef;
        public bool array;
        public int arrays;  //# of dimensions
        public void setPrimative() {
            switch (type) {
                case "void": primative = true; break;
                case "bool": primative = true; break;
                case "byte": primative = true; break;
                case "sbyte": primative = true; break;
                case "short": primative = true; break;
                case "ushort": primative = true; break;
                case "int": primative = true; break;
                case "uint": primative = true; break;
                case "long": primative = true; break;
                case "ulong": primative = true; break;
                case "char": primative = true; break;
                case "float": primative = true; break;
                case "double": primative = true; break;
                default: primative = false; break;
            }
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
        public string GetTypeDecl() {
            if (type == null || type == "") {
                if (name.StartsWith("~")) return "";
                Console.WriteLine("type unknown:" + name);
            }
            StringBuilder sb = new StringBuilder();
            for(int a=0;a<arrays;a++) {
                sb.Append("std::shared_ptr<QVector<");
            }
            if (primative) {
                sb.Append(ConvertType());
            } else {
                if (weakRef)
                    sb.Append("std::weak_ptr<" + ConvertType() + ">");
                else
                    sb.Append("std::shared_ptr<" + ConvertType() + ">");
            }
            for(int a=0;a<arrays;a++) {
                sb.Append(">>");
            }
            return sb.ToString();
        }
    }

    interface OutputBuffer {
        void Append(string s);
        void Insert(int idx, string s);
        int Length();
        bool isField();
        bool isMethod();
    }

    class Field : Type, OutputBuffer
    {
        public StringBuilder src = new StringBuilder();
        public void Append(string s) {
            src.Append(s);
        }
        public void Insert(int idx, string s) {
            src.Insert(idx, s);
        }
        public int Length() {return src.Length;}
        public bool isField() {return true;}
        public bool isMethod() {return false;}

        public string GetDeclaration() {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetFlags(false));
            if (array) {
                sb.Append(" ");
                for(int a=0;a<arrays;a++) {
                    sb.Append("std::shared_ptr<QVector<");
                }
                sb.Append(GetTypeDecl());
                for(int a=0;a<arrays;a++) {
                    sb.Append(">>");
                }
                sb.Append(" ");
                sb.Append(name);
                sb.Append(";\r\n");
            } else {
                sb.Append(" ");
                sb.Append(GetTypeDecl());
                sb.Append(" ");
                sb.Append(name);
                sb.Append(";\r\n");
            }
            return sb.ToString();
        }
    }

    class Method : Type, OutputBuffer
    {
        public StringBuilder src = new StringBuilder();
        public void Append(string s) {
            src.Append(s);
        }
        public void Insert(int idx, string s) {
            src.Insert(idx, s);
        }
        public int Length() {return src.Length;}
        public bool isField() {return false;}
        public bool isMethod() {return true;}

        public bool ctor;
        public String basector;
        public List<Type> args = new List<Type>();
        public Class cls;
        public string GetArgs() {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            bool first = true;
            foreach(var arg in args) {
                if (!first) sb.Append(","); else first = false;
                sb.Append(arg.GetTypeDecl());
                sb.Append(" ");
                sb.Append(arg.name);
            }
            sb.Append(")");
            return sb.ToString();
        }
        public string GetPureVirtual() {
            if (Abstract)
                return " = 0";
            else
                return "";
        }
        public string GetDeclaration() {
            return GetFlags(false) + " " + GetTypeDecl() + " " + name + GetArgs() + GetPureVirtual() + ";\r\n";
        }
    }
}
