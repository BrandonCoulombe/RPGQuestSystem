using System;
using System.Collections.Generic;
using QuestSystem.Core.QuestLogic;
using QuestSystem.Core.Utils;
using UnityEngine;

namespace QuestSystem.Core
{
    [CreateAssetMenu(menuName = "QuestSystem/Quest")]
    public class Quest : ScriptableObject
    {

        public string QuestName;
        public bool ShowInQuestLog;
        [ReadOnly] public bool HasBeenInitialized = false;
        public QuestArea QuestArea;
        [ReadOnly][HideInInspector] public List<GameObject> QuestObjects;
        public QuestInitStyle InitializationStyle;
        public List<QuestStepData> Steps;
        public static event Action<Quest> OnQuestComplete;
        public static event Action<QuestStepData> OnStepComplete;
        [TextArea(4,20)]
        public string FullDescription;

        public bool IsQuestComplete
        {
            get
            {
                foreach (var mQuestStepData in Steps)
                {
                    if (!mQuestStepData.IsStepComplete) return false;
                }

                return true;
            }
        }

        private void OnDisable(){
            ResetDynamicQuestData();
        }

        public void InitAllObjectiveSteps(Quest mQuest)
        {
            foreach (var mQuestStepData in mQuest.Steps)
            {
                switch (InitializationStyle)
                {
                    case QuestInitStyle.Spawner:
                        break;
                    case QuestInitStyle.ParentObjectGrouping:
                        Transform mParentQuestObjectContainer = mQuestStepData.QuestObject.transform;
                        switch (mQuestStepData.Type)
                        {
                            case QuestType.WalkTo:
                           
                                for (int i = 0; i < mQuestStepData.QuestObject.transform.childCount; i++)
                                {
                                    WalkToObjectQuest mWalkToListener = mParentQuestObjectContainer.GetChild(i).gameObject.AddComponent<WalkToObjectQuest>();
                                    mWalkToListener.DistanceToTrigger = 4f;
                                    mWalkToListener.Subscribe(QuestStepComplete);
                                    mWalkToListener.SetDefaultData(mQuestStepData);
                                    mWalkToListener.Quest = mQuest;
                                    mWalkToListener.enabled = false;
                                    mQuestStepData.QuestStepComponents.Add(mWalkToListener);
                                }
                                break;
                            case QuestType.PickUp:
                                for (int i = 0; i < mQuestStepData.QuestObject.transform.childCount; i++)
                                {
                                    PickupObjectQuest mPickupObjectQuest = mParentQuestObjectContainer.GetChild(i).gameObject.AddComponent<PickupObjectQuest>();
                                    mPickupObjectQuest.Subscribe(QuestStepComplete);
                                    mPickupObjectQuest.SetDefaultData(mQuestStepData);
                                    mPickupObjectQuest.Quest = mQuest;
                                    mPickupObjectQuest.enabled = false;
                                    mQuestStepData.QuestStepComponents.Add(mPickupObjectQuest);
                                }
                                break;
                            case QuestType.LookAt:
                                for (int i = 0; i < mQuestStepData.QuestObject.transform.childCount; i++)
                                {
                                    LookAtObjectQuest mLookAtListener = mParentQuestObjectContainer.GetChild(i).gameObject.AddComponent<LookAtObjectQuest>();
                                    mLookAtListener.Subscribe(QuestStepComplete);
                                    mLookAtListener.SetDefaultData(mQuestStepData);
                                    mLookAtListener.Quest = mQuest;
                                    mLookAtListener.enabled = false;
                                    mQuestStepData.QuestStepComponents.Add(mLookAtListener);
                                }

                                break;
                            default:
                                break;
                        }
                        break;
                    case QuestInitStyle.SingleObject:
                        switch (mQuestStepData.Type)
                        {
                            case QuestType.WalkTo:
                                WalkToObjectQuest mWalkToListener = mQuestStepData.QuestObject.gameObject.AddComponent<WalkToObjectQuest>();
                                mWalkToListener.DistanceToTrigger = 4f;
                                mWalkToListener.Subscribe(QuestStepComplete);
                                mWalkToListener.SetDefaultData(mQuestStepData);
                                mWalkToListener.Quest = mQuest;
                                mWalkToListener.enabled = false;
                                mQuestStepData.QuestStepComponents.Add(mWalkToListener);
                                break;
                            case QuestType.PickUp:
                                PickupObjectQuest mPickupObjectQuest = mQuestStepData.QuestObject.gameObject.AddComponent<PickupObjectQuest>();
                                mPickupObjectQuest.Subscribe(QuestStepComplete);
                                mPickupObjectQuest.SetDefaultData(mQuestStepData);
                                mPickupObjectQuest.Quest = mQuest;
                                mPickupObjectQuest.enabled = false;
                                mQuestStepData.QuestStepComponents.Add(mPickupObjectQuest);
                                break;
                            case QuestType.LookAt:
                                LookAtObjectQuest mLookAtListener = mQuestStepData.QuestObject.AddComponent<LookAtObjectQuest>();
                                mLookAtListener.Subscribe(QuestStepComplete);
                                mLookAtListener.SetDefaultData(mQuestStepData);
                                mLookAtListener.Quest = mQuest;
                                mLookAtListener.enabled = false;
                                mQuestStepData.QuestStepComponents.Add(mLookAtListener);
                                break;
                            case QuestType.Administer:
                                AdministerQuest mAdminister = mQuestStepData.QuestObject.AddComponent<AdministerQuest>();
                                mAdminister.Subscribe(QuestStepComplete);
                                mAdminister.SetDefaultData(mQuestStepData);
                                mAdminister.Quest = mQuest;
                                mAdminister.enabled = true;
                                mQuestStepData.QuestStepComponents.Add(mAdminister);
                                break;
                            case QuestType.WaitForTime:
                                WaitForTimeQuest mWaitForTime = mQuestStepData.QuestObject.AddComponent<WaitForTimeQuest>();
                                mWaitForTime.Subscribe(QuestStepComplete);
                                mWaitForTime.SetDefaultData(mQuestStepData);
                                mWaitForTime.Quest = mQuest;
                                mWaitForTime.enabled = false;
                                mWaitForTime.TimeToWait = 15;
                                mQuestStepData.QuestStepComponents.Add(mWaitForTime);
                                break;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            
            }
            HasBeenInitialized = true;
        }


        private void QuestStepComplete(QuestStepData mQuestStep)
        {
            //TODO: Move this to the quest system maybe?
        
            if (IsQuestComplete){
                OnQuestComplete?.Invoke(this);
            } else {
                OnStepComplete?.Invoke(mQuestStep);
            }
       
            switch (mQuestStep.Type)
            {
                case QuestType.WalkTo:  
                    break;
                case QuestType.PickUp:
                    break;
                case QuestType.LookAt:
                    break;
                default:
                    break;
            }

        }
    
        private void ResetDynamicQuestData()
        {
            foreach(QuestStepData mStepData in Steps)
            {
                mStepData.CurrentQuestProgress = 0;
            }
            HasBeenInitialized = false;
            QuestObjects = new List<GameObject>();
        }
    }
}
