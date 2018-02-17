using Qt.Core;
using Qt.QSharp;

namespace Qt.Core {
    public delegate void Symbol();
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
        /** Returns a symbol as a delegate.  You can type cast to another delegate type as needed. */
        public Symbol GetSymbol(String symname) {
            return (Symbol)CPP.ReturnObject("$q->resolve($check(symname)->cstring())");
        }
    }
}
