using Qt.QSharp;

namespace Qt.Core {
    public delegate void LibraryMain(Object obj);
    [CPPClass(
        "std::shared_ptr<QLibrary> $q;"
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
        public Callable GetSymbol(String symname) {
            return (Callable)CPP.ReturnObject("(void (*)())($q->resolve($check(symname)->cstring()))");
        }
        public LibraryMain GetLibraryMain() {
            return (LibraryMain)CPP.ReturnObject("(void (*)(std::shared_ptr<Qt::Core::Object>))($q->resolve(\"LibraryMain\"))");
        }
    }
}
