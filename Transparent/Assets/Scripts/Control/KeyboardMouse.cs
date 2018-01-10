using UnityEngine;

namespace Lite
{

	public class KeyboardMouse : MonoBehaviour
	{
		private TransparentWindow window;
		private bool isDragging = false;
		private Vector3 lastDragPosition;

		private void Start()
		{
			window = GetComponent<TransparentWindow>();
		}

		void LateUpdate()
		{
			try
			{
				CheckInput();
			}
			catch (System.Exception e)
			{
				Log.Error(e.ToString());
			}
		}

		void CheckInput()
		{
			float scrollValue = Input.GetAxis("Mouse ScrollWheel");
			SpinCamera.Instance.Scroll(scrollValue);

			// drag window
			if (!isDragging && (Input.GetMouseButton(0) && Input.GetMouseButton(1)))
			{
				isDragging = true;
				lastDragPosition = Input.mousePosition;
				OnDragBegan();
			}
			if (isDragging && (!Input.GetMouseButton(0) || !Input.GetMouseButton(1)))
			{
				isDragging = false;
				OnDragEnd();
			}
			if (isDragging && lastDragPosition != Input.mousePosition)
			{
				Vector3 delta = Input.mousePosition - lastDragPosition;
				lastDragPosition = Input.mousePosition;
				lastDragPosition -= delta;
				OnDragMove(delta.x, delta.y);
			}
			if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
			{
				window.MoveWindow(0, 0);
			}
			else
			{
				// mouse rotate
				if (Input.GetMouseButton(0))
				{
					float rotY = Input.GetAxis("Mouse X");
					float rotX = Input.GetAxis("Mouse Y");
					SpinCamera.Instance.Rotate(-rotX, rotY);
				}
				if (Input.GetMouseButton(2))
				{
					float movy = Input.GetAxis("Mouse X");
					float movx = Input.GetAxis("Mouse Y");
					SpinCamera.Instance.Drag(movy, movx);
				}
				else
				{
					SpinCamera.Instance.ResetDrag();
				}
			}
		}

		private void OnDragBegan()
		{

		}

		private void OnDragEnd()
		{

		}

		private void OnDragMove(float x, float y)
		{
			window.MoveWindow(x, -y);
		}
	}

}
