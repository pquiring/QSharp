namespace Qt.Core {
    public interface IEnumerator<T> {
        bool MoveNext();
        T Current {get;}
        void Reset();
    }
}
