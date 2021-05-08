using System;
using UnityEngine;
using UnityEngine.UI;

namespace QuestSystem.Core
{
    public class QuestPrompt : MonoBehaviour
    {
        public GameObject PromptWindow;
        public Text QuestDescription;

        private Action<bool, QuestGiverData> _playerChoice;
        private QuestGiverData _promptedQuest;

        private FirstPersonLook _cameraControlReference;

        private void Awake()
        {
            _cameraControlReference =
                global::QuestSystem.Core.QuestSystem.Instance.Player.GetComponentInChildren<FirstPersonLook>();
        }

        private void OnEnable()
        {
            QuestGiver.OnPromptQuest -= OpenWindow;
            QuestGiver.OnPromptQuest += OpenWindow;
        }
    

        private void OpenWindow(QuestGiverData mQuestGiverData, Action<bool, QuestGiverData> mCallback)
        {
            EnableCursor();
            LockCamera();

            _promptedQuest = mQuestGiverData;
            _playerChoice = mCallback;
        
            QuestDescription.text = mQuestGiverData.Quest.FullDescription;
            PromptWindow.SetActive(true);
        }



        public void ExitWindow()
        {
            QuestDescription.text = "";
            _playerChoice?.Invoke(false, _promptedQuest);
            PromptWindow.SetActive(false);
        
            DisableCursor();
            UnlockCamera();
        }

        public void QuestAccepted()
        {
            _playerChoice?.Invoke(true, _promptedQuest);
            PromptWindow.SetActive(false);
        
            DisableCursor();
            UnlockCamera();
        }

        public void QuestDeclined() => ExitWindow();

        private static void EnableCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    
        private static void DisableCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    
        private void LockCamera() => _cameraControlReference.CanControl = false;
    
        private void UnlockCamera() => _cameraControlReference.CanControl = true;
    
    }
}
