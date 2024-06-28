using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[ExecuteInEditMode]
public class ExplosiveBarrel : MonoBehaviour
{
    [Range(1f,8f)]
    public float Radius = 1f;
    
    public float Damage = 1f;
    public Color Color = Color.red;

    
    // in editor properties
    static readonly int shPropColor = Shader.PropertyToID("_Color");
    
    private MaterialPropertyBlock _mpb;
    public MaterialPropertyBlock Mpb
    {
        get
        {
            if (_mpb == null)
                _mpb = new MaterialPropertyBlock();
            return _mpb;
        }
    }
    
    
    private void OnEnable()
    {
        if (_mpb == null)
            _mpb = new MaterialPropertyBlock();
        ApplyColor();
        ExplosiveBarrelManager.AllBarrel.Add(this);
    }

    private void OnDisable() => ExplosiveBarrelManager.AllBarrel.Remove(this);

    private void OnValidate() => ApplyColor();
    
    void ApplyColor()
    {
        MeshRenderer rnd = GetComponent<MeshRenderer>();
        Mpb.SetColor(shPropColor,Color);
        rnd.SetPropertyBlock(Mpb);
        //rnd.sharedMaterial.color = Color;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color;
        Gizmos.DrawWireSphere(transform.position,Radius);
    }
    
    #endif
}
