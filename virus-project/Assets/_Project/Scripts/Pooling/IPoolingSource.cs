namespace UnityEngine
{
    public interface IPoolingSource
    {
        GameObject Borrow(GameObject prefab);
        T Borrow<T>(GameObject prefab) where T : class;
        T Borrow<T>(T prefab) where T : MonoBehaviour;
        void Return(GameObject gameObject);
    }
}
