
using UnityEngine;


namespace Lite
{

	public class SpinCamera : MonoBehaviour
	{
		public static SpinCamera Instance { get; private set; }

		public float distance = 12;
		public float minDistance = 5f;
		public float maxDistance = 40;

		public float angelX = 0;
		public float angelY = 180;
		private float angelSpeedX = 360;
		private float angelSpeedY = 720;
		public float minAngelX = -80;
		public float maxAngelX = 80;

		public float zoomSpeed = 5f;

		private float dragSpeed = 1;
		private Vector3 dragOffset = Vector3.zero;

		public bool enableDamping = true;
		public float damping = 50f;

		public Vector3 target;


		// Original values
		private float oriAngelX;
		private float oriAngelY;

		void Awake()
		{
			Instance = this;
			oriAngelX = angelX;
			oriAngelY = angelY;
		}

		void LateUpdate()
		{
			try
			{
				AdjustCamera();
			}
			catch (UnityException ue)
			{
				Log.Error(ue.ToString());
			}
		}

		public void Reset()
		{
			angelX = oriAngelX;
			angelY = oriAngelY;
			dragOffset = Vector3.zero;
		}

		public void Scroll(float scrollValue)
		{
			distance -= scrollValue * zoomSpeed;
			distance = Mathf.Clamp(distance, minDistance, maxDistance);
		}

		public void Rotate(float x, float y)
		{
			angelX += x * angelSpeedX * Time.deltaTime;
			angelX = ClampAngle(angelX, minAngelX, maxAngelX);
			angelY += y * angelSpeedY * Time.deltaTime;
			angelY = ClampAngle(angelY, -360, 360);
		}

		float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360)
				angle += 360;
			if (angle > 360)
				angle -= 360;
			return Mathf.Clamp(angle, min, max);
		}

		public void Drag(float x, float y)
		{
			dragSpeed = distance * 2f;
			Vector3 rightOffset = x * Vector3.right * dragSpeed * Time.deltaTime;
			Vector3 upOffset = y * Vector3.up * dragSpeed * Time.deltaTime;
			dragOffset += rightOffset + upOffset;
		}

		public void ResetDrag()
		{
			dragOffset = Vector3.zero;
		}

		bool force = true;
		private void AdjustCamera()
		{
			if (null == target)
				return;

			Vector3 targetPosition = target/*.position + Vector3.up * 1*/;

			if (force)
			{
				Quaternion rotation = Quaternion.Euler(angelX, angelY, 0);
				Vector3 position = rotation * new Vector3(0, 0, -distance) + targetPosition;
				transform.rotation = rotation;
				transform.position = position;
				force = false;
			}
			else
			{
				Quaternion rotation = Quaternion.Euler(angelX, angelY, 0);
				if (enableDamping)
				{
					transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * damping);
					Vector3 position = transform.rotation * new Vector3(0, 0, -distance) + targetPosition;
					transform.position = position;
				}
				else
				{
					Vector3 position = rotation * new Vector3(0, 0, -distance) + targetPosition;
					transform.rotation = rotation;
					transform.position = position;
				}
				transform.position += transform.rotation * dragOffset;
			}

		}

	}

}