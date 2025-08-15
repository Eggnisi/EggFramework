using System;

namespace EggFramework.Procedure
{
    public class NodeTitleAttribute : Attribute
    {
        public string Title { get; }
        public NodeTitleAttribute(string title) => Title = title;
    }
}