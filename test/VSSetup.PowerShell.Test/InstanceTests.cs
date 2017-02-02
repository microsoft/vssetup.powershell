// <copyright file="InstanceTests.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Configuration;
    using Moq;
    using Xunit;

    public class InstanceTests
    {
        private readonly Mock<ISetupInstance2> instance;
        private readonly Mock<ISetupPropertyStore> store;

        public InstanceTests()
        {
            instance = new Mock<ISetupInstance2>();
            store = instance.As<ISetupPropertyStore>();
        }

        [Fact]
        public void New_Instance_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>("instance", () => new Instance(null));
        }

        [Fact]
        public void New_Missing_InstanceId_Throws()
        {
            instance.Setup(x => x.GetInstanceId()).Throws(new COMException("Not found", NativeMethods.E_NOTFOUND));

            var ex = Assert.Throws<COMException>(() => new Instance(instance.Object));
            Assert.Equal(unchecked((int)0x80070490), ex.ErrorCode);
        }

        [Fact]
        public void New_Missing_InstallationName()
        {
            instance.Setup(x => x.GetInstanceId()).Returns("test");
            instance.Setup(x => x.GetInstallationName()).Throws(new COMException("Not found", NativeMethods.E_NOTFOUND));

            var sut = new Instance(instance.Object);
            Assert.Null(sut.InstallationName);
        }

        [Fact]
        public void New_Null_InstallationVersion()
        {
            instance.Setup(x => x.GetInstanceId()).Returns("test");
            instance.Setup(x => x.GetInstallationVersion()).Returns<string>(null);

            var sut = new Instance(instance.Object);
            Assert.Null(sut.InstallationVersion);
        }

        [Fact]
        public void New_Invalid_InstallationVersion()
        {
            instance.Setup(x => x.GetInstanceId()).Returns("test");
            instance.Setup(x => x.GetInstallationVersion()).Returns("invalid");

            var sut = new Instance(instance.Object);
            Assert.Null(sut.InstallationVersion);
        }

        [Fact]
        public void New_InstallationVersion()
        {
            instance.Setup(x => x.GetInstanceId()).Returns("test");
            instance.Setup(x => x.GetInstallationVersion()).Returns("1.2.3.4");

            var sut = new Instance(instance.Object);
            Assert.Equal(new Version(1, 2, 3, 4), sut.InstallationVersion);
        }

        [Fact]
        public void New_Product_Null()
        {
            instance.Setup(x => x.GetInstanceId()).Returns("test");

            var sut = new Instance(instance.Object);
            Assert.Null(sut.Product);
        }

        [Fact]
        public void New_Product()
        {
            var product = Mock.Of<ISetupPackageReference>(x => x.GetId() == "product");

            instance.Setup(x => x.GetInstanceId()).Returns("test");
            instance.Setup(x => x.GetProduct()).Returns(product);

            var sut = new Instance(instance.Object);
            Assert.NotNull(sut.Product);
            Assert.Equal("product", sut.Product.Id);
        }

        [Fact]
        public void New_Packages_Null()
        {
            instance.Setup(x => x.GetInstanceId()).Returns("test");

            var sut = new Instance(instance.Object);
            Assert.Empty(sut.Packages);
        }

        [Fact]
        public void New_Packages_Empty()
        {
            instance.Setup(x => x.GetInstanceId()).Returns("test");
            instance.Setup(x => x.GetPackages()).Returns(Enumerable.Empty<ISetupPackageReference>().ToArray());

            var sut = new Instance(instance.Object);
            Assert.Empty(sut.Packages);
        }

        [Fact]
        public void New_Packages()
        {
            var references = new[]
            {
                Mock.Of<ISetupPackageReference>(x => x.GetId() == "a"),
                Mock.Of<ISetupPackageReference>(x => x.GetId() == "b"),
            };

            instance.Setup(x => x.GetInstanceId()).Returns("test");
            instance.Setup(x => x.GetPackages()).Returns(references);

            var sut = new Instance(instance.Object);
            Assert.Collection(
                sut.Packages,
                x => Assert.Equal("a", x.Id),
                x => Assert.Equal("b", x.Id));
        }

        [Fact]
        public void New_Copies_AdditionalProperties()
        {
            instance.Setup(x => x.GetInstanceId()).Returns("test");
            store.Setup(x => x.GetNames()).Returns(new[] { "a", "b" });
            store.Setup(x => x.GetValue("a")).Returns(1);
            store.Setup(x => x.GetValue("b")).Returns(2);

            var sut = new Instance(instance.Object);
            Assert.Equal(1, sut.AdditionalProperties["A"]);
            Assert.Equal(1, sut.AdditionalProperties["a"]);
            Assert.Equal(2, sut.AdditionalProperties["B"]);
            Assert.Equal(2, sut.AdditionalProperties["b"]);
        }

        [Fact]
        public void New_No_Errors()
        {
            instance.Setup(x => x.GetInstanceId()).Returns("test");

            var sut = new Instance(instance.Object);

            Assert.Null(sut.Errors);
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

            instance.Setup(x => x.GetInstanceId()).Returns("test");
            instance.Setup(x => x.GetErrors()).Returns(errors.Object);

            var sut = new Instance(instance.Object);

            Assert.NotNull(sut.Errors);
            Assert.Collection(
                sut.Errors.FailedPackages,
                x => Assert.Equal("a", x.Id),
                x => Assert.Equal("b", x.Id));
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

            instance.Setup(x => x.GetInstanceId()).Returns("test");
            instance.Setup(x => x.GetErrors()).Returns(errors.Object);

            var sut = new Instance(instance.Object);

            Assert.NotNull(sut.Errors);
            Assert.Collection(
                sut.Errors.SkippedPackages,
                x => Assert.Equal("a", x.Id),
                x => Assert.Equal("b", x.Id));
        }
    }
}
