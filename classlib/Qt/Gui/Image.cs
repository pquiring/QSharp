using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "std::qt_ptr<QImage> $q;" +
        "uint32* $px;" +
        "uint8* $px8;" +
        "QPainter $painter;" +
        "QIcon $icon() {return QIcon(QPixmap::fromImage(*$q.get()));}"
    )]
    public class Image {
        public static String PNG = "PNG";
        public static String JPEG = "JPEG";
        public static String BMP = "BMP";
        private int _width;
        private int _height;
        private Image(Image image, int x, int y, int width, int height) {
            CPP.Add("$q = new QImage($check(image)->$q->copy(x,y,width,height));");
            GetPtr();
        }
        private Image(Image image, int newWidth, int newHeight) {
            CPP.Add("$q = new QImage($check(image)->$q->scaled(newWidth,newHeight));");
            GetPtr();
        }
        public Image(int width, int height) {
            CPP.Add("$q = new QImage(width, height, QImage::Format_ARGB32);");
            CPP.Add("$q->fill(0);");
            GetPtr();
        }
        private void GetPtr() {
            CPP.Add("$px = (uint32*)$q->bits();");
            CPP.Add("$px8 = (uint8*)$px;");
            _width = GetWidth();
            _height = GetHeight();
        }
        public int GetWidth() {return CPP.ReturnInt("$q->width()");}
        public int GetHeight() {return CPP.ReturnInt("$q->height()");}
        public bool Load(String file, String fmt = null) {
            bool ok = false;
            CPP.Add("ok = $q->load($check(file)->qstring(), fmt == nullptr ? nullptr : fmt->cstring().constData());");
            if (ok) {
                GetPtr();
            }
            return ok;
        }
        public bool Load(IOStream io, String fmt = null) {
            bool ok = false;
            CPP.Add("ok = $q->load($check(io)->$q.get(), fmt == nullptr ? nullptr : fmt->cstring().constData());");
            if (ok) {
                GetPtr();
            }
            return ok;
        }
        public bool Save(String file, String fmt = null) {
            return CPP.ReturnBool("$q->save($check(file)->qstring(), fmt == nullptr ? nullptr : fmt->cstring().constData())");
        }
        public bool Save(IOStream io, String fmt) {
            return CPP.ReturnBool("$q->save($check(io)->$q.get(), fmt == nullptr ? nullptr : fmt->cstring().constData())");
        }
        public void SetPixel(int x, int y, uint px) {
            px |= 0xff000000;
            int offset = y * _width + x;
            CPP.Add("$px[offset] = px;");
        }
        public uint GetPixel(int x, int y) {
            int offset = y * _width + x;
            return CPP.ReturnUInt("$px[offset]");
        }
        public void SetAlpha(int x, int y, byte alpha) {
            int offset = (y * _width + x) * 4 + 3;
            CPP.Add("$px8[offset] = alpha;");
        }
        public byte GetAlpha(int x, int y) {
            int offset = (y * _width + x) * 4 + 3;
            return CPP.ReturnByte("$px8[offset]");
        }
        public uint[] GetPixels() {
            return (uint[])CPP.ReturnObject("new Qt::QSharp::FixedArray<uint32>($px, _width * _height)");
        }
        public void SetPixels(uint[] px) {
            int pxs = _width * _height;
            if (px == null) throw new NullPointerException();
            if (px.Length != pxs) return;
            CPP.Add("std::memcpy($px, px->data(), pxs * 4);");
        }
        /** Sets color used by all Draw operations. */
        public void SetColor(int clr) {
            CPP.Add("$painter.setPen(QColor(clr));");
        }
        public void DrawLine(int x1, int y1, int x2, int y2) {
            CPP.Add("$painter.begin($q.get());");
            CPP.Add("$painter.drawLine(x1,y1,x2,y2);");
            CPP.Add("$painter.end();");
        }
        public void DrawArc(int x, int y, int width, int height, int startAngle, int spanAngle) {
            CPP.Add("$painter.begin($q.get());");
            CPP.Add("$painter.drawArc(x,y,width,height,startAngle,spanAngle);");
            CPP.Add("$painter.end();");
        }
        public void DrawChord(int x, int y, int width, int height, int startAngle, int spanAngle) {
            CPP.Add("$painter.begin($q.get());");
            CPP.Add("$painter.drawChord(x,y,width,height,startAngle,spanAngle);");
            CPP.Add("$painter.end();");
        }
        public void DrawEllipse(int x, int y, int width, int height) {
            CPP.Add("$painter.begin($q.get());");
            CPP.Add("$painter.drawEllipse(x,y,width,height);");
            CPP.Add("$painter.end();");
        }
        public void DrawImage(int x, int y, Image src, int sx = 0, int sy = 0, int width = -1, int height = -1) {
            CPP.Add("$painter.begin($q.get());");
            CPP.Add("$painter.drawImage(x,y,*$check(src)->$q,sx,sy,width,height);");
            CPP.Add("$painter.end();");
        }
        public void DrawPie(int x, int y, int width, int height, int startAngle, int spanAngle) {
            CPP.Add("$painter.begin($q.get());");
            CPP.Add("$painter.drawPie(x,y,width,height,startAngle,spanAngle);");
            CPP.Add("$painter.end();");
        }
        public void DrawPoint(int x, int y) {
            CPP.Add("$painter.begin($q.get());");
            CPP.Add("$painter.drawPoint(x,y);");
            CPP.Add("$painter.end();");
        }
        public void DrawRect(int x, int y, int width, int height) {
            CPP.Add("$painter.begin($q.get());");
            CPP.Add("$painter.drawRect(x,y,width,height);");
            CPP.Add("$painter.end();");
        }
        public void DrawRoundedRect(int x, int y, int width, int height, float xRadius, float yRadius) {
            CPP.Add("$painter.begin($q.get());");
            CPP.Add("$painter.drawRoundedRect(x,y,width,height,xRadius,yRadius);");
            CPP.Add("$painter.end();");
        }
        private Font font;
        public void SetFont(Font font) {
            this.font = font;
            CPP.Add("$painter.setFont(*$check(font)->$q);");
        }
        public void DrawText(int x, int y, String text) {
            CPP.Add("$painter.begin($q.get());");
            CPP.Add("$painter.drawText(x,y,$check(text)->qstring());");
            CPP.Add("$painter.end();");
        }
        public Image GetSubImage(int x, int y, int width, int height) {
            return new Image(this, x, y, width, height);
        }
        public Image GetScaledImage(int newWidth, int newHeight) {
            return new Image(this, newWidth, newHeight);
        }
    }
}
