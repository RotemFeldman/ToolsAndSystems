using System;
using UnityEngine;

namespace CardsSystem
{
    public class HandHandler : MonoBehaviour
    {
        public static HandHandler Instance;
        
        [SerializeField] private GameObject _slot1;
        [SerializeField] private GameObject _slot2;
        [SerializeField] private GameObject _slot3;

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

        public void AddToHand(GameObject card)
        {
            if (_slot2.transform.childCount == 0)
            {
                card.transform.SetParent(_slot2.transform);
            }
            else if (_slot1.transform.childCount == 0)
            {
                card.transform.SetParent(_slot1.transform);
            }
            else if (_slot3.transform.childCount == 0)
            {
                card.transform.SetParent(_slot3.transform);
            }
            else
            {
                Destroy(card);
                Debug.Log("card destroy due to limited hand size");
            }
        }
    }
}