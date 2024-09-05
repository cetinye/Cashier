using UnityEngine;
using UnityEditor;
using System.IO;

namespace Cashier
{
    public class CSVtoSO_Cashier
    {
        //Check .csv path
        private static string CSVPath = "/Editor/LevelCSV_Cashier.csv";

        [MenuItem("Tools/CSV_to_SO/Cashier/Generate")]
        public static void GenerateSO()
        {
            int startingNamingIndex = 1;
            string[] allLines = File.ReadAllLines(Application.dataPath + CSVPath);

            for (int i = 1; i < allLines.Length; i++)
            {
                allLines[i] = RedefineString(allLines[i]);
            }

            for (int i = 1; i < allLines.Length; i++)
            {
                string[] splitData = allLines[i].Split(';');

                //Check data indexes
                LevelSO level = ScriptableObject.CreateInstance<LevelSO>();
                level.levelId = int.Parse(splitData[0]);
                level.numOfItemsDisplayedPerLevel = int.Parse(splitData[1]);
                level.maxBarcodeLength = int.Parse(splitData[2]);
                level.barcodeLength = int.Parse(splitData[3]);
                level.barcodeEntryTime = int.Parse(splitData[4]);
                level.barcodeDigitOrder = int.Parse(splitData[5]);
                level.barcodeDisplayFormat = int.Parse(splitData[6]);
                level.numOfQuestionsToLevelUp = int.Parse(splitData[7]);
                level.numOfQuestionsToLevelDown = int.Parse(splitData[8]);
                level.totalItemsShownInEndGame = int.Parse(splitData[9]);
                level.totalNumberOfFakeItems = int.Parse(splitData[10]);
                level.pointsPerCorrectAnswer = int.Parse(splitData[11]);
                level.maxInGame = int.Parse(splitData[12]);
                level.penaltyPoints = int.Parse(splitData[13]);

                AssetDatabase.CreateAsset(level, $"Assets/Data/Cashier/Levels/{"Cashier_Level " + startingNamingIndex}.asset");
                startingNamingIndex++;
            }

            AssetDatabase.SaveAssets();

            static string RedefineString(string val)
            {
                char[] charArr = val.ToCharArray();
                bool isSplittable = true;

                for (int i = 0; i < charArr.Length; i++)
                {
                    if (charArr[i] == '"')
                    {
                        charArr[i] = ' ';
                        isSplittable = !isSplittable;
                    }

                    if (isSplittable && charArr[i] == ',')
                        charArr[i] = ';';

                    if (isSplittable && charArr[i] == '.')
                        charArr[i] = ',';
                }

                return new string(charArr);
            }
        }
    }
}