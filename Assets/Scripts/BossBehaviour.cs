using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

public abstract class BossBehaviour : MonoBehaviour
{
    [SerializeField] protected Queue<BossPhase> BossPhases = new();
    protected Damageable DamageableComp;
    protected BossPhase CurrentPhase;
    
    protected virtual void Awake()
    {
        DamageableComp = GetComponent<Damageable>();
        CurrentPhase = BossPhases.Peek();

        DamageableComp.OnDeath += ChangePhase;
    }

    protected virtual void ChangePhase()
    {
        BossPhases.Dequeue();
        CurrentPhase=BossPhases.Peek();
        DamageableComp.ChangeMaxLife(CurrentPhase.maxLife);
    }
}
