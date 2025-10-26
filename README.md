# ILAccess.Fody 
[![Build](https://github.com/huoshan12345/ILAccess.Fody/workflows/Build/badge.svg)](https://github.com/huoshan12345/ILAccess.Fody/actions?query=workflow%3ABuild)
[![NuGet package](https://img.shields.io/nuget/v/ILAccess.Fody.svg?logo=NuGet)](https://www.nuget.org/packages/ILAccess.Fody)
[![.net](https://img.shields.io/badge/.net%20standard-2.0-ff69b4.svg?)](https://www.microsoft.com/net/download)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/huoshan12345/ILAccess.Fody/blob/master/LICENSE)  

## ✨ Overview

`ILAccess.Fody` provides functionality similar to .NET 8's `UnsafeAccessor`, but works on older .NET platforms. It is a [Fody](https://github.com/Fody/Fody) weaver that injects IL at compile-time, enabling access to private or internal members without runtime reflection. This results in faster access and compile-time safety compared to traditional reflection-based approaches.

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

You can use `ILAccessor` to access private fields, methods, or constructors — similar to `UnsafeAccessor` in .NET 8.

### Example: Access Private Field

```csharp
using System;
using ILAccess;

class Example
{
    private int _value = 42;
}

static class Accessors
{
    [ILAccessor(ILAccessorKind.Field, Name = "_value")]
    public static partial ref int GetValue(Example instance);
}

class Program
{
    static void Main()
    {
        var obj = new Example();
        ref int valueRef = ref Accessors.GetValue(obj);
        Console.WriteLine(valueRef);  // 42
        valueRef = 99;
        Console.WriteLine(Accessors.GetValue(obj));  // 99
    }
}
```

### Example: Access Private Method

```csharp
class Example
{
    private string GetSecretMessage(int code) => $"Secret #{code}";
}

static class Accessors
{
    [ILAccessor(ILAccessorKind.Method, Name = "GetSecretMessage")]
    public static partial string GetSecretMessage(Example instance, int code);
}

class Program
{
    static void Main()
    {
        var obj = new Example();
        Console.WriteLine(Accessors.GetSecretMessage(obj, 7)); // Secret #7
    }
}
```

### Example: Access Constructor

```csharp
class Example
{
    private Example(int x) { Value = x; }
    public int Value { get; }
}

static class Accessors
{
    [ILAccessor(ILAccessorKind.Constructor)]
    public static partial Example Create(int x);
}

class Program
{
    static void Main()
    {
        var obj = Accessors.Create(123);
        Console.WriteLine(obj.Value); // 123
    }
}
```

---

## ⚖️ Comparison

| Feature | Reflection | `UnsafeAccessor` (.NET 8) | ILAccess.Fody |
|---|---:|---:|---:|
| Performance | ❌ Slow | ✅ Fast | ✅ Fast |
| Works before .NET 8 | ✅ | ❌ | ✅ |
| Compile-time validation | ❌ | ✅ | ✅ |

---

## 📄 License

MIT License — see [LICENSE](LICENSE) for details.


