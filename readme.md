
# SimpleElastic Elasticsearch Client

This is a .NET client for Elasticsearch. Compared to NEST, the official .NET client,
it is designed to be a lightweight minimal abstraction which exposes the underlying
Elasticsearch API to the developer. This means when you are referencing the API 
documentation you can write code that more or less resembles the JSON directly, rather
than needing to figure out your query in JSON first then translate it to a NEST query.

This client depart most substantially from others in that a type model of the query and
command language itself is not provided. Most API methods such as `_search` which use a
body simply accept an object which is serialized using Json.NET. There is no attempt made
to use the type system to enforce correctly formed queries.

## Current Status

This project is a work in progress in pre-alpha status, not ready for use yet.

# Types and Serialization

In SimpleElastic generally follows the pattern that requests to Elasticsearch is untyped and
open, while responses are serialized to specific response types. The main search method for
example uses the following signature:

```csharp
Task<SearchResult<TSource>> SearchAsync<TSource>(
    string index,
    object query,
    object options,
    CancellationToken cancel)
```

The query is defined as `System.Object`, there's no abstract "Query" class with "TermQuery",
"TermsQuery", etc. child classes provided. Instead you can use whatever combination of anonymous
or dynamic classes, dictionaries, or custom classes. These will be serialized using Json.NET to
generate the query body.

## Why Use System.Object for Query Definition?

The Elasticsearch query language does not lend itself to being modeled as an set of .NET
objects in my opinion. Any attempt to provide a fluent interface is impeded by the need 
to create nested object definitions, creating delegates within delegates. While there is
some convenience in being able to utilize the typeahead system when composing queries,
"translating" JSON queries written for example in Kibana is not particularly intuitive.
The problem is that much worse for the fact the Elasticsearch queries contain many small
objects for which an obvious name does not exist.

A non-fluent approach which still provides the language as a type model loses the benefits
of typeahead discoverability, and adds a lot of extra noise to queries. An Elasticsearch
query may have many small objects which do not have an obvious name, and these may have 
many different potential properties. Combine this with .NET naming guidelines favoring 
more verbose, non-abbreviated type names and you get a lot of clutter in your code. And yet
for all the drawbacks typing can only save you from a certain narrow class of errors.

A better solution might be a [Visual Studio Code Analysis Rule], similar to the
validation that Kibana's query tool provides, but this is an effort outside the scope of
this project for now.

[Visual Studio Code Analysis Rule]: https://docs.microsoft.com/en-us/visualstudio/code-quality/code-analysis-for-managed-code-overview?view=vs-2017

## The Map Class

The `Map` class is provided to make writing queries with fields which are not valid
C# field name easier.

Consider the following search query:

```json
{
  "query": {
    "terms": {
      "document.name": "elastic"
    }
  }
}
```

Recreating in C#, the `document.name` field is problematic:

```csharp
new {
    query = new {
        terms = new {
            document.name = "elastic" // Error here
        }
    }
}
```

We could use a `Dictionary<string, object>` here, but this client supplies a `Map` 
class which it is shorter to type and makes the resulting query easier to read.

With the Map class, our query now looks like this:

```csharp
new {
    query = new {
        terms = new Map {
            { "document.name", "elastic" }
        }
    }
}
```

The Map class also implements DynamicObject, allowing usage as a dynamic object, 
`IEnumerable` and `Add(string, object)` for collection initialization, and a property
name object, resulting in quite flexible usage options:

```csharp

// Collection initialization
dynamic query = new Map { 
    { "query", new { match_all = new {} }} 
};

// Setting dynamic fields
query.size = 10;

// Indexer
query["from"] = 0;

```

## Flatten Deserializer

For nested query [inner hits] and aggregation [top hits] results, a type to be returned cannot be easily
passed to the client for use when deserializing. Instead, these are returned as a "flattened" dictionary,
represented as a `FlatObject`. Each property in the hits object is added to the resulting dictionary, using
the entire path as a key, with arrays assigned a numeric index. For example, this object:

```json
{
	"id": 123,
	"name": "Example",
	"properties": {
		"title": "abc",
		"values": [ "one", "two" ]
	}
}
```

Will be serialized as a `FlatObject` with the following keys:

```json
{
	"id": 123,
	"name": "Example",
	"properties.title": "abc",
	"properties.values[0]": "one",
	"properties.values[1]": "two"
}
```

[inner hits]: https://www.elastic.co/guide/en/elasticsearch/reference/current/search-request-inner-hits.html
[top hits]: https://www.elastic.co/guide/en/elasticsearch/reference/current/search-aggregations-metrics-top-hits-aggregation.html

## Name Helper

The `NameHelper` provides the `ToName<TDocument>` extension method to obtain the property
name that will be used when the object is serialized for Elasticsearch. For example, 
given the class:

```csharp
public class MyIndex
{
	[JsonProperty("nm")]
	public string Name { get; set; }
}
```

The following code can be used to obtain the property name:

```csharp
string propertyName = nameof(MyIndex.Name).ToName<MyIndex>();

Assert.Equal("nm", propertyName); // success
```

The `ToName<TDocument>` method supports `JsonPropertyAttribute` and `JsonObjectAttribute`
custom naming strategy, or will fall back to the global default if none was specified.

# Version Support

Since the queries are user generated, it is up to consumers to create queries which are
compatible with the version of Elasticsearch you are using. The minimal models and conventions
which are present support conventions which are generally stable, these are based on the 
documentation from the latest release (6.4).