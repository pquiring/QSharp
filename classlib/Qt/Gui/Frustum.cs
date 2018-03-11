using Qt.Core;

namespace Qt.Gui {

/**
 * Represents the frustum (viewable area of a perspective)
 *
 * @author pquiring
 */

public class Frustum {
    private Vector3D ntl, ntr, nbl, nbr, ftl, ftr, fbl, fbr;
    private float nearD, farD;
    private float nw, nh, fw, fh;
    private Plane[] pl;
    private Vector3D nc, fc, X, Y, Z;
    private Vector3D XX, YY, ZZ;

    private Vector3D _p, _l, _u;    //pointers

    public Frustum() {
        ntl = new Vector3D();
        ntr = new Vector3D();
        nbl = new Vector3D();
        nbr = new Vector3D();
        ftl = new Vector3D();
        ftr = new Vector3D();
        fbl = new Vector3D();
        fbr = new Vector3D();
        nc = new Vector3D();
        fc = new Vector3D();
        X = new Vector3D();
        Y = new Vector3D();
        Z = new Vector3D();
        XX = new Vector3D();
        YY = new Vector3D();
        ZZ = new Vector3D();
        pl = new Plane[6];
        for (int a = 0; a < 6; a++) {
            pl[a] = new Plane();
        }
    }

    private const float ANG2RAD = (float) Math.PI / 180f;

    public void SetPerspecive(float angle, float ratio, float near, float far) {
        nearD = near;
        farD = far;

        float tan = (float) Math.Tan(angle * ANG2RAD * 0.5);
        nh = nearD * tan;
        nw = nh * ratio;
        fh = farD * tan;
        fw = fh * ratio;
    }

    private const int TOP = 0;
    private const int BOTTOM = 1;
    private const int LEFT = 2;
    private const int RIGHT = 3;
    private const int NEARP = 4;
    private const int FARP = 5;

    public void SetPosition(Vector3D p, Vector3D l, Vector3D u) {
        _p = p;
        _l = l;
        _u = u;
        Z.Set(p);
        Z.Sub(l);
        Z.Normalize();

        X = Vector3D.CrossProduct(u, Z);
        X.Normalize();

        Y = Vector3D.CrossProduct(Z, X);

        ZZ.Set(Z);
        ZZ.Multiply(nearD);
        nc.Set(p);
        nc.Sub(ZZ);
        ZZ.Set(Z);
        ZZ.Multiply(farD);
        fc.Set(p);
        fc.Sub(ZZ);

//        ntl = nc + Y * nh - X * nw;
//        ntr = nc + Y * nh + X * nw;
//        nbl = nc - Y * nh - X * nw;
//        nbr = nc - Y * nh + X * nw;
        YY.Set(Y);
        YY.Multiply(nh);
        XX.Set(X);
        XX.Multiply(nw);

        ntl.Set(nc);
        ntl.Add(YY);
        ntl.Sub(XX);
        ntr.Set(nc);
        ntr.Add(YY);
        ntr.Add(XX);
        nbl.Set(nc);
        nbl.Sub(YY);
        nbl.Sub(XX);
        nbr.Set(nc);
        nbr.Sub(YY);
        nbr.Add(XX);

//        ftl = fc + Y * fh - X * fw;
//        ftr = fc + Y * fh + X * fw;
//        fbl = fc - Y * fh - X * fw;
//        fbr = fc - Y * fh + X * fw;
        YY.Set(Y);
        YY.Multiply(fh);
        XX.Set(X);
        XX.Multiply(fw);

        ftl.Set(fc);
        ftl.Add(YY);
        ftl.Sub(XX);
        ftr.Set(fc);
        ftr.Add(YY);
        ftr.Add(XX);
        fbl.Set(fc);
        fbl.Sub(YY);
        fbl.Sub(XX);
        fbr.Set(fc);
        fbr.Sub(YY);
        fbr.Add(XX);

        pl[TOP].Set3Points(ntr, ntl, ftl);
        pl[BOTTOM].Set3Points(nbl, nbr, fbr);
        pl[LEFT].Set3Points(ntl, nbl, fbl);
        pl[RIGHT].Set3Points(nbr, ntr, fbr);
        pl[NEARP].Set3Points(ntl, ntr, nbr);
        pl[FARP].Set3Points(ftr, ftl, fbl);
    }

    public const int OUTSIDE = 0;
    public const int INTERSECT = 1;
    public const int INSIDE = 2;

    public int PointInside(Vector3D p) {
        int result = INSIDE;
        float d;
        for(int i=0; i < 6; i++) {
            d = pl[i].Length(p);
            if (d < 0) return OUTSIDE;
            if (d == 0f) result = INTERSECT;
        }
        return result;
    }

    /** Tests if sphere is within frustum.
     * @param p = center if sphere
     * @param size = size of sphere (radius or diameter?)
     */
    public int SphereInside(Vector3D p, float size) {
        float distance;
        int result = INSIDE;

        for(int i=0; i < 6; i++) {
            distance = pl[i].Length(p);
            if (distance < -size) return OUTSIDE;
            if (distance < size) result = INTERSECT;
        }
        return result;
    }

    /** Tests if box is within frustum.
     * @param pts = 8 points of box
     */
    public int BoxInside(Vector3D[] pts) {
        int result = INSIDE, _in, _out;
        for(int i=0; i < 6; i++) {
            // reset counters for corners _in and _out
            _out=0;_in=0;
            // for each corner of the box do ...
            // get _out of the cycle as soon as a box as corners
            // both inside and _out of the frustum
            for (int k = 0; k < 8 && (_in==0 || _out==0); k++) {
                // is the corner outside or inside
                if (pl[i].Length(pts[k]) < 0)
                    _out++;
                else
                    _in++;
            }
            //if all corners are _out
            if (_in == 0) return (OUTSIDE);
            // if some corners are _out and others are _in
            if (_out > 0) result = INTERSECT;
        }
        return result;
    }

/*
    public void print() {
        Console.WriteLine(String.format("GLFrustum:%7.3f,%7.3f,%7.3f,%7.3f", nw, nh, fw, fh));
        Console.WriteLine(String.format("GLFrustum:p:%7.3f,%7.3f,%7.3f", _p.v[0], _p.v[1], _p.v[2]));
        Console.WriteLine(String.format("GLFrustum:l:%7.3f,%7.3f,%7.3f", _l.v[0], _l.v[1], _l.v[2]));
        Console.WriteLine(String.format("GLFrustum:u:%7.3f,%7.3f,%7.3f", _u.v[0], _u.v[1], _u.v[2]));
        for(int a=0;a<6;a++) {
            pl[a].print();
        }
    }
*/
}

}
