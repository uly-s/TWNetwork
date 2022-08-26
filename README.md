# TWNetwork
## Summary
Class Library to use the built in GameNetwork in Mount and Blade 2: Bannerlord on any kind of Server and Client System.
This repository is going to use [HarmonyLib](https://harmony.pardeike.net/articles/intro.html) library to patch out
the methods for GameNetwork class and other classes about networking that is related to missions in Mount and Blade 2: Bannerlord.
## How to build
- Soon
## Tutorial to Use
- Soon
## Problem
I want to be able to create a server on my local computer that can use the GameNetwork class without the NetworkMain class.
Basically, I want to separate my server from TaleWorld's server. (NetworkMain class is connecting to the lobby server of TaleWorlds)
## Structure of Project
 ### Projects:
  - TWNetwork (Main project, where implemented interfaces and the server and client classes are found.)
  - TWNetworkPatcher (InterfaceImplementer and MethodPatcher is found here.)
  - TWNetworkTestMod (Test Mod to Test Missions.)
  - TWNetworkTests (Unit Tests for InterfaceImplementer and MethodPatcher.)
  - DebugMod1 (Used for debugging.)
  - DebugMod2 (Used for debugging.)
## Already Done
- Created MethodPatcher that uses Harmony to patch out methods with attributes and deriving from a base class.
- Created InterfaceImplementer to implement the IMBNetwork and IMBPeer internal interfaces.
- Written Unit Tests for the MethodPatcher and InterfaceImplementer.
- Created Client and Server classes as well as implemented the IMBNetwork and IMBPeer interfaces, which uses the Client and Server classes.
- Written some extensions and helpers for the Server and Client classes.
- Created a test project to test the main project in missions.
## In Progress
- Find the problem on Agent spawning on client side. For some reason, the engine code throws exception on client side when spawning an agent. (IMBAgent.Build method)
- Refactor the project for easier usability.
- Write comments for the main projects.
## Plan
- Create MethodPatcher that uses Harmony to patch out methods easily and in a better form. (done)
- Create InterfaceImplementer to implement the IMBNetwork and IMBPeer internal interfaces. (done)
- Write Unit Tests for the MethodPatcher and InterfaceImplementer. (done)
- Create Client and Server classes as well as implemented the IMBNetwork and IMBPeer interfaces, which uses the Client and Server classes. (done)
- Write some extensions and helpers for the Server and Client classes. (done)
- Create a test project to test the main project in missions. (done)
- Fix bugs in the test project.
- Comment the main projects.
- Refactor the project.
