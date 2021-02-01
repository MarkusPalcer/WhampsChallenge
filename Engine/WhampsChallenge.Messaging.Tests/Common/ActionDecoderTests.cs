using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WhampsChallenge.Core.Common;
using WhampsChallenge.Core.Maps;
using WhampsChallenge.Messaging.Common;

namespace WhampsChallenge.Messaging.Tests.Common
{
    [TestClass]
    public class ActionDecoderTests
    {
        public struct TestCase
        {
            public object SerializedData;
            public object ExpectedResult;
        }

        [TestMethod]
        public void AllLevelsAreCovered()
        {
            var discoverer = new Discoverer();
            foreach (var level in discoverer.Levels)
            {
                TestData.Should().ContainKey(level, "test cases should be defined for all levels");
                var cases = TestData[level].ToDictionary(x => x.ExpectedResult.GetType());

                foreach (var action in discoverer[level].Actions)
                {
                    cases.Should()
                         .ContainKey(action, $"there should be a test case for the action {action.FullName} for level {level}");
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(TestCases))]
        public void DecodingTestData(int level, TestCase data)
        {
            var sut = new ActionDecoder(level);

            var json = JsonConvert.SerializeObject(data.SerializedData);
            var deserialized = JsonConvert.DeserializeObject<JObject>(json);
            deserialized["Action"] = new JValue(data.SerializedData.GetType().Name);

            var result = sut.Decode(deserialized);

            result.GetType().Should()
                  .Be(data.ExpectedResult.GetType(), $"decoding the action {data.SerializedData.GetType()} should yield an object of type {data.ExpectedResult.GetType()}.");

            if (result.GetType().Properties().Any())
            {
                result.Should()
                      .BeEquivalentTo(data.ExpectedResult,
                                      $"decoding the action {data.SerializedData.GetType()} should yield an object with the same data.");
            }

        }


        private static IDictionary<int, TestCase[]> TestData = new Dictionary<int, TestCase[]>
        {
            {1, new[]
            {
                new TestCase
                {
                    SerializedData = new WhampsChallenge.Library.Level1.Actions.Move {Direction = Library.Shared.Enums.Direction.Up},
                    ExpectedResult = new Core.Level1.Actions.Move {Direction = Direction.Up}
                },
                new TestCase
                {
                    SerializedData = new Library.Level1.Actions.Pickup() ,
                    ExpectedResult = new Core.Level1.Actions.Pickup()
                },
            }},
            {2, new[]
            {
                new TestCase
                {
                    SerializedData = new WhampsChallenge.Library.Level2.Actions.Move {Direction = Library.Shared.Enums.Direction.Up},
                    ExpectedResult = new Core.Level2.Actions.Move {Direction = Direction.Up}
                },
                new TestCase
                {
                    SerializedData = new Library.Level2.Actions.Pickup(),
                    ExpectedResult = new Core.Level2.Actions.Pickup()
                },

            }},
            {3, new[]
                {
                new TestCase
                {
                    SerializedData = new WhampsChallenge.Library.Level3.Actions.Move {Direction = Library.Shared.Enums.Direction.Up},
                    ExpectedResult = new Core.Level3.Actions.Move {Direction = Direction.Up}
                },
                new TestCase
                {
                    SerializedData = new Library.Level3.Actions.Pickup(),
                    ExpectedResult = new Core.Level3.Actions.Pickup()
                },

                new TestCase
                {
                    SerializedData = new Library.Level3.Actions.Shoot {Direction = Library.Shared.Enums.Direction.North},
                    ExpectedResult = new Core.Level3.Actions.Shoot {Direction = Direction.North}
                }
            }}
        };

        public static IEnumerable<object[]> TestCases
        {
            get
            {
                foreach (var level in TestData)
                {
                    foreach (var testCase in level.Value)
                    {
                        yield return new object[] {level.Key, testCase};
                    }
                }
            }
        }
    }
}
