# BitData.NET

This is an experimental toy for declaratively working with bit-level data on .NET
along the same lines as some Haskell libraries [1].

You can declare a class contained bit-level types as follows:

	[StructLayout.Sequential] // ensures fields are processed in order
	public class Foo
	{
		Bit<_7> sevenBits;
		Bit<_11> elevenBits;
		Repeat<_3, Bit<_2>> twoBitsRepeatedThreeTimes;
	}

You can then read this type from a BitArray as follows:

	int pos = 0;
	BitArray bits = ...;
	Foo foo = new Foo();
	BitData<Foo>.Read(foo, ref pos, bits);
	//foo now populated with data from 'bits'

This interface isn't final since this is just an experimental toy. You can optionally
elide the sequential layout attribute and manually implement the reader logic if you
need more control:

	public class Foo : IBitData
	{
		Bit<_7> sevenBits;
		Bit<_11> elevenBits;
		Repeat<_3, Bit<_2>> twoBitsRepeatedThreeTimes;
		public void Read(ref int pos, BitArray bits)
		{
			sevenBits.Read(ref pos, bits);
			elevenBits.Read(ref pos, bits);
			twoBitsRepeatedThreeTimes.Read(ref pos, bits);
		}
	}

# References

This library is in the same spirit as the paper "High-level Views on Low-level Representations" [1].

[1] High-level Views on Low-level Representations, http://web.cecs.pdx.edu/~mpj/pubs/bitdata-icfp05.pdf