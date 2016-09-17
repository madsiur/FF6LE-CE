using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace FF3LE
{
    public class LCDecomp
    {
        byte[] data;

        public LCDecomp(byte[] data)
        {
            this.data = data;
        }

        public int Compress(byte[] source, byte[] dest)
        {
            byte[] buf = new byte[0x12000];
            byte[] b = new byte[16];
            byte p, n, run, bp;
            int maxx = 0, maxrun, x, w, start;
            ulong bpos, bpos2, fsize, size;

            fsize = (ulong)dest.Length;

            source.CopyTo(buf, 0);

            int pTempPtr = 2;

            size = 0;
            bpos = 0; bp = 0;
            bpos2 = 2014;
            n = 0; p = 0;
            while (bpos < fsize)
            {
                maxrun = 0;
                if (bpos < 2048)
                    start = (int)bpos;
                else
                    start = 2048;
                for (x = 1; x <= start; x++)
                {
                    run = 0;
                    while ((run < 31 + 3) &&
                        (buf[bpos - (ulong)x + run] == buf[bpos + run]) &&
                        (bpos + run < fsize))
                    {
                        run++;
                    }

                    if (run > maxrun)
                    {
                        maxrun = run;
                        maxx = (int)((bpos2 - (ulong)x) & 2047);
                    }
                }
                if (maxrun >= 3)
                {
                    w = ((maxrun - 3) << 11) + maxx;
                    b[bp] = (byte)(w & 255);
                    b[bp + 1] = (byte)(w >> 8);
                    bp += 2;
                    bpos += (ulong)maxrun;
                    bpos2 = (bpos2 + (ulong)maxrun) & 2047;
                }
                else
                {
                    n = (byte)(n | (1 << p));
                    b[bp] = buf[bpos];
                    bp++; bpos++;
                    bpos2 = (bpos2 + 1) & 2047;
                }
                p = (byte)((p + 1) & 7);
                if (p == 0)
                {
                    dest[pTempPtr++] = n;
                    for (int tc = 0; tc < bp; tc++)
                        dest[pTempPtr++] = b[tc];

                    size += (ulong)(bp + 1);
                    n = 0; bp = 0;
                }
            }
            if (p != 0)
            {
                dest[pTempPtr++] = n;
                for (int tc = 0; tc < bp; tc++)
                    dest[pTempPtr++] = b[tc];

                size += (ulong)(bp + 1);
                n = 0; bp = 0;
            }

            size += 2;

            ByteManage.SetShort(dest, 0, (ushort)size);

            return (int)size;
        }

        //madsiur experimental
        public int CompressT(byte[] source, byte[] dest)
        {
            byte[] buf = new byte[0x12000];
            byte[] b = new byte[16];
            byte p, n, run, bp;
            int maxx = 0, maxrun, x, w, start;
            ulong bpos, bpos2, fsize, size;

            fsize = (ulong)dest.Length;

            source.CopyTo(buf, 0);

            int pTempPtr = 2;

            size = 0;
            bpos = 0; bp = 0;
            bpos2 = 2014;
            n = 0; p = 0;
            while (bpos < fsize)
            {
                maxrun = 0;
                if (bpos < 2048)
                    start = (int)bpos;
                else
                    start = 2048;
                for (x = 1; x <= start; x++)
                {
                    run = 0;
                    while ((run < 31 + 3) &&
                        (buf[bpos - (ulong)x + run] == buf[bpos + run]) &&
                        (bpos + run < fsize))
                    {
                        run++;
                    }

                    if (run > maxrun)
                    {
                        maxrun = run;
                        maxx = (int)((bpos2 - (ulong)x) & 2047);
                    }
                }
                if (maxrun >= 3)
                {
                    w = ((maxrun - 3) << 11) + maxx;
                    if(bp < 16)
                        b[bp] = (byte)(w & 255);
                    if (bp < 15)
                        b[bp + 1] = (byte)(w >> 8);
                    bp += 2;
                    bpos += (ulong)maxrun;
                    bpos2 = (bpos2 + (ulong)maxrun) & 2047;
                }
                else
                {
                    n = (byte)(n | (1 << p));
                    if (bp < 16)
                        b[bp] = buf[bpos];
                    bp++; bpos++;
                    bpos2 = (bpos2 + 1) & 2047;
                }
                p = (byte)((p + 1) & 7);
                if (p == 0)
                {
                    if (pTempPtr < dest.Length - 1)
                    {
                        dest[pTempPtr++] = n;
                        for (int tc = 0; tc < bp; tc++)
                        {
                            if (pTempPtr < dest.Length)
                                dest[pTempPtr++] = b[tc];
                        }

                        size += (ulong)(bp + 1);
                        n = 0; bp = 0;
                    }
                }
            }
            if (p != 0)
            { 
                if (pTempPtr < dest.Length - 1)
                {
                    dest[pTempPtr++] = n;
                    for (int tc = 0; tc < bp; tc++)
                    {
                        if (pTempPtr < dest.Length)
                            dest[pTempPtr++] = b[tc];
                    }

                    size += (ulong)(bp + 1);
                    n = 0; bp = 0;
                }
            }

            size += 2;

            if (dest.Length < 2 && size == 2)
                dest = new byte[] { 0x00, 0x00 };

            ByteManage.SetShort(dest, 0, (ushort)size);

            return (int)size;
        }
        public byte[] Decompress(int offset, int length)
        {
            byte[] temp = new byte[0x100000];		// 1MB should be enough to hold any decompressed data in a 3MB ROM.
            int tempPtr = 0;

            byte[] buf = new byte[0x12000];
            byte[] buf2 = new byte[2048];
            byte n, x, b;
            uint size, w, num, i;
            ulong bpos, bpos2;
            uint finalCount = 0;

            size = ByteManage.GetShort(data, offset); offset += 2;

            bpos = 0; bpos2 = 2014;

            do
            {
                n = data[bpos + (ulong)offset]; bpos++;

                for (x = 0; x < 8; x++)
                {
                    if (((n >> x) & 1) == 1)
                    {
                        b = data[bpos + (ulong)offset]; bpos++;
                        temp[tempPtr++] = b;
                        finalCount++;
                        buf2[bpos2 & 2047] = b; bpos2++;
                    }
                    else
                    {
                        w = (uint)(data[bpos + (ulong)offset] + (data[bpos + 1 + (ulong)offset] << 8));
                        bpos += 2;
                        num = (w >> 11) + 3;
                        w = w & 2047;

                        for (i = 0; i < num; i++)
                        {
                            b = buf2[(w + i) & 2047];
                            temp[tempPtr++] = b;
                            finalCount++;
                            buf2[bpos2 & 2047] = b; bpos2++;
                        }
                    }
                    if (bpos >= size)
                        x = 8;
                }
            }
            while (bpos < size);

            byte[] dest = new byte[length];
            Buffer.BlockCopy(temp, 0, dest, 0, length);

            return dest;
        }
        public byte[] Decompress(int offset, int length, ref ushort totalSize)
        {
            byte[] temp = new byte[0x100000];		// 1MB should be enough to hold any decompressed data in a 3MB ROM.
            int tempPtr = 0;

            byte[] buf = new byte[0x12000];
            byte[] buf2 = new byte[2048];
            byte n, x, b;
            uint size, w, num, i;
            ulong bpos, bpos2;
            uint finalCount = 0;

            size = ByteManage.GetShort(data, offset); offset += 2;

            bpos = 0; bpos2 = 2014;

            do
            {
                n = data[bpos + (ulong)offset]; bpos++;

                for (x = 0; x < 8; x++)
                {
                    if (((n >> x) & 1) == 1)
                    {
                        b = data[bpos + (ulong)offset]; bpos++;
                        temp[tempPtr++] = b;
                        finalCount++;
                        buf2[bpos2 & 2047] = b; bpos2++;
                    }
                    else
                    {
                        w = (uint)(data[bpos + (ulong)offset] + (data[bpos + 1 + (ulong)offset] << 8));
                        bpos += 2;
                        num = (w >> 11) + 3;
                        w = w & 2047;

                        for (i = 0; i < num; i++)
                        {
                            b = buf2[(w + i) & 2047];
                            temp[tempPtr++] = b;
                            finalCount++;
                            buf2[bpos2 & 2047] = b; bpos2++;
                        }
                    }
                    if (bpos >= size)
                        x = 8;
                }
            }
            while (bpos < size);

            byte[] dest = new byte[length];
            Buffer.BlockCopy(temp, 0, dest, 0, length);

            totalSize = (ushort)(finalCount & 0xFF00);
            return dest;
        }

        public int CompressData(byte[] source, ref byte[] dest)
        {
            FileStream fs;
            BinaryWriter bw;
            BinaryReader br;

            fs = new FileStream("decomp.bin", FileMode.Create, FileAccess.ReadWrite);
            bw = new BinaryWriter(fs);
            bw.Write(source);
            bw.Close();

            System.Diagnostics.Process proc = new System.Diagnostics.Process();

            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = "COMPRESS.BAT";
            //proc.StartInfo.Arguments = "compress c decomp.bin";
            proc.Start();

            fs = File.OpenRead("comp.bin");
            br = new BinaryReader(fs);

            dest = br.ReadBytes((int)fs.Length);

            return (int)fs.Length;
        }
        public byte[] DecompressData(int offset, int length)
        {
            //first two bytes of the compressed data indicate its length
            int len = data[offset] + (data[offset + 1] << 8);
            byte[] rom = new byte[length];
            int i, j, k, l;
            int flags, foo, count, loc;
            i = 0;
            j = 2;
            k = 8;
            flags = 0;
            while (i < length && j < len)
            {
                if (k == 8)
                {
                    flags = data[offset + j++];
                    k = 0;
                }
                else if ((flags & (1 << k)) != 0)
                { //uncompressed byte
                    rom[i++] = data[offset + j++];
                    ++k;
                }
                else
                { //compressed byte
                    foo = (data[offset + j] + (data[offset + j + 1] << 8));
                    j += 2;
                    count = ((foo & 0xF800) >> 11) + 3;
                    loc = (foo & 0x7FF);
                    loc -= 0x7DE;
                    while (loc + 0x800 <= i)
                    {
                        loc += 0x800;
                    }
                    for (l = 0; l < count; l++)
                    {
                        if (i >= length)
                            break;
                        if (loc < length && loc >= 0)
                            rom[i++] = rom[loc];
                        else
                            rom[i++] = 0;

                        ++loc;
                    }
                    ++k;
                }
            }
            return rom;
        }
    }
}
