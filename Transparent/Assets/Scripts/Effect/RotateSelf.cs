using UnityEngine;


namespace Float
{

	public class RotateSelf : MonoBehaviour
	{
		public float x = 0f;
		public float y = 0f;
		public float z = 0f;


		void Update()
		{
			if (!Mathf.Approximately(x, 0))
				transform.Rotate(Time.deltaTime * x, 0, 0, Space.World);
			if (!Mathf.Approximately(y, 0))
				transform.Rotate(0, Time.deltaTime * y, 0, Space.World);
			if (!Mathf.Approximately(z, 0))
				transform.Rotate(0, 0, Time.deltaTime * z, Space.World);
		}

	}

}