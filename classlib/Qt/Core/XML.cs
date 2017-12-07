using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "public: QDomElement *$q;"
    )]
    public class XMLTag {
        public XMLTag() {
            CPP.Add("$q = new QDomElement();");
        }
        public XMLTag(XML xml) {
            CPP.Add("$q = new QDomElement();");
            CPP.Add("*$q = xml->$q->documentElement();");
        }
        public void SetName(String name) {
            CPP.Add("$q->setTagName(name->qstring());");
        }
        public String GetName() {
            return CPP.ReturnString("String::$new($q->tagName())");
        }
        public XMLTag AddChild(String name) {
            CPP.Add("std::shared_ptr<XMLTag> child = std::make_shared<XMLTag>();");
            CPP.Add("child->SetName(name);");
            CPP.Add("$q->insertAfter(*child->$q, $q->lastChild());");
            return (XMLTag)CPP.ReturnObject("child");
        }
        public XMLTag AddChild(XMLTag child) {
            CPP.Add("$q->insertAfter(*child->$q, $q->lastChild());");
            return (XMLTag)CPP.ReturnObject("child");
        }
        public XMLTag InsertChild(int index, String name) {
            CPP.Add("std::shared_ptr<XMLTag> child = std::make_shared<XMLTag>();");
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
            if (tag == null) throw new NullPointerException();
            CPP.Add("$q = new QDomAttr();");
            CPP.Add("*$q = tag->$q->attributeNode(name->qstring());");
        }
        public XMLAttr(XMLTag tag, int index) {
            if (tag == null) throw new NullPointerException();
            CPP.Add("$q = new QDomAttr();");
            CPP.Add("QString name = tag->$q->attributes().item(index).nodeName();");
            CPP.Add("*$q = tag->$q->attributeNode(name);");
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
        "public: std::shared_ptr<QDomDocument> $q;"
    )]
    public class XML {
        public XML() {
            CPP.Add("$q = std::make_shared<QDomDocument>();");
        }
        public bool Input(String input) {
            return CPP.ReturnBool("$q->setContent(input->qstring());");
        }
        public String Output() {
            return CPP.ReturnString("String::$new($q->toString())");
        }
        public XMLTag GetRoot() {
            return new XMLTag(this);
        }
    }
}
