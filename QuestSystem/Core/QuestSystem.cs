using System;
using System.Collections.Generic;
using QuestSystem.Core.QuestLogic;
using QuestSystem.Core.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace QuestSystem.Core
{
    public class QuestSystem : MonoBehaviour
    {
        private static QuestSystem _instance;
        public static QuestSystem Instance { get { return _instance; } }

        [Tooltip("The Player that the quest log belongs to")]
        [ReadOnly] public GameObject Player;
        public bool UseUnityEvents;
        public ObjectiveUnityEvents UnityEvents;
    
        public List<Quest> QuestLog;
        [ReadOnly] public List<Quest> ActiveQuests;
        [ReadOnly] public List<Quest> CompletedQuests;

        public static event Action<Quest, bool> OnQuestInRange;
        public static event Action OnNeedsUpdate; // To update UI



        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }

            DontDestroyOnLoad(gameObject);
            Player = GameObject.FindWithTag("Player");
        }

        private void OnEnable()
        {
            if (Player == null) { 
                this.enabled = false;
                return;
            }
        
            // -- Subs -- \\
            OnQuestInRange -= SetQuestState;
            Quest.OnQuestComplete -= CompleteQuest;
            Quest.OnStepComplete -= CompletedStep;      
            QuestStep.OnSubStepComplete -= CompletedSubStep;     
        
            OnQuestInRange += SetQuestState;
            Quest.OnQuestComplete += CompleteQuest;
            Quest.OnStepComplete += CompletedStep;
            QuestStep.OnSubStepComplete += CompletedSubStep;
        }


        private void OnDisable()
        {        
            // -- Subs -- \\
            OnQuestInRange -= SetQuestState;
            QuestStep.OnSubStepComplete -= CompletedSubStep;
            Quest.OnQuestComplete -= CompleteQuest;
            Quest.OnStepComplete -= CompletedStep;
        }

        public void CompleteQuest(Quest mQuest)
        {
            print("completed quest!");

            if (QuestLog.Contains(mQuest))
            {
                RemoveQuest(mQuest);
                CompletedQuests.Add(mQuest);
                OnNeedsUpdate?.Invoke();
            }
        }

        private void RemoveQuest(Quest mQuest)
        {
            int index = QuestLog.IndexOf(mQuest);
            QuestLog.RemoveAt(index);
        }
    
        private static void CompletedStep(QuestStepData mQuestData)
        {
            print("completed step!");
            OnNeedsUpdate?.Invoke();
        }

        private void CompletedSubStep()
        {
            print("completed substep!");
            OnNeedsUpdate?.Invoke();
        }

        private void Start()
        {
            // Start the system by updating it
            //OnNeedsUpdate?.Invoke();
        }



        public void AddQuest(Quest mQuest)
        {
            //TODO: use the quest giver quest reference to pass it down to the quest step component in addobjectitve step
            InitializeQuest(mQuest);

            // Add quest to log
            QuestLog.Add(mQuest);
            UnityEvents.UE_OnQuestAccepted?.Invoke();
        
            mQuest.InitAllObjectiveSteps(mQuest);
            OnNeedsUpdate?.Invoke();
        }

        /// <summary>
        /// Quest in range
        /// </summary>
        private void SetQuestState(Quest mQuest, bool mState) {
            Debug.Log("Set Quest State: " + mState.ToString());
            if (mState)//area entered
            {
                UnityEvents.UE_OnEnteredQuestArea?.Invoke();
                if (!mQuest.HasBeenInitialized)
                {
                    mQuest.HasBeenInitialized = true;
                    mQuest.InitAllObjectiveSteps(mQuest);
                    ActiveQuests.Add(mQuest);
                } 
                else {
                    SetStateOfObjectives(mQuest, true);
                }
            }
            else//area exited
            {
                UnityEvents.UE_OnExitedQuestArea?.Invoke();
                SetStateOfObjectives(mQuest, false);
                ActiveQuests.Remove(mQuest);
            }
        }

        ///// <summary>
        ///// Loops through all the objective steps in the objective and initializes them 
        ///// </summary>
        ///// <param name="objective">Current objective that needs to be initialized</param>
        //private void InitializeAllQuests(List<Quest> questLog)
        //{     
        //   foreach(Quest m_quest in questLog)
        //   {
        //        // Set location
        //        GameObject questLocation = new GameObject("Quest " + questLog.ToString());
        //        questLocation.transform.position = m_quest.questArea.ZoneData.WorldPos;
        //        // Set Trigger Radius
        //        SphereCollider areaCollider = questLocation.AddComponent<SphereCollider>();
        //        areaCollider.radius = m_quest.questArea.Radius;
        //        areaCollider.isTrigger = true;
        //        // Add component for in-radius callback
        //        QuestLocation locationComponent = questLocation.AddComponent<QuestLocation>();
        //        locationComponent.SetInRangeCallback(OnQuestInRange);
        //        locationComponent.SetTargetPlayer(Player);
        //        locationComponent.SetQuest(m_quest);
        //   }
        //}

    
        private void InitializeQuest(Quest mQuest)
        {
        
            // Set location
            GameObject questLocation = new GameObject("Quest " + mQuest.ToString());
            questLocation.transform.position = mQuest.QuestArea.ZoneData.WorldPos;
            // Set Trigger Radius
            SphereCollider areaCollider = questLocation.AddComponent<SphereCollider>();
            areaCollider.radius = mQuest.QuestArea.Radius;
            areaCollider.isTrigger = true;
            // Add component for in-radius callback
            QuestLocation locationComponent = questLocation.AddComponent<QuestLocation>();
            locationComponent.SetInRangeCallback(OnQuestInRange);
            locationComponent.SetTargetPlayer(Player);
            locationComponent.SetQuest(mQuest);   
        }


        private static void SetStateOfObjectives(Quest mQuest, bool mState)
        {
            foreach (var mQuestStepData in mQuest.Steps)
            {
                foreach (Component mObjective in mQuestStepData.QuestStepComponents)
                {          
                    if(mObjective != null && mObjective.GetComponent<QuestStep>()){
                        mObjective.GetComponent<QuestStep>().enabled = mState;
                    }
                }
            }
        }        
    
        private void CalculateFinalScore(GameObject obj)
        {

        }

    }

    [System.Serializable]
    public class ObjectiveUnityEvents {
        public UnityEvent UE_OnObjectiveComplete;
        public UnityEvent UE_OnQuestAccepted;
        public UnityEvent UE_OnEnteredQuestArea;
        public UnityEvent UE_OnExitedQuestArea;
    }
}