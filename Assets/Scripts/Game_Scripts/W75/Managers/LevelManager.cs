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
			AssignLevel();
			SpawnProduct();
		}

		void AssignLevel()
		{
			// levelId = PlayerPrefs.GetInt("Cashier_LevelId", 1);
			levelId = Mathf.Clamp(levelId, 1, levels.Count);
			levelSO = levels[levelId - 1];
		}

		void OnStateChanged()
		{
			switch (GameStateManager.GetGameState())
			{
				case GameState.ProductEnter:
					uiManager.SetCashboxTextState(true);
					uiManager.ClearDigitalScreen();
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
			int maxBarcodeLength = levelSO.maxBarcodeLength;
			BarcodeDigitOrder barcodeDigitOrder = (BarcodeDigitOrder)levelSO.barcodeDigitOrder;
			BarcodeDisplayFormat barcodeDisplayFormat = (BarcodeDisplayFormat)levelSO.barcodeDisplayFormat;
			barcodeController.SetBarcode(CreateBarcode(), maxBarcodeLength, barcodeDigitOrder, barcodeDisplayFormat);
			barcodeController.Generate();
		}

		private List<int> CreateBarcode()
		{
			List<int> barcode = new List<int>();
			for (int i = 0; i < levelSO.barcodeLength; i++)
			{
				barcode.Add(Random.Range(0, 10));
			}
			return barcode;
		}

		public void NumberPressed(int number)
		{
			pressedNumbers.Add(number);
			uiManager.AddToDigitalScreen(number);
		}
	}
}
