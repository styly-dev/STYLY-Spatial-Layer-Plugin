using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UnityEngine;

namespace Styly.SpatialLayer.Plugin
{
    public class StylyUrlParameter
    {
        public static string Get(string key)
        {
            return GetUrlParameters().ContainsKey(key) ? GetUrlParameters()[key] : null;
        }

        public static List<string> GetParameterList()
        {
            return GetUrlParameters().Keys.ToList();
        }

        private static IReadOnlyDictionary<string, string> GetUrlParameters()
        {
            var urlString = StylySystemInfo.GetInfo(StylySystemInfo.URL_KEY);
            return (urlString != null ) ? ParseUrlParameters(urlString) : null;
        }
        
        private static Dictionary<string, string> ParseUrlParameters(string url)
        {
            var result = new Dictionary<string, string>();
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                Debug.LogError($"Invalid URL: {url}");
                return result;
            }

            var query = uri.Query;

            if (string.IsNullOrEmpty(query)) return result;
            var queryParameters = HttpUtility.ParseQueryString(query);

            foreach (string key in queryParameters)
            {
                if (key != null)
                {
                    result[key] = queryParameters[key];
                }
            }

            return result;
        }
    }
}