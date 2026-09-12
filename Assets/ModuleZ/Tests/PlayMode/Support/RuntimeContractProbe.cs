using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace ModuleZ.Tests.PlayMode.Support
{
    internal static class RuntimeContractProbe
    {
        private const BindingFlags PublicStatic =
            BindingFlags.Public | BindingFlags.Static;
        private const BindingFlags PublicInstance =
            BindingFlags.Public | BindingFlags.Instance;

        public static Type RequireType(string fullName)
        {
            Type type = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType(fullName, false))
                .FirstOrDefault(candidate => candidate != null);

            Assert.That(type, Is.Not.Null, $"Production type not found: {fullName}");
            return type;
        }

        public static Component[] FindSceneComponents(string fullName)
        {
            Type type = RequireType(fullName);
            return Resources.FindObjectsOfTypeAll(type)
                .OfType<Component>()
                .Where(component =>
                    component != null &&
                    component.gameObject.scene.IsValid() &&
                    component.gameObject.scene.isLoaded)
                .ToArray();
        }

        public static object GetPublicInstanceProperty(
            object target,
            string propertyName)
        {
            Assert.That(target, Is.Not.Null);
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                PublicInstance
            );
            Assert.That(property, Is.Not.Null,
                $"Public property not found: {target.GetType().FullName}.{propertyName}");
            return property.GetValue(target);
        }

        public static object GetPublicStaticMember(Type type, string memberName)
        {
            PropertyInfo property = type.GetProperty(memberName, PublicStatic);
            if (property != null)
                return property.GetValue(null);

            FieldInfo field = type.GetField(memberName, PublicStatic);
            Assert.That(field, Is.Not.Null,
                $"Public static member not found: {type.FullName}.{memberName}");
            return field.GetValue(null);
        }

        public static object InvokePublicStatic(
            Type type,
            string methodName,
            params object[] arguments)
        {
            MethodInfo method = type.GetMethod(methodName, PublicStatic);
            Assert.That(method, Is.Not.Null,
                $"Public static method not found: {type.FullName}.{methodName}");
            return method.Invoke(null, arguments);
        }

        public static string GetStateName(Component root)
        {
            object state = GetPublicInstanceProperty(root, "State");
            return state?.ToString() ?? "<null>";
        }
    }
}
