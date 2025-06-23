using UnityEngine;
using Zenject;
using UnityEngine.SceneManagement;
using System;

namespace GameManagement
{
    public class GameManager : IInitializable, IDisposable
    {
        public GameState CurrentState { get; private set; }

        private readonly SignalBus _signalBus;
        private readonly UIManager _uiManager;

        private int _enemyCount;

        public GameManager(SignalBus signalBus, UIManager uiManager)
        {
            _signalBus = signalBus;
            _uiManager = uiManager;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
            _signalBus.Subscribe<EnemyDiedSignal>(OnEnemyDied);
            _signalBus.Subscribe<RestartGameSignal>(RestartGame);

            _enemyCount = UnityEngine.Object.FindObjectsOfType<EnemyController>().Length;

            CurrentState = GameState.Playing;
            Debug.Log($"Game started with {_enemyCount} enemies.");
        }

        private void OnPlayerDied()
        {
            if (CurrentState != GameState.Playing) return;

            CurrentState = GameState.Lose;
            Debug.Log("Game Over!");
            _uiManager.ShowLosePanel();
        }

        private void OnEnemyDied()
        {
            if (CurrentState != GameState.Playing) return;

            _enemyCount--;
            Debug.Log($"An enemy died. Remaining: {_enemyCount}");

            if (_enemyCount <= 0)
            {
                CurrentState = GameState.Win;
                Debug.Log("You Win!");
                _uiManager.ShowWinPanel();
            }
        }

        private void RestartGame()
        {
            Debug.Log("Restarting game via Signal...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<PlayerDiedSignal>(OnPlayerDied);
            _signalBus.Unsubscribe<EnemyDiedSignal>(OnEnemyDied);
            _signalBus.Unsubscribe<RestartGameSignal>(RestartGame);
        }
    }
}