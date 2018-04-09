
using UnityEngine;
using UnityEngine.UI;


namespace Float
{
	public class DebugPanel : MonoBehaviour
	{
		private Button button;

		private InputField input;

		private void Awake()
		{
			button = transform.Find("Button").GetComponent<Button>();
			input = transform.Find("InputField").GetComponent<InputField>();
			button.onClick.AddListener(delegate() { OnClickButton(); });
		}

		private void OnClickButton()
		{
			string str = input.text;
			int id = int.Parse(str);
			var cmd = new Command(id);
			NetworkManager.Instance.SendBytes((ushort)CommandId.PlayThisOne, ProtobufUtil.Serialize(cmd));
		}

	}
}