using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Patterns
{
    public class MovingPattern : Pattern
    {
        public List<Step> steps = new();
        public ReturnBehaviour returnBehaviour;
        public float returnDuration;
        private Collision2D _lastHitObject;
        private bool _collisionDone;
        public override IEnumerator Do()
        {
            BossBehaviour currentBoss = GameManager.CurrentBoss;
            currentBoss.OnBorderHit += CollisionDetected;
            InnerCount = 0;
            int currentIndex = 0;
            Vector3 originalPos = currentBoss.transform.position;
            while (true)
            {
                var currentStep = steps[currentIndex];
                if (currentStep.direction == Vector2.zero) currentStep.direction = new Vector2(Random.value, Random.value).normalized;
                
                while (InnerCount < currentStep.duration)
                {
                    if (currentStep.direction != Vector2.zero)
                        currentBoss.transform.Translate(currentStep.direction * (currentStep.speed * Time.deltaTime), Space.World);

                    if (_collisionDone)
                    {
                        switch (currentStep.collisionBehaviour)
                        {
                            case CollisionBehaviour.Bounce:
                                var normal = _lastHitObject.contacts[0].normal;
                                currentStep.direction = Vector2.Reflect(currentStep.direction, normal);
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
                currentBoss.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
                if(returnBehaviour == ReturnBehaviour.AfterStep) yield return ReturnToOriginal(originalPos);
                
                InnerCount = 0;
                _collisionDone = false;
                currentIndex++;
                if (currentIndex >= steps.Count) break;
            }
            currentBoss.OnBorderHit -= CollisionDetected;
            if(returnBehaviour == ReturnBehaviour.AfterEnd) yield return ReturnToOriginal(originalPos);
        }

        private void CollisionDetected(Collision2D hitObject)
        {
            _collisionDone = true;
            _lastHitObject = hitObject;
        }

        private IEnumerator ReturnToOriginal(Vector3 originalPos)
        {
            Transform bossTransform = GameManager.CurrentBoss.gameObject.transform;
            Vector3 currentPos = bossTransform.position;
            float tempTimer = 0;
            while (originalPos != bossTransform.position)
            {
                tempTimer += Time.deltaTime;
                bossTransform.position = Vector3.Lerp(currentPos, originalPos, tempTimer / returnDuration);
                yield return new WaitForEndOfFrame();
            }
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
        
        [Serializable]
        public enum ReturnBehaviour
        {
            AfterStep,
            AfterEnd,
            Never
        }
    }
}