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
        private static readonly string ObjectTypeName = typeof(object).FullName;
        private static readonly string StringTypeName = typeof(string).FullName;
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
            return property.TypeName ?? ObjectTypeName;
        }

        /// <inheritdoc/>
        public override object GetPropertyValue(PSAdaptedProperty adaptedProperty)
        {
            var instance = adaptedProperty.BaseObject as Instance;
            if (instance != null)
            {
                if (string.Equals(adaptedProperty.Name, PSPathPropertyName, StringComparison.OrdinalIgnoreCase))
                {
                    return GetPSPath(instance);
                }

                var property = adaptedProperty.Tag as InstanceProperty;
                return property?.Property?.GetValue(instance, null);
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
            yield return new PSAdaptedProperty("PSPath", new InstanceProperty { TypeName = StringTypeName });

            var properties = instance.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                var tag = new InstanceProperty { TypeName = property.PropertyType.FullName, Property = property };
                yield return new PSAdaptedProperty(property.Name, tag);
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
            public string TypeName { get; set; }

            public PropertyInfo Property { get; set; }
        }
    }
}
