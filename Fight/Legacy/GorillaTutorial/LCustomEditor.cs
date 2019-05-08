using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/*
[CustomEditor(typeof(sMechanics))]
public class CustomEdit : Editor
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        sMechanics mech = (sMechanics)target;
        mech.GetSliderVal = EditorGUILayout.Slider(mech.GetSliderVal, 0, 1);

        if(mech.GetSliderVal > 0) { AnimationMode.StartAnimationMode(); }
        else { AnimationMode.StopAnimationMode(); }

        Animator anim = mech.gameObject.GetComponent<Animator>();
        if(anim != null && anim.runtimeAnimatorController == null) { return; }
        if(!EditorApplication.isPlaying && AnimationMode.InAnimationMode())
        {
            AnimationMode.BeginSampling();
            AnimationMode.SampleAnimationClip(mech.gameObject, anim.GetCurrentAnimatorClipInfo(0)[0].clip, anim.GetCurrentAnimatorClipInfo(0)[0].clip.averageDuration * mech.GetSliderVal);
            AnimationMode.EndSampling();

            SceneView.RepaintAll();
        }
    }
}
*/