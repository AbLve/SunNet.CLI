using System.IO;
using Sunnet.Cli.UIBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using Sunnet.Framework;

namespace Sunnet.Cli.MainSite.Controllers
{
    /*
    [RoutePrefix("api/twitter")]
    public class TwitterController : ApiController
    {
        private string _Key_token_time = "lastAccesstokenTime";
        private string _Key_token = "access_token";
        private string _key_tweets = "tweets";
        private int cache_time_minutes = 10;
        private string cache_dependency_file = "";

        private IEnumerable<Tweet> GetTweetsFromCache()
        {
            var obj =
            HttpRuntime.Cache.Get(_key_tweets);
            var tweets = obj as IList<Tweet>;
            return tweets;
        }

        [Route("tweet")]
        public async Task<IEnumerable<Tweet>> GetLatestTwitter()
        {
            var tweets = GetTweetsFromCache();
            if (tweets != null && tweets.Any())
                return tweets;

            cache_dependency_file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Web.config");

            var lastAccessTokenTime = DateTime.Now;
            var _obj = HttpRuntime.Cache.Get(_Key_token_time);
            var access_token = "";
            if (_obj == null || !DateTime.TryParse(_obj.ToString(), out lastAccessTokenTime) ||
                (lastAccessTokenTime - DateTime.Now).TotalMinutes > cache_time_minutes)
            {
                lastAccessTokenTime = DateTime.Now;
                HttpRuntime.Cache.Add(_Key_token_time, lastAccessTokenTime, new CacheDependency(cache_dependency_file),
                    DateTime.Now.AddMinutes(cache_time_minutes), TimeSpan.Zero,
                    CacheItemPriority.Low, null);
                var token = TwitterHelper.Instance.GetTwitterAccessToken();
                if (token != null) access_token = token.access_token;
            }
            else
            {
                _obj = HttpRuntime.Cache.Get(_Key_token);
                if (_obj != null)
                    access_token = _obj.ToString();
            }
            if (string.IsNullOrEmpty(access_token))
            {
                var token = TwitterHelper.Instance.GetTwitterAccessToken();
                if (token != null) access_token = token.access_token;
            }
            if (string.IsNullOrEmpty(access_token))
            {
                return null;
            }
            HttpRuntime.Cache.Add(_Key_token, access_token, new CacheDependency(cache_dependency_file),
                DateTime.Now.AddMinutes(cache_time_minutes), TimeSpan.Zero
                , CacheItemPriority.Low, null);

            tweets = TwitterHelper.Instance.GetTweets(access_token, SFConfig.Twitter_ScreenName, 3);
            HttpRuntime.Cache.Add(_key_tweets, tweets, new CacheDependency(cache_dependency_file),
                DateTime.Now.AddMinutes(cache_time_minutes), TimeSpan.Zero,
                CacheItemPriority.Low, null);
            return tweets;
        }
        [Route("twitter_api")]
        [HttpGet]
        public async Task<string> GetTwitterApi(string method, string api, string query, string scheme, string authorization, bool base64 = false)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://api.twitter.com/"),
                Timeout = new TimeSpan(0, 0, 10)
            };
            if (base64)
                authorization = Convert.ToBase64String(Encoding.Default.GetBytes(authorization));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, authorization);

            HttpResponseMessage response = null;
            if (method.Equals("post", StringComparison.CurrentCultureIgnoreCase))
            {
                response = await client.PostAsync(api, new StringContent(query)); // Blocking call!
            }
            else
            {
                response = await client.GetAsync(string.Format("{0}?{1}", api, query));
            }
            var responseStatus = response.IsSuccessStatusCode ? "Success:" : "Fail:";
            responseStatus += response.StatusCode.ToString() + (int)response.StatusCode + ",";
            var tweets = await response.Content.ReadAsStringAsync();
            return responseStatus + tweets;
        }
    }
     * */
}
