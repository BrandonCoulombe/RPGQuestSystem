using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem.Core
{
    [CreateAssetMenu(menuName = "SriptableObjects/RequiredObjectives")]
    public class RequiredObjectives : ScriptableObject
    {
        public List<Quest> RequiredToComplete;
    }
}
