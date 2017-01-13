// <copyright file="ExtensionsTests.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup.PowerShell
{
    using System;
    using System.Management.Automation;
    using Xunit;

    public class ExtensionsTests
    {
        [Fact]
        public void GetPropertyValue_Object_Null_Throws()
        {
            PSObject sut = null;
            Assert.Throws<ArgumentNullException>("object", () => sut.GetPropertyValue<string>("Foo"));
        }

        [Fact]
        public void GetPropertyValue_PropertyName_Null_Throws()
        {
            var sut = new PSObject();
            Assert.Throws<ArgumentNullException>("propertyName", () => sut.GetPropertyValue<string>(null));
        }

        [Fact]
        public void GetPropertyValue_PropertyName_Empty_Throws()
        {
            var sut = new PSObject();
            Assert.Throws<ArgumentException>("propertyName", () => sut.GetPropertyValue<string>(string.Empty));
        }

        [Fact]
        public void GetPropertyValue_PropertyName_Undefined_Returns_Default()
        {
            var sut = new PSObject();
            Assert.Equal(null, sut.GetPropertyValue<string>("Foo"));
        }

        [Fact]
        public void GetPropertyValue_PropertyName_Undefined_Returns_Custom()
        {
            var sut = new PSObject();
            Assert.Equal("Bar", sut.GetPropertyValue("Foo", "Bar"));
        }

        [Fact]
        public void GetPropertyValue_String()
        {
            var sut = new PSObject(new { Foo = "Bar" });
            Assert.Equal("Bar", sut.GetPropertyValue<string>("Foo"));
        }

        [Fact]
        public void GetPropertyValue_Integer_Converted()
        {
            var sut = new PSObject(new { Foo = "1" });
            Assert.Equal(1, sut.GetPropertyValue<int>("Foo"));
        }

        [Fact]
        public void GetPropertyValue_Integer()
        {
            var sut = new PSObject(new { Bar = 1 });
            Assert.Equal(1, sut.GetPropertyValue<int>("Bar"));
        }
    }
}
