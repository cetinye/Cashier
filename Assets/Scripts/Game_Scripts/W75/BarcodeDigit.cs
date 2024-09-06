using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cashier
{
	public class BarcodeDigit : MonoBehaviour
	{
		[SerializeField] private float textShowTime;
		[SerializeField] private TMP_Text barcodeText;
		[SerializeField] private Image barcodeImg;
		[SerializeField] private float timeToFade;

		public void SetDigit(int v)
		{
			barcodeText.text = v.ToString();
		}

		public Tween Show()
		{
			return barcodeText.DOFade(1, textShowTime).SetEase(Ease.OutExpo).SetLoops(2, LoopType.Yoyo);
		}

		public Tween SetState(bool state, float timeToFade)
		{
			return barcodeImg.DOFade(state ? 1 : 0, timeToFade);
		}
	}
}
