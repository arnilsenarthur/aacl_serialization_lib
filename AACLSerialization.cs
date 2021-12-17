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
using System.Linq;
using System.Collections.Generic;

namespace AACLSerialization
{
    #region Converters
    /// <summary>
    /// Used to provide conversion for custom types that you can't extend to SerializableObject.
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Serialize an object to bytes
        /// </summary>
        /// <param name="value"></param>
        /// <param name="writer"></param>
        public static void Serialize(this Object value, BinaryWriter writer)
        {
            switch (value)
            {
                default:
                    throw new NotImplementedException($"No serialization method found for type {value.GetType().FullName}");
            }
        }

        /// <summary>
        /// Deserialize an object of type T from bytes
        /// </summary>
        /// <param name="reader"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Deserialize<T>(BinaryReader reader)
        {
            throw new NotImplementedException($"No deserialization method found for type {typeof(T).FullName}");
        }
    }
    #endregion

    #region Main Objects
    /// <summary>
    /// Main base class for seraliizing objects
    /// </summary>
    public interface SerializableObject
    {
        void Serialize(ByteWriter writer);
        void Deserialize(ByteReader reader);
    }

    /// <summary>
    /// Attribute used to register an packet by an 'Int16' id
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class PacketAttribute : System.Attribute
    {
        private Int16 id = 0;
        public Int16 Id => id;

        public bool FixedSize = false;
        public int CalculatedFixedSize = 0;

        public PacketAttribute(Int16 id) => this.id = id;

    }

    /// <summary>
    /// Base class for all packets, with basic functionality (casts, etc...)
    /// </summary>
    [Serializable]
    public abstract class Packet : SerializableObject
    {
        #region Private Static Fields
        private static Dictionary<Int16, Func<Packet>> types = new Dictionary<Int16, Func<Packet>>();
        #endregion

        #region Public Fields
        public byte[] Bytes => Serialize();
        public bool HasFixedSize => ((PacketAttribute)Attribute.GetCustomAttribute(GetType(), typeof(PacketAttribute))).FixedSize;
        public int CalculatedFixedSize => ((PacketAttribute)Attribute.GetCustomAttribute(GetType(), typeof(PacketAttribute))).CalculatedFixedSize;
        #endregion

        #region Abstract Methods
        public abstract void Deserialize(ByteReader reader);
        public abstract void Serialize(ByteWriter writer);
        #endregion

        #region Public Methods
        public byte[] Serialize()
        {
            PacketAttribute packet = (PacketAttribute)Attribute.GetCustomAttribute(GetType(), typeof(PacketAttribute));

            ByteWriter writer = (packet.FixedSize && packet.CalculatedFixedSize > 0) ? new ByteWriter(packet.CalculatedFixedSize + 2) : new ByteWriter();
            writer.Write(packet.Id);
            Serialize(writer);
            writer.Close();
            return writer.Bytes;
        }

        public static T Deserialize<T>(byte[] bt) where T : Packet
        {
            ByteReader reader = new ByteReader(bt);
            Packet packet = types[reader.ReadInt16()]();
            packet.Deserialize(reader);
            reader.Close();

            return (T)packet;
        }

        public static Packet Deserialize(byte[] bt)
        {
            ByteReader reader = new ByteReader(bt);
            Packet packet = types[reader.ReadInt16()]();
            packet.Deserialize(reader);
            reader.Close();

            return packet;
        }
        #endregion

        #region Protected Methods
        protected virtual int GetFixedSize()
        {
            return Bytes.Length;
        }
        #endregion

        #region Public Casting
        public static explicit operator Packet(byte[] bytes) => Deserialize(bytes);
        public static explicit operator byte[](Packet packet) => packet.Bytes;
        #endregion

        #region Static Init Method
        /// <summary>
        /// Main packet system init method
        /// </summary>
        public static void Init()
        {
            var typesWithMyAttribute =
            from a in AppDomain.CurrentDomain.GetAssemblies()
            from t in a.GetTypes()
            let attributes = t.GetCustomAttributes(typeof(PacketAttribute), true)
            where attributes != null && attributes.Count() > 0
            select new { Type = t, Attributes = (PacketAttribute)attributes[0] };

            foreach (var t in typesWithMyAttribute.Select(x => x))
            {
                if (t.Attributes.FixedSize && t.Attributes.CalculatedFixedSize == 0)
                    t.Attributes.CalculatedFixedSize = ((Packet)Activator.CreateInstance(t.Type)).GetFixedSize();

                types[t.Attributes.Id] = () => (Packet)Activator.CreateInstance(t.Type);
            }
        }

        #endregion
    }
    #endregion

    #region Utils
    /// <summary>
    /// Class used to read objects from a stream of bytes
    /// </summary>
    public class ByteReader : BinaryReader
    {
        #region Constructor
        public ByteReader(byte[] bytes) : base(new MemoryStream(bytes)) { }
        #endregion

        #region Objects
        public T Read<T>() where T : SerializableObject, new()
        {
            T o = new T();
            o.Deserialize(this);
            return o;
        }

        public T ReadDynamic<T>() => Converter.Deserialize<T>(this);

        public void ReadBooleans(out bool a, out bool b, out bool c, out bool d, out bool e, out bool f, out bool g, out bool h)
        {
            byte v = ReadByte();
            a = (v & 1) == 1;
            b = (v & 2) == 2;
            c = (v & 4) == 4;
            d = (v & 8) == 8;
            e = (v & 16) == 16;
            f = (v & 32) == 32;
            g = (v & 64) == 64;
            h = (v & 128) == 128;
        }

        public bool[] ReadBooleans()
        {
            byte v = ReadByte();

            return new bool[]{
                (v & 1) == 1,
                (v & 2) == 2,
                (v & 4) == 4,
                (v & 8) == 8,
                (v & 16) == 16,
                (v & 32) == 32,
                (v & 64) == 64,
                (v & 128) == 128
            };
        }
        #endregion

        #region Fixed Size Arrays
        public T[] ReadArray<T>(int length) where T : SerializableObject, new()
        {
            T[] v = new T[length];

            for (int i = 0; i < length; i++)
            {
                T o = new T();
                o.Deserialize(this);
                v[i] = o;
            }

            return v;
        }

        public T[] ReadArrayGeneric<T>(int length)
        {
            T[] v = new T[length];

            for (int i = 0; i < length; i++)
            {
                v[i] = Converter.Deserialize<T>(this);
            }

            return v;
        }

        public bool[] ReadBooleanArray(int length)
        {
            bool[] v = new bool[length];
            for (int i = 0; i < length; i++)
            {
                v[i] = ReadBoolean();
            }
            return v;
        }

        public byte[] ReadByteArray(int length)
        {
            byte[] v = new byte[length];
            for (int i = 0; i < length; i++)
            {
                v[i] = ReadByte();
            }
            return v;
        }

        public Int16[] ReadInt16Array(int length)
        {
            Int16[] v = new Int16[length];
            for (int i = 0; i < length; i++)
            {
                v[i] = ReadInt16();
            }
            return v;
        }

        public Int32[] ReadInt32Array(int length)
        {
            Int32[] v = new Int32[length];
            for (int i = 0; i < length; i++)
            {
                v[i] = ReadInt32();
            }
            return v;
        }

        public Int64[] ReadInt64Array(int length)
        {
            Int64[] v = new Int64[length];
            for (int i = 0; i < length; i++)
            {
                v[i] = ReadInt64();
            }
            return v;
        }

        public float[] ReadFloatArray(int length)
        {
            float[] v = new float[length];
            for (int i = 0; i < length; i++)
            {
                v[i] = ReadSingle();
            }
            return v;
        }

        public double[] ReadDoubleArray(int length)
        {
            double[] v = new double[length];
            for (int i = 0; i < length; i++)
            {
                v[i] = ReadDouble();
            }
            return v;
        }

        public string[] ReadStringArray(int length)
        {
            string[] v = new string[length];
            for (int i = 0; i < length; i++)
            {
                v[i] = ReadString();
            }
            return v;
        }

        public char[] ReadCharArray(int length)
        {
            char[] v = new char[length];
            for (int i = 0; i < length; i++)
            {
                v[i] = ReadChar();
            }
            return v;
        }
        #endregion

        #region Arrays
        public T[] ReadArray<T>() where T : SerializableObject, new() => ReadArray<T>(ReadInt32());
        public T[] ReadArrayGeneric<T>() => ReadArrayGeneric<T>(ReadInt32());

        public bool[] ReadBooleanArray() => ReadBooleanArray(ReadInt32());

        public byte[] ReadByteArray() => ReadByteArray(ReadInt32());

        public Int16[] ReadInt16Array() => ReadInt16Array(ReadInt32());

        public Int32[] ReadInt32Array() => ReadInt32Array(ReadInt32());

        public Int64[] ReadInt64Array() => ReadInt64Array(ReadInt32());

        public float[] ReadFloatArray() => ReadFloatArray(ReadInt32());

        public double[] ReadDoubleArray() => ReadDoubleArray(ReadInt32());

        public string[] ReadStringArray() => ReadStringArray(ReadInt32());

        public char[] ReadCharArray() => ReadCharArray(ReadInt32());
        #endregion

        #region Dictionaries
        public Dictionary<A, B> ReadDictionary<A, B>(int length, Func<A> keys, Func<B> values)
        {
            Dictionary<A, B> v = new Dictionary<A, B>(length);
            for (int i = 0; i < length; i++)
            {
                v.Add(keys(), values());
            }
            return v;
        }

        public Dictionary<A, B> ReadDictionary<A, B>(Func<A> keys, Func<B> values)
        {
            return ReadDictionary(ReadInt32(), keys, values);
        }
        #endregion

        #region Utils
        public override void Close()
        {
            base.Close();
            BaseStream.Close();
        }
        #endregion
    }

    /// <summary>
    /// Class used to write bytes from objects to an stream of bytes
    /// </summary>
    public class ByteWriter : BinaryWriter
    {
        #region Public Fields
        public byte[] Bytes => ((MemoryStream)BaseStream).ToArray();
        #endregion

        #region Constructor
        public ByteWriter() : base(new MemoryStream()) { }
        public ByteWriter(int length) : base(new MemoryStream(length)) { }
        #endregion

        #region Objects
        public void Write(SerializableObject value) => value.Serialize(this);
        public void WriteDynamic(dynamic value) => Converter.Serialize(value, this);
        public void Write(bool a, bool b = false, bool c = false, bool d = false, bool e = false, bool f = false, bool g = false, bool h = false)
        {
            Write((byte)((a ? 1 : 0) | (b ? 2 : 0) | (c ? 4 : 0) | (d ? 8 : 0) | (e ? 16 : 0) | (f ? 32 : 0) | (g ? 64 : 0) | (h ? 128 : 0)));
        }
        #endregion

        #region Arrays
        public void Write<T>(IList<T> value, bool fixedSize = false) where T : SerializableObject
        {
            if (!fixedSize) Write(value.Count);
            foreach (var v in value) v.Serialize(this);
        }

        public void WriteArrayGeneric<T>(IList<T> value, bool fixedSize = false)
        {
            if (!fixedSize) Write(value.Count);
            foreach (var v in value) Converter.Serialize(v, this);
        }

        public void WriteArrayDynamic<T>(dynamic value, bool fixedSize = false)
        {
            if (!fixedSize) Write(value.Count);
            foreach (var v in value) Write(v);
        }

        public void Write(IList<bool> value, bool fixedSize = false)
        {
            if (!fixedSize) Write(value.Count);
            foreach (var v in value) Write(v);
        }

        public void Write(IList<byte> value, bool fixedSize = false)
        {
            if (!fixedSize) Write(value.Count);
            Write(value);
        }

        public void Write(IList<dynamic> value, bool fixedSize = false)
        {
            if (!fixedSize) Write(value.Count);
            foreach (var v in value) Write(v);
        }

        public void Write(IList<Int16> value, bool fixedSize = false)
        {
            if (!fixedSize) Write(value.Count);
            foreach (var v in value) Write(v);
        }

        public void Write(IList<Int32> value, bool fixedSize = false)
        {
            if (!fixedSize) Write(value.Count);
            foreach (var v in value) Write(v);
        }

        public void Write(IList<Int64> value, bool fixedSize = false)
        {
            if (!fixedSize) Write(value.Count);
            foreach (var v in value) Write(v);
        }

        public void Write(IList<float> value, bool fixedSize = false)
        {
            if (!fixedSize) Write(value.Count);
            foreach (var v in value) Write(v);
        }

        public void Write(IList<double> value, bool fixedSize = false)
        {
            if (!fixedSize) Write(value.Count);
            foreach (var v in value) Write(v);
        }

        public void Write(IList<char> value, bool fixedSize = false)
        {
            if (!fixedSize) Write(value.Count);
            foreach (var v in value) Write(v);
        }

        public void Write(IList<string> value, bool fixedSize = false)
        {
            if (!fixedSize) Write(value.Count);
            foreach (var v in value) Write(v);
        }
        #endregion

        #region Dictionaries
        public void Write<A, B>(dynamic value, bool fixedSize = false)
        {
            if (!fixedSize) Write(value.Count);
            foreach (var v in value)
            {
                Write(v.Key);
                Write(v.Value);
            }
        }

        public void Write<A, B>(IDictionary<A, B> value, Action<A> keys, Action<B> values, bool fixedSize = false)
        {
            if (!fixedSize) Write(value.Count);
            foreach (var v in value)
            {
                keys(v.Key);
                values(v.Value);
            }
        }
        #endregion

        #region Override Methods
        public override void Close()
        {
            base.Close();
            BaseStream.Close();
        }
        #endregion
    }
    #endregion
}