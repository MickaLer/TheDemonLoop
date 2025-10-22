using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

namespace Patterns
{
    public class SpawnLaserPattern : Pattern
    {
        public List<Step> steps =  new();
        private Step _currentStep;

        private float _innerCount;
        
        public override IEnumerator Do()
        {
            Step[] tempSteps = new Step[steps.Count];
            steps.CopyTo(tempSteps);
            int currentIndex = 0;
            Transform bossTransform = GameManager.CurrentBoss.gameObject.transform;
            while (true)
            {
                _currentStep = tempSteps[currentIndex];

                List<LineRenderer> tempObjects = new List<LineRenderer>();
                    
                foreach (var currentLaser in _currentStep.spawnedLasers)
                {
                    GameObject temp = Instantiate(currentLaser.spawned, bossTransform.position, Quaternion.identity, bossTransform);
                    tempObjects.Add(temp.GetComponent<LineRenderer>());

                    tempObjects.Last().SetPosition(0, temp.transform.position);
                    ModifyLaser(tempObjects.Last(), currentLaser.angle);
                }

                while (_innerCount < _currentStep.duration)
                {
                    if (_currentStep.direction != 0)
                    {
                        for (var index = 0; index < tempObjects.Count; index++)
                        {
                            var tempLaser = tempObjects[index];
                            var spawnLaser = _currentStep.spawnedLasers[index];
                            ModifyLaser(tempLaser, spawnLaser.angle + _innerCount * _currentStep.speed * _currentStep.direction);
                        }
                    }
                    
                    _innerCount += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }


                for (; tempObjects.Count != 0; )
                {
                    var tempObject = tempObjects[0];
                    Destroy(tempObject.gameObject);
                    tempObjects.RemoveAt(0);
                }
                tempObjects.Clear();
                yield return new WaitForEndOfFrame();
                _innerCount = 0;
                currentIndex++;
                if (currentIndex >= steps.Count) break;
            }
        }

        private void ModifyLaser(LineRenderer laser, float angle)
        {
            float deg2Rad = Mathf.Deg2Rad * angle;
            Vector2 direction = new Vector2(Mathf.Sin(deg2Rad), Mathf.Cos(deg2Rad));
            var hit2D = Physics2D.Raycast(laser.transform.position, direction);
            
            if (hit2D.collider)
            {
                laser.SetPosition(1, hit2D.point);
            }
        }

        [Serializable]
        public struct Step
        {
            public float direction;
            public float duration;
            public float speed;
            public List<Laser> spawnedLasers;
            public int order;
        }
        [Serializable]
        public struct Laser
        {
            public float angle;
            public float radius;
            public GameObject spawned;
        }
    }
}