﻿namespace Domain.Dtos.Notices
{
    public class NewsSummary
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Image { get; set; }
        public String Headline { get; set; }
        public DateTime DateOfRegisration { get; set; }
        public int ShortLink { get; set; }
        public int Order { get; set; }
    }
}
