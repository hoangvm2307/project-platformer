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
    }

    private void Start()
    { 
        _restartButton.onClick.AddListener(() => _signalBus.Fire<RestartGameSignal>());
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