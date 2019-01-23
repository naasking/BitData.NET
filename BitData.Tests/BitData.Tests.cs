using System;
using System.Collections;
using BitData;
using Xunit;

namespace BitData.Tests
{
    public class Tests
    {
        public class Foo : IBitData
        {
            public Bit<_5> something;
            public Bit<_3> other;
            public Repeat<_7, Bit<_1>> bools;

            public void Read(ref int pos, BitArray bits)
            {
                something.Read(ref pos, bits);
                other.Read(ref pos, bits);
                bools.Read(ref pos, bits);
            }
        }

        [Fact]
        public static void TestFoo()
        {
            var bits = new BitArray(15);
            bits.Set(4, true);
            bits.Set(5, true);
            bits.Set(13, true);
            var foo = new Foo();
            var pos = 0;
            foo.Read(ref pos, bits);
            Assert.Equal((uint)1, foo.something.Value);
            Assert.Equal((uint)(1 << 2), foo.other.Value);
            Assert.Equal(7, foo.bools.Values.Length);
            for (var i = 0; i < foo.bools.Values.Length; ++i)
            {
                if (i == 5)
                    Assert.Equal((uint)1, foo.bools.Values[i].Value);
                else
                    Assert.Equal((uint)0, foo.bools.Values[i].Value);
            }
        }

        public class Foo2
        {
            public Bit<_5> something;
            public Bit<_3> other;
            public Repeat<_7, Bit<_1>> bools;
        }

        [Fact]
        public static void TestFoo2()
        {
            var bits = new BitArray(15);
            bits.Set(4, true);
            bits.Set(5, true);
            bits.Set(13, true);
            var foo = new Foo2();
            var pos = 0;
            BitData<Foo2>.Read(foo, ref pos, bits);
            Assert.Equal((uint)1, foo.something.Value);
            Assert.Equal((uint)(1 << 2), foo.other.Value);
            Assert.Equal(7, foo.bools.Values.Length);
            for (var i = 0; i < foo.bools.Values.Length; ++i)
            {
                if (i == 5)
                    Assert.Equal((uint)1, foo.bools.Values[i].Value);
                else
                    Assert.Equal((uint)0, foo.bools.Values[i].Value);
            }
        }
    }
}
