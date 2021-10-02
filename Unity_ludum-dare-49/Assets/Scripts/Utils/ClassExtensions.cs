using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Utils
{
    public static partial class ColorExtensions
    {
        public static Color WithAlpha(this Color color, float alpha, bool multiply = false)
        {
            Color newColor = color;
            if (multiply)
            {
                newColor.a *= alpha;
            }
            else
            {
                newColor.a = alpha;
            }
            return newColor;
        }

        // Note: Hue values are 0-1, not 0-360
        public static Color ShiftBy(this Color color, float addHue = 0f, float addSaturation = 0f, float addValue = 0f)
        {
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            return Color.HSVToRGB(h + addHue, Mathf.Clamp01(s + addSaturation), Mathf.Clamp01(v + addValue));
        }

        public static Color MultiplyBy(this Color color, float saturation = 1f, float value = 1f)
        {
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            return Color.HSVToRGB(h, Mathf.Clamp01(s * saturation), Mathf.Clamp01(v * value));
        }

        public static float GetValue(this Color color)
        {
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            return v;
        }

        public static float GetSaturation(this Color color)
        {
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            return s;
        }

        public static float GetNormalizedHue(this Color color)
        {
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            return h;
        }

        // Returns the hex version of a single component, without a leading hash (so can more easily be used to build a full hex color)
        // eg. ColorComponentToHex(1f) => "ff"
        public static string ColorComponentToHex(float linearColor)
        {
            byte shortColor = (byte) Mathf.Clamp(Mathf.RoundToInt(linearColor * (float)byte.MaxValue), 0, (int) byte.MaxValue);
            return shortColor.ToString("X2");
        }

        public static Color HexToColor(string hexColorWithHash)
        {
            if (ColorUtility.TryParseHtmlString(hexColorWithHash, out var color))
            {
                return color;
            }

            throw new System.ArgumentException(nameof(hexColorWithHash), $"Could not parse {hexColorWithHash} to Color");
        }

        public static string ColorToHex(this Color color)
        {
            return "#" + ColorUtility.ToHtmlStringRGBA(color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this Color color)
        {
            return new Vector4(color.r, color.g, color.b, color.a);
        }

        public static bool ApproximatelyEquals(this Color color, Color other, float tolerance = 0.001f)
        {
            return color.r.ApproximatelyEquals(other.r, tolerance)
                   && color.g.ApproximatelyEquals(other.g, tolerance)
                   && color.g.ApproximatelyEquals(other.g, tolerance)
                   && color.b.ApproximatelyEquals(other.b, tolerance);
        }
    }

    public static partial class VectorExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this float value)
        {
            return new Vector4(value, value, value, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this Vector2 vec, float z = 0, float w = 0)
        {
            return new Vector4(vec.x, vec.y, z, w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this Vector3 vec, float w = 0)
        {
            return new Vector4(vec.x, vec.y, vec.z, w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Vector2 vec, float z = 0)
        {
            return new Vector3(vec.x, vec.y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 PlanarToVector3(this Vector2 vec, float y = 0)
        {
            return new Vector3(vec.x, y, vec.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this float value)
        {
            return new Vector3(value, value, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this float value)
        {
            return new Vector2(value, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this Vector3 value)
        {
            return new Vector2(value.x, value.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int RoundToVector2Int(this Vector2 value)
        {
            return new Vector2Int(Mathf.RoundToInt(value.x), Mathf.RoundToInt(value.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int FloorToVector2Int(this Vector2 value)
        {
            return new Vector2Int(Mathf.FloorToInt(value.x), Mathf.FloorToInt(value.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int CeilToVector2Int(this Vector2 value)
        {
            return new Vector2Int(Mathf.CeilToInt(value.x), Mathf.CeilToInt(value.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithX(this Vector3 value, float x)
        {
            return new Vector3(x, value.y, value.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithY(this Vector3 value, float y)
        {
            return new Vector3(value.x, y, value.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithZ(this Vector3 value, float z)
        {
            return new Vector3(value.x, value.y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WithX(this Vector2 value, float x)
        {
            return new Vector2(x, value.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WithY(this Vector2 value, float y)
        {
            return new Vector2(value.x, y);
        }

        public static Vector2 Rotated(this Vector2 v, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinComponent(this Vector4 v)
        {
            return Mathf.Min(v.x, v.y, v.z, v.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinComponent(this Vector2 v)
        {
            return Mathf.Min(v.x, v.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxComponent(this Vector4 v)
        {
            return Mathf.Max(v.x, v.y, v.z, v.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxComponent(this Vector2 v)
        {
            return Mathf.Max(v.x, v.y);
        }

        public static Vector4 ClampedComponents(this Vector4 v, Vector4 min, Vector4 max)
        {
            return Vector4.Max(min, Vector4.Min(v, max));
        }

        public static Vector3 PlanarDirection(this Vector3 lhs, Vector3 rhs, bool normalize = true)
        {
            lhs.y = 0;
            rhs.y = 0;
            if (normalize) return (rhs - lhs).normalized;

            return (rhs - lhs);
        }

        public static Vector3 Direction(this Vector3 lhs, Vector3 rhs, bool normalize = true)
        {
            if (normalize) return (rhs - lhs).normalized;

            return (rhs - lhs);
        }

        public static float PlanarDistance(this Vector3 from, Vector3 to)
        {
            from.y = 0;
            to.y = 0;
            return Vector3.Distance(from, to);
        }

        /// <summary>
        /// More efficient version PlanarDistance where the exact value is not needed, skipping the
        /// expensive square root function. Useful for comparing against a known distance
        /// (_pos.PlanarDistanceSqr(targetPos) > _dist * _dist)
        /// </summary>
        public static float PlanarDistanceSqr(this Vector3 from, Vector3 to)
        {
            from.y = 0;
            to.y = 0;
            return Mathf.Abs(Vector3.SqrMagnitude(to - from));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceBetweenSq(this Vector3 from, Vector3 to)
        {
            return (to - from).sqrMagnitude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceBetween(this Vector3 from, Vector3 to)
        {
            return (to - from).magnitude;
        }

        public static Vector3 PlanarRandomDir(bool normalize = false)
        {
            Vector3 dir = UnityEngine.Random.insideUnitCircle;
            if (normalize) dir = UnityEngine.Random.onUnitSphere;
            dir.z = dir.y;
            dir.y = 0;
            return dir;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNaN(this Vector3 vec)
        {
            return float.IsNaN(vec.x) || float.IsNaN(vec.y) || float.IsNaN(vec.z);
        }

        public static Vector3 Abs(Vector3 vec)
        {
            vec.x = Mathf.Abs(vec.x);
            vec.y = Mathf.Abs(vec.y);
            vec.z = Mathf.Abs(vec.z);
            return vec;
        }

        public static bool Approximately(this Vector3 vec, Vector3 rhs, float threshold = float.Epsilon)
        {
            return FastApproximately(vec.x, rhs.x, threshold) &&
                   FastApproximately(vec.y, rhs.y, threshold) &&
                   FastApproximately(vec.z, rhs.z, threshold);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool FastApproximately(float a, float b, float threshold)
        {
            return ((a < b) ? (b - a) : (a - b)) <= threshold;
        }

        /// <summary>
        /// A Vector3 extension method that multiplies component-wise
        /// </summary>
        public static Vector3 CompMul(this Vector3 left, Vector3 right)
        {
            Vector3 result;
            result.x = left.x * right.x;
            result.y = left.y * right.y;
            result.z = left.z * right.z;
            return result;
        }
    }

    public static partial class MathUtils
    {
        public static float RemapRange(float fromStart, float fromEnd, float toStart, float toEnd, float value)
        {
            float divisor = (fromEnd - fromStart);
#if UNITY_EDITOR
            // Note: Stripping this from builds since have measured a very slight perf cost
            if (Mathf.Approximately(divisor, 0f))
            {
                Debug.LogWarning("Tried to remap a range where start and end are the same number");
                return toStart;
            }
#endif

            return Mathf.LerpUnclamped(toStart, toEnd, (value - fromStart) / divisor);
        }

        public static float RemapRangeClamped(float fromStart, float fromEnd, float toStart, float toEnd, float value)
        {
            float divisor = (fromEnd - fromStart);
#if UNITY_EDITOR
            // Note: Stripping this from builds since have measured a very slight perf cost on Switch
            if (Mathf.Approximately(divisor, 0f))
            {
                Debug.LogWarning("Tried to remap a range where start and end are the same number");
                return toStart;
            }
#endif

            // Lerp will clamp the ratio for us
            return Mathf.Lerp(toStart, toEnd, (value - fromStart) / divisor);
        }

        // Extension versions don't have Range in their name for disambiguation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Remap(this float value, float fromStart, float fromEnd, float toStart, float toEnd)
        {
            return RemapRange(fromStart, fromEnd, toStart, toEnd, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Remap(this float value, Vector2 from, Vector2 to)
        {
            return RemapRange(from.x, from.y, to.x, to.y, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RemapClamped(this float value, float fromStart, float fromEnd, float toStart, float toEnd)
        {
            return RemapRangeClamped(fromStart, fromEnd, toStart, toEnd, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RemapClamped(this float value, Vector2 from, Vector2 to)
        {
            return RemapRangeClamped(value, from.x, from.y, to.x, to.y);
        }

        public static Vector2 ClosestPointOnLine(Vector2 vA, Vector2 vB, Vector2 vPoint)
        {
            Vector2 vVector1 = vPoint - vA;
            Vector2 vVector2 = (vB - vA).normalized;

            float d = Vector2.Distance(vA, vB);
            float t = Vector2.Dot(vVector2, vVector1);

            if (t <= 0)
            {
                return vA;
            }

            if (t >= d)
            {
                return vB;
            }

            Vector2 vVector3 = vVector2 * t;
            Vector2 vClosestPoint = vA + vVector3;

            return vClosestPoint;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Abs(this Vector3 vector)
        {
            return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Abs(this Vector2 vector)
        {
            return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
        }

        // Note: This isn't as precise as Mathf.Approximately by default, since that will still return false even
        // when comparing 0 to like 10e8
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproximatelyEquals(this Vector2 a, Vector2 b, float tolerance = 0.0001f)
        {
            return Mathf.Abs(a.x - b.x) < tolerance && Mathf.Abs(a.y - b.y) < tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproximatelyEquals(this Vector3 a, Vector3 b, float tolerance = 0.0001f)
        {
            return Mathf.Abs(a.x - b.x) < tolerance && Mathf.Abs(a.y - b.y) < tolerance && Mathf.Abs(a.z - b.z) < tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproximatelyEquals(this float a, float b, float tolerance = 0.0001f)
        {
            return Mathf.Abs(a - b) < tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproximatelyEquals(this double a, double b, double tolerance = 0.0001f)
        {
            double delta = a - b;
            return delta < 0 ? delta > tolerance : delta < tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproximatelyEquals(this Vector4 a, Vector4 b, float tolerance = 0.0001f)
        {
            return Mathf.Abs(a.x - b.x) < tolerance && Mathf.Abs(a.y - b.y) < tolerance && Mathf.Abs(a.z - b.z) < tolerance && Mathf.Abs(a.w - b.w) < tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ClampedMagnitude(this Vector3 v, float maxMagnitude)
        {
            if (v.sqrMagnitude > (maxMagnitude * maxMagnitude))
            {
                return v * (maxMagnitude / v.magnitude);
            }

            return v;
        }

        // TODO: Rename to WithMaxMagnitude, then make a separate WithClampedMagnitude that takes a min and max
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ClampedMagnitude(this Vector2 v, float maxMagnitude)
        {
            if (v.sqrMagnitude > (maxMagnitude * maxMagnitude))
            {
                return v * (maxMagnitude / v.magnitude);
            }

            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WithMinMagnitude(this Vector2 v, float minMagnitude)
        {
            // Sqrt's are pretty cheap these days (only 2x the latency of a float multiply),
            // would need to do extra multiplies to even compare to sqrMagnitude,
            // and we need to use real magnitude in commonly hit branch
            var currentMagnitude = v.magnitude;
            if (currentMagnitude > 0.01f && currentMagnitude < minMagnitude)
            {
                return v * (minMagnitude / currentMagnitude);
            }

            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Squared(this float f)
        {
            return f * f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow(this float f, float exponent)
        {
            return Mathf.Pow(f, exponent);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxComponent(this Vector3 v)
        {
            return Mathf.Max(v.x, v.y, v.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinComponent(this Vector3 v)
        {
            return Mathf.Min(v.x, v.y, v.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAlmostZero(this Vector3 v)
        {
            // Same as CineMachine's util, but probs a cheaper way
            return v.sqrMagnitude < 0.001f;
        }
    }

    public static class UIExtensions
    {
        public static void SetAlpha(this Graphic graphic, float alpha)
        {
            graphic.color = graphic.color.WithAlpha(alpha);
        }

        // Setting the color property directly will always dirty the verts, causing a canvas rebuild
        // So can use this if setting eg. every frame but only want to dirty when actually changed
        public static void SetColorIfDifferent(this Graphic graphic, Color color)
        {
            if (!graphic.color.ApproximatelyEquals(color))
            {
                graphic.color = color;
            }
        }

        public static void SetAllColors(this ref ColorBlock block, Color color)
        {
            block.normalColor = color;
            block.highlightedColor = color;
            block.pressedColor = color;
            block.selectedColor = color;
            block.disabledColor = color;
        }

        // Note: This assumes that positive Y is up, like in UI canvases
        public static Vector2 ToVector2(this MoveDirection direction)
        {
            return direction switch
            {
                MoveDirection.Down => new Vector2(0, -1),
                MoveDirection.Up => new Vector2(0, 1),
                MoveDirection.Left => new Vector2(-1, 0),
                MoveDirection.Right => new Vector2(1, 0),
                MoveDirection.None => new Vector2(0, 0),
                _ => throw new ArgumentException("Invalid slide direction"),
            };
        }

        public static bool IsHorizontal(this MoveDirection direction)
        {
            return direction == MoveDirection.Left || direction == MoveDirection.Right;
        }

        public static bool IsVertical(this MoveDirection direction)
        {
            return direction == MoveDirection.Up || direction == MoveDirection.Down;
        }

        public static void SetAnchoredPositionX(this RectTransform rectTransform, float x)
        {
            rectTransform.anchoredPosition = rectTransform.anchoredPosition.WithX(x);
        }

        public static void SetAnchoredPositionY(this RectTransform rectTransform, float y)
        {
            rectTransform.anchoredPosition = rectTransform.anchoredPosition.WithY(y);
        }
    }

    public static class CollectionUtils
    {
        public static int ClampIndex(this int index, int listCount, bool wrap = false)
        {
            if (listCount <= 0)
            {
                Debug.LogWarning("Tried to clamp index for an empty list, which may be a mistake.");
                return 0;
            }

            if (!wrap)
            {
                return Mathf.Clamp(index, 0, listCount - 1);
            }

            while (index >= listCount) index -= listCount;
            while (index < 0) index += listCount;

            return index;
        }

        public static int ClampIndex<T>(this IList<T> list, int index, bool wrap = false)
        {
            return list == null ? 0 : index.ClampIndex(list.Count, wrap);
        }

        public static T GetClamped<T>(this IList<T> list, int index, bool wrap = false)
        {
            return (list == null || list.Count == 0) ? default(T) : list[index.ClampIndex(list.Count, wrap)];
        }

        public static int ClampIndex<T>(this int index, IList<T> list, bool wrap = false)
        {
            return list == null ? 0 : index.ClampIndex(list.Count, wrap);
        }

        public static T GetClamped<T>(this int index, IList<T> list, bool wrap = false)
        {
            return (list == null || list.Count == 0) ? default(T) : list[index.ClampIndex(list.Count, wrap)];
        }

        public static T GetClamped<T>(this T[] list, int index, bool wrap = false)
        {
            return (list == null || list.Length == 0) ? default(T) : list[index.ClampIndex(list.Length, wrap)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidIndex<T>(this IList<T> list, int index)
        {
            int count = list.Count;
            return count > 0 && index >= 0 && index < count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidIndex<T>(this T[] array, int index)
        {
            int count = array.Length;
            return count > 0 && index >= 0 && index < count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>(this T[] array, T item)
        {
            return Array.IndexOf(array, item) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetRandomIndex<T>(this T[] array)
        {
            if (array == null || array.Length <= 0)
            {
                return -1;
            }

            return UnityEngine.Random.Range(0, array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetRandomIndex<T>(this IList<T> list)
        {
            if (list == null || list.Count <= 0)
            {
                return -1;
            }

            return UnityEngine.Random.Range(0, list.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrDefault<T>(this IList<T> array, int index)
        {
            return array.IsValidIndex(index) ? array[index] : default;
        }

        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> range)
        {
            foreach (T obj in range)
            {
                queue.Enqueue(obj);
            }
        }

        // Util for lists to match HashSet<T>'s .Add(), returns true if the item was actually added
        // Note: If you don't need to index into the list, then HashSet<T> may still be a better choice,
        // but there are times where need to use a List<T> for certain reasons and want a compact way to a Contains/Add
        public static bool AddUnique<T>(this IList<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
                return true;
            }

            return false;
        }

        // Similar to List<T>'s ForEach, but works with any IEnumerable
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }

        public static void ForEach<T>(this IList<T> collection, Action<T, int> action)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                action(collection[i], i);
            }
        }

        public static List<T> Clone<T>(this IEnumerable<T> list)
        {
            return new List<T>(list);
        }

        public static T GetLastOrDefault<T>(this IList<T> list)
        {
            if (list.Count > 0)
            {
                return list[list.Count - 1];
            }

            return default;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var count = list.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }

        public static int CountIf<T>(this IEnumerable<T> list, in System.Func<T, bool> countIf)
        {
            int sum = 0;

            foreach (var item in list)
            {
                if (countIf(item))
                {
                    sum += 1;
                }
            }

            return sum;
        }

        public static bool HasAny<T>(this IEnumerable<T> list, in System.Func<T, bool> predicate)
        {
            foreach (var item in list)
            {
                if (predicate(item))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasAll<T>(this IEnumerable<T> list, in System.Func<T, bool> predicate)
        {
            foreach (var item in list)
            {
                if (!predicate(item))
                {
                    return false;
                }
            }

            return true;
        }

        // Some utils to make finding the closest thing to some other thing easier at the expense of some function calls
        // If something's really in the hot path then you'd probably want to manually loop and check stuff so the IL
        // can inline more things and you don't pay for some useless abs calls
        public static T GetClosest<T>(this IEnumerable<T> list, float targetValue, in System.Func<T, float> getValue)
        {
            float closestValue = float.MaxValue;
            bool hasSeenFirst = false;
            T closestItem = default;

            foreach (var item in list)
            {
                var dist = Mathf.Abs(getValue(item) - targetValue);
                if (dist < closestValue || !hasSeenFirst)
                {
                    hasSeenFirst = true;
                    closestValue = dist;
                    closestItem = item;
                }
            }

            return closestItem;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetClosest(this IEnumerable<Vector3> list, Vector3 position)
        {
            return GetClosest(list, position, v => v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetClosest(this IEnumerable<Vector2> list, Vector2 position)
        {
            return GetClosest(list, position, v => v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetClosest(this IEnumerable<float> list, float position)
        {
            return GetClosest(list, position, v => v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetClosest<T>(this IEnumerable<T> list, Vector3 position, System.Func<T, Vector3> getPosition)
        {
            return GetClosest(list, 0f, (T value) => (position - getPosition(value)).sqrMagnitude);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetClosest<T>(this IEnumerable<T> list, Vector2 position, System.Func<T, Vector2> getPosition)
        {
            return GetClosest(list, 0f, (T value) => (position - getPosition(value)).sqrMagnitude);
        }

        public static bool ContainsString(this List<string> list, string value, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            return list.Exists(v => v.Equals(value, comparisonType));
        }

        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
        {
            return dict.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public static bool Contains(this AnimatorControllerParameter[] parameters, string paramName)
        {
            return parameters.HasAny(p => p.name == paramName);
        }

        public static bool ShallowEquals<T>(this IList<T> first, IList<T> second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (first == null || second == null)
            {
                return false;
            }

            if (first.Count != second.Count)
            {
                return false;
            }

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < first.Count; i++)
            {
                if (!comparer.Equals(first[i], second[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }

}

public static class MonoBehaviourExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Nullify<T>(this T obj) where T : UnityEngine.Object
    {
        if (obj == null)
        {
            return null;
        }

        return obj;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DestroyIfValid(this UnityEngine.Object obj)
    {
        if (obj != null)
        {
            UnityEngine.Object.Destroy(obj);
        }
    }

    // Some extension methods to help avoid easy-to-miss bugs from destroying a component
    // when you really meant to destroy its game object etc.
    // All allow passing null (though may end up wanting that to only be swallowed in IfValid() versions?)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DestroyGameObject(this GameObject obj)
    {
        DestroyIfValid(obj);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DestroyGameObject(this Component comp)
    {
        if (comp != null)
        {
            Object.Destroy(comp.gameObject);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DestroyGameObject(this Transform transform)
    {
        if (transform != null)
        {
            Object.Destroy(transform.gameObject);
        }
    }

    public static void ForEachComponentInChildren<T>(this Component component, System.Action<T> callback)
    {
        ForEachComponentInChildren(component.transform, callback);
    }

    public static void ForEachComponentInChildren<T>(this GameObject component, System.Action<T> callback)
    {
        ForEachComponentInChildren(component.transform, callback);
    }

    public static void ForEachComponentInChildren<T>(this Transform transform, System.Action<T> callback)
    {
        var children = transform.GetComponentsInChildren<T>();
        foreach (var child in children)
        {
            callback.Invoke(child);
        }
    }

    public static void ForEachComponentInChildren<T>(this Component component, System.Action<T, int> callback)
    {
        ForEachComponentInChildren(component.transform, callback);
    }

    public static void ForEachComponentInChildren<T>(this GameObject component, System.Action<T, int> callback)
    {
        ForEachComponentInChildren(component.transform, callback);
    }

    public static void ForEachComponentInChildren<T>(this Transform transform, System.Action<T, int> callback)
    {
        var children = transform.GetComponentsInChildren<T>();
        for (int i = 0; i < children.Length; i++)
        {
            callback.Invoke(children[i], i);
        }
    }

    public static T GetComponentInParentCached<T>(this Component component, ref T cachedValue, bool includeInactive = true)
    {
        if (cachedValue == null)
        {
            cachedValue = component.gameObject.GetComponentInParent<T>(includeInactive);
        }

        return cachedValue;
    }

    public static C GetComponentInParentSkipSelf<C>(this Transform t, int maxStepsUpHierarchy = 10) where C : Component
    {
        t = t.parent;
        while (t && maxStepsUpHierarchy > 0)
        {
            C c = t.GetComponent<C>();
            if (c)
                return c;
            t = t.parent;
            maxStepsUpHierarchy--;
        }
        return null;
    }

    public static C GetComponentInParentHierarchy<C>(this Transform t, int maxStepsUpHierarchy = 10) where C : Component
    {
        while (t && maxStepsUpHierarchy > 0)
        {
            C c = t.GetComponent<C>();
            if (c)
                return c;
            t = t.parent;
            maxStepsUpHierarchy--;
        }
        return null;
    }

    public static T GetComponentInChildrenCached<T>(this Component component, ref T cachedValue, bool includeInactive = true)
    {
        if (cachedValue == null)
        {
            cachedValue = component.GetComponentInChildren<T>(includeInactive);
        }

        return cachedValue;
    }

    public static T[] GetComponentsInChildrenCached<T>(this Component component, ref T[] cachedValue, bool includeInactive = true)
    {
        if (cachedValue == null)
        {
            cachedValue = component.GetComponentsInChildren<T>(includeInactive);
        }

        return cachedValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetComponentCached<T>(this Component component, ref T cachedValue) where T : Component
    {
        if (cachedValue == null)
        {
            cachedValue = component.GetComponent<T>();
        }

        return cachedValue;
    }

    public static GameObject InstantiatePrefab(GameObject prefab, Transform parent, bool worldPositionStays = true)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return UnityEditor.PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
        }
#endif

        return Object.Instantiate(prefab, parent, worldPositionStays);
    }

    public static void DestroyChild(this Component comp, GameObject childObject, bool destroyImmediate)
    {
        if (destroyImmediate)
        {
            Object.DestroyImmediate(childObject);
        }
        else
        {
            Object.Destroy(childObject);
        }
    }

    public static void DestroyChildren(this Component comp)
    {
        DestroyChildren(comp, destroyImmediate: !Application.isPlaying);
    }

    public static void DestroyChildren(this GameObject gameObject)
    {
        DestroyChildren(gameObject, destroyImmediate: !Application.isPlaying);
    }

    public static void DestroyChildren(this Transform transform)
    {
        DestroyChildren(transform, destroyImmediate: !Application.isPlaying);
    }

    public static void DestroyChildren(this Component comp, bool destroyImmediate)
    {
        DestroyChildren(comp.transform, destroyImmediate);
    }

    public static void DestroyChildren(this GameObject gameObject, bool destroyImmediate)
    {
        DestroyChildren(gameObject.transform, destroyImmediate);
    }

    public static void DestroyChildren(this Transform transform, bool destroyImmediate, bool detach = false)
    {
        int childCount = transform.childCount;
        if (destroyImmediate)
        {
            for (int i = childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate( transform.GetChild(i).gameObject );
            }
        }
        else
        {
            for (int i = childCount - 1; i >= 0; i--)
            {
                Object.Destroy(transform.GetChild(i).gameObject);

                if (detach)
                {
                    transform.GetChild(i).SetParent(null);
                }
            }
        }
    }

    public delegate void InitChildDelegate<T>(T listItem, GameObject spawnedObject, int index);

    public static void InstantiateChildForEachIfNeeded<T>(this IList<T> list, GameObject prefab, Transform parent, InitChildDelegate<T> initChild, bool worldPositionStays = false)
    {
        var numExistingChildren = parent.childCount;

        for (int i = 0; i < list.Count; i++)
        {
            GameObject spawned;
            if (i < numExistingChildren)
            {
                spawned = parent.GetChild(i).gameObject;
            }
            else
            {
                spawned = Object.Instantiate(prefab, parent, worldPositionStays);
            }

            initChild?.Invoke(list[i], spawned, i);
        }

        // Clear out any extra items leftover from design time
        for (int i = list.Count; i < numExistingChildren; i++)
        {
            parent.GetChild(i).DestroyGameObject();
        }
    }

    public static void SetChildrenActive(this Transform transform, bool isActive)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(isActive);
        }
    }

    public static void SetChildrenActive(this GameObject obj, bool isActive)
    {
        SetChildrenActive(obj.transform, isActive);
    }

    public static void SetChildrenActive(this Component component, bool isActive)
    {
        SetChildrenActive(component.transform, isActive);
    }
}

public static class StringExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NullifyIfEmpty(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }

        return str;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NullifyIfEmptyOrWhiteSpace(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return null;
        }

        return str;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrWhiteSpace(this string str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    // TODO: Might be nice to add an IDebugStringProvider thing if objects wanted to provide extra info
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToDebugString(this Object obj)
    {
        if (obj == null)
        {
            return "null";
        }

        return obj.ToString();
    }

    public static string TrimStart(this string target, string trimString)
    {
        if (string.IsNullOrEmpty(trimString) || string.IsNullOrEmpty(target))
        {
            return target;
        }

        string result = target;
        while (result.StartsWith(trimString))
        {
            result = result.Substring(trimString.Length);
        }

        return result;
    }

    public static string TrimEnd(this string target, string trimString)
    {
        if (string.IsNullOrEmpty(trimString) || string.IsNullOrEmpty(target))
        {
            return target;
        }

        string result = target;
        while (result.EndsWith(trimString))
        {
            result = result.Substring(0, result.Length - trimString.Length);
        }

        return result;
    }

    public static string FormatSeconds(float seconds, float? monospacedSecondsEm = 0.6f)
    {
        return FormatSeconds(Mathf.CeilToInt(seconds), monospacedSecondsEm);
    }

    public static string FormatSeconds(int seconds, float? monospacedSecondsEm = 0.6f)
    {
        // Note: Since we only display to second-precision, round up to avoid
        // weird staggers if multiple things are counting with a slight offset
        var ts = TimeSpan.FromSeconds(seconds);
        return FormatTimeSpan(ts, monospacedSecondsEm);
    }

    // Formats a timespan into a string in the most commonly needed way for UI
    // Will always show minutes and second, and only hours if needed.
    // Seconds has leading zeros but hours/minutes don't
    // Note: This won't handle anything greater than 24 hours
    public static string FormatTimeSpan(TimeSpan ts, float? monospacedSecondsEm = 0.6f)
    {
        // TODO: Need to double check if .Minutes goes above 60
        var text = monospacedSecondsEm == null ? $"{ts.Minutes}:{ts.Seconds:D2}" : $"{ts.Minutes}:<mspace={monospacedSecondsEm.Value}em>{ts.Seconds:D2}";
        if (ts.Hours > 0)
        {
            text = $"{ts.Hours}:{text}";
        }

        return text;
    }

    // Takes a "normalized percent" (0-1) and stringifies it to one decimal place,
    // rounding away from 100% unless it was within 0.1%
    public static string FormatPercent(float normalizedPercent, int decimalPlaces = 1)
    {
        normalizedPercent *= 100;
        if (normalizedPercent >= 99.99f)
        {
            return "100%";
        }

        // Show a single decimal place, rounding down so that 99.95% doesn't show as 100%
        var flooredValue = RoundDown(Convert.ToDecimal(normalizedPercent), decimalPlaces: decimalPlaces);
        return $"{flooredValue:0.0}%";
    }

    private static decimal RoundDown(decimal i, double decimalPlaces)
    {
        var power = Convert.ToDecimal(Math.Pow(10, decimalPlaces));
        return Math.Floor(i * power) / power;
    }

}

public static class GameObjectExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MatchesLayerMask(this GameObject gameObject, int layerMask)
    {
        return (gameObject.GetLayerAsMask() & layerMask) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLayerAsMask(this GameObject gameObject)
    {
        return 1 << gameObject.layer;
    }

    public static void SetLayerRecursive(this GameObject gameObject, int layer)
    {
        gameObject.layer = layer;

        foreach (Transform child in gameObject.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }
}

public static class AnimationExtensions
{
    public static float GetDuration(this AnimationCurve curve)
    {
        int keyCount = curve.length;
        if (keyCount > 0)
        {
            return curve[keyCount - 1].time;
        }

        return 0f;
    }
}