using CardsSystem.Base;
using UnityEngine;

namespace CardsSystem.Effects.Scripts
{
    [CreateAssetMenu(fileName = "Effect",menuName = "Card System/Effects/new DebugEffect",order = 0)]
    public class DebugEffect : CardEffect
    {
        [SerializeField] private string message;
        public override void PerformEffect(Card card)
        {
            Debug.Log($"{card.name} was used, the effect was {message}");
        }
    }
}