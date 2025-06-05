using UnityEngine;

namespace UnityUtils
{
	public static class ColorExtensions
    {
#if UNITY_EDITOR
    	public static Color EditorBlue => new(0.173f,0.365f,0.529f,1f);
	    public static Color EditorDarkGray => new(0.1647f,0.1647f,0.1647f,1f);
	    public static Color EditorLightGray => new(0.3451f,0.3451f,0.3451f,1f);
	    public static Color EditorMidGray => new(0.2196f,0.2196f,0.2196f,1f);
	    public static Color EditorWhiteText => new(0.8824f,0.8824f,0.8824f,1f);
#endif

	    public static void SetAlpha(this Color color, float alpha)=>color.a = alpha;
	    
    }
}


