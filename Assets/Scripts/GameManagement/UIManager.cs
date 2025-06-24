using GameManagement;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;
    [SerializeField] private Button _restartButton;

    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
        _signalBus.Subscribe<GameStateChangedSignal>(OnGameStateChanged);
    }

    private void Start()
    {
        _restartButton.onClick.AddListener(() => _signalBus.Fire<RestartGameSignal>());
    }
    private void OnGameStateChanged(GameStateChangedSignal signal)
    {
        if (signal.NewState == GameState.Win)
        {
            ShowWinPanel();
        }
        else if (signal.NewState == GameState.Lose)
        {
            ShowLosePanel();
        }
    }
    public void ShowWinPanel()
    {
        _winPanel.SetActive(true);
        _restartButton.gameObject.SetActive(true);
    }

    public void ShowLosePanel()
    {
        _losePanel.SetActive(true);
        _restartButton.gameObject.SetActive(true);
    }
}