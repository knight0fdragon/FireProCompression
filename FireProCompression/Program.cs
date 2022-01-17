using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FireProCompression
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Error, no files to decompress");
            }

            //Decompress only
            foreach (var arg in args)
            {
                var output = new List<byte>();
                var data = File.ReadAllBytes(arg);
                //Date is big endian, so we need to convert it
                data = data.Reverse().ToArray();
                var data16 = new ushort[data.Length / 2];
                //This will convert the bytes to a short
                Buffer.BlockCopy(data, 0, data16, 0, data.Length);
                data16 = data16.Reverse().ToArray();
                //Data is now short but converted to big endian, just need to go back to byte
                Buffer.BlockCopy(data16, 0, data, 0, data.Length);

                using var ms = new MemoryStream(data);
                using var br = new BinaryReader(ms);

                while (ms.Position < ms.Length)
                {



                    var repeatCount = br.ReadUInt16();
                    var valueToRepeat = br.ReadUInt16();
                    var rawReadCount = br.ReadUInt16();

                    for (var i = 0; i < repeatCount; i++)
                    {
                        output.Add((byte)(valueToRepeat >> 8));
                        output.Add((byte)(valueToRepeat & 0xFF));

                    }
                    for (var i = rawReadCount; i < 0; i++)
                    {
                        var read = br.ReadUInt16();
                        output.Add((byte)( read >> 8));
                        output.Add((byte)(read & 0xFF));
                    }
                    File.WriteAllBytes(arg + ".out", output.ToArray());
                }
            }
        }
    }
}
