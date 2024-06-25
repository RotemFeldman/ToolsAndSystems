using UnityEngine;

namespace CardsSystem.Cards
{
    public class MyCard : Card
    {
        private const string ID = PATH + "MyCard";
        public MyCard()
        {
            SetCardInfo(ID);
        }
        protected override void Use()
        {
            Debug.Log($"{CardInfo.Name}, {CardInfo.Cost}, {CardInfo.Description}");
            base.Use();
        }
    }
}