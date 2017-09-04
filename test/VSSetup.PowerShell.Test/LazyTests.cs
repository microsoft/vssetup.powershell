// <copyright file="LazyTests.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using Moq;
    using Xunit;

    public class LazyTests
    {
        [Fact]
        public void New_Factory_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>("factory", () => new Lazy<string>(null));
        }

        [Fact]
        public void Value_Initializes()
        {
            var sut = new Lazy<string>(() => new string(new[] { 't', 'e', 's', 't' }));
            Assert.False(sut.HasValue);

            Assert.Equal("test", sut.Value);
            Assert.True(sut.HasValue);
        }

        [Fact]
        public void Disposes()
        {
            var disposable = new Mock<IDisposable>();
            using (var sut = new Lazy<IDisposable>(() => disposable.Object))
            {
                Assert.NotNull(sut.Value);
                Assert.True(sut.HasValue);
            }

            disposable.Verify(x => x.Dispose(), Times.Once);
        }

        [Fact]
        public void No_Initialize_No_Dispose()
        {
            var disposable = new Mock<IDisposable>();
            using (var sut = new Lazy<IDisposable>(() => disposable.Object))
            {
                Assert.False(sut.HasValue);
            }

            disposable.Verify(x => x.Dispose(), Times.Never);
        }
    }
}
