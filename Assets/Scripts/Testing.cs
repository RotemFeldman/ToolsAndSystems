using CardsSystem.Base;
using UnityEngine;
using Utility.Attributes;
using UtilityClass;

public class Testing : MonoBehaviour
{

    private void Start()
    {
        Create.WorldText("Hello World", Vector3.zero);
        Create.UIText("test", Vector3.zero,color: Color.red);
        Create.UIText("test2", EightDirection.TopLeft);
        
        
    }

    
}
