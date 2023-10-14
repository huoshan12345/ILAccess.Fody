namespace System.Runtime.CompilerServices

open System

/// <summary>
/// Allows the current assembly to access the internal types of a specified assembly that are ordinarily invisible.
/// </summary>
[<AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)>]
type internal IgnoresAccessChecksToAttribute(assemblyName: string) =
    inherit Attribute()
    member val AssemblyName: string = assemblyName
