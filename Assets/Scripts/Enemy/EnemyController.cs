using UnityEngine;
using DG.Tweening;
using Zenject;
using GameManagement;

public class EnemyController : MonoBehaviour, IDamageable
{
    public float MinVelocityToKill = 10f;

    private bool _isDead = false;
    private SignalBus _signalBus;
    public void TakeHit(Collision2D collision)
    {
        float impactVelocity = collision.relativeVelocity.magnitude;
        if (impactVelocity > MinVelocityToKill)
        {
            Die();
        }
        else
        {

        }
    }
    public void Die()
    {
        if (_isDead) return;
        _isDead = true;
        _signalBus.Fire<EnemyDiedSignal>();
        
        GetComponent<Collider2D>().enabled = false;

        Sequence deathSequence = DOTween.Sequence();
        deathSequence.Append(transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0), 0.3f, 5, 0.5f));
        deathSequence.Join(GetComponent<SpriteRenderer>().DOFade(0, 0.5f).SetEase(Ease.InQuad));
        deathSequence.OnComplete(() =>
        {
            Destroy(gameObject);
        });

    }
}