using System;
using QuestSystem.Core.QuestLogic;
using UnityEngine;

namespace QuestSystem.Core
{
    public class QuestLocation : MonoBehaviour
    {
        private event Action<Quest, bool> inRangeEvent;
        private GameObject targetPlayer = null;
        private Quest questPointer;

        private void OnEnable()
        {
            QuestSystem.OnNeedsUpdate -= SystemUpdate;
            QuestSystem.OnNeedsUpdate += SystemUpdate;
        }
        private void OnDestroy()
        {
            QuestSystem.OnNeedsUpdate -= SystemUpdate;
        }

        private void SystemUpdate()
        {
            if (questPointer.IsQuestComplete)
            {
                Destroy(gameObject);
            }
        }

        void OnDrawGizmos() {
            if(gameObject == null || questPointer == null) { return; }

            if(!questPointer.IsQuestComplete){
                for (int i = 0; i < questPointer.QuestObjects.Count; i++)
                {
                    if(!questPointer.QuestObjects[i].GetComponent<QuestStep>()) { 
                        //return;
                    }else {
                        if(questPointer.QuestObjects[i].GetComponent<QuestStep>().enabled){
                            Gizmos.color = Color.green;
                        }else {
                            Gizmos.color = Color.red;
                        }
                    }
                }
            }
            var center = transform.position;
            Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius);
        }

        public void SetInRangeCallback(Action<Quest, bool> mActionToCall) {
            inRangeEvent = mActionToCall;
        }
        public void SetTargetPlayer(GameObject mPlayer) {
            targetPlayer = mPlayer;
        }

        public void SetQuest(Quest mQuest){
            questPointer = mQuest;
        }

        private void OnTriggerEnter(Collider mOther){
            if(mOther.GetInstanceID() == targetPlayer.GetComponent<Collider>().GetInstanceID()){
                inRangeEvent?.Invoke(questPointer, true);
            }
        }

        private void OnTriggerExit(Collider mOther) {
            if (mOther.GetInstanceID() == targetPlayer.GetComponent<Collider>().GetInstanceID()) {
                inRangeEvent?.Invoke(questPointer, false);
                if(questPointer.IsQuestComplete) { Destroy(gameObject); }
            }
        } 
    }
}
