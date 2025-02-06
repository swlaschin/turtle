
Caller             Implementor

Complexity
   Ease of use        Ease of implementation
Trust
  - reasoning          
  - easy to test
 -- can call wrong     -- can implement wrong
 -- error handling     
Change
  - Robustness/decoupling
  -- can change         -- can change (decoupling)


//===================
Intro

cousin to my /fppatterns talk

This is like a tasting menu -- you'll get a taste of each dish not enough to really digest it,
but enought to be introduced to the ideas and concepts at least, so tyhat you can come back later and
go more in depth into ideas that seem useful to you.

functional tapas

//===================
Part 1 basic implementation - 6 ways of managing state


//===================
2. OO Turtle - "Tootle"



//===================
1. Abstract Data Type

Separate the functions and data together. Older than OO, but still useful because it forces you to 
avoid inheritance and use composition instead.


//===================
3.FP

Alternative -- why encapsate -- make everything immutable!

but passing the state around is painful?
    let s2 = move(20,s1)
    let s3 = move(20,s3)


enter partial application
if we think of a function as a box with inputs and outputs
    daigram of tracks before and after partial
then we can connect the tracks, where the output of one function is used as the input for the next one
This is composition, and in F# is typically done by "piping"
    diagram of piping

but what if we need to make decisions?


    let s2,distanceMoved = move(20,s1)
    if distanceMoved < 20 then
        printf "at well"
        let s3 = turn(180)
    else
        let s3,distanceMoved2 = move(20,s2)

becomes painful!

//===================
4. State monad

again, lets look at the diagram -- make a thing of (state->'a*state)

    let s = move 20
    // s is a State<distanceMoved>
    if distanceMoved < 20 then
        ...
    else
        ...

how can we extract the distanceMoved from the State?
using a "bind" function.  

    let s = move 20 
    s |> bind (fun distanceMoved ->
        if distanceMoved < 20 then
            ...
        else
            ...
       )

Most FP languages have ways to make this easier
F# computation experession, Haskell-do notation, Scala, for-comprehensions,

    state {
       let! distanceMoved  = move 20 
       if distanceMoved < 20 then
          printf "at well"
          do!  turn(180)
       else
          let! distanceMoved2 = move 20
       }

looks quite imperative!

One problem is that the output of all this is a function -- nothing happens until you run it!

let run (State f) initialState =
   f initialState 
   // returns finalState and ouput value


used like this:

   let myExample = state {
       let! distanceMoved  = move 20 
       if distanceMoved < 20 then
          ...
       else
          ...
       }

   let initialState = TurtleState.default
   let output,finalState = run myExample initialState 




//===================
5. Batch commands

the state monad had to be "run" -- lets look at another way to hide state, which is to issue a set of commands
and run them all at once. The caller doesn't have to keep track of state.

Say that we have a function like this

    let processCommand oldState command =
       let newState = do something with command 
       return newState 

then to process a list of commands, we can use "fold" (equivalent to for-loop)
    let fold processCommand initialState commands =
        let mutable state = initialState
        for each command in commands do
           state <- processCommand state command 
        // return final state        
        state 

    List.fold processCommand initialState commands 
       // explain initialState
       // processCommand 


let's forget about decisions again for now, but lets say we do want to run it as a batch.

We need store "commands" for later use. That is we need to turn our API calls into data?
How can we do that?

Use a union type

  TurtleCommand = 
    | Move of int
    | Turn of int
    | PenUp
    | PenDown

 then just fold the state through the list of commands

//===================
6. Turtle actor

which leads to another way of handling state -- using a recursive loop

    let rec actorLoop state =
       let command = read command from queue
       let newState = do something with command and state
       actorLoop newState // call loop again


//===================
Part 2 commands vs events

what if the actor crashes? 

* We could store the state in the DB and reload it for each command
   -- typical web service
* We could store each command in the DB 
   -- like we did for batch processing)

let's lo

Don't save the current state of objects
Save the events that lead to the current state

What if we reach the edge

Event Sourcing & Command Sourcing confusion
NO Side-effects on event replay!!!

Compare with command sourcing (the batch example)
