using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class BossBehaviour : MonoBehaviour
{
    public Action<Collision2D> OnBorderHit;
    [SerializeField] protected List<BossPhase> bossPhases = new();
    protected Damageable DamageableComp;
    protected BossPhase CurrentPhase;
    protected GameObject OwnObject;

    private float _innerTimer;
    private bool _launchingAttack;
    private Coroutine _attackCoroutine;
    
    //Override it to change behavior
    protected virtual void Update()
    {
        if(!_launchingAttack) _innerTimer += Time.deltaTime;
        if(_innerTimer >= CurrentPhase.patternCooldown)
        {
            var incomingPatterns = CurrentPhase.bossPatterns[Random.Range(0, CurrentPhase.bossPatterns.Count)].patterns;
            _attackCoroutine = StartCoroutine(DoPattern(incomingPatterns));
            _innerTimer = 0;
            _launchingAttack = true;
        }
    }

    private IEnumerator DoPattern(List<Pattern> pattern)
    {
        foreach (var currentPattern in pattern)
        {
            // If the followingPatternDelay is lower than 0, that means that the next pattern will be played along the current one
            if (currentPattern.followingPatternDelay < 0)
            {
                StartCoroutine(currentPattern.Do());
                continue;
            }
            yield return currentPattern.Do();
            yield return new WaitForSeconds(currentPattern.followingPatternDelay);
        }
        _attackCoroutine = null;
        _launchingAttack = false;
    }

    protected virtual void Awake()
    {
        DamageableComp = GetComponent<Damageable>();
        CurrentPhase = bossPhases[0];
        
        //Load the sprite stocked inside the phase
		if(CurrentPhase.bossObject != null)
            OwnObject = Instantiate(CurrentPhase.bossObject, transform);
        GameManager.StartFight(this);
        DamageableComp.OnDeath += ChangePhase;
    }

    protected virtual void ChangePhase()
    {
        if(_launchingAttack)StopCoroutine(_attackCoroutine);
        bossPhases.RemoveAt(0);
        if(bossPhases.Count == 0) DeathBehavior();
        else
        {
            CurrentPhase = bossPhases[0];
            DamageableComp.ChangeMaxLife(CurrentPhase.maxLife);
            Destroy(OwnObject);
            OwnObject = Instantiate(CurrentPhase.bossObject, transform);
        }
    }

    protected virtual void DeathBehavior()
    {
        Destroy(gameObject);
        GameManager.EndFight();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Border")) OnBorderHit?.Invoke(other);
    }
}
