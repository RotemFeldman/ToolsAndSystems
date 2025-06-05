using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class RadialMenuActions
{
	[RadialMenu("Play")]
	public static void DebugTest()
	{
		if (EditorApplication.isPlaying) return;
		EditorApplication.isPlaying = true; ;
	}

	[RadialMenu("spawn Cube")]
	public static void SpawnCube()
	{
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.position = new Vector3(0, 0, 0);
		
		Undo.RegisterCreatedObjectUndo(cube, "Create " + cube.name);
	}

	[RadialMenu("Scenes/Scene 1")]
	public static void OpenSceneOne()
	{
		if(SceneManager.GetActiveScene().name == "New Scene 1")
			return;

		if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
		{
			EditorSceneManager.OpenScene("Assets/Scenes/New Scene 1.unity");
		}
	}
	
	[RadialMenu("Scenes/Scene 2")]
	public static void OpenSceneTwo()
	{
		if(SceneManager.GetActiveScene().name == "New Scene 2")
			return;

		if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
		{
			EditorSceneManager.OpenScene("Assets/Scenes/New Scene 2.unity");
		}
	}
}
