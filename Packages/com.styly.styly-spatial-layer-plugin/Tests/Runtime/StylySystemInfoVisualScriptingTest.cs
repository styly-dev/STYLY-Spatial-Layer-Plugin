using NUnit.Framework;
using Styly.SpatialLayer.Plugin;
using UnityEngine;
using UnityEngine.TestTools;

namespace Styly.SpatialLayer.Test
{
    public class StylySystemInfoVisualScriptingTest
    {
        private const string TEST_URL = "https://styly.cc?type=Unbounded&param1=123456&param2=true";
        private GameObject prefabInstance;
        
        [SetUp]
        public void SetUp()
        {
            StylySystemInfoManager.SetInfo(StylySystemInfo.URL_KEY,TEST_URL);
            var prefab = Resources.Load<GameObject>("StylySystemInfo");
            Assert.IsNotNull(prefab, "Prefab 'StylySystemInfo' not found in Resources folder.");
            
            prefabInstance = Object.Instantiate(prefab);
            Assert.IsNotNull(prefabInstance, "Failed to instantiate prefab.");
            prefabInstance.SetActive(true);
        }

        [TearDown]
        public void TearDown()
        {
            if (prefabInstance != null)
            {
                Object.DestroyImmediate(prefabInstance);
            }
        }
        
        [UnityTest]
        public System.Collections.IEnumerator GetInfoAndURLParameterByVisualScriptingTest()
        {
            yield return new WaitForSeconds(0.5f);
            
            LogAssert.Expect(LogType.Log, TEST_URL);
            LogAssert.Expect(LogType.Log, "Unbounded");
            LogAssert.Expect(LogType.Log, "123456");
            LogAssert.Expect(LogType.Log, "true");
        }
    }
}