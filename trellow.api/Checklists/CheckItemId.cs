using trellow.api.Internal;

namespace trellow.api.Checklists
{
    public class CheckItemId : ICheckItemId
    {
        private readonly string _id;

		public CheckItemId(string id)
		{
			Guard.NotNullOrEmpty(id, "id");

			_id = id;
		}

		public string Id
		{
			get { return _id; }
		}

        public string GetCheckItemId()
        {
            return Id;
        }
    }
}
