
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpinCamera : MonoBehaviour
{
	public static SpinCamera Instance { get; private set; }

	// camera positon
	public float Distance = 12;
	public float angelX = 60;
	public float angelY = 0;
	private float SpeedX = 120;
	private float SpeedY = 240;
	private float minAngelX = 5;
	private float maxAngelX = 80;
	//鼠标缩放距离最值
	private float MaxDistance = 40;
	private float MinDistance = 5f;
	//鼠标缩放速率
	public float ZoomSpeed = 5f;

	//是否启用差值
	public bool EnableDamping = true;
	//速度
	public float Damping = 5f;

	public GameObject target;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
	}

	void Update()
	{
		
	}

	void LateUpdate()
	{
        try
        {
            AdjustCamera();
        }
        catch (UnityException ue)
        {
            Log.Instance.Error(ue.ToString());
        }
	}

	public void Scroll(float scrollValue)
	{
		Distance -= scrollValue * ZoomSpeed;
		Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
	}

	public void Rotate(float x, float y)
	{
		angelX += x * SpeedX * Time.deltaTime;
		angelX = ClampAngle(angelX, minAngelX, maxAngelX);
		angelY += y * SpeedY * Time.deltaTime;
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
		
		Vector3 targetPosition = target.transform.position + Vector3.up * 4;

		if (force)
		{
			Quaternion rotation = Quaternion.Euler(angelX, angelY, 0);
			Vector3 position = rotation * new Vector3(0, 0, -Distance) + targetPosition;
			transform.rotation = rotation;
			transform.position = position;
			force = false;
		}
		else
		{
			Quaternion rotation = Quaternion.Euler(angelX, angelY, 0);
			if (EnableDamping)
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * Damping);
				Vector3 position = transform.rotation * new Vector3(0, 0, -Distance) + targetPosition;
				transform.position = position;
			}
			else
			{
				Vector3 position = rotation * new Vector3(0, 0, -Distance) + targetPosition;
				transform.rotation = rotation;
				transform.position = position;
			}
		}

	}

}
