using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhampsChallenge.Level1;
using WhampsChallenge.Level1.actions;
using WhampsChallenge.Markers;

namespace ContractGeneration.Test
{
    [TestClass]
    public class ContractGenerationTests
    {
        [Result]
        private class EmptyResultType {}

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ResultTypeIsNeccesary()
        {
            var contract = SutCreator.Generate(typeof(Move), typeof(Pickup), typeof(Shoot));
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void OnlyOneResultAllowed()
        {
            var contract = SutCreator.Generate(typeof(Result), typeof(EmptyResultType));
        }

        [TestMethod]
        public void ResultTypeIsDetected()
        {
            var contract = SutCreator.Generate(typeof(EmptyResultType));
            contract.ResultType.Should().Be(nameof(EmptyResultType));
            contract.Types.Keys.Should().BeEquivalentTo(nameof(EmptyResultType));
        }

        enum ResultTypeEnum
        {
            Value1,
            Value2
        }

        [Result]
        private class ResultTypeWithEnum
        {
            public ResultTypeEnum Enum { get; set; }
        }

        [TestMethod]
        public void EnumsInResultTypeAreDetected()
        {
            var contract = SutCreator.Generate(typeof(ResultTypeWithEnum));
            contract.Enums.Keys.Should().BeEquivalentTo(nameof(ResultTypeEnum));
            contract.Enums[nameof(ResultTypeEnum)].Should().BeEquivalentTo(Enum.GetNames(typeof(ResultTypeEnum)), "the names of the enums should be included");
            contract.Types.Keys.Should().BeEquivalentTo(nameof(ResultTypeWithEnum));
        }


        //[TestMethod]
        //public void TestGeneratedContract()
        //{
        //    var contract = Contract.Generate(typeof(Move), typeof(Pick), typeof(Result));
        //    // Check that all enums are detected
        //    contract.Enums.Keys.Should().BeEquivalentTo(nameof(Direction), nameof(Perception));
        //    // Exemplary check that all enum items are present
        //    contract.Enums[nameof(Direction)].Should().BeEquivalentTo(Enum.GetNames(typeof(Direction)));
        //    // Check that all class types are detected
        //    contract.Types.Keys.Should().BeEquivalentTo(nameof(Result), nameof(GameState));
        //    // Check for class type and array type property handling
        //    contract.Types[nameof(Result)].Should().BeEquivalentTo(new Dictionary<string,string>()
        //    {
        //        {"Perceptions", "Perception[]"},
        //        {"GameState", "GameState"}
        //    });
        //    // Check for primitive type property handling and property ignoring by attribute
        //    contract.Types[nameof(GameState)].Should().BeEquivalentTo(new Dictionary<string,string>()
        //    {
        //        {"MovesLeft", "Int32"},
        //        {"HasArrow", "Boolean"}
        //    });

        //    // Check action detection
        //    contract.Actions.Keys.Should()
        //        .BeEquivalentTo(nameof(IActions.Move), nameof(IActions.Pickup), nameof(IActions.Shoot));

        //    contract.Actions[nameof(IActions.Move)].Parameters.Should().BeEquivalentTo(new Dictionary<string,string>()
        //    {
        //        {"direction", "Direction"}
        //    });

        //    contract.Actions[nameof(IActions.Pickup)].Parameters.Should().BeEmpty();

        //    contract.Actions[nameof(IActions.Shoot)].Parameters.Should().BeEquivalentTo(new Dictionary<string,string>()
        //    {
        //        {"direction", "Direction"}
        //    });
        //}
    }
}
 