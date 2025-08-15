#region

// 作者： Egg
// 时间： 2025 07.19 09:26

#endregion

using System;
using System.Collections.Generic;
using EggFramework.Util.EggCMD;

namespace EggFramework
{
    public abstract class FuncHandle<TResult> : IFuncHandle
    {
        protected FuncContext _context { get; private set; }

        public void SetContext(FuncContext context)
        {
            _context = context;
        }

        public object Handle(params object[] paramsObjects)
        {
            if (paramsObjects.Length > 0) 
                throw new Exception("此函数不需要参数");
            
            return Handle();
        }

        public bool ParamValidCheck(List<string> rawParams)
        {
            if (rawParams.Count > 0)
                throw new Exception("此函数不需要参数");
            
            return true;
        }

        protected abstract TResult Handle();
    }

    // 单参数版本
    public abstract class FuncHandle<T, TResult> : IFuncHandle
    {
        protected FuncContext _context { get; private set; }

        public void SetContext(FuncContext context)
        {
            _context = context;
        }

        public object Handle(params object[] paramsObjects)
        {
            if (paramsObjects.Length < 1) 
                throw new Exception("需要1个参数");
            
            return Handle((T)paramsObjects[0]);
        }

        public bool ParamValidCheck(List<string> rawParams)
        {
            if (rawParams.Count < 1) 
                throw new Exception("需要1个参数");
            
            var ret = TokenSplitter.ParseParam(rawParams[0], typeof(T), out _);
            return ret;
        }

        protected abstract TResult Handle(T p1);
    }

    // 双参数版本
    public abstract class FuncHandle<T1, T2, TResult> : IFuncHandle
    {
        protected FuncContext _context { get; private set; }

        public void SetContext(FuncContext context)
        {
            _context = context;
        }

        public object Handle(params object[] paramsObjects)
        {
            if (paramsObjects.Length < 2) 
                throw new Exception("需要2个参数");
            
            return Handle(
                (T1)paramsObjects[0],
                (T2)paramsObjects[1]
            );
        }

        public bool ParamValidCheck(List<string> rawParams)
        {
            if (rawParams.Count < 2) 
                throw new Exception("需要2个参数");
            
            var ret = true;
            ret &= TokenSplitter.ParseParam(rawParams[0], typeof(T1), out _);
            ret &= TokenSplitter.ParseParam(rawParams[1], typeof(T2), out _);
            return ret;
        }

        protected abstract TResult Handle(T1 p1, T2 p2);
    }

    // 三参数版本
    public abstract class FuncHandle<T1, T2, T3, TResult> : IFuncHandle
    {
        protected FuncContext _context { get; private set; }

        public void SetContext(FuncContext context)
        {
            _context = context;
        }

        public object Handle(params object[] paramsObjects)
        {
            if (paramsObjects.Length < 3) 
                throw new Exception("需要3个参数");
            
            return Handle(
                (T1)paramsObjects[0],
                (T2)paramsObjects[1],
                (T3)paramsObjects[2]
            );
        }

        public bool ParamValidCheck(List<string> rawParams)
        {
            if (rawParams.Count < 3) 
                throw new Exception("需要3个参数");
            
            var ret = true;
            ret &= TokenSplitter.ParseParam(rawParams[0], typeof(T1), out _);
            ret &= TokenSplitter.ParseParam(rawParams[1], typeof(T2), out _);
            ret &= TokenSplitter.ParseParam(rawParams[2], typeof(T3), out _);
            return ret;
        }

        protected abstract TResult Handle(T1 p1, T2 p2, T3 p3);
    }

    // 四参数版本
    public abstract class FuncHandle<T1, T2, T3, T4, TResult> : IFuncHandle
    {
        protected FuncContext _context { get; private set; }

        public void SetContext(FuncContext context)
        {
            _context = context;
        }

        public object Handle(params object[] paramsObjects)
        {
            if (paramsObjects.Length < 4) 
                throw new Exception("需要4个参数");
            
            return Handle(
                (T1)paramsObjects[0],
                (T2)paramsObjects[1],
                (T3)paramsObjects[2],
                (T4)paramsObjects[3]
            );
        }

        public bool ParamValidCheck(List<string> rawParams)
        {
            if (rawParams.Count < 4) 
                throw new Exception("需要4个参数");
            
            var ret = true;
            ret &= TokenSplitter.ParseParam(rawParams[0], typeof(T1), out _);
            ret &= TokenSplitter.ParseParam(rawParams[1], typeof(T2), out _);
            ret &= TokenSplitter.ParseParam(rawParams[2], typeof(T3), out _);
            ret &= TokenSplitter.ParseParam(rawParams[3], typeof(T4), out _);
            return ret;
        }

        protected abstract TResult Handle(T1 p1, T2 p2, T3 p3, T4 p4);
    }
}