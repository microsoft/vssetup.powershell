// <copyright file="ErrorsTests.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Setup.Configuration;
    using Moq;
    using Xunit;

    public class ErrorsTests
    {
        [Fact]
        public void New_Errors_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>("errors", () => new Errors(null));
        }

        [Fact]
        public void New_Missing_FailedPackages()
        {
            var errors = new Mock<ISetupErrorState>();
            errors.Setup(x => x.GetFailedPackages()).Throws(new COMException("Not found", NativeMethods.E_NOTFOUND));

            var sut = new Errors(errors.Object);

            Assert.Empty(sut.FailedPackages);
        }

        [Fact]
        public void New_FailedPackages()
        {
            var a = new Mock<ISetupFailedPackageReference>();
            a.As<ISetupPackageReference>().Setup(x => x.GetId()).Returns("a");

            var b = new Mock<ISetupFailedPackageReference>();
            b.As<ISetupPackageReference>().Setup(x => x.GetId()).Returns("b");

            var errors = new Mock<ISetupErrorState>();
            errors.Setup(x => x.GetFailedPackages()).Returns(new[] { a.Object, b.Object });

            var sut = new Errors(errors.Object);

            Assert.NotNull(sut.FailedPackages);
            Assert.Collection(
                sut.FailedPackages,
                x => Assert.Equal("a", x.Id),
                x => Assert.Equal("b", x.Id));
        }

        [Fact]
        public void New_Missing_SkippedPackages()
        {
            var errors = new Mock<ISetupErrorState>();
            errors.Setup(x => x.GetSkippedPackages()).Throws(new COMException("Not found", NativeMethods.E_NOTFOUND));

            var sut = new Errors(errors.Object);

            Assert.Empty(sut.SkippedPackages);
        }

        [Fact]
        public void New_SkippedPackages()
        {
            var skippedPackages = new[]
            {
                Mock.Of<ISetupPackageReference>(x => x.GetId() == "a"),
                Mock.Of<ISetupPackageReference>(x => x.GetId() == "b"),
            };

            var errors = new Mock<ISetupErrorState>();
            errors.Setup(x => x.GetSkippedPackages()).Returns(skippedPackages);

            var sut = new Errors(errors.Object);

            Assert.NotNull(sut.SkippedPackages);
            Assert.Collection(
                sut.SkippedPackages,
                x => Assert.Equal("a", x.Id),
                x => Assert.Equal("b", x.Id));
        }
    }
}
