
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

		//重新计算位置和角度
		Quaternion mRotation = Quaternion.Euler(angelX, angelY, 0);
		Vector3 mPosition = mRotation * new Vector3(0, 0, -Distance) + targetPosition;

		//设置相机的角度和位置
		if (!force && EnableDamping)
		{
			//球形插值
			transform.rotation = Quaternion.Lerp(transform.rotation, mRotation, Time.deltaTime * Damping);
			//线性插值
			transform.position = Vector3.Lerp(transform.position, mPosition, Time.deltaTime * Damping);
		}
		else
		{
			force = false;
			transform.rotation = mRotation;
			transform.position = mPosition;
		}

	}

}
