#region

//文件创建者：Egg
//创建时间：04-08 07:20

#endregion

using System;

namespace EggFramework
{
    [Serializable]
    public sealed class ExamplePayload : IFileInfoPayload
    {
        public string CharacterName;
        public string CharacterType;
        public string CurrentScene;
        public float  GameProgress;
    }
}