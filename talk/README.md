## Scripts for "13 Ways of Looking at a Turtle"

This directory contains all the scripts used in the talk "13 Ways of Looking at a Turtle"

## Introduction

This is cousin to my [FP patterns](https://fsharpforfunandprofit.com/fppatterns/) talk.

This is like a tasting menu -- you'll get a taste of each dish.
Not enough to really digest it, but enough to be introduced to the ideas and concepts at least,
so that you can come back later and go more in depth into ideas that seem useful to you.

## Part 1. Three fundamental paradigms

* [Object Oriented Turtle](1-OO_Turtle.fsx)
* [Abstract Data Turtle](2-AbstractDataTurtle.fsx)
* [Functional Turtle](3-FP_Turtle.fsx)

## Part 2. More complex turtles

* [Working with state](4-StateMonad.fsx)
* [Working with errors](5-ErrorHandling.fsx)

## Part 3. Decoupled turtles

* [Batch Processing](6-BatchCommands.fsx)
* [Actor model](6b-TurtleActor.fsx)
* [Event Sourcing](7-EventSourcing.fsx)
* [Stream Processing](8-StreamProcessing.fsx)

## Part 4. Dependent turtles

* [Dependency **injection**](9-OO_DependencyInjection.fsx)
* [Dependency **parameterization**](10-FP_DependencyParameterization.fsx)
* [Dependency **rejection**](11-OO_DependencyRejection.fsx)

## Part 5. Really complex turtles

* [Interpreted turtle](12-Interpreter.fsx)
* [Capability-based turtle](13-Capabilities.fsx)

## Table summarizing each approach

| Name                               | Can you reason about the code?                                                           | Ease of use                        | Ease of Testing                                     | Ease of change/Decoupled                                          | Implementation Complexity                                | Recommendation                                              |
|------------------------------------|------------------------------------------------------------------------------------------|------------------------------------|-----------------------------------------------------|-------------------------------------------------------------------|----------------------------------------------------------|-------------------------------------------------------------|
| 1. OO with hard-coded dependency   | Can't see inside. Mutable state.                                                         | Good                               | Hard                                                | Low. Coupled to canvas                                            | Easy but also can to fall into traps                     | Prefer with dependency injection                            |   
| 2. ADT with hard-coded dependency  | Can't see inside. Mutable state.                                                         | Good                               | Okay                                                | ditto                                                             | Easy. Less likely to fall into complexity traps like OO  | ADT focuses on composition more than OO                     |
| 3. FP with hard-coded dependency   | Yes because immutable state.                                                             | Good                               | Good because stateless                              | ditto                                                             | Easy.                                                    | Yes, but even better with dependency rejection              | 
| 4. Tracking state with state monad | Yes because immutable state.                                                             | Can be tricky                      | Good because stateless                              |                                                                   | Harder.                                                  | Avoid unless you are going full FP                          |
| 5. Error handling with Result      | Excellent. Explicit errors rather than exceptions. Immutable state.                      | Can be tricky                      | Errors as values make testing the unhappy path easy |                                                                   | Straightforward.                                         | Recommended                                                 |
| 6. Batch processing                | Both client and server are easy to reason about                                          | Easy                               | Easy                                                | Client decoupled from server, so implementation change is easy    | Straightforward.                                         | Simplest form of client/server decoupling                   |
| 6 1/2. Agent                       | Both client and server are easy to reason about                                          | Easy                               | Agent state can be opaque, like OO                  | ditto                                                             | Can be complex. Best to use an agent library             | For specific scenarios only                                 |
| 7. Event sourcing                  | Modeling the events in your head can be tricky                                           | Trickier than expected.            | Somewhat                                            | Changing event schemas and versioning is tricky                   | Tricky. Recommend a third party tool for the event store | For specific scenarios only. Don't use because of the hype. |
| 8. Stream processing               | In theory, keeping the parts separate is cleaner. But more to keep track of in your head | Poor                               | Somewhat                                            | As for event sourcing + now you have a distributed system as well | Tricky. Recommend a third party tool                     | For very specific scenarios only. Avoid otherwise.          |
| 9. Dependency injection            | If using a DI tool, can be hard to track where the dependencies are coming from          | Good, with a DI library            | Very good                                           | Good. Decoupled from dependencies                                 | Follow ISP and SRP. Avoid too-large interfaces           | Common, but try dependency rejection first.                 |
| 10. Dependency parameterization    | Easy to track the dependencies                                                           | Very good as not too deeply nested | Very good                                           | ditto                                                             | Easy                                                     | Okay, but try dependency rejection first.                   |
| 11. Dependency rejection           | Yes. And the decision type acts as documentation                                         | Very Good                          | Very good                                           | ditto                                                             | Easy                                                     | Recommended                                                 |
| 12. Interpreter                    | Yes. Every possible instruction is explicit.                                             | Medium                             | Very good                                           | Completely decoupled                                              | Tricky                                                   | For very specific scenarios only (few instructions)         |
| 13. Capability-based               | Yes. Every capability is explicit.                                                       | Medium                             | Good                                                | Hides business rules from client                                  | Tricky                                                   | Recommended for dynamic web sites (HATEOAS)                 |

