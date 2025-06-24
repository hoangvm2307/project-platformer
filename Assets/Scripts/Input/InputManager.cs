using UnityEngine;
using Zenject;
using UnityEngine.InputSystem;
using System;
public class InputManager : IInitializable, ITickable, IDisposable
{
    private readonly Camera _mainCamera;
    private readonly PlayerSettings _playerSettings;
    private readonly PlayerController _playerController;

    private Vector2 _dragStartPosition; 
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
 
        if (launchVector.sqrMagnitude > 0.01f)
        {
            _playerController.Launch(launchVector * _playerSettings.LaunchForceMultiplier);
        }
    }
    
     public void Dispose()
    {
        _controls.Disable();  
    }
}