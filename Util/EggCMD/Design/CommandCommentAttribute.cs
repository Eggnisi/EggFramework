#region

//文件创建者：Egg
//创建时间：03-26 04:27

#endregion

using System;

namespace EggFramework.Util.EggCMD
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Class)]
    public sealed class CommandCommentAttribute : Attribute
    {
        public string Comment;
        
        public CommandCommentAttribute(string comment)
        {
            Comment = comment;
        }
    }
}