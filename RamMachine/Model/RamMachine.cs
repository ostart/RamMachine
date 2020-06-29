using System;
using System.Collections.Generic;
using System.Linq;

namespace RamMachine.Model
{
    public class RamMachine
    {
        public readonly Dictionary<int,sbyte> Registers = new Dictionary<int, sbyte>();
        public string LastLabel = null;
        public Dictionary<int, sbyte> SnapshotRegisters = new Dictionary<int, sbyte>();
        public int CommandPointer = 0;
        public int DataPointer = 0;
        public List<string> Commands = new List<string>();
        public List<string> Data = new List<string>();
        private readonly IAssemblerInterpreter _interpreter = new AssemblerInterpreter();

        public event EventHandler<OutputEventArgs> OutputWriteOccured;

        public void RaisesOutputWriteOccuredEvent()
        {
            OutputWriteOccured?.Invoke(this, new OutputEventArgs());
        }

        public void NextStep()
        {
            _interpreter.Interpret(this);
        }

        public void SetState(string commands, string data, bool anew = false)
        {
            Commands = Parse(commands);
            Data = Parse(data);
            if (anew)
            {
                CommandPointer = 0;
                DataPointer = 0;
                SnapshotRegisters = new Dictionary<int, sbyte>();
                LastLabel = null;
            }
        }

        public string GetCurrentExecutedCommand()
        {
            return Commands[CommandPointer];
        }

        public string GetAdder()
        {
            return Registers[0].ToString();
        }

        private static List<string> Parse(string allLines)
        {
            var lines = allLines.Replace("\r", "").Split('\n');
            return lines.Where(line => !string.IsNullOrEmpty(line)).ToList();
        }
    }
}
