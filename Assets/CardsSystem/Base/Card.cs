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
        [SerializeReference] private CardEffect effect;


        public void TryUse()
        {
            //TODO cost and conditions
            if(true)
                Use();
        }
        public void Use()
        {
            effect.PerformEffect();
        }
        
    }

    
}
