using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Drawing;
using WClipboard.Core.DI;
using Xunit;

namespace WClipboard.Core.Tests.DI
{
    public class AppInfo : IAppInfo
    {
        public string Name { get; }

        public string Path { get; }

        public int ProcessId { get; }

        public IReadOnlyList<string> Args { get; }

        public Version Version { get; }

        public Icon Icon { get; }
    }

    public class DIContainerTests
    {
        [Fact]
        public void Should_Work()
        {
            // act dispose of none should work
            DiContainer.Dispose();

            // act setup
            var container = DiContainer.Setup();

            // assert Setup may not be double started
            Assert.Throws<InvalidOperationException>(() => DiContainer.Setup());

            // arrange startup
            var startupMock = new Mock<IStartup>();
            var disposableMock = new Mock<IDisposable>();

            startupMock.Setup(x => x.ConfigureServices(It.IsAny<IServiceCollection>(), It.IsAny<IStartupContext>()))
                .Callback<IServiceCollection, IStartupContext>((services, _) => services.AddSingleton(disposableMock.Object));

            // act add
            container.Add(startupMock.Object);

            // assert: Setup may not be double started
            Assert.Throws<InvalidOperationException>(() => DiContainer.Setup());

            // act build
            container.Build(new AppInfo());

            // assert SP should have value, startup executed and if it is injected
            Assert.NotNull(DiContainer.SP);
            startupMock.Verify(x => x.ConfigureServices(It.IsAny<IServiceCollection>(), It.IsAny<IStartupContext>()));
            Assert.Equal(disposableMock.Object, DiContainer.SP.GetService<IDisposable>());

            // assert Setup may not be started when completed
            Assert.Throws<InvalidOperationException>(() => DiContainer.Setup());

            // act dispose should work
            DiContainer.Dispose();
            disposableMock.Verify(x => x.Dispose());
        }

        public interface IA { }
        public interface IB { }
        public class C : IA, IB { }

        [Fact]
        public void Should_Be_Able_To_Register_Singleton_As_Multiple_Interfaces()
        {
            // act setup
            var container = DiContainer.Setup();

            // arrange startup
            var startupMock = new Mock<IStartup>();

            startupMock.Setup(x => x.ConfigureServices(It.IsAny<IServiceCollection>(), It.IsAny<IStartupContext>()))
                .Callback<IServiceCollection, IStartupContext>((services, _) => services.AddSingleton<IA, IB, C>());

            // act add
            container.Add(startupMock.Object);

            // act build
            container.Build(new AppInfo());

            // act get IA and IB
            var ia = DiContainer.SP.GetService<IA>();
            var ib = DiContainer.SP.GetService<IB>();

            DiContainer.Dispose();

            // assert that ia and ib are the same instance
            Assert.Same(ia, ib);
        }
    }
}
