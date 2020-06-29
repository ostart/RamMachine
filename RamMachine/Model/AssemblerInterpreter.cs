using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RamMachine.Model
{
    public class AssemblerInterpreter : IAssemblerInterpreter
    {
        public void Interpret(RamMachine ramMachine)
        {
            var currentCommand = ramMachine.Commands[ramMachine.CommandPointer];
            ramMachine.CommandPointer += 1;
            if (currentCommand.StartsWith("WRITE", true, CultureInfo.InvariantCulture))
            {
                ramMachine.RaisesOutputWriteOccuredEvent();
            }
            else if(currentCommand.StartsWith("READ", true, CultureInfo.InvariantCulture))
            {
                try
                {
                    var value = sbyte.Parse(ramMachine.Data[ramMachine.DataPointer]);
                    ramMachine.Registers[0] = value;
                    ramMachine.DataPointer += 1;
                }
                catch (Exception)
                {
                    ramMachine.Registers[0] = -128;
                }
            }
            else if (currentCommand.StartsWith("LOAD", true, CultureInfo.InvariantCulture))
            {
                var strValues = currentCommand.Substring(4).Trim();
                var arrValues = strValues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

                var value = GetValue(arrValues[0], ramMachine.Registers);
                var address = GetAddress(arrValues[1], ramMachine.Registers);
                ramMachine.Registers[address] = value;
            }
            else if (currentCommand.StartsWith("ADD", true, CultureInfo.InvariantCulture))
            {
                var strValues = currentCommand.Substring(3).Trim();
                var arrValues = strValues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

                var value1 = GetValue(arrValues[0], ramMachine.Registers);
                var value2 = GetValue(arrValues[1], ramMachine.Registers);
                var address = GetAddress(arrValues[2], ramMachine.Registers);
                ramMachine.Registers[address] = (sbyte) (value1 + value2);
            }
            else if (currentCommand.StartsWith("SUB", true, CultureInfo.InvariantCulture))
            {
                var strValues = currentCommand.Substring(3).Trim();
                var arrValues = strValues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

                var value1 = GetValue(arrValues[0], ramMachine.Registers);
                var value2 = GetValue(arrValues[1], ramMachine.Registers);
                var address = GetAddress(arrValues[2], ramMachine.Registers);
                var diff = value1 - value2;
                var result = diff > 127 ? 127 : diff;
                result = result < -128 ? -128 : result;
                ramMachine.Registers[address] = (sbyte)result;
            }
            else if (currentCommand.StartsWith("CPY", true, CultureInfo.InvariantCulture))
            {
                var strValues = currentCommand.Substring(3).Trim();
                var arrValues = strValues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

                var address1 = GetAddress(arrValues[0], ramMachine.Registers);
                var address2 = GetAddress(arrValues[1], ramMachine.Registers);
                ramMachine.Registers[address2] = ramMachine.Registers[address1];
            }
            else if (currentCommand.StartsWith("JNZ", true, CultureInfo.InvariantCulture))
            {
                var strValues = currentCommand.Substring(3).Trim();
                var arrValues = strValues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

                var value = GetValue(arrValues[0], ramMachine.Registers);
                var label = arrValues[1];
                if (value > 0)
                {
                    for (var i = 0; i < ramMachine.Commands.Count; i++)
                    {
                        if (ramMachine.Commands[i].StartsWith($"{label}:"))
                        {
                            if (ramMachine.SnapshotRegisters.Keys.OrderBy(x => x).SequenceEqual(ramMachine.Registers.Keys.OrderBy(x => x)) &&
                                ramMachine.SnapshotRegisters.OrderBy(kvp => kvp.Key).SequenceEqual(ramMachine.Registers.OrderBy(kvp => kvp.Key)) &&
                                label == ramMachine.LastLabel)
                            {
                                throw new Exception("Deadlock");
                            }

                            ramMachine.CommandPointer = i;
                            ramMachine.LastLabel = label;
                            ramMachine.SnapshotRegisters = ramMachine.Registers.ToDictionary(entry => entry.Key, entry => entry.Value);
                        }
                    }
                }
            }
            else if (currentCommand.StartsWith("HALT", true, CultureInfo.InvariantCulture))
            {
                ramMachine.CommandPointer -= 1;
            }
        }

        private static int GetAddress(string value, Dictionary<int, sbyte> registers)
        {
            if(value.StartsWith("[[") && value.EndsWith("]]"))
            {
                var val2 = value.Substring(2);
                var addressOfAddress = int.Parse(val2.Substring(0, val2.Length - 2));
                var address = registers[addressOfAddress];
                return address;
            }
            if (value.StartsWith("[") && value.EndsWith("]"))
            {
                var val1 = value.Substring(1);
                var address = int.Parse(val1.Substring(0, val1.Length - 1));
                return address;
            }
            throw new NotImplementedException();
        }

        private static sbyte GetValue(string value, Dictionary<int, sbyte> registers)
        {
            if (value.StartsWith("[[") && value.EndsWith("]]"))
            {
                var val2 = value.Substring(2);
                var addressOfAddress = int.Parse(val2.Substring(0, val2.Length - 2));
                var address = registers[addressOfAddress];
                return registers[address];
            }

            if (value.StartsWith("[") && value.EndsWith("]"))
            {
                var val1 = value.Substring(1);
                var address = int.Parse(val1.Substring(0, val1.Length - 1));
                return registers[address];
            }

            return sbyte.Parse(value);
        }
    }
}
