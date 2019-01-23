using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq.Expressions;

namespace BitData
{
    /// <summary>
    /// The base interface for bit-level data.
    /// </summary>
    public interface IBitData
    {
        void Read(ref int pos, BitArray bits);
        //void Write(ref int pos, BitArray bits);
    }

    /// <summary>
    /// Declare a fixed number of bits.
    /// </summary>
    /// <typeparam name="TCount">The number of bits to consume.</typeparam>
    public struct Bit<TCount> : IBitData
        where TCount :INat, new()
    {
        static readonly int Count = new TCount().Value;
        public uint Value { get; private set; }
        public int Length => Count;

        public void Read(ref int pos, BitArray bits)
        {
            for (int i = 0; i < Count; ++i)
            {
                Value <<= 1;
                Value |= (byte)(bits[pos++] ? 1 : 0);
            }
        }

        //public void Write(ref int pos, BitArray bits)
        //{
        //    var x = Value;
        //    for (int i = 0; i < Count; ++i)
        //    {
        //        x >>= 1;
        //        if (pos > bits.Length)
        //            bits.Length *= 2;
        //        bits.Set(pos++, 1 == (x & 0x01));
        //    }
        //}
    }

    /// <summary>
    /// A sequence <typeparamref name="TBits"/> of length <typeparamref name="TLength"/>.
    /// </summary>
    /// <typeparam name="TLength">The length of the bit sequence.</typeparam>
    /// <typeparam name="TBits">The bits in the sequence.</typeparam>
    public struct Repeat<TLength, TBits> : IBitData
        where TLength : INat, new()
        where TBits : IBitData, new()
    {
        public static readonly int Count = new TLength().Value;
        public TBits[] Values { get; private set; }

        public void Read(ref int pos, BitArray bits)
        {
            Values = new TBits[Count];
            for (int i = 0; i < Count; ++i)
            {
                Values[i] = new TBits();
                Values[i].Read(ref pos, bits);
            }
        }
    }

    /// <summary>
    /// Constructs <typeparamref name="TBits"/> until a termination clause fails.
    /// </summary>
    /// <typeparam name="TBits">The bits in the sequence.</typeparam>
    public struct Loop<TBits> : IBitData
        where TBits : IBitData, new()
    {
        public List<TBits> Values { get; private set; }
        Func<TBits, bool> clause;

        public Loop(Func<TBits, bool> clause)
        {
            this.Values = null;
            this.clause = clause;
        }

        public void Read(ref int pos, BitArray bits)
        {
            Values = new List<TBits>();
            while (true)
            {
                var backtrack = pos;
                var x = new TBits();
                x.Read(ref backtrack, bits);
                if (!clause(x)) break;
                Values.Add(x);
                pos = backtrack;
            }
        }
    }

    // Dispatcher logic for handling classes that enclose bitdata, but don't implement IBits
    public delegate void BitReader<T>(T obj, ref int pos, BitArray bits)
        where T : class;

    public static class BitData<T>
        where T : class
    {
        public static BitReader<T> Read { get; private set; }

        static BitData()
        {
            var type = typeof(T);
            if (typeof(IBitData).IsAssignableFrom(type))
            {
                // use the native bit reading method
                Read = (BitReader<T>)type.GetMethod("Read").CreateDelegate(typeof(BitReader<T>), null);
            }
            else
            {
                // infer a bit-level reader from the object structure
                var obj = Expression.Parameter(typeof(T), "obj");
                var pos = Expression.Parameter(typeof(int).MakeByRefType(), "pos");
                var bits = Expression.Parameter(typeof(BitArray), "bits");
                var e = new List<Expression>();
                var read = typeof(IBitData).GetMethod("Read");
                foreach (var x in type.GetFields())
                {
                    if (typeof(IBitData).IsAssignableFrom(x.FieldType))
                    {
                        // call IBitData.Read directly
                        e.Add(Expression.Call(Expression.Field(obj, x), read, pos, bits));
                    }
                    else
                    {
                        // construct an expression that calls Read on each field
                        if (!x.FieldType.IsClass)
                            throw new InvalidOperationException($"{type.Name}.{x.Name} must be a BitData type or a reference type.");
                        var fbd = typeof(BitData<>).MakeGenericType(x.FieldType);
                        var fread = Expression.Constant(fbd.GetProperty("Read"));
                        var ffield = Expression.Field(obj, x);
                        e.Add(Expression.Assign(ffield, Expression.New(x.FieldType)));
                        e.Add(Expression.Invoke(fread, ffield, pos, bits));
                    }
                }
                Read = Expression.Lambda<BitReader<T>>(Expression.Block(e), obj, pos, bits)
                                 .Compile();
            }
        }
    }

    /// <summary>
    /// Base interface for type-level natural numbers.
    /// </summary>
    public interface INat { int Value { get; } }
    public struct _1 : INat { public int Value => 1; }
    public struct _2 : INat { public int Value => 2; }
    public struct _3 : INat { public int Value => 3; }
    public struct _4 : INat { public int Value => 4; }
    public struct _5 : INat { public int Value => 5; }
    public struct _6 : INat { public int Value => 6; }
    public struct _7 : INat { public int Value => 7; }
    public struct _8 : INat { public int Value => 8; }
    public struct _9 : INat { public int Value => 9; }
    public struct _10 : INat { public int Value => 10; }
    public struct _11 : INat { public int Value => 11; }
    public struct _12 : INat { public int Value => 12; }
    public struct _13 : INat { public int Value => 13; }
    public struct _14 : INat { public int Value => 14; }
    public struct _15 : INat { public int Value => 15; }
}
