using System.Collections;
using System.IO;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Styly.VisionOs.Plugin
{
    public class VisualScriptingParameterUtilityTest
    {
        [Test]
        [TestCase("Value.json","PrefabWithVariables_Value.prefab")]
        [TestCase("NoValue.json","PrefabWithVariables_NoValue.prefab")]
        [TestCase("NoVariable.json","PrefabWithVariables_NoVariable.prefab")]
        [TestCase("DontHaveVariable.json","Prefab_DontHaveVariables.prefab")]
        public void GenerateMetaFromPrefab(string jsonRelativePath, string prefabRelativePath)
        {
            var jsonPath = $"Packages/{Config.PackageName}/Tests/Editor/TestData/Json/{jsonRelativePath}";
            var targetPrefabPath = $"Packages/{Config.PackageName}/Tests/Editor/TestData/Prefab/{prefabRelativePath}";
          
            var expectedJson = File.ReadAllText(jsonPath).TrimEnd();
            var target = AssetDatabase.LoadAssetAtPath(targetPrefabPath, typeof(GameObject)) as GameObject;
            var resultJson = VisualScriptingParameterUtility.GetParameterDefinitionJson(target);
            Debug.Log(resultJson);
            
            Assert.That(JsonTestTool.JsonEquals(resultJson, expectedJson), Is.True);
        }

        [Test]
        [TestCase("Value.json","PrefabWithVariables_Value.prefab", "PrefabWithVariables_NoValue.prefab")]
        [TestCase("NoVariable.json","PrefabWithVariables_NoVariable.prefab", "PrefabWithVariables_NoVariable.prefab")]
        [TestCase("DontHaveVariable.json","Prefab_DontHaveVariables.prefab", "Prefab_DontHaveVariables.prefab")]
        public void SetParameterValuesToPrefab(string jsonRelativePath, string prefabRelativePath, string prefabNoValueRelativePath)
        {
            var jsonPath = $"Packages/{Config.PackageName}/Tests/Editor/TestData/Json/{jsonRelativePath}";
            var paramWithValuePrefabPath = $"Packages/{Config.PackageName}/Tests/Editor/TestData/Prefab/{prefabRelativePath}";
            var paramWithNoValuePrefabPath = $"Packages/{Config.PackageName}/Tests/Editor/TestData/Prefab/{prefabNoValueRelativePath}";

            var target = AssetDatabase.LoadAssetAtPath(paramWithNoValuePrefabPath, typeof(GameObject)) as GameObject;
            var referenceJson = File.ReadAllText(jsonPath).TrimEnd();
            var reference = AssetDatabase.LoadAssetAtPath(paramWithValuePrefabPath, typeof(GameObject)) as GameObject;
            var referenceVariables = reference.GetComponent<Variables>();
            
            VisualScriptingParameterUtility.SetParameterValuesToPrefab(target, referenceJson);

            var result = target.GetComponent<Variables>();

            if(referenceVariables == null)
            {
                Assert.That(result, Is.Null);
                return;
            }
            
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
            
            Assert.That(JsonTestTool.JsonEquals(resultJson, referenceJson), Is.True);
        }

    }
}