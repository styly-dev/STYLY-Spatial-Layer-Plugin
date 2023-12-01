using System.Collections;
using System.IO;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Styly.VisionOs.Plugin
{
    public class VisualScriptingParameterUtilityTest
    {
        string jsonPath = $"Packages/{Config.PackageName}/Tests/Editor/TestData/Json/test.json";
        string paramWithValuePrefabPath = $"Packages/{Config.PackageName}/Tests/Editor/TestData/Prefab/PrefabWithVariables_Value.prefab";
        string paramWithNoValuePrefabPath = $"Packages/{Config.PackageName}/Tests/Editor/TestData/Prefab/PrefabWithVariables_NoValue.prefab";

        [Test]
        public void GenerateParameterFromPrefab()
        {
            var expectedJson = File.ReadAllText(jsonPath).TrimEnd();
            var target = AssetDatabase.LoadAssetAtPath(paramWithValuePrefabPath, typeof(GameObject)) as GameObject;
            var resultJson = VisualScriptingParameterUtility.GetParameterDefinitionJson(target);
            Debug.Log(resultJson);
            
            Assert.That(resultJson, Is.Not.Empty);
            Assert.That(resultJson, Is.EqualTo(expectedJson));
        }

        [Test]
        public void SetParameterValuesToPrefab()
        {
            var target = AssetDatabase.LoadAssetAtPath(paramWithNoValuePrefabPath, typeof(GameObject)) as GameObject;
            var referenceJson = File.ReadAllText(jsonPath).TrimEnd();
            var reference = AssetDatabase.LoadAssetAtPath(paramWithValuePrefabPath, typeof(GameObject)) as GameObject;
            var referenceVariables = reference.GetComponent<Variables>();
            
            VisualScriptingParameterUtility.SetParameterValuesToPrefab(target, referenceJson);

            var result = target.GetComponent<Variables>();
            Assert.That(result, Is.Not.Null);
            foreach (var declaration in result.declarations)
            {
                var name = declaration.name;
                var value = declaration.value;
                if(value == null){continue;}
                
                var type = declaration.value.GetType();

                Debug.Log($"result {value},expect {referenceVariables.declarations.Get(name, type )}");
                var expectedValue = referenceVariables.declarations.Get(name, type);
                Assert.That(value, Is.EqualTo(expectedValue));
                if (value is IList)
                {
                    var valueList = value as IList;
                    var expectList = expectedValue as IList;
                    Assert.That(valueList.Count, Is.EqualTo(expectList.Count));
                    for (int i = 0; i < valueList.Count; i++)
                    {
                        Debug.Log($"result {valueList[i]}, expect {expectList[i]}");
                        Assert.That(valueList[i], Is.EqualTo(expectList[i]));
                    }
                }
            }

            var resultJson = VisualScriptingParameterUtility.GetParameterDefinitionJson(target);
            Debug.Log(resultJson);
            
            Assert.That(resultJson, Is.EqualTo(referenceJson));

        }

    }
}