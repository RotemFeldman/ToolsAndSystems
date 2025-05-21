using UnityEngine;
using UnityEngine.Serialization;

namespace UnityUtils.ScenesPlus
{
	internal class QuickScenePlaySettings : ScriptableObject
    {
	    [HideInInspector] public string mainScenePath;
    	[HideInInspector] public bool enabled = true;
    }
}
