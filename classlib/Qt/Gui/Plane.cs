using Qt.Core;

namespace Qt.Gui {

public class Plane {
  private Vector3D aux1 = new Vector3D();
  private Vector3D aux2 = new Vector3D();

  public Vector3D normal = new Vector3D();
  public float d;

  public void Set3Points(Vector3D v1, Vector3D v2, Vector3D v3) {
    aux1.Set(v1);
    aux1.Sub(v2);
    aux2.Set(v3);
    aux2.Sub(v2);

    normal = Vector3D.CrossProduct(aux2, aux1);
    normal.Normalize();
    d = -Vector3D.DotProduct(normal, v2);
  }

  public void SetNormalAndPoint(Vector3D normal, Vector3D point) {
    this.normal.Set(normal);
    this.normal.Normalize();
    d = -(Vector3D.DotProduct(this.normal, point));
  }

  public void SetCoefficients(float a, float b, float c, float d) {
    // set the normal vector
    normal.Set(a,b,c);
    //compute the lenght of the vector
    float l = normal.Length();
    // normalize the vector
    normal.Set(a/l,b/l,c/l);
    // and divide d by th length as well
    this.d = d/l;
  }

  public float Length(Vector3D p) {
    return (d + Vector3D.DotProduct(normal, p));
  }

/*
  public void print() {
    System.out.println(String.format("GLPlane:%7.3f,%7.3f,%7.3f : %7.3f", normal.v[0], normal.v[1], normal.v[2], d));
  }
*/
}

}
