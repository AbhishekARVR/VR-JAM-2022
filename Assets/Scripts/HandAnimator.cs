using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ControllerInput;

[RequireComponent(typeof(Animator))]
public class HandAnimator : MonoBehaviour
{
    public float speed = 5f;

    [SerializeField]
    private ControllerInput contIn;

    private Animator animator = null;

    private void Start()
    {
        //animator
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogError("Hand missing animator component.", this);

        //controller input
        contIn = GetComponentInParent<ControllerInput>();

        if (contIn == null)
            Debug.LogError("Hand missing controller input component.", this);
    }

    private void Update()
    {
        //Apply pose
        ApplyPose(contIn.currentPose);
        
        //Apply animations
        AnimateFingers(contIn.triggerFingers);
        AnimateFingers(contIn.gripFingers);
        AnimateFingers(contIn.thumbFingers);
    }

    private void ApplyPose(HandPose pose)
    {
        int curVal = (int)animator.GetFloat("Pose");

        if ((int)pose != curVal)
            animator.SetFloat("Pose", (float)pose);
    }

    private void AnimateFingers(List<Finger> fingers)
    {
        foreach (Finger f in fingers)
        {
            //Smooth input value
            float time = speed * Time.unscaledDeltaTime;
            f.current = Mathf.MoveTowards(f.current, f.target, time);

            //Apply animation
            AnimateFinger(f.type.ToString(), f.current);
        }
    }

    private void AnimateFinger(string scriptFinger, float blend)
    {
        animator.SetFloat(scriptFinger, blend);
    }
}
