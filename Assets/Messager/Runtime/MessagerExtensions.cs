static public class MessagerExtensions
{
    static public Messager Instance { get; set; } = Messager.DefaultInstance;

    static public void Listen<T>(this object obj, System.Action<T> handler) =>
        Instance.Listen(obj, handler);

    static public void Cut<T>(this object obj) => Instance.Cut<T>(obj);

    static public void Dispatch<T>(this object _, T payload) => Instance.Dispatch(payload);
}