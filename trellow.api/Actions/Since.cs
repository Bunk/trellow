using System;
using trellow.api.Actions.Internal;

namespace trellow.api.Actions
{
	public static class Since
	{
		public static ISince LastView { get { return new SinceLastView();} }
		public static ISince Date(DateTime date) { return new SinceDate(date); }
	}
}