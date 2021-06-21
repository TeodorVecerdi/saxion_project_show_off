using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime {
    public class NpcAI : MonoBehaviour {
        private enum AIState {
            Wander,
            ThrowTrashGround,
            ApproachBin,
            ThrowTrashBin
        }

        private NavMeshAgent agent;
        private AIState aiState;
        private bool shouldChangeState;
        private Coroutine currentStateCoroutine;

        private void Awake() {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Start() {
            shouldChangeState = true;
        }

        private void Update() {
            if (shouldChangeState) {
                StartCoroutine(GetNextState());
            }
        }

        private IEnumerator GetNextState() {
            // Do some fancy calculations to figure out which state
            
             /*
             if (aiState == AIState.ApproachBin) {
                 aiState = AIState.ThrowTrashBin;
                 return ThrowTrashBin();
             }
             if (shouldThrowTrash) {
                if (TrashBinInRadius()) {
                    aiState = AIState.ApproachBin;
                    return ApproachBin();
                }
                aiState = AIState.ThrowTrashGround;
                return ThrowTrashGround();
             }
             */

            aiState = AIState.Wander;
            return Wander();
        }

        private IEnumerator Wander() {
            while (true) {
                
                Debug.Log("Wander");
                yield return new WaitForSeconds(1.0f);
                
                if(shouldChangeState) yield break;
            }
        }
    }
}