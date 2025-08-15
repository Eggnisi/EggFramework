#region

//文件创建者：Egg
//创建时间：09-23 02:19

#endregion

using System.Collections.Generic;

namespace EggFramework.CodeGenKit
{
    public abstract class CodeScope : ICodeScope
    {
        public bool Semicolon { get; set; }

        public virtual void Gen(ICodeWriter writer)
        {
            GenFirstLine(writer);

            new OpenBraceCode().Gen(writer);

            writer.IndentCount++;

            foreach (var code in Codes)
            {
                code.Gen(writer);
            }

            writer.IndentCount--;

            new CloseBraceCode(Semicolon).Gen(writer);
        }

        protected abstract void GenFirstLine(ICodeWriter writer);

        public List<ICode> Codes { get; } = new();
    }
}