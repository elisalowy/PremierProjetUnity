using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAnimationBehavior : StateMachineBehaviour
{
    private bool eventThrowDone = false;
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ThrowObject to = animator.GetComponent<ThrowObject>();
        if (stateInfo.normalizedTime > to.MomentToThrow)
        {
            if(!eventThrowDone)
                animator.GetComponent<ThrowObject>().ThrowTheObject();
            eventThrowDone = true;
        }
        else
        {
            eventThrowDone = false;
        }
            
    }
}
