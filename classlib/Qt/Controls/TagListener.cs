using Qt.Core;

namespace Qt.Controls {

/** Tag Listener
 *
 * @author pquiring
 */

public interface TagListener {
    /** Invoked when a tag's value has changed. */
    void tagChanged(Tag tag, String value);
}

}
