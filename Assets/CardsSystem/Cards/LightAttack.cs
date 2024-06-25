using UnityEngine;

namespace CardsSystem.Cards
{
    public class LightAttack : Card
    {
        private const string ID = PATH + "LightAttack";
        
        public LightAttack()
        {
            SetCardInfo(ID);
        }
        protected override void Use()
        {
            Debug.Log("lightattack");
            base.Use();
        }
    }
}