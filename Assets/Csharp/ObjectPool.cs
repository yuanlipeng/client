using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool<T>
{
    private Queue<T> objList = new Queue<T>();
    private int poolLen = 0;
    private Func<T> createFunc = null;
    private Action<T> disposeFunc = null;
    private Action<T> initFunc = null;

    public ObjectPool(int len, Func<T> cFunc, Action<T> dFunc = null, Action<T>iFunc = null)
    {
        poolLen = len;
        createFunc = cFunc;
        disposeFunc = dFunc;
        initFunc = iFunc;
    }

    public T GetItem()
    {
        if (objList.Count == 0)
        {
            T newItem = createFunc();
            objList.Enqueue(newItem);
        }

        return objList.Dequeue();
    }

    public void ReturnItem(T item)
    {
        if(initFunc != null)
        {
            initFunc(item);
        }

        if (objList.Count >= poolLen)
        {
            if (disposeFunc != null)
            { 
                disposeFunc(item);
            }
        }
        else
        {
            objList.Enqueue(item);
        }
    }

    public void Clear()
    {
        while(objList.Count!=0)
        {
            T item = objList.Dequeue();
            if(initFunc!=null)
            {
                initFunc(item);
            }
            if (disposeFunc != null)
            {
                disposeFunc(item);
            }
        }
    }
}
