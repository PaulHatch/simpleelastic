
# SimpleElastic Elasticsearch Client

This is a .NET client for elasticsearch. Compared to NEST, the official .NET client,
it is designed to be a lightweight minimal abstraction which exposes the underlying
elasticsearch API to the developer. This means when you are referencing the API 
documentation you can write code that more or less resembles the JSON directly, rather
than needing to figure out your query in JSON first then translate it to a NEST query.

This client depart most substantially from others in that a type model of the query and
command language itself is not provided. Most API methods such as `_search` which use a
body simply accept an object which is serialized using Json.NET. There is no attempt made
to use the type system to enforce correctly formed queries.

## Current Status

This project is a work in progress in pre-alpha status, not ready for use yet.

# Types and Serialization

In SimpleElastic generally follows the pattern that requests to elasticsearch is untyped and
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

The elasticsearch query language does not lend itself to being modeled as an set of .NET
objects in my opinion. Any attempt to provide a fluent interface is impeded by the need 
to create nested object definitions, creating delegates within delegates. While there is
some convenience in being able to utilize the typeahead system when composing queries,
"translating" JSON queries written for example in Kibana is not particularly intuitive.

A non-fluent approach which still provides the language as a type model loses the benefits
of typeahead discoverability, and adds a lot of extra noise to queries. An elasticsearch
query may have many small objects which do not have an obvious name, and these may have 
many different potential properties. Combine this with .NET naming guidelines favoring 
more verbose, non-abbreviated type names and you get a lot of clutter in your code. And yet
for all the drawbacks typing can only save you from a certain narrow class of errors.

I believe an ideal solution would be a [Visual Studio Code Analysis Rule], similar to the
validation that Kibana's query tool provides, but this is an effort outside the scope of
this project for now.

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
	"values.0": "one",
	"values.1": "two"
}
```

[inner hits]: https://www.elastic.co/guide/en/elasticsearch/reference/current/search-request-inner-hits.html
[top hits]: https://www.elastic.co/guide/en/elasticsearch/reference/current/search-aggregations-metrics-top-hits-aggregation.html

# Version Support

Since the queries are user generated, it is up to consumers to create queries which are
compatible with the version of elasticsearch you are using. The minimal models and conventions
which are present support conventions which are generally stable, these are based on the 
documentation from the latest release (6.4).