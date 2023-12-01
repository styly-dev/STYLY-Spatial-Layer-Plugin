using System.Linq;
using Newtonsoft.Json.Linq;

namespace Styly.VisionOs.Plugin
{
    public class JsonTestTool
    {
        public static bool JsonEquals(string json1, string json2)
        {
            if (json1 == json2) { return true;}
            JToken token1 = JObject.Parse(json1);
            JToken token2 = JObject.Parse(json2);
            return JsonEquals(token1, token2);
        }
    
        private static bool JsonEquals(JToken token1, JToken token2)
        {
            if (token1.Type == JTokenType.Integer && token2.Type == JTokenType.Float ||
                token1.Type == JTokenType.Float && token2.Type == JTokenType.Integer)
            {
                return (double)token1 == (double)token2;
            }
            
            if (token1.Type != token2.Type)
            {
                return false;
            }
            
            switch (token1.Type)
            {
                case JTokenType.Object:
                    var obj1 = (JObject)token1;
                    var obj2 = (JObject)token2;
                    var keys1 = obj1.Properties().Select(p => p.Name).OrderBy(name => name);
                    var keys2 = obj2.Properties().Select(p => p.Name).OrderBy(name => name);
                    if (!keys1.SequenceEqual(keys2))
                    {
                        return false;
                    }
                    
                    if (keys1.Any(key => !JsonEquals(obj1[key], obj2[key])))
                    {
                        return false;
                    }
                    break;
                case JTokenType.Array:
                    var arr1 = (JArray)token1;
                    var arr2 = (JArray)token2;
                    if (arr1.Count != arr2.Count)
                    {
                        return false;
                    }
                    
                    if (arr1.Where((t, i) => !JsonEquals(t, arr2[i])).Any())
                    {
                        return false;
                    }
                    
                    break;
                default:
                    if (!JToken.DeepEquals(token1, token2))
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }
    }

}
