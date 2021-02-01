using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhampsChallenge.Runner.Shared.Direct;

namespace WhampsChallenge.Runner.Shared.Tests.Direct
{
    [TestClass]
    public class DirectCommunicatorTests
    {
        [TestMethod]
        public void SendingAndReceiving()
        {
            var sut = new DirectCommunicator();

            var hostReceiveTask = sut.ReceiveFromContestantAsync();
            hostReceiveTask.Status.Should().Be(TaskStatus.WaitingForActivation);

            sut.SendAsync("{'Test':true}");
            hostReceiveTask.Status.Should().Be(TaskStatus.RanToCompletion);
            var hostReceiveResult = hostReceiveTask.Result;
            hostReceiveResult.Value<bool>("Test").Should().BeTrue();

            var contestantReceiveTask = sut.ReceiveAsync();
            contestantReceiveTask.Status.Should().Be(TaskStatus.WaitingForActivation);

            sut.SendToContestant(new {Test = false});

            contestantReceiveTask.Status.Should().Be(TaskStatus.RanToCompletion);
            var contestantReceiveResult = contestantReceiveTask.Result;

            contestantReceiveResult.Should().Be("{\"Test\":false}");

            hostReceiveTask = sut.ReceiveFromContestantAsync();
            hostReceiveTask.Status.Should().Be(TaskStatus.WaitingForActivation);
        }
    }
}
