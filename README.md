# ILAccess.Fody 
[![Build](https://github.com/huoshan12345/ILAccess.Fody/workflows/Build/badge.svg)](https://github.com/huoshan12345/ILAccess.Fody/actions?query=workflow%3ABuild)
[![NuGet package](https://img.shields.io/nuget/v/ILAccess.Fody.svg?logo=NuGet)](https://www.nuget.org/packages/ILAccess.Fody)
[![.net](https://img.shields.io/badge/.net%20standard-2.0-ff69b4.svg?)](https://www.microsoft.com/net/download)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/huoshan12345/ILAccess.Fody/blob/master/LICENSE)  

## ✨ Overview

`ILAccess.Fody` provides functionality similar to the [UnsafeAccessor](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.unsafeaccessorattribute?view=net-8.0) introduced in .NET 8, but supports older .NET platforms. It is a [Fody](https://github.com/Fody/Fody) weaver that injects IL at compile-time, enabling access to private or internal members without runtime reflection. This results in faster access and compile-time safety compared to traditional reflection-based approaches.

---

## 🚀 Installation
- Include the [`Fody`](https://www.nuget.org/packages/Fody) and [`ILAccess.Fody`](https://www.nuget.org/packages/ILAccess.Fody) NuGet packages with a `PrivateAssets="all"` attribute on their `<PackageReference />` items. Installing `Fody` explicitly is needed to enable weaving.

  ```XML
  <PackageReference Include="Fody" Version="..." PrivateAssets="all" />
  <PackageReference Include="ILAccess.Fody" Version="..." PrivateAssets="all" />
  ```

- If you already have a `FodyWeavers.xml` file in the root directory of your project, add the `<ILAccess />` tag there. This file will be created on the first build if it doesn't exist:

  ```XML
  <?xml version="1.0" encoding="utf-8" ?>
  <Weavers>
    <ILAccess />
  </Weavers>
  ```
See [Fody usage](https://github.com/Fody/Home/blob/master/pages/usage.md) for general guidelines, and [Fody Configuration](https://github.com/Fody/Home/blob/master/pages/configuration.md) for additional options.

---

## 🧩 Usage Example

You can use `ILAccessor` to access private fields, methods, or constructors — similar to `UnsafeAccessor` since .NET 8.

```csharp
public class TestModel
{
    private static int _staticValue = 42;
    private int _value;
    private TestModel(int value) => _value = value;
    private string GetMessage(int code) 
        => $"Current value: {_value}, code: {code}";
    private static string GetStaticMessage(int code) 
        => $"Current static value: {_staticValue}, code: {code}";
}

public static class Accessors
{
    [ILAccessor(ILAccessorKind.Field, Name = "_value")]
    public static extern ref int Value(TestModel instance);

    [ILAccessor(ILAccessorKind.StaticField, Name = "_staticValue")]
    public static extern ref int StaticValue(TestModel instance);

    [ILAccessor(ILAccessorKind.Method, Name = "GetMessage")]
    public static extern string GetMessage(TestModel instance, int code);

    [ILAccessor(ILAccessorKind.StaticMethod, Name = "GetStaticMessage")]
    public static extern string GetStaticMessage(TestModel? instance, int code);

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel NewTestModel(int x);
}

internal class Program
{
    private static void Main(string[] args)
    {
        var model = Accessors.NewTestModel(100);
        ref var value = ref Accessors.Value(model);
        Console.WriteLine($"_value: {value}");

        value += 50;
        Console.WriteLine($"_value updated: {value}");

        ref var staticValue = ref Accessors.StaticValue(model);
        Console.WriteLine($"_staticValue: {staticValue}");
        staticValue += 10;
        Console.WriteLine($"_staticValue updated: {staticValue}");

        var message = Accessors.GetMessage(model, 7);
        Console.WriteLine($"GetMessage: {message}");

        var staticMessage = Accessors.GetStaticMessage(null, 7);
        Console.WriteLine($"GetStaticMessage: {message}");

        Console.Read();
    }
}
```

---

## ⚖️ Comparison

| Feature | Reflection | `UnsafeAccessor` (.NET 8) | ILAccess.Fody |
|---|---:|---:|---:|
| Performance | Slow ❌ | Fast ✅ | Fast ✅ |
| Works before .NET 8 | ✅ | ❌ | ✅ |
| Compile-time validation | ❌ | ✅ | ✅ |

---

## 📄 License

MIT License — see [LICENSE](LICENSE) for details.


