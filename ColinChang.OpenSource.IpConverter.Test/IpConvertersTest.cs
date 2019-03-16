using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ColinChang.OpenSource.IpConverter.Test
{
    public class IpConvertersTest : IClassFixture<IpConvertersFixture>
    {
        private readonly IpConvertersFixture _fixture;

        public IpConvertersTest(IpConvertersFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void ToNumberTest()
        {
            const string invalid = "invalid ip";
            Assert.Throws<ArgumentException>(() => invalid.ToIpNumber());

            Assert.Equal(_fixture.MinIpNumber, _fixture.MinIp.ToIpNumber());
            Assert.Equal(_fixture.MaxIpNumber, _fixture.MaxIp.ToIpNumber());
        }

        [Fact]
        public void ToIpAddressTest()
        {
            Assert.Equal(_fixture.MinIp, _fixture.MinIpNumber.ToIpAddress());
            Assert.Equal(_fixture.MaxIp, _fixture.MaxIpNumber.ToIpAddress());
        }

        [Fact]
        public void ToIpAndMaskTest()
        {
            Assert.Equal(_fixture.IpMask, IpConverter.ToIpAndMask(_fixture.StartIp, _fixture.EndIp).FirstOrDefault());
        }

        [Fact]
        public void ToIpPeriodTest()
        {
            var (s, e) = _fixture.IpMask.ToIpPeriod();

            Assert.Equal(_fixture.StartIp, s);
            Assert.Equal(_fixture.EndIp, e);
        }

        [Fact]
        public void ToIpListTest()
        {
            var ips = _fixture.IpMask.ToIpList();

            Assert.Equal(_fixture.StartIp, ips.FirstOrDefault());
            Assert.Equal(_fixture.EndIp, ips.LastOrDefault());
        }
    }

    public class IpConvertersFixture
    {
        public string MinIp { get; set; } = "0.0.0.0";
        public uint MinIpNumber { get; set; } = 0;
        public string MaxIp { get; set; } = "255.255.255.255";
        public uint MaxIpNumber { get; set; } = 4294967295;

        public string StartIp { get; set; } = "192.168.0.0";
        public string EndIp { get; set; } = "192.168.0.255";
        public string IpMask { get; set; } = "192.168.0.0/24";
    }
}