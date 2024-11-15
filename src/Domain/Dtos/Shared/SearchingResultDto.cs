using Domain.Dtos.Multimedia;
using Domain.Dtos.Notices;
using Domain.Dtos.Resources;

namespace Domain.Dtos.Shared
{
    public class SearchingResultDto
    {
        public List<NewsSummary> News { get; set; }
        public List<BookSummary> Books { get; set; }
        public List<BroadcastSummary> Broadcasts { get; set; }
        public List<ArticleSummary> Articles { get; set; }
        public List<GallerySummary> Galleries { get; set; }
        public List<VideoContentSummary> Videos { get; set; }
    }
}
