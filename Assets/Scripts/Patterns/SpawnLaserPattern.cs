using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DamageDealers;
using Managers;
using Unity.VisualScripting;
using UnityEngine;

namespace Patterns
{
    public class SpawnLaserPattern : Pattern
    {
        public List<Step> steps = new();

        public override IEnumerator Do()
        {
            int indexStep = 0;
            Transform bossTransform = GameManager.CurrentBoss.gameObject.transform;
            InnerCount = 0;
            while (true)
            {
                Step currentStep = steps[indexStep];
                
                List<LineRenderer> tempObjects = new List<LineRenderer>();

                foreach (var currentLaser in currentStep.spawnedLasers)
                {
                    GameObject temp = Instantiate(currentLaser.spawned, bossTransform.position, Quaternion.identity, bossTransform);
                    tempObjects.Add(temp.GetComponent<LineRenderer>());
                    temp.GetComponent<LaserDamageDealer>().owner = bossTransform.gameObject;
                    tempObjects.Last().SetPosition(0, temp.transform.position);
                    ModifyLaser(tempObjects.Last(), currentLaser.angle, bossTransform);
                }

                while (InnerCount < currentStep.duration)
                {
                    for (var index = 0; index < tempObjects.Count; index++)
                    {
                        var tempLaser = tempObjects[index];
                        var spawnLaser = currentStep.spawnedLasers[index];
                        if (spawnLaser.direction == 0) continue;
                        
                        float newAngle = spawnLaser.angle + InnerCount * currentStep.speed * spawnLaser.direction;
                        if ((int)Mathf.Round(newAngle) ==  (int)Mathf.Round(spawnLaser.maxAngle) )
                        {
                            newAngle = spawnLaser.maxAngle;
                            spawnLaser.direction = 0;
                            
                            //TODO : fix this line that change directly the ScriptableObject
                            currentStep.spawnedLasers[index] = spawnLaser;
                        }
                        
                        ModifyLaser(tempLaser, newAngle, bossTransform);
                    }

                    InnerCount += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }


                while (tempObjects.Count != 0)
                {
                    var tempObject = tempObjects[0];
                    Destroy(tempObject.gameObject);
                    tempObjects.RemoveAt(0);
                }

                tempObjects.Clear();
                yield return new WaitForEndOfFrame();
                InnerCount = 0;
                indexStep++;
                if (indexStep >= steps.Count) break;
            }
        }

        private void ModifyLaser(LineRenderer laser, float angle, Transform boss)
        {
            float deg2Rad = Mathf.Deg2Rad * angle;
            Vector2 direction = new Vector2(Mathf.Sin(deg2Rad), Mathf.Cos(deg2Rad));
            LayerMask mask = ~0;
            mask -= LayerMask.GetMask("Boss");
            mask -= LayerMask.GetMask("Player");
            mask -= LayerMask.GetMask("Ignore Raycast");
            var hit2D = Physics2D.Raycast(laser.transform.position, direction, 100.0f, mask);

            laser.SetPosition(0, boss.position);
            if (hit2D.collider) laser.SetPosition(1, hit2D.point);
        }

        [Serializable]
        public struct Step
        {
            public float duration;
            public float speed;
            public List<Laser> spawnedLasers;
            public int order;
        }

        [Serializable]
        public struct Laser
        {
            public float direction;
            public float angle;
            public float maxAngle;
            public GameObject spawned;
        }
    }
}