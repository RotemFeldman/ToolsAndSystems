using System.Collections.Generic;
using UnityEngine;

namespace CardsSystem
{
    [System.Serializable]
    public abstract class Card
    {
        protected const string PATH = "CardSystem/CardInfos/";
        
        public CardInfo CardInfo;
        public GameObject GameObjectRef;
        
        

        public virtual bool TryUse()
        {
            Use();
            return true;
        }

        protected virtual void Use()
        {
            CardManager.Instance.Discard(this);
            CardManager.Instance.Draw();
        }

        protected void SetCardInfo(string path)
        {
            CardInfo = Resources.Load<CardInfo>(path);
        }
        
    }
    
}
