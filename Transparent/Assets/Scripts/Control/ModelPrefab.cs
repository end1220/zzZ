﻿
using UnityEngine;



public class ModelPrefab : MonoBehaviour
{
	public float minCameraDistance = 2;

	public float maxCameraDistance = 5;

	public float defaultCameraDistance = 3;

	public Vector3 lookAtOffset;

	//public Vector3 scale;

	private void OnDrawGizmos()
	{
		/*Matrix4x4 defaultMatrix = Gizmos.matrix;
		Gizmos.matrix = transform.localToWorldMatrix;*/

		Color defaultColor = Gizmos.color;

		Vector3 target = transform.position + lookAtOffset;
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(target, minCameraDistance);
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(target, maxCameraDistance);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(target, defaultCameraDistance);

		Gizmos.color = defaultColor;
		//Gizmos.matrix = defaultMatrix;
	}

}
