using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace SimpleElastic.Test
{
    /// <summary>
    /// Specifies the order to run integration tests.
    /// </summary>
    public class OrderAttribute : Attribute
    {
        public int Order { get; }

        public OrderAttribute(int i)
        {
            Order = i;
        }
    }

    public class IntegrationTestCaseOrderer : ITestCaseOrderer
    {
        public const string TypeName =
            IntegrationTestCaseOrderer.AssemblyName + "." +
            nameof(SimpleElastic.Test.IntegrationTestCaseOrderer);

        public const string AssemblyName =
            nameof(SimpleElastic) + "." +
            nameof(SimpleElastic.Test);

        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) 
            where TTestCase : ITestCase
        {
            return testCases.OrderBy(t => t.TestMethod.Method
                .ToRuntimeMethod()
                .GetCustomAttribute<OrderAttribute>()?.Order ?? 0);
        }
    }
}
