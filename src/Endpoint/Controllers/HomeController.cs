using Application.Contracts.Persistence.Repositories;
using Application.Queries;
using Domain.Dtos.Multimedia;
using Domain.Dtos.Notices;
using Domain.Dtos.Resources;
using Domain.Dtos.Shared;
using Domain.Entities.Contact;
using Domain.Entities.Mutimedia;
using Domain.Entities.Notices;
using Domain.Entities.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IRepository<News> _newsRepo;
        private readonly IRepository<Goal> _goalRepo;
        private readonly IRepository<Book> _bookRepo;
        private readonly IRepository<Broadcast> _broadcastRepo;
        private readonly IRepository<Article> _articleRepo;
        private readonly IRepository<Gallery> _galleryRepo;
        private readonly IRepository<RelatedCompany> _compRepo;
        private readonly IRepository<VideoContent> _videoRepo;

        public HomeController(IRepository<News> newsRepo, IRepository<Goal> goalRepo, IRepository<RelatedCompany> compRepo, IRepository<Book> bookRepo, IRepository<Article> articleRepo, IRepository<Broadcast> broadcastRepo, IRepository<Gallery> galleryRepo, IRepository<VideoContent> videoRepo)
        {
            _newsRepo = newsRepo;
            _goalRepo = goalRepo;
            _compRepo = compRepo;
            _bookRepo = bookRepo;
            _articleRepo = articleRepo;
            _broadcastRepo = broadcastRepo;
            _galleryRepo = galleryRepo;
            _videoRepo = videoRepo;
        }


        [Route("[action]")]
        [HttpGet]
        public async Task<HomeSummary> Index(CancellationToken cancellationToken)
        {
            var news = await _newsRepo.GetAllAsync<NewsSummary>(new GridQuery(), cancellationToken: cancellationToken);
            var companies = await _compRepo.GetAllAsync<RelatedCompany>(filters: null, cancellationToken: cancellationToken);
            var goals = await _goalRepo.GetAllAsync<Goal>(filters: null, cancellationToken: cancellationToken);

            var data = new HomeSummary { Companies = companies, Goals = goals, News = news.Data };

            return data;
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<SearchingResultDto> Search([FromQuery] String? query, CancellationToken cancellationToken)
        {
            query = query?.Trim();
            bool hasQuery = !String.IsNullOrEmpty(query);

            return new SearchingResultDto
            {
                News = await _newsRepo.GetAllAsync<NewsSummary>(b => !hasQuery || b.Title.Contains(query)),
                Books = await _bookRepo.GetAllAsync<BookSummary>(b => !hasQuery || b.Title.Contains(query)),
                Broadcasts = await _broadcastRepo.GetAllAsync<BroadcastSummary>(b => !hasQuery || b.Title.Contains(query)),
                Articles = await _articleRepo.GetAllAsync<ArticleSummary>(b => !hasQuery || b.Title.Contains(query)),
                Galleries = await _galleryRepo.GetAllAsync<GallerySummary>(b => !hasQuery || b.Title.Contains(query)),
                Videos = await _videoRepo.GetAllAsync<VideoContentSummary>(b => !hasQuery || b.Title.Contains(query))
            };
        }
    }


}
