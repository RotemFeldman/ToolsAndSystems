using UnityEngine;


namespace CardsSystem.Base
{
    [System.Serializable]
    public class CardEffect : ScriptableObject
    {
        public virtual void PerformEffect(Card cardUsed){}
    }
}