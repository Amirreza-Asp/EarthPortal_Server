using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Utilities;
using Domain.Dtos.Contact;
using Domain.Dtos.Resources;
using Domain.Entities.Account;
using Domain.Entities.Contact;
using Domain.Entities.Contact.Enums;
using Domain.Entities.Mutimedia;
using Domain.Entities.Notices;
using Domain.Entities.Regulation;
using Domain.Entities.Regulation.Enums;
using Domain.Entities.Regulation.ValueObjects;
using Domain.Entities.Resources;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Utilities
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly Random rnd;
        private readonly IPasswordManager _passManager;
        public DbInitializer(ApplicationDbContext context, IPasswordManager passManager)
        {
            _context = context;
            rnd = new Random();
            _passManager = passManager;
        }

        public async Task Execute()
        {
            await _context.Database.EnsureDeletedAsync();

            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
            }


            #region Regulation
            if (!_context.ApprovalAuthority.Any())
            {
                _context.ApprovalAuthority.AddRange(ApprovalAuthorities);
                await _context.SaveChangesAsync();
            }

            if (!_context.ApprovalStatus.Any())
            {
                _context.ApprovalStatus.AddRange(ApprovalStatuses);
                await _context.SaveChangesAsync();
            }

            if (!_context.ApprovalType.Any())
            {
                _context.ApprovalType.AddRange(ApprovalTypes);
                await _context.SaveChangesAsync();
            }

            if (!_context.ExecutorManagment.Any())
            {
                _context.ExecutorManagment.AddRange(ExecutorManagments);
                await _context.SaveChangesAsync();
            }

            if (!_context.LawCategory.Any())
            {
                _context.LawCategory.AddRange(LawCategories);
                await _context.SaveChangesAsync();
            }

            if (!_context.Law.Any())
            {
                var laws = Laws;
                var categories = await _context.LawCategory.ToListAsync();
                var approvalAuthority = await _context.ApprovalAuthority.ToListAsync();
                var approvalStatus = await _context.ApprovalStatus.ToListAsync();
                var approvalType = await _context.ApprovalType.ToListAsync();
                var executorManagment = await _context.ExecutorManagment.ToListAsync();
                var rnd = new Random();

                laws.ForEach(law =>
                {
                    law.LawCategoryId = categories[rnd.Next(0, categories.Count)].Id;
                    law.ApprovalAuthorityId = approvalAuthority[rnd.Next(0, approvalAuthority.Count)].Id;
                    law.ApprovalStatusId = approvalStatus[rnd.Next(0, approvalStatus.Count)].Id;
                    law.ApprovalTypeId = approvalType[rnd.Next(0, approvalType.Count)].Id;
                    law.ExecutorManagmentId = executorManagment[rnd.Next(0, executorManagment.Count)].Id;
                });
                _context.Law.AddRange(laws);

                await _context.SaveChangesAsync();

            }
            #endregion

            #region Multimedia
            if (!_context.Gallery.Any())
            {
                for (int i = 1; i <= 60; i++)
                {
                    var imageGallery = new Gallery($"عنوان {i}", lorem.Substring(50));
                    _context.Gallery.Add(imageGallery);
                }

                _context.SaveChanges();


                var galleriesId = await _context.Gallery.Select(b => b.Id).ToListAsync();

                foreach (var id in galleriesId)
                {
                    foreach (var image in Images)
                    {
                        image.GalleryId = id;
                        _context.GalleryPhoto.Add(image);
                    }
                }


                await _context.SaveChangesAsync();
            }

            if (!_context.VideoContent.Any())
            {
                for (int i = 1; i < 53; i++)
                {
                    var videoContent = new VideoContent($"عنوان {i}", lorem.Substring(50), video);
                    _context.VideoContent.Add(videoContent);
                }

                await _context.SaveChangesAsync();
            }

            if (!_context.Infographic.Any())
            {
                for (int i = 1; i < 58; i++)
                {
                    var infographic = new Infographic($"{rnd.Next(1, 4)}.jpg");
                    _context.Infographic.Add(infographic);
                }

                await _context.SaveChangesAsync();
            }
            #endregion

            #region Notices
            if (!_context.NewsCategory.Any())
            {
                _context.NewsCategory.AddRange(NewsCategories);
                await _context.SaveChangesAsync();
            }

            if (!_context.News.Any())
            {
                var categoriesId = _context.NewsCategory.Select(b => b.Id).ToList();
                for (int i = 0; i < 43; i++)
                {
                    var shortLink = rnd.Next(Convert.ToInt32(Math.Pow(10, 7)), Convert.ToInt32(Math.Pow(10, 8)));
                    var news = new News(News.Title + i, News.Description, News.Headline, News.Source, News.DateOfRegisration, News.NewsCategoryId, shortLink);
                    news.NewsCategoryId = categoriesId[rnd.Next(categoriesId.Count)];
                    _context.News.Add(news);
                }

                _context.SaveChanges();
            }

            if (!_context.NewsImage.Any())
            {
                var newsIds = _context.News.Select(b => b.Id).ToList();
                foreach (var newsId in newsIds)
                {
                    var newsImage = new NewsImage($"{rnd.Next(1, 6)}.jpg", newsId, 0);
                    _context.NewsImage.Add(newsImage);
                }

                await _context.SaveChangesAsync();
            }

            if (!_context.Link.Any())
            {
                for (int i = 1; i <= 10; i++)
                {
                    var link = new Link($"کلید واژه {i}");
                    _context.Link.Add(link);
                }
                await _context.SaveChangesAsync();
            }

            if (!_context.NewsLink.Any())
            {
                var newsIds = await _context.News.Select(b => b.Id).ToListAsync();
                var linksIds = await _context.Link.Select(b => b.Id).ToListAsync();

                foreach (var newsId in newsIds)
                {
                    var links = linksIds.Skip(rnd.Next(0, linksIds.Count - 5)).Take(rnd.Next(1, 6)).ToList();
                    links.ForEach(linkId =>
                    {
                        var newsLink = new NewsLink(newsId, linkId);
                        _context.NewsLink.Add(newsLink);
                    });
                }

                await _context.SaveChangesAsync();
            }
            #endregion

            #region Contact
            if (!_context.FrequentlyAskedQuestions.Any())
            {
                for (int i = 0; i < 6; i++)
                    _context.FrequentlyAskedQuestions.Add(FrequentlyAskedQuestions);
                await _context.SaveChangesAsync();
            }

            if (!_context.Guide.Any())
            {
                for (int i = 0; i < 10; i++)
                {
                    var guide = Guide;
                    guide.IsPort = i < 5;
                    _context.Guide.Add(guide);
                }
                await _context.SaveChangesAsync();
            }

            if (!_context.Info.Any())
            {
                var info = new Info("1649", "iraneland@ito.gov.ir", "#", "#", "#", "#");
                _context.Info.Add(info);
                await _context.SaveChangesAsync();
            }

            if (!_context.GeoAddress.Any())
            {
                var infoId = await _context.Info.Select(b => b.Id).FirstOrDefaultAsync();
                var geoAddress1 = new GeoAddress(35.690732000445955, 51.38562655309216, $"تهران ، خیابان نواب صفوی ، کوچه شهید صفوی ، ساختمان 2", infoId);
                var geoAddress2 = new GeoAddress(35.680832000445955, 51.37562655309216, $"تهران ، خیابان نواب صفوی ، کوچه شهید صفوی ، ساختمان 2", infoId);

                _context.GeoAddress.Add(geoAddress1);
                _context.GeoAddress.Add(geoAddress2);

                await _context.SaveChangesAsync();
            }

            if (!_context.EducationalVideo.Any())
            {
                for (int i = 0; i < 10; i++)
                {
                    var video = new EducationalVideo($"عنوان ${i + 1}", "درگاه متقاضیان خدمات زمین با هدف کاهش سردرگمی در فرآیندهای دریافت خدمات حوزه زمین و کاهش هزینه و زمان ارائه این خدمات، ثبت و پیگیری درخواست متقاضیان از طریق درگاه ارتباطی متقاضیان خدمات زمین را فراهم می‌کند",
                        "<style>.h_iframe-aparat_embed_frame{position:relative;}.h_iframe-aparat_embed_frame .ratio{display:block;width:100%;height:auto;}.h_iframe-aparat_embed_frame iframe{position:absolute;top:0;left:0;width:100%;height:100%;}</style><div class=\"h_iframe-aparat_embed_frame\"><span style=\"display: block;padding-top: calc(57% + 65px)\"></span><iframe src=\"https://www.aparat.com/video/video/embed/videohash/tL4gz/vt/frame\"  allowFullScreen=\"true\" webkitallowfullscreen=\"true\" mozallowfullscreen=\"true\"></iframe></div>");

                    _context.EducationalVideo.Add(video);
                }

                await _context.SaveChangesAsync();
            }

            if (!_context.RelatedCompany.Any())
            {
                var items = RelatedCompanies;
                items.ForEach(item => item.Image = $"comp{item.Image}");
                _context.RelatedCompany.AddRange(items);
                await _context.SaveChangesAsync();
            }

            if (!_context.Goal.Any())
            {
                for (int i = 1; i <= 30; i++)
                {
                    var goal = new Goal($"جلوگیری از تخریب محیط زیست کشور {i}");
                    _context.Goal.Add(goal);
                }
                await _context.SaveChangesAsync();
            }

            if (!_context.AboutUs.Any())
            {
                for (int i = 1; i < 6; i++)
                {
                    var aboutUs = new AboutUs("لورم ایپسوم متن", "لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع.", null, null);

                    if (i % 2 == 0)
                        aboutUs.Image = "1.jpg";
                    else
                        aboutUs.Video = "<style>.h_iframe-aparat_embed_frame{position:relative;}.h_iframe-aparat_embed_frame .ratio{display:block;width:100%;height:auto;}.h_iframe-aparat_embed_frame iframe{position:absolute;top:0;left:0;width:100%;height:100%;}</style><div class=\"h_iframe-aparat_embed_frame\"><span style=\"display: block;padding-top: 57%\"></span><iframe src=\"https://www.aparat.com/video/video/embed/videohash/QUrRF/vt/frame\"  allowFullScreen=\"true\" webkitallowfullscreen=\"true\" mozallowfullscreen=\"true\"></iframe></div>";

                    _context.AboutUs.Add(aboutUs);
                }

                await _context.SaveChangesAsync();
            }

            if (!_context.SystemEvaluation.Any())
            {
                for (int i = 0; i < 50; i++)
                {
                    var se = new SystemEvaluation(rnd.Next(1, 6));
                    _context.SystemEvaluation.Add(se);

                    var pagesCount = rnd.Next(3, 10);
                    for (int j = 0; j < pagesCount; j++)
                    {
                        var page = rnd.Next(1, 8);
                        var sePage = new SystemEvaluationPage((Pages)(page * 10), se.Id);

                        _context.SystemEvaluationPage.Add(sePage);
                    }

                    var methodsCount = rnd.Next(3, 10);
                    for (int j = 0; j < methodsCount; j++)
                    {
                        var methods = rnd.Next(1, 6);
                        var seMethod = new SystemEvaluationIntroductionMethod((IntroductionMethod)(methods * 10), se);

                        _context.IntroductionMethod.Add(seMethod);
                    }
                }

                await _context.SaveChangesAsync();
            }
            #endregion

            #region Account
            if (!_context.Role.Any())
            {
                var role = new Role("Admin", "ادمین", "ادمین کل سیستم");
                _context.Role.Add(role);
                await _context.SaveChangesAsync();
            }


            if (!_context.User.Any())
            {
                var role = await _context.Role.FirstOrDefaultAsync();
                var user = new User("3360408330", role.Id, "امیررضا", "محمدی", "Admin", _passManager.HashPassword("Admin"), "amirrezamohammadi8102@gmail.com", "09211573936");

                _context.User.Add(user);
                await _context.SaveChangesAsync();
            }
            #endregion

            #region Resources
            if (!_context.Author.Any())
            {
                _context.Author.AddRange(Authors);
                await _context.SaveChangesAsync();
            }

            if (!_context.Translator.Any())
            {
                var translator = new Translator("مهدی امینی");
                _context.Translator.Add(translator);
                await _context.SaveChangesAsync();
            }

            if (!_context.Publication.Any())
            {
                var publication = new Publication("انتشارات کتابسرای تندیس");
                _context.Publication.Add(publication);
                await _context.SaveChangesAsync();
            }

            if (!_context.Book.Any())
            {
                var authorId = _context.Author.First().Id;
                var translatorId = _context.Translator.First().Id;
                var publicationId = _context.Publication.First().Id;
                for (int i = 1; i <= 25; i++)
                {
                    String image = $"{i % 5 + 1}.png";
                    var book = new Book($"کتاب سواد بصری {i}", "<p>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها، و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.</p><h3 class='mt-40'>لورم ایپسوم متن ساختگی با تولید</h3><p class='mt-30'>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها، و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.</p><p class='mt-40'>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها، و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.</p><p class='mt-40'>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها، و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.</p>", "test.pdf",
                        publishDate: DateTime.Now.AddDays(-40), authorId, image, shortDescription: lorem, 150, translatorId, publicationId);

                    _context.Book.Add(book);
                }

                await _context.SaveChangesAsync();
            }

            if (!_context.Broadcast.Any())
            {
                var authorId = _context.Author.First().Id;
                var translatorId = _context.Translator.First().Id;
                var publicationId = _context.Publication.First().Id;
                for (int i = 1; i <= 25; i++)
                {
                    String image = $"{i % 5 + 1}.png";
                    var broadcast = new Broadcast($"نشریه سواد بصری {i}", "<p>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها، و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.</p><h3 class='mt-40'>لورم ایپسوم متن ساختگی با تولید</h3><p class='mt-30'>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها، و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.</p><p class='mt-40'>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها، و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.</p><p class='mt-40'>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها، و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.</p>", "test.pdf",
                        publishDate: DateTime.Now.AddDays(-40), authorId, image, shortDescription: lorem, 150, translatorId, publicationId);

                    _context.Broadcast.Add(broadcast);
                }

                await _context.SaveChangesAsync();
            }

            if (!_context.Article.Any())
            {
                var authorId = _context.Author.First().Id;
                for (int i = 1; i <= 25; i++)
                {
                    String image = $"{i % 5 + 1}.png";
                    var translatorId = _context.Translator.First().Id;
                    var publicationId = _context.Publication.First().Id;
                    var article = new Article($"مقاله سواد بصری {i}", "<p>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها، و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.</p><h3 class='mt-40'>لورم ایپسوم متن ساختگی با تولید</h3><p class='mt-30'>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها، و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.</p><p class='mt-40'>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها، و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.</p><p class='mt-40'>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها، و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.</p>", "test.pdf",
                        publishDate: DateTime.Now.AddDays(-40), authorId, image, shortDescription: lorem, 150, translatorId, publicationId);

                    _context.Article.Add(article);
                }

                await _context.SaveChangesAsync();
            }
            #endregion

        }

        #region Regulation
        private List<ApprovalAuthority> ApprovalAuthorities =>
            new List<ApprovalAuthority>
            {
                new ApprovalAuthority("وزراي عضو شوراي عالي مناطق آزاد تجاري -صنعتي و ويژه اقتصادي") ,
                new ApprovalAuthority("هيات امناي حساب ذخيره ارزي"),
                new ApprovalAuthority("شوراي نگهبان"),
                new ApprovalAuthority("وزارت صنعت،معدن و تجارت"),
                new ApprovalAuthority("وزراي عضو كميسيون ماده (1) آيين نامه اجرايي قانون مقررات صادرات و واردات"),
                new ApprovalAuthority("مجمع تشخيص مصلحت نظام"),
                new ApprovalAuthority("كميسيون ارزيابي و تصويب نشانهاي دولتي"),
                new ApprovalAuthority("هيات مديره سازمان بورس و اوراق بهادار"),
                new ApprovalAuthority("شوراي عالي فرهنگ"),
                new ApprovalAuthority("دادستان كل كشور"),
                new ApprovalAuthority("مقام معظم رهبري"),
                new ApprovalAuthority("وزيران عضو كميسيون امور زير بنايي،صنعت و محيط زيست"),
                new ApprovalAuthority("ستاد ملي مقابله با كرونا"),
                new ApprovalAuthority("شوراي عالي نظارت بر اتاق ايران"),
                new ApprovalAuthority("سازمان ثبت احوال (معاون وزير كشور و رييس سازمان"),
                new ApprovalAuthority("وزراي عضو شوراي اقتصاد")
            };

        private List<ApprovalStatus> ApprovalStatuses =>
            new List<ApprovalStatus>
            {
                new ApprovalStatus("تمدید"),
                new ApprovalStatus("تنفیذ"),
                new ApprovalStatus("با اجرا منتفی می شود"),
                new ApprovalStatus("منسوخه"),
                new ApprovalStatus("موقت"),
                new ApprovalStatus("آزمایشی"),
                new ApprovalStatus("معتبر"),
            };

        private List<ApprovalType> ApprovalTypes =>
            new List<ApprovalType>
            {
                new ApprovalType("موافقتنامه"),
                new ApprovalType("بخشنامه"),
                new ApprovalType("اساسنامه"),
                new ApprovalType("آیین نامه"),
                new ApprovalType("تصمیم نامه"),
                new ApprovalType("اساسنامه قانونی"),
                new ApprovalType("آیین نامه قانونی"),
                new ApprovalType("مصوبات \"مجلس-مجمع تشخیص مصلحت نظام\""),
                new ApprovalType("عادی"),
                new ApprovalType("مصوبات \"رهبری\""),
                new ApprovalType("اساسی"),
                new ApprovalType("مصوبات \"مجمع تشخیص مصلحت نظام\""),
            };

        private List<ExecutorManagment> ExecutorManagments =>
            new List<ExecutorManagment>
            {
                new ExecutorManagment("سازمان حمايت توليدكنندگان و مصرف كنندگان"),
                new ExecutorManagment("اتاق بازرگاني، صنايع، معادن و كشاورزي ايران"),
                new ExecutorManagment("معاون رئيس جمهور و رئيس سازمان برنامه و بودجه كشور"),
                new ExecutorManagment("نخست وزيري"),
                new ExecutorManagment("كليه مؤسسات دولتي"),
                new ExecutorManagment("معاونت امور مجلس رئيس جمهور"),
                new ExecutorManagment("وزير امور اقتصاد و دارايي"),
                new ExecutorManagment("سازمان اوقاف"),
                new ExecutorManagment("كميته امداد امام خميني"),
                new ExecutorManagment("اتاق تعاون ايران"),
                new ExecutorManagment("مدير عامل بانك شهر")
            };

        private List<LawCategory> LawCategories =>
            new List<LawCategory>
            {
                new LawCategory("وظايف هيات وزيران و دولت"),
                new LawCategory("وظايف فرماندهي كل قوا"),
                new LawCategory("وظايف مجلس خبرگان"),
                new LawCategory("وظايف مقام معظم رهبري"),
                new LawCategory("وظايف قوه قضاييه"),
                new LawCategory("وظايف امور خارجه"),
                new LawCategory("وظايف بازرگاني"),
                new LawCategory("وظايف سازمان مديريت و برنامه ريزي"),
                new LawCategory("وظايف وزارت جهاد كشاورزي"),
                new LawCategory("وظايف صنايع و معادن"),
                new LawCategory("وظايف سازمان ميراث فرهنگي و گردشگري"),
                new LawCategory("وظايف وزارت رفاه و تامين اجتماعي"),
                new LawCategory("وظايف تعاون ، كار و رفاه اجتماعي"),
                new LawCategory("وظايف صنعت ، معدن و تجارت"),
                new LawCategory("تحولات مراجع وضع"),
            };

        private List<Law> Laws =>
            new List<Law>
            {
                new Law("آیین نامه اجرایی قانون اصلاح قانون حفظ کاربری اراضی زراعی و باغها" , new Announcement("44545" , DateTime.Now.AddDays(-20)), new Newspaper("58653" , DateTime.Now.AddDays(-53)) ,
                        lorem , DateTime.Now.AddDays(-5) , LawType.Regulation , true , Guid.Empty , Guid.Empty , Guid.Empty , Guid.Empty ,Guid.Empty),
                new Law("اختصاص اعتبار به وزارت نيرو به منظور تثبيت و ايمن سازي آبراهه هاي مشرف به محدوده سد امير كبير و لايروبي و رسوب برداري رودخانه پايين دست و جبران خسارات ناشي از بارندگي شديد و سيلاب مورخ 1402/03/18 در محور كرج - چالوس حد فاصل تونل هاي (2- الف) و (2-ب)" , new Announcement("44545" , DateTime.Now.AddDays(-27)), new Newspaper("58653" , DateTime.Now.AddDays(-43)) ,
                        lorem , DateTime.Now.AddDays(-23) , LawType.Rule , true ,  Guid.Empty , Guid.Empty , Guid.Empty , Guid.Empty ,Guid.Empty),
                new Law(" آيين نامه شناسايي و صيانت از وسيله هاي نقليه تاريخي" , new Announcement("44545" , DateTime.Now.AddDays(-73)), new Newspaper("58653" , DateTime.Now.AddYears(-86)) ,
                        lorem , DateTime.Now.AddMonths(-2) , LawType.Regulation , true ,  Guid.Empty , Guid.Empty , Guid.Empty , Guid.Empty ,Guid.Empty),
                new Law("همتراز شدن دبيركل كميسيون ملي يونسكو با مقامات موضوع بند (هـ) ماده (71) قانون مديريت خدمات كشوري" , new Announcement("44545" , DateTime.Now.AddDays(-55)), new Newspaper("58653" , DateTime.Now.AddDays(-120)) ,
                        lorem , DateTime.Now.AddDays(-125) , LawType.Rule , true , Guid.Empty , Guid.Empty , Guid.Empty , Guid.Empty ,Guid.Empty),
                new Law("تخصيص اعتبار به مبلغ سيصد ميليارد (300/000/000/000) ريال براي تامين مواد مصرفي آزمايشگاهي به منظور خريد تجهيزات براي شناسايي و تشخيص هويت متوفيان ناشي از وقوع حوادث غيرمترقبه در اختيار سازمان پزشكي قانوني كشور" , new Announcement("44545" , DateTime.Now.AddDays(-2)), new Newspaper("58653" , DateTime.Now.AddDays(-155)) ,
                        lorem , DateTime.Now.AddDays(-50) , LawType.Rule , true ,  Guid.Empty , Guid.Empty , Guid.Empty , Guid.Empty ,Guid.Empty),
                new Law(" \tتعيين صفر درصد سود بازرگاني قطعات گوشت مرغ ا" , new Announcement("44545" , DateTime.Now.AddDays(-23)), new Newspaper("58653" , DateTime.Now.AddDays(-17)) ,
                        lorem , DateTime.Now.AddDays(-20) , LawType.Regulation , true , Guid.Empty , Guid.Empty , Guid.Empty , Guid.Empty ,Guid.Empty),
            };
        #endregion

        #region Multimedia

        public List<GalleryPhoto> Images =>
            new List<GalleryPhoto>()
            {
                new GalleryPhoto("1.jpg", new Random().Next(0,5) , Guid.Empty),
                new GalleryPhoto("2.jpg", new Random().Next(0,5) , Guid.Empty),
                new GalleryPhoto("3.jpg", new Random().Next(0,5) , Guid.Empty),
                new GalleryPhoto("4.jpg", new Random().Next(0,5) , Guid.Empty),
                new GalleryPhoto("5.jpg", new Random().Next(0,5) , Guid.Empty),
            };

        #endregion

        #region News
        public List<NewsCategory> NewsCategories =>
            new List<NewsCategory>
            {
                new NewsCategory("سیاسی" , null),
                new NewsCategory("اقتصادی" , null),
                new NewsCategory("اجتماعی" , null),
            };

        public News News =
            new News(
                "عنوان خبر",
                 description: "<p>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون\r\n                بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع\r\n                با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه\r\n                و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و\r\n                فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها،\r\n                و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل\r\n                دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.\r\n            </p>\r\n            <h3 class=\"mt-30\">\r\n                لورم ایپسوم متن ساختگی با تولید\r\n            </h3>\r\n            <p class=\"mt-10\">لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است،\r\n                چاپگرها و متون\r\n                بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع\r\n                با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه\r\n                و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و\r\n                فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها،\r\n                و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل\r\n                دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.\r\n            </p>\r\n            <img src=\"/banner.jpg\" class=\"mt-30\" />\r\n            <p class=\"mt-30\">لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است،\r\n                چاپگرها و متون\r\n                بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع\r\n                با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه\r\n                و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و\r\n                فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها،\r\n                و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل\r\n                دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.\r\n            </p>\r\n            <p class=\"mt-20\">لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است،\r\n                چاپگرها و متون\r\n                بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع\r\n                با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه\r\n                و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و\r\n                فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها،\r\n                و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل\r\n                دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.\r\n            </p>",
                 headline: "لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها، و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.",
                 "mehrnews.com/x33LcG", DateTime.Now, Guid.Empty, 0);
        #endregion

        #region Contact
        public FrequentlyAskedQuestions FrequentlyAskedQuestions =>
            new FrequentlyAskedQuestions("لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ؟", "<p>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها، و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد. لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ.</p>");

        public Guide Guide =>
            new Guide("لورم ایپسوم متن ساختگی با تولید", "<style>.h_iframe-aparat_embed_frame{position:relative;}.h_iframe-aparat_embed_frame .ratio{display:block;width:100%;height:auto;}.h_iframe-aparat_embed_frame iframe{position:absolute;top:0;left:0;width:100%;height:100%;}</style><div class=\"h_iframe-aparat_embed_frame\"><span style=\"display: block;padding-top: 57%\"></span><iframe src=\"https://www.aparat.com/video/video/embed/videohash/Dqev1/vt/frame\"  allowFullScreen=\"true\" webkitallowfullscreen=\"true\" mozallowfullscreen=\"true\"></iframe></div>\r\n<p class='mt-40'>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی.</p>\r\n<h3 class='font-dana mt-40'>لورم ایپسوم متن ساختگی با تولید</h3>\r\n<p class='mt-20'>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها، و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.</p>\r\n<img class='mt-40' src='/banner.jpg'/>\r\n<p class='mt-20'>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود</p>\r\n", true);


        public List<RelatedCompany> RelatedCompanies =>
            new List<RelatedCompany>
        {
            new RelatedCompany( "وزارت کشور" , "1.png",0),
            new RelatedCompany( "سازمان امور اراضی" , "2.png",0),
            new RelatedCompany( "سازمان شهرداری ها و دهیاری های کشور" , "3.png",0),
            new RelatedCompany( "وزارت صنعت، معدن و تجارت" , "4.png",0),
            new RelatedCompany( "شرکت سهامی مادر تخصصی مدیریت منابع آب ایران" , "5.png",0),
            new RelatedCompany( "شرکت ملی پست جمهوری اسلامی ایران" , "6.png",0),
            new RelatedCompany( "وزارت میراث فرهنگی، صنایع دستی و گردشگری" , "7.png",0),
            new RelatedCompany( "سازمان اوقاف و امور خیریه" , "8.png",0),
            new RelatedCompany( "سازمان جنگلها مراتع و آبخیزداری کشور" , "9.png",1),
            new RelatedCompany( "سازمان فضایی ایران" , "10.png",0),
            new RelatedCompany( "سازمان حفاظت محیط زیست" , "11.png",0),
            new RelatedCompany( "دبیرخانه شورای عالی معماری و شهرسازی" , "12.png",0),
            new RelatedCompany( "سازمان تنظیم مقررات و ارتباطات رادیویی" , "13.png",0),
            new RelatedCompany( "سازمان اداری و استخدامی کشور" , "14.png",0),
            new RelatedCompany( "سازمان ملی زمین و مسکن" , "15.png",0),
            new RelatedCompany( "شرکت ملی مخابرات ایران" , "16.png",0),
            new RelatedCompany( "شرکت اتباطات و زیرساخت" , "17.png",0),
            new RelatedCompany( "وزارت ارتباطات و فناوری اطلاعات" , "18.png",0),
            new RelatedCompany( "وزارت نفت" , "19.png",0),
            new RelatedCompany( "سازمان ثبت احوال کشور" , "20.png",0),
            new RelatedCompany( "شرکت راه‌آهن جمهوری اسلامی ایران" , "21.png",0),
            new RelatedCompany( "شرکت سهامی مادرتخصصی مهندسی آب و فاضلاب کشور" , "22.png",0),
            new RelatedCompany( "شرکت فرودگاه‌ها و ناوبری هوایی ایران" , "23.png",0),
            new RelatedCompany( "سازمان جغرافیایی نیروهای مسلح" , "24.png",0),
            new RelatedCompany( "سازمان ثبت اسناد و املاک کشور" , "25.png",0),
            new RelatedCompany( "شرکت مادر تخصصی عمران شهرهای جدید" , "26.png",0),
            new RelatedCompany( "سازمان برنامه و بودجه کشور" , "27.png",0),
            new RelatedCompany( "سازمان صنایع کوچک و شهرکهای صنعتی ایران" , "28.png",0),
            new RelatedCompany( "بنیاد مسکن انقلاب اسلامی" , "29.png",0),
            new RelatedCompany( "سازمان بنادر و دریانوردی" , "30.png",0),
            new RelatedCompany( "شرکت سهامی مادر تخصصی تولید، انتقال و توزیع نیروی برق ایران" , "31.png",0),
            new RelatedCompany( "سازمان نقشه برداری کشور" , "32.png",0),
            new RelatedCompany( "وزارت جهاد کشاورزی" , "33.png",0),
            new RelatedCompany( "سازمان فناوری اطلاعات ایران" , "34.png",1),
            new RelatedCompany( "وزارت راه و شهرسازی" , "35.png",0),
            new RelatedCompany( "سازمان راهداری و حمل و نقل جاده ای" , "36.png",0),
            new RelatedCompany( "دبیرخانه ستاد مبارزه با مفاسد اقتصادی" , "37.png",1),
            };

        #endregion

        #region Resources
        public List<Author> Authors =>
            new List<Author>
            {
                new Author("دوئیس اِ. داندیس")
            };
        #endregion

        private String lorem = "لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است. چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد. کتابهای زیادی در شصت و سه درصد گذشته، حال و آینده شناخت فراوان جامعه و متخصصان را می طلبد تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی و فرهنگ پیشرو در زبان فارسی ایجاد کرد. در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها و شرایط سخت تایپ به پایان رسد وزمان مورد نیاز شامل حروفچینی دستاوردهای اصلی و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.";
        private String video = "<div class=\"h_iframe-aparat_embed_frame\"><span style=\"display: block;padding-top: 57%\"></span><iframe\r\n                            src=\"https://www.aparat.com/video/video/embed/videohash/jq0lh/vt/frame\" allowFullScreen=\"true\"\r\n                            webkitallowfullscreen=\"true\" mozallowfullscreen=\"true\"></iframe></div>";
    }
}
