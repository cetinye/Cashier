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
				levelManager.NumberPressed(value);
			}
		}
	}
}
