using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sCollisionStateController : StateMachineBehaviour
{
    [SerializeField] sUtil.MoveType _moveType;
    float[,] _fData;
    float _animLength;
    int numHB;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _fData = animator.gameObject.GetComponent<sPlayer>().GetPlayerMoves[_moveType];
        _animLength = _fData[0, 1];
        numHB = _fData.GetLength(0);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        for(int i = 1; i < numHB; i++)
        {
            if (stateInfo.normalizedTime >= _fData[i, 0] / _animLength && stateInfo.normalizedTime < _fData[i, 1] / _animLength)
            {
                animator.gameObject.GetComponent<sPlayer>().GetPlayerSensors.Find(
                    x => x.GetAtkType == _moveType).GetSensors[i-1].GetColliderType = sUtil.ColliderState.HitBox;

            }
            else
            {
                animator.gameObject.GetComponent<sPlayer>().GetPlayerSensors.Find(
                    x => x.GetAtkType == _moveType).GetSensors[i-1].GetColliderType = sUtil.ColliderState.None;
            }
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
