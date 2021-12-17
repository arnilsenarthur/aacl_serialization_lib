# AACL Serialization Lib
Created by **Árnilsen Arthur**, **AACL Serialization Lib** is a better alternative for **C#** serialization system. Useful for Network Packets or any another byte-object conversions.

- Faster (Read/write)
- Simple (Simple to use. Just drop the files and voila!)
- Light (Light code and packets)

## Table Of Contents
<!-- TOC start -->
- [Features](#features)
- [Simple Example](#simple-example)
- [How to use](#how-to-use)
- [Performance Comparision](#performance-comparision)
  * [1 - Float3[] Example](#1-float3-example)
  * [2 - Bool Example](#2-bool-example)
  * [3 - List Example](#3-list-example)
  * [4 - Dictionary Example](#4-dictionary-example)
  * [5 - Overall](#5-overall)
  * [6 - Dictionary Comparision](#6-dictionary-comparision)
- [Guide](#guide)
  * [1 - Creating a new packet](#1-creating-a-new-packet)
  * [2 - Serializing and deserializing a packet](#2-serializing-and-deserializing-a-packet)
  * [3 - Fixed Size Packets](#3-fixed-size-packets)
  * [4 - Creating custom objects with SerializableObject](#4-creating-custom-objects-with-serializableobject)
  * [5 - Creating custom converters for existing objects](#5-creating-custom-converters-for-existing-objects)
  * [6 - Fixed Size Arrays](#6-fixed-size-arrays)
- [Reading & Writing Guide](#reading-writing-guide)
  * [1 - Writing](#1-writing)
    + [1.1 - Primitives, Arrays/List of Primitives](#11-primitives-arrayslist-of-primitives)
    + [1.2 - SerializableObjects](#12-serializableobjects)
    + [1.3 - Converter implemented types](#13-converter-implemented-types)
    + [1.4 - 8 Compressed boolean](#14-8-compressed-boolean)
    + [1.5 - Array/List of SerializableObjects](#15-arraylist-of-serializableobjects)
    + [1.6 - Array/List of converter implemented objects](#16-arraylist-of-converter-implemented-objects)
    + [1.7 - Dictionary](#17-dictionary)
  * [2 - Reading](#2-reading)
    + [2.1 - Primitives, Arrays/List of Primitives](#21-primitives-arrayslist-of-primitives)
    + [2.2 - Serializable Objects](#22-serializable-objects)
    + [2.3 - Converter implemented types](#23-converter-implemented-types)
    + [2.4 - 8 Compressed boolean](#24-8-compressed-boolean)
    + [2.5 - Array/List of SerializableObjects](#25-arraylist-of-serializableobjects)
    + [2.6 - Array/List of converter implemented objects](#26-arraylist-of-converter-implemented-objects)
    + [2.7 - Dictionary](#27-dictionary)
<!-- TOC end -->


<!-- TOC --><a name="features"></a>
## Features
- Supports primitives (bool,byte,char,Int16,Int32,Int64,float,double)
- Supports strings
- Supports arrays, lists and dictionaries
- Supports custom objects based in interface **SerializableObject**
- Supports any other objects with implementation of custom converters
- Supports fixed length arrays (If an array will always have the same size, you can save the **4 length bytes**)
- Can compact 8 booleans in a single byte (Saving 7 bytes)

<!-- TOC --><a name="simple-example"></a>
## Simple Example
```cs
//Create new packet
PacketPosition packet = new PacketPosition(new float3(4,2,1));

//Serialize
byte[] bt = packet.Bytes

//Deserialize
PacketPosition input = (PacketPosition) bt;
```

<!-- TOC --><a name="how-to-use"></a>
## How to use

You can just drop **AACLSerialization.cs** into your project and it's done (You can see the **example** folder for more info)! Just remember:

```cs
//Import the namespace
using AACLSerialization;

//Register all packets
Packet.Init();
```

-------------------------------------

<!-- TOC --><a name="performance-comparision"></a>
## Performance Comparision
Each results bellow show the time elapsed in the serialization/deserialization of **10000** packets of each type
<!-- TOC --><a name="1-float3-example"></a>
### 1 - Float3[] Example
```cs
public class Packet
{
    float3[] data;
}
```
|                       | AACL  | .NET  |
|-----------------------|-------|-------|
| Serialization Time    | 33ms  | 116ms |
| Byte Size (Per Packet)| 38    | 323   |
| Deserialization Time  | 4ms   | 145ms |

<!-- TOC --><a name="2-bool-example"></a>
### 2 - Bool Example
```cs
public class Packet
{
    bool a,b,c,d,e,f,g,h;
}
```
|                       | AACL  | .NET  |
|-----------------------|-------|-------|
| Serialization Time    | 27ms  | 52ms  |
| Byte Size (Per Packet)| 3     | 168   |
| Deserialization Time  | 2ms   | 79 ms |

<!-- TOC --><a name="3-list-example"></a>
### 3 - List Example
```cs
public class Packet
{
    List<int> numbers;
}
```
|                       | AACL  | .NET  |
|-----------------------|-------|-------|
| Serialization Time    | 23ms  | 89ms  |
| Byte Size (Per Packet)| 26    | 476   |
| Deserialization Time  | 9ms   | 129ms |

<!-- TOC --><a name="4-dictionary-example"></a>
### 4 - Dictionary Example
```cs
public class Packet
{
    Dictionary<int, float> numbers;
}
```
|                       | AACL  | .NET  |
|-----------------------|-------|-------|
| Serialization Time    | 45ms  | 232ms |
| Byte Size (Per Packet)| 46    | 1778  |
| Deserialization Time  | 7ms   | 337ms |


<!-- TOC --><a name="5-overall"></a>
### 5 - Overall
|                       | AACL  | .NET  |
|-----------------------|-------|-------|
| (Float3[]) Serialization Time    | **33ms**  | 116ms |
| (Float3[]) Byte Size (Per Packet)| **38**    | 323   |
| (Float3[]) Deserialization Time  | **4ms**   | 145ms |
| (Bool) Serialization Time    | **27ms**  | 52ms  |
| (Bool) Byte Size (Per Packet)| **3**     | 168   |
| (Bool) Deserialization Time  | **2ms**   | 79 ms |
| (List) Serialization Time    | **23ms**  | 89ms  |
| (List) Byte Size (Per Packet)| **26**    | 476   |
| (List) Deserialization Time  | **9ms**   | 129ms |
| (Dict) Serialization Time    | **45ms**  | 232ms |
| (Dict) Byte Size (Per Packet)| **46**    | 1778  |
| (Dict) Deserialization Time  | **7ms**   | 337ms |

<!-- TOC --><a name="6-dictionary-comparision"></a>
### 6 - Dictionary Comparision

Comparision of serialization of **1000** of instances of the packet shown bellow, with a variable number of elements:

```cs
public class Packet
{
    Dictionary<int, float> numbers;
}
```
![Packet byte size comparision](/images/bytesizecomparision.png)
![Elapsed time comparision](/images/timecomparision.png)

----------------------------------------------

<!-- TOC --><a name="guide"></a>
## Guide
<!-- TOC --><a name="1-creating-a-new-packet"></a>
### 1 - Creating a new packet

To create a new packet, you need to create a new packet class. See the example bellow:

```cs
[Packet(15)] //Defines the id of this packet (You must guarantee that each packet has a unique id)
public class PacketTransformSync : Packet
{
    public float3 position;
    public float3 scale;
    public float3 euler;

    public PacketTransformSync() {} //Each packet must have an empty constructor

    public PacketTransformSync(float3 position,float3 scale,float3 euler)
    {
        this.position = position;
        this.scale = scale;
        this.euler = euler;
    }
}
```

Now me must implement our Serialize and Deserialize methods, as show bellow:

```cs
[Packet(15)]
public class PacketTransformSync : Packet
{
    //[...]

    public override void Deserialize(ByteReader reader)
    {
        this.position = reader.Read<float3>();
        this.scale = reader.Read<float3>();
        this.euler = reader.Read<float3>();
    }

    public override void Serialize(ByteWriter writer)
    {
        writer.Write(position);
        writer.Write(scale);
        writer.Write(euler);
    }
}
```

<!-- TOC --><a name="2-serializing-and-deserializing-a-packet"></a>
### 2 - Serializing and deserializing a packet

Now our packet is ready to be serialized/deserialized:

```cs
public static void Main(string[] args)
{
    Packet.Init(); //Register all packets
    
    //Creating a new packet
    PacketTransformSync packet = new PacketTransformSync(new float3(0,0,0),new float3(5,3,12),new float3(60,90,15));

    //Serializing
    byte[] bt = packet.Bytes;

    //Deserializing
    PacketTransformSync p = (PacketTransformSync) bt;
}
```
<!-- TOC --><a name="3-fixed-size-packets"></a>
### 3 - Fixed Size Packets

To improve the memory management, you can define that a packet will always have the same length (or at last a maximum length of bytes) like **PacketTransformSync** that will always have (36 bytes + 2 bytes for type header), so you can explicit tell this to the system:

```cs
[Packet(15, FixedSize = true, CalculatedFixedSize = 36)]
public class PacketTransformSync : Packet {}
```
If your packet is only made of types that are non nullable (primitives,fixed-length-structs,etc..) you don't need to set the **CalculatedFixedSize**. The system will auto calculate it from an empty instance:
```cs
[Packet(15, FixedSize = true)]
public class PacketTransformSync : Packet {}
```

<!-- TOC --><a name="4-creating-custom-objects-with-serializableobject"></a>
### 4 - Creating custom objects with SerializableObject

If you need to create custom objects, like **float3** you just need to implment the **SerializableObject** interface, as shown bellow:

```cs
public struct float3 : SerializableObject
{
    public float x;
    public float y;
    public float z;

    public float3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public void Deserialize(ByteReader reader)
    {
        x = reader.ReadSingle();
        y = reader.ReadSingle();
        z = reader.ReadSingle();
    }

    public void Serialize(ByteWriter writer)
    {
        writer.Write(x);
        writer.Write(y);
        writer.Write(z);
    }
    //[...]
}
```
Now you can serialize/deserialize float3 objects with no problems

<!-- TOC --><a name="5-creating-custom-converters-for-existing-objects"></a>
### 5 - Creating custom converters for existing objects

If you want to add support for an existing object, you can add it on the class **PacketSystem.Converter**, as shown bellow:

```cs
public static class Converter
{
    public static void Serialize(this Object value, BinaryWriter writer)
    {
        switch (value)
        {
            case float3 f:
                writer.Write(f.x);
                writer.Write(f.y);
                writer.Write(f.z);
                break;

            //Add your type here

            default:
                //[...]
        }
    }

    public static T Deserialize<T>(BinaryReader reader)
    {
        if (typeof(T) == typeof(float3))
        {
            float3 f = new float3();
            f.x = reader.ReadSingle();
            f.y = reader.ReadSingle();
            f.z = reader.ReadSingle();
            return (T)Convert.ChangeType(f, typeof(T));
        }
        else //Add your type here
    }
}
```

<!-- TOC --><a name="6-fixed-size-arrays"></a>
### 6 - Fixed Size Arrays

As Fixed-Size-Packets, you can also define Fixed-Size-Arrays. Every time you serialize an array, the system will auto write 4 header bytes that represents the length of an array/collection. If your array/list will always have the same number of elements, you can set the fixed length to save this 4 bytes:

```cs
public override void Deserialize(ByteReader reader)
{
    //data = reader.reader.ReadArrayGeneric<float3>();
    data = reader.ReadArrayGeneric<float3>(3); //Will read 3 float3
}

public override void Serialize(ByteWriter writer)
{
    //writer.WriteArrayGeneric(data);
    writer.WriteArrayGeneric(data, true); //Will write array ignoring header bytes
}
```
-------------------------------------------
<!-- TOC --><a name="reading-writing-guide"></a>
## Reading & Writing Guide

Here you can find infomation to choose which method you will need to read/write a certain value

<!-- TOC --><a name="1-writing"></a>
### 1 - Writing

<!-- TOC --><a name="11-primitives-arrayslist-of-primitives"></a>
#### 1.1 - Primitives, Arrays/List of Primitives
If your value is a primive, or an array/list/collection of primitives, you can use:
```cs
ByteWriter.Write(value)

//If you want to write an array/list you can use.
//It will be slower, but functional as well
ByteWriter.WriteArrayDynamic<T>(list,fixedSize=false)
```

<!-- TOC --><a name="12-serializableobjects"></a>
#### 1.2 - SerializableObjects

If your object is a serializable object you can use:

```cs
ByteWriter.Write(value)
```

<!-- TOC --><a name="13-converter-implemented-types"></a>
#### 1.3 - Converter implemented types

If you want to write any object that will use converter you can use:

```cs
ByteWriter.WriteDynamic(value)
```

<!-- TOC --><a name="14-8-compressed-boolean"></a>
#### 1.4 - 8 Compressed boolean

If you want to write n (where 9 > n > 0) booleans as a single compressed byte, you can use:

```cs
ByteWriter.Write(a,b,c,...h)
```

<!-- TOC --><a name="15-arraylist-of-serializableobjects"></a>
#### 1.5 - Array/List of SerializableObjects

If you want to write an array/list of SerializableObjects you can use one of the two options bellow:

```cs
//Faster, indicated
ByteWriter.Write<T>(list,fixedSize=false)

//Slower but functional
ByteWriter.WriteArrayDynamic<T>(list,fixedSize=false)
```

<!-- TOC --><a name="16-arraylist-of-converter-implemented-objects"></a>
#### 1.6 - Array/List of converter implemented objects

If you want to write an array/list of any type implemented by converter, you can use the following method:

```cs
ByteWriter.WriteArrayGeneric<T>(list,fixedSize=false)
```

<!-- TOC --><a name="17-dictionary"></a>
#### 1.7 - Dictionary

If you want to write a dictionary of any 2 given types, you can use the following syntax:

```cs
//Faster
ByteWriter.Write<A, B>(value,keyWriter,valueWriter,fixedSize=false);
    //Ex.:
    ByteWriter.Write<int,int>(...,writer.Write,writer.Write);

//Slower
ByteWriter.Write<A, B>(value,fixedSize=false)
```

<!-- TOC --><a name="2-reading"></a>
### 2 - Reading

<!-- TOC --><a name="21-primitives-arrayslist-of-primitives"></a>
#### 2.1 - Primitives, Arrays/List of Primitives

To read a primitive or a array/list of primitives you must use the right method for each type, as shown bellow:

```cs
float a = ByteReader.ReadSingle();
int b = ByteReader.ReadInt32();
int[] c = ByteReader.ReadInt32Array();
int[] d = ByteReader.ReadInt32Array(3); //Fixed Length Array
List<int> = new List<int>(ByteReader.ReadInt32Array());
```

<!-- TOC --><a name="22-serializable-objects"></a>
#### 2.2 - Serializable Objects

To read a serializable object you can use the following method:

```cs
T name = ByteReader.Read<T>();
```

<!-- TOC --><a name="23-converter-implemented-types"></a>
#### 2.3 - Converter implemented types

To read any object that implements a conversor, you must use:

```cs
T name = ByteReader.ReadDynamic<T>();
```

<!-- TOC --><a name="24-8-compressed-boolean"></a>
#### 2.4 - 8 Compressed boolean

If you want to read a byte as 8 separated booleans, you can use one of the two methods bellow:

```cs
bool[] b = ByteReader.ReadBooleans();

ByteReader.ReadBooleans(out bool a, out bool b, out bool c, out bool d, out bool e, out bool f, out bool g, out bool h);
```

<!-- TOC --><a name="25-arraylist-of-serializableobjects"></a>
#### 2.5 - Array/List of SerializableObjects

If you want to read an array/list of SerializableObjects you must use the following method:

```cs
T[] name = ByteReader.ReadArray<T>();
T[] name = ByteReader.ReadArray<T>(4); //Fixed Length Array
```

<!-- TOC --><a name="26-arraylist-of-converter-implemented-objects"></a>
#### 2.6 - Array/List of converter implemented objects

If you want to read an array/list of any object that implements a converter you must use the following method:

```cs
T[] name = ByteReader.ReadArrayGeneric<T>();
T[] name = ByteReader.ReadArrayGeneric<T>(4); //Fixed Length Array
```

<!-- TOC --><a name="27-dictionary"></a>
#### 2.7 - Dictionary

If you want to read a dictionary you must use the following method:

```cs
Dictionary<A, B> name = ByteReader.ReadDictionary<A, B>(keysReader,valuesReader);

Dictionary<A, B> name = ByteReader.ReadDictionary<A, B>(length,keysReader, valuesReader); //Fixed Length Dictionary
```