using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SimpleElastic.Test
{
    [Trait("type", "unit")]
    public class NameHelperTests
    {
        [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public class Test
        {
            public int NoAttribute { get; set; }

            [JsonProperty("explicit")]
            public int ExplicitName { get; set; }

            [JsonProperty(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
            public int CamelCase { get; set; }

            [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
            public int SnakeCase { get; set; }

            [JsonProperty(
                PropertyName = "NamedCamel",
                NamingStrategyType = typeof(CamelCaseNamingStrategy),
                NamingStrategyParameters = new object[] { true, true })]
            public int NamedCamelCaseWithParams { get; set; }

            [JsonProperty(
                PropertyName = "NamedCamel",
                NamingStrategyType = typeof(CamelCaseNamingStrategy))]
            public int NamedCamelCase { get; set; }
        }

        public class TestWithoutAttribute
        {
            public int NoAttribute { get; set; }
        }

        [Fact]
        public void NoAttributeCorrect()
        {
            var name = nameof(Test.NoAttribute).ToName<Test>();
            Assert.Equal("no_attribute", name);
        }

        [Fact]
        public void NoAttributeWithDefaultStrategyCorrect()
        {
            var name = nameof(TestWithoutAttribute.NoAttribute).ToName<TestWithoutAttribute>();
            Assert.Equal("noAttribute", name);
        }


        [Fact]
        public void ExplicitNameCorrect()
        {
            var name = nameof(Test.ExplicitName).ToName<Test>();
            Assert.Equal("explicit", name);
        }

        [Fact]
        public void CamelCaseCorrect()
        {
            var name = nameof(Test.CamelCase).ToName<Test>();
            Assert.Equal("camelCase", name);
        }

        [Fact]
        public void SnakeCaseCorrect()
        {
            var name = nameof(Test.SnakeCase).ToName<Test>();
            Assert.Equal("snake_case", name);
        }

        [Fact]
        public void NamedCamelCaseCorrect()
        {
            var name = nameof(Test.NamedCamelCase).ToName<Test>();
            Assert.Equal("NamedCamel", name);
        }

        [Fact]
        public void NamedCamelCaseWithParamsCorrect()
        {
            var name = nameof(Test.NamedCamelCaseWithParams).ToName<Test>();
            Assert.Equal("namedCamel", name);
        }


        [Fact]
        public void PropertiesSerializeCorrectly()
        {
            var properties = new Properties<Test>
            {
                { nameof(Test.SnakeCase), "123" }
            };

            var json = JsonConvert.SerializeObject(properties);

            Assert.Equal("{\"snake_case\":\"123\"}", json);
            
        }

    }
}
