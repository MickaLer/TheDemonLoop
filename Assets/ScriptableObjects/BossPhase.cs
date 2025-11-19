using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "BossPhase", menuName = "ScriptableObjects/BossPhase")]
    public class BossPhase : ScriptableObject
    {
        
        public List<BossPhaseData> bossPatterns = new();
        public float maxLife;
        public GameObject bossObject;
        public float patternCooldown;
    }

    [Serializable]
    public struct BossPhaseData : IEquatable<BossPhaseData>
    {
        public List<Pattern> patterns;

        public bool Equals(BossPhaseData other)
        {
            return Equals(patterns, other.patterns);
        }
        public override bool Equals(object obj)
        {
            return obj is BossPhaseData other && Equals(other);
        }
        public override int GetHashCode()
        {
            return (patterns != null ? patterns.GetHashCode() : 0);
        }
    }
}