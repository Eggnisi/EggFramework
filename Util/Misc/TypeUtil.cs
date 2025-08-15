#region

//文件创建者：Egg
//创建时间：09-19 10:26

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using EggFramework.Util.Res;
using QFramework;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace EggFramework.Util
{
    public static class TypeUtil
    {
#if UNITY_EDITOR
        public static IEnumerable<string> ResTypes
        {
            get
            {
                var types = GetDerivedClassesFromGenericClass(typeof(ResRefData<>));
                return types.Select(type => type!.BaseType!.GenericTypeArguments[0].Name);
            }
        }

        public static Type GetResTypeByTypeName(string name)
        {
            var types = GetDerivedClassesFromGenericClass(typeof(ResRefData<>))
                .Select(type => type!.BaseType!.GenericTypeArguments[0]);
            return types.FirstOrDefault(tp => tp.Name == name);
        }
#endif

        private static IArchitecture _architectureInst;

        public static IArchitecture GetFirstActiveArchitecture()
        {
            if (_architectureInst != null)
            {
                return _architectureInst;
            }

            var archTypes = GetDerivedClassesFromGenericClass(typeof(Architecture<>));
            foreach (var archType in archTypes.Where(archType => archType.Name != "DefaultApp"))
            {
                _architectureInst =
                    (IArchitecture)archType!.BaseType!.GetProperty("Interface")!.GetGetMethod().Invoke(null, null);
                break;
            }

            return _architectureInst;
        }

        public static IEnumerable<string> UnityStructTypes =>
            new[] { "Vector2", "Vector3", "Color" };

        public static IEnumerable<string> DefaultTypes =>
            new[] { "Single", "Int32", "String", "Boolean", "Double" };

        public static IEnumerable<PropertyInfo> GetSerializePropertyInfos(this Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public static IEnumerable<FieldInfo> GetSerializeFieldInfos(this Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        }

        public static List<Assembly> CurrentAssemblies
        {
            get
            {
                if (!_loaded)
                {
                    LoadAssemblies();
                }

                return _currentAssemblies;
            }
        }

        private static          Type[]                   _allTypes;
        private static readonly Dictionary<string, Type> _typesMap = new();

        private static void LoadAssemblies()
        {
            _loaded            = true;
            _currentAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        }

        private static bool _loaded;

        private static List<Assembly> _currentAssemblies;

        public static List<Type> GetDerivedClassesFromGenericInterfaces(Type interfaceType)
        {
            var ret = new List<Type>();
            foreach (var currentAssembly in CurrentAssemblies)
            {
                var types = currentAssembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsClass && type.GetInterfaces().Any(i =>
                            i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType))
                    {
                        ret.Add(type);
                    }
                }
            }

            return ret;
        }

        public static List<Type> GetDerivedClassesFromGenericClass(Type targetType)
        {
            var ret = new List<Type>();
            foreach (var assembly in CurrentAssemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsClass && !type.IsAbstract &&
                        type.BaseType is { IsGenericType: true } &&
                        targetType.IsAssignableFrom(type.BaseType.GetGenericTypeDefinition()))
                    {
                        ret.Add(type);
                    }
                }
            }

            return ret;
        }

        public static List<Type> GetDerivedClasses(Type targetType)
        {
            var ret = new List<Type>();
            foreach (var assembly in CurrentAssemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsClass && !type.IsAbstract && targetType.IsAssignableFrom(type))
                    {
                        ret.Add(type);
                    }
                }
            }

            return ret;
        }

        public static List<Type> GetDerivedInterfaces(Type targetType)
        {
            var ret = new List<Type>();
            foreach (var assembly in CurrentAssemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsInterface && !type.IsAbstract && targetType.IsAssignableFrom(type))
                    {
                        ret.Add(type);
                    }
                }
            }

            return ret;
        }

        public static List<Type> GetDerivedStructs(Type targetType)
        {
            var ret = new List<Type>();
            foreach (var assembly in CurrentAssemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsValueType && !type.IsAbstract && targetType.IsAssignableFrom(type))
                    {
                        ret.Add(type);
                    }
                }
            }

            return ret;
        }

        public static string GetLabelTextTag(Type type)
        {
            var customAttributes = type.GetCustomAttributes(typeof(LabelTextAttribute), false);
            if (customAttributes.Length <= 0)
            {
                return type.Name;
            }

            return ((LabelTextAttribute)customAttributes[0]).Text;
        }

        public static T GetAttribute<T>(this Type type, bool inherit = false) where T : Attribute
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(T), inherit);
            return customAttributes.Length == 0 ? default : (T)customAttributes[0];
        }

        public static IEnumerable<string> GetEnumNames(Type type)
        {
            var list = new List<string>();
            foreach (var value in Enum.GetValues(type))
            {
                list.Add(value.ToString());
            }

            return list;
        }

        public static IList CastListElements(IList list, Type targetType)
        {
            var ret = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(targetType));

            foreach (var o in list)
            {
                ret.Add(o);
            }

            return ret;
        }

        public static object InvokeMemberFunc(this object obj, string memberName, params object[] args)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            try
            {
                return obj.GetType().InvokeMember(
                    name: memberName,
                    invokeAttr: BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                    binder: null,
                    target: obj,
                    args: args
                );
            }
            catch (MissingMethodException ex)
            {
                throw new MissingMethodException($"Method '{memberName}' with specified arguments not found", ex);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        public static object GetMemberValue(this object obj, string memberName)
        {
            var type        = obj.GetType();
            var memberInfos = type.GetMember(memberName, BindingFlags.Public | BindingFlags.Instance);
            if (memberInfos is { Length: > 0 })
            {
                var memberInfo = memberInfos[0];
                if (memberInfo is FieldInfo fieldInfo)
                {
                    return fieldInfo.GetValue(obj);
                }

                if (memberInfo is PropertyInfo propertyInfo)
                {
                    return propertyInfo.GetValue(obj);
                }
            }

            return null;
        }

        public static Type ReflectedOrDeclaredType(this MemberInfo member) =>
            member.ReflectedType ?? member.DeclaringType;

        public static bool IsReadOnly(this FieldInfo field) => field.IsInitOnly || field.IsLiteral;

        ///<summary>Is the field a Constant?</summary>
        public static bool IsConstant(this FieldInfo field) => field.IsReadOnly() && field.IsStatic;

        //Alternative to Type.GetType to work with FullName instead of AssemblyQualifiedName when looking up a type by string
        //This also handles Generics and their arguments, assembly changes and namespace changes to some extend.
        public static Type GetType(string typeFullName)
        {
            return GetType(typeFullName, false, null);
        }

        public static Type GetType(string typeFullName, Type fallbackAssignable)
        {
            return GetType(typeFullName, true, fallbackAssignable);
        }

        public static Type GetType(string typeFullName, bool fallbackNoNamespace = false,
            Type fallbackAssignable = null)
        {
            if (string.IsNullOrEmpty(typeFullName))
            {
                return null;
            }

            Type type = null;
            if (_typesMap.TryGetValue(typeFullName, out type))
            {
                return type;
            }

            //direct look up
            type = GetTypeDirect(typeFullName);
            if (type != null)
            {
                return _typesMap[typeFullName] = type;
            }

            //handle generics now
            type = TryResolveGenericType(typeFullName, fallbackNoNamespace, fallbackAssignable);
            if (type != null)
            {
                // Logger.LogWarning(string.Format("Type with name '{0}' was resolved using a fallback resolution (Generics).", typeFullName), "Type Request");
                return _typesMap[typeFullName] = type;
            }

            //get type regardless namespace
            if (fallbackNoNamespace)
            {
                type = TryResolveWithoutNamespace(typeFullName, fallbackAssignable);
                if (type != null)
                {
                    // Logger.LogWarning(string.Format("Type with name '{0}' was resolved using a fallback resolution (NoNamespace).", typeFullName), "Type Request");
                    return _typesMap[typeFullName] = type;
                }
            }

            Debug.LogError($"Type with name '{typeFullName}' could not be resolved.");
            return _typesMap[typeFullName] = null;
        }

        //direct type look up with it's FullName
        static Type GetTypeDirect(string typeFullName)
        {
            var type = Type.GetType(typeFullName);
            if (type != null)
            {
                return type;
            }

            foreach (var asm in CurrentAssemblies)
            {
                try
                {
                    type = asm.GetType(typeFullName);
                }
                catch
                {
                    continue;
                }

                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        //Resolve generic types by their .FullName or .ToString
        //Remark: a generic's type .FullName returns a string where it's arguments only are instead printed as AssemblyQualifiedName.
        private static Type TryResolveGenericType(string typeFullName, bool fallbackNoNamespace = false,
            Type fallbackAssignable = null)
        {
            //ensure that it is a generic type implementation, not a definition
            if (typeFullName.Contains('`') == false || typeFullName.Contains('[') == false)
            {
                return null;
            }

            try //big try/catch block cause maybe there is a bug. Hopefully not.
            {
                var quoteIndex         = typeFullName.IndexOf('`');
                var genericTypeDefName = typeFullName.Substring(0, quoteIndex + 2);
                var genericTypeDef     = GetType(genericTypeDefName, fallbackNoNamespace, fallbackAssignable);
                if (genericTypeDef == null)
                {
                    return null;
                }

                int      argCount = Convert.ToInt32(typeFullName.Substring(quoteIndex + 1, 1));
                var      content  = typeFullName.Substring(quoteIndex + 2, typeFullName.Length - quoteIndex - 2);
                string[] split;
                if (content.StartsWith("[["))
                {
                    //this means that assembly qualified name is contained. Name was generated with FullName.
                    var startIndex = typeFullName.IndexOf("[[", StringComparison.Ordinal) + 2;
                    var endIndex   = typeFullName.LastIndexOf("]]", StringComparison.Ordinal);
                    content = typeFullName.Substring(startIndex, endIndex - startIndex);
                    split   = content.Split(new[] { "],[" }, argCount, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    //this means that the name was generated with type.ToString().
                    var startIndex = typeFullName.IndexOf('[') + 1;
                    var endIndex   = typeFullName.LastIndexOf(']');
                    content = typeFullName.Substring(startIndex, endIndex - startIndex);
                    split   = content.Split(new[] { ',' }, argCount, StringSplitOptions.RemoveEmptyEntries);
                }

                var argTypes = new Type[argCount];
                for (var i = 0; i < split.Length; i++)
                {
                    var subName = split[i];
                    if (!subName.Contains('`') && subName.Contains(','))
                    {
                        //remove assembly info since we work with FullName, but only if it's not yet another generic.
                        subName = subName[..subName.IndexOf(',')];
                    }

                    var argType = GetType(subName, true /*fallback no namespace*/);
                    if (argType == null)
                    {
                        return null;
                    }

                    argTypes[i] = argType;
                }

                return genericTypeDef.MakeGenericType(argTypes);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to resolve generic type '{typeFullName}': {e.Message}");
                return null;
            }
        }


        //fallback type look up with it's FullName. This is slow.
        private static Type TryResolveWithoutNamespace(string typeName, Type fallbackAssignable = null)
        {
            //dont handle generic implementations this way (still handles definitions though).
            if (typeName.Contains('`') && typeName.Contains('['))
            {
                return null;
            }

            //remove assembly info if any
            if (typeName.Contains(','))
            {
                typeName = typeName.Substring(0, typeName.IndexOf(','));
            }

            //ensure strip namespace
            if (typeName.Contains('.'))
            {
                var dotIndex = typeName.LastIndexOf('.') + 1;
                typeName = typeName.Substring(dotIndex, typeName.Length - dotIndex);
            }

            //check all types
            var allTypes = GetAllTypes(true);
            return allTypes.FirstOrDefault(t => t.Name == typeName && (fallbackAssignable == null || fallbackAssignable.IsAssignableFrom(t)));
        }

        public static Type[] GetAllTypes(bool includeObsolete)
        {
            if (_allTypes != null)
            {
                return _allTypes;
            }

            var result = new List<Type>();
            foreach (var asm in CurrentAssemblies)
            {
                try
                {
                    result.AddRange(asm.GetExportedTypes().Where(t =>
                        includeObsolete || !t.IsDefined<ObsoleteAttribute>(false)));
                }
                catch
                {
                    // ignored
                }
            }

            return _allTypes = result.OrderBy(t => t.Namespace).ThenBy(t => t.Name).ToArray();
        }
        
        public static FieldInfo GetField(this Type type, string name, bool includePrivateBase = false)
        {
            var current = type;
            while (current != null)
            {
                var fields = current.GetFields();
                foreach (var f in fields)
                {
                    if (f.Name == name)
                    {
                        return f;
                    }
                }

                if (!includePrivateBase)
                {
                    break;
                }

                current = current.BaseType;
            }

            Debug.LogError(
                $"Field with name '{name}' on type '{type.Name}', could not be resolved.");
            return null;
        }

        public static PropertyInfo GetProperty(this Type type, string name)
        {
            var props = type.GetProperties();
            foreach (var p in props)
            {
                if (p.Name == name) return p;
            }

            Debug.LogError(
                $"Property with name '{name}' on type '{type.Name}', could not be resolved.");
            return null;
        }

        ///<summary>returns either field or property member info </summary>
        public static MemberInfo GetFieldOrProp(this Type type, string name)
        {
            var fields = type.GetFields();
            foreach (var f in fields)
            {
                if (f.Name == name) return f;
            }

            var props = type.GetProperties();
            foreach (var p in props)
            {
                if (p.Name == name) return p;
            }

            Debug.LogError(
                $"Field Or Property with name '{name}' on type '{type.Name}', could not be resolved.");
            return null;
        }
        
        public static Func<T, TResult> GetFieldGetter<T, TResult>(FieldInfo info)
        {
            return instance => (TResult)info.GetValue(instance);
        }
        
        public static Type GetEnumerableElementType(this Type type)
        {
            if (type == null)
            {
                return null;
            }

            if (!typeof(IEnumerable).IsAssignableFrom(type))
            {
                return null;
            }

            if (type.HasElementType || type.IsArray)
            {
                return type.GetElementType();
            }

            if (type.IsGenericType)
            {
                //These are not exactly correct, but serve the purpose of usage.
                var args = type.GetGenericArguments();
                if (args.Length == 1)
                {
                    return args[0];
                }

                //This is special. We only support Dictionary<string, T> and always consider 1st arg to be string.
                if (typeof(IDictionary).IsAssignableFrom(type) && args.Length == 2)
                {
                    return args[1];
                }
            }
            
            return null;
        }
        
        public static Array Resize(this Array array, int newSize)
        {
            if (array == null)
            {
                return null;
            }

            var oldSize        = array.Length;
            var elementType    = array.GetType().GetElementType();
            var newArray       = Array.CreateInstance(elementType, newSize);
            var preserveLength = Math.Min(oldSize, newSize);
            if (preserveLength > 0)
            {
                Array.Copy(array, newArray, preserveLength);
            }

            return newArray;
        }
        
        public static object CreateObjectUninitialized(this Type type)
        {
            if (type == null) return null;
            return FormatterServices.GetUninitializedObject(type);
        }
    }
}