using CardsSystem.Base;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private Card card;

    private void Start()
    {
        card.Use();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
