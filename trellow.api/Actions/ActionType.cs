namespace trellow.api.Actions
{
	public enum ActionType
	{
		CreateCard,
		CommentCard,
		UpdateCard,
		AddMemberToCard,
		RemoveMemberFromCard,
		UpdateCheckItemStateOnCard,
		AddAttachmentToCard,
		AddChecklistToCard,
		RemoveChecklistFromCard,
		CreateList,
		UpdateList,
		CreateBoard,
		UpdateBoard,
		AddMemberToBoard,
		RemoveMemberFromBoard, 
		AddToOrganizationBoard,
		RemoveFromOrganizationBoard,
		CreateOrganization,
		UpdateOrganization,
		MoveCardToBoard,
		MoveCardFromBoard,
		ConvertToCardFromCheckItem
	}
}