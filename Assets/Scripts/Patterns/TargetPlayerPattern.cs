using System;
using System.Collections;
using System.Collections.Generic;
using DamageDealers;
using Managers;
using UnityEngine;

namespace Patterns
{
    public class TargetPlayerPattern : Pattern
    {
        public List<TargetInfos> steps = new();
        private bool _collisionDone;

        public override IEnumerator Do()
        {
            GameObject boss = GameManager.CurrentBoss.gameObject;
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            foreach (var currentStep in steps)
            {
                switch (currentStep.targetType)
                {
                    case TargetType.Ball:
                        for (int i = 0; i < currentStep.ballNumbers; i++)
                        {
                            GameObject newBall = Instantiate(currentStep.ballPrefab, boss.transform.position, Quaternion.identity);
                            newBall.transform.LookAt(player.transform);
                            yield return new WaitForSeconds(currentStep.timeBetweenBalls);
                        }
                        break;
                    case TargetType.Laser:
                        GameObject newLaser = Instantiate(currentStep.laserPrefab, boss.transform.position, Quaternion.identity);
                        newLaser.GetComponent<LaserDamageDealer>().owner = boss;
                        
                        LineRenderer newLaserLine = newLaser.GetComponent<LineRenderer>();
                        ModifyLaser(newLaserLine, player.transform, boss.transform);
                        
                        float ownTimer = 0;
                        while (currentStep.duration > ownTimer)
                        {
                            if (currentStep.followAfterSpawn) ModifyLaser(newLaserLine, player.transform, boss.transform);
                            ownTimer += Time.deltaTime;
                            yield return new WaitForEndOfFrame();
                        }
                        Destroy(newLaser);
                        break;
                    case TargetType.Movement:
                        yield return new WaitForSeconds(currentStep.chargingTime);
                        boss.GetComponent<BossBehaviour>().OnBorderHit += OnCollisionType;
                        boss.GetComponent<Rigidbody2D>().linearVelocity = (player.transform.position - boss.transform.position) * currentStep.speed;
                        while (!_collisionDone) yield return new WaitForEndOfFrame();
                        if(currentStep.spawnedObject) Instantiate(currentStep.spawnedObject, boss.transform.position, Quaternion.identity);
                        _collisionDone = false;
                        boss.GetComponent<BossBehaviour>().OnBorderHit -= OnCollisionType;
                        break;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        public void ModifyLaser(LineRenderer laser, Transform player, Transform boss)
        {
            laser.SetPosition(0, boss.transform.position);
            Vector2 direction = (player.transform.position - boss.transform.position).normalized;
            LayerMask mask = ~0;
            mask -= LayerMask.GetMask("Boss");
            mask -= LayerMask.GetMask("Player");
            mask -= LayerMask.GetMask("Ignore Raycast");
            var hit2D = Physics2D.Raycast(boss.transform.position, direction, 100.0f, mask); 
            if(hit2D.collider) laser.SetPosition(1, hit2D.point);
        }

        public void OnCollisionType(Collision2D type)
        {
            _collisionDone = true;
        }

        [Serializable]
        public class TargetInfos
        {
            public TargetType targetType;
            public int order; 
            
            //BallType
            public int ballNumbers;
            public float timeBetweenBalls;
            public GameObject ballPrefab;

            //LaserType
            public float duration;
            public bool followAfterSpawn;
            public GameObject laserPrefab;

            //MovementType
            public float speed;
            public float chargingTime;
            public GameObject spawnedObject;
        }

        public enum TargetType
        {
            Ball,
            Laser,
            Movement
        }
    }
}