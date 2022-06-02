# TWNetwork
## Summary
Class Library to use the built in GameNetwork in Mount and Blade 2: Bannerlord on any kind of Server and Client System.
This repository is going to use Protobuf-net library for serializing messages and [HarmonyLib]() library to patch out
the methods for GameNetwork class and other classes about networking that is related to missions in Mount and Blade 2: Bannerlord.
## Problem
I want to be able to create a server on my local computer that can use the GameNetwork class without the NetworkMain class.
Basically, I want to separate my server from TaleWorld's server. (NetworkMain class is connecting to the lobby server of TaleWorlds)
## Plan
- Collect the methods that needs to be patched out with dnSpy.
- Find the best way to patch out these methods with HarmonyLib.
- Create Initial Class Diagram.
- Code and debug.
- Test in missions.
