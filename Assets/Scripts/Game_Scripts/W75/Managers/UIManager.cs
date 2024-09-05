using System.Collections;
using System.Collections.Generic;
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

		[Header("Character Animations")]
		[SerializeField] private List<GameObject> characters = new List<GameObject>();
		[SerializeField] Transform rightEdge, leftEdge;

		void Start()
		{
			PlayCharacterAnim();
		}

		public void SetCashboxTextState(bool value)
		{
			textOnCashbox.DOFade(value ? 1f : 0f, textOnCashboxFadeTime);
		}

		public void AddToDigitalScreen(int value)
		{
			textOnDigitalScreen.text += value.ToString();
		}

		public void DeleteLastDigit()
		{
			textOnDigitalScreen.text = textOnDigitalScreen.text.Substring(0, textOnDigitalScreen.text.Length - 1);
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

		private void PlayCharacterAnim()
		{
			StartCoroutine(StartCharAnims());
		}

		IEnumerator StartCharAnims()
		{
			for (int i = 0; i < characters.Count; i++)
			{
				StartCoroutine(CharacterAnim(characters[i]));
				yield return new WaitForSeconds(1.33f);
			}
		}

		IEnumerator CharacterAnim(GameObject character)
		{
			if (character.transform.localScale.x > 0)
			{
				Animator animController = character.GetComponent<Animator>();
				animController.enabled = true;
				Tween moveCharLeft = character.transform.DOLocalMove(leftEdge.localPosition, 7f);
				yield return moveCharLeft.WaitForCompletion();
				animController.enabled = false;

				character.transform.localScale = new Vector3(-character.transform.localScale.x, character.transform.localScale.y, character.transform.localScale.z);
				animController.enabled = true;
				Tween moveCharRight = character.transform.DOLocalMove(rightEdge.localPosition, 7f);
				yield return moveCharRight.WaitForCompletion();
				animController.enabled = false;
				character.transform.localScale = new Vector3(-character.transform.localScale.x, character.transform.localScale.y, character.transform.localScale.z);
			}
			else
			{
				Animator animController = character.GetComponent<Animator>();
				animController.enabled = true;
				Tween moveCharRight = character.transform.DOLocalMove(rightEdge.localPosition, 7f);
				yield return moveCharRight.WaitForCompletion();
				animController.enabled = false;

				character.transform.localScale = new Vector3(-character.transform.localScale.x, character.transform.localScale.y, character.transform.localScale.z);
				animController = character.GetComponent<Animator>();
				animController.enabled = true;
				Tween moveCharLeft = character.transform.DOLocalMove(leftEdge.localPosition, 7f);
				yield return moveCharLeft.WaitForCompletion();
				animController.enabled = false;
				character.transform.localScale = new Vector3(-character.transform.localScale.x, character.transform.localScale.y, character.transform.localScale.z);
			}

			yield return CharacterAnim(character);
		}
	}
}
