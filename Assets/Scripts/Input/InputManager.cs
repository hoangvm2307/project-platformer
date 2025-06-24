using UnityEngine;
using Zenject;
using UnityEngine.InputSystem;
using System;
using GameManagement;
public class InputManager : IInitializable, ITickable, IDisposable
{
    private readonly GameObject _arrowPrefab;
    private readonly SignalBus _signalBus;
    private readonly Camera _mainCamera;
    private readonly PlayerSettings _playerSettings;
    private readonly PlayerController _playerController;

    private Vector2 _dragStartPosition;
    private bool _isDragging = false;
    private PlayerControls _controls;

    private GameObject _currentArrow;
    private ArrowController _arrowController;


    public InputManager(
        Camera mainCamera,
        PlayerSettings playerSettings,
        PlayerController playerController,
        [Inject(Id = "ArrowPrefab")] GameObject arrowPrefab,
        SignalBus signalBus)
    {
        _mainCamera = mainCamera;
        _playerSettings = playerSettings;
        _playerController = playerController;
        _arrowPrefab = arrowPrefab;
        _signalBus = signalBus;
    }
    public void Initialize()
    {
        _controls = new PlayerControls();
        _controls.Enable();

        _controls.Gameplay.Drag.performed += OnDragStart;
        _controls.Gameplay.Drag.canceled += OnDragEnd;
    }
    public void Tick()
    {
        if (_isDragging)
        {
            Vector2 currentMouseWorldPos = _mainCamera.ScreenToWorldPoint(_controls.Gameplay.Position.ReadValue<Vector2>());
            Vector2 launchVector = _dragStartPosition - currentMouseWorldPos;

            // Only show arrow if drag is significant
            if (launchVector.sqrMagnitude > 0.1f)
            {
                _currentArrow.SetActive(true);
                
                float distance = launchVector.magnitude;
                _arrowController.UpdateArrow(distance, _playerSettings.ArrowMaxLength, _playerSettings.ArrowTailOpacity);

                float angle = Mathf.Atan2(launchVector.y, launchVector.x) * Mathf.Rad2Deg;
                // Subtract 90 degrees to account for the arrow sprite pointing upwards
                _currentArrow.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
                _currentArrow.transform.position = _playerController.Transform.position;
            }
            else
            {
                _currentArrow.SetActive(false);
            }
        }
    }

    private void OnDragStart(InputAction.CallbackContext context)
    {
        _isDragging = true;
        Vector2 screenPosition = _controls.Gameplay.Position.ReadValue<Vector2>();
        _dragStartPosition = _mainCamera.ScreenToWorldPoint(screenPosition);

        _currentArrow = GameObject.Instantiate(_arrowPrefab, _playerController.Transform.position, Quaternion.identity);
        _arrowController = _currentArrow.GetComponent<ArrowController>();
        _currentArrow.SetActive(false);
    }

    private void OnDragEnd(InputAction.CallbackContext context)
    {
        _isDragging = false;
        if (_currentArrow != null)
        {
            GameObject.Destroy(_currentArrow);
        }

        Vector2 screenPosition = _controls.Gameplay.Position.ReadValue<Vector2>();
        Vector2 endPosition = _mainCamera.ScreenToWorldPoint(screenPosition);
        Vector2 launchVector = _dragStartPosition - endPosition;

        if (launchVector.sqrMagnitude > 0.1f)
        {
            _signalBus.Fire(new PlayerLaunchSignal { Force = launchVector * _playerSettings.LaunchForceMultiplier });
        }
    }

    public void Dispose()
    {
        if (_currentArrow != null)
        {
            GameObject.Destroy(_currentArrow);
        }
        _controls.Disable();
    }
}