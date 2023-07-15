using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game;
using Core.Utils;
using UnityEngine;

namespace Core.Character
{
    public abstract class Character : MonoBehaviour
    {
        #region Ready

        public delegate void Ready();

        public event Ready OnReady;

        private void InvokeReady()
        {
            if (IsReady)
            {
                Debug.LogError($"{nameof(GameManager)} already ready");
            }

            else
            {
                OnReady?.Invoke();

                IsReady = true;
            }
        }

        #endregion
        
        //Used to dispatch events during runtime
        //such as animation events that are only invoked on the GameObject containing the animator
        #region EventDispatched

        public delegate void EventDispatched(string label, params object[] args);

        public event EventDispatched OnEventDispatched;

        protected void InvokeEventDispatched(string label, params object[] args)
        {
            OnEventDispatched?.Invoke(label, args);
        }

        #endregion
        
        [field: SerializeField] public string Title { get; private set; }
        
        [field: TextArea]
        [field: SerializeField] public string Description { get; private set; }

        [field: Space]
        
        [field: SerializeField] public Beamer Beamer { get; private set; }
        
        [field: SerializeField] public Animator Animator { get; private set; }
        
        private Controller[] _controllers = { };

        private CharacterController _characterController;

        public bool IsReady { get; private set; }

        public CharacterController CharacterController
        {
            get
            {
                if (_characterController != null) return _characterController;
                
                if (!TryGetComponent(out _characterController)) Debug.LogError("Character Controller Component not found on Character");

                return _characterController;
            }
        }
        
        protected virtual void Start()
        {
            if (GameManager.Instance.IsReady)
            {
                Initialize();
            }

            else
            {
                GameManager.Instance.OnReady += Initialize;
            }
        }

        protected virtual void Initialize()
        {
            Controller[] controllers = GetComponentsInChildren<Controller>();

            foreach (Controller controller in controllers)
            {
                //to make sure we're not adding duplicates
                if (AddController(controller))
                {
                    controller.Initialize(this);
                
                    controller.InvokeReady();
                }
            }
            
            InvokeReady();
        }

        public bool AddController<T>(T controller) where T : Controller
        {
            Type type = controller.GetType();
            
            if (_controllers.Any(c => c.GetType() == type))
            {
                Debug.LogWarning($"can't add, {type.Name} already exists");
                
                return false;
            }

            _controllers = _controllers.Append(controller).ToArray();

            return true;
        }
        
        public bool GetController<T>(out T controller) where T : Controller
        {
            controller = null;

            controller = (T) _controllers.FirstOrDefault(c => c is T);
        
            return controller != null;
        }
    
        #region Animation Events

        public void Equipped(int slot)
        {
            InvokeEventDispatched(nameof(Equipped), slot);
        }
        
        public void UnEquipped(int slot)
        {
            InvokeEventDispatched(nameof(UnEquipped), slot);
        }
        
        public void Equipped()
        {
            InvokeEventDispatched(nameof(Equipped), 0);
        }
        
        public void UnEquipped()
        {
            InvokeEventDispatched(nameof(UnEquipped), 0);
        }
        
        #endregion
    }
}
