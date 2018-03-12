using Qt.Core;

namespace Qt.Controls.jfc {

/** jfControls Tag
 *
 * @author pquiring
 */

public class JFTag {
    public JFTag(String name) {
        this.tag = name;
    }
    public String tag;
    public int type;
    public byte[] data;
}

}
