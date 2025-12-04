using System.Collections.Generic;
using UnityEngine;

namespace Styly.VisionOs.Plugin.Validation
{
    public class PrefabValidationManager
    {
        private List<IPrefabValidator> validators = new List<IPrefabValidator>();

        /// <summary>
        /// Method to add a validation item
        /// </summary>
        /// <param name="validator"></param>        
        public void AddValidator(IPrefabValidator validator)
        {
            validators.Add(validator);
        }

        /// <summary>
        /// Performs all verifications and returns final results
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public bool ValidateAll(GameObject prefab)
        {
            bool allPassed = true;  // Whether all verifications passed

            foreach (var validator in validators)
            {
                bool result = validator.Validate(prefab);
                if (!result)
                {
                    allPassed = false;  // If any of them fail, set the flag to false.
                }
            }

            return allPassed;
        }
    }
}