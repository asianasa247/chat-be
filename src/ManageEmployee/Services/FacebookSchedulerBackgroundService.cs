using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces;
using System;
using System.Net.Http.Headers;
namespace ManageEmployee.Services
{
    public class FacebookSchedulerBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public FacebookSchedulerBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var scheduledPostService = scope.ServiceProvider.GetRequiredService<IScheduledPostService>();

                    var activeAccess = dbContext.FaceAccesses.FirstOrDefault(f => f.IsActive);
                    if (activeAccess == null)
                    {
                        Console.WriteLine("[FB Scheduler] Không tìm thấy Page Access Token đang hoạt động.");
                        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                        continue;
                    }

                    var pageId = activeAccess.PageId;
                    var pageAccessToken = activeAccess.PageAccessToken;

                    var allPosts = await scheduledPostService.GetAllScheduledPostsAsync();
                    var posts = allPosts
                        .Where(p => p.IsActive && p.ScheduledTime.ToUniversalTime() <= DateTime.UtcNow)
                        .ToList();

                    foreach (var post in posts)
                    {
                        try
                        {
                            if (post.PhotoData == null || post.PhotoData.Length == 0)
                            {
                                Console.WriteLine($"[FB Scheduler] Bài đăng {post.Id} không có ảnh, bỏ qua.");
                                continue;
                            }

                            using var client = new HttpClient();
                            var postUrl = $"https://graph.facebook.com/{pageId}/photos?access_token={pageAccessToken}";

                            using var content = new MultipartFormDataContent();
                            content.Add(new StringContent(post.Message ?? ""), "caption");

                            using var stream = new MemoryStream(post.PhotoData);
                            var streamContent = new StreamContent(stream);
                            streamContent.Headers.ContentType = new MediaTypeHeaderValue(post.PhotoContentType);
                            content.Add(streamContent, "source", post.PhotoFileName);

                            var response = await client.PostAsync(postUrl, content);

                            if (response.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"[FB Scheduler] Đăng bài thành công: {post.Id}");
                                Console.WriteLine($"[FB Scheduler] PageAccessToken: {pageAccessToken}");

                                post.PostedTime = DateTime.UtcNow.AddHours(7); // VN time

                                switch (post.RepeatType?.ToLower())
                                {
                                    case "daily":
                                        post.ScheduledTime = post.ScheduledTime.AddDays(1);
                                        break;
                                    case "weekly":
                                        post.ScheduledTime = post.ScheduledTime.AddDays(7);
                                        break;
                                    default:
                                        post.IsActive = false;
                                        break;
                                }

                                await scheduledPostService.UpdateScheduledPostAsync(post);
                            }
                            else
                            {
                                var error = await response.Content.ReadAsStringAsync();
                                Console.WriteLine($"[FB Scheduler] Lỗi khi đăng bài {post.Id}: {error}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[FB Scheduler] Exception: {ex.Message}");
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
