using UnityEngine;

namespace QuestSystem.Core.Utils
{
    /// <summary>
    /// Causes this object to always face towards the camera
    /// </summary>
    public class Billboarding : MonoBehaviour
    {
        private Camera _cam;
        private void Awake()
        {
            _cam = Camera.main;
        }

        private void Update()
        {
            transform.LookAt(transform.position + _cam.transform.rotation * Vector3.forward, _cam.transform.rotation * Vector3.up);
        }
    }
}
