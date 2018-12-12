using ZStart.Common.Enum;
using ZStart.Common.Manager;
using ZStart.Common.Model;
using UnityEngine;

namespace ZStart.Common.Action
{
    public class AnimatorAction : StateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            AnimatorInfo info = new AnimatorInfo();
            info.animator = animator;
            info.status = AnimActionStatus.Enter;
            info.nameHash = stateInfo.shortNameHash;
            NotifyManager.SendNotify(NotifyType.OnAnimatorUpdate, info);
        }


        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            AnimatorInfo info = new AnimatorInfo();
            info.animator = animator;
            info.status = AnimActionStatus.Exit;
            info.nameHash = stateInfo.shortNameHash;
            NotifyManager.SendNotify(NotifyType.OnAnimatorUpdate, info);
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
    }
}
