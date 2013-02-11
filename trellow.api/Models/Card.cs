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

        public List<CheckItemState> CheckItemStates { get; set; }

        public List<Activity> Actions { get; set; }

        public Badges Badges { get; set; }

        public Board Board { get; set; }

        public List List { get; set; }

        public Card()
        {
            Badges = new Badges();
            Labels = new List<Label>();
            IdChecklists = new List<string>();
            Members = new List<Member>();
            Attachments = new List<Attachment>();
            Checklists = new List<CheckList>();
            CheckItemStates = new List<CheckItemState>();
            Actions = new List<Activity>();
        }
    }

    public class CheckList
    {
        public string Id { get; set; }

        public string IdBoard { get; set; }

        public string IdCard { get; set; }

        public string Name { get; set; }

        public List<CheckListItem> CheckItems { get; set; }

        public CheckList()
        {
            CheckItems = new List<CheckListItem>();
        }
    }

    // todo: This is a bug because the API doesn't return this with the CheckListItem currently
    public class CheckItemState
    {
        public string IdCheckItem { get; set; }

        public CheckListItem.CheckState State { get; set; }
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

    public class Activity
    {
        public string Id { get; set; }

        public ActivityType Type { get; set; }

        public DateTime Date { get; set; }

        public Member Member { get; set; }

        public Member MemberCreator { get; set; }

        public ActivityData Data { get; set; }
    }

    public enum ActivityType
    {
        CreateCard, DeleteCard, CommentCard, 
        AddAttachmentToCard, AddChecklistToCard, AddMemberToCard,
        DeleteAttachmentFromCard, RemoveChecklistFromCard, RemoveMemberFromCard
    }

    public class ActivityData
    {
        public DateTime? DateLastEdited { get; set; }

        public string Text { get; set; }

        public Identifier Board { get; set; }

        public Identifier Card { get; set; }

        public Identifier Attachment { get; set; }

        public Identifier Checklist { get; set; }

        public Identifier List { get; set; }

        public Identifier ListBefore { get; set; }

        public Identifier ListAfter { get; set; }

        public class Identifier
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public string Url { get; set; }
        }
    }
}
