/*
MIT License

Copyright (c) 2021 Árnilsen Arthur Castilho Lopes

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using PacketSystem;
using PacketSystem.Types;

namespace Test
{
    #region Main Class
    public class Program
    {
        /// <summary>
        /// Main test class
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            //Init packet system
            Packet.Init();

            #region Conversion Examples
            //Conversion example
            PacketTestFloat3 packet = new PacketTestFloat3(new float3(0, 0, 0), new float3(0, 0, 0), new float3(0, 0, 0));

            //Convert packet to bytes (3 ways)
            byte[] bta = (byte[])packet;
            byte[] btb = packet.Serialize();
            byte[] btc = packet.Bytes;

            //Convert bytes to packet (2 ways)
            PacketTestFloat3 pa = (PacketTestFloat3)bta;
            PacketTestFloat3 pb = Packet.Deserialize<PacketTestFloat3>(btb);
            #endregion

            /*
            #region Dict Test
            Dictionary<int,float> f = new Dictionary<int,float>();

            for (int i = 0; i < 100; i += 10)
            {
                PacketTestDictionary test = new PacketTestDictionary(f);
                PacketTestDictionarySerializable test2 = new PacketTestDictionarySerializable(f);
                
                byte[] a = null;
                byte[] b = null;

                Stopwatch st = Stopwatch.StartNew();
                for (int j = 0; j < 1000; j++)
                    a = test.Bytes;
                st.Stop();
                long a1 = st.ElapsedMilliseconds;

                st = Stopwatch.StartNew();
                for (int j = 0; j < 1000; j++)
                    b = ObjectToByteArray(test2);
                st.Stop();
                long b1 = st.ElapsedMilliseconds;

                st = Stopwatch.StartNew();
                for (int j = 0; j < 1000; j++)
                {
                    PacketTestDictionary pca = (PacketTestDictionary)a;
                }
                st.Stop();
                long a2 = st.ElapsedMilliseconds;

                st = Stopwatch.StartNew();
                for (int j = 0; j < 1000; j++)
                {
                    PacketTestDictionarySerializable pcb = FromByteArray<PacketTestDictionarySerializable>(b);
                }
                st.Stop();
                long b2 = st.ElapsedMilliseconds;

                Console.WriteLine("{0}: A({1}ms {2}ms {3}B) B({4}ms {5}ms {6}B)",i, a1, a2,a.Length,b1,b2,b.Length);
                
                for(int n = 0; n < 10; n ++)
                    f.Add(i + n,i + n * 2);

            }
            #endregion
            */

            #region Tests
            //Float3 test
            PacketTestFloat3 test = new PacketTestFloat3(new float3(1, 2, 3), new float3(4, 5, 6), new float3(7, 8, 9));
            PacketTestFloat3Serializable test2 = new PacketTestFloat3Serializable(new float3(1, 2, 3), new float3(4, 5, 6), new float3(7, 8, 9));
            Test("Float3", test, test2);

            //Bool test
            PacketTestBool test3 = new PacketTestBool(true, false, true, true, false, true, false, false);
            PacketTestBoolSerializable test4 = new PacketTestBoolSerializable(true, false, true, true, false, true, false, false);
            Test("Bool", test3, test4);

            //List test
            PacketTestList test5 = new PacketTestList(6, 7, 8, 9, 10);
            PacketTestListSerializable test6 = new PacketTestListSerializable(6, 7, 8, 9, 10);
            Test("List", test5, test6);

            //Dict test
            Dictionary<int, float> d = new Dictionary<int, float>();
            d.Add(1, 2);
            d.Add(3, 4);
            d.Add(5, 6);
            d.Add(7, 8);
            d.Add(9, 10);

            PacketTestDictionary test7 = new PacketTestDictionary(d);
            PacketTestDictionarySerializable test8 = new PacketTestDictionarySerializable(d);
            Test("Dictionary", test7, test8);
            #endregion
        }

        public static void Test(string title, Packet a, object b)
        {
            Console.WriteLine("==== Test '{0}':", title);
            byte[] bytesa = null;
            byte[] bytesb = null;
            int times = 10000;

            {
                Console.WriteLine(" @ Serialization:\t{0,9} | {1,9}", "Lib", ".NET");

                Stopwatch sta = Stopwatch.StartNew();
                for (int i = 0; i < times; i++)
                    bytesa = a.Serialize();
                sta.Stop();
                Stopwatch stb = Stopwatch.StartNew();
                for (int i = 0; i < times; i++)
                    bytesb = ObjectToByteArray(b);
                stb.Stop();

                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                Console.WriteLine("   - Bytes:\t\t{0,7} B | {1,7} B", bytesa.Length, bytesb.Length);
                Console.WriteLine("   - Total Bytes:\t{0,9} | {1,9}", FormatFileSize(bytesa.Length * times), FormatFileSize(bytesb.Length * times));
                Console.WriteLine("   - Times:\t\t{0,6} ms | {1,6} ms", sta.ElapsedMilliseconds, stb.ElapsedMilliseconds);
            }

            {
                Console.WriteLine("\n @ Deserialization:\t{0,9} | {1,9}", "Lib", ".NET");

                Packet outputa = null;
                object outputb = null;

                Stopwatch sta = Stopwatch.StartNew();
                for (int i = 0; i < times; i++)
                    outputa = Packet.Deserialize(bytesa);
                sta.Stop();
                Stopwatch stb = Stopwatch.StartNew();
                for (int i = 0; i < times; i++)
                    outputb = FromByteArray<object>(bytesb);
                stb.Stop();

                Console.WriteLine("   - Times:\t\t{0,6} ms | {1,6} ms", sta.ElapsedMilliseconds, stb.ElapsedMilliseconds);
                Console.WriteLine("   - Equals:\t\t{0,9} | {1,9}", a.Equals(outputa), b.Equals(outputb));
                Console.WriteLine("   : " + a);
                Console.WriteLine("   : " + outputa);
            }
            Console.WriteLine("====\n");
        }

        static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
#pragma warning disable SYSLIB0011
                bf.Serialize(ms, obj);
#pragma warning restore SYSLIB0011
                return ms.ToArray();
            }
        }

        static public T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
#pragma warning disable SYSLIB0011
                object obj = bf.Deserialize(ms);
#pragma warning restore SYSLIB0011
                return (T)obj;
            }
        }

        public static string FormatFileSize(long bytes)
        {
            var unit = 1024;
            if (bytes < unit) { return $"{bytes} B"; }

            var exp = (int)(Math.Log(bytes) / Math.Log(unit));
            return $"{bytes / Math.Pow(unit, exp):0.0} {("KMGTPE")[exp - 1]}B";
        }
    }
    #endregion

    #region Test Types And Packets
    [Packet(0, FixedSize = true, CalculatedFixedSize = 36)]
    public class PacketTestFloat3 : Packet
    {
        public float3[] data;

        public PacketTestFloat3() { }

        public PacketTestFloat3(params float3[] data)
        {
            this.data = data;
        }

        public override void Deserialize(ByteReader reader)
        {
            data = reader.ReadArrayGeneric<float3>(3);
        }

        public override void Serialize(ByteWriter writer)
        {
            writer.WriteArrayGeneric(data, true);
        }

        public override bool Equals(object obj)
        {
            if (obj is PacketTestFloat3)
                return Compare((PacketTestFloat3)obj);
            return false;
        }

        public bool Compare(PacketTestFloat3 c)
        {
            if (c.data.Length != data.Length)
                return false;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != c.data[i])
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return data.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("PacketTestFloat3({0})", string.Join(",", data));
        }
    }

    [Serializable]
    public class PacketTestFloat3Serializable
    {
        public float3[] data;
        public PacketTestFloat3Serializable(params float3[] data)
        {
            this.data = data;
        }

        public override bool Equals(object obj)
        {
            if (obj is PacketTestFloat3Serializable)
                return Compare((PacketTestFloat3Serializable)obj);
            return false;
        }

        public bool Compare(PacketTestFloat3Serializable c)
        {
            if (c.data.Length != data.Length)
                return false;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != c.data[i])
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return data.GetHashCode();
        }
    }

    [Packet(1, FixedSize = true)]
    public class PacketTestBool : Packet
    {
        public bool a, b, c, d, e, f, g, h;

        public PacketTestBool() { }
        public PacketTestBool(bool a, bool b, bool c, bool d, bool e, bool f, bool g, bool h)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.f = f;
            this.g = g;
            this.h = h;
        }

        public override void Deserialize(ByteReader reader)
        {
            reader.ReadBooleans(out a, out b, out c, out d, out e, out f, out g, out h);
        }

        public override void Serialize(ByteWriter writer)
        {
            writer.Write(a, b, c, d, e, f, g, h);
        }

        public override bool Equals(object obj)
        {
            if (obj is PacketTestBool)
                return Compare((PacketTestBool)obj);
            return false;
        }

        public bool Compare(PacketTestBool o)
        {
            return o.a == a && o.b == b && o.c == c && o.d == d && o.e == e && o.f == f && o.g == g && o.h == h;
        }

        public override int GetHashCode()
        {
            return a.GetHashCode() ^ b.GetHashCode() ^ c.GetHashCode() ^ d.GetHashCode() ^ e.GetHashCode() ^ f.GetHashCode() ^ g.GetHashCode() ^ h.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("PacketTestBool({0})", string.Join(",", a, b, c, d, e, f, g, h));
        }
    }

    [Serializable]
    public class PacketTestBoolSerializable
    {
        public bool a, b, c, d, e, f, g, h;

        public PacketTestBoolSerializable(bool a, bool b, bool c, bool d, bool e, bool f, bool g, bool h)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.f = f;
            this.g = g;
            this.h = h;
        }

        public override bool Equals(object obj)
        {
            if (obj is PacketTestBoolSerializable)
                return Compare((PacketTestBoolSerializable)obj);
            return false;
        }

        public bool Compare(PacketTestBoolSerializable o)
        {
            return o.a == a && o.b == b && o.c == c && o.d == d && o.e == e && o.f == f && o.g == g && o.h == h;
        }

        public override int GetHashCode()
        {
            return a.GetHashCode() ^ b.GetHashCode() ^ c.GetHashCode() ^ d.GetHashCode() ^ e.GetHashCode() ^ f.GetHashCode() ^ g.GetHashCode() ^ h.GetHashCode();
        }

    }

    [Packet(2)]
    public class PacketTestList : Packet
    {
        public List<int> numbers;

        public PacketTestList() { }

        public PacketTestList(params int[] numbers)
        {
            this.numbers = new List<int>();
            this.numbers.AddRange(numbers);
        }

        public PacketTestList(List<int> numbers)
        {
            this.numbers = new List<int>();
            this.numbers.AddRange(numbers);
        }

        public override void Deserialize(ByteReader reader)
        {
            numbers = new List<int>(reader.ReadInt32Array());
        }

        public override void Serialize(ByteWriter writer)
        {
            writer.Write(numbers);
        }

        public override bool Equals(object obj)
        {
            if (obj is PacketTestList)
                return Compare((PacketTestList)obj);
            return false;
        }

        public bool Compare(PacketTestList o)
        {
            if (o.numbers.Count != numbers.Count)
                return false;

            for (int i = 0; i < numbers.Count; i++)
            {
                if (numbers[i] != o.numbers[i])
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return numbers.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("PacketTestList({0})", string.Join(",", numbers));
        }
    }

    [Serializable]
    public class PacketTestListSerializable
    {
        public List<int> numbers;

        public PacketTestListSerializable(params int[] numbers)
        {
            this.numbers = new List<int>();
            this.numbers.AddRange(numbers);
        }

        public PacketTestListSerializable(List<int> numbers)
        {
            this.numbers = new List<int>();
            this.numbers.AddRange(numbers);
        }

        public override bool Equals(object obj)
        {
            if (obj is PacketTestListSerializable)
                return Compare((PacketTestListSerializable)obj);
            return false;
        }

        public bool Compare(PacketTestListSerializable o)
        {
            if (o.numbers.Count != numbers.Count)
                return false;

            for (int i = 0; i < numbers.Count; i++)
                if (numbers[i] != o.numbers[i])
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            return numbers.GetHashCode();
        }
    }

    [Packet(3)]
    public class PacketTestDictionary : Packet
    {
        public Dictionary<int, float> dict;

        public PacketTestDictionary() { }

        public PacketTestDictionary(Dictionary<int, float> dict)
        {
            this.dict = dict;
        }

        public override void Deserialize(ByteReader reader)
        {
            dict = reader.ReadDictionary<int, float>(reader.ReadInt32, reader.ReadSingle);
        }

        public override void Serialize(ByteWriter writer)
        {
            writer.Write<int, float>(dict, writer.Write, writer.Write);
        }

        public override bool Equals(object obj)
        {
            if (obj is PacketTestDictionary)
                return Compare((PacketTestDictionary)obj);
            return false;
        }

        public bool Compare(PacketTestDictionary o)
        {
            if (o.dict.Count != dict.Count)
                return false;

            foreach (var kvp in dict)
            {
                if (!o.dict.ContainsKey(kvp.Key))
                    return false;
                if (o.dict[kvp.Key] != kvp.Value)
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return dict.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("PacketTestDictionary({0})", string.Join(",", dict));
        }
    }

    [Serializable]
    public class PacketTestDictionarySerializable
    {
        public Dictionary<int, float> dict;

        public PacketTestDictionarySerializable(Dictionary<int, float> dict)
        {
            this.dict = dict;
        }

        public override bool Equals(object obj)
        {
            if (obj is PacketTestDictionarySerializable)
                return Compare((PacketTestDictionarySerializable)obj);
            return false;
        }

        public bool Compare(PacketTestDictionarySerializable o)
        {
            if (o.dict.Count != dict.Count)
                return false;

            foreach (var kvp in dict)
            {
                if (!o.dict.ContainsKey(kvp.Key))
                    return false;
                if (o.dict[kvp.Key] != kvp.Value)
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return dict.GetHashCode();
        }
    }
    #endregion
}