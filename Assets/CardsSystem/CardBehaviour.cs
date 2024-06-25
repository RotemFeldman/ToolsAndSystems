using System;
using CardsSystem.Cards;
using UnityEngine;

namespace CardsSystem
{
    public class CardBehaviour : MonoBehaviour
    {
        public Card Card;
        [SerializeField] private CardVisual _cardVisual;
        

        public void InitCard(Card card)
        {
            Card = card;
            
            if (Card == null)
            {
                Debug.Log("NullCard was destroyed");
                Destroy(gameObject);
            }
            else
            {
                Card.GameObjectRef = gameObject;
                _cardVisual.Set(Card.CardInfo);
            }
        }

        public bool TryUse()
        {
           return Card.TryUse();
        }
        
        private void Awake()
        {
            
        }
        
        private void Start()
        {
    
            
        }
        
        
    }
}