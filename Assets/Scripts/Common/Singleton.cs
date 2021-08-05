using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : Singleton<T>, new()
{
    static T m_instance = null;

    public static T Instance()
    {
        if(m_instance == null)
        {
            m_instance = new T();
            m_instance.OnCreate();
        }
        return m_instance;
    }

    protected virtual void OnDestroy() {
        
    }

    protected virtual void OnCreate()
    {

    }

    public static void Destroy()
    {
        if(m_instance != null)
        {
            m_instance.OnDestroy();
            m_instance = null;
        }
    }
}
