namespace trello.Services.Handlers
{
    /// <summary>
    /// The direction (relative to the list in the UI) that the user wanted to move the item
    /// </summary>
    public enum ListMovementDirection
    {
        Left, Right
    }

    public class CardMovedToList
    {
        public string CardId { get; set; }

        public ListMovementDirection Direction { get; set; }
    }
}
