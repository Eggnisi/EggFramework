#region

//文件创建者：Egg
//创建时间：09-23 02:57

#endregion

using System;

namespace EggFramework.CodeGenKit
{
    public sealed class ClassScope : CodeScope
    {
        private readonly bool _isPartial;
        private readonly bool _isStatic;
        private readonly bool _isSealed;
        private readonly string _parentClassName;
        private readonly string _className;

        public bool IsStatic => _isStatic;

        public ClassScope(bool isPartial, bool isStatic, bool isSealed, string className, string parentClassName = "")
        {
            _isPartial = isPartial;
            _isStatic = isStatic;
            _isSealed = isSealed;
            _parentClassName = parentClassName;
            _className = className;
        }

        protected override void GenFirstLine(ICodeWriter writer)
        {
            writer.WriteLine(
                $"public {(_isSealed ? "sealed " : string.Empty)}{(_isStatic ? "static " : string.Empty)}{(_isPartial ? "partial " : string.Empty)}class {_className}{(string.IsNullOrEmpty(_parentClassName) ? string.Empty : " : " + _parentClassName)}");
        }
    }

    public static partial class CodeScopeExtensions
    {
        public static ICodeScope Class(this ICodeScope self, string className, bool isPartial, bool isStatic,
            bool isSealed,
            string parentClassName = "", Action<ClassScope> classScopeSetting = null)
        {
            var classScope = new ClassScope(isPartial, isStatic, isSealed, className, parentClassName);
            classScopeSetting?.Invoke(classScope);
            self.Codes.Add(classScope);
            return self;
        }
    }
}