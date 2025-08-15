#region

//文件创建者：Egg
//创建时间：11-29 04:06

#endregion

#if UNITY_EDITOR

using System.IO;
using EggFramework.Util;
using Sirenix.OdinInspector;
using UnityEditor;

namespace EggFramework
{
    public static class ScriptInitUtil
    {
        public static void InitProject(string nameSpace, bool withNotify, ref string msg)
        {
            if (withNotify && !EditorUtility.DisplayDialog("初始化项目", "一个项目只能初始化一次，不然会出大问题！！！", "确认", "取消"))
            {
                msg = "初始化终止";
                return;
            }

            msg = "初始化开始";
            DirectoryUtil.MakeSureDirectory("Assets/Scripts/Controllers/Global");
            DirectoryUtil.MakeSureDirectory("Assets/Scripts/Events");
            File.WriteAllText("Assets/Scripts/Controllers/Global/Singleton.cs", "using UnityEngine;\n" +
                "\n" +
                $"namespace {nameSpace}\n" +
                "{\n" +
                "    public class MonoSingleton<T> : AbstractController where T : MonoSingleton<T>, new()\n" +
                "    {\n" +
                "        private static T _instance;\n" +
                "\n" +
                "        public static T Inst\n" +
                "        {\n" +
                "            get\n" +
                "            {\n" +
                "                if (_instance == null)\n" +
                "                {\n" +
                "                    // 如果实例不存在，则查找场景中已有的SingletonClass组件并将其设置为_instance\n" +
                "                    _instance = FindObjectOfType<T>();\n" +
                "\n" +
                "                    // 如果没有找到实例，创建一个新的GameObject挂载SingletonClass脚本并设置为_instance\n" +
                "                    if (_instance == null)\n" +
                "                    {\n" +
                "                        var singletonObject = new GameObject();\n" +
                "                        _instance            = singletonObject.AddComponent<T>();\n" +
                "                        singletonObject.name = typeof(T).Name;\n" +
                "                        DontDestroyOnLoad(singletonObject); // 保持单例在整个应用生命周期内不被销毁\n" +
                "                    }\n" +
                "                }\n" +
                "\n" +
                "                return _instance;\n" +
                "            }\n" +
                "        }\n" +
                "\n" +
                "        protected MonoSingleton()\n" +
                "        {\n" +
                "        }\n" +
                "\n" +
                "        protected virtual void Awake()\n" +
                "        {\n" +
                "            // 如果_instance尚未初始化，并且当前对象不是_instance（即第一次Awake调用）\n" +
                "            if (_instance == null && this != _instance)\n" +
                "            {\n" +
                "                // 设置当前对象为_instance\n" +
                "                _instance = this as T;\n" +
                "\n" +
                "                // 防止切换场景时该单例对象被销毁\n" +
                "                DontDestroyOnLoad(gameObject);\n" +
                "            }\n" +
                "            else if (this != _instance) // 若_instance已存在但不是当前对象，则销毁多余的实例\n" +
                "            {\n" +
                "                Destroy(gameObject);\n" +
                "            }\n" +
                "        }\n" +
                "        protected void Log(object log) => Debug.Log($\"[{GetType().Name}]:\" + log);\n"+
                "        protected void LogWarning(object log) => Debug.LogWarning($\"[{GetType().Name}]:\" + log);\n"+
                "        protected void LogError(object log) => Debug.LogError($\"[{GetType().Name}]:\" + log);\n"+
                "    }\n" +
                "}");
            AssetDatabase.ImportAsset("Assets/Scripts/Controllers/Global/Singleton.cs");

            File.WriteAllText("Assets/Scripts/Controllers/DemoEntry.cs", "using UnityEngine;\n" +
                                                                         "using EggFramework.Modules.Launch;\n" +
                                                                         "using QFramework;\n" +
                                                                         "\n" +
                                                                         $"namespace {nameSpace}\n" +
                                                                         "{\n" +
                                                                         "    public sealed class DemoEntry : AbstractController\n" +
                                                                         "    {\n" +
                                                                         "        private void Awake()\n" +
                                                                         "        {\n" +
                                                                         $"            var lfsm = new LaunchFSM({nameSpace}App.Interface);\n" +
                                                                         "            lfsm.OnLaunchComplete(() => this.SendEvent(new ArchitectureInitFinishEvent()));\n" +
                                                                         "            lfsm.Start();\n" +
                                                                         "        }\n" +
                                                                         "    }\n" +
                                                                         "}");
            AssetDatabase.ImportAsset("Assets/Scripts/Controllers/DemoEntry.cs");

            File.WriteAllText("Assets/Scripts/Events/ArchitectureInitFinishEvent.cs", $"namespace {nameSpace}\n" +
                "{\n" +
                "    public struct ArchitectureInitFinishEvent\n" +
                "    {\n" +
                "    }\n" +
                "}");
            AssetDatabase.ImportAsset("Assets/Scripts/Events/ArchitectureInitFinishEvent.cs");

            File.WriteAllText("Assets/Scripts/Controllers/AbstractController.cs", "using QFramework;\n" +
                "using UnityEngine;\n" +
                "\n" +
                $"namespace {nameSpace}\n" +
                "{\n" +
                "    public class AbstractController : MonoBehaviour, IController\n" +
                "    {\n" +
                $"        public IArchitecture GetArchitecture() => {nameSpace}App.Interface;\n" +
                "    }\n" +
                "}");
            AssetDatabase.ImportAsset("Assets/Scripts/Controllers/AbstractController.cs");
            File.WriteAllText($"Assets/Scripts/{nameSpace}App.cs", "using EggFramework;\n" +
                                                                   "using EggFramework.ObjectPool;\n" +
                                                                   "using EggFramework.AudioSystem;\n" +
                                                                   "using EggFramework.TimeSystem;\n" +
                                                                   "using EggFramework.Storage;\n" +
                                                                   "using QFramework;\n" +
                                                                   "\n" +
                                                                   $"namespace {nameSpace}\n" +
                                                                   "{\n" +
                                                                   $"    public sealed class {nameSpace}App : Architecture<{nameSpace}App>\n" +
                                                                   "    {\n" +
                                                                   "        protected override void Init()\n" +
                                                                   "        {\n" +
                                                                   "            RegisterSystem<IAudioSystem>(new AudioSystem());\n" +
                                                                   "            RegisterSystem<IObjectPoolSystem>(new ObjectPoolSystem());\n" +
                                                                   "            RegisterSystem<IFileSystem>(new FileSystem());\n" +
                                                                   "            RegisterSystem<ITimeSystem>(new TimeSystem());\n" +
                                                                   "            RegisterSystem<IQuickShotSystem>(new QuickShotSystem());\n" +
                                                                   "            RegisterUtility<IStorage>(new JsonStorage());\n" +
                                                                   "        }\n" +
                                                                   "    }\n" +
                                                                   "}");
            AssetDatabase.ImportAsset($"Assets/Scripts/{nameSpace}App.cs");
        }
    }
}
#endif