//ȱ�㣺������Ա�new��������Ϊnew()Լ����֪��ΪʲôҪ��һ��Ҫ�й������캯����
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
