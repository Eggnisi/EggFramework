#region

//文件创建者：Egg
//创建时间：09-23 02:44

#endregion

using System.Text;

namespace EggFramework.CodeGenKit.Writer
{
    public sealed class StringWriter : ICodeWriter
    {
        public int IndentCount { get; set; }

        private string _indent
        {
            get
            {
                var ret = string.Empty;
                for (var i = 0; i < IndentCount; i++)
                {
                    ret += '\t';
                }

                return ret;
            }
        }

        private readonly StringBuilder _builder;

        public StringWriter(StringBuilder builder)
        {
            _builder = builder;
        }

        public void WriteLine(string code = null)
        {
            _builder.Append(_indent + code + '\n');
        }

        public void Dispose()
        {
            _builder.Clear();
        }
    }
}