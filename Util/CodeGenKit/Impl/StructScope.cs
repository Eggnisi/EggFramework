#region

//文件创建者：Egg
//创建时间：09-23 02:57

#endregion

using System;

namespace EggFramework.CodeGenKit
{
    public sealed class StructScope : CodeScope
    {
        private readonly bool _isPartial;
        private readonly string _parentClassName;
        private readonly string _className;

        public StructScope(bool isPartial, string className, string parentClassName = "")
        {
            _isPartial = isPartial;
            _parentClassName = parentClassName;
            _className = className;
        }

        protected override void GenFirstLine(ICodeWriter writer)
        {
            writer.WriteLine(
                $"public {(_isPartial ? "partial " : string.Empty)}struct {_className}{(string.IsNullOrEmpty(_parentClassName) ? string.Empty : " : " + _parentClassName)}");
        }
    }

    public static partial class CodeScopeExtensions
    {
        public static ICodeScope Struct(this ICodeScope self, string className, bool isPartial,
            string parentClassName = "", Action<StructScope> structScopeSetting = null)
        {
            var structScope = new StructScope(isPartial, className, parentClassName);
            structScopeSetting?.Invoke(structScope);
            self.Codes.Add(structScope);
            return self;
        }
    }
}