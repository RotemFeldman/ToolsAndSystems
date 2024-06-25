using UnityEngine;

namespace CardsSystem.Cards
{
    public class HeavyAttack : Card
    {
        private const string ID = PATH + "HeavyAttack";
        
        public HeavyAttack()
        {
            SetCardInfo(ID);
        }
        
        protected override void Use()
        {
            Debug.Log("heavy attack");
            base.Use();
        }
    }
}