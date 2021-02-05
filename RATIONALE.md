# Table of Contents

- [Technology stack](#orga35b836)
- [Architectural decisions](#orgfa801c0)
- [Test strategy](#org5b0ffdf)
  - [Adopted convention](#org9517611)


<a id="orga35b836"></a>

# Technology stack

The software was built with .NET 5.0 and the C# language, mainly due to the author's familiarity with the platform. The use of "self-authored" code was favored over external libraries and frameworks, but some external packages were included:

-   [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting): Microsoft's utility to make it easier to set up and host an application with injected dependencies.
-   [FluentAssertions](https://fluentassertions.com/): a library that make use of C#'s extension methods to allow you to write much more readable tests.
-   [xUnit](https://xunit.net/): Chosen test runner.


<a id="orgfa801c0"></a>

# Architectural decisions

The software is laid out in a component-based fashion, following an enterprise-like architecture. There are layers that resemble a domain-driven approach, each one with a designated responsibility.

The Object-oriented paradigm was used, promoting composition over inheritance and making use of immutability when appropriate.

Below is a diagram sketch of the software layers and its dependencies:

```

               +---------+                    +--------+
+--------+     |         |------------------->|        |
|        |     |         |                    |        |
|        |     | Applica |    +----------+    |        |
|  Host  |     | tion    |    |          |    | Domain |
|        |---->|         |--->| Services |--->|        |
|        |     |         |    |          |    |        |
+--------+     +---------+    +----------+    +--------+
    |              |               |              ^
    v              v               v              |
+--------+     +---------------------------------------+
|  Api   |     |             Infrastructure            |
+--------+     +---------------------------------------+

```

The following is a list of short descriptions for each of these components:

-   **Infrastructure**: responsible for interacting with the data persistence system and providing query-like functionality to other components.
-   **Domain**: responsible for incorporating the core business concepts and its invariant rules.
-   **Services**: responsible for handling specific business requirements not suited for a single domain entity.
-   **Application**: responsible for providing the user-facing functionalities. The "use cases" of the software are incorporated here.
-   **Api**: responsible for intefacing the software with the outer world.
-   **Host**: not a layer but the top-most component that holds everything together and serves the software.

It is worth noting that, given the present simplicity of the business requirements, it can be stated that the chosen architecture is an overkill. Nevertheless, the groundwork is laid for future features that shall emerge.

The relationship between an `Account` and a `Transaction` was not completely clear to the author, and as immutability was favored over state changes, these entities became more like data-holder objects, heading towards the so-called "anemic" domain model. A `TransactionService` was created to enforce the main business rules validations, not suited to any of the particular entities.


<a id="org5b0ffdf"></a>

# Test strategy

The software relies mostly on entities/services tests and a full integration/snapshot test to guarantee that business requirements are met. Tests on the application (use cases) layer are more focused on assuring that the underlying components are linked and communicating properly.

The infrastructure and api layers' components were not tested directly, since they exist only to supply other components and, for the time being, are not shared with other modules.


<a id="org9517611"></a>

## Adopted convention

Tests aim to be short and to provide a single, specific guarantee, rather than asserting multiple things at once. This approach depends heavily on well-suited, general-purpose test setup helpers to avoid repetitive test arrangements in several places, but provides easy and concise test maintainability. One can guess test expectations just by looking at their signature, and they always break for a single reason.

The following naming convention was used:

```
<ComponentName>Tests.<FunctionName>_Should<ExpectedBehavior>[_When<Predicate>]
```

Being that the predicate part is ommited for the tests asserts for the most common, error-free scenario.
