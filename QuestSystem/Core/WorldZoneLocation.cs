using UnityEngine;

namespace QuestSystem.Core
{
    [CreateAssetMenu(menuName = ("ScriptableObject/ZoneLocations"))]
    public class WorldZoneLocation : ScriptableObject
    {
        public string Name;
        public Vector3 WorldPos;
    }
}
