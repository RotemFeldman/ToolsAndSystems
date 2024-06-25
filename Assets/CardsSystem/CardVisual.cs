using System;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;

namespace CardsSystem
{
    public class CardVisual : MonoBehaviour
    {
        private CardInfo _card;

        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _cost;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Image _image;

        private void Start()
        {
         
        }

        public void Set(CardInfo cardInfo)
        {
            _card = cardInfo;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            _name.text = _card.Name;
            _cost.text = _card.Cost.ToString();
            _description.text = _card.Description;
            _image.sprite = _card.Sprite;
        }
    }
}
