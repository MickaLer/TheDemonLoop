using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Patterns
{
    public class SpawnLaserPattern : Pattern
    {
        public Queue<Step> Steps =  new();
        private Step _currentStep;

        private float _innerCount;
        
        public override IEnumerator Do()
        {
            while (true)
            {
                _currentStep = Steps.Dequeue();

                List<LineRenderer> tempObjects = new List<LineRenderer>();
                    
                foreach (var currentLaser in _currentStep.SpawnedLasers)
                {
                    GameObject temp = Object.Instantiate(currentLaser.Spawned);
                    tempObjects.Add(temp.GetComponent<LineRenderer>());

                    tempObjects.Last().SetPosition(0, temp.transform.position);
                    ModifyLaser(tempObjects.Last(), currentLaser.Angle);
                }

                while (_innerCount < _currentStep.Duration)
                {
                    if (_currentStep.Direction != 0)
                    {
                        for (var index = 0; index < tempObjects.Count; index++)
                        {
                            var tempLaser = tempObjects[index];
                            var spawnLaser = _currentStep.SpawnedLasers[index];
                            ModifyLaser(tempLaser, spawnLaser.Angle + _innerCount * _currentStep.Speed * _currentStep.Direction);
                        }
                    }
                    
                    _innerCount += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }


                for (; tempObjects.Count != 0; )
                {
                    var tempObject = tempObjects[0];
                    Object.Destroy(tempObject);
                    tempObjects.RemoveAt(0);
                }
                tempObjects.Clear();
                yield return new WaitForEndOfFrame();
                _innerCount = 0;
                if (Steps.Count <= 0) break;
            }
        }

        private void ModifyLaser(LineRenderer laser, float angle)
        {
            float deg2Rad = Mathf.Deg2Rad * angle;
            Vector2 direction = new Vector2(Mathf.Sin(deg2Rad), Mathf.Cos(deg2Rad));
            var hit2D = Physics2D.Raycast(laser.transform.position, direction);
            
            if (hit2D.collider)
            {
                laser.SetPosition(1, hit2D.transform.position);
            }
        }

        public struct Step
        {
            public float Direction;
            public float Duration;
            public float Speed;
            public List<Laser> SpawnedLasers;
        }

        public struct Laser
        {
            public float Angle;
            public float Radius;
            public GameObject Spawned;
        }
    }
}