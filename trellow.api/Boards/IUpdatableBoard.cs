namespace trellow.api.Boards
{
	public interface IUpdatableBoard
	{
		string Id { get; }
		string Name { get; }
		string Desc { get; }
		bool Closed { get; }
	}
}