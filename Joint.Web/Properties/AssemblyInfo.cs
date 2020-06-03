using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 有关程序集的常规信息是通过以下项进行控制的
// 控制。更改这些特性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("Joint.Web")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Joint.Web")]
[assembly: AssemblyCopyright("版权所有(C)  2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 将 ComVisible 设置为 false 将使此程序集中的类型
// 对 COM 组件不可见。如果需要
// 从 COM 访问此程序集中的某个类型，请针对该类型将 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于 typelib 的 ID
[assembly: Guid("0e4b9d6a-55f6-4b47-add9-6f70e5319270")]

// 程序集的版本信息由下列四个值组成:
//
//      主版本
//      次版本
//      内部版本号
//      修订版本
//
// 你可以指定所有值，也可以让修订版本和内部版本号采用默认值，
// 方法是按如下所示使用 "*":
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

//注册Log4Net日志
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Log4net.config", Watch = true)]
//[assembly: log4net.Config.DOMConfigurator(ConfigFile = "Web.config", Watch = true)]
