using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using StructureMap;
using Sunnet.Framework;
using Sunnet.Framework.Log;

namespace Sunnet.Cli.UIBase.Models
{
    public class TwitterHelper
    {
        private ISunnetLog logger;
        private HttpClient client;
        private TwitterHelper()
        {
            logger = ObjectFactory.GetInstance<ISunnetLog>();
            client = new HttpClient
            {
                BaseAddress = new Uri(SFConfig.TwitterUrl_ApiBase),
                Timeout = new TimeSpan(0, 0, 30)
            };
        }
        private static readonly TwitterHelper _ins = new TwitterHelper();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static TwitterHelper Instance { get { return _ins; } }

        public AccessToken GetTwitterAccessToken(string appKey = "", string appSecret = "")
        {
            if (string.IsNullOrEmpty(appKey))
                appKey = SFConfig.TwitterKey;
            if (string.IsNullOrEmpty(appSecret))
                appSecret = SFConfig.TwitterSecret;
            var authorization = Convert.ToBase64String(Encoding.Default.GetBytes(string.Format("{0}:{1}", appKey, appSecret)));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorization);
            try
            {
                var response = client.PostAsync(SFConfig.TwitterUrl_Oauth2Token, new StringContent("")).Result;
                /* sample response
             {"token_type":"bearer","access_token":"AAAAAAAAAAAAAAAAAAAAAC20UgAAAAAAYVQLfy%2Ba1V8KnWDtjYAZ4d2F47U%3DoW19uANbIPqc7RzbffpvzYWiWd3h8bG3aWfhHI4OZm4RDcYy23"}
             */
                var token = response.Content.ReadAsAsync<AccessToken>().Result;
                return token;
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                return null;
            }
        }

        public IEnumerable<Tweet> GetTweets(string access_token, string username, int count)
        {
            var tokenType = "bearer";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, access_token);
            var query = string.Format("screen_name={0}&count={1}", username, count);
            try
            {
                var response = client.GetAsync(string.Format("{0}?{1}", SFConfig.TwitterUrl_UserTimeline, query)).Result;
                var jsonFormatter = new List<MediaTypeFormatter>()
                {
                    new JsonMediaTypeFormatter()
                };
                var tweets = response.Content.ReadAsAsync<IEnumerable<Tweet>>(jsonFormatter).Result;
                return tweets;
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                return null;
            }
        }
    }
}