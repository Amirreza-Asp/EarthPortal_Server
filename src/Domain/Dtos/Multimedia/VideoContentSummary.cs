﻿namespace Domain.Dtos.Multimedia
{
    public class VideoContentSummary
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public String Video { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
        public String Link { get; set; }
    }
}
