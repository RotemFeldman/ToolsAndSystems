using UnityEngine;

namespace UnityUtils
{
	public static class Texture2DExtensions
    {
	    public static Texture2D MakeTex(int width, int height, Color col)
	    {
		    Color[] pix = new Color[width * height];

		    for (int i = 0; i < pix.Length; i++)
			    pix[i] = col;

		    Texture2D result = new Texture2D(width, height);
		    result.SetPixels(pix);
		    result.Apply();
		    
		    return result;
	    }
	    
	    public static Texture2D Fill(this Texture2D texture, Color col)
	    {
		    if (texture == null) return null;

		    Color[] pix = new Color[texture.width * texture.height];

		    for (int i = 0; i < pix.Length; i++)
			    pix[i] = col;

		    texture.SetPixels(pix);
		    texture.Apply();
		    
		    return texture;
	    }

	    
    }
}


