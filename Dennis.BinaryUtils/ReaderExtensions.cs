using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dennis.BinaryUtils
{
	public static class ReaderExtensions
	{
		/// <summary>
		/// Reads a varint and returns it as an int
		/// </summary>
		/// <returns>varint value</returns>
		/// <exception cref="InvalidOperationException">WHen varint too big</exception>
		public static int ReadVarInt(this BinaryReader binaryReader)
		{
			int readCounter = 0,
				result = 0,
				tv = 0;
			byte current;

			do
			{
				current = binaryReader.ReadByte();
				tv = current & VarIntConvert.SEGMENT_BITS;
				result |= tv << (7 * readCounter++);

				if (readCounter > 5)
				{
					throw new InvalidOperationException("The specified VarInt is too big!");
				}
			} while ((current & VarIntConvert.CONTINUE_BIT) != 0);

			return result;
		}

		/// <summary>
		/// Read a VarInt prefixed string.
		/// Minecraft Protocol uses this instead of int prefixed/null terminated strings
		/// </summary>
		/// <returns>The string value</returns>
		public static string ReadVString(this BinaryReader binaryReader) =>
			Encoding.UTF8.GetString(binaryReader.ReadBytes(binaryReader.ReadVarInt()));

		/// <summary>
		/// Helper method for reading typed values
		/// </summary>
		/// <typeparam name="T">Type of value to read</typeparam>
		/// <param name="varint">If <c>true</c>, will read int and string with varint.
		/// <c>false</c> to disable.
		/// <c>null</c> to get default from <see cref="VarIntConvert.VarIntMode"/></param>
		/// <returns></returns>
		public static T? Read<T>(this BinaryReader reader, bool? varint = null)
		{
			return (T?)reader.Read(typeof(T), varint);
		}

		/// <summary>
		/// Dictionary containing custom converters for <see cref="Read{T}(BinaryReader, bool?)"/> or <see cref="Read(BinaryReader, Type, bool?)"/>
		/// </summary>
		public static Dictionary<Type, Func<BinaryReader, object>> CustomConverters = new();

		/// <summary>
		/// Helper method to read typed values. <see cref="Read{T}(BinaryReader, bool?)"/> applies
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="type"></param>
		/// <param name="varint"></param>
		/// <returns></returns>
		public static object? Read(this BinaryReader reader, Type type, bool? varint = null)
		{
			if (varint == null) varint = VarIntConvert.VarIntMode;
			if (type == typeof(byte)) return reader.ReadByte();
			if (type == typeof(bool)) return reader.ReadBoolean();
			if (type == typeof(char)) return reader.ReadChar();
			if (type == typeof(decimal)) return reader.ReadDecimal();
			if (type == typeof(double)) return reader.ReadDouble();
			if (type == typeof(Half)) return reader.ReadHalf();
			if (type == typeof(short)) return reader.ReadInt16();
			if (type == typeof(int))
			{
				return varint == true ? reader.ReadVarInt() : (object)reader.ReadInt32();
			}
			if (type == typeof(long)) return reader.ReadInt64();
			if (type == typeof(sbyte)) return reader.ReadSByte();
			if (type == typeof(float)) return reader.ReadSingle();
			if (type == typeof(string))
			{
				return varint == true ? reader.ReadVString() : reader.ReadString();
			};
			if (type == typeof(ushort)) return reader.ReadUInt16();
			if (type == typeof(uint)) return reader.ReadUInt32();
			if (type == typeof(ulong)) return reader.ReadUInt64();
			if (CustomConverters.ContainsKey(type))
			{
				return CustomConverters[type](reader);
			}
			return null;
		}
	}
}
