#region

//文件创建者：Egg
//创建时间：09-06 11:07

#endregion

using UnityEngine;

namespace EggFramework.Util
{
    public static class MaterialUtil
    {
        public static readonly MaterialPropertyBlock Block = new();

        public static void SetMaterialParam(this Component self, int id, float value)
        {
            var sr = self.GetComponent<Renderer>();
            sr.GetPropertyBlock(Block);
            Block.SetFloat(id, value);
            sr.SetPropertyBlock(Block);
        }
        
        public static void SetMaterialParam(this Component self, int id, Vector4 value)
        {
            var sr = self.GetComponent<Renderer>();
            sr.GetPropertyBlock(Block);
            Block.SetVector(id, value);
            sr.SetPropertyBlock(Block);
        }
        
        public static void SetMaterialParam(this Component self, int id, Color value)
        {
            var sr = self.GetComponent<Renderer>();
            sr.GetPropertyBlock(Block);
            Block.SetColor(id, value);
            sr.SetPropertyBlock(Block);
        }
        
        public static void SetMaterialParam(this Component self, int id, int value)
        {
            var sr = self.GetComponent<Renderer>();
            sr.GetPropertyBlock(Block);
            Block.SetInt(id, value);
            sr.SetPropertyBlock(Block);
        }
    }
}