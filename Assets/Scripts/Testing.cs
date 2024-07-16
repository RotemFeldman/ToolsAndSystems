using CardsSystem.Base;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private Card card;

    private void Start()
    {
        card.TryUse();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
