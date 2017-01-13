// <copyright file="ExtensionsTests.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup.PowerShell
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Xunit;

    public class ExtensionsTests
    {
#pragma warning disable SA1401 // Fields must be private
        public static IEnumerable<object[]> ContainsAllExceptionCases = new[]
        {
            new object[] { "source", null, null, null },
            new object[] { "selector", new[] { new Tuple<string, int>("a", 1) }, null, null },
            new object[] { "keys", new[] { new Tuple<string, int>("a", 1) }, new Func<Tuple<string, int>, string>(item => item.Item1), null },
        };
#pragma warning restore SA1401 // Fields must be private

        [Theory]
        [MemberData(nameof(ContainsAllExceptionCases))]
        public void ContainsAll_Null_Throws(string paramName, IEnumerable<Tuple<string, int>> source, Func<Tuple<string, int>, string> selector, IEnumerable<string> keys)
        {
            Assert.Throws<ArgumentNullException>(paramName, () => source.ContainsAll(selector, keys));
        }

        [Fact]
        public void ContainsAll_Empty_Source_Empty_Keys()
        {
            var items = new Tuple<string, int>[0];
            var keys = Enumerable.Empty<string>();

            Assert.True(items.ContainsAll(item => item.Item1, keys));
        }

        [Fact]
        public void ContainsAll_Empty_Source_With_Keys()
        {
            var items = new Tuple<string, int>[0];
            var keys = new[] { "a" };

            Assert.False(items.ContainsAll(item => item.Item1, keys));
        }

        [Fact]
        public void ContainsAll_With_Source_Empty_Keys()
        {
            var items = new[]
            {
                new { Key = "a", Value = 1 },
                new { Key = "b", Value = 2 },
                new { Key = "c", Value = 3 },
            };

            var keys = Enumerable.Empty<string>();

            Assert.True(items.ContainsAll(item => item.Key, keys));
        }

        [Fact]
        public void ContainsAll_Without_Keys()
        {
            var items = new[]
            {
                new { Key = "a", Value = 1 },
                new { Key = "b", Value = 2 },
                new { Key = "c", Value = 3 },
            };

            var keys = new[]
            {
                "a",
                "x",
            };

            Assert.False(items.ContainsAll(item => item.Key, keys));
        }

        [Fact]
        public void ContainsAll()
        {
            var items = new[]
            {
                new { Key = "a", Value = 1 },
                new { Key = "b", Value = 2 },
                new { Key = "c", Value = 3 },
            };

            var keys = new[]
            {
                "a",
                "c",
            };

            Assert.True(items.ContainsAll(item => item.Key, keys));
        }

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
