using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuestSystem.Core
{
	public class UIQuestLog : MonoBehaviour
	{
		private global::QuestSystem.Core.QuestSystem system;
		public GridLayoutGroup Grid;
		public GameObject QuestObjectiveField;
		public GameObject Label;

		//is the questlog in the middle of updating its content

		void Awake(){
			global::QuestSystem.Core.QuestSystem.OnNeedsUpdate -= ObjectiveSystem_OnUpdate;
			global::QuestSystem.Core.QuestSystem.OnNeedsUpdate += ObjectiveSystem_OnUpdate;
		}
		private void Start()
		{
			system = global::QuestSystem.Core.QuestSystem.Instance;
		}


		/// <summary>
		/// Re loads the objective list
		/// </summary>
		private void ObjectiveSystem_OnUpdate()
		{
			Label.SetActive(system.QuestLog.Count != 0);
			StartCoroutine(ReloadUIData());
		}

		private IEnumerator ReloadUIData()
		{
			yield return new WaitForSeconds(0.1f); // Data buffer for event call

			for (int i = 0; i < Grid.transform.childCount; i++)
			{
				Destroy(Grid.transform.GetChild(i).gameObject);
			}

			yield return new WaitForEndOfFrame();
		
			foreach (var mQuest in system.QuestLog)
			{
				if (!mQuest.ShowInQuestLog) continue;
				print("adding quest " + mQuest.QuestName);
				AddQuestToUI(mQuest);
			}
		
			StopAllCoroutines();
			yield return null;
		}

		private void AddQuestToUI(Quest mQuest)
		{
			var mNewQuestField = Instantiate(QuestObjectiveField, Grid.transform);
			mNewQuestField.GetComponent<TextMeshProUGUI>().text = GetQuestString(mQuest);
		}

		private static string GetQuestString(Quest mQuest)
		{
			string mText = "\n<color=#ffb347>" + mQuest.QuestName + "</color>";

			foreach (var mQuestStepData in mQuest.Steps)
			{
				var mProgress = 0;
				mProgress = !mQuestStepData.IsStepComplete ? mQuestStepData.CurrentQuestProgress : mQuestStepData.NumQuestActionsNeeded;
			
				mText += "\n ";
				mText += "<size=80%>" + mProgress.ToString() + "/" + mQuestStepData.NumQuestActionsNeeded;
				mText += mQuestStepData.Description;
			}
			return mText;
		}
	}
}
