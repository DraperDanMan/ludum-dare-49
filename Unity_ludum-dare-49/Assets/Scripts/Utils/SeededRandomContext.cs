using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utils
{
    public class SeededRandomContext
    {
        private readonly int _seed;
        private Random.State _internalState;

        private Random.State _tempState;
        public SeededRandomContext(int seed)
        {
            _seed = seed;
            Reset();
        }

        public SeededRandomContext(string seed)
        {
            _seed = CalculateSeedFromString(seed);
            Reset();
        }


        // ReSharper disable once MemberCanBePrivate.Global
        public void Reset()
        {
            //cache the state in case we're used inside another seeded context
            var outOfScopeState = Random.state;
            Random.InitState(_seed);
            _internalState = Random.state;
            Random.state = outOfScopeState;
        }

        private void PushState()
        {
            //cache the state in case we're used inside another seeded context
            _tempState = Random.state;
            Random.state = _internalState;
        }

        private void PopState()
        {
            _internalState = Random.state;
            Random.state = _tempState;
        }

        public float Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                PushState();
                var value = Random.value; //Get the next seeded value
                PopState();
                return value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Range(float min, float max)
        {
            PushState();
            var value = Random.Range(min, max); //passthrough to random underneath
            PopState();
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Range(int min, int max)
        {
            PushState();
            var value = Random.Range(min, max); //passthrough to random underneath
            PopState();
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 OnUnitSphere()
        {
            PushState();
            var value = Random.onUnitSphere; //passthrough to random underneath
            PopState();
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 InsideUnitSphere()
        {
            PushState();
            var value = Random.insideUnitSphere; //passthrough to random underneath
            PopState();
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 InsideUnitCircle()
        {
            PushState();
            var value = Random.insideUnitCircle; //passthrough to random underneath
            PopState();
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Quaternion Rotation()
        {
            PushState();
            var value = Random.rotation; //passthrough to random underneath
            PopState();
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Quaternion RotationUniform()
        {
            PushState();
            var value = Random.rotationUniform; //passthrough to random underneath
            PopState();
            return value;
        }

        private int CalculateSeedFromString(string stringSeed)
        {
            MD5 md5Hasher = MD5.Create();
            var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(stringSeed));
            //NOTE dan: This is converting from 128 bit hash into a 32bit int, Collisions are possible,
            //However, as this is a random seed and this is MD5 the collisions will be reliable and should still produce the same results.
            return BitConverter.ToInt32(hashed, 0);
        }

        public IDisposable SeededScope()
        {
            return new ScopedSeed(this);
        }

        private class ScopedSeed : IDisposable
        {
            private readonly SeededRandomContext _context;
            private readonly Random.State _previousState;
            private bool _disposed;

            public ScopedSeed(SeededRandomContext seededRandomContext)
            {
                _context = seededRandomContext;
                _previousState = Random.state;//Don't use push pop here to abuse the stack holding these values while in nested usings
                Random.state = _context._internalState;
            }

            public void Dispose()
            {
                if (_disposed) return; //Safety, somethings call dispose when the object isn't becoming null
                _context._internalState = Random.state;
                Random.state = _previousState;
                _disposed = true;
            }
        }
    }
}