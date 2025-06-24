
using UnityEngine;

namespace GameManagement
{
    public class PlayerDiedSignal { }
    public class EnemyDiedSignal { }
    public class RestartGameSignal { }
    public class EnemySpawnedSignal { }
    public class PlayerLaunchSignal
    {
        public Vector2 Force { get; set; }
    }
    public class GameStateChangedSignal
    {
        public GameState NewState { get; set; }
    }
}