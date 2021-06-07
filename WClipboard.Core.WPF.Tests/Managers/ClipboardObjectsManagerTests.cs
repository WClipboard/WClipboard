using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.WPF.Clipboard;
using WClipboard.Core.WPF.Clipboard.Implementation;
using WClipboard.Core.WPF.Clipboard.Trigger;
using Xunit;

using SysClipboard = System.Windows.Clipboard;

namespace WClipboard.Core.WPF.Tests.Managers
{
    public class ClipboardObjectsManagerTests
    {
        private readonly Mock<IClipboardFormatsManager> _formatsManagerMock;
        private readonly Mock<IClipboardObjectManager> _clipboardObjectManager;
        private readonly Mock<IClipboardObjectsListener> _listenerMock;
        private readonly Mock<IServiceProvider> _serviceProviderMock;

        public ClipboardObjectsManagerTests()
        {
            _formatsManagerMock = new Mock<IClipboardFormatsManager>();
            _clipboardObjectManager = new Mock<IClipboardObjectManager>();
            _listenerMock = new Mock<IClipboardObjectsListener>();
            _serviceProviderMock = new Mock<IServiceProvider>();
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
            using (var sut = new ClipboardObjectsManager(_formatsManagerMock.Object, _clipboardObjectManager.Object, _serviceProviderMock.Object))
            {
                sut.AddListener(_listenerMock.Object);
                sut.AddTriggerToQueue();

                await Task.Delay(100);
            }

            //assert
            _listenerMock.Verify(x => x.OnResolvedTrigger(It.IsAny<ResolvedClipboardTrigger>()), Times.Never());
            _listenerMock.Verify(x => x.OnResolvedTriggerUpdated(It.IsAny<ResolvedClipboardTrigger>()), Times.Never());
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
            using (var sut = new ClipboardObjectsManager(_formatsManagerMock.Object, _clipboardObjectManager.Object, _serviceProviderMock.Object))
            {
                sut.AddListener(_listenerMock.Object);
                sut.AddTriggerToQueue();

                await Task.Delay(100);
            }

            //assert
            _listenerMock.Verify(x => x.OnResolvedTrigger(It.IsAny<ResolvedClipboardTrigger>()), Times.Never());
            _listenerMock.Verify(x => x.OnResolvedTriggerUpdated(It.IsAny<ResolvedClipboardTrigger>()), Times.Never());
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

            _formatsManagerMock.Setup(x => x.Values).Returns(new[] { new ClipboardFormat(testFormat, testFormat, "Test", new ClipboardFormatCategory("Test Category", "Test")) });
            _listenerMock.Setup(x => x.IsInterestedIn(It.IsAny<ClipboardObject>())).Returns(true);

            //act
            using (var sut = new ClipboardObjectsManager(_formatsManagerMock.Object, _clipboardObjectManager.Object, _serviceProviderMock.Object))
            {
                sut.AddListener(_listenerMock.Object);
                sut.AddTriggerToQueue();

                await Task.Delay(100);
            }

            //assert
            _listenerMock.Verify(x => x.OnResolvedTrigger(It.IsAny<ResolvedClipboardTrigger>()), Times.Once());
            _listenerMock.Verify(x => x.OnResolvedTriggerUpdated(It.IsAny<ResolvedClipboardTrigger>()), Times.Never());
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

            _formatsManagerMock.Setup(x => x.Values).Returns(new[] { new ClipboardFormat(testFormat, testFormat, "Test", new ClipboardFormatCategory("Test Category", "Test")) });
            _listenerMock.Setup(x => x.IsInterestedIn(It.IsAny<ClipboardObject>())).Returns(true);

            //act
            using (var sut = new ClipboardObjectsManager(_formatsManagerMock.Object, _clipboardObjectManager.Object, _serviceProviderMock.Object))
            {
                sut.AddListener(_listenerMock.Object);
                sut.AddTriggerToQueue();

                await Task.Delay(100);

                sut.AddTriggerToQueue(new ClipboardTriggerType("Second", "Second_Icon", ClipboardTriggerSourceType.Extern, 100000));

                await Task.Delay(100);
            }

            //assert
            _listenerMock.Verify(x => x.OnResolvedTrigger(It.IsAny<ResolvedClipboardTrigger>()), Times.Once());
            _listenerMock.Verify(x => x.OnResolvedTriggerUpdated(It.IsAny<ResolvedClipboardTrigger>()), Times.Once());
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

            _formatsManagerMock.Setup(x => x.Values).Returns(new[] { new ClipboardFormat(testFormat, testFormat, "Test", new ClipboardFormatCategory("Test Category", "Test")) });
            _listenerMock.Setup(x => x.IsInterestedIn(It.IsAny<ClipboardObject>())).Returns(true);

            //act
            using (var sut = new ClipboardObjectsManager(_formatsManagerMock.Object, _clipboardObjectManager.Object, _serviceProviderMock.Object))
            {
                sut.AddListener(_listenerMock.Object);
                sut.AddTriggerToQueue();

                await Task.Delay(100);

                sut.AddTriggerToQueue();

                await Task.Delay(100);
            }

            //assert
            _listenerMock.Verify(x => x.OnResolvedTrigger(It.IsAny<ResolvedClipboardTrigger>()), Times.Exactly(2));
            _listenerMock.Verify(x => x.OnResolvedTriggerUpdated(It.IsAny<ResolvedClipboardTrigger>()), Times.Never());
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

            _formatsManagerMock.Setup(x => x.Values).Returns(new[] { new ClipboardFormat(testFormat, testFormat, "Test", new ClipboardFormatCategory("Test Category", "Test")) });
            _listenerMock.Setup(x => x.IsInterestedIn(It.IsAny<ClipboardObject>())).Returns(false);

            //act
            using (var sut = new ClipboardObjectsManager(_formatsManagerMock.Object, _clipboardObjectManager.Object, _serviceProviderMock.Object))
            {
                sut.AddListener(_listenerMock.Object);
                sut.AddTriggerToQueue();

                await Task.Delay(100);
            }

            //assert
            _listenerMock.Verify(x => x.OnResolvedTrigger(It.IsAny<ResolvedClipboardTrigger>()), Times.Never());
            _listenerMock.Verify(x => x.OnResolvedTriggerUpdated(It.IsAny<ResolvedClipboardTrigger>()), Times.Never());
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
    }

    public static class ClipboardObjectsManagerExtensions
    {
        public static void AddTriggerToQueue(this ClipboardObjectsManager sut, ClipboardTriggerType triggerType = null)
        {
            triggerType ??= new ClipboardTriggerType("Test", "Test", ClipboardTriggerSourceType.Intern, 0);
            sut.ProcessClipboardTrigger(new ClipboardTrigger(triggerType, new ProgramInfo(Process.GetCurrentProcess()), new WindowInfo("Test", "Test")));
        }
    }
}
