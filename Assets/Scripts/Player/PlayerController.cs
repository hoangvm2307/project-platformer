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

    private Sequence _runningSequence;

    private readonly SignalBus _signalBus;
    private bool _isDead = false;
    private bool _isGrounded = false;

    public PlayerController(
        Rigidbody2D rigidbody,
        Transform transform,
        PlayerCollisionHandler collisionHandler,
        PlayerSettings settings,
        SignalBus signalBus)
    {
        _rigidbody = rigidbody;
        _transform = transform;
        _collisionHandler = collisionHandler;
        _settings = settings;
        _signalBus = signalBus;
    }

    public void Initialize()
    {
        _collisionHandler.OnTookHardImpact += HandleHardImpact;
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
    }
    private void CheckIfGrounded()
    {
        var hit = Physics2D.Raycast(_transform.position, Vector2.down, _settings.GroundCheckDistance, LayerMask.GetMask("Ground"));

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
    }
}
