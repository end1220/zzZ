
using UnityEngine;


public class SpinCamera : MonoBehaviour
{
	public static SpinCamera Instance { get; private set; }

	public float distance = 12;
	public float minDistance = 5f;
	public float maxDistance = 40;

	public float angelX = 60;
	public float angelY = 0;
	private float speedX = 120;
	private float speedY = 240;
	public float minAngelX = 5;
	public float maxAngelX = 80;
	
	public float zoomSpeed = 5f;

	public bool enableDamping = true;
	public float damping = 5f;

	public Vector3 target;

	void Awake()
	{
		Instance = this;
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

	public void Scroll(float scrollValue)
	{
		distance -= scrollValue * zoomSpeed;
		distance = Mathf.Clamp(distance, minDistance, maxDistance);
	}

	public void Rotate(float x, float y)
	{
		angelX += x * speedX * Time.deltaTime;
		angelX = ClampAngle(angelX, minAngelX, maxAngelX);
		angelY += y * speedY * Time.deltaTime;
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
		}

	}

}
