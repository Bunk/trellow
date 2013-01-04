﻿using System;
using System.Collections.Generic;

namespace trello.Services.Models
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

        public Badges Badges { get; set; }

        public Card()
        {
            Badges = new Badges();
            Labels = new List<Label>();
            IdChecklists = new List<string>();
            Members = new List<Member>();
        }
    }

    public class Label
    {
        public string Color { get; set; }

        public string Name { get; set; }
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

    public class Member
    {
        public string Id { get; set; }

        public string AvatarHash { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }
    }
}
