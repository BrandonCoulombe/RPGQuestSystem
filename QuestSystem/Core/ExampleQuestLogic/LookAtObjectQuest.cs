using UnityEngine;

namespace QuestSystem.Core.QuestLogic
{
    [RequireComponent(typeof(PC_Inspectable))]
    public class LookAtObjectQuest : QuestStep
    {   
        private PC_Inspectable _inspectableScript;
        private void Awake() => _inspectableScript = GetComponent<PC_Inspectable>();
        private bool _isSubStepActive = true;
        public override void Update()
        {
            if (_isSubStepActive)
            {

                if (_inspectableScript.IsBeingInspected && !StepData.IsStepComplete)
                {
                    base.CompletedSubStep();
                    _isSubStepActive = false;
                }
                else
                {
                    base.Update();
                }
            }
            else
            {
                base.Update();
            }
        }
    }
}
