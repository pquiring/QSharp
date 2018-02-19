using Qt.Core;
using Qt.QSharp;

namespace Qt.Core {
    public delegate void Symbol();
    public delegate void LibraryMain(Object obj);
    [CPPClass(
        "private: std::shared_ptr<QLibrary> $q;"
    )]
    public class Library {
        public Library(String filename) {
            CPP.Add("$q = std::make_shared<QLibrary>($check(filename)->qstring());");
        }
        public bool Load() {
            return CPP.ReturnBool("$q->load()");
        }
        public void Unload() {
            CPP.Add("$q->unload();");
        }
        /** Returns a symbol as a delegate.*/
        public Symbol GetSymbol(String symname) {
            return (Symbol)CPP.ReturnObject("$q->resolve($check(symname)->cstring())");
        }
        public LibraryMain GetLibraryMain() {
            return (LibraryMain)CPP.ReturnObject("(void (*)(std::shared_ptr<Qt::Core::Object>))($q->resolve(\"LibraryMain\"))");
        }
    }
}
