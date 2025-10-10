using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "BossPhase", menuName = "ScriptableObjects/BossPhase")]
    public class BossPhase : ScriptableObject
    {
        public List<Pattern> BossPatterns = new();
        public float maxLife;
        public Texture2D bossSprite;
    }
}