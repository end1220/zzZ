
using UnityEngine;


public class BreathLoop : MonoBehaviour
{
	public float speed = 1.0f;
	public float minSize = 0.8f;
	public float maxSize = 1.5f;
	private float angle = 0;

	private void Update()
	{
		angle += speed * Time.deltaTime;
		angle %= 2 * Mathf.PI;
		float midSize = (minSize + maxSize) * 0.5f;
		float factor = (maxSize - minSize) * 0.5f;
		float scale = Mathf.Cos(angle) * factor + midSize;
		transform.localScale = Vector3.one * scale;
	}

}
