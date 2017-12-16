using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "public: QDomElement *$q;"
    )]
    public class XMLTag {
        public XMLTag() {
            CPP.Add("$q = new QDomElement();");
        }
        [CPPReplaceArgs("QDomElement elem")]
        private XMLTag(NativeArg1 arg) {
            CPP.Add("$q = new QDomElement(elem);");
        }
        public void SetName(String name) {
            CPP.Add("$q->setTagName(name->qstring());");
        }
        public String GetName() {
            return CPP.ReturnString("String::$new($q->tagName())");
        }
        public XMLTag GetChild(int idx) {
            return (XMLTag)CPP.ReturnObject("XMLTag::$new($q->childNodes().at(idx).toElement())");
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
            CPP.Add("std::shared_ptr<XMLTag> child = XMLTag::$new();");
            CPP.Add("child->SetName(name);");
            CPP.Add("$q->insertAfter(*child->$q, $q->lastChild());");
            return (XMLTag)CPP.ReturnObject("child");
        }
        public XMLTag AddChild(XMLTag child) {
            CPP.Add("$q->insertAfter(*child->$q, $q->lastChild());");
            return (XMLTag)CPP.ReturnObject("child");
        }
        public XMLTag InsertChild(int index, String name) {
            CPP.Add("std::shared_ptr<XMLTag> child = XMLTag::$new();");
            CPP.Add("child->SetName(name);");
            CPP.Add("$q->insertBefore(*child->$q, $q->childNodes().at(index));");
            return (XMLTag)CPP.ReturnObject("child");
        }
        public XMLTag InsertChild(int index, XMLTag child) {
            CPP.Add("$q->insertBefore(*child->$q, $q->childNodes().at(index));");
            return (XMLTag)CPP.ReturnObject("child");
        }
        public void RemoveChild(int index) {
            CPP.Add("$q->removeChild($q->childNodes().at(index));");
        }
        public void RemoveChild(XMLTag child) {
            CPP.Add("$q->removeChild(*child->$q);");
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
            CPP.Add("$q->setAttribute(name->qstring(), value->qstring());");
        }
        public void RemoveAttr(int index) {
            CPP.Add("std::shared_ptr<XMLAttr> attr = GetAttr(index);");
            CPP.Add("$q->removeAttribute($deref(attr)->GetName()->qstring());");
        }
        public void RemoveAttr(String name) {
            CPP.Add("$q->removeAttribute(name->qstring());");
        }
        public int GetAttrCount() {
            return CPP.ReturnInt("$q->attributes().count()");
        }
        public String GetContent() {
            return CPP.ReturnString("String::$new($q->nodeValue())");
        }
        public void SetContent(String text) {
            CPP.Add("$q->setNodeValue(text->qstring());");
        }
    }
    [CPPClass(
        "private: QDomAttr *$q;"
    )]
    public class XMLAttr {
        public XMLAttr(XMLTag tag, String name) {
            CPP.Add("$q = new QDomAttr($deref(tag)->$q->attributeNode($deref(name)->qstring()));");
        }
        public XMLAttr(XMLTag tag, int index) {
            CPP.Add("QString name = $deref(tag)->$q->attributes().item(index).nodeName();");
            CPP.Add("$q = new QDomAttr($deref(tag)->$q->attributeNode(name));");
        }
        public String GetName() {
            return CPP.ReturnString("String::$new($q->name())");
        }
        public String GetValue() {
            return CPP.ReturnString("String::$new($q->value())");
        }
        public void SetValue(String text) {
            CPP.Add("$q->setValue(text->qstring());");
        }
    }
    [CPPClass(
        "public: std::unique_ptr<QDomDocument> $q;"
    )]
    public class XML {
        public XML() {
            CPP.Add("$q = std::make_unique<QDomDocument>();");
        }
        public bool Load(String input) {
            return CPP.ReturnBool("$q->setContent(input->qstring());");
        }
        public String Save() {
            return CPP.ReturnString("String::$new($q->toString())");
        }
        public XMLTag GetRoot() {
            return (XMLTag)CPP.ReturnObject("XMLTag::$new($q->documentElement())");
        }
    }
}
