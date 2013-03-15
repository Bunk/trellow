using System;

namespace trellow.api.Cards
{
	public interface IUpdatableCard
	{
		string Id { get; }
		string Name { get; }
		string Desc { get; }
		bool Closed { get; }
		string IdList { get; }
		DateTime? Due { get; }
	}
}