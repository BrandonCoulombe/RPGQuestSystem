using System;
using UnityEngine;

namespace QuestSystem.Core.QuestLogic
{
    [System.Serializable]
    public class QuestStep : MonoBehaviour
    {
        public QuestStepData StepData;
        public Quest Quest { get; set; }
        protected event Action<QuestStepData> StepCompleteCallback;
        public static event Action OnSubStepComplete;
    
        public virtual void StartObjectiveStep() { }
        public virtual void CompleteObjectiveStep() { }

        private void Start(){
            StartObjectiveStep();
        }
        public virtual void Update()
        {
            if (!StepData.IsStepComplete) return;
        
            StepCompleteCallback?.Invoke(StepData);
            CompleteObjectiveStep();
            Destroy(this);
        }

        public void SetDefaultData(QuestStepData mData) => StepData = mData;
        public void Subscribe(Action<QuestStepData> mAction)
        {
            StepCompleteCallback -= mAction;
            StepCompleteCallback += mAction;
        }
        public virtual void CompletedSubStep()
        {
            StepData.CurrentQuestProgress++;
            OnSubStepComplete?.Invoke();
        }
    }
}