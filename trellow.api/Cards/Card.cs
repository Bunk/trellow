using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using trellow.api.Checklists;
using trellow.api.Members;

namespace trellow.api.Cards
{
    public class Card : ICardId, IUpdatableCard
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public bool Closed { get; set; }
        public string IdList { get; set; }
        public string IdBoard { get; set; }
        public string IdAttachmentCover { get; set; }
        public DateTime? Due { get; set; }
        public List<Label> Labels { get; set; }
        public int IdShort { get; set; }
        public CardBadges Badges { get; set; }
        public List<Checklist> Checklists { get; set; }
        public List<Attachment> Attachments { get; set; }
        public List<Member> Members { get; set; }
        public BoardLink Board { get; set; }
        public ListLink List { get; set; }
        public string Url { get; set; }
        public double Pos { get; set; }

        public Card()
        {
            Labels = new List<Label>();
            Checklists = new List<Checklist>();
            Attachments = new List<Attachment>();
            Members = new List<Member>();
        }

        public string GetCardId()
        {
            return Id;
        }

        public override string ToString()
        {
            return Name;
        }

        public class Label
        {
            public Color Color { get; set; }
            public string Name { get; set; }
        }

        public class CardBadges
        {
            public int Votes { get; set; }
            public DateTime? Due { get; set; }
            public bool Description { get; set; }
            public int Attachments { get; set; }
            public int Comments { get; set; }
            public int CheckItemsChecked { get; set; }
            public int CheckItems { get; set; }
            public string FogBugz { get; set; }
        }

        public class Attachment : IAttachmentId
        {
            public string Id { get; set; }
            public string IdMember { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
            public int Bytes { get; set; }
            public DateTime? Date { get; set; }
            public List<Preview> Previews { get; set; }

            public string GetAttachmentId()
            {
                return Id;
            }
        }

        public class Preview
        {
            public int Width { get; set; }

            public int Height { get; set; }

            public string Url { get; set; }
        }

        public class BoardLink
        {
            public string Name { get; set; }
        }

        public class ListLink
        {
            public string Name { get; set; }
        }

        // Handling of check item state :(
        // -------------------------------
        // A Card has a list of Checklists and a Checklist has a list of CheckItems, but a CheckItem does not include the state (checked/unchecked).
        // A Card also has a list of CheckItemState with the ID of it's CheckItem and the state (which always seems to be "complete").
        // After we have deserialized go through each CheckItemState and set Checked to true on the corresponding CheckItem.
        public class CheckItem : Checklists.CheckItem
        {
            public bool Checked { get; set; }
        }

        public class Checklist : Checklist<CheckItem>
        {
        }

        private class CheckItemState
        {
            public string IdCheckItem { get; set; }
            public string State { get; set; }
        }

        [JsonProperty]
        private List<CheckItemState> CheckItemStates { get; set; }

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            var checkItems = from cl in Checklists ?? Enumerable.Empty<Checklist>()
                             from ci in cl.CheckItems
                             join cis in CheckItemStates on ci.Id equals cis.IdCheckItem
                             where cis.State == "complete"
                             select ci;

            foreach (var checkItem in checkItems)
                checkItem.Checked = true;

            if (Due.HasValue)
                Due = Due.Value.ToLocalTime();
        }
    }
}