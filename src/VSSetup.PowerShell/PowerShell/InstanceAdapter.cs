// <copyright file="InstanceAdapter.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup.PowerShell
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using System.Reflection;
    using System.Threading;

    /// <summary>
    /// Gets all properties set on the <see cref="Instance"/> including custom PowerShell properties.
    /// </summary>
    public class InstanceAdapter : PSPropertyAdapter
    {
        private static readonly string PSPathPropertyName = "PSPath";
        private static readonly string FileSystemProviderPrefix = @"Microsoft.PowerShell.Core\FileSystem::";

        private PropertySet properties = null;

        /// <inheritdoc/>
        public override Collection<PSAdaptedProperty> GetProperties(object baseObject)
        {
            var instance = baseObject as Instance;
            if (instance != null)
            {
                EnsureProperties(instance);
                return new Collection<PSAdaptedProperty>(properties);
            }

            return null;
        }

        /// <inheritdoc/>
        public override PSAdaptedProperty GetProperty(object baseObject, string propertyName)
        {
            var instance = baseObject as Instance;
            if (instance != null)
            {
                EnsureProperties(instance);

                // Clone the property so the right property value is retrieve.d
                var property = properties.TryGet(propertyName);
                if (property != null)
                {
                    return new PSAdaptedProperty(propertyName, property.Tag);
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public override string GetPropertyTypeName(PSAdaptedProperty adaptedProperty)
        {
            var property = adaptedProperty.Tag as InstanceProperty;
            var type = property?.Type ?? typeof(object);

            // TODO: If upgrading to newer PowerShell version use LanguagePrimitives.ConvertTypeNameToPSTypeName.
            return type.FullName;
        }

        /// <inheritdoc/>
        public override object GetPropertyValue(PSAdaptedProperty adaptedProperty)
        {
            var instance = adaptedProperty.BaseObject as Instance;
            if (instance != null)
            {
                var name = adaptedProperty.Name;
                if (string.Equals(name, PSPathPropertyName, StringComparison.OrdinalIgnoreCase))
                {
                    return GetPSPath(instance);
                }

                object value = null;

                var property = adaptedProperty.Tag as InstanceProperty;
                if (property?.Property != null)
                {
                    return property.Property.GetValue(instance, null);
                }
                else if (instance.AdditionalProperties.TryGetValue(name, out value))
                {
                    return value;
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public override bool IsGettable(PSAdaptedProperty adaptedProperty)
        {
            return true;
        }

        /// <inheritdoc/>
        public override bool IsSettable(PSAdaptedProperty adaptedProperty)
        {
            return false;
        }

        /// <inheritdoc/>
        public override void SetPropertyValue(PSAdaptedProperty adaptedProperty, object value)
        {
            throw new NotSupportedException();
        }

        private void EnsureProperties(Instance instance)
        {
            LazyInitializer.EnsureInitialized(ref properties, () =>
            {
                var properties = new PropertySet();
                foreach (var property in GetProperties(instance))
                {
                    properties.Add(property);
                }

                return properties;
            });
        }

        private IEnumerable<PSAdaptedProperty> GetProperties(Instance instance)
        {
            yield return new PSAdaptedProperty("PSPath", new InstanceProperty { Type = typeof(string) });

            var properties = instance.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                var tag = new InstanceProperty { Type = property.PropertyType, Property = property };
                yield return new PSAdaptedProperty(property.Name, tag);
            }

            foreach (var keyValue in instance.AdditionalProperties)
            {
                var tag = new InstanceProperty { Type = keyValue.Value?.GetType() ?? typeof(object) };
                yield return new PSAdaptedProperty(keyValue.Key, tag);
            }
        }

        private string GetPSPath(Instance instance)
        {
            if (!string.IsNullOrEmpty(instance.InstallationPath))
            {
                return FileSystemProviderPrefix + instance.InstallationPath;
            }

            return null;
        }

        private class InstanceProperty
        {
            public Type Type { get; set; }

            public PropertyInfo Property { get; set; }
        }
    }
}
