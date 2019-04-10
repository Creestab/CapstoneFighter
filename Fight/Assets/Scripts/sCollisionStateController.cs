using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionStateController : StateMachineBehaviour
{
    [SerializeField] sSensor.ColliderState _tempCollider;
    [SerializeField] sPlayer.enumMoves _moveType;
    float[,] _fData;
    float _animLength;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _fData = sMoveData.GetFrameData[animator.gameObject.GetComponent<sPlayer>().GetPlayerMoves[_moveType].moveName];
        _animLength = _fData[0, 1];
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool active = false;
        int hit = 1;
        if (stateInfo.normalizedTime >= _fData[1,0] / _animLength && stateInfo.normalizedTime < _fData[1,1] / _animLength)
        {
            if(!active) { active = true; }
            animator.gameObject.GetComponent<sPlayer>().GetPlayerSensors.Find(
                x => x.GetAtkColliders == _moveType).GetSensors.GetColliderState = sSensor.ColliderState.HitBox;
        }
        else
        {
            if(active) { active = false; }
            animator.gameObject.GetComponent<sPlayer>().GetPlayerSensors.Find(
                x => x.GetAtkColliders == _moveType).GetSensors.GetColliderState = sSensor.ColliderState.None;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
