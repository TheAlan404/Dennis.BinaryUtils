using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dennis.BinaryUtils
{
	public static class WriterExtensions
	{
		/// <summary>
		/// Writes a VarInt
		/// </summary>
		public static void WriteVarInt(this BinaryWriter binaryWriter, int value) =>
			binaryWriter.Write(VarIntConvert.VarIntToBytes(value));

		/// <summary>
		/// Writes a VarInt prefixed string.
		/// Minecraft Protocol sends strings as VarInt prefixed
		/// </summary>
		public static void WriteVString(this BinaryWriter binaryWriter, string value)
		{
			binaryWriter.WriteVarInt(value.Length);
			binaryWriter.Write(value.ToCharArray());
		}

		public static T? Write<T>(this BinaryWriter writer, T value, bool? varIntMode = null)
		{
			return (T?)writer.Write(typeof(T), value, varIntMode);
		}

		/// <summary>
		/// Dictionary containing custom converters for <see cref="Write(BinaryWriter, Type, bool?)"/> or <see cref="Write{T}(BinaryWriter, bool?)"/>
		/// </summary>
		public static Dictionary<Type, Action<BinaryWriter, object>> CustomConverters = new();

		public static object? Write(this BinaryWriter writer, Type type, object value, bool? varIntMode = null)
		{
			if (varIntMode == null) varIntMode = VarIntConvert.VarIntMode;
			if (type == typeof(byte)) writer.Write((byte)value);
			if (type == typeof(bool)) writer.Write((bool)value);
			if (type == typeof(char)) writer.Write((char)value);
			if (type == typeof(decimal)) writer.Write((decimal)value);
			if (type == typeof(double)) writer.Write((double)value);
			if (type == typeof(Half)) writer.Write((Half)value);
			if (type == typeof(short)) writer.Write((short)value);
			if (type == typeof(int))
			{
				if (varIntMode == true)
					writer.WriteVarInt((int)value);
				else
					writer.Write((int)value);
			}
			if (type == typeof(long)) writer.Write((long)value);
			if (type == typeof(sbyte)) writer.Write((sbyte)value);
			if (type == typeof(float)) writer.Write((float)value);
			if (type == typeof(string))
			{
				if (varIntMode == true)
					writer.WriteVString((string)value);
				else
					writer.Write((string)value);
			};
			if (type == typeof(ushort)) writer.Write((ushort)value);
			if (type == typeof(uint)) writer.Write((uint)value);
			if (type == typeof(ulong)) writer.Write((ulong)value);
			if (CustomConverters.ContainsKey(type))
			{
				CustomConverters[type](writer, value);
			}
			return null;
		}
	}
}
