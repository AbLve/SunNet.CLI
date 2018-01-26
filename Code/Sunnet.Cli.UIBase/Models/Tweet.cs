using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Sunnet.Cli.UIBase.Models
{
    public class AccessToken
    {
        public string token_type { get; set; }
        public string access_token { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class TweetUser
    {
        public UInt64 id { get; set; }
        public string id_str { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public int followers_count { get; set; }
        public int friends_count { get; set; }
        public int listed_count { get; set; }
        public string created_at { get; set; }
        public int favourites_count { get; set; }
        public int listutc_offseted_count { get; set; }
        public string time_zone { get; set; }
        public bool geo_enabled { get; set; }
        public int statuses_count { get; set; }
        public string lang { get; set; }
        public bool contributors_enabled { get; set; }
        public bool is_translator { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Tweet
    {
        [JsonProperty]
        public string created_at { get; set; }
        [JsonProperty]
        public UInt64 id { get; set; }
        [JsonProperty]
        public string id_str { get; set; }
        [JsonProperty]
        public string text { get; set; }
        public string source { get; set; }
        public bool truncated { get; set; }
        public TweetUser user { get; set; }
        public int retweet_count { get; set; }
        public int favorite_count { get; set; }
        public bool favorited { get; set; }
        public bool retweeted { get; set; }
        public bool possibly_sensitive { get; set; }
        public string lang { get; set; }
    }

    /*
     * sample tweet :
     * {
        "created_at": "Wed Aug 06 21:47:55 +0000 2014",
        "id": 497137014530383900,
        "id_str": "497137014530383872",
        "text": "RT @TwitterDev: Learn how to consume millions of tweets with @twitterapi at #TDC2014 in São Paulo #bigdata tomorrow at 2:10pm http://t.co/p…",
        "source": "<a href=\"http://itunes.apple.com/us/app/twitter/id409789998?mt=12\" rel=\"nofollow\">Twitter for Mac</a>",
        "truncated": false,
        "in_reply_to_status_id": null,
        "in_reply_to_status_id_str": null,
        "in_reply_to_user_id": null,
        "in_reply_to_user_id_str": null,
        "in_reply_to_screen_name": null,
        "user": {
            "id": 6253282,
            "id_str": "6253282",
            "name": "Twitter API",
            "screen_name": "twitterapi",
            "location": "San Francisco, CA",
            "description": "The Real Twitter API. I tweet about API changes, service issues and happily answer questions about Twitter and our API. Don't get an answer? It's on my website.",
            "url": "http://t.co/78pYTvWfJd",
            "entities": {
                "url": {
                    "urls": [
                        {
                            "url": "http://t.co/78pYTvWfJd",
                            "expanded_url": "http://dev.twitter.com",
                            "display_url": "dev.twitter.com",
                            "indices": [
                                0,
                                22
                            ]
                        }
                    ]
                },
                "description": {
                    "urls": []
                }
            },
            "protected": false,
            "followers_count": 2299455,
            "friends_count": 48,
            "listed_count": 12803,
            "created_at": "Wed May 23 06:01:13 +0000 2007",
            "favourites_count": 26,
            "utc_offset": -25200,
            "time_zone": "Pacific Time (US & Canada)",
            "geo_enabled": true,
            "verified": true,
            "statuses_count": 3514,
            "lang": "en",
            "contributors_enabled": false,
            "is_translator": false,
            "is_translation_enabled": false,
            "profile_background_color": "C0DEED",
            "profile_background_image_url": "http://pbs.twimg.com/profile_background_images/656927849/miyt9dpjz77sc0w3d4vj.png",
            "profile_background_image_url_https": "https://pbs.twimg.com/profile_background_images/656927849/miyt9dpjz77sc0w3d4vj.png",
            "profile_background_tile": true,
            "profile_image_url": "http://pbs.twimg.com/profile_images/2284174872/7df3h38zabcvjylnyfe3_normal.png",
            "profile_image_url_https": "https://pbs.twimg.com/profile_images/2284174872/7df3h38zabcvjylnyfe3_normal.png",
            "profile_banner_url": "https://pbs.twimg.com/profile_banners/6253282/1347394302",
            "profile_link_color": "0084B4",
            "profile_sidebar_border_color": "C0DEED",
            "profile_sidebar_fill_color": "DDEEF6",
            "profile_text_color": "333333",
            "profile_use_background_image": true,
            "default_profile": false,
            "default_profile_image": false,
            "following": null,
            "follow_request_sent": null,
            "notifications": null
        },
        "geo": null,
        "coordinates": null,
        "place": null,
        "contributors": null,
         
        "retweet_count": 23,
        "favorite_count": 0,
        "entities": {
            "hashtags": [
                {
                    "text": "TDC2014",
                    "indices": [
                        76,
                        84
                    ]
                },
                {
                    "text": "bigdata",
                    "indices": [
                        98,
                        106
                    ]
                }
            ],
            "symbols": [],
            "urls": [
                {
                    "url": "http://t.co/pTBlWzTvVd",
                    "expanded_url": "http://www.thedevelopersconference.com.br/tdc/2014/saopaulo/trilha-bigdata",
                    "display_url": "thedevelopersconference.com.br/tdc/2014/saopa…",
                    "indices": [
                        126,
                        140
                    ]
                }
            ],
            "user_mentions": [
                {
                    "screen_name": "TwitterDev",
                    "name": "TwitterDev",
                    "id": 2244994945,
                    "id_str": "2244994945",
                    "indices": [
                        3,
                        14
                    ]
                },
                {
                    "screen_name": "twitterapi",
                    "name": "Twitter API",
                    "id": 6253282,
                    "id_str": "6253282",
                    "indices": [
                        61,
                        72
                    ]
                }
            ]
        },
        "favorited": false,
        "retweeted": false,
        "possibly_sensitive": false,
        "lang": "en"
    }
     */
}