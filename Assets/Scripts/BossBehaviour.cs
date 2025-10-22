using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class BossBehaviour : MonoBehaviour
{
    [SerializeField] protected List<BossPhase> bossPhases = new();
    protected Damageable DamageableComp;
    protected BossPhase CurrentPhase;
    protected SpriteRenderer OwnSprite;

    private float _innerTimer;
    private bool _launchingAttack;
    private Coroutine _attackCoroutine;
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
            yield return currentPattern.Do();
            yield return new WaitForSeconds(currentPattern.followingPatternDelay);
        }
        _attackCoroutine = null;
        _launchingAttack = false;
    }

    protected virtual void Awake()
    {
        DamageableComp = GetComponent<Damageable>();
        OwnSprite = GetComponent<SpriteRenderer>();
        CurrentPhase = bossPhases[0];
        OwnSprite.sprite = Sprite.Create(CurrentPhase.bossSprite, new Rect(0, 0, CurrentPhase.bossSprite.width, CurrentPhase.bossSprite.height), new Vector2(0.5f, 0.5f), 100);
        GameManager.StartFight(this);
        DamageableComp.OnDeath += ChangePhase;
    }

    protected virtual void ChangePhase()
    {
        if(_launchingAttack)StopCoroutine(_attackCoroutine);
        bossPhases.RemoveAt(0);
        CurrentPhase = bossPhases[0];
        DamageableComp.ChangeMaxLife(CurrentPhase.maxLife);
        OwnSprite.sprite = Sprite.Create(CurrentPhase.bossSprite, new Rect(0, 0, CurrentPhase.bossSprite.width, CurrentPhase.bossSprite.height), new Vector2(0.5f, 0.5f), 100);
    }
}
