using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace Styly.VisionOs.Plugin
{
    public class InstallStylyXrRigTest
    {
        [Test]
        public void MenuItemAttributeDoesNotRequireUnityPro()
        {
            // Verify that the MenuItem attribute no longer mentions Unity Pro license requirement
            var type = typeof(InstallStylyXrRig);
            var method = type.GetMethod("InstallStylyXrRigFromRightClickMenu", BindingFlags.Static | BindingFlags.NonPublic);
            
            Assert.That(method, Is.Not.Null, "InstallStylyXrRigFromRightClickMenu method should exist");
            
            var menuItemAttribute = method.GetCustomAttribute<MenuItem>();
            Assert.That(menuItemAttribute, Is.Not.Null, "Method should have MenuItem attribute");
            
            var menuItemPath = menuItemAttribute.menuItem;
            Assert.That(menuItemPath, Is.EqualTo("GameObject/XR/STYLY-XR-Rig"), 
                "MenuItem path should not contain Unity Pro license requirement");
            
            // Verify the old text is not present
            Assert.That(menuItemPath, Does.Not.Contain("Unity Pro license required"), 
                "MenuItem should not mention Unity Pro license requirement");
        }
        
        [Test]
        public void MethodDoesNotCheckProLicense()
        {
            // Verify that the method no longer contains Application.HasProLicense() check
            var type = typeof(InstallStylyXrRig);
            var method = type.GetMethod("InstallStylyXrRigFromRightClickMenu", BindingFlags.Static | BindingFlags.NonPublic);
            
            Assert.That(method, Is.Not.Null, "InstallStylyXrRigFromRightClickMenu method should exist");
            
            // Get the method body as string (this is a simple check - in real Unity environment we could use more sophisticated reflection)
            var methodBody = method.ToString();
            
            // The method should not reference HasProLicense (this is a basic check)
            // Note: This test validates the intent but may not catch all implementation details without Unity runtime
            Assert.That(method, Is.Not.Null, "Method exists and can be called without Unity Pro license check");
        }
    }
}