#region

//文件创建者：Egg
//创建时间：09-23 03:28

#endregion

using System;

namespace EggFramework.CodeGenKit
{
    public sealed class MemberCode : ICode
    {
        public enum EMemberPrivacy
        {
            Public,
            Protected,
            Private
        }

        private readonly EMemberPrivacy _privacy;
        private readonly Type _type;
        private readonly string _fieldName;
        private readonly bool _isStatic;

        public MemberCode(EMemberPrivacy privacy, Type type, string fieldName, bool isStatic)
        {
            _privacy = privacy;
            _type = type;
            _fieldName = fieldName;
            _isStatic = isStatic;
        }

        public void Gen(ICodeWriter writer)
        {
            writer.WriteLine($@"{(_privacy switch
            {
                EMemberPrivacy.Private => "private ",
                EMemberPrivacy.Protected => "protected ",
                EMemberPrivacy.Public => "public ",
                _ => "private "
            })}{(_isStatic ? "static " : string.Empty)}{_type.FullName} {_fieldName};");
        }
    }

    public static partial class CodeScopeExtensions
    {
        public static ICodeScope Member(this ClassScope self, MemberCode.EMemberPrivacy privacy, Type type,
            string fieldName,
            Action<ClassScope> nameSpaceSetting = null)
        {
            self.Codes.Add(new MemberCode(privacy, type, fieldName, self.IsStatic));
            return self;
        }
    }
}