using System;

namespace RamMachine.Model
{
    internal interface IAssemblerInterpreter
    {
        //void Interpret(List<string> commands, ref int commandPointer, List<string> data, ref int dataPointer, Dictionary<int, sbyte> registers, EventHandler<OutputEventArgs> outputOccured);
        void Interpret(RamMachine ramMachine);
    }
}
