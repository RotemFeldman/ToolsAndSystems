using UnityEngine;

namespace UnityUtils
{
	public static class ColorExtensions
    {
    	public static Color EditorBlue => new(0.173f,0.365f,0.529f,1f);

	    public static void SetAlpha(this Color color, float alpha)=>color.a = alpha;
	    
    }
}


