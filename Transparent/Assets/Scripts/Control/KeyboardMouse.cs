using UnityEngine;


public class KeyboardMouse : MonoBehaviour
{
	private float lastMultiTouchDistance = 0;
	private bool isMultiTouching = false;

	void Start()
	{

	}

	void Update()
	{
		try
		{
			Check();
		}
		catch (System.Exception e)
		{
			Log.Error(e.ToString());
		}
	}

	void Check()
	{
		// mouse scroll
		float scrollValue = Input.GetAxis("Mouse ScrollWheel");
		SpinCamera.Instance.Scroll(scrollValue);

		// mouse rotate
		if (Input.GetMouseButton(0))
		{
			float rotY = Input.GetAxis("Mouse X");
			float rotX = Input.GetAxis("Mouse Y");
			SpinCamera.Instance.Rotate(-rotX, rotY);
		}

		// keyboard
		int x = 0;
		int z = 0;
		z += Input.GetKey(KeyCode.W) ? 1 : 0;
		z -= Input.GetKey(KeyCode.S) ? 1 : 0;
		x -= Input.GetKey(KeyCode.A) ? 1 : 0;
		x += Input.GetKey(KeyCode.D) ? 1 : 0;

		if (x != 0 || z != 0)
		{

		}

	}

}
