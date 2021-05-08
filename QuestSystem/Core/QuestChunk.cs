using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem.Core
{
    [Serializable]
    public class QuestChunk
    {
        [HideInInspector] public string Name;
        public List<QuestGiverData> Quests;
    }
}
