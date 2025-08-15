#region

//文件创建者：Egg
//创建时间：09-23 02:48

#endregion

using System;

namespace EggFramework.CodeGenKit
{
    public sealed class NameSpaceScope : CodeScope
    {
        private readonly string _nameSpace;

        public NameSpaceScope(string nameSpace)
        {
            _nameSpace = nameSpace;
        }

        protected override void GenFirstLine(ICodeWriter writer)
        {
            writer.WriteLine($"namespace {_nameSpace}");
        }
    }

    public static partial class CodeScopeExtensions
    {
        public static ICodeScope NameSpace(this ICodeScope self, string nameSpace,
            Action<NameSpaceScope> nameSpaceSetting = null)
        {
            var nameSpaceCode = new NameSpaceScope(nameSpace);
            nameSpaceSetting?.Invoke(nameSpaceCode);
            self.Codes.Add(nameSpaceCode);
            return self;
        }
    }
}