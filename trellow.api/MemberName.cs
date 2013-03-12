namespace TrelloNet
{
    public class MemberName : IMemberId
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string GetMemberId()
        {
            return Id;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}