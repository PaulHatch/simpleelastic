
# Elasticsearch Lite Client

This is a .NET client for elasticsearch. Compared to NEST, the offical .NET client,
it is designed to be a lightweight minimal abstraction which exposes the underlying
elasticsearch API to the developer. This means when you are referencing the API 
documentation you can write code that more or less resembles the JSON directly, rather
than needing to figure out your query in JSON first then translate it to a NEST query.

Most API methods such as `_search` which use a body simply accept an object which is
serialized using Json.NET. There is no attempt made to use the type system to enforce
correctly formed queries.

## The Map Class

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
class which it is shorter to type and makes the resulting query easier to read, and
implements DynamicObject, allowing direct property assignment.

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

# Version Support

Since the queries are user generated, it is up to consumers to create queries which are
compatible with the version of elasticsearch you are using. The minimal models and conventions
which are present support conventions which are generally stable, these are based on the 
documentation from the latest release (6.4).