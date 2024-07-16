using System;
using CardsSystem.Effects;
using UnityEngine;

namespace CardsSystem.Base
{
    [CreateAssetMenu(fileName = "card",menuName = "Card System/new Card",order = 0)]
    public class Card : ScriptableObject
    {
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private int cost;

        [Header("Effect")]
        [SerializeField] private int value;
        [SerializeReference] private CardEffect effect;
        
        [SerializeField] private TargetType targetType;

        private enum TargetType
        {
            SingleUnit,
            RandomSingleUnit,
            AreaOfEffect,
        }


        
        // TODO card use conditions
        public bool TryUse()
        {
            if (true)
            {
                Use();
                return true;
            }

            return false;
        }
        
        private void Use()
        {
            effect.PerformEffect(this);
        }
        
    }

    
}
