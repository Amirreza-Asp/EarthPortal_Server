using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Utilities;
using Domain;
using Domain.Dtos.Resources;
using Domain.Entities.Account;
using Domain.Entities.Contact;
using Domain.Entities.Contact.Enums;
using Domain.Entities.Mutimedia;
using Domain.Entities.Notices;
using Domain.Entities.Pages;
using Domain.Entities.Regulation;
using Domain.Entities.Regulation.Enums;
using Domain.Entities.Regulation.ValueObjects;
using Domain.Entities.Resources;
using Domain.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.Json;

namespace Persistence.Utilities
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly Random rnd;
        private readonly IPasswordManager _passManager;
        private readonly IHostingEnvironment _env;
        private readonly IPhotoManager _photoManager;
        private readonly IFileManager _fileManager;
        private readonly IMemoryCache _memoryCache;

        public DbInitializer(ApplicationDbContext context, IPasswordManager passManager, IHostingEnvironment env, IPhotoManager photoManager, IFileManager fileManager, IMemoryCache memoryCache)
        {
            _context = context;
            rnd = new Random();
            _passManager = passManager;
            _env = env;
            _photoManager = photoManager;
            _fileManager = fileManager;
            _memoryCache = memoryCache;
        }

        public async Task Execute()
        {
            //await _context.Database.EnsureDeletedAsync();

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

            var lawJsonData = File.ReadAllText(_env.WebRootPath + "/regulation/file/law.json");
            var lawData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LawData>>(lawJsonData);


            if (!_context.ApprovalAuthority.Any())
            {
                var approvalAuthorities =
                    lawData.Select(b => b.Reference)
                    .Where(b => !String.IsNullOrEmpty(b))
                    .Distinct()
                    .Select(b => new ApprovalAuthority(b))
                    .ToList();

                _context.ApprovalAuthority.AddRange(approvalAuthorities);
                _context.ApprovalAuthority.Add(new ApprovalAuthority("نامشخص"));
                await _context.SaveChangesAsync();
            }

            if (!_context.ApprovalStatus.Any())
            {
                _context.ApprovalStatus.AddRange(ApprovalStatuses);
                _context.ApprovalStatus.Add(new ApprovalStatus("نامشخص"));
                await _context.SaveChangesAsync();
            }

            if (!_context.ApprovalType.Any())
            {
                var approvalTypes =
                    lawData.Select(b => b.Type)
                    .Where(b => !String.IsNullOrEmpty(b))
                    .Distinct()
                    .Select(b => new ApprovalType(b))
                    .ToList();

                _context.ApprovalType.AddRange(approvalTypes);
                _context.ApprovalType.Add(new ApprovalType("نامشخص"));
                await _context.SaveChangesAsync();
            }

            if (!_context.ExecutorManagment.Any())
            {
                var executorManagements =
                    lawData.Select(b => b.Presenter)
                    .Where(b => !String.IsNullOrEmpty(b))
                    .Distinct()
                    .Select(b => new ExecutorManagment(b))
                    .ToList();

                _context.ExecutorManagment.AddRange(executorManagements);
                _context.ExecutorManagment.Add(new ExecutorManagment("نامشخص"));
                await _context.SaveChangesAsync();
            }

            if (!_context.LawCategory.Any())
            {
                _context.LawCategory.AddRange(LawCategories);
                _context.LawCategory.Add(new LawCategory("نامشخص"));
                await _context.SaveChangesAsync();
            }

            if (!_context.Law.Any())
            {
                var categories = await _context.LawCategory.ToListAsync();
                var approvalAuthority = await _context.ApprovalAuthority.ToListAsync();
                var approvalStatus = await _context.ApprovalStatus.ToListAsync();
                var approvalType = await _context.ApprovalType.ToListAsync();
                var executorManagment = await _context.ExecutorManagment.ToListAsync();

                try
                {
                    var pc = new PersianCalendar();
                    var laws = new List<Law>();


                    foreach (var b in lawData.DistinctBy(b => b.Title).Where(b => !String.IsNullOrEmpty(b.Date) && !String.IsNullOrEmpty(b.CommunicatedDate)))
                    {
                        try
                        {
                            var entity =
                                 new Law(
                                     title: b.Title,
                                     announcement: new Announcement(b.CommunicatedNumber, DateTimeExtension.ConvertShamsiStringToMiladiDateTime(b.CommunicatedDate)),
                                     newspaper: new Newspaper(b.NewspaperNumber, DateTimeExtension.ConvertShamsiStringToMiladiDateTime(b.NewspaperDate)),
                                     description: b.LawText,
                                     approvalDate: DateTimeExtension.ConvertShamsiStringToMiladiDateTime(b.Date),
                                     type: b.Type == "آیین‌نامه" ? LawType.Rule : LawType.Regulation,
                                     isOriginal: true,
                                     approvalTypeId: String.IsNullOrEmpty(b.Type) ? approvalType.First(s => s.Value == "نامشخص").Id : approvalType.First(s => s.Value == b.Type).Id,
                                     approvalStatusId: approvalStatus.First(s => s.Status == "نامشخص").Id,
                                     executorManagmentId: String.IsNullOrEmpty(b.Presenter) ? executorManagment.First(s => s.Name == "نامشخص").Id : executorManagment.First(s => s.Name == b.Presenter).Id,
                                     approvalAuthorityId: String.IsNullOrEmpty(b.Reference) ? approvalAuthority.First(s => s.Name == "نامشخص").Id : approvalAuthority.First(s => s.Name == b.Reference).Id,
                                     lawCategoryId: categories.First(s => s.Title == "نامشخص").Id,
                                     pdf: b.LawsId + ".pdf"
                                 );

                            laws.Add(entity);

                            _context.Law.Add(entity);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }

                    if (await _context.SaveChangesAsync() > 0)
                    {
                        if (!Directory.Exists(_env.WebRootPath + SD.LawPdfPath))
                            Directory.CreateDirectory(_env.WebRootPath + SD.LawPdfPath);


                        foreach (var item in laws)
                        {
                            try
                            {
                                if (!File.Exists(_env.WebRootPath + SD.LawPdfPath + item.Pdf))
                                    _fileManager.ConvertHtmlToPdf(item.Description, _env.WebRootPath + SD.LawPdfPath + item.Pdf);
                            }
                            catch (Exception ex)
                            {
                                _context.Law.Remove(item);
                            }
                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
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
                for (int i = 1; i < 2; i++)
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

                var category = new NewsCategory("نامشخص", null);
                _context.NewsCategory.Add(category);
                await _context.SaveChangesAsync();
            }


            if (!_context.News.Any())
            {
                var path = _env.WebRootPath + "/notices/news.json";

                var jsonData = await File.ReadAllTextAsync(path);
                var data = System.Text.Json.JsonSerializer.Deserialize<List<NewsData>>(jsonData);

                var categoryId = _context.NewsCategory.Where(b => b.Title == "نامشخص").Select(b => b.Id).First();

                foreach (var item in data)
                {

                    while (true)
                    {
                        var shortLink = rnd.Next(Convert.ToInt32(Math.Pow(10, 7)), Convert.ToInt32(Math.Pow(10, 8)));

                        if (!_context.News.Where(b => b.ShortLink == shortLink).Any())
                        {
                            var news = new News(
                                item.title,
                                item.description,
                                item.newsText,
                                source: "#",
                                item.newsDateO,
                                categoryId,
                                shortLink);

                            _context.News.Add(news);

                            var imageName = item._id + Path.GetExtension(item.newsImage.name);

                            var upload = _env.WebRootPath + SD.NewsImagePath + imageName;

                            if (!Directory.Exists(_env.WebRootPath + SD.NewsImagePath))
                                Directory.CreateDirectory(_env.WebRootPath + SD.NewsImagePath);

                            var image = new NewsImage(imageName, news.Id, 0);
                            _context.NewsImage.Add(image);
                            if (!File.Exists(upload))
                            {

                                await _photoManager.SaveFromBase64Async(item.newsImage.value, upload);

                            }

                            break;
                        }
                    }
                }

                _context.SaveChanges();
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
                for (int i = 0; i < 0; i++)
                {
                    var guide = Guide;
                    guide.IsPort = i < 5;
                    _context.Guide.Add(guide);
                }
                await _context.SaveChangesAsync();
            }

            if (!_context.Info.Any())
            {
                var info = new Info("1649", "iraneland@ito.gov.ir", "#", "#", "#", "#", "#", "#");
                _context.Info.Add(info);
                await _context.SaveChangesAsync();
            }

            if (!_context.GeoAddress.Any())
            {
                var infoId = await _context.Info.Select(b => b.Id).FirstOrDefaultAsync();
                var geoAddress1 = new GeoAddress(35.690732000445955, 51.38562655309216, $"تهران ، خیابان نواب صفوی ، کوچه شهید صفوی ، ساختمان 2", infoId, "آدرس 1");
                var geoAddress2 = new GeoAddress(35.680832000445955, 51.37562655309216, $"تهران ، خیابان نواب صفوی ، کوچه شهید صفوی ، ساختمان 2", infoId, "آدرس 2");

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
                items.ForEach(item => { item.Image = $"comp{item.Image}"; item.Link = "#"; });
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
                    var se = new SystemEvaluation(rnd.Next(1, 6), rnd.Next(1, 5000000).ToString());
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

            var rLinks = _context.RelatedLink.ToList();
            _context.RelatedLink.RemoveRange(rLinks);
            _context.SaveChanges();

            if (!_context.RelatedLink.Any())
            {
                _context.RelatedLink.AddRange(RelatedLinks);
                await _context.SaveChangesAsync();
            }
            #endregion

            #region Account
            if (!_context.Role.Any())
            {
                var role = new Role("Admin", "ادمین", "ادمین کل سیستم");
                var role2 = new Role("Publisher", "ناشر", "ناشر");
                _context.Role.Add(role);
                _context.Role.Add(role2);
                await _context.SaveChangesAsync();
            }


            if (!_context.User.Any())
            {
                var role = await _context.Role.FirstOrDefaultAsync(b => b.Title == "Admin");
                var user = new User(role.Id, "امیررضا", "محمدی", "Admin", _passManager.HashPassword("Admin2002"), "amirrezamohammadi8102@gmail.com", "09211573936");

                _context.User.Add(user);


                var motorchi = new User(role.Id, "", "موتورچی", "Motorchi", _passManager.HashPassword("Motorchi1234"), null, null);
                var keshavarz = new User(role.Id, "علی", "کشاورز", "Keshavarz", _passManager.HashPassword("Keshavarz1234"), null, null);

                _context.User.Add(motorchi);
                _context.User.Add(keshavarz);

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

            #region Pages
            if (!_context.HomePage.Any())
            {
                var homePage = new HomePage()
                {
                    Header =
                        new HomeHeader
                        {
                            AppBtnEnable = true,
                            PortBtnEnable = true,
                            Title = "سامانه پنجره واحد مدیریت زمین",
                            Content = "لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان. "
                        },
                    Work =
                        new HomeWork
                        {
                            Title = "کار ما چیست؟",
                            Content = "سامانه پنجره واحد مدیریت زمین به عنوان یکی از 23 پروژه اولویت‌دار دولت الکترونیک و با دو هدف کلی پیشگیری از مفاسد زمین‌خواری و ساماندهی نحوه ارائه خدمات حوزه زمین و ساختمان راه‌اندازی شده است. ",
                            App = "همچنین یکی از مهم‌ترین اهداف ترسیم شده پروژه، در میان‌مدت شامل تسهیل و ساماندهی خدمات حوزه زمین با ابزارهای دولت الکترونیک و در بلندمدت شامل هدایت مناسب سرمایه‌گذاران و متقاضیان به بهره‌برداری از اراضی کشور بر مبنای ضوابط و اصول آمایش سرزمین می‌گردد. تحقق زیرساخت‌های لازم جهت جاری‌سازی اصول آمایش سرزمین بر بهره‌برداری اراضی کشور به عنوان مهمترین افق توسعه پایدار و بهره‌برداری کارآمد از اراضی کشور محسوب می‌گردد که با ابزارهای مکان‌محور و مدیریت سیستمی شاخص‌های آمایشی فراهم می‌گردد. ",
                            Port = "این سامانه از یک‌سو قابلیت شناسایی تخلفات زمین‌خواری و ساخت‌وسازهای غیرمجاز در اراضی حساس کشور را با استفاده از تصاویر ماهواره‌ای فراهم نموده که به عنوان بستری برای معرفی موارد زمین‌خواری و نظارت بر عملکرد دستگاه‌های اجرایی ذی‌ربط قابل استفاده می‌باشد. از سوی دیگر سامانه با فراهم نمودن زیرساخت‌های الکترونیکی مدیریت هوشمندانه و سیستمی فرایندهای بین دستگاهی جهت ارائه مجوزها و استعلامات حوزه زمین و ساختمان، سازوکارهای مناسبی را جهت پیشگیری از بروز مفاسد و مشکلاتی نظیر جعل استعلامات و مجوزها ، تبانی و ارتشا توسط زمین‌خواران، تداخل وظایف بین‌دستگاهی و عدم شفافیت در تصمیم‌گیری‌های کمیسیون‌ها و شوراها فراهمی‌نماید."
                        }

                };

                _context.HomePage.Add(homePage);
                await _context.SaveChangesAsync();
            }

            if (!_context.AboutUsPage.Any())
            {
                var aboutUsPage = new AboutUsPage(
                    " سامانه پنجره واحد مدیریت زمین به عنوان یکی از 23 پروژه اولویت‌دار دولت الکترونیک و با دو هدف کلی پیشگیری از مفاسد زمین‌خواری و ساماندهی نحوه ارائه خدمات حوزه زمین و ساختمان راه‌اندازی شده است.",
                    "لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع.",
                    "برای مردم ایران زمین");

                _context.AboutUsPage.Add(aboutUsPage);

                await _context.SaveChangesAsync();
            }

            if (!_context.LawPage.Any())
            {
                var lawPage = new LawPage(
                    "متقاضی گرامی!",
                    "خدمات حوزه اراضی و املاک، یکی از حوزه‌هایی است که از نظر قوانین و مقررات دارای پیچیدگی بالایی می‌باشد. در این سامانه بخشی از این قوانین و مقررات جهت اطلاع و مطالعه عمومی جمع‌آوری و ارائه گردیده است ."); ;

                _context.LawPage.Add(lawPage);

                await _context.SaveChangesAsync();
            }

            if (!_context.EnglishPage.Any())
            {
                var englishPage = new EnglishPage
                {
                    Id = Guid.Parse("62CF6284-C415-4009-93EF-2B79EE90A113"),
                    Intro = new EnglishIntro
                    {
                        Title = "Intro",
                        Content = "IranEland is asingle-window platform equipped with emerging technologies, ArtificialIntelligence (AI) in particular, to ensure the legal use of land, providecitizens with high-quality service and information on land and constructionpermits and support the government on the best practices to achieve sustainableland management and protection. IranEland has provided a software environmentfor the electronic delivery of land and building permits, allowing citizens tosubmit and track their requests without visiting multiple organizations. " +
                         "<br/><br/><br/>Byutilizing high-quality images obtained from the Iranian satellite Khayyam, thissystem prevents any unauthorized changes in land use. Additionally, it providescontinuous monitoring of land status for the government, leading to moreprecise management and planning across various sectors in the country.IranEland wasestablished in 2020 by the ITO Information Technology Organization of Iran andoperated officially at the beginning of 2022. IranEland also pursuing the ideaof “Government as a Platform” compatible with iternational frameworks andstandards to enable offering all the land-related services in one place. ",
                    },
                    MainIdea = new EnglishMainIdea
                    {
                        Title = "Main Idea",
                        Content1 = "IranEland intelligentlyprocesses high-quality images from the Iranian Khayam satellite via AI tools,and shares the information and services between government organizations. As aresult, the problem of land grabbing is solved, and the possibility ofobtaining land permits quickly and cheaply is provided, leading to greatersatisfaction among the public. ",
                        Bold = "Common Ground & Problems To Solve",
                        Content2 = "Citizens, especially realinvestors, face complex, time-consuming, and sometimes opaque processes to obtain land usage permits. On the other hand, individuals engage in corruptpractices within these processes, contributing to land grabbing. It is nowpossible to intelligently monitor changes in the land through high-qualityimages from the Iranian Khayyam satellite and by sharing information andservices from various relevant organizations. These challenges can be addressedby creating an intelligent land management single window. "
                    },

                    CurrentSituation = new EnglishCurrentSituation
                    {
                        Title = "Current Situation",
                        Content = " By utilizing the IranEland system, the expenses of citizens' visits andpaper usage are significantly reduced. Currently, the system receives anaverage of 1,000 acceptance requests per day in its first year of operation,with each request requiring an average of 10 inquiries.</br></br>Considering the average cost of commuting, paperwork, and ancillaryexpenses for responding to an inquiry to be around $25, IranEland has costsreduced by about $250,000 and prevented the cutting down of 5 large treesdaily, based on the average paper required for files and inquiry responses.With the widespread use of the system across the country and the increase inland-based services, these numbers can increase up to 20 times the currentcapacity. It means that IranEland has the potential to save more than 1.8billion dollars and to prevent the cutting down of more than 1800 large treesper year.</br></br>In addition, an average of 200hectares of land are identified as suspicious environmental destruction casesdaily, and this is only a small part of Iran's land that is monitoredcurrently. These statistics only highlight a few specific aspects of thesystem, and its capacity for positive impact is significant and evident ineconomic, social (significantly increasing social justice following decisiontransparency), and environmental fields. ",
                        Image = "currentSituation.png"
                    },
                    Vision = new EnglishVision
                    {
                        Title = "Vision",
                        Content = "This particular project can be regarded as the most extensivesingle-window service project in the country regarding the number ofresponsible organizations (over 45) and associated services and processes (over200). The system's goal is to monitor the entire land of Iran intelligently.This requires the use of fast and intelligent processing algorithms. Inconclusion, this system is highly complex in technical, operational, andmanagement aspects. This level of complexity is rare among other methods. Fortunately,these issues are controlled to some extent, and the project is on track toultimate success. These features make IranEland a single window for thecountry's land management, which means it is a continuous project."
                    },
                    Cards = new List<EnglishCard>
                    {
                        #region Beggining
                        new EnglishCard
                        {
                            Id = Guid.Parse("564FCE78-CC0E-41CB-9E6F-7BC90E5061BB"),
                            Type = EnglishCardType.Beginning,
                            Color = EnglishCardColor.Yellow,
                            Title = "PROOF",
                            Line = true,
                            Order = 0,
                            SiblingId = Guid.Parse("564FCE78-CC0E-41CB-9E6F-7BC90E5061BC"),
                            Content =  "1. In many countries the landgrabbing is a big nationalproblem.</br></br>2. Citizens, especially realinvestors, face complex,time-consuming, andsometimes opaque processesto obtain land usage permits.</br></br>3. Individuals engage incorrupt practices withinthese processes, contributingto land grabbing."
                        },
                        new EnglishCard
                        {
                            Id = Guid.Parse("564FCE78-CC0E-41CB-9E6F-7BC90E5061BC"),
                            Type = EnglishCardType.Beginning,
                            Color = EnglishCardColor.Yellow,
                            Title = "PROOF",
                            Order = 1,
                            SiblingId = Guid.Parse("564FCE78-CC0E-41CB-9E6F-7BC90E5061BB"),
                            Content =  "1. In Iran, an average of 500 casesof land speculation are detectedon national lands monthly, andthe actual statistics are likelyhigher.</br></br>2. It is now possible to intelligentlymonitor changes in the landthrough high-quality imagesfrom satellite.</br></br>3. Now, depending on its type,obtaining a land usage permitinvolves numerous inquiriesfrom various executing bodies,taking an average of 6 months."
                        },

                        new EnglishCard
                        {
                            Id = Guid.NewGuid(),
                            Type = EnglishCardType.Beginning,
                            Color = EnglishCardColor.Blue,
                            Title = "IDEA",
                            Line = true,
                            Order = 2,
                            Content=  "It is possible to build a system intelligentlyprocesses high-quality images from the IranianKhayam satellite, and shares the informationand services between governmentorganizations. We named it as IranEland.</br></br>As a result, the problem of land grabbing issolved, and the possibility of obtaining landpermits quickly and cheaply is provided,leading to greater satisfaction among thepublic."
                        },
                        #endregion

                        #region Middle
                        new EnglishCard
                        {
                            Id = Guid.Parse("664FCE78-CC0E-41CB-9E6F-7BC90E5061BB"),
                            Type = EnglishCardType.Middle,
                            Color = EnglishCardColor.Red,
                            Title = "PROBLEMS",
                            Line = true,
                            Order = 0,
                            SiblingId = Guid.Parse("664FCE78-CC0E-41CB-9E6F-7BC90E5061BC"),
                            Content =  "1. Artificial intelligence techniques,especially in image processing,may exhibit unacceptableaccuracy.</br></br>2. The responsible entities'information, processes, andservices must be uniform andintegrated, especially maps andinformational layers."
                        },
                        new EnglishCard
                        {
                            Id = Guid.Parse("664FCE78-CC0E-41CB-9E6F-7BC90E5061BC"),
                            Type = EnglishCardType.Middle,
                            Color = EnglishCardColor.Yellow,
                            Title = "PROOF",
                            Order = 1,
                            SiblingId = Guid.Parse("664FCE78-CC0E-41CB-9E6F-7BC90E5061BB"),
                            Content =  "1. The high volume ofdistributed data in thecountry need to be managedfor land management. Forexample, maps of roadboundaries, national landsand cultural heritage, orinformation on existingpermits and geographicdivisions are just some cases"
                        },

                        new EnglishCard
                        {
                            Id = Guid.NewGuid(),
                            Type = EnglishCardType.Middle,
                            Color = EnglishCardColor.Yellow,
                            Title = "PROOF",
                            Line = true,
                            Order = 2,
                            Content=  "1. Recently, artificial intelligence has grown rapidly and found manyapplications.</br></br>2. Now it is possible to receive high-quality images from Iranian satellite,Khayyam, quickly, continuously and cheaply.</br></br>3. The trial use of the system during 6 months led to the identification of17,000 hectares of illegal land changes. Investigations and local visitsshowed that more than 60% of these identifications were correct."
                        },

                        new EnglishCard
                        {
                            Id = Guid.Parse("a64FCE78-CC0E-41CB-9E6F-7BC90E5061BB"),
                            Type = EnglishCardType.Middle,
                            Color = EnglishCardColor.Green,
                            Title = "SOLUTION",
                            Order = 3,
                            SiblingId = Guid.Parse("a64FCE78-CC0E-41CB-9E6F-7BC90E5061BC"),
                            Content =  "1. Cutting-edge technologies likedeep learning is utilized.</br></br>2. Data exchange interfaces havebeen established, enablingsystems to interact with theunified land managementportal while maintaining theirindependence."
                        },
                        new EnglishCard
                        {
                            Id = Guid.Parse("a64FCE78-CC0E-41CB-9E6F-7BC90E5061BC"),
                            Type = EnglishCardType.Middle,
                            Color = EnglishCardColor.Yellow,
                            Title = "COMMON GROUND",
                            Order = 4,
                            SiblingId = Guid.Parse("a64FCE78-CC0E-41CB-9E6F-7BC90E5061BB"),
                            Content =  "1. The accuracy of artificialintelligence algorithms increasesover time, which makes themreliable and prevents thenegative influence of people inthe process of combating landgrabbing.</br></br>2. Coordination between variousorganizations and systems thatare active in the land issuerequires standardization andalignment between them."
                        },

                        new EnglishCard
                        {
                            Id = Guid.Parse("b64FCE78-CC0E-41CB-9E6F-7BC90E5061BB"),
                            Type = EnglishCardType.Middle,
                            Color = EnglishCardColor.Red,
                            Title = "PROBLEMS",
                            Line = true,
                            Order = 5,
                            SiblingId = Guid.Parse("b64FCE78-CC0E-41CB-9E6F-7BC90E5061BC"),
                            Content =  "1. Even, we solve the dataexchange among informationsystems, The existence ofnumerous stakeholders andbeneficiaries in the countryposes a significant challenge dueto their need for coordinationand alignment.</br></br>2. People may need help to be ableto use the system conveniently.</br></br>3. After identifying the illegal landchanges, it is necessary to makethe investigation process,detection of violation and issuingof the judicial orderelectronically."
                        },
                        new EnglishCard
                        {
                            Id = Guid.Parse("b64FCE78-CC0E-41CB-9E6F-7BC90E5061BC"),
                            Type = EnglishCardType.Middle,
                            Color = EnglishCardColor.Yellow,
                            Title = "PROOF",
                            Order = 6,
                            SiblingId = Guid.Parse("b64FCE78-CC0E-41CB-9E6F-7BC90E5061BB"),
                            Content =  "1. In Iran, more than 40government and publicorganizations are involved inland issues.</br></br>2. All citizens, especiallyinvestors and farmers, areusers of the system. It isclear that the simplicity ofworking with the system isvery key and necessary."
                        },

                        new EnglishCard
                        {
                            Id = Guid.NewGuid(),
                            Type = EnglishCardType.Middle,
                            Color = EnglishCardColor.Yellow,
                            Title = "PROOF",
                            Line = true,
                            Order = 7,
                            Content =  "1. Currently, 8 main services of the government in the field of land areprovided within IranEland single window, in which more than 500000users have registered so far."
                        },

                        new EnglishCard
                        {
                            Id = Guid.Parse("c64FCE78-CC0E-41CB-9E6F-7BC90E5061BB"),
                            Type = EnglishCardType.Middle,
                            Color = EnglishCardColor.Green,
                            Title = "SOLUTION",
                            Order = 8,
                            SiblingId = Guid.Parse("c64FCE78-CC0E-41CB-9E6F-7BC90E5061BC"),
                            Content =  "1. Clear and enforceable lawsand regulations should beformulated.</br></br>2. The system has a user-friendlyinterface, and also allowingpeople to seek assistance fromknowledgeable agentselectronically"
                        },
                        new EnglishCard
                        {
                            Id = Guid.Parse("c64FCE78-CC0E-41CB-9E6F-7BC90E5061BC"),
                            Type = EnglishCardType.Middle,
                            Color = EnglishCardColor.Yellow,
                            Title = "COMMON GROUND",
                            Order = 9,
                            SiblingId = Guid.Parse("c64FCE78-CC0E-41CB-9E6F-7BC90E5061BB"),
                            Content =  "1. For the success of IranEland, themost important concern iscoordination at all technical,expert and managerial levels. Itsrequirement is to formulatesimple and standard regulationsand follow them.</br></br>2. Simple access to the system andreceiving all services from onepoint, along with the legalrequirement to use it, is animportant principle in thesuccess of the system."
                        },

                        new EnglishCard
                        {
                            Id = Guid.Parse("d64FCE78-CC0E-41CB-9E6F-7BC90E5061BB"),
                            Type = EnglishCardType.Middle,
                            Color = EnglishCardColor.Red,
                            Line = true,
                            Title = "PROBLEMS",
                            Order = 10,
                            SiblingId = Guid.Parse("d64FCE78-CC0E-41CB-9E6F-7BC90E5061BC"),
                            Content =  "1. Existing systems withinexecuting bodies, such asmunicipalities already in use,must be adapted to the newsystem.</br></br>2. Considering the number ofsystems in the country, both atthe national level (such asG4B.ir) and within theorganization, user confusionand work interference willoccur."
                        },
                        new EnglishCard
                        {
                            Id = Guid.Parse("d64FCE78-CC0E-41CB-9E6F-7BC90E5061BC"),
                            Type = EnglishCardType.Middle,
                            Color = EnglishCardColor.Yellow,
                            Title = "PROOF",
                            Order = 11,
                            SiblingId = Guid.Parse("d64FCE78-CC0E-41CB-9E6F-7BC90E5061BB"),
                            Content =  "1. A good practice that exists inthe country and manyadvanced countries have alsoexperienced it is to use asingle window to consolidateservices and access them allthrough one point."
                        },

                        new EnglishCard
                        {
                            Id = Guid.NewGuid(),
                            Type = EnglishCardType.Middle,
                            Color = EnglishCardColor.Yellow,
                            Title = "PROOF",
                            Order = 12,
                            Line = true,
                            Content =  "1. The registration of more than 400,000 requests in a 6-month period shows that it is accepted by citizens."
                        },

                        new EnglishCard
                        {
                            Id = Guid.Parse("e64FCE78-CC0E-41CB-9E6F-7BC90E5061BB"),
                            Type = EnglishCardType.Middle,
                            Color = EnglishCardColor.Green,
                            Title = "SOLUTION",
                            Order = 13,
                            SiblingId = Guid.Parse("e64FCE78-CC0E-41CB-9E6F-7BC90E5061BC"),
                            Content =  "1. IranEland system is a singleservice window that providesaccess to services from onepoint.</br></br>2. The architecture of theIranEland is characterized bythe idea of ​providing most ofits services in the backgroundand in interaction with othersystems.</br></br>3. All parallel systems are eitherremoved or, according to theapproved regulations, theymust be called in thebackground without involvingthe user."
                        },
                        new EnglishCard
                        {
                            Id = Guid.Parse("e64FCE78-CC0E-41CB-9E6F-7BC90E5061BC"),
                            Type = EnglishCardType.Middle,
                            Color = EnglishCardColor.Yellow,
                            Title = "COMMON GROUND",
                            Order = 14,
                            SiblingId = Guid.Parse("e64FCE78-CC0E-41CB-9E6F-7BC90E5061BB"),
                            Content =  "1. A good practice thatexists in the country andmany advanced countrieshave also experienced itis to use a single servicewindow to consolidateservices and access themall through one channel.</br></br>2. Eliminating parallelsystems, not needing togo in person and makingthe request for a permitelectronically, will leadto a major reduction intime and financial costs,which will lead to thesatisfaction of citizens."
                    },
                        
                        #endregion

                        #region End           
                        new EnglishCard
                        {
                            Id = Guid.NewGuid(),
                            Type = EnglishCardType.End,
                            Color = EnglishCardColor.Yellow,
                            Title = "COMMON GROUND",
                            Line = true,
                            Order = 0,
                            Content=  "1. Considering the existinginfrastructure in the fields of legal,communication and information, aswell as the possibility of usingartificial intelligence algorithmsbased on deep learning technologiesbased on high quality satelliteimages, it is possible to realize theidea and actually exploit it."
                        },

                        new EnglishCard
                        {
                            Id = Guid.Parse("164FCE78-CC0E-41CB-9E6F-7BC90E5061BB"),
                            Type = EnglishCardType.End,
                            Color = EnglishCardColor.Blue,
                            Title = "DISTINATION",
                            Line = true,
                            Order = 1,
                            SiblingId = Guid.Parse("164FCE78-CC0E-41CB-9E6F-7BC90E5061BC"),
                            Content =  "1. The land of the country is underconstant and intelligentsurveillance. Any suspiciouscases are automatically reportedand dealt with in the executingsystem.</br></br>2. Citizens can now apply for landand building permits onlinewithout visiting variousorganizations.</br></br>3. The system also provides theanalytical information needed forgovernment planning andindividuals involved in land\u0002related areas"
                        },
                        new EnglishCard
                        {
                            Id = Guid.Parse("164FCE78-CC0E-41CB-9E6F-7BC90E5061BC"),
                            Type = EnglishCardType.End,
                            Color = EnglishCardColor.Blue,
                            Title = "PROOF",
                            Order = 2,
                            SiblingId = Guid.Parse("164FCE78-CC0E-41CB-9E6F-7BC90E5061BB"),
                            Content =  "It is expected that,</br></br>1. The government will be able toreact quickly against landgrabbing and prevent itsuccessfully</br></br>2. The citizens can submit andtrack their requests throughIranEland without visitingmultiple organizations.</br></br>Available on:<a href='https://newportal.iraneland.ir'>https://newportal.iraneland.ir</a>"
                        },
                        #endregion
                    }
                };

                _context.EnglishPage.Add(englishPage);

                _context.SaveChanges();

                var englishPageId = await _context.EnglishPage.Select(b => b.Id).FirstAsync();

                var problems = new List<EnglishProblem>
                {
                        new EnglishProblem("People may need help to be able to use the system conveniently." ,englishPageId),
                        new EnglishProblem("The existence ofnumerous beneficiaries in the country poses a significant challenge due to their need for coordination and alignment.", englishPageId),
                        new EnglishProblem("The responsibleentities' information, processes, and services must be uniform and integrated,especially maps and informational layers.", englishPageId),
                        new EnglishProblem("Existing systemswithin executing bodies, such as municipalities already in use, must be adaptedto the new system.",englishPageId),
                        new EnglishProblem("Artificialintelligence techniques, especially in image processing, may exhibitunacceptable accuracy.", englishPageId),
                };

                _context.EnglishPageProblem.AddRange(problems);


                var solutions = new List<EnglishSolution>
                {
                        new EnglishSolution("The system has a user-friendly interface, and also allowing people to seek assistance from knowledgeableagents electronically.", englishPageId),
                        new EnglishSolution("Clear and enforceable laws and regulations should be formulated.", englishPageId),
                        new EnglishSolution("The system should only receive essential services such as maps from their primary sources.", englishPageId),
                        new EnglishSolution("Data exchange interfaces have been established, enabling systems to interact with the unified land managementportal while maintaining their independence.", englishPageId),
                        new EnglishSolution("Cutting-edge technologies like deeplearning and image processing are utilized.", englishPageId),
                        new EnglishSolution("The architecture of the IranEland is characterized by the idea of ​​providing most of its services in the background and ininteraction with other systems.", englishPageId),
                        new EnglishSolution("Following international frameworks and standards in this regard.", englishPageId),
                };

                _context.EnglishPageSolution.AddRange(solutions);

                await _context.SaveChangesAsync();

            }

            if (!_context.FooterPage.Any())
            {
                var footer = new FooterPage();
                _context.FooterPage.Add(footer);
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

        private List<ApprovalType> approvalTypes =>
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
                        lorem , DateTime.Now.AddDays(-5) , LawType.Regulation , true , Guid.Empty , Guid.Empty , Guid.Empty , Guid.Empty ,Guid.Empty , "test.pdf"),
                new Law("اختصاص اعتبار به وزارت نيرو به منظور تثبيت و ايمن سازي آبراهه هاي مشرف به محدوده سد امير كبير و لايروبي و رسوب برداري رودخانه پايين دست و جبران خسارات ناشي از بارندگي شديد و سيلاب مورخ 1402/03/18 در محور كرج - چالوس حد فاصل تونل هاي (2- الف) و (2-ب)" , new Announcement("44545" , DateTime.Now.AddDays(-27)), new Newspaper("58653" , DateTime.Now.AddDays(-43)) ,
                        lorem , DateTime.Now.AddDays(-23) , LawType.Rule , true ,  Guid.Empty , Guid.Empty , Guid.Empty , Guid.Empty ,Guid.Empty, "test.pdf"),
                new Law(" آيين نامه شناسايي و صيانت از وسيله هاي نقليه تاريخي" , new Announcement("44545" , DateTime.Now.AddDays(-73)), new Newspaper("58653" , DateTime.Now.AddYears(-86)) ,
                        lorem , DateTime.Now.AddMonths(-2) , LawType.Regulation , true ,  Guid.Empty , Guid.Empty , Guid.Empty , Guid.Empty ,Guid.Empty, "test.pdf"),
                new Law("همتراز شدن دبيركل كميسيون ملي يونسكو با مقامات موضوع بند (هـ) ماده (71) قانون مديريت خدمات كشوري" , new Announcement("44545" , DateTime.Now.AddDays(-55)), new Newspaper("58653" , DateTime.Now.AddDays(-120)) ,
                        lorem , DateTime.Now.AddDays(-125) , LawType.Rule , true , Guid.Empty , Guid.Empty , Guid.Empty , Guid.Empty ,Guid.Empty, "test.pdf"),
                new Law("تخصيص اعتبار به مبلغ سيصد ميليارد (300/000/000/000) ريال براي تامين مواد مصرفي آزمايشگاهي به منظور خريد تجهيزات براي شناسايي و تشخيص هويت متوفيان ناشي از وقوع حوادث غيرمترقبه در اختيار سازمان پزشكي قانوني كشور" , new Announcement("44545" , DateTime.Now.AddDays(-2)), new Newspaper("58653" , DateTime.Now.AddDays(-155)) ,
                        lorem , DateTime.Now.AddDays(-50) , LawType.Rule , true ,  Guid.Empty , Guid.Empty , Guid.Empty , Guid.Empty ,Guid.Empty, "test.pdf"),
                new Law(" \tتعيين صفر درصد سود بازرگاني قطعات گوشت مرغ ا" , new Announcement("44545" , DateTime.Now.AddDays(-23)), new Newspaper("58653" , DateTime.Now.AddDays(-17)) ,
                        lorem , DateTime.Now.AddDays(-20) , LawType.Regulation , true , Guid.Empty , Guid.Empty , Guid.Empty , Guid.Empty ,Guid.Empty, "test.pdf"),
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
                 description: "<p>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون</br>                بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع</br>                با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه</br>                و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و</br>                فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها،</br>                و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل</br>                دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.</br>            </p></br>            <h3 class=\"mt-30\"></br>                لورم ایپسوم متن ساختگی با تولید</br>            </h3></br>            <p class=\"mt-10\">لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است،</br>                چاپگرها و متون</br>                بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع</br>                با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه</br>                و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و</br>                فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها،</br>                و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل</br>                دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.</br>            </p></br>            <img src=\"/banner.jpg\" class=\"mt-30\" /></br>            <p class=\"mt-30\">لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است،</br>                چاپگرها و متون</br>                بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع</br>                با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه</br>                و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و</br>                فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها،</br>                و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل</br>                دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.</br>            </p></br>            <p class=\"mt-20\">لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است،</br>                چاپگرها و متون</br>                بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع</br>                با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه</br>                و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و</br>                فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها،</br>                و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل</br>                دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.</br>            </p>",
                 headline: "لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها، و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.",
                 "mehrnews.com/x33LcG", DateTime.Now, Guid.Empty, 0);
        #endregion

        #region Contact
        public FrequentlyAskedQuestions FrequentlyAskedQuestions =>
            new FrequentlyAskedQuestions("لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ؟", "<p>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها، و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد. لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ.</p>");

        public Guide Guide =>
            new Guide("لورم ایپسوم متن ساختگی با تولید", "<style>.h_iframe-aparat_embed_frame{position:relative;}.h_iframe-aparat_embed_frame .ratio{display:block;width:100%;height:auto;}.h_iframe-aparat_embed_frame iframe{position:absolute;top:0;left:0;width:100%;height:100%;}</style><div class=\"h_iframe-aparat_embed_frame\"><span style=\"display: block;padding-top: 57%\"></span><iframe src=\"https://www.aparat.com/video/video/embed/videohash/Dqev1/vt/frame\"  allowFullScreen=\"true\" webkitallowfullscreen=\"true\" mozallowfullscreen=\"true\"></iframe></div></br><p class='mt-40'>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی.</p></br><h3 class='font-dana mt-40'>لورم ایپسوم متن ساختگی با تولید</h3></br><p class='mt-20'>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد، کتابهای زیادی در شصت و سه درصد گذشته حال و آینده، شناخت فراوان جامعه و متخصصان را می طلبد، تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی، و فرهنگ پیشرو در زبان فارسی ایجاد کرد، در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها، و شرایط سخت تایپ به پایان رسد و زمان مورد نیاز شامل حروفچینی دستاوردهای اصلی، و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.</p></br><img class='mt-40' src='/banner.jpg'/></br><p class='mt-20'>لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ، و با استفاده از طراحان گرافیک است، چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است، و برای شرایط فعلی تکنولوژی مورد نیاز، و کاربردهای متنوع با هدف بهبود</p></br>", true);

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
            new RelatedCompany( "ارتباطات زیرساخت" , "17.png",0),
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

        public List<RelatedLink> RelatedLinks =>
            new List<RelatedLink>
            {
                new RelatedLink("پایگاه اطلاع رسانی دفتر مقام معظم رهبری" , "#" , 0),
                new RelatedLink("پایگاه اطلاع رسانی ریاست جمهوری" , "#" , 1),
                new RelatedLink("پایگاه اطلاع رسانی دولت" , "#",2),
                new RelatedLink("وزارت ارتباطات و فناوری اطلاعات" , "#",3),
                new RelatedLink("سازمان فناوری اطلاعات ایران" , "#",4),
                new RelatedLink("ستاد هماهنگی مبارزه با مفاسد اقتصادی" , "#",5)
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
        private String video = "<div class=\"h_iframe-aparat_embed_frame\"><span style=\"display: block;padding-top: 57%\"></span><iframe</br>                            src=\"https://www.aparat.com/video/video/embed/videohash/jq0lh/vt/frame\" allowFullScreen=\"true\"</br>                            webkitallowfullscreen=\"true\" mozallowfullscreen=\"true\"></iframe></div>";
    }

    class NewsData
    {
        public String title { get; set; }
        public String _id { get; set; }

        public String newsText { get; set; }
        public NewsDataImage newsImage { get; set; }

        public DateTime _createDate { get; set; }
        public String newsDate { get; set; }
        public DateTime newsDateO { get; set; }

        public String description { get; set; }

    }

    class NewsDataImage
    {
        public String name { get; set; }
        public int size { get; set; }
        public String type { get; set; }
        public String value { get; set; }

    }

    class LawData
    {
        [JsonProperty("LawsID")]
        public long LawsId { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Articles")]
        public string Articles { get; set; }

        [JsonProperty("Date")]
        public string Date { get; set; }

        [JsonProperty("Reference")]
        public string Reference { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("CommunicatedNumber")]
        public string CommunicatedNumber { get; set; }

        [JsonProperty("CommunicatedDate")]
        public string CommunicatedDate { get; set; }

        [JsonProperty("NewspaperNumber")]
        public dynamic NewspaperNumber { get; set; }

        [JsonProperty("NewspaperDate")]
        public string NewspaperDate { get; set; }

        [JsonProperty("Presenter")]
        public string Presenter { get; set; }

        [JsonProperty("LawText")]
        public string LawText { get; set; }
    }

    internal class ParseStringConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}

