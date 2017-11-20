

namespace Lite
{
	public struct Packet
	{
		public ushort length;
		public ushort msgId;
		public int stamp;
		public byte[] data;
	}

}