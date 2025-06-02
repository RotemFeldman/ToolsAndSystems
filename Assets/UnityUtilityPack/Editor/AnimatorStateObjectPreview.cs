using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomPreview(typeof(AnimatorState))]
public class AnimatorStateObjectPreview : ObjectPreview
{
    private Editor _preview;
    private int _animationClipId;
    
    public override void Initialize(Object[] targets)
    {
        base.Initialize(targets);

        if (targets.Length > 1 || Application.isPlaying)
            return;

        AnimationClip clip = GetAnimationClip(target as AnimatorState);
        if (clip)
        {
            _preview = Editor.CreateEditor(clip);
            _animationClipId = clip.GetInstanceID();
        }

    }

    public override void Cleanup()
    {
        base.Cleanup();
        
        CleanupPreviewEditor();
    }

    public override bool HasPreviewGUI()
    {
        return _preview?.HasPreviewGUI() ?? false;
    }

    public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
    {
        base.OnInteractivePreviewGUI(r, background);
        
        AnimationClip currentClip = GetAnimationClip(target as AnimatorState);
        if (currentClip && currentClip.GetInstanceID() != _animationClipId)
        {
            CleanupPreviewEditor();
            _preview = Editor.CreateEditor(currentClip);
            _animationClipId = currentClip.GetInstanceID();
            return;
        }
        
        if(_preview)
            _preview.OnInteractivePreviewGUI(r, background);
    }

    private AnimationClip GetAnimationClip(AnimatorState state)
    {
        return state?.motion as AnimationClip;
    }

    private void CleanupPreviewEditor()
    {
        if (_preview)
        {
            UnityEngine.Object.DestroyImmediate(_preview);
            _preview = null;
            _animationClipId = 0;
        }
    }
}
