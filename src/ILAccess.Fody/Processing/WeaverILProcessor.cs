namespace ILAccess.Fody.Processing;

internal sealed class WeaverILProcessor
{
    private readonly ILProcessor _il;
    private readonly HashSet<Instruction> _referencedInstructions;

    public MethodDefinition Method { get; }
    public MethodLocals Locals { get; }

    public WeaverILProcessor(MethodDefinition method)
    {
        Method = method;
        Locals = new MethodLocals(method);
        _il = method.Body.GetILProcessor();
        _referencedInstructions = GetAllReferencedInstructions();
    }

    public void Remove(Instruction instruction)
    {
        var newRef = instruction.Next ?? instruction.Previous ?? throw new InstructionWeavingException(instruction, "Cannot remove single instruction of method");
        _il.Remove(instruction);
        UpdateReferences(instruction, newRef);
    }

    public HashSet<Instruction> GetAllReferencedInstructions()
    {
        var refs = new HashSet<Instruction>(ReferenceEqualityComparer<Instruction>.Instance);

        if (_il.Body.HasExceptionHandlers)
        {
            foreach (var handler in _il.Body.ExceptionHandlers)
                refs.UnionWith(handler.GetInstructions());
        }

        foreach (var instruction in _il.Body.Instructions)
        {
            switch (instruction.Operand)
            {
                case Instruction target:
                    refs.Add(target);
                    break;

                case Instruction[] targets:
                    refs.UnionWith(targets.Where(t => t != null));
                    break;
            }
        }

        return refs;
    }
        
    private void UpdateReferences(Instruction oldInstruction, Instruction newInstruction)
    {
        if (!_referencedInstructions.Contains(oldInstruction))
            return;

        if (_il.Body.HasExceptionHandlers)
        {
            foreach (var handler in _il.Body.ExceptionHandlers)
            {
                if (handler.TryStart == oldInstruction)
                    handler.TryStart = newInstruction;

                if (handler.TryEnd == oldInstruction)
                    handler.TryEnd = newInstruction;

                if (handler.FilterStart == oldInstruction)
                    handler.FilterStart = newInstruction;

                if (handler.HandlerStart == oldInstruction)
                    handler.HandlerStart = newInstruction;

                if (handler.HandlerEnd == oldInstruction)
                    handler.HandlerEnd = newInstruction;
            }
        }

        foreach (var instruction in _il.Body.Instructions)
        {
            switch (instruction.Operand)
            {
                case Instruction target when target == oldInstruction:
                    instruction.Operand = newInstruction;
                    break;

                case Instruction[] targets:
                    for (var i = 0; i < targets.Length; ++i)
                    {
                        if (targets[i] == oldInstruction)
                            targets[i] = newInstruction;
                    }

                    break;
            }
        }

        _referencedInstructions.Remove(oldInstruction);
        _referencedInstructions.Add(newInstruction);
    }

    private static WeavingException ExceptionInvalidOperand(OpCode opCode)
    {
        switch (opCode.OperandType)
        {
            case OperandType.InlineNone:
                return new WeavingException($"Opcode {opCode} does not expect an operand");

            case OperandType.InlineBrTarget:
            case OperandType.ShortInlineBrTarget:
                return ExpectedOperand("label name");

            case OperandType.InlineField:
                return ExpectedOperand(KnownNames.Short.FieldRefType);

            case OperandType.InlineI:
            case OperandType.ShortInlineI:
            case OperandType.InlineArg:
            case OperandType.ShortInlineArg:
                return ExpectedOperand(nameof(Int32));

            case OperandType.InlineI8:
                return ExpectedOperand(nameof(Int64));

            case OperandType.InlineMethod:
                return ExpectedOperand(KnownNames.Short.MethodRefType);

            case OperandType.InlineR:
            case OperandType.ShortInlineR:
                return ExpectedOperand(nameof(Double));

            case OperandType.InlineSig:
                return ExpectedOperand(KnownNames.Short.StandAloneMethodSigType);

            case OperandType.InlineString:
                return ExpectedOperand(nameof(String));

            case OperandType.InlineSwitch:
                return ExpectedOperand("array of label names");

            case OperandType.InlineType:
                return ExpectedOperand($"{KnownNames.Short.TypeRefType} or {nameof(Type)}");

            case OperandType.InlineVar:
            case OperandType.ShortInlineVar:
                return ExpectedOperand("local variable name or index");

            default:
                return ExpectedOperand(opCode.OperandType.ToString());
        }

        WeavingException ExpectedOperand(string expected) => new($"Opcode {opCode} expects an operand of type {expected}");
    }

    public Instruction InsertAfter(Instruction target, Instruction instruction)
    {
        _il.InsertAfter(target, instruction);
        return instruction;
    }

    public Instruction InsertAfter(Instruction target, IEnumerable<Instruction> instructions)
    {
        return instructions.Aggregate(target, (current, ins) => InsertAfter(current, ins));
    }
}
