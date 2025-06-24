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
            _signalBus.Subscribe<EnemySpawnedSignal>(OnEnemySpawned);

            CurrentState = GameState.Playing;
            Debug.Log($"Game started with {_enemyCount} enemies.");
        }

        private void OnPlayerDied()
        {
            if (CurrentState != GameState.Playing) return;
            Debug.Log("Game Over!");
            SetState(GameState.Lose);
        }

        private void OnEnemyDied()
        {
            _enemyCount--;
            if (_enemyCount <= 0)
            {
                Debug.Log("You Win!");
                SetState(GameState.Win);
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
        private void OnEnemySpawned()
        {
            _enemyCount++;
            Debug.Log($"An enemy has spawned. Total: {_enemyCount}");
        }
        private void SetState(GameState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            _signalBus.Fire(new GameStateChangedSignal { NewState = newState });
        }
    }
}