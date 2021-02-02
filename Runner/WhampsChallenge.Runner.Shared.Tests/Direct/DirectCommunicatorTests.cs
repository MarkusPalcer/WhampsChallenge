using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhampsChallenge.Core.Common.Discovery;
using WhampsChallenge.Core.Level3.Actions;
using WhampsChallenge.Runner.Shared.Direct;

namespace WhampsChallenge.Runner.Shared.Tests.Direct
{
    [TestClass]
    public class DirectCommunicatorTests
    {
        [TestMethod]
        public void SendingAndReceiving()
        {
            var sut = new DirectCommunicator(new Discoverer(), 3);

            var hostReceiveTask = sut.ReceiveFromContestantAsync();
            hostReceiveTask.Status.Should().Be(TaskStatus.WaitingForActivation);

            sut.SendAsync("{'Action':'Pickup'}");
            hostReceiveTask.Status.Should().Be(TaskStatus.RanToCompletion);
            var hostReceiveResult = hostReceiveTask.Result;
            hostReceiveResult.GetType().Should().Be(typeof(Pickup));

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
