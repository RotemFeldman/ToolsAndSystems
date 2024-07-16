using System;
using CardsSystem.Effects;
using UnityEngine;

namespace CardsSystem.Base
{
    [CreateAssetMenu(fileName = "card",menuName = "Card System/new Card",order = 0)]
    public class Card : ScriptableObject
    {
        [SerializeReference] private CardEffect effect;

        
        [ContextMenu("Use Card")]
        public void Use()
        {
            effect.PerformEffect();
        }
        
    }

    
}
