using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cashier
{
	public class UIManager : MonoBehaviour
	{
		[SerializeField] private TMP_Text textOnCashbox;
		[SerializeField] private TMP_Text textOnDigitalScreen;
		[SerializeField] private Image prodOnCashbox;
		[SerializeField] private float textOnCashboxFadeTime;

		public void SetCashboxTextState(bool value)
		{
			textOnCashbox.DOFade(value ? 1f : 0f, textOnCashboxFadeTime);
		}

		public void AddToDigitalScreen(int value)
		{
			textOnDigitalScreen.text += value.ToString();
		}

		public void ClearDigitalScreen()
		{
			textOnDigitalScreen.text = "";
		}

		public void SetProductOnCashbox(Sprite sprite)
		{
			prodOnCashbox.sprite = sprite;
			prodOnCashbox.DOFade(1f, textOnCashboxFadeTime);
		}

		public void ClearProductOnCashbox()
		{
			prodOnCashbox.DOFade(0f, textOnCashboxFadeTime);
		}
	}
}
