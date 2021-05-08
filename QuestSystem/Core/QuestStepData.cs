using System.Collections.Generic;
using QuestSystem.Core.Utils;
using UnityEngine;

namespace QuestSystem.Core
{
  [System.Serializable]
  public class QuestStepData
  {
    [HideInInspector] public string Name;
    public QuestType Type;
    public GameObject InteractTarget;
    [TextArea(1, 2)]
    public string Description;

    [HideInInspector] public GameObject QuestObject;
    [Tooltip("1 if its a simple find objet - increase this for mob kill counts and finding multiple objects of the same type")]
    public int NumQuestActionsNeeded;
    [Tooltip("current progress towards the num actions needed")]
    [ReadOnly] public int CurrentQuestProgress;
    [HideInInspector] public List<Component> QuestStepComponents;
  
    public bool IsStepComplete => CurrentQuestProgress == NumQuestActionsNeeded;
  }
}

