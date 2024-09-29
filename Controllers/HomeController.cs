using SimpleDotNetForum.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleDotNetForum.Data;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;

namespace SimpleDotNetForum.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly JsonDataHandler _dataHandler;
        private List<Post> _posts;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _dataHandler = new JsonDataHandler();
            _posts = _dataHandler.LoadPosts();
        }
        private readonly string _jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "posts.json");

        public IActionResult Index(string thread = null)
        {
            var posts = LoadPostsFromJson();
            if (!string.IsNullOrEmpty(thread))
            {
                posts = posts.Where(p => p.Thread.Equals(thread, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            ViewBag.SelectedThread = thread;
            return View(posts);
        }

        [HttpGet]
        public IActionResult Create(string thread)
        {
            ViewBag.Threads = new SelectList(new List<string> { "Politics", "Religion", "Programming" }, thread);
            ViewBag.Thread = thread; // Passing thread name to pre-select the category
            return View();
        }

        private static readonly object _lock = new object(); // Lock object

        [HttpPost]
        public IActionResult Create(Post post)
        {
            if (ModelState.IsValid)
            {
                lock (_lock) // Locking to ensure thread safety
                {
                    post.Id = _posts.Count > 0 ? _posts.Max(p => p.Id) + 1 : 1; // Generate a new ID
                    post.CreatedAt = DateTime.Now; // Set the created timestamp
                    _posts.Add(post); // Add post to the list
                    _dataHandler.SavePosts(_posts); // Save posts to JSON
                }

                return RedirectToAction("Index"); // Redirect to index after successful creation
            }

            // Prepare the threads for the view if model state is invalid
            ViewBag.Threads = new SelectList(new List<string> { "Politics", "Religion", "Programming" }, post.Thread);
            return View(post); // Return the view with the invalid model
        }

        public IActionResult Details(int id, string thread)
        {
            var posts = LoadPostsFromJson();
            var post = posts.FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            ViewBag.SelectedThread = post.Thread; // Set the selected thread here
            return View(post); // This returns the single post to the Details view
        }

        [HttpPost]
        public IActionResult Reply(int postId, string content)
        {
            // Read the existing posts from the JSON file
            var posts = ReadPostsFromJson();

            // Find the post with the specified postId
            var post = posts.FirstOrDefault(p => p.Id == postId);

            if (post != null)
            {
                var reply = new Reply
                {
                    PostId = postId,
                    Content = content,
                    CreatedAt = DateTime.Now
                };

                // Add the reply to the post's collection
                if (post.Replies == null)
                {
                    post.Replies = new List<Reply>();
                }

                post.Replies.Add(reply);

                // Save the updated posts back to the JSON file
                WritePostsToJson(posts);
            }

            // Redirect back to the post details page
            return RedirectToAction("Details", new { id = postId });
        }

        private List<Post> ReadPostsFromJson()
        {
            if (!System.IO.File.Exists(_jsonFilePath))
            {
                return new List<Post>();
            }

            var jsonData = System.IO.File.ReadAllText(_jsonFilePath);
            return JsonConvert.DeserializeObject<List<Post>>(jsonData) ?? new List<Post>();
        }

        private void WritePostsToJson(List<Post> posts)
        {
            var jsonData = JsonConvert.SerializeObject(posts, Formatting.Indented);
            System.IO.File.WriteAllText(_jsonFilePath, jsonData);
        }


        private List<Post> LoadPostsFromJson()
        {
            if (!System.IO.File.Exists(_jsonFilePath))
            {
                // Return an empty list if the JSON file does not exist
                return new List<Post>();
            }

            var jsonData = System.IO.File.ReadAllText(_jsonFilePath);
            var posts = JsonConvert.DeserializeObject<List<Post>>(jsonData);
            return posts ?? new List<Post>(); // Return an empty list if deserialization fails
        }
    }
}

