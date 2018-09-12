using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
namespace SimpleElastic
{
    internal static class NameHelper<T>
    {
        internal static readonly Dictionary<string, string> Values
            = new Dictionary<string, string>();

        
        static NameHelper()
        {
            var objectAttribute = typeof(T).GetCustomAttribute<JsonObjectAttribute>();

            NamingStrategy naming;
            if (objectAttribute?.NamingStrategyType != null)
            {
                naming = (NamingStrategy)Activator.CreateInstance(
                    objectAttribute.NamingStrategyType,
                    objectAttribute.NamingStrategyParameters);
            }
            else
            {
                naming = (SimpleElasticClient.DefaultJsonSettings.ContractResolver as DefaultContractResolver)?.NamingStrategy;
            }

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<JsonPropertyAttribute>();

                var specificName = !String.IsNullOrWhiteSpace(attribute?.PropertyName);
                string name = specificName ? attribute.PropertyName : property.Name;

                if (attribute?.NamingStrategyType != null)
                {
                    var propNaming = (NamingStrategy)Activator.CreateInstance(
                        attribute.NamingStrategyType, 
                        attribute.NamingStrategyParameters);

                    name = propNaming.GetPropertyName(name, specificName);
                }
                else
                {
                    name = naming?.GetPropertyName(name, specificName) ?? name;
                }

                Values.Add(property.Name, name);
            }
        }

        internal static string GetName(string name)
        {
            if (Values.ContainsKey(name))
            {
                return Values[name];
            }
            else
            {
                return name;
            }
        }
    }

    /// <summary>
    /// Extension helper for getting names of properties.
    /// </summary>
    public static class NameHelper
    {
        /// <summary>
        /// Use to get the elasticsearch property name of a class
        /// property name. Converts the name of a property to the
        /// name used when serializing the property to JSON.
        /// </summary>
        /// <typeparam name="TDocument">
        /// The type to get the property name from.
        /// </typeparam>
        /// <param name="name">The name to look up.</param>
        /// <returns>
        /// The JSON name that will be used for the specified property.
        /// </returns>
        public static string ToName<TDocument>(this string name) =>
            NameHelper<TDocument>.GetName(name);
    }
}
