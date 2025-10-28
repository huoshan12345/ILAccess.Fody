using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILAccess.Tests.AssemblyToProcess;

/// <summary>
/// Marks a test method as a fake fact to help SourceGenerator to generate the actual test method.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class FakeFactAttribute : Attribute;
