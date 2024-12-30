using WireMock.Server;
using Xunit.Abstractions;

namespace LocalAI.NET.Oobabooga.Tests.Common
{
    public abstract class UnitTestBase : TestBase
    {
        protected readonly WireMockServer Server;
        protected readonly string BaseUrl;

        protected UnitTestBase(ITestOutputHelper output) : base(output)
        {
            Server = WireMockServer.Start();
            BaseUrl = Server.Urls[0];
        }

        public override void Dispose()
        {
            Server?.Dispose();
            base.Dispose();
        }
    }
}