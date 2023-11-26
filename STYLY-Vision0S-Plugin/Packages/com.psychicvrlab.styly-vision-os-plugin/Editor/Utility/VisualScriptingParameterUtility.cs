using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System;
using Newtonsoft.Json;

namespace Styly.VisionOs.Plugin
{
    public class VisualScriptingParameterUtility
    {
        [Serializable]
        private class ParameterDefinition
        {
            public VariableDefinitionClass VariableDefinition { get; set; }
        }

        [Serializable]
        private class VariableDefinitionClass
        {
            public VariableClass[] Variables { get; set; }
        }

        [Serializable]
        private class VariableClass
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public object Value { get; set; }
        }

        /// <summary>
        /// Return ParameterDefinition as JSON format.
        /// ParameterDefinition is generated from the variables of VisualScripting.Variables attached in the GameObject.
        /// Float, Integer, String, Boolen, Color and list of them are only supported for custom parameters. 
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns>ParameterDefinition JSON</returns>

        public static string GetParameterDefinition(GameObject gameObject)
        {
            List<VariableClass> variables = new List<VariableClass>();
            if (gameObject.TryGetComponent<Variables>(out var VariableComponent))
            {
                var decs = VariableComponent.declarations;
                foreach (var d in decs)
                {
                    string name = d.name;
                    string type;
                    object value;

                    if (d.value is IList && d.value.GetType().IsGenericType && d.value.GetType().GetGenericTypeDefinition() == typeof(List<>))
                    {
                        type = GetVariableTypeString(((IList)d.value).GetType().ToString());
                        var values = new List<object>();
                        foreach (var v in (IList)d.value)
                        {
                            values.Add(SafeCastVariable(v.GetType().ToString(), v));
                        }
                        value = values.ToArray();
                    }
                    else
                    {
                        if (d.value == null)
                        {
                            type = null;
                            value = null;
                        }
                        else
                        {
                            type = GetVariableTypeString((d.value).GetType().ToString());
                            value = SafeCastVariable((d.value).GetType().ToString(), (d.value));
                        }
                    }
                    if (type != null) variables.Add(new VariableClass { Type = type, Name = name, Value = value });
                }
            }

            var variableDefinition = new VariableDefinitionClass { Variables = variables.ToArray() };
            var parameterDefinition = new ParameterDefinition { VariableDefinition = variableDefinition };

            string JsonText = JsonConvert.SerializeObject(parameterDefinition);
            Debug.Log(JsonText);
            return JsonText;
        }

        private static string GetVariableTypeString(string TypeFullName)
        {
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

        private static object SafeCastVariable(string TypeFullName, object value)
        {
            object ret = TypeFullName switch
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





        public static void SetParameterToVariables()
        {

        }




    }
}
