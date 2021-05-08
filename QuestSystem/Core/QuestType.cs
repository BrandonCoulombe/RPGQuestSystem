namespace QuestSystem.Core
{
	public enum QuestType
	{
		WalkTo,
		PickUp,
		LookAt,
		Administer,
		WaitForTime,
		FinishDiagnosis,
	}

	public enum QuestInitStyle
	{
		Spawner,
		ParentObjectGrouping,
		SingleObject,
	}
}