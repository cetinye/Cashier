using UnityEngine;

namespace Cashier
{
	public class LevelSO : ScriptableObject
	{
		public int levelId;
		public int numOfItemsDisplayedPerLevel;
		public int maxBarcodeLength;
		public int barcodeLength;
		public int barcodeEntryTime;
		public int barcodeDigitOrder;
		public int barcodeDisplayFormat;
		public int numOfQuestionsToLevelUp;
		public int numOfQuestionsToLevelDown;
		public int totalItemsShownInEndGame;
		public int totalNumberOfFakeItems;
		public int pointsPerCorrectAnswer;
		public int maxInGame;
		public int penaltyPoints;
	}
}
