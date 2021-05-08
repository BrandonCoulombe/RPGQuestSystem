using QuestSystem.Core.Utils;
using UnityEngine;

namespace QuestSystem.Core.QuestLogic
{
    [RequireComponent(typeof(MeshRenderer))]
    public class PickupObjectQuest : QuestStep
    {
        private bool _isSubStepActive = true;
        public bool IsPickedUp { get; set; }

        public override void Update()
        {
            if (_isSubStepActive)
            {
                if (!StepData.IsStepComplete)
                {
                    if (IsPickedUp)
                    {
                        base.CompletedSubStep();
                        _isSubStepActive = false;
                    }
                    else
                    {
                        base.Update();
                    }
                }
            }
            else
            {
                base.Update();
            }
        }
    }
}