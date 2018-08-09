using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "QDomDocument *$doc;" +
        "QDomElement *$q;"
    )]
    public class XMLTag {
        [CPPReplaceArgs("QDomDocument *doc, QString name")]
        private XMLTag(NativeArg1 arg1, NativeArg2 arg2) {
            CPP.Add("$doc = doc;");
            CPP.Add("$q = new QDomElement($doc->createElement(name));");
        }
        [CPPReplaceArgs("QDomDocument *doc, QDomElement elem")]
        private XMLTag(NativeArg3 arg1, NativeArg4 arg2) {
            CPP.Add("$doc = doc;");
            CPP.Add("$q = new QDomElement(elem);");
        }
        public void SetName(String name) {
            CPP.Add("$q->setTagName($check(name)->qstring());");
        }
        public String GetName() {
            return CPP.ReturnString("new Qt::Core::String($q->tagName())");
        }
        public XMLTag GetParent() {
            return (XMLTag)CPP.ReturnObject("new XMLTag($doc, $q->parentNode().toElement())");
        }
        public XMLTag GetChild(int idx) {
            return (XMLTag)CPP.ReturnObject("new XMLTag($doc, $q->childNodes().at(idx).toElement())");
        }
        public XMLTag[] GetChilds() {
            int cnt = GetChildCount();
            XMLTag[] childs = new XMLTag[cnt];
            for(int idx=0;idx<cnt;idx++) {
                childs[idx] = GetChild(idx);
            }
            return childs;
        }
        public XMLTag AddChild(String name) {
            CPP.Add("XMLTag* child = new XMLTag($doc, $check(name)->qstring());");
            CPP.Add("$q->appendChild(*$check(child)->$q);");
            return (XMLTag)CPP.ReturnObject("child");
        }
        public XMLTag AddChild(XMLTag child) {
            CPP.Add("$q->appendChild(*$check(child)->$q);");
            return (XMLTag)CPP.ReturnObject("child");
        }
        public XMLTag InsertChild(int index, String name) {
            CPP.Add("XMLTag* child = new XMLTag($doc, $check(name)->qstring());");
            CPP.Add("$q->insertBefore(*$check(child)->$q, $q->childNodes().at(index));");
            return (XMLTag)CPP.ReturnObject("child");
        }
        public XMLTag InsertChild(int index, XMLTag child) {
            CPP.Add("$q->insertBefore(*$check(child)->$q, $q->childNodes().at(index));");
            return (XMLTag)CPP.ReturnObject("child");
        }
        public void RemoveChild(int index) {
            CPP.Add("$q->removeChild($q->childNodes().at(index));");
        }
        public void RemoveChild(XMLTag child) {
            CPP.Add("$q->removeChild(*$check(child)->$q);");
        }
        public int GetChildCount() {
            return CPP.ReturnInt("$q->childNodes().size();");
        }
        public XMLAttr GetAttr(int index) {
            return new XMLAttr(this, index);
        }
        public XMLAttr GetAttr(String name) {
            return new XMLAttr(this, name);
        }
        public void SetAttr(String name, String value) {
            CPP.Add("$q->setAttribute($check(name)->qstring(), $check(value)->qstring());");
        }
        public void RemoveAttr(int index) {
            CPP.Add("XMLAttr* attr = GetAttr(index);");
            CPP.Add("$q->removeAttribute($check(attr)->GetName()->qstring());");
        }
        public void RemoveAttr(String name) {
            CPP.Add("$q->removeAttribute($check(name)->qstring());");
        }
        public int GetAttrCount() {
            return CPP.ReturnInt("$q->attributes().count()");
        }
        public String GetContent() {
            return CPP.ReturnString("new Qt::Core::String($q->nodeValue())");
        }
        public void SetContent(String text) {
            CPP.Add("$q->setNodeValue($check(text)->qstring());");
        }
    }
    [CPPClass(
        "QDomAttr *$q;"
    )]
    public class XMLAttr {
        public XMLAttr(XMLTag tag, String name) {
            CPP.Add("$q = new QDomAttr($check(tag)->$q->attributeNode($check(name)->qstring()));");
        }
        public XMLAttr(XMLTag tag, int index) {
            CPP.Add("QString name = $check(tag)->$q->attributes().item(index).nodeName();");
            CPP.Add("$q = new QDomAttr($check(tag)->$q->attributeNode(name));");
        }
        public String GetName() {
            return CPP.ReturnString("new Qt::Core::String($q->name())");
        }
        public String GetValue() {
            return CPP.ReturnString("new Qt::Core::String($q->value())");
        }
        public void SetValue(String text) {
            CPP.Add("$q->setValue($check(text)->qstring());");
        }
    }
    [CPPClass(
        "std::qt_ptr<QDomDocument> $q;"
    )]
    public class XML {
        public XML() {
            CPP.Add("$q = new QDomDocument();");
            CPP.Add("$q->appendChild($q->createElement(\"root\"));");
        }
        public bool Load(String input) {
            return CPP.ReturnBool("$q->setContent($check(input)->qstring())");
        }
        public String Save() {
            return CPP.ReturnString("new Qt::Core::String($q->toString())");
        }
        public XMLTag GetRoot() {
            return (XMLTag)CPP.ReturnObject("new XMLTag($q.get(), $q->documentElement())");
        }
    }
}
