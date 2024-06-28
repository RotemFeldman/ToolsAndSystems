using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExplosiveBarrelManager : MonoBehaviour
{
    public static List<ExplosiveBarrel> AllBarrel = new();

    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.zTest = CompareFunction.LessEqual;
        Vector3 managerPos = transform.position;

        
        foreach (var barrel in AllBarrel)
        {
            Vector3 barrelPos = barrel.transform.position;
            float halfHeight = (managerPos.y - barrelPos.y) * 0.5f;
            Vector3 offset = Vector3.up * halfHeight;
            
            Handles.DrawBezier(managerPos,barrelPos,managerPos-offset,barrelPos+offset,Color.white,EditorGUIUtility.whiteTexture,1f);
            
        }
    }
    #endif
}
