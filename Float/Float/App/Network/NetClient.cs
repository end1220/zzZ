

namespace Lite
{

	public abstract class NetClient
	{
		public virtual void Init() { }
		public virtual void Destroy() { }
		public virtual void Send(byte[] buffer) { }
	}

}