using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SingletonMono<T> : MonoBehaviour where T: SingletonMono<T>
{
    public static T Instance;
    protected virtual void Awake()
    {
        if (Instance==null)
        {
            Instance = (T)this;
        }
    }
}
