namespace Dennis.BinaryUtils
{
	public static class VarIntConvert
	{
		/// <summary>
		/// Weather to use varints for int reading on reader extensions
		/// </summary>
		public static bool VarIntMode = false;

		public const byte SEGMENT_BITS = 0x7F;
		public const byte CONTINUE_BIT = 0x80;

		public static byte[] VarIntToBytes(int value)
		{
			var buffer = new byte[10];
			var pos = 0;
			do
			{
				var byteVal = value & SEGMENT_BITS;
				value >>= 7;

				if (value != 0)
				{
					byteVal |= CONTINUE_BIT;
				}

				buffer[pos++] = (byte)byteVal;

			} while (value != 0);

			var result = new byte[pos];
			Buffer.BlockCopy(buffer, 0, result, 0, pos);

			return result;
		}

		public static byte[] VarLongToBytes(long value)
		{
			List<byte> bytes = new List<byte>();
			while (true)
			{
				if ((value & unchecked((long)0xFFFFFFFFFFFFFF80)) == 0)
				{
					bytes.Add((byte)value);
					return bytes.ToArray();
				}

				bytes.Add((byte)((value & SEGMENT_BITS) | CONTINUE_BIT));
				// Note: >>> means that the sign bit is shifted with the rest of the number rather than being left alone
				value >>= 7;
			}
		}

		public static int ReadVarInt(byte[] bytes)
		{
			var ms = new MemoryStream(bytes);
			var w = new BinaryReader(ms);
			return w.ReadVarInt();
		}
	}
}