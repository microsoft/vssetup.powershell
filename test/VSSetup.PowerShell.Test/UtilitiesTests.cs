// <copyright file="UtilitiesTests.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Configuration;
    using Moq;
    using Xunit;

    public class UtilitiesTests
    {
        [Fact]
        public void EmptyReadOnlyCollection_Empty()
        {
            Assert.Empty(Utilities.EmptyReadOnlyCollection<PackageReference>());
        }

        [Fact]
        public void EmptyReadOnlyCollection_Singleton()
        {
            Assert.Same(Utilities.EmptyReadOnlyCollection<PackageReference>(), Utilities.EmptyReadOnlyCollection<PackageReference>());
        }

        [Fact]
        public void GetAdaptedProperties_Action_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>("action", () => Utilities.GetAdaptedPackages<ISetupPackageReference, PackageReference>(null, null));
        }

        [Fact]
        public void GetAdaptedProperties_Creator_Null_Throws()
        {
            var references = new[]
            {
                Mock.Of<ISetupPackageReference>(x => x.GetId() == "a"),
                Mock.Of<ISetupPackageReference>(x => x.GetId() == "b"),
            };

            Assert.Throws<ArgumentNullException>("creator", () => Utilities.GetAdaptedPackages<ISetupPackageReference, PackageReference>(() => references, null));
        }

        [Fact]
        public void GetAdaptedProperties_Empty()
        {
            var references = new ISetupPackageReference[0];
            Func<ISetupPackageReference, PackageReference> creator = reference => PackageReferenceFactory.Create(reference);

            var packages = Utilities.GetAdaptedPackages(() => references, creator).ToList();

            Assert.Empty(packages);
        }

        [Fact]
        public void GetAdaptedProperties()
        {
            var references = new[]
            {
                Mock.Of<ISetupPackageReference>(x => x.GetId() == "a"),
                Mock.Of<ISetupPackageReference>(x => x.GetId() == "b"),
            };

            Func<ISetupPackageReference, PackageReference> creator = reference => PackageReferenceFactory.Create(reference);

            var packages = Utilities.GetAdaptedPackages(() => references, creator).ToList();

            Assert.Collection(
                packages,
                x => Assert.Equal("a", x.Id),
                x => Assert.Equal("b", x.Id));
        }

        [Fact]
        public void TryParseVersion_Success()
        {
            Version version;
            var success = Utilities.TryParseVersion("1.2.3", out version);
            Assert.True(success);
            Assert.Equal(new Version("1.2.3"), version);
        }

        [Fact]
        public void TryParseVersion_Failure()
        {
            Version version;
            var success = Utilities.TryParseVersion("xxx", out version);
            Assert.False(success);
            Assert.Equal(new Version(), version);
        }

        [Fact]
        public void TryParseVersion_Null_Yields_Null()
        {
            Version version;
            var success = Utilities.TryParseVersion(null, out version);
            Assert.False(success);
            Assert.Equal(null, version);
        }

        [Fact]
        public void TryParseVersion_Empty_String_Yields_Null()
        {
            Version version;
            var success = Utilities.TryParseVersion(string.Empty, out version);
            Assert.False(success);
            Assert.Equal(null, version);
        }

        [Fact]
        public void TrySet_PropertyName_Null_Throws()
        {
            string property = null;
            Assert.Throws<ArgumentNullException>("propertyName", () => Utilities.TrySet(ref property, null, null));
        }

        [Fact]
        public void TrySet_PropertyName_Empty_Throws()
        {
            string property = null;
            Assert.Throws<ArgumentException>("propertyName", () => Utilities.TrySet(ref property, string.Empty, null));
        }

        [Fact]
        public void TrySet_Action_Null_Throws()
        {
            string property = null;
            Assert.Throws<ArgumentNullException>("action", () => Utilities.TrySet(ref property, nameof(property), null));
        }

        [Fact]
        public void TrySet_COMException_E_NOTFOUND()
        {
            string property = null;
            Utilities.TrySet(ref property, nameof(property), () => { throw new COMException("Not found", NativeMethods.E_NOTFOUND); });

            Assert.Null(property);
        }

        [Fact]
        public void TrySet_COMException_REGDB_E_CLASSNOTREG()
        {
            string property = null;
            var ex = Assert.Throws<COMException>(() => Utilities.TrySet(ref property, nameof(property), () => { throw new COMException("Not registered", NativeMethods.REGDB_E_CLASSNOTREG); }));

            Assert.Equal(NativeMethods.REGDB_E_CLASSNOTREG, ex.ErrorCode);
        }

        [Fact]
        public void TrySet()
        {
            string property = null;
            Utilities.TrySet(ref property, nameof(property), () => { return "test"; });

            Assert.Equal("test", property);
        }

        [Fact]
        public void TrySet_Error_Callback()
        {
            string property = null;
            var called = false;

            Utilities.TrySet(
                ref property,
                nameof(property),
                () => { throw new COMException("Not found", NativeMethods.E_NOTFOUND); },
                propertyName => called = true);

            Assert.Null(property);
            Assert.True(called);
        }

        [Fact]
        public void TrySetCollection_PropertyName_Null_Throws()
        {
            IList<PackageReference> property = null;
            Assert.Throws<ArgumentNullException>("propertyName", () => Utilities.TrySetCollection<ISetupPackageReference, PackageReference>(ref property, null, null, null));
        }

        [Fact]
        public void TrySetCollection_PropertyName_Empty_Throws()
        {
            IList<PackageReference> property = null;
            Assert.Throws<ArgumentException>("propertyName", () => Utilities.TrySetCollection<ISetupPackageReference, PackageReference>(ref property, string.Empty, null, null));
        }

        [Fact]
        public void TrySetCollection_Action_Null_Throws()
        {
            IList<PackageReference> property = null;
            Assert.Throws<ArgumentNullException>("action", () => Utilities.TrySetCollection<ISetupPackageReference, PackageReference>(ref property, nameof(property), null, null));
        }

        [Fact]
        public void TrySetCollection_Creator_Null_Throws()
        {
            IList<PackageReference> property = null;
            var references = new[]
            {
                Mock.Of<ISetupPackageReference>(x => x.GetId() == "a"),
                Mock.Of<ISetupPackageReference>(x => x.GetId() == "b"),
            };

            Assert.Throws<ArgumentNullException>("creator", () => Utilities.TrySetCollection(ref property, nameof(property), () => references, null));
        }

        [Fact]
        public void TrySetCollection_Empty()
        {
            IList<PackageReference> property = null;
            var references = new ISetupPackageReference[0];
            Func<ISetupPackageReference, PackageReference> creator = reference => PackageReferenceFactory.Create(reference);

            var packages = Utilities.TrySetCollection(ref property, nameof(property), () => references, creator);

            Assert.Empty(packages);
        }

        [Fact]
        public void TrySetCollection()
        {
            IList<PackageReference> property = null;
            var references = new[]
            {
                Mock.Of<ISetupPackageReference>(x => x.GetId() == "a"),
                Mock.Of<ISetupPackageReference>(x => x.GetId() == "b"),
            };

            Func<ISetupPackageReference, PackageReference> creator = reference => PackageReferenceFactory.Create(reference);

            var packages = Utilities.TrySetCollection(ref property, nameof(property), () => references, creator);

            Assert.Collection(
                packages,
                x => Assert.Equal("a", x.Id),
                x => Assert.Equal("b", x.Id));
        }

        [Fact]
        public void TrySetCollection_Error_Callback()
        {
            IList<PackageReference> property = null;
            var called = false;
            var references = new[]
            {
                Mock.Of<ISetupPackageReference>(x => x.GetId() == "a"),
                Mock.Of<ISetupPackageReference>(x => x.GetId() == "b"),
            };

            Func<ISetupPackageReference, PackageReference> creator = reference => PackageReferenceFactory.Create(reference);

            var packages = Utilities.TrySetCollection(
                ref property,
                nameof(property),
                () => { throw new COMException("Not found", NativeMethods.E_NOTFOUND); },
                creator,
                propertyName => called = true);

            Assert.Null(property);
            Assert.Empty(packages);
            Assert.True(called);
        }
    }
}
