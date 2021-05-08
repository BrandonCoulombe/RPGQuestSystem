using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace QuestSystem.Core
{
	public class QuestGiver : MonoBehaviour
	{
		public bool AutoAccept;
		public List<QuestChunk> QuestLine;
		
		private GameObject _player;
		private QuestSystem _playerQuestSystem;
		private int _questChunkIndex = 0;
		private bool _currentlyOfferingQuest;
		private Coroutine _offerQuestsRoutine;
		public static event Action<QuestGiverData, Action<bool, QuestGiverData>> OnPromptQuest; 
		
		public bool HasCompletedQuestChunk
		{
			get
			{
				if (_questChunkIndex >= QuestLine.Count) return false;
				print("here");
				foreach (var mQuestChunk in QuestLine[_questChunkIndex].Quests)
				{
					if (mQuestChunk.CanGiveThisQuest || !mQuestChunk.Quest.IsQuestComplete) return false;
				}
				return true;
			}
		}


		private void OnDrawGizmos()
		{
			//Gizmos.color = Color.yellow;
			//Gizmos.DrawWireSphere(transform.position, 5);
		}

		private void Start()
		{
			_playerQuestSystem = QuestSystem.Instance;
			_player = QuestSystem.Instance.Player;
		
		}

		private void OnEnable() {
			QuestSystem.OnNeedsUpdate -= QuestSystemOnOnNeedsUpdate;
			QuestSystem.OnNeedsUpdate += QuestSystemOnOnNeedsUpdate;
			TryInitializeQuestGiver();
		}

		private void QuestSystemOnOnNeedsUpdate()
		{
			print(HasCompletedQuestChunk);

			if (HasCompletedQuestChunk)
			{
				_questChunkIndex++;
				AutoAcceptAvailableQuests(QuestLine[_questChunkIndex].Quests);
			}
		}

		/// <summary>
		/// This method bridges the scriptable object data with in-game runtime data
		/// The quest objects (referenced in this quest giver) are sent to the scriptable object and are stored on the player
		/// </summary>
		private void TryInitializeQuestGiver()
		{
			if (QuestLine.Count <= 0) return;

			for (int i = 0; i < QuestLine.Count; i++)
			{
				for (int j = 0; j < QuestLine[i].Quests.Count; j++)
				{	
					SetQuestDataToSO(QuestLine[i].Quests[j]);
				}
			}
		}

		private static void SetQuestDataToSO(QuestGiverData mQuestData)
		{
			if(mQuestData.Quest != null) {		 
				mQuestData.Quest.QuestObjects = mQuestData.QuestObjects; 
				for (int i = 0; i < mQuestData.Quest.Steps.Count; i++)
				{	
					mQuestData.Quest.Steps[i].QuestObject = mQuestData.QuestObjects[i];
					if (mQuestData.Quest.Steps[i].Type == QuestType.Administer)
					{
						mQuestData.Quest.Steps[i].InteractTarget = mQuestData.InteractTarget;
					}
				}
			} else {
				throw new NullReferenceException("Quest Giver Has No Quest");
			}
		}

		private void OnTriggerEnter(Collider mOther)
		{
			if(_player.GetInstanceID() == mOther.gameObject.GetInstanceID())
			{
				OfferPlayerQuests(QuestLine[_questChunkIndex].Quests);
			}	
		}
	
		private void OfferPlayerQuests(IReadOnlyList<QuestGiverData> mQuestChunks) =>
			_offerQuestsRoutine = StartCoroutine(Co_OfferPlayerQuests(mQuestChunks));


		private IEnumerator Co_OfferPlayerQuests(IReadOnlyList<QuestGiverData> mQuestChunks)
		{
			if (AutoAccept)
			{
				AutoAcceptAvailableQuests(mQuestChunks);
				if(_offerQuestsRoutine != null) StopCoroutine(_offerQuestsRoutine);
			}
			
			_currentlyOfferingQuest = true;
			foreach (var mQuestGiverData in mQuestChunks)
			{
				if (!mQuestGiverData.CanGiveThisQuest) continue;

				OnPromptQuest?.Invoke(mQuestGiverData, PlayerChoice);
				yield return new WaitUntil(() => _currentlyOfferingQuest == false);
			}
			yield return null;
		}

		private void PlayerChoice(bool mChoice, QuestGiverData mQuestGiverData)
		{
			_currentlyOfferingQuest = false;
		
			if (mChoice == false) return;
			_playerQuestSystem.AddQuest(mQuestGiverData.Quest);
			mQuestGiverData.CanGiveThisQuest = false;
		}
		
		public void GiveQuest(Quest mQuest) => _playerQuestSystem.AddQuest(mQuest);
		
		private void AutoAcceptAvailableQuests(IReadOnlyList<QuestGiverData> mQuestChunks)
		{
			foreach (var mQuestGiverData in mQuestChunks)
			{
				PlayerChoice(true, mQuestGiverData);
			}
		}
	}

	[Serializable]
	public class QuestGiverData
	{
		public Quest Quest;
		public List<GameObject> QuestObjects;
		[CanBeNull] public GameObject InteractTarget;
		public bool CanGiveThisQuest;
	}
}