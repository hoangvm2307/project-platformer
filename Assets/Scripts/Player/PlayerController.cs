using UnityEngine;
using Zenject;
using DG.Tweening;
using GameManagement;
using System;

public class PlayerController : IInitializable, ILateTickable, IDisposable
{
    private readonly Rigidbody2D _rigidbody;
    private readonly Transform _transform;
    private readonly PlayerCollisionHandler _collisionHandler;
    private readonly PlayerSettings _settings;
    private readonly SignalBus _signalBus;
    private readonly Camera _camera;
    private readonly SpriteRenderer _spriteRenderer;

    private Sequence _runningSequence;
    private bool _isDead = false;
    private bool _isGrounded = false;
    private GameObject _ghost;
    private float _screenWidth;

    public Transform Transform => _transform;

    public PlayerController(
        Rigidbody2D rigidbody,
        Transform transform,
        PlayerCollisionHandler collisionHandler,
        PlayerSettings settings,
        SignalBus signalBus,
        Camera camera,
        SpriteRenderer spriteRenderer)
    {
        _rigidbody = rigidbody;
        _transform = transform;
        _collisionHandler = collisionHandler;
        _settings = settings;
        _signalBus = signalBus;
        _camera = camera;
        _spriteRenderer = spriteRenderer;
    }

    public void Initialize()
    {
        _collisionHandler.OnTookHardImpact += HandleHardImpact;
        _signalBus.Subscribe<PlayerLaunchSignal>(OnPlayerLaunch);
        float screenHeight = _camera.orthographicSize * 2;
        _screenWidth = screenHeight * _camera.aspect;

        _ghost = new GameObject("PlayerGhost");
        var ghostRenderer = _ghost.AddComponent<SpriteRenderer>();
        ghostRenderer.sprite = _spriteRenderer.sprite;
        ghostRenderer.sortingLayerID = _spriteRenderer.sortingLayerID;
        ghostRenderer.sortingOrder = _spriteRenderer.sortingOrder - 1;
        _ghost.SetActive(false);

        _spriteRenderer.color = _settings.PlayerColor;
        ghostRenderer.color = _settings.PlayerColor;
    }
    private void OnPlayerLaunch(PlayerLaunchSignal signal)
    {
        Launch(signal.Force);
    }

    public void Launch(Vector2 force)
    {
        _runningSequence?.Kill();

        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.AddForce(force, ForceMode2D.Impulse);

        float angle = Mathf.Atan2(force.y, force.x) * Mathf.Rad2Deg;
        _transform.rotation = Quaternion.Euler(0, 0, angle);

        _runningSequence = DOTween.Sequence();
        _runningSequence.Append(_transform.DOScaleX(_settings.StretchScale, _settings.StretchDuration))
                        .Append(_transform.DOScaleX(1f, _settings.StretchBounceDuration).SetEase(Ease.OutBounce));
    }
    private void HandleHardImpact(Collision2D collision)
    {
        Die();
    }
    private void HandleLanding()
    {
        Debug.Log("Player landed!");
        // _runningSequence?.Kill();
        // _runningSequence = DOTween.Sequence();
        // _runningSequence.Append(_transform.DOScale(new Vector2(1.25f, 0.75f), 0.1f))
        //                .Append(_transform.DOScale(Vector2.one, 0.3f).SetEase(Ease.OutElastic));
    }

    public void LateTick()
    {
        CheckIfGrounded();
        HandleScreenWrap();
    }
    private void CheckIfGrounded()
    {
        var hit = Physics2D.Raycast(_transform.position, Vector2.down, _settings.GroundCheckDistance, LayerMask.GetMask("Ground"));

        Color rayColor = hit.collider != null ? Color.green : Color.red;
        Debug.DrawRay(_transform.position, Vector2.down * _settings.GroundCheckDistance, rayColor);

        if (hit.collider != null)
        {
            if (!_isGrounded)
            {
                _isGrounded = true;
                HandleLanding();
            }
        }
        else
        {
            _isGrounded = false;
        }
    }

    private void HandleScreenWrap()
    {
        var cameraPosition = _camera.transform.position;
        var leftBound = cameraPosition.x - _screenWidth / 2;
        var rightBound = cameraPosition.x + _screenWidth / 2;
        var playerWidth = _spriteRenderer.bounds.size.x / 2;

        bool isCrossingLeft = _transform.position.x - playerWidth < leftBound;
        bool isCrossingRight = _transform.position.x + playerWidth > rightBound;

        if (isCrossingLeft)
        {
            _ghost.SetActive(true);
            _ghost.transform.position = new Vector3(_transform.position.x + _screenWidth, _transform.position.y, _transform.position.z);
        }
        else if (isCrossingRight)
        {
            _ghost.SetActive(true);
            _ghost.transform.position = new Vector3(_transform.position.x - _screenWidth, _transform.position.y, _transform.position.z);
        }
        else
        {
            _ghost.SetActive(false);
        }

        if (_ghost.activeSelf)
        {
            _ghost.transform.rotation = _transform.rotation;
            _ghost.transform.localScale = _transform.localScale;
        }

        if (_transform.position.x + playerWidth < leftBound)
        {
            _transform.position = new Vector3(_transform.position.x + _screenWidth, _transform.position.y, _transform.position.z);
        }
        else if (_transform.position.x - playerWidth > rightBound)
        {
            _transform.position = new Vector3(_transform.position.x - _screenWidth, _transform.position.y, _transform.position.z);
        }
    }

    public void Die()
    {
        if (_isDead) return;
        _isDead = true;

        Debug.Log("Player Died!");

        // Tạm thời, chúng ta chỉ vô hiệu hóa GameObject của người chơi
        // TODO trong tương lai: Thêm hiệu ứng chết, hiện UI "Game Over", nút chơi lại...
        _signalBus.Fire<PlayerDiedSignal>();
        _rigidbody.gameObject.SetActive(false);
    }
    public void Dispose()
    {
        _collisionHandler.OnTookHardImpact -= HandleHardImpact;
        _signalBus.TryUnsubscribe<PlayerLaunchSignal>(OnPlayerLaunch); 
        GameObject.Destroy(_ghost);
    }
}
