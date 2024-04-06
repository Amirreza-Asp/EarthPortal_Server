using Application.Contracts.Persistence.Repositories;
using Application.Queries;
using Domain.Dtos.Notices;
using Domain.Dtos.Shared;
using Domain.Entities.Contact;
using Domain.Entities.Notices;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IRepository<News> _newsRepo;
        private readonly IRepository<Goal> _goalRepo;
        private readonly IRepository<RelatedCompany> _compRepo;

        public HomeController(IRepository<News> newsRepo, IRepository<Goal> goalRepo, IRepository<RelatedCompany> compRepo)
        {
            _newsRepo = newsRepo;
            _goalRepo = goalRepo;
            _compRepo = compRepo;
        }


        [Route("Index")]
        [HttpGet]
        public async Task<HomeSummary> Index(CancellationToken cancellationToken)
        {
            var news = await _newsRepo.GetAllAsync<NewsSummary>(new GridQuery(), cancellationToken: cancellationToken);
            var companies = await _compRepo.GetAllAsync<RelatedCompany>(filters: null, cancellationToken: cancellationToken);
            var goals = await _goalRepo.GetAllAsync<Goal>(filters: null, cancellationToken: cancellationToken);

            var data = new HomeSummary { Companies = companies, Goals = goals, News = news.Data };

            return data;
        }
    }
}
