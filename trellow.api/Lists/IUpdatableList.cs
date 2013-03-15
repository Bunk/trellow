namespace trellow.api.Lists
{
	public interface IUpdatableList
	{
		string Id { get; }
		string Name { get; }
		bool Closed { get; }
	}
}