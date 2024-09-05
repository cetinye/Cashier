using System;

namespace Cashier
{
    public static class GameStateManager
    {
        private static GameState gameState = GameState.Idle;
        public static event Action OnGameStateChanged;

        public static void SetGameState(GameState newGameState)
        {
            gameState = newGameState;
            OnGameStateChanged?.Invoke();
        }

        public static GameState GetGameState()
        {
            return gameState;
        }
    }

    public enum GameState
    {
        Idle,
        ProductEnter,
        BarcodeShow,
        EnterBarcode,
        ProductExit,
        TimesUp,
        MemoryGame,
        GameOver,
    }
}