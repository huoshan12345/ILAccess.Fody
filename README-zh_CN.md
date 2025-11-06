# ILAccess.Fody 
[![构建状态](https://github.com/huoshan12345/ILAccess.Fody/workflows/Build/badge.svg)](https://github.com/huoshan12345/ILAccess.Fody/actions?query=workflow%3ABuild)
[![NuGet 包](https://img.shields.io/nuget/v/ILAccess.Fody.svg?logo=NuGet)](https://www.nuget.org/packages/ILAccess.Fody)
[![.NET](https://img.shields.io/badge/.net%20standard-2.0-ff69b4.svg?)](https://www.microsoft.com/net/download)
[![许可证](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/huoshan12345/ILAccess.Fody/blob/main/LICENSE)
[![en](https://img.shields.io/badge/lang-en-red.svg)](https://github.com/huoshan12345/ILAccess.Fody/blob/main/README.md)

## ✨ 概述

`ILAccess.Fody` 提供与 .NET 8 引入的 [UnsafeAccessor](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.unsafeaccessorattribute?view=net-8.0) 类似的功能，但它支持旧版本的 .NET 平台。  
这是一个基于 [Fody](https://github.com/Fody/Fody) 的编织器（weaver），在编译时注入 IL 指令，从而无需运行时反射即可访问私有或内部成员。  
与传统的基于反射的方式相比，这种方法具有更快的访问速度和编译时安全性。

---

## 🚀 安装

- 在项目中添加 [`Fody`](https://www.nuget.org/packages/Fody) 和 [`ILAccess.Fody`](https://www.nuget.org/packages/ILAccess.Fody) NuGet 包，并在 `<PackageReference />` 元素中添加 `PrivateAssets="all"` 属性。  
  需要显式安装 `Fody` 以启用编织功能。

  ```XML
  <PackageReference Include="Fody" Version="..." PrivateAssets="all" />
  <PackageReference Include="ILAccess.Fody" Version="..." PrivateAssets="all" />
  ```

- 如果项目根目录下已经存在 `FodyWeavers.xml` 文件，请在其中添加 `<ILAccess />` 标签。  
  如果该文件不存在，它会在首次构建时自动生成：

  ```XML
  <?xml version="1.0" encoding="utf-8" ?>
  <Weavers>
    <ILAccess />
  </Weavers>
  ```

更多通用说明请参考 [Fody 使用指南](https://github.com/Fody/Home/blob/main/pages/usage.md)，以及 [Fody 配置文档](https://github.com/Fody/Home/blob/main/pages/configuration.md)。

---

## 🧩 使用示例

你可以使用 `ILAccessor` 来访问私有字段、方法或构造函数 —— 类似于 .NET 8 的 `UnsafeAccessor`。

```csharp
public class TestModel
{
    private static int _staticValue = 42;
    private int _value;
    private TestModel(int value) => _value = value;
    private string GetMessage(int code) 
        => $"当前值: {_value}, 代码: {code}";
    private static string GetStaticMessage(int code) 
        => $"当前静态值: {_staticValue}, 代码: {code}";
}

public static class Accessors
{
    [ILAccessor(ILAccessorKind.Field, Name = "_value")]
    public static extern ref int Value(this TestModel instance);

    [ILAccessor(ILAccessorKind.StaticField, Name = "_staticValue")]
    public static extern ref int StaticValue(TestModel instance);

    [ILAccessor(ILAccessorKind.Method, Name = "GetMessage")]
    public static extern string GetMessage(this TestModel instance, int code);

    [ILAccessor(ILAccessorKind.StaticMethod, Name = "GetStaticMessage")]
    public static extern string GetStaticMessage(TestModel? instance, int code);

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel Ctor(int x);
}

internal class Program
{
    private static void Main(string[] args)
    {
        var model = Ctor(100);
        ref var value = ref model.Value();
        Console.WriteLine($"_value: {value}");

        value += 50;
        Console.WriteLine($"_value 更新后: {value}");

        ref var staticValue = ref StaticValue(model);
        Console.WriteLine($"_staticValue: {staticValue}");
        staticValue += 10;
        Console.WriteLine($"_staticValue 更新后: {staticValue}");

        var message = model.GetMessage(7);
        Console.WriteLine($"GetMessage: {message}");

        var staticMessage = GetStaticMessage(null, 7);
        Console.WriteLine($"GetStaticMessage: {staticMessage}");

        Console.Read();
    }
}
```

---

## 🛠️ 工作原理

在编译时，`TestModel` 中的存根方法会被替换为直接访问目标成员的 IL 指令。  
下面是编织（weaving）后生成的 IL 示例：

```il
.method public hidebysig static int32& Value(class ILAccess.Example.TestModel 'instance') cil managed
{
    IL_0000: ldarg.0      // 'instance'
    IL_0001: ldflda       int32 ILAccess.Example.TestModel::_value
    IL_0006: ret
}

.method public hidebysig static int32& StaticValue(class ILAccess.Example.TestModel 'instance') cil managed
{
    IL_0000: ldsflda      int32 ILAccess.Example.TestModel::_staticValue
    IL_0005: ret
}

.method public hidebysig static string GetMessage(class ILAccess.Example.TestModel 'instance', int32 code) cil managed
{
    IL_0000: ldarg.0      // 'instance'
    IL_0001: ldarg.1      // code
    IL_0002: callvirt     instance string ILAccess.Example.TestModel::GetMessage(int32)
    IL_0007: ret
}

.method public hidebysig static string GetStaticMessage(class ILAccess.Example.TestModel 'instance', int32 code) cil managed
{
	IL_0000: ldarg.1      // code
	IL_0001: call         string ILAccess.Example.TestModel::GetStaticMessage(int32)
	IL_0006: ret
}

.method public hidebysig static class ILAccess.Example.TestModel Ctor(int32 x) cil managed
{
	IL_0000: ldarg.0      // x
	IL_0001: newobj       instance void ILAccess.Example.TestModel::.ctor(int32)
	IL_0006: ret
}
```

这些注入的方法体有效地实现了对私有和静态成员的强类型访问，无需使用反射。

---

## ⚖️ 对比

| 特性 | Reflection | UnsafeAccessor | ILAccess.Fody |
|---|---:|---:|---:|
| 性能 | 慢 🐌 | 快 🚀 | 快 🚀 |
| 支持 .NET 8 之前版本 | ✅ | ❌ | ✅ |
| 编译时验证 | ❌ | ❌ | ✅ |
| AOT 支持 | 部分支持 ⚠️ | ✅ | ✅ |

---

## 🧭 待办事项

- [ ] 增加更多测试用例。
- [ ] 增加更多编译时验证与诊断信息。

---

## 📄 许可证

MIT 许可证 — 详情请参阅 [LICENSE](LICENSE)。
