using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace FF3LE
{
    public static class ByteManage
    {
        public static byte GetByte(byte[] data, int offset)
        {
            try
            {
                return data[offset];
            }
            catch
            {
                MessageBox.Show("Error reading from byte[] data at offset " + offset.ToString("X6") + " \n data size: " + data.Length.ToString("X6") + "\n Please report this", Model.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception();
            }
        }
        public static ushort GetShort(byte[] data, int offset)
        {
            ushort ret = 0;
            try
            {
                ret += (ushort)(data[offset + 1] * 0x100);
                ret += (ushort)(data[offset]);
            }

            catch
            {
                MessageBox.Show("Error reading short from byte[] data at offset " + offset.ToString("X6") + " \n data size: " + data.Length.ToString("X6"), Model.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception();
            }
            return ret;
        }
        public static int GetInt(byte[] data, int offset)
        {
            int ret = 0;

            try
            {
                ret += (int)(data[offset + 2] * 0x10000);
                ret += (int)(data[offset + 1] * 0x100);
                ret += (int)(data[offset]);
            }

            catch
            {
                MessageBox.Show("Error reading short from byte[] data at offset " + offset.ToString("X6") + " \n data size: " + data.Length.ToString("X6"), Model.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception();
            }
            return ret;
        }
        public static bool GetBit(byte[] data, int offset, int bit)
        {
            try
            {
                if ((data[offset] & (byte)(Math.Pow(2, bit) - 1)) == (byte)(Math.Pow(2, bit) - 1))
                    return true;
                return false;
            }
            catch
            {
                MessageBox.Show("GetBit error reading from byte[] data at offset " + offset.ToString("X6") + " \n Data size: " + data.Length.ToString("X6") + "\n Please report this", Model.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception();
            }
        }
        public static bool GetBit(byte data, int bit)
        {
            try
            {
                if ((data & (byte)(Math.Pow(2, bit) - 1)) == (byte)(Math.Pow(2, bit) - 1))
                    return true;
                return false;
            }
            catch
            {
                MessageBox.Show("GetBit error reading from byte[] data. Please report this", Model.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception();
            }
        }
        public static byte[] GetByteArray(byte[] data, int offset, int size)
        {
            byte[] toGet = new byte[size];

            try
            {
                for (int i = 0; i < size; i++)
                {
                    toGet[i] = data[offset + i];
                }
                return toGet;
            }

            catch
            {
                MessageBox.Show("Error Getting byte[] at " + offset + "\ndata size: " + data.Length + "\nsubarray size: " + toGet.Length + "\nPlease report this", "Error", MessageBoxButtons.OK);
                throw new Exception();
            }
        }

        public static void SetByte(byte[] data, int offset, byte set)
        {
            try
            {
                data[offset] = set;
            }

            catch
            {
                MessageBox.Show("Error Writing byte: " + set + " to byte[] data at offset " + offset + " \n data size: " + data.Length + "\n Please report this", "Error", MessageBoxButtons.OK);
                throw new Exception();
            }

        }
        public static void SetByteBits(byte[] data, int offset, byte set, byte check)
        {
            // "check" are the bits to set exclusively
            try
            {
                // clear the bits to set
                data[offset] &= (byte)(check ^ 0xFF);

                // set the byte bits
                data[offset] |= (byte)set;
            }

            catch
            {

            }

        }
        public static void SetShort(byte[] data, int offset, ushort set)
        {
            try
            {
                data[offset] = (byte)(set & 0xff);
                data[offset + 1] = (byte)(set >> 8);
            }

            catch
            {
                MessageBox.Show("Error Writing short: " + set + " to byte[] data at offset " + offset + " \n data size: " + data.Length, "Error", MessageBoxButtons.OK);
                throw new Exception();
            }

        }
        public static void SetShortBits(byte[] data, int offset, ushort set, ushort check)
        {
            // "check" are the bits to set exclusively
            try
            {
                // clear the bits to set
                data[offset] &= (byte)(check ^ 0xFF);
                data[offset + 1] &= (byte)((check >> 8) ^ 0xFF);

                // set the ushort bits
                data[offset] |= (byte)set;
                data[offset + 1] |= (byte)(set >> 8);
            }

            catch
            {

            }

        }
        public static void SetBit(byte[] data, int offset, int bit, bool value)
        {
            try
            {
                if (value)
                    data[offset] |= (byte)(Math.Pow(2, bit));
                else if (!value)
                    data[offset] &= (byte)((byte)(Math.Pow(2, bit)) ^ 0xFF);
            }
            catch
            {
                MessageBox.Show("SetBit error reading from byte[] data at offset " + offset + " \n Data size: " + data.Length + "\n Please report this", "Error", MessageBoxButtons.OK);
                throw new Exception();
            }
        }

        public static void SetByteArray(byte[] data, int offset, byte[] toSet)
        {
            try
            {
                for (int i = 0; i < toSet.Length; i++)
                {
                    data[offset + i] = toSet[i];
                }
            }

            catch
            {
                MessageBox.Show("Error Setting byte[] at " + offset + "\ndata size: " + data.Length + "\nsubarray size: " + toSet.Length + "\nPlease report this", "Error", MessageBoxButtons.OK);
                throw new Exception();
            }

        }
        public static void SetByteArray(byte[] data, int offset, byte[] toSet, int copyStart, int copyEnd)
        {
            try
            {
                for (int i = copyStart; i < toSet.Length && i <= copyEnd; i++)
                {
                    data[offset + i] = toSet[i];
                }
            }

            catch
            {
                MessageBox.Show("Error Setting byte[] at " + offset + "\ndata size: " + data.Length + "\nsubarray size: " + toSet.Length + "\nPlease report this", "Error", MessageBoxButtons.OK);
                throw new Exception();
            }

        }
    }
}
