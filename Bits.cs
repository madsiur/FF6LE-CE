using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FF3LE
{
    public static class Bits
    {
        private static void ShowError(int offset, int length)
        {
            throw new Exception("Error accessing data at $" + offset.ToString("X6") + " in " + length.ToString("X6") + " byte array.");
        }

        public static bool IsValidFilePath(string path)
        {
            string pattern = @"^(([a-zA-Z]:)|((\\|/){1,2}\w+)\$?)((\\|/)(\w[\w ]*.*))+\.([bin]+)$";

            return Regex.IsMatch(path, pattern, RegexOptions.CultureInvariant);
        }

        public static bool IsValidMapId(string id)
        {
            string pattern = @"^\[\$[0-1][A-F0-9]{2}]$";

            return Regex.IsMatch(id, pattern, RegexOptions.CultureInvariant);
        }

        public static bool IsValidMapName(string name)
        {
            string pattern =
                @"[A-Za-z0-9]|\x20|\*|!|\?|:|\\|'|-|\.|,|\.\.\.|;|#|\+|\(|\)|%|~|@|=|<TERRA>|<LOCKE>|<CYAN>|<SHADOW>|<EDGAR>|<SABIN>|<CELES>|<STRAGO>|<RELM>|<SETZER>|<MOG>|<GAU>|<GOGO>|<UMARO>|<note>|<pearl>|<death>|<lit>|<wind>|<earth>|<ice>|<fire>|<water>|<poison>";

            return Regex.IsMatch(name, pattern, RegexOptions.CultureInvariant);
        }

        public static MatchCollection GetMatchCollection(string message)
        {
            string pattern =
                @"<TERRA>|<LOCKE>|<CYAN>|<SHADOW>|<EDGAR>|<SABIN>|<CELES>|<STRAGO>|<RELM>|<SETZER>|<MOG>|<GAU>|<GOGO>|<UMARO>|<note>|74|75|<pearl>|<death>|<lit>|<wind>|<earth>|<ice>|<fire>|<water>|<poison>|e\x20|\x20t|:\x20|th|t\x20|he|s\x20|er|\x20a|re|in|ou|d\x20|\x20w|\x20s|an|o\x20|\x20h|\x20o|r\x20|n\x20|at|to|\x20i|,\x20|ve|ng|ha|\x20m|Th|st|on|yo|\x20b|me|\x20y|y\x20|en|it|ar|ll|ea|I\x20|ed|\x20f|hi|is|es|or|l\x20|\x20c|ne|'s|nd|le|se|\x20I|a\x20|te|\x20l|pe|as|ur|u\x20|al|\x20p|g\x20|om|\x20d|f\x20|\x20g|ow|rs|be|ro|us|ri|wa|we|Wh|et|\x20r|nt|m\x20|ma|I'|li|ho|of|Yo|h\x20|\x20n|ee|de|so|gh|ca|ra|n'|ta|ut|el|!\x20|fo|ti|We|lo|e!|ld|no|ac|ce|k\x20|\x20u|oo|ke|ay|w\x20|!!|ag|il|ly|co|\.\x20|ch|go|ge";

            return Regex.Matches(message, pattern);
        }

        public static string AddMapId(int id, string mapName)
        {
            return "[$" + id.ToString("X3") + "] " + mapName;
        }
        public static void SetInt(byte[] data, int offset, int value)
        {
            data[offset++] = (byte)(value & 0xFF);
            data[offset++] = (byte)((value >> 8) & 0xFF);
            data[offset] = (byte)((value >> 16) & 0xFF);
        }

        public static void Fill(byte[] src, byte value, int start, int size)
        {
            for (int i = start; i < size + start; i++)
                src[i] = value;
        }

        public static byte ToHiROM(byte value)
        {
            if (value < 0x40)
                value += 0xC0;

            return value;
        }

        public static int ToHiROM(int value)
        {
            if (value < 0x400000)
                value += 0xC00000;

            return value;
        }

        public static byte ToAbs(byte value)
        {
            if (value >= 0xC0)
                value -= 0xC0;

            return value;
        }

        public static int ToAbs(int value)
        {
            if (value >= 0xC00000)
                value -= 0xC00000;

            return value;
        }

        public static bool IsValidBank(byte value)
        {
            return (value <= 0x6F) || (value >= 0xC0 && value <= 0xFF);
        }

        public static bool IsValidOffset(int value)
        {
            return (value <= 0x6FFFFF) || (value >= 0xC00000 && value <= 0xFFFFFF);
        }

        public static bool IsMatchingOffset(byte[] data, int offset, int offsetROM, ref List<int[]> faults)
        {
            int offsetB = ByteManage.GetInt(data, ToAbs(offsetROM) + 1);

            if (ToHiROM(offset) != offsetB)
            {
                faults.Add(new[] { offsetROM, ToHiROM(offset), offsetB });
                return false;
            }

            Log.SetEntry("IsMatchingOffset(true)");
            Log.SetEntry("IsMatchingOffset", "match", "offsetROM", offsetROM + 1);
            Log.SetEntry("IsMatchingOffset", "match", "offset", offset);
            return true;
        }

        public static bool IsMatchingShort(byte[] data, ushort val, int offsetROM, ref List<int[]> faults)
        {
            ushort valB = ByteManage.GetShort(data, ToAbs(offsetROM));

            if (val != valB)
            {
                faults.Add(new[] { offsetROM, val, valB });
                return false;
            }

            Log.SetEntry("IsMatchingShort(true)");
            Log.SetEntry("IsMatchingShort", "match", "offsetROM", offsetROM);
            Log.SetEntry("IsMatchingShort", "match", "offset", val);
            return true;
        }

        public static bool IsMatchingByte(byte[] data, byte val, int offsetROM, ref List<int[]> faults)
        {
            byte valB = data[ToAbs(offsetROM)];

            if (val != valB)
            {
                faults.Add(new[] { offsetROM, val, valB });
                return false;
            }

            Log.SetEntry("IsMatchingShort(true)");
            Log.SetEntry("IsMatchingShort", "match", "offsetROM", offsetROM);
            Log.SetEntry("IsMatchingShort", "match", "offset", val);
            return true;
        }

        public static int findArrayMax(byte[] data, int size)
        {
            if (size < 1 || size > 3)
                return 0;

            int max = 0;

            for (int i = 0; i < data.Length; i += size)
            {
                int current;

                if (size == 1)
                {
                    current = data[i];

                    if (current > max)
                        max = current;
                }
                else if (size == 2)
                {
                    current = ByteManage.GetShort(data, i);

                    if (current != 0xFFFF && current > max)
                        max = current;
                }
                else
                {
                    current = ByteManage.GetInt(data, i);

                    if (current != 0xFFFFFF && current > max)
                        max = current;
                }
            }

            Log.SetEntry("findArrayMax", "max found", "max", max);
            return max;
        }

        public static void FillShort(byte[] data, ushort val)
        {
            int i = 0;
            try
            {
                for (i = 0; i < data.Length; i += 2)
                {
                    data[i] = (byte)(val & 0xff);
                    data[i + 1] = (byte)(val >> 8);
                }
            }
            catch (Exception)
            {
                ShowError(i, data.Length);
            }
        }

        public static void IncShort(byte[] data, ushort inc)
        {
            int i = 0;

            try
            {
                for (i = 0; i < data.Length; i += 2)
                {
                    ushort val = 0;
                    val += (ushort)(data[i + 1] << 8);
                    val += (ushort)(data[i]);
                    val += inc;
                    data[i] = (byte)(val & 0xff);
                    data[i + 1] = (byte)(val >> 8);
                }
            }
            catch (Exception)
            {
                ShowError(i, data.Length);
                throw new Exception();
            }
        }

        public static void setAsmArray(byte[] data, int[] asmArray, int[] varArray, int offset)
        {
            if (asmArray.Length != varArray.Length)
            {
                throw new Exception("ASM and variation arrays are not equal: " + asmArray.Length + " - " + varArray.Length);
            }

            for (int i = 0; i < asmArray.Length; i++)
            {
                SetInt(data, ToAbs(asmArray[i]) + 1, offset + varArray[i]);
                Log.SetEntry("setAsmArray", "int", "ROM offset", ToAbs(asmArray[i]) + 1);
                Log.SetEntry("setAsmArray", "int", "value set", offset + varArray[i]);
            }
        }

        public static void setAsmArray(byte[] data, int[] asmArray, ushort val)
        {
            for (int i = 0; i < asmArray.Length; i++)
            {
                ByteManage.SetShort(data, ToAbs(asmArray[i]), val);
                Log.SetEntry("setAsmArray", "short", "ROM offset", ToAbs(asmArray[i]));
                Log.SetEntry("setAsmArray", "short", "value set", val);
            }
        }

        public static void setAsmArray(byte[] data, int[] asmArray, byte val)
        {
            for (int i = 0; i < asmArray.Length; i++)
            {
                data[ToAbs(asmArray[i])] = val;
                Log.SetEntry("setAsmArray", "byte", "ROM offset", ToAbs(asmArray[i]));
                Log.SetEntry("setAsmArray", "byte", "value set", val);
            }
        }

        public static void setData(byte[] dest, int offset, byte[] data, byte[] data2, byte[] data3)
        {
            ByteManage.SetByteArray(dest, offset, data);
            Log.SetEntry("setData", "SetBytes A", "offset", offset);
            Log.SetEntry("data", data.Length);
            offset += data.Length;
            ByteManage.SetByteArray(dest, offset, data2);
            Log.SetEntry("setData", "SetBytes B", "offset", offset);
            Log.SetEntry("data2", data2.Length);
            offset += data2.Length;
            ByteManage.SetByteArray(dest, offset, data3);
            Log.SetEntry("setData", "SetBytes C", "offset", offset);
            Log.SetEntry("data3", data3.Length);
        }
    }
}
