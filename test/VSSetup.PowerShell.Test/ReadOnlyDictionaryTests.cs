// <copyright file="ReadOnlyDictionaryTests.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class ReadOnlyDictionaryTests
    {
        private readonly IDictionary<string, int> dictionary;

        public ReadOnlyDictionaryTests()
        {
            dictionary = new Dictionary<string, int>()
            {
                ["A"] = 1,
                ["B"] = 2,
            };
        }

        [Fact]
        public void New_Null_Dictionary_Throws()
        {
            Assert.Throws<ArgumentNullException>("dictionary", () => new ReadOnlyDictionary<string, int>(null));
        }

        [Fact]
        public void Count_Updates()
        {
            var sut = new ReadOnlyDictionary<string, int>(dictionary);
            Assert.Equal(2, sut.Count);

            dictionary.Add("C", 3);
            Assert.Equal(3, sut.Count);
        }

        [Fact]
        public void Keys_Updates()
        {
            var sut = new ReadOnlyDictionary<string, int>(dictionary);
            Assert.Contains("A", sut.Keys);
            Assert.Contains("B", sut.Keys);
            Assert.DoesNotContain("C", sut.Keys);

            dictionary.Add("C", 3);
            Assert.Contains("C", sut.Keys);
        }

        [Fact]
        public void Values_Updates()
        {
            var sut = new ReadOnlyDictionary<string, int>(dictionary);
            Assert.Contains(1, sut.Values);
            Assert.Contains(2, sut.Values);
            Assert.DoesNotContain(3, sut.Values);

            dictionary.Add("C", 3);
            Assert.Contains(3, sut.Values);
        }

        [Fact]
        public void Add_Throws()
        {
            var sut = new ReadOnlyDictionary<string, int>(dictionary) as IDictionary<string, int>;
            Assert.Throws<NotSupportedException>(() => sut.Add("C", 3));
        }

        [Fact]
        public void Collection_Add_Throws()
        {
            var sut = new ReadOnlyDictionary<string, int>(dictionary) as ICollection<KeyValuePair<string, int>>;
            Assert.Throws<NotSupportedException>(() => sut.Add(new KeyValuePair<string, int>("C", 3)));
        }

        [Fact]
        public void Clear_Throws()
        {
            var sut = new ReadOnlyDictionary<string, int>(dictionary) as IDictionary<string, int>;
            Assert.Throws<NotSupportedException>(() => sut.Clear());
        }

        [Fact]
        public void Remove_Throws()
        {
            var sut = new ReadOnlyDictionary<string, int>(dictionary) as IDictionary<string, int>;
            Assert.Throws<NotSupportedException>(() => sut.Remove("C"));
        }

        [Fact]
        public void Collection_Remove_Throws()
        {
            var sut = new ReadOnlyDictionary<string, int>(dictionary) as ICollection<KeyValuePair<string, int>>;
            Assert.Throws<NotSupportedException>(() => sut.Remove(new KeyValuePair<string, int>("C", 3)));
        }

        [Fact]
        public void Item_Set_Throws()
        {
            var sut = new ReadOnlyDictionary<string, int>(dictionary);
            Assert.Equal(1, sut["A"]);
            Assert.Throws<NotSupportedException>(() => sut["C"] = 3);
        }
    }
}
