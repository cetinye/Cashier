using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Cashier
{
	public class LevelManager : MonoBehaviour
	{
		[Header("Level Variables")]
		[SerializeField] private int levelId;
		[SerializeField] private LevelSO levelSO;
		[SerializeField] private List<LevelSO> levels = new List<LevelSO>();

		[Header("Controllers")]
		[SerializeField] private UIManager uiManager;
		[SerializeField] private BarcodeController barcodeController;

		[Header("Lists")]
		[SerializeField] private List<Sprite> productSprites = new List<Sprite>();
		private List<Sprite> chosenProducts = new List<Sprite>();

		[Header("Product")]
		[SerializeField] private Product product;
		private Sprite chosenProductSp;

		private List<int> pressedNumbers = new List<int>();
		private bool enterPressed = false;

		void OnEnable()
		{
			GameStateManager.OnGameStateChanged += OnStateChanged;
		}

		void OnDisable()
		{
			GameStateManager.OnGameStateChanged -= OnStateChanged;
		}

		void Start()
		{
			StartGame();
		}

		void StartGame()
		{
			Reset();
			barcodeController.Reset();
			product.Reset();
			uiManager.ClearDigitalScreen();
			uiManager.ClearProductOnCashbox();

			AssignLevel();
			SpawnProduct();
		}

		void AssignLevel()
		{
			levelId = PlayerPrefs.GetInt("Cashier_LevelId", 1);
			levelId = Mathf.Clamp(levelId, 1, levels.Count);
			levelSO = levels[levelId - 1];

			uiManager.SetLevelTime(levelSO.barcodeEntryTime);
		}

		void OnStateChanged()
		{
			switch (GameStateManager.GetGameState())
			{
				case GameState.Idle:
					enterPressed = false;
					break;

				case GameState.ProductEnter:
					Reset();
					barcodeController.Reset();
					product.Reset();
					uiManager.SetCashboxTextState(true);
					uiManager.ClearDigitalScreen();
					uiManager.ClearProductOnCashbox();
					SpawnProduct();
					break;

				case GameState.BarcodeShow:
					GenerateBarcode();
					break;

				case GameState.EnterBarcode:
					uiManager.SetProductOnCashbox(chosenProductSp);
					Number.isPressable = true;
					uiManager.SetCashboxTextState(false);
					break;

				case GameState.ProductExit:
					uiManager.ClearDigitalScreen();
					Number.isPressable = false;
					break;

				case GameState.TimesUp:
					Number.isPressable = false;
					uiManager.GiveCashboxFeedback(false, true);
					product.Exit().OnComplete(() => StartGame());
					break;

				case GameState.MemoryGame:
					break;

				case GameState.GameOver:
					break;
			}
		}

		void SpawnProduct()
		{
			do
			{
				chosenProductSp = productSprites[Random.Range(0, productSprites.Count)];
			} while (chosenProducts.Contains(chosenProductSp));
			chosenProducts.Add(chosenProductSp);
			product.SetSprite(chosenProductSp);

			product.Enter().OnComplete(() =>
			{
				GameStateManager.SetGameState(GameState.BarcodeShow);
			});
		}

		void GenerateBarcode()
		{
			int barcodeLength = levelSO.barcodeLength;
			int maxBarcodeLength = levelSO.maxBarcodeLength;
			BarcodeDigitOrder barcodeDigitOrder = (BarcodeDigitOrder)levelSO.barcodeDigitOrder;
			BarcodeDisplayFormat barcodeDisplayFormat = (BarcodeDisplayFormat)levelSO.barcodeDisplayFormat;
			barcodeController.SetBarcode(barcodeLength, maxBarcodeLength, barcodeDigitOrder, barcodeDisplayFormat);
		}

		public void NumberPressed(int number)
		{
			pressedNumbers.Add(number);
			uiManager.AddToDigitalScreen(number);
		}

		public void Delete()
		{
			if (GameStateManager.GetGameState() != GameState.EnterBarcode)
				return;

			if (pressedNumbers.Count > 0)
			{
				pressedNumbers.RemoveAt(pressedNumbers.Count - 1);
				uiManager.DeleteLastDigit();
			}
		}

		public void Check()
		{
			if (GameStateManager.GetGameState() != GameState.EnterBarcode)
				return;

			if (!enterPressed)
				enterPressed = true;

			GameStateManager.SetGameState(GameState.Idle);

			List<int> barcode = new List<int>(barcodeController.GetBarcode());

			for (int i = 0; i < barcode.Count; i++)
			{
				if (pressedNumbers.Count != barcode.Count)
				{
					Wrong();
					return;
				}

				if (barcode[i] != pressedNumbers[i])
				{
					Wrong();
					return;
				}
			}

			Correct();
		}

		public void Correct()
		{
			uiManager.GiveCashboxFeedback(true);

			DOTween.Sequence()
				.Append(product.transform.DOScale(350f, 0.5f))
				.AppendInterval(0.3f)
				.Append(product.transform.DOScale(300f, 0.3f))
				.OnComplete(() =>
				{
					product.Exit().OnComplete(() =>
					{
						levelId++;
						PlayerPrefs.SetInt("Cashier_LevelId", levelId);
						StartGame();
					});
				});
		}

		public void Wrong()
		{
			float rotation = 15f;

			uiManager.GiveCashboxFeedback(false);

			Sequence seq = DOTween.Sequence()
				.Append(product.transform.DORotate(new Vector3(0, 0, rotation), 0.1f))
				.Append(product.transform.DORotate(new Vector3(0, 0, -rotation), 0.1f))
				.SetLoops(2);

			seq.OnComplete(() =>
			{
				product.transform.DORotate(new Vector3(0, 0, 0), 0.2f);
				product.Exit().OnComplete(() => StartGame());
			});
		}

		private void Reset()
		{
			pressedNumbers.Clear();
			enterPressed = false;
		}
	}
}
