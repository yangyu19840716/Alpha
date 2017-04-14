using UnityEngine;

class LogModule
{
    static public void Log(string format, params object[] args)
    {
        string msg = string.Format(format, args);
        Debug.Log(msg);
    }
}