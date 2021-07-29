using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.Utilities;
using WClipboard.Core.WPF.Clipboard;
using WClipboard.Core.WPF.Clipboard.Filter;
using WClipboard.Core.WPF.Clipboard.Format;
using WClipboard.Core.WPF.Clipboard.Trigger;
using Xunit;

using SysClipboard = System.Windows.Clipboard;

namespace WClipboard.Core.WPF.Tests.Managers
{
    public class ClipboardObjectsManagerTests
    {
        private readonly Mock<IFormatsExtractor> _formatsExtractor;
        private readonly Mock<IClipboardFilter> _clipboardFilter;
        private readonly Mock<IClipboardObjectManager> _clipboardObjectManager;
        private readonly Mock<IClipboardObjectsListener> _listenerMock;
        private readonly Mock<IServiceProvider> _serviceProviderMock;

        public ClipboardObjectsManagerTests()
        {
            _formatsExtractor = new();
            _clipboardFilter = new();
            _clipboardObjectManager = new();
            _listenerMock = new();
            _serviceProviderMock = new();

            _serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ILogger<>)))).Returns((Type t) => MockCreator.CreateMock(typeof(ILogger<>), t.GetGenericArguments()[0]).Object);
        }

        [Fact]
        public async Task Should_Not_Create_ClipboardObject_If_Clipboard_Empty()
        {
            //arrange
            await RunOnSTA(() =>
            {
                SysClipboard.Clear();
            });

            //act
            using (var sut = new ClipboardObjectsManager(_clipboardObjectManager.Object, new[] { _formatsExtractor.Object }, new[] { _clipboardFilter.Object }, _serviceProviderMock.Object))
            {
                sut.AddListener(_listenerMock.Object);
                sut.AddTriggerToQueue();

                await Task.Delay(100);
            }

            //assert
            _listenerMock.Verify(x => x.OnResolvedTrigger(It.IsAny<ResolvedClipboardTrigger>()), Times.Never());
            _listenerMock.Verify(x => x.IsInterestedIn(It.IsAny<ClipboardObject>()), Times.Never());

            _clipboardObjectManager.Verify(x => x.AddImplementationsAsync(It.IsAny<ClipboardObject>(), It.IsAny<IEnumerable<EqualtableFormat>>()), Times.Never());
        }

        [Fact]
        public async Task Should_Ignore_Unknown_Format()
        {
            //arrange
            await RunOnSTA(() =>
            {
                SysClipboard.Clear();
                SysClipboard.SetData("Unkown_Format", "Test");
            });

            //act
            using (var sut = new ClipboardObjectsManager(_clipboardObjectManager.Object, new[] { _formatsExtractor.Object }, new[] { _clipboardFilter.Object }, _serviceProviderMock.Object))
            {
                sut.AddListener(_listenerMock.Object);
                sut.AddTriggerToQueue();

                await Task.Delay(100);
            }

            //assert
            _listenerMock.Verify(x => x.OnResolvedTrigger(It.IsAny<ResolvedClipboardTrigger>()), Times.Never());
            _listenerMock.Verify(x => x.IsInterestedIn(It.IsAny<ClipboardObject>()), Times.Never());

            _clipboardObjectManager.Verify(x => x.AddImplementationsAsync(It.IsAny<ClipboardObject>(), It.IsAny<IEnumerable<EqualtableFormat>>()), Times.Never());
        }

        [Fact]
        public async Task Should_Create_With_Known_Format()
        {
            //arrange
            var testFormat = "Test_Format";
            await RunOnSTA(() =>
            {
                SysClipboard.Clear();
                SysClipboard.SetData(testFormat, "Test");
            });

            _formatsExtractor.Setup(x => x.Extract(It.IsAny<ClipboardTrigger>(), It.IsAny<IDataObject>())).Returns(new[] { new TestEquatableFormat(new ClipboardFormat(testFormat, testFormat, "Test", new ClipboardFormatCategory("Test Category", "Test"))) });
            _listenerMock.Setup(x => x.IsInterestedIn(It.IsAny<ClipboardObject>())).Returns(true);

            //act
            using (var sut = new ClipboardObjectsManager(_clipboardObjectManager.Object, new[] { _formatsExtractor.Object }, new[] { _clipboardFilter.Object }, _serviceProviderMock.Object))
            {
                sut.AddListener(_listenerMock.Object);
                sut.AddTriggerToQueue();

                await Task.Delay(100);
            }

            //assert
            _listenerMock.Verify(x => x.OnResolvedTrigger(It.IsAny<ResolvedClipboardTrigger>()), Times.Once());
            _listenerMock.Verify(x => x.IsInterestedIn(It.IsAny<ClipboardObject>()), Times.Once());

            _clipboardObjectManager.Verify(x => x.AddImplementationsAsync(It.IsAny<ClipboardObject>(), It.IsAny<IEnumerable<EqualtableFormat>>()), Times.Once());
        }

        [Fact]
        public async Task Should_Merge_With_Last()
        {
            //arrange
            var testFormat = "Test_Format";
            await RunOnSTA(() =>
            {
                SysClipboard.Clear();
                SysClipboard.SetData(testFormat, "Test");
            });

            _formatsExtractor.Setup(x => x.Extract(It.IsAny<ClipboardTrigger>(), It.IsAny<IDataObject>())).Returns(new[] { new TestEquatableFormat(new ClipboardFormat(testFormat, testFormat, "Test", new ClipboardFormatCategory("Test Category", "Test"))) });
            _listenerMock.Setup(x => x.IsInterestedIn(It.IsAny<ClipboardObject>())).Returns(true);

            //act
            using (var sut = new ClipboardObjectsManager(_clipboardObjectManager.Object, new[] { _formatsExtractor.Object }, new[] { _clipboardFilter.Object }, _serviceProviderMock.Object))
            {
                sut.AddListener(_listenerMock.Object);
                sut.AddTriggerToQueue(new MergableClipboardTriggerType("Fist_Mergable", "First_Mergable_Icon", TimeSpan.FromMilliseconds(120), TimeSpan.Zero));

                await Task.Delay(100);

                sut.AddTriggerToQueue(new OSClipboardTriggerType("Second", "Second_Icon"));

                await Task.Delay(100);
            }

            //assert
            _listenerMock.Verify(x => x.OnResolvedTrigger(It.IsAny<ResolvedClipboardTrigger>()), Times.Once());
            _listenerMock.Verify(x => x.IsInterestedIn(It.IsAny<ClipboardObject>()), Times.Once());

            _clipboardObjectManager.Verify(x => x.AddImplementationsAsync(It.IsAny<ClipboardObject>(), It.IsAny<IEnumerable<EqualtableFormat>>()), Times.Once());
        }

        [Fact]
        public async Task Should_Not_Merge_With_Last()
        {
            //arrange
            var testFormat = "Test_Format";
            await RunOnSTA(() =>
            {
                SysClipboard.Clear();
                SysClipboard.SetData(testFormat, "Test");
            });

            _formatsExtractor.Setup(x => x.Extract(It.IsAny<ClipboardTrigger>(), It.IsAny<IDataObject>())).Returns(new[] { new TestEquatableFormat(new ClipboardFormat(testFormat, testFormat, "Test", new ClipboardFormatCategory("Test Category", "Test"))) });
            _listenerMock.Setup(x => x.IsInterestedIn(It.IsAny<ClipboardObject>())).Returns(true);

            //act
            using (var sut = new ClipboardObjectsManager(_clipboardObjectManager.Object, new[] { _formatsExtractor.Object }, new[] { _clipboardFilter.Object }, _serviceProviderMock.Object))
            {
                sut.AddListener(_listenerMock.Object);
                sut.AddTriggerToQueue();

                await Task.Delay(100);

                sut.AddTriggerToQueue();

                await Task.Delay(100);
            }

            //assert
            _listenerMock.Verify(x => x.OnResolvedTrigger(It.IsAny<ResolvedClipboardTrigger>()), Times.Exactly(2));
            _listenerMock.Verify(x => x.IsInterestedIn(It.IsAny<ClipboardObject>()), Times.Exactly(2));

            _clipboardObjectManager.Verify(x => x.AddImplementationsAsync(It.IsAny<ClipboardObject>(), It.IsAny<IEnumerable<EqualtableFormat>>()), Times.Exactly(2));
        }

        [Fact]
        public async Task Should_Not_Add_When_Not_Interested()
        {
            //arrange
            var testFormat = "Test_Format";
            await RunOnSTA(() =>
            {
                SysClipboard.Clear();
                SysClipboard.SetData(testFormat, "Test");
            });

            _formatsExtractor.Setup(x => x.Extract(It.IsAny<ClipboardTrigger>(), It.IsAny<IDataObject>())).Returns(new[] { new TestEquatableFormat(new ClipboardFormat(testFormat, testFormat, "Test", new ClipboardFormatCategory("Test Category", "Test"))) });
            _listenerMock.Setup(x => x.IsInterestedIn(It.IsAny<ClipboardObject>())).Returns(false);

            //act
            using (var sut = new ClipboardObjectsManager(_clipboardObjectManager.Object, new[] { _formatsExtractor.Object }, new[] { _clipboardFilter.Object }, _serviceProviderMock.Object))
            {
                sut.AddListener(_listenerMock.Object);
                sut.AddTriggerToQueue();

                await Task.Delay(100);
            }

            //assert
            _listenerMock.Verify(x => x.OnResolvedTrigger(It.IsAny<ResolvedClipboardTrigger>()), Times.Never());
            _listenerMock.Verify(x => x.IsInterestedIn(It.IsAny<ClipboardObject>()), Times.Once());

            _clipboardObjectManager.Verify(x => x.AddImplementationsAsync(It.IsAny<ClipboardObject>(), It.IsAny<IEnumerable<EqualtableFormat>>()), Times.Once());
        }

        private static Task RunOnSTA(Action action)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            var thread = new Thread(() =>
            {
                try
                {
                    action();
                    taskCompletionSource.SetResult(true);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            })
            {
                Name = nameof(ClipboardObjectsManagerTests) + nameof(RunOnSTA) + nameof(Thread)
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            return taskCompletionSource.Task;
        }

        private class TestEquatableFormat : EqualtableFormat
        {
            public TestEquatableFormat(ClipboardFormat format) : base(format) { }
        }
    }

    public static class ClipboardObjectsManagerExtensions
    {
        public static void AddTriggerToQueue(this ClipboardObjectsManager sut, ClipboardTriggerType triggerType = null)
        {
            triggerType ??= new CustomClipboardTriggerType("Test", "Test");
            sut.ProcessClipboardTrigger(new ClipboardTrigger(triggerType, new ProgramInfo(Process.GetCurrentProcess()), new ProgramInfo(Process.GetCurrentProcess()), new WindowInfo("Test", "Test")));
        }
    }

    public class MockCreator
    {
        public static Mock CreateMock(Type genericType, Type itemType)
        {
            var typeToMock = genericType.MakeGenericType(itemType);
            var creator = typeof(Mock<>).MakeGenericType(typeToMock);
            return (Mock)Activator.CreateInstance(creator);
        }
    }
}
