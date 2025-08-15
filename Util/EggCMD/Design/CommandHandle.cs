#region

//文件创建者：Egg
//创建时间：03-25 07:22

#endregion

using System;
using System.Collections.Generic;

namespace EggFramework.Util.EggCMD
{
    public abstract class CommandHandle : ICommandHandle
    {
        protected CommandContext _context { get; private set; }

        public void SetContext(CommandContext context)
        {
            _context = context;
        }

        void ICommandHandle.Handle(params object[] paramsObjects)
        {
            Handle();
        }

        bool ICommandHandle.ParamValidCheck(List<string> rawParams)
        {
            return true;
        }

        protected abstract void Handle();
    }

    public abstract class CommandHandle<T> : ICommandHandle
    {
        protected CommandContext _context { get; private set; }

        public void SetContext(CommandContext context)
        {
            _context = context;
        }

        void ICommandHandle.Handle(params object[] paramsObjects)
        {
            if (paramsObjects.Length < 1) throw new Exception("至少需要1个参数");
            Handle((T)paramsObjects[0]);
        }

        bool ICommandHandle.ParamValidCheck(List<string> rawParams)
        {
            if (rawParams.Count < 1) throw new Exception("至少需要1个参数");
            var ret = true;
            ret &= TokenSplitter.ParseParam(rawParams[0], typeof(T), out _);
            return ret;
        }

        protected abstract void Handle(T p1);
    }

    public abstract class CommandHandle<T1, T2> : ICommandHandle
    {
        protected CommandContext _context { get; private set; }

        public void SetContext(CommandContext context)
        {
            _context = context;
        }

        void ICommandHandle.Handle(params object[] paramsObjects)
        {
            if (paramsObjects.Length < 2) throw new Exception("至少需要2个参数");
            Handle(
                (T1)paramsObjects[0],
                (T2)paramsObjects[1]
            );
        }

        bool ICommandHandle.ParamValidCheck(List<string> rawParams)
        {
            if (rawParams.Count < 2) throw new Exception("至少需要2个参数");
            var ret = true;
            ret &= TokenSplitter.ParseParam(rawParams[0], typeof(T1), out _);
            ret &= TokenSplitter.ParseParam(rawParams[1], typeof(T2), out _);
            return ret;
        }

        protected abstract void Handle(T1 p1, T2 p2);
    }

    public abstract class CommandHandle<T1, T2, T3> : ICommandHandle
    {
        protected CommandContext _context { get; private set; }

        public void SetContext(CommandContext context)
        {
            _context = context;
        }

        void ICommandHandle.Handle(params object[] paramsObjects)
        {
            if (paramsObjects.Length < 3) throw new Exception("至少需要3个参数");
            Handle(
                (T1)paramsObjects[0],
                (T2)paramsObjects[1],
                (T3)paramsObjects[2]
            );
        }

        bool ICommandHandle.ParamValidCheck(List<string> rawParams)
        {
            if (rawParams.Count < 3) throw new Exception("至少需要3个参数");
            var ret = true;
            ret &= TokenSplitter.ParseParam(rawParams[0], typeof(T1), out _);
            ret &= TokenSplitter.ParseParam(rawParams[1], typeof(T2), out _);
            ret &= TokenSplitter.ParseParam(rawParams[2], typeof(T3), out _);
            return ret;
        }

        protected abstract void Handle(T1 p1, T2 p2, T3 p3);
    }

    public abstract class CommandHandle<T1, T2, T3, T4> : ICommandHandle
    {
        protected CommandContext _context { get; private set; }

        public void SetContext(CommandContext context)
        {
            _context = context;
        }

        void ICommandHandle.Handle(params object[] paramsObjects)
        {
            if (paramsObjects.Length < 4) throw new Exception("至少需要4个参数");
            Handle(
                (T1)paramsObjects[0],
                (T2)paramsObjects[1],
                (T3)paramsObjects[2],
                (T4)paramsObjects[3]
            );
        }

        bool ICommandHandle.ParamValidCheck(List<string> rawParams)
        {
            if (rawParams.Count < 4) throw new Exception("至少需要4个参数");
            var ret = true;
            ret &= TokenSplitter.ParseParam(rawParams[0], typeof(T1), out _);
            ret &= TokenSplitter.ParseParam(rawParams[1], typeof(T2), out _);
            ret &= TokenSplitter.ParseParam(rawParams[2], typeof(T3), out _);
            ret &= TokenSplitter.ParseParam(rawParams[3], typeof(T4), out _);
            return ret;
        }

        protected abstract void Handle(T1 p1, T2 p2, T3 p3, T4 p4);
    }
}