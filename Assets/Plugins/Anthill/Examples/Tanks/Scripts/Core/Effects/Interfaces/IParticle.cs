namespace Anthill.Effects
{
	using UnityEngine;

	public interface IParticle
	{
		void Activate(AntEffect aEffect, AntEmitter aEmitter);
		void Kill();

		int EffectID { get; set; }
		float BirthTime { get; set; }
		string SortingLayer { get; set; }
		int SortingOrder { get; set; }
		GameObject GameObject { get; }
		AntEffect Effect { get; }
		float LifeTime { get; set; }
		float CurrentLifeTime { get; set; }
		bool IsActive { get; set; }
		bool IsHasCollision { get; set; }
	}
}