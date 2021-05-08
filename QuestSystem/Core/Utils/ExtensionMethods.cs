using UnityEngine;

namespace QuestSystem.Core.Utils
{
	public static class ExtensionMethods
	{
		/// <summary>
		/// Loops through children and returns the first object with the specified layer
		/// </summary>
		public static GameObject GetFirstChildWithLayer(string targetLayer, Transform parentTransform)
		{
			for (int i = 0; i < parentTransform.childCount; i++)
			{	
				if (LayerMask.NameToLayer(targetLayer) == parentTransform.GetChild(i).gameObject.layer)
				{
					return parentTransform.GetChild(i).gameObject;
				}
			}
			return null;
		}

	}
}
