using UnityEngine;
using UnityEngine.Serialization;

namespace ScenesPlus
{
	internal class QuickScenePlaySettings : ScriptableObject
    {
	    [HideInInspector] public string mainScenePath;
    	[HideInInspector] public bool enabled = true;
    }
}
