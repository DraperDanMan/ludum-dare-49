using System.Runtime.CompilerServices;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// A generic Singleton Monobehaviour will all the safety systems already set up.
    /// </summary>
    /// <typeparam name="TBehaviourType">This is generally going to be your class name that will become the singleton.</typeparam>
    public abstract class SingletonBehaviour<TBehaviourType> : MonoBehaviour where TBehaviourType : class
    {
        public static TBehaviourType Instance
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        }

        public bool IsInitialized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        }

        private bool _isDuplicate;

        /// <summary>
        /// The Awake method for this singleton. All your Awaking should be done in Initialize and/or OnEnable
        /// </summary>
        protected virtual void Awake()
        {
            if (Instance != null)
            {
                _isDuplicate = true;
                Destroy(gameObject);
                Debug.LogError($"Multiple instances of {typeof(TBehaviourType)} detected", this);
            }
            else
            {
                Instance = this as TBehaviourType;
                if (Instance == null)
                    Debug.LogError($"SingletonBehaviour could not cast to {typeof(TBehaviourType)}", this);

                UnityEngine.Profiling.Profiler.BeginSample(GetType().Name + ".Initialize");
                Initialize();
                UnityEngine.Profiling.Profiler.EndSample();
                IsInitialized = true;
            }
        }

        /// <summary>
        /// Safely Handles the shutdown of the Singleton if it is destroyed from somewhere.
        /// </summary>
        protected void OnDestroy()
        {
            if (!_isDuplicate)
            {
                Instance = null;
                Shutdown();
                IsInitialized = false;
            }
            _isDuplicate = false;
        }

        /// <summary>
        /// Replaces the Awake function of the singleton and is called only when it is safe to do so (i.e, Not a duplicate.)
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Called when the Single is being shutdown (If you ever expect it to die. Like the player perhaps?)
        /// </summary>
        protected abstract void Shutdown();
    }
}