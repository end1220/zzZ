
using System.Collections.Generic;


namespace Float
{
	public delegate void CmdCallback(object[] Args);

	public class MsgSystem : IManager
	{
		public struct Message
		{
			public int Id;

			public object[] Args;

			public Message(int _id, object[] args)
			{
				Id = _id;
				Args = args;
			}
		}

		protected readonly object SyncMsgMap = new object();
		protected Dictionary<int, CmdCallback> MsgMap = new Dictionary<int, CmdCallback>();
		protected readonly object SyncMsgQ = new object();
		protected Queue<Message> MsgQueue = new Queue<Message>();

		public override void Tick()
		{
			lock (SyncMsgQ)
			{
				while (MsgQueue.Count > 0)
				{
					Message msg = MsgQueue.Dequeue();
					Execute(msg.Id, msg.Args);
				}
			}
		}

		public void Push(int id, params object[] args)
		{
			lock (SyncMsgQ)
			{
				MsgQueue.Enqueue(new Message(id, args));
			}
		}

		public virtual void Execute(int id, params object[] args)
		{
			lock (SyncMsgMap)
			{
				CmdCallback func;
				MsgMap.TryGetValue(id, out func);
				if (func != null)
					func.Invoke(args);
			}
		}

		public virtual void Register(int id, CmdCallback func)
		{
			lock (SyncMsgMap)
			{
				if (!MsgMap.ContainsKey(id))
					MsgMap.Add(id, func);
				else
					MsgMap[id] += func;
			}
		}

		public virtual void Unregister(int id, CmdCallback func)
		{
			lock (SyncMsgMap)
			{
				if (MsgMap.ContainsKey(id))
				{
					MsgMap[id] -= func;
					if (MsgMap[id] == null)
						MsgMap.Remove(id);
				}
			}
		}

	}
}