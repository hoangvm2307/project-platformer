using System;
using UnityEngine;
using Zenject;

public class PlayerCollisionHandler : MonoBehaviour
{ 
    public Action<Collision2D> OnTookHardImpact;
    private PlayerSettings _playerSettings;

    [Inject]
    public void Construct(PlayerSettings playerSettings)
    {
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
                OnTookHardImpact?.Invoke(collision);
            }

            return;

        }

    }
}
