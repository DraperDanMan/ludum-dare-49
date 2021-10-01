using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace Utils
{
    public readonly struct ShaderId
    {
        public readonly int Id;
        public readonly string PropertyName;

        private ShaderId(int id, string propertyName = null)
        {
            Id = id;
            PropertyName = propertyName;
        }

        public override string ToString()
        {
            return PropertyName == null ? Id.ToString() : $"{Id} \"{PropertyName}\"";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(ShaderId id) { return id.Id; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ShaderId(string paramName) { return new ShaderId(Shader.PropertyToID(paramName), paramName); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ShaderId(int paramId) { return new ShaderId(paramId); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator RenderTargetIdentifier(ShaderId id) { return new RenderTargetIdentifier(id); }
    }
}