using Fody;
using Mono.Cecil.Cil;

namespace ILAccess.Fody.Support;

internal sealed class InstructionWeavingException : WeavingException
{
    public Instruction? Instruction { get; }

    public InstructionWeavingException(Instruction? instruction, string message)
        : base(message)
    {
        Instruction = instruction;
    }
}
