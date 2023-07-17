using System.Collections.Generic;
using Core.Game;
using Unity.Netcode;
using UnityEngine;

namespace Core.Character
{
    public abstract class Controller : MonoBehaviour
    {
        #region Ready

        public delegate void Ready();

        public event Ready OnReady;

        public void InvokeReady()
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
        
        private Character _character;

        public bool IsReady { get; private set; } = false;
        
        public virtual void Initialize(Character character)
        {
            if (IsReady) return;
            
            _character = character;
        }

        public Character GetCharacter()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return GetComponentInParent<Character>();
#endif
            return _character;
        }
    }
}
