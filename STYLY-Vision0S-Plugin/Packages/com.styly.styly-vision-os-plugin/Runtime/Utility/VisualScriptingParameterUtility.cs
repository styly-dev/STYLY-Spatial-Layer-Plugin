using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Styly.VisionOs.Plugin
{
    public static class VisualScriptingParameterUtility
    {
        [Serializable]
        private class ParameterDefinition
        {
            [JsonProperty("graph")]
            public VariableDefinitionClass VariableDefinition { get; set; }
        }

        [Serializable]
        private class VariableDefinitionClass
        {
            [JsonProperty("parameters")]
            public VariableClass[] Variables { get; set; }
        }

        [Serializable]
        private class VariableClass
        {
            [JsonProperty("type")]
            public string Type { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("value")]
            public object Value { get; set; }
        }

        /// <summary>
        /// Set parameters to VisualScripting.Variables by JSON.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="parameterValueJson"></param>
        /// <returns></returns>
        public static bool SetParameterValuesToPrefab(GameObject gameObject, string parameterValueJson)
        {
            var parameterValue = JsonConvert.DeserializeObject<ParameterDefinition>(parameterValueJson);
            if (parameterValue == null) {return false;}
            if (!gameObject.TryGetComponent<Variables>(out var VariableComponent)) { return false; }

            void SetVariable(string name, object value) => Variables.Object(VariableComponent).Set(name, value);

            Color ConvertToColor(float[] rgba) => new (rgba[0], rgba[1], rgba[2], rgba[3]);

            foreach (var variable in parameterValue.VariableDefinition.Variables)
            {
                
                switch (variable.Type)
                {
                    case "String":
                    case "Boolean":
                        SetVariable(variable.Name, variable.Value);
                        break;
                    case "Float":
                        SetVariable(variable.Name, (float)(double)variable.Value);
                        break;
                    case "Integer":
                        SetVariable(variable.Name, (int)(long)variable.Value);
                        break;
                    case "Color":
                        SetVariable(variable.Name, ConvertToColor(((JArray)variable.Value).Select(jToken => jToken.ToObject<float>()).ToArray()));
                        break;
                    case "String[]":
                        var valueAsList = variable.Value as IList;
                        if (valueAsList == null) { break; }
                        
                        var stringValues = valueAsList.Cast<JValue>()
                            .Select(jValue => jValue.ToObject<string>())
                            .ToList();
                        SetVariable(variable.Name, stringValues);
                        break;
                    case "Float[]":
                        SetVariable(variable.Name, ((IList)variable.Value)?.Cast<JArray>().Select(item => item.ToObject<System.Single>()).ToList());
                        break;
                    case "Integer[]":
                        SetVariable(variable.Name, ((IList)variable.Value)?.Cast<JArray>().Select(item => item.ToObject<System.Int32>()).ToList());
                        break;
                    case "Boolean[]":
                        SetVariable(variable.Name, ((IList)variable.Value)?.Cast<JArray>().Select(item => item.ToObject<System.Boolean>()).ToList());
                        break;
                    case "Color[]":
                        SetVariable(variable.Name, ((IList)variable.Value).Cast<JArray>().Select(j => new Color((float)j[0], (float)j[1], (float)j[2], (float)j[3])).ToList());
                        break;
                    default:
                        break;
                }
            }
            return true;
        }
        
        /// <summary>
        /// Return ParameterDefinition as JSON format.
        /// ParameterDefinition is generated from the variables of VisualScripting.Variables attached in the GameObject.
        /// Float, Integer, String, Boolean, Color and list of them are only supported for custom parameters. 
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns>ParameterDefinition JSON</returns>
        public static string GetParameterDefinitionJson(GameObject gameObject)
        {
            var emptyJson = JsonConvert.SerializeObject(new ParameterDefinition()
            {
                VariableDefinition = new()
                {
                    Variables = new List<VariableClass>().ToArray()
                }
            });
            if (!gameObject.TryGetComponent<Variables>(out var variableComponent))
            {
                return emptyJson;
            }

            var variables = new List<VariableClass>();
            
            foreach (var d in variableComponent.declarations)
            {
                if(d.value == null ){continue;}

                var name = d.name;
                string type;
                object value;
                    
                if (IsList(d))
                {
                    type = GetVariableTypeString(d);
                    var values = new List<object>();
                    foreach (var v in (IList)d.value)
                    {
                        values.Add(CastVariableUnityToJson(v.GetType().ToString(), v));
                    }
                    value = values.ToArray();
                }
                else
                {
                    type = GetVariableTypeString(d);
                    value = CastVariableUnityToJson((d.value).GetType().ToString(), (d.value));
                }

                if(type == null){continue;}
                    
                variables.Add(new VariableClass { Type = type, Name = name, Value = value });
            }

            var variableDefinition = new VariableDefinitionClass { Variables = variables.ToArray() };

            var parameterDefinition = new ParameterDefinition { VariableDefinition = variableDefinition };
            
            string jsonText = JsonConvert.SerializeObject(parameterDefinition);
            return jsonText;
        }

        private static bool IsList(VariableDeclaration d)
        {
            return d.value is IList && d.value.GetType().IsGenericType &&
                   d.value.GetType().GetGenericTypeDefinition() == typeof(List<>);
        }

        private static string GetVariableTypeString(VariableDeclaration d)
        {
            string TypeFullName = (d.value).GetType().ToString();
            
            string type = TypeFullName switch
            {
                "System.Single" => "Float",
                "System.Int32" => "Integer",
                "System.String" => "String",
                "System.Boolean" => "Boolean",
                "UnityEngine.Color" => "Color",
                "System.Collections.Generic.List`1[System.Single]" => "Float[]",
                "System.Collections.Generic.List`1[System.Int32]" => "Integer[]",
                "System.Collections.Generic.List`1[System.String]" => "String[]",
                "System.Collections.Generic.List`1[System.Boolean]" => "Boolean[]",
                "System.Collections.Generic.List`1[UnityEngine.Color]" => "Color[]",
                _ => null,
            };
            return type;
        }

        private static object CastVariableUnityToJson(string typeFullNameInUnity, object value)
        {
            var ret = typeFullNameInUnity switch
            {
                "System.Single" => value,
                "System.Int32" => value,
                "System.String" => value,
                "System.Boolean" => value,
                "UnityEngine.Color" => new Func<Color, float[]>((Color color) => new float[] { color.r, color.g, color.b, color.a })((Color)value),
                _ => null,
            };
            return ret;
        }

    }
}
