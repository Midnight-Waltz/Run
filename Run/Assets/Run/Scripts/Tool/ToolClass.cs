using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tool
{
    /// <summary>
    /// 单例模式
    /// </summary>
    public class Singleton : MonoBehaviour
    {
        private static Singleton _instance;
        protected Singleton(){}

        public static Singleton Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = (Singleton)FindObjectOfType(typeof(Singleton));
                    if(_instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<Singleton>();
                        singletonObject.name = typeof(Singleton).ToString();
                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if(_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else{
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            
        }
    }
}

