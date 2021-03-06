# MagicUnion
MS VisualStudio extension for generating discriminated union in C#.

Unions are awesome feature and you can read what they can bring to you [here](https://fsharpforfunandprofit.com/posts/discriminated-unions/). 
In contrast to C++ and F# for instance, the C# does not have discriminated unions.Unions are the number one in ["The 8 most missing features in C#"](https://tooslowexception.com/the-8-most-missing-features-in-c/). 
It's really frustrating, first of all because it forces developer to type unnecessary hundreds lines of code and secondary because it is certainly not even a technical problem for Microsoft to implement them in C#. Thus the aim of this VS extension was to generate union as a separate class when developer needs a new union.

Generated union has following features:

1. Union is a class with read-only members.
1. Type safe construction for every union member via Creat method.
1. Null check for every reference type in the Create method.
1. Provides Match method for access to all members:
  1. Match method with Func as arguments.
  1. Match method with Action as arguments
  1. Math method for both above but all arguments boxed together into one ValueTuple.
1. Provides equality comparison by value
1. Provides GetHash by value.


The example of how to use this extension is in MagicUnion/MagicUnion.Test/MagicUnionTest.cs.


