using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SimpleElastic.Test
{
    public class NameHelperTests
    {
        public class TestClass
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
                NamingStrategyParameters =new object[] { true,true })]
            public int NamedCamelCaseWithParams { get; set; }

            [JsonProperty(
                PropertyName = "NamedCamel",
                NamingStrategyType = typeof(CamelCaseNamingStrategy))]
            public int NamedCamelCase { get; set; }

        }

        [Fact]
        public void NoAttributeCorrect()
        {
            var name = nameof(TestClass.NoAttribute).ToName<TestClass>();
            Assert.Equal("noAttribute", name);
        }

        [Fact]
        public void ExplicitNameCorrect()
        {
            var name = nameof(TestClass.ExplicitName).ToName<TestClass>();
            Assert.Equal("explicit", name);
        }

        [Fact]
        public void CamelCaseCorrect()
        {
            var name = nameof(TestClass.CamelCase).ToName<TestClass>();
            Assert.Equal("camelCase", name);
        }

        [Fact]
        public void SnakeCaseCorrect()
        {
            var name = nameof(TestClass.SnakeCase).ToName<TestClass>();
            Assert.Equal("snake_case", name);
        }

        [Fact]
        public void NamedCamelCaseCorrect()
        {
            var name = nameof(TestClass.NamedCamelCase).ToName<TestClass>();
            Assert.Equal("NamedCamel", name);
        }

        [Fact]
        public void NamedCamelCaseWithParamsCorrect()
        {
            var name = nameof(TestClass.NamedCamelCaseWithParams).ToName<TestClass>();
            Assert.Equal("namedCamel", name);
        }

    }
}
