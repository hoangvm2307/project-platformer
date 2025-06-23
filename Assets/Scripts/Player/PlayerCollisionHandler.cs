using System;
using UnityEngine;
using Zenject;

public class PlayerCollisionHandler : MonoBehaviour
{
    public Action OnLanded;
    private PlayerController _playerController;
    private PlayerSettings _playerSettings;

    [Inject]
    public void Construct(PlayerController playerController, PlayerSettings playerSettings)
    {
        _playerController = playerController;
        _playerSettings = playerSettings;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageableObject = collision.gameObject.GetComponent<IDamageable>();

        if (damageableObject != null)
        {
            damageableObject.TakeHit(collision);

            if (collision.relativeVelocity.magnitude < _playerSettings.MinImpactVelocityToSurvive)
            {
                _playerController.Die();
            }

            return;

        }
        if (collision.GetContact(0).normal.y > 0.5f)
        {
            OnLanded?.Invoke();
        }
    }
}
