# Dennis.BinaryUtils

net6.0 Library for BinaryWriter and BinaryReader utilities. Oh and also VarInt support

## Examples
```cs
byte[] data = VarIntConvert.VarIntToBytes(21);
int num = VarIntConvert.ReadVarInt(data);
// num == 21
```

```cs
var ms = new MemoryStream();
var writer = new BinaryWriter(ms);

writer.WriteVarInt(367);
```

Generic reading/writing
```cs
reader.Read<string>();
reader.Read<CustomObject>();
reader.Read<byte>();
// varint arg can be turned on by default using VarIntConvert.VarIntMode
reader.Read<string>(varint: true);

writer.Write<byte>(0x01);
// your IDE will make that this:
writer.Write(0x01);

// but you can use that generic like this:
public void DoThing<T>(T thing)
{
	Writer.Write<T>(thing);
}
```

Custom reading/writing
```cs
WriterExtensions.CustomConverters[typeof(Square)] = (BinaryWriter w, object o) =>
{
	Square sq = (Square)o;
	w.Write(sq.Width);
	w.Write(sq.Height);
};

ReaderExtensions.CustomConverters[typeof(Square)] = (BinaryReader r) =>
{
	var sq = new Square();
	sq.Width = r.Read<int>();
	sq.Height = r.Read<int>();
	return sq;
};

reader.Read<Square>();
// Write also accepts Square now
```