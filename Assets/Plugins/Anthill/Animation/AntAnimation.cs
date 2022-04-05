namespace Anthill.Animation
{
	using UnityEngine;

	[CreateAssetMenuAttribute(fileName = "ActorAnimation", menuName = "Anthill/Animation", order = 1)]
	public class AntAnimation : ScriptableObject
	{
		public string key = "<Unnamed>";
		public Sprite[] frames;
	}
}