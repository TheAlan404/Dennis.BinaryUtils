using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Dennis.BinaryUtils.Test
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void ReadVarInt()
		{
			var ms = new MemoryStream();
			var writer = new BinaryWriter(ms);
			var reader = new BinaryReader(ms);
			writer.Write(new byte[]
			{
				0xff, 0xff, 0x7f
			});
			Assert.IsTrue(ms.Position > 0);
			ms.Position = 0;
			Assert.IsTrue(reader.ReadVarInt() == 2097151);
		}

		[TestMethod]
		public void WriteVarInt()
		{
			var ms = new MemoryStream();
			var writer = new BinaryWriter(ms);
			var reader = new BinaryReader(ms);
			writer.WriteVarInt(2097151);
			Assert.IsTrue(ms.Position > 0);
			Assert.IsTrue(ms.Length == 3);
			ms.Position = 0;
			Assert.IsTrue(reader.ReadBytes(3).SequenceEqual(new byte[] { 0xff, 0xff, 0x7f }));
		}

		public class Square
		{
			public int Width;
			public int Height;

			public override string ToString()
			{
				return $"Square {{ {Width} ; {Height} }}";
			}
		}

		[TestMethod]
		public void CustomReadWrite()
		{
			WriterExtensions.CustomConverters[typeof(Square)] = (BinaryWriter w, object o) =>
			{
				Square sq = (Square)o;
				Console.WriteLine(sq);
				w.Write(sq.Width);
				w.Write(sq.Height);
			};

			ReaderExtensions.CustomConverters[typeof(Square)] = (BinaryReader r) =>
			{
				var sq = new Square();
				sq.Width = r.Read<int>();
				sq.Height = r.Read<int>();
				Console.WriteLine(sq);
				return sq;
			};

			MemoryStream customRW = new MemoryStream();
			var writer = new BinaryWriter(customRW);

			var mySquare = new Square()
			{
				Width = 21,
				Height = 2346,
			};

			//writer.Write<Square>(mySquare);
			writer.Write(mySquare);

			customRW.Position = 0;
			var reader = new BinaryReader(customRW);
			var square = reader.Read<Square>();
			Assert.IsNotNull(square);
			Assert.IsTrue(square.Width == 21);
			Assert.IsTrue(square.Height == 2346);
		}
	}
}