//缺点：子类可以被new出来。因为new()约束不知道为什么要求一定要有公共构造函数。
public abstract class Singleton<T> where T : new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }
}
