using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CardsSystem
{
    [CreateAssetMenu(fileName = "CardInfo", menuName = "Card System/new CardInfo", order = 0)]
    public class CardInfo : ScriptableObject
    {
        public string Name;
        public float Cost;
        public string Description;
        public Sprite Sprite;
        
    }
}