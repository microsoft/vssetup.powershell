// <copyright file="InstanceComparerTests.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices.ComTypes;
    using Microsoft.VisualStudio.Setup.Configuration;
    using Moq;
    using Xunit;

    public class InstanceComparerTests
    {
        public static IEnumerable<object[]> GetCompareVersionAndDateData()
        {
            var empty = new string[0];

            var v1 = new Version(1, 0);
            var v2 = new Version(2, 0);

            var ft1 = new FILETIME { dwHighDateTime = 1 };
            var ft2 = new FILETIME { dwHighDateTime = 2 };

            Instance Mock(Version v = null, FILETIME ft = default(FILETIME))
            {
                var mock = new Mock<ISetupInstance2>();
                mock.As<ISetupPropertyStore>().Setup(x => x.GetNames()).Returns(empty);

                mock.Setup(x => x.GetInstallationVersion()).Returns(v?.ToString());
                mock.Setup(x => x.GetInstallDate()).Returns(ft);

                return new Instance(mock.Object);
            }

            return new[]
            {
                new object[] { null, null, 0 },
                new object[] { null, Mock(), -1 },
                new object[] { Mock(), null, 1 },
                new object[] { Mock(), Mock(v1), -1 },
                new object[] { Mock(v1), Mock(), 1 },
                new object[] { Mock(v1), Mock(v1), 0 },
                new object[] { Mock(v1), Mock(v2), -1 },
                new object[] { Mock(v2), Mock(v1), 1 },
                new object[] { Mock(v2, ft1), Mock(v2, ft2), -1 },
                new object[] { Mock(v2, ft2), Mock(v2, ft1), 1 },
            };
        }

        [Theory]
        [MemberData(nameof(GetCompareVersionAndDateData))]
        public void CompareVersionAndDate(Instance x, Instance y, int expected)
        {
            var actual = InstanceComparer.VersionAndDate.Compare(x, y);
            Assert.Equal(expected, actual);
        }
    }
}
