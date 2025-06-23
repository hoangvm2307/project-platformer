using UnityEngine;
using Zenject;
using UnityEngine.InputSystem;
using System;
public class InputManager : IInitializable, IDisposable
{
    private readonly Camera _mainCamera;
    private readonly PlayerSettings _playerSettings;
    private readonly PlayerController _playerController;

    private Vector2 _dragStartPosition;
    private bool _isDragging = false;
    private PlayerControls _controls;

    public InputManager(Camera mainCamera, PlayerSettings playerSettings, PlayerController playerController)
    {
        _mainCamera = mainCamera;
        _playerSettings = playerSettings;
        _playerController = playerController;
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
        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = true;
            _dragStartPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (_isDragging && Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            Vector2 endPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 launchVector = _dragStartPosition - endPosition;

            _playerController.Launch(launchVector * _playerSettings.LaunchForceMultiplier);
        }
    }

    private void OnDragStart(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = _controls.Gameplay.Position.ReadValue<Vector2>();
        _dragStartPosition = _mainCamera.ScreenToWorldPoint(screenPosition);
    }

    private void OnDragEnd(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = _controls.Gameplay.Position.ReadValue<Vector2>();
        Vector2 endPosition = _mainCamera.ScreenToWorldPoint(screenPosition);
        Vector2 launchVector = _dragStartPosition - endPosition;

        _playerController.Launch(launchVector * _playerSettings.LaunchForceMultiplier);
    }
    
     public void Dispose()
    {
        _controls.Disable();  
    }
}