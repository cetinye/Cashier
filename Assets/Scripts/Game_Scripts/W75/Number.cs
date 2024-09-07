using UnityEngine;

namespace Cashier
{
	public class Number : MonoBehaviour
	{
		public static bool isPressable = false;

		[SerializeField] private LevelManager levelManager;

		public void ButtonDown(int value)
		{
			if (isPressable)
			{
				AudioManager.instance.PlayOneShot(SoundType.OneNumber);
				levelManager.NumberPressed(value);
			}
		}
	}
}
