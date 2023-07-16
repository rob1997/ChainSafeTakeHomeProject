using UnityEngine;

namespace Core.Utils
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        #region InstanceInitialized

        public delegate void InstanceInitialized(T instance);

        public static event InstanceInitialized OnInstanceInitialized;

        private void InvokeInstanceInitialized(T instance)
        {
            OnInstanceInitialized?.Invoke(instance);
        }

        #endregion
        
        static T _instance;
    
        public static T Instance => _instance;

        public static bool IsInitialized => _instance != null;

        [SerializeField] private bool _dontDestroyOnLoad;
        
        protected virtual void Awake()
        {
            if (_instance != null)
            {
                Debug.LogError("More than one Singleton Instance of " + _instance.name);
            }

            else
            {
                _instance = (T) this;

                InvokeInstanceInitialized(_instance);
                
                if (_dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}