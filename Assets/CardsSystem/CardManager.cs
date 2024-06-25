using System;
using System.Collections.Generic;
using CardsSystem.Cards;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CardsSystem
{
    public class CardManager : MonoBehaviour
    {
        public static CardManager Instance;

        [SerializeField] private Transform _handTransform;
        [SerializeField] private GameObject _cardPrefab;
        
        [Header("Cards")]
        [SerializeField] private List<Card> _deck = new();
        [SerializeField] private List<Card> _discard = new();
        [SerializeField] private List<Card> _hand = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitDeck();
            
            Draw();
            Draw();
            Draw();
        }

        public void Draw()
        {
            if (_deck.Count == 0)
            {
                Shuffle();
            }

            int rnd = Random.Range(0, _deck.Count);

            var newCard = Instantiate(_cardPrefab);
            HandHandler.Instance.AddToHand(newCard);
            var cb = newCard.GetComponent<CardBehaviour>();
            
            cb.InitCard(_deck[rnd]);
            _hand.Add(_deck[rnd]);
            _deck.RemoveAt(rnd);
        }

        public void Discard(Card c)
        {
            if (!_hand.Contains(c))
                return;
                
            _discard.Add(c);

            Destroy(c.GameObjectRef);
            _hand.Remove(c);
        }

        private void Shuffle()
        {
            foreach (var c in _discard)
            {
                _deck.Add(c);
            }
            
            _discard.Clear();
            
        }

        private void InitDeck()
        {
            for (int i = 0; i < 5; i++)
            {
                _deck.Add(new HeavyAttack());
                _deck.Add(new LightAttack());
                _deck.Add(new MyCard());
            }
        }
    }
}