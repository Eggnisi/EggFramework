# EggCMD设计文档
+ 愿景：实现一个类命令行的工具，可以使用命令行执行框架中的所有操作
+ 具体工作：
  + 命令行窗口
    + 输入框和显示口
    + 支持命令补全
    + 支持命令回溯
    + 支持清屏（clear）
    + 支持快捷键呼出
  + 命令程序
    + 命令解析器
      + 将命令文本转化为原始命令： 命令Id，命令参数，选项和选项参数
      + ````
        RawCommand{
          string CommandId
          string[] CommandParams
          Dictionary<string,string> OptionDic
        }
        ````
      + 命令处理器 CommandHandle<T1,T2,T3...etc>
        + 存储RawCommand，OptionDic
        + void Handle(T1 t1, T2 t2, T3 t3 etc...)
        + 首先查找参数数量一致的原始命令
        + 其次按照优先级依次匹配参数
      + Logger
        + 在命令面板激活时输出到命令面板上，否则输出到Console
      + 支持自定义命令，支持环境变量
    