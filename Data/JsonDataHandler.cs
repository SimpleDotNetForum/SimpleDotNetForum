using Newtonsoft.Json;
using SimpleDotNetForum.Models;

namespace SimpleDotNetForum.Data
{
    public class JsonDataHandler
    {
        private const string DataFilePath = "Data/posts.json";

        public List<Post> LoadPosts()
        {
            if (!File.Exists(DataFilePath))
            {
                return new List<Post>();
            }

            var jsonData = File.ReadAllText(DataFilePath);
            return JsonConvert.DeserializeObject<List<Post>>(jsonData) ?? new List<Post>();
        }

        public void SavePosts(List<Post> posts)
        {
            var jsonData = JsonConvert.SerializeObject(posts, Formatting.Indented);
            File.WriteAllText(DataFilePath, jsonData);
        }
    }
}


