using System;
using System.Collections.Generic;

namespace trellow.api.Models
{
    public class Card
    {
        public string Id { get; set; }

        public int IdShort { get; set; }

        public string IdBoard { get; set; }

        public string IdList { get; set; }

        public List<string> IdChecklists { get; set; }

        public string IdAttachmentCover { get; set; }

        public bool ManualCoverAttachment { get; set; }

        public bool Closed { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public DateTime? Due { get; set; }

        public List<Label> Labels { get; set; }

        public List<Member> Members { get; set; }

        public List<Attachment> Attachments { get; set; }

        public List<CheckList> Checklists { get; set; }

        public Badges Badges { get; set; }

        public InnerBoard Board { get; set; }

        public InnerList List { get; set; }

        public Card()
        {
            Badges = new Badges();
            Labels = new List<Label>();
            IdChecklists = new List<string>();
            Members = new List<Member>();
            Attachments = new List<Attachment>();
            Checklists = new List<CheckList>();
        }

        public class InnerBoard
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public string Desc { get; set; }

            public bool Pinned { get; set; }

            public bool Closed { get; set; }
        }

        public class InnerList
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public bool Closed { get; set; }
        }
    }

    public class CheckList
    {
        public string Id { get; set; }

        public string IdBoard { get; set; }

        public string Name { get; set; }

        public List<CheckListItem> CheckItems { get; set; }

        public CheckList()
        {
            CheckItems = new List<CheckListItem>();
        }
    }

    public class CheckListItem
    {
        public enum CheckState
        {
            Incomplete, Complete
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public CheckState State { get; set; }
    }

    public class Badges
    {
        public int Votes { get; set; }

        public bool ViewingMemberVoted { get; set; }

        public bool Subscribed { get; set; }

        public int CheckItems { get; set; }

        public int CheckItemsChecked { get; set; }

        public int Comments { get; set; }

        public int Attachments { get; set; }

        public bool Description { get; set; }

        public DateTime? Due { get; set; }
    }
}
