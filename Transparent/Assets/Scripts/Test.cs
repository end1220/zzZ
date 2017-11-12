

using UnityEngine;


public class Test: MonoBehaviour
{
	Send send = new Send();

	private void OnGUI()
	{
		if (GUI.Button(new Rect(10, 10, 100, 30), "send"))
		{
			var id = CommandId.ShowWindow;
			Command cmd = new Command();
			cmd.number1 = 1;
			cmd.number2 = 22;
			cmd.number3 = 33;
			cmd.string1 = "s1111";
			cmd.string2 = "s2222";
			cmd.string3 = "s3333";
			byte[] bytes = ProtobufUtil.Serialize<Command>(cmd);
			send.SendTest((ushort)id, bytes);
		}
	}

}