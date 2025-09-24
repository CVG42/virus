# Game flow creation
This is a breakdown on how the game flow was created for the project. It can be used a guide for future projects or as a reference for future optimization, corrections and/or adjustments.
### Content
- [Figma prototype and screens](#figma-prototype-and-screens)
- [Development requirements](#development-requirements)
- [Game Flow Manager](#game-flow-manager)
- [Game Manager](#game-manager)
- [UI Manager](#ui-manager)

## Figma prototype and screens
Before coding right away, a basic game flow wireframe was created using Figma. You can checkout the prototype [here](https://www.figma.com/proto/uYfUzyfZ0ECDTcTxifLfHQ/Virus-Game-Flow?node-id=1-3&p=f&t=rzIAVQdL8BlytL9J-1&scaling=min-zoom&content-scaling=fixed&page-id=0%3A1&starting-point-node-id=1%3A3).

The game flow consists of the following screens and their main elements:
- Splash screen
  - Unity logo
  - Amerike logo
- Main menu
  - Start button
  - Settings button
  - Exit button
- In-Game screen
  - HUD (not implemented here yet since we're only getting the flow right).
  NOTE FOR FIGMA'S PROTOTYPE PURPOSES, THE GAME OVER AND PAUSE BUTTONS WHERE ADDED. THEY WON'T BE PRESENT IN-GAME.
- Pause
  - Resume button
  - Settings button
  - Back to main menu button
- Settings
  - Music slider
  - SFX slider
  - Resolution dropdown
  - Fullscreen or windowed toggle 
  - Back button
- Credits
  - Credits text. NOTE AN END CREDITS BUTTON WAS ADDED FOR THE FIGMA PROTOTYPE, IN THE GAME THE PLAYER IS REDIRECTED TO THE MAIN MENU SCREEN AFTER THE CREDITS FINISH.

![Game flow wireframe image](../images/gameflow.png)

## Development requirements
In order to start coding, the following packages need to be included in the Unity project:
- [DOTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676)
- [UniTask](https://github.com/Cysharp/UniTask)

## Game Flow Manager
According to the [Game Structure](https://github.com/CVG42/virus/blob/game-flow-dev/dev-log/Week%201/Game%20Structure), the managers will be using the singleton design pattern, so we created the `FlowManager.cs` and its respective interface `IFlowSource.cs`. The flow manager has the function of changing scenes whenever it's required using an async method and it's also in control of the scene transition effect.

As for now we have a very simple fade scene transition made with DOTWeen and its property `DOFade`, in which we specify the initial and finaly opacity and its transition time.
```
private async UniTask LoadSceneAsync(string sceneName)
{
    _fadeTransitionCanvasGroup.gameObject.SetActive(true);
    await _fadeTransitionCanvasGroup.DOFade(1, 2).AsyncWaitForCompletion();
    await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single).ToUniTask();
    await _fadeTransitionCanvasGroup.DOFade(0, 1.5f).AsyncWaitForCompletion();
    _fadeTransitionCanvasGroup.gameObject.SetActive(false);
}
```
> [!NOTE]
> Remember that the canvas reference in the code must be a `CanvasGroup` and it needs the component `CanvasGroup` in the inspector since `DOFade` takes its opacity as reference.

## Game Manager
In many games, a `GameManager.cs` script is needed in order to keep control of the states of the game. The different states can listed as an enum. In this case we have the following:
```
public enum GameState
{
    OnPlay,
    OnPause,
    OnGameOver,
    OnMenu,
    OnAntivirusEvent,
    OnTyping
}
```
This script has the function of chaging the state of the game when needed with the use of an event indicating the change of the state and a direct reference to the actual game state `GameState CurrentGameState { get; private set; }`. That way other scripts can change the state or do things they need according to certain states.

> [!TIP]
> Not every game uses the game manager this way, some other manager their player's HP, score and other stuff here. Or they simply don't have a game manager at all.

> [!NOTE]
> The game manager of this project will continue to expand as the game develops. 

## UI Manager
The UI Manager in this project controls the interactive UI elements of the game. 
