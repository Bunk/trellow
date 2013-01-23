using System;
using System.Collections.Generic;

namespace trellow.api.Models
{
    public class Attachment
    {
        public string Id { get; set; }

        public string IdMember { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public int Bytes { get; set; }

        public DateTime Date { get; set; }

        public List<Preview> Previews { get; set; }

        public Attachment()
        {
            Previews = new List<Preview>();
        }

        public class Preview
        {
            public int Width { get; set; }

            public int Height { get; set; }

            public string Url { get; set; }
        }
    }
}