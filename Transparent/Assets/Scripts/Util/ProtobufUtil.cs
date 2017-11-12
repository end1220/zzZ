
using System.IO;
using ProtoBuf;


public class ProtobufUtil
{

	public static byte[] Serialize<T>(T t)
	{
		using (MemoryStream ms = new MemoryStream())
		{
			Serializer.Serialize<T>(ms, t);
			return ms.ToArray();
		}
	}

	public static T DeSerialize<T>(byte[] buffer)
	{
		using (MemoryStream ms = new MemoryStream(buffer))
		{
			T t = Serializer.Deserialize<T>(ms);
			return t;
		}
	}

}
