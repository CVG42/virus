# Project Setup
This provides an overview of the way the project was set up. It also describes the technologies that were used and the packages that need to be added to the project.

## GitHub repository
Before creating the Unity project, the GitHub repository was set up. Every developer will use [Fork](https://git-fork.com/) as their Git client in a desktop environment.

### Guidelines
- Changes must not be made directly to the main branch, first the developers must create a branch and commit their changes.
- In order to update main each developer must make a pull request, the changes will be reviewed and approved. Once their changes are approved they can Squash and merge into main.
- Each developer must keep their branches updated to main and rebase if necessary.
- In case there are conflicts while rebasing, they must contact the other person who made the changes and ask them about it.
- Whenever making a Pull Request they must specify which issue they're working on and close the corresponding number.

## Unity Project
The Unity version used for the project is 2022.3.54f1, based on the team’s preferences. The project was created in a 3D URP template. Once the editor window is open, the folder structure will follow this arrangement:
```
├── _Project
    ├─ Art
    ├─ Prefabs
    ├─ Scenes
    ├─ Scriptable Objects
    ├─ Scripts
    ├─ Shaders

├── _Utilities

├── Settings

├── ScriptTemplates
```
The `_Project` folder is used as the project's main container, any asset and script related to the game goes in there. The `_Utilities` folder contains general use tools and scripts while the `ScriptTemplate` folder is used for a single script dedicated to formatting the content of scripts when they are created.

## Singleton
We'll be using the Singleton design pattern. This singleton is generic driven interface, meaning their public methods will be defined in the interface. Every manager script must implement this pattern and its own interface. In order to do that, we included the following script:
```csharp
public class Singleton<I> : MonoBehaviour where I : class
{
  public static I Source { get; private set; }

  [SerializeField] private bool _isPersistent = false;

  protected bool _hasBeenDestroyed = false;

  protected virtual void Awake()
  {
    if (_isPersistent && Source != null)
    {
      DestroyImmediate(gameObject);
      _hasBeenDestroyed = true;
      return;
    }

    Source = this as I;

    if (_isPersistent)
    {
      DontDestroyOnLoad(gameObject);
    }
  }
}
```

## Packages
These are the packages that must be installed right away:
- [DOTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676)
- [UniTask](https://github.com/Cysharp/UniTask)
