using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Styly.SpatialLayer.Plugin;
using UnityEngine;

namespace Styly.SpatialLayer.Test
{
    public class StylySystemInfoTest : MonoBehaviour
    {
        private const string TEST_URL = "https://styly.cc?type=Unbounded&param1=123456&param2=true";
        [SetUp]
        public void Setup()
        {
            StylySystemInfoManager.SetInfo(StylySystemInfo.URL_KEY,TEST_URL);
            StylySystemInfoManager.SetInfo("hoge", "fuga");
        }
        
        [Test]
        public void GetInfoNameListTest()
        {
            var nameList = StylySystemInfo.GetInfoNameList();

            Assert.That(nameList.Count, Is.EqualTo(2));
            Assert.That(nameList.Contains(StylySystemInfo.URL_KEY));
            Assert.That(nameList.Contains("hoge"));
        }

        [Test]
        public void GetSystemInfoTest()
        {
            var url = StylySystemInfo.GetInfo(StylySystemInfo.URL_KEY);
            Assert.That(url, Is.EqualTo(TEST_URL));

            var val = StylySystemInfo.GetInfo("hoge");
            Assert.That(val, Is.EqualTo("fuga"));
        }

        [Test]
        public void GetParameterListTest()
        {
            var parameterList = StylyUrlParameter.GetParameterList();
            
            Assert.That(parameterList.Capacity, Is.EqualTo(3));
            Assert.That(parameterList.Contains("type"));
            Assert.That(parameterList.Contains("param1"));
            Assert.That(parameterList.Contains("param2"));
        }
        
        [Test]
        public void GetUrlParameterTest()
        {
            Assert.That(StylyUrlParameter.Get("type"), Is.EqualTo("Unbounded"));
            Assert.That(StylyUrlParameter.Get("param1"), Is.EqualTo("123456"));
            Assert.That(StylyUrlParameter.Get("param2"), Is.EqualTo("true"));
        }
    }
}