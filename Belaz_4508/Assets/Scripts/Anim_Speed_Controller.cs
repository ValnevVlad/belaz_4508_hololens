using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit.UX;
using UnityEngine;

public class Anim_Speed_Controller : MonoBehaviour
{
    [SerializeField] Animator mainAnimator1;
    [SerializeField] Animator mainAnimator2;
    public Slider xr_slider;

    private void Update()
    {
        float animSpeedControl = xr_slider.Value;
        mainAnimator1.SetFloat("anim_speed", animSpeedControl);
        mainAnimator2.SetFloat("anim_speed", animSpeedControl);
    }

}