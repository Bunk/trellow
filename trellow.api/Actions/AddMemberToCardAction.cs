﻿namespace trellow.api.Actions
{
	public class AddMemberToCardAction : Action
	{
		public ActionData Data { get; set; }
		public ActionMember Member { get; set; }

		public class ActionData
		{
			public BoardName Board { get; set; }
			public CardName Card { get; set; }
			public string IdMember { get; set; }
		}
	}
}