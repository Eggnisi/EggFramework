#region

//文件创建者：Egg
//创建时间：07-28 04:21

#endregion

using System;
using UnityEngine;

namespace EggFramework.Procedure
{
    ///<summary>Auto "Convenience Converters" from type to type (boxing). This includes unconventional data conversions like for example GameObject to Vector3 (by transform.position).</summary>
    public static class TypeConverter
    {
        ///<summary>Subscribe custom converter</summary>
        private static Func<Type, Type, Func<object, object>> _customConverter;

        public static void RegisterCustomConverter(Func<Type, Type, Func<object, object>> converter)
        {
            _customConverter += converter;
        }

        ///<summary>Returns a function that can convert provided first arg value from type to type</summary>
        public static Func<object, object> Get(Type fromType, Type toType)
        {
            // Custom Converter
            var converter = _customConverter?.Invoke(fromType, toType);
            if (converter != null)
            {
                return converter;
            }

            // Normal assignment.
            if (toType.IsAssignableFrom(fromType))
            {
                return (value) => value;
            }

            // Anything to string
            if (toType == typeof(string))
            {
                return (value) => value != null ? value.ToString() : "NULL";
            }

            // Convertible to convertible.
            if (typeof(IConvertible).IsAssignableFrom(toType) && typeof(IConvertible).IsAssignableFrom(fromType))
            {
                return (value) =>
                {
                    try
                    {
                        return Convert.ChangeType(value, toType);
                    }
                    catch
                    {
                        return !toType.IsAbstract ? Activator.CreateInstance(toType) : null;
                    }
                };
            }

            // Unity Object to bool.
            if (typeof(UnityEngine.Object).IsAssignableFrom(fromType) && toType == typeof(bool))
            {
                return (value) => value != null;
            }

            // GameObject to Component.
            if (fromType == typeof(GameObject) && typeof(Component).IsAssignableFrom(toType))
            {
                return (value) => value as GameObject != null ? ((GameObject)value).GetComponent(toType) : null;
            }

            // Component to GameObject.
            if (typeof(Component).IsAssignableFrom(fromType) && toType == typeof(GameObject))
            {
                return (value) => value as Component != null ? ((Component)value).gameObject : null;
            }

            // Component to Component.
            if (typeof(Component).IsAssignableFrom(fromType) && typeof(Component).IsAssignableFrom(toType))
            {
                return (value) =>
                    value as Component != null ? ((Component)value).gameObject.GetComponent(toType) : null;
            }

            // GameObject to Interface
            if (fromType == typeof(GameObject) && toType.IsInterface)
            {
                return (value) => value as GameObject != null ? ((GameObject)value).GetComponent(toType) : null;
            }

            // Component to Interface
            if (typeof(Component).IsAssignableFrom(fromType) && toType.IsInterface)
            {
                return (value) =>
                    value as Component != null ? ((Component)value).gameObject.GetComponent(toType) : null;
            }

            // GameObject to Vector3 (position).
            if (fromType == typeof(GameObject) && toType == typeof(Vector3))
            {
                return value => (value as GameObject) ? ((GameObject)value).transform.position : Vector3.zero;
            }

            // Component to Vector3 (position).
            if (typeof(Component).IsAssignableFrom(fromType) && toType == typeof(Vector3))
            {
                return value => (value as Component) ? ((Component)value).transform.position : Vector3.zero;
            }

            // GameObject to Quaternion (rotation).
            if (fromType == typeof(GameObject) && toType == typeof(Quaternion))
            {
                return (value) => (value as GameObject)
                    ? ((GameObject)value).transform.rotation
                    : Quaternion.identity;
            }

            // Component to Quaternion (rotation).
            if (typeof(Component).IsAssignableFrom(fromType) && toType == typeof(Quaternion))
            {
                return (value) => (value as Component)
                    ? ((Component)value).transform.rotation
                    : Quaternion.identity;
            }

            // Quaternion to Vector3 (Euler angles).
            if (fromType == typeof(Quaternion) && toType == typeof(Vector3))
            {
                return value => ((Quaternion)value).eulerAngles;
            }

            // Vector3 (Euler angles) to Quaternion.
            if (fromType == typeof(Vector3) && toType == typeof(Quaternion))
            {
                return value => Quaternion.Euler((Vector3)value);
            }

            // Vector2 to Vector3.
            if (fromType == typeof(Vector2) && toType == typeof(Vector3))
            {
                return value => (Vector3)(Vector2)value;
            }

            // Vector3 to Vector2.
            if (fromType == typeof(Vector3) && toType == typeof(Vector2))
            {
                return value => (Vector2)(Vector3)value;
            }

            return null;
        }

        ///<summary>Returns whether a conversion exists</summary>
        public static bool CanConvert(Type fromType, Type toType) => Get(fromType, toType) != null;
    }
}