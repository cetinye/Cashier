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

		[Header("Memory Panel")]
		[SerializeField] private GameObject questionPanel;
		[SerializeField] private Image questionProduct;
		[SerializeField] private Image questionCorrect;
		[SerializeField] private Image questionWrong;
		[SerializeField] private RectTransform questionProductRect;
		[SerializeField] private RectTransform questionProductPosStart;
		[SerializeField] private RectTransform questionProductPos;
		[SerializeField] private RectTransform questionProductPosEnd;
		private bool isQuestionAnswerable = false;
		private float questionTime;
		private bool isQuestionTimerOn;
		private bool questionCountdownLock;
		private bool questionWrongFlag;
		[SerializeField] private List<Sprite> productSprites = new List<Sprite>();
		private List<Sprite> shownProducts = new List<Sprite>();
		private List<Sprite> wrongProducts = new List<Sprite>();
		private int shownProductIndex;
		private int questionCorrectCount;
		private int questionWrongCount;

		void Start()
		{
			PlayCharacterAnim();
		}

		void Update()
		{
			QuestionCountdown();
			LevelTimer();
		}

		private void LevelTimer()
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

		public void ClearLevelTimer()
		{
			timerText.text = "";
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
			AddToShownProducts(sprite);
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

		#region Memory Panel

		void ShowQuestionPanel()
		{
			questionProduct.color = new Color(255f, 255f, 255f, 0);
			questionProductRect.anchoredPosition = questionProductPosStart.anchoredPosition;
			questionPanel.SetActive(true);

			questionProduct.DOFade(1, 0.25f);
			questionProductRect.DOAnchorPos(questionProductPos.anchoredPosition, 0.5f).OnComplete(() =>
			{
				isQuestionAnswerable = true;
				questionTime = 10f;
				isQuestionTimerOn = true;
				questionCountdownLock = false;
			});

			if (shownProducts.Count > shownProductIndex)
				questionProduct.sprite = shownProducts[shownProductIndex];
			else
			{
				// StartCoroutine(gameOver());
				Debug.Log("Game Over");
				UnityEngine.SceneManagement.SceneManager.LoadScene(0);
			}
		}

		public void CheckShownProduct(bool userChoice)
		{
			if (!isQuestionAnswerable) return;

			isQuestionTimerOn = false;
			isQuestionAnswerable = false;

			if (!wrongProducts.Contains(questionProduct.sprite) == userChoice)
			{
				shownProductIndex++;
				// W75_AudioManager.instance.PlayOneShot("CorrectOnPanel");
				questionCorrectCount++;
				questionCorrect.enabled = true;
				questionCorrect.transform.DOShakePosition(0.3f, 0.5f);

				questionProduct.DOFade(0, 0.25f);
				questionProductRect.DOAnchorPos(questionProductPosEnd.anchoredPosition, 0.5f).OnComplete(() =>
				{
					ShowQuestionPanel();
					questionCorrect.enabled = false;
				});

			}
			else
			{
				shownProductIndex++;
				// W75_AudioManager.instance.PlayOneShot("Wrong");
				questionWrongCount++;

				if (questionWrongFlag)
				{
					questionWrongFlag = false;
					// LevelUp(false);
				}

				questionWrong.enabled = true;
				questionWrong.transform.DOShakePosition(0.3f, 0.5f);

				questionProduct.DOFade(0, 0.25f);
				questionProductRect.DOAnchorPos(questionProductPosEnd.anchoredPosition, 0.5f).OnComplete(() =>
				{
					ShowQuestionPanel();
					questionWrong.enabled = false;
				});
			}
		}

		public void AddToShownProducts(Sprite sprite)
		{
			if (shownProducts.Contains(sprite)) return;

			shownProducts.Add(sprite);
		}

		public void AddRandomSpritesToQuestion(int amount)
		{
			for (int i = 0; i < amount; i++)
			{
				Sprite randProduct = productSprites[Random.Range(0, productSprites.Count)];

				if (!shownProducts.Contains(randProduct))
				{
					wrongProducts.Add(randProduct);
					shownProducts.Add(randProduct);
				}
				else
					i--;
			}

			shownProducts.Shuffle();
			ShowQuestionPanel();
		}

		private void QuestionCountdown()
		{
			if (GameStateManager.GetGameState() != GameState.MemoryGame)
				return;

			if (isQuestionTimerOn)
			{
				questionTime -= Time.deltaTime;
				timerText.text = ((int)questionTime).ToString();

				if (questionTime < 0)
				{
					isQuestionTimerOn = false;
					questionCountdownLock = true;
				}
			}
			else if (!isQuestionTimerOn && questionCountdownLock)
			{
				questionCountdownLock = false;
				shownProductIndex++;
				// W75_AudioManager.instance.PlayOneShot("Wrong");
				questionWrong.enabled = true;
				questionWrong.transform.DOShakePosition(0.3f, 0.5f);

				questionProduct.DOFade(0, 0.25f);
				questionProductRect.DOAnchorPos(questionProductPosEnd.anchoredPosition, 0.5f).OnComplete(() =>
				{
					ShowQuestionPanel();
					questionWrong.enabled = false;
				});
				questionTime = 10f;
			}
		}

		#endregion

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

	public static class IListExtensions
	{
		/// <summary>
		/// Shuffles the element order of the specified list.
		/// </summary>
		public static void Shuffle<T>(this IList<T> ts)
		{
			var count = ts.Count;
			var last = count - 1;
			for (var i = 0; i < last; ++i)
			{
				var r = UnityEngine.Random.Range(i, count);
				var tmp = ts[i];
				ts[i] = ts[r];
				ts[r] = tmp;
			}
		}
	}
}
