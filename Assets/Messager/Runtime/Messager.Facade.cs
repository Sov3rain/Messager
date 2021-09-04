using Messaging;

static public partial class Facade
{
    static public Messager Messager => Messager.DefaultInstance;
}