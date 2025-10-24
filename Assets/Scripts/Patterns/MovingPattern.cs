using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Patterns
{
    public class MovingPattern : Pattern
    {
        public List<Step> steps = new();

        private bool _collisionDone;
        public override IEnumerator Do()
        {
            Debug.Log(name);
            BossBehaviour currentBoss = GameManager.CurrentBoss;
            currentBoss.OnBorderHit += CollisionDetected;
            InnerCount = 0;
            int currentIndex = 0;
            while (true)
            {
                var currentStep = steps[currentIndex];
                
                while (InnerCount < currentStep.duration)
                {
                    if (currentStep.direction != Vector2.zero)
                    {
                        currentBoss.transform.Translate(currentStep.direction * (currentStep.speed * Time.deltaTime), Space.World);
                    }

                    if (_collisionDone)
                    {
                        switch (currentStep.collisionBehaviour)
                        {
                            case CollisionBehaviour.Bounce:
                                _collisionDone = false;
                                break;
                            case CollisionBehaviour.NextStep:
                                InnerCount = currentStep.duration;
                                break;
                            case CollisionBehaviour.Stop:
                                currentIndex = steps.Count;
                                InnerCount = currentStep.duration;
                                break;
                        }
                    }
                    InnerCount += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForEndOfFrame();
                _collisionDone = false;
                currentIndex++;
                if (currentIndex >= steps.Count) break;
            }
            currentBoss.OnBorderHit -= CollisionDetected;
        }

        private void CollisionDetected(Collision2D hitObject)
        {
            _collisionDone = true;
        }
        
        [Serializable]
        public struct Step
        {
            public Vector2 direction;
            public float duration;
            public int order;
            public float speed;
            public CollisionBehaviour collisionBehaviour;
        }
        
        [Serializable]
        public enum CollisionBehaviour
        {
            Bounce,
            Stop,
            NextStep
        }
    }
}