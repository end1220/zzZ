
using UnityEngine;

namespace Lite
{

	public class AnimationPlayer : MonoBehaviour
	{
		public Animator animator;

		public void CrossFade()
		{
			//animator.CrossFade();
		}

		public void SetBool(string name, bool value)
		{
			animator.SetBool(name, value);
		}

		public void SetInteger(string name, int value)
		{
			animator.SetInteger(name, value);
		}

		public void SetFloat(string name, float value)
		{
			animator.SetFloat(name, value);
		}
	}

}