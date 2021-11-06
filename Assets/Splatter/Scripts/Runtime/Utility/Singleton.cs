using System;
using UnityEngine;

namespace Splatter.Utility {
    public class Singleton<T> : MonoBehaviour where T : Singleton<T> {
        private static T instance;
        [SerializeField] private bool dontDestroyOnLoad;

        public static T Instance => instance;
        public static bool IsInitialised => instance != null;

        private void Awake() {
            if (IsInitialised) {
                throw new InvalidOperationException("Trying to initialise another instance of a singleton");
            }

            instance = this as T;

            if (dontDestroyOnLoad) {
                DontDestroyOnLoad(gameObject);
            }

            AwakeSingleton();
        }

        /// <summary>
        /// Override this to initialise values when the singleton is created
        /// </summary>
        protected virtual void AwakeSingleton() { }

        protected virtual void OnDestroy() {
            if (instance == this) {
                instance = null;
            }
        }
    }

}
