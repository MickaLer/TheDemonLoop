using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Patterns
{
    public class MovingPattern : Pattern
    {
        public List<Step> steps = new();
        public override IEnumerator Do()
        {
            int currentIndex = 0;
            while (true)
            {
                var currentStep = steps[currentIndex];
                yield return new WaitForEndOfFrame();
                currentIndex++;
                break;
            }
        }
        
        [Serializable]
        public struct Step
        {
            public Vector2 direction;
            public float duration;
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