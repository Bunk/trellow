using System;

namespace trellow.api.Actions
{
	public interface ISince
	{
		bool LastView { get; }
		DateTime Date { get; }
	}
}