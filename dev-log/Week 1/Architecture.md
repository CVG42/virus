# CODE ARCHITECTURE
Decided to keep it simple for the architecture since this project is pretty small. A Game Manager will be the core of the game. Why? Because it'll manage every state of the game and statistic such as the player's score. Every other manager will communicate with it. These are the principal managers the game will have:
- __Flow Manager__: responsible of scene changing and transitions, it'll keep in check the flow of the game.
- __Input Manager__: every input is managed in here.
- __UI Manager__: it's in charge of every interactive UI element, menu controllers and keeping the HUD updated during gameplay.
- __Application Manager__: it's more of a saving system for the game's settings.
- __Level Manager__: controls the platforms and checkpoints around the level. It's the main bridge between the player with the level itself.
- __Enemy Manager__: manages the enemies of the game.
- __Typing Manager__: it always communicates with the Input Manager so the player can type and manages every typing event that happens in the game.
- __Audio Manager__: will have an audio library where all of the game sounds and music will be stored. It'll manage the way music and SFX play and their volume, which means it'll communicate with the Application Manager.

Almost every manager has its respective controllers and more can derivate as the game develops, but this is for now a blueprint of the core. Here's a short and simple visualization of how the architecture is designed.

![Code architecture](../images/architecture.png)
