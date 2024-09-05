using System.Collections.Generic;
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
		[SerializeField] private BarcodeController barcodeController;

		void Start()
		{
			StartGame();
		}

		void StartGame()
		{
			AssignLevel();
			GenerateBarcode();
		}

		void AssignLevel()
		{
			// levelId = PlayerPrefs.GetInt("Cashier_LevelId", 1);
			levelId = Mathf.Clamp(levelId, 1, levels.Count);
			levelSO = levels[levelId - 1];
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
	}
}
