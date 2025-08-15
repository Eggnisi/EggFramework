#region

//文件创建者：Egg
//创建时间：07-24 03:43

#endregion

using System;
using System.Collections.Generic;

namespace EggFramework.Executable
{
    public static class ExecutableExample
    {
        public static void RunExample()
        {
            var script =
                "var $enemy = $enemy_list[0];"+
                "print($enemy);" +
                "print($enemy_list.index_of(\"猴哥\"));" +
                "print($enemy_info.to_string());" +
                "print($enemy_info.tags[0])" +
                "if($shield == 3 || $health > 8 && has_buff(\"flame\")){" +
                "   attack(add(1, $dice), \"flame\");" +
                "}else{" +
                "   heal($shield, \"flame\");" +
                "   heal($shield);" +
                "}";
            var parser = new ScriptParser();
            var ast    = parser.ParseStatement(script); 
            var context = new ExecutionContext
            {
                Variables =
                {
                    ["enemy_info"] = new EnemyInfo
                    {
                        Name         = "测试敌人",
                        Desc         = "我是初始NPC",
                        MaxHealth    = 5,
                        Cards        = new List<string> { "天下无贼", "天外飞鲜" },
                        DicePerRound = 5,
                        Tags         = new List<string> { "小怪", "火焰抗性低" }
                    },
                    ["enemy_list"] = new List<string>() { "猴哥", "天蓬元帅" },
                    ["health"]     = 3,
                    ["shield"]     = 3,
                    ["dice"]      = 3
                }
            };
            
            var executor = new ScriptExecutor();
            executor.ExecuteStatement(ast, context);
        }
    }
    
    [Serializable]
    public sealed class EnemyInfo
    {
        public string Name;
        public string Desc;
        public int    MaxHealth;
        public List<string> Cards;
        public int DicePerRound;
        public List<string> Tags;

        public override string ToString()
        {
            return $"{GetType().Name} {{ " +
                   $"{nameof(Name)} = {Name}, " +
                   $"{nameof(Desc)} = {Desc}, " +
                   $"{nameof(MaxHealth)} = {MaxHealth}, " +
                   $"{nameof(Cards)} = [{string.Join(", ", Cards)}], " +
                   $"{nameof(DicePerRound)} = {DicePerRound}, " +
                   $"{nameof(Tags)} = [{string.Join(", ", Tags)}] }}";
        }
    }
}