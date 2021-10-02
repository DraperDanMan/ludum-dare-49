using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Utils
{
    public class FloatEffectorStack
    {
        private readonly Dictionary<Object, float> _effectors;
        private readonly Func<float, float, float> _compareFunction;
        private readonly float _defaultValue = 0;

        public Action<float> Changed;
        public float Value => LookupValue();

        public FloatEffectorStack(Func<float, float, float> compFunc, float defaultValue = 0)
        {
            _compareFunction = compFunc;
            _defaultValue = defaultValue;
            _effectors = new Dictionary<Object, float>();
        }

        private float LookupValue()
        {
            float result = _defaultValue;
            foreach (var val in _effectors.Values)
                result = _compareFunction(result, val);

            return result;
        }

        public void Push(Object owner, float value)
        {
            if (!_effectors.ContainsKey(owner))
            {
                _effectors.Add(owner, value);
                Changed?.Invoke(LookupValue());
            }
        }

        public void Update(Object owner, float value)
        {
            if (_effectors.ContainsKey(owner))
            {
                _effectors[owner] = value;
                Changed?.Invoke(LookupValue());
            }
        }

        public void Pop(Object owner)
        {
            if (_effectors.ContainsKey(owner))
            {
                _effectors.Remove(owner);
                Changed?.Invoke(LookupValue());
            }
            //Fail Silently if you try to pop and it doesn't exist.
        }

        public void Clear()
        {
            _effectors.Clear();
            Changed?.Invoke(LookupValue());
        }

        public static float RoughAverage(float left, float right)
        {
            return (left + right) * 0.5f;
        }
    }
}