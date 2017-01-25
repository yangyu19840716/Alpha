using System.Collections.Generic;

public class Singleton<T> where T : new()
{
    static T instance;
    public static T GetInstacne()
    {
        if (instance == null)
            instance = new T();
        return instance;
    }
}