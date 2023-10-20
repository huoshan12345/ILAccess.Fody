# ILAccess.Fody

[![Build](https://github.com/huoshan12345/ILAccess.Fody/workflows/Build/badge.svg)](https://github.com/huoshan12345/ILAccess.Fody/actions?query=workflow%3ABuild)
[![NuGet package](https://img.shields.io/nuget/v/ILAccess.Fody.svg?logo=NuGet)](https://www.nuget.org/packages/ILAccess.Fody)
[![.net](https://img.shields.io/badge/.net%20standard-2.0-ff69b4.svg?)](https://www.microsoft.com/net/download)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/huoshan12345/ILAccess.Fody/blob/master/LICENSE)  
![Icon](https://raw.githubusercontent.com/huoshan12345/ILAccess.Fody/master/icon.png)

This is an add-in for [Fody](https://github.com/Fody/Fody) 
---
 - [Installation](#installation)
 - [Usage](#usage)
 - [Example](#examples) 
---

## Installation
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

## Usage


## Examples

