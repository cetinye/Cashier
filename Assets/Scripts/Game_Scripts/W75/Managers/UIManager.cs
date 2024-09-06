using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cashier
{
	public class UIManager : MonoBehaviour
	{
		[Header("Cashbox UI Elements")]
		[SerializeField] private TMP_Text textOnCashbox;
		[SerializeField] private TMP_Text textOnDigitalScreen;
		[SerializeField] private Image prodOnCashbox;
		[SerializeField] private Image correctImg;
		[SerializeField] private Image wrongImg;
		[SerializeField] private float textOnCashboxFadeTime;
		[SerializeField] private TMP_Text resultTxt;
		[SerializeField] private Image resultSp;
		[SerializeField] private GameObject resultPanel;

		[Header("Timer Variables")]
		[SerializeField] private TMP_Text timerText;
		private float timer;
		private float levelTime;

		[Header("Character Animations")]
		[SerializeField] private List<GameObject> characters = new List<GameObject>();
		[SerializeField] Transform rightEdge, leftEdge;

		void Start()
		{
			PlayCharacterAnim();
		}

		void Update()
		{
			if (GameStateManager.GetGameState() != GameState.EnterBarcode)
				return;

			timer -= Time.deltaTime;
			timerText.text = ((int)timer).ToString();

			if (timer < 0)
			{
				GameStateManager.SetGameState(GameState.TimesUp);
				timer = levelTime;
				timerText.text = "";
			}
		}

		public void SetLevelTime(int value)
		{
			levelTime = value;
			timer = levelTime;
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

		public void GiveCashboxFeedback(bool isCorrect, bool isTimesUp = false)
		{
			if (isCorrect)
			{
				correctImg.DOFade(1f, textOnCashboxFadeTime).OnComplete(() => correctImg.DOFade(0f, textOnCashboxFadeTime));
				SetDigitalScreenText("CORRECT", new Color(0f, 1f, 0f), new Color(0f, 0.75f, 0f));
			}
			else
			{
				wrongImg.DOFade(1f, textOnCashboxFadeTime).OnComplete(() => wrongImg.DOFade(0f, textOnCashboxFadeTime));

				if (isTimesUp)
					SetDigitalScreenText("TIME'S UP", new Color(1f, 0f, 0f), new Color(0.75f, 0f, 0f));
				else
					SetDigitalScreenText("WRONG", new Color(1f, 0f, 0f), new Color(0.75f, 0f, 0f));
			}
		}

		private void SetDigitalScreenText(string msg, Color color, Color color2)
		{
			ClearDigitalScreen();

			resultTxt.text = msg;
			resultTxt.color = Color.black;
			resultSp.color = color;
			resultPanel.SetActive(true);

			Sequence seq = DOTween.Sequence()
				  .Append(resultSp.DOColor(color, 0.2f))
				  .Append(resultSp.DOColor(color2, 0.2f))
				  .SetLoops(4);

			seq.OnComplete(() => { resultPanel.SetActive(false); });
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
