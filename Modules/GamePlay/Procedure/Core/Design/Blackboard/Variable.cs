#region

//文件创建者：Egg
//创建时间：07-28 04:04

#endregion

using System;
using System.Reflection;
using EggFramework.Util;
using QFramework;
using UnityEngine;

namespace EggFramework.Procedure
{
    [Serializable]
    public abstract class Variable
    {
        [SerializeField] internal string _name;
        [SerializeField] internal string _id;
        [SerializeField] private  bool   _isPublic;
        [SerializeField] private  bool   _debugBoundValue;

        ///<summary>Raised when name change</summary>
        private Action<string> _onNameChanged;

        ///<summary>Raised when value change</summary>
        private Action<object> _onValueChanged;

        ///<summary>Raised when variable is destroyed/removed from blackboard</summary>
        private Action _onDestroy;

        ///<summary>The name of the variable</summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                _onNameChanged?.Invoke(value);
            }
        }

        ///<summary>A Unique ID</summary>
        public string ID => string.IsNullOrEmpty(_id) ? _id = Guid.NewGuid().ToString() : _id;

        ///<summary>The value as object type when accessing from base class</summary>
        public object Value
        {
            get => GetValueBoxed();
            set => SetValueBoxed(value);
        }

        ///<summary>Is the variable exposed public?</summary>
        public bool IsExposedPublic
        {
            get => _isPublic && !IsPropertyBound;
            set => _isPublic = value;
        }

        ///<summary>For debugging data bound value in inspector (editor only)</summary>
        public bool DebugBoundValue
        {
            get => _debugBoundValue;
            set => _debugBoundValue = value;
        }

        ///<summary>Is the variable bound to a property/field?</summary>
        public bool IsPropertyBound => !string.IsNullOrEmpty(PropertyPath);

        ///<summary>Is the variable data bound now?</summary>
        public abstract bool IsDataBound { get; }

        ///<summary>The Type this Variable holds</summary>
        public abstract Type VarType { get; }

        ///<summary>The path to the property this data is binded to. Null if none</summary>
        public abstract string PropertyPath { get; set; }

        ///----------------------------------------------------------------------------------------------
        ///<summary>Used to bind variable to a property</summary>
        public abstract void BindProperty(MemberInfo prop, GameObject target = null);

        ///<summary>Used to un-bind variable from a property</summary>
        public abstract void UnBind();

        ///<summary>Called from Blackboard in Awake to Initialize the binding on specified game object</summary>
        public abstract void InitializePropertyBinding(GameObject go, bool callSetter = false);

        ///<summary>Same as .value. Used for binding.</summary>
        public abstract object GetValueBoxed();

        ///<summary>Same as .value. Used for binding.</summary>
        public abstract void SetValueBoxed(object value);

        ///----------------------------------------------------------------------------------------------

        //required
        public Variable()
        {
            _id = Guid.NewGuid().ToString();
        }

        public Variable(string name, string id)
        {
            _name = name;
            _id   = id;
        }

        //...
        internal void OnDestroy() => _onDestroy?.Invoke();

        public IUnRegister RegisterOnDestroy(Action onDestroy)
        {
            _onDestroy += onDestroy;
            return new CustomUnRegister(() => _onDestroy -= onDestroy);
        }

        public IUnRegister RegisterOnNameChanged(Action<string> onNameChanged)
        {
            _onNameChanged += onNameChanged;
            return new CustomUnRegister(() => _onNameChanged -= onNameChanged);
        }

        public IUnRegister RegisterOnValueChanged(Action<object> onValueChanged)
        {
            _onValueChanged += onValueChanged;
            return new CustomUnRegister(() => _onValueChanged -= onValueChanged);
        }

        ///<summary>Duplicate this Variable into target Blackboard</summary>
        public Variable Duplicate(IBlackboard targetBB)
        {
            var finalName = this.Name;
            while (targetBB.Variables.ContainsKey(finalName))
            {
                finalName += ".";
            }

            var newVar = targetBB.AddVariable(finalName, VarType);
            if (newVar != null)
            {
                newVar.Value           = Value;
                newVar.PropertyPath    = PropertyPath;
                newVar.IsExposedPublic = IsExposedPublic;
            }

            return newVar;
        }

        //we need this since onValueChanged is an event and we can't check != null outside of this class
        protected bool HasValueChangeEvent() => _onValueChanged != null;

        //invoke value changed event
        protected void TryInvokeValueChangeEvent(object value) => _onValueChanged?.Invoke(value);

        ///<summary>Checks whether a convertion to type is possible</summary>
        public bool CanConvertTo(Type toType) => GetGetConverter(toType) != null;

        ///<summary>Gets a Func<object> that converts the value ToType if possible. Null if not.</summary>
        public Func<object> GetGetConverter(Type toType)
        {
            if (toType.IsAssignableFrom(VarType))
            {
                return () => Value;
            }

            var converter = TypeConverter.Get(VarType, toType);
            if (converter != null)
            {
                return () => converter(Value);
            }

            return null;
        }

        ///<summary>Checks whether a convertion from type is possible</summary>
        public bool CanConvertFrom(Type fromType) => GetSetConverter(fromType) != null;

        ///<summary>Gets an Action<object> that converts the value fromType if possible. Null if not.</summary>
        public Action<object> GetSetConverter(Type fromType)
        {
            if (VarType.IsAssignableFrom(fromType))
            {
                return (x) => Value = x;
            }

            var converter = TypeConverter.Get(fromType, VarType);
            if (converter != null)
            {
                return (x) => Value = converter(x);
            }

            return null;
        }

        //...
        public override string ToString() => Name;
    }

    ///----------------------------------------------------------------------------------------------
    ///<summary>The actual Variable</summary>
    public class Variable<T> : Variable
    {
        [SerializeField] private T      _value;
        [SerializeField] private string _propertyPath;

        //delegates for binding
        private event Func<T> getter;

        private event Action<T> setter;

        //
        public override Type VarType     => typeof(T);
        public override bool IsDataBound => getter != null || setter != null;

        public override string PropertyPath
        {
            get => _propertyPath;
            set => _propertyPath = value;
        }

        ///<summary>The value as type T when accessing as this type</summary>
        public new T Value
        {
            get => getter != null ? getter() : _value;
            set
            {
                if (HasValueChangeEvent())
                {
                    //check this first to avoid unescessary value boxing
                    var boxed = (object)value;
                    if (!ObjectUtils.AnyEquals(_value, boxed))
                    {
                        _value = value;
                        setter?.Invoke(value);

                        TryInvokeValueChangeEvent(boxed);
                    }

                    return;
                }

                _value = value;
                setter?.Invoke(value);
            }
        }

        ///----------------------------------------------------------------------------------------------

        //required
        public Variable()
        {
        }

        public Variable(string name, string ID) : base(name, ID)
        {
        }

        ///<summary>Same as .value. Used for binding.</summary>
        public override object GetValueBoxed()
        {
            return Value;
        }

        ///<summary>Same as .value. Used for binding.</summary>
        public override void SetValueBoxed(object newValue)
        {
            this.Value = (T)newValue;
        }

        ///<summary>Same as .value. Used for binding.</summary>
        public T GetValue()
        {
            return Value;
        }

        ///<summary>Same as .value. Used for binding.</summary>
        public void SetValue(T newValue)
        {
            this.Value = newValue;
        }

        ///<summary>Set the property binding. Providing target also initializes the property binding</summary>
        public override void BindProperty(MemberInfo prop, GameObject target = null)
        {
            if (prop is not (PropertyInfo or FieldInfo)) return;
            _propertyPath = $"{prop.ReflectedOrDeclaredType().FullName}.{prop.Name}";
            if (target) InitializePropertyBinding(target);
        }

        ///<summary>Bind getter and setter directly</summary>
        public void BindGetSet(Func<T> _get, Action<T> _set)
        {
            this.getter = _get;
            this.setter = _set;
        }

        ///<summary>Removes the property and data binding</summary>
        public override void UnBind()
        {
            _propertyPath = null;
            getter        = null;
            setter        = null;
        }

        ///<summary>Initialize the property binding for target gameobject. The gameobject is only used in case the binding is not static.</summary>
        public override void InitializePropertyBinding(GameObject go, bool callSetter = false)
        {
            if (!IsPropertyBound)
            {
                return;
            }

            getter = null;
            setter = null;

            var idx          = _propertyPath.LastIndexOf('.');
            var typeString   = _propertyPath[..idx];
            var memberString = _propertyPath[(idx + 1)..];
            var type         = TypeUtil.GetType(typeString, /*fallback?*/ true, typeof(Component));

            if (type == null)
            {
                Debug.LogError(
                    $"Type '{typeString}' not found for Blackboard Variable '{Name}' Binding.");
                return;
            }

            var member = type.GetFieldOrProp(memberString);

            if (member is FieldInfo field)
            {
                var instance = field.IsStatic ? null : go.GetComponent(type);
                if (!instance && !field.IsStatic)
                {
                    Debug.LogError(
                        $"A Blackboard Variable '{Name}' is due to bind to a Component type that is missing '{typeString}'. Binding ignored");
                    return;
                }

                if (field.IsConstant())
                {
                    var value = (T)field.GetValue(instance);
                    getter = () => value;
                }
                else
                {
                    getter = () => (T)field.GetValue(instance);
                    setter = o => { field.SetValue(instance, o); };
                }

                return;
            }

            if (member is PropertyInfo prop)
            {
                var getMethod = prop.GetGetMethod();
                var setMethod = prop.GetSetMethod();
                var isStatic  = (getMethod != null && getMethod.IsStatic) || (setMethod != null && setMethod.IsStatic);
                var instance  = isStatic ? null : go.GetComponent(type);
                if (instance == null && !isStatic)
                {
                    Debug.LogError(
                        $"A Blackboard Variable '{Name}' is due to bind to a Component type that is missing '{typeString}'. Binding ignored.");
                    return;
                }

                if (prop.CanRead && getMethod != null)
                {
                    getter = () => (T)getMethod.Invoke(instance, null);
                }
                else
                {
                    getter = () =>
                    {
                        Debug.LogError(
                            $"You tried to Get a Property Bound Variable '{Name}', but the Bound Property '{_propertyPath}' is Write Only!");
                        return default;
                    };
                }

                if (prop.CanWrite && setMethod != null)
                {
                    setter = o => setMethod.Invoke(instance, new object[] { o });

                    if (callSetter)
                    {
                        setter(_value);
                    }
                }
                else
                {
                    setter = _ =>
                    {
                        Debug.LogError(
                            $"You tried to Set a Property Bound Variable '{Name}', but the Bound Property '{_propertyPath}' is Read Only!");
                    };
                }

                return;
            }

            Debug.LogError(
                $"A Blackboard Variable '{Name}' is due to bind to a property/field named '{memberString}' that does not exist on type '{type.FullName}'. Binding ignored");
        }
    }
}