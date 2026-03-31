## B.E.A.N - the name doesn't mean anything; But the engine does.

<img width="2560" height="1253" alt="thumbnail" src="https://github.com/user-attachments/assets/95c657af-1316-4a2e-bb30-276e0275c183" />

## The story
Three years ago I got curious about what actually makes a game so I did some reasearch and I'm not that brave, so I went into this project knowing that I should make use of a framework. When it comes to making engines in C#, the options were pretty limited; whilst lots of engines use C# for their scripts, most of them are built on C++.

During my research, I found out that Microsoft once created a game framework called XNA for Windows and the Xbox 360 using C#, as they created the language. The issue was that XNA was so dead in the water that it had met with the Titanic; well, at least that's what I thought.

The beauty of open source cannot be understated is the opinion I immediately gained once I discovered MonoGame. A 100% free and open-source revival of XNA with incredible documentation and an even better selection of games; take Stardew Valley as an example. It didn't take much convincing for me to start working on my engine using MonoGame.

## The workflow

### Scenes

If I wanted to create a player that could move around and collide I would first need to create a scene class.

```cs
public class TestScene : Scene
{
  public TestScene(string name) : base(name)
  {
  }

  public override LoadScene()
  {
    base.LoadScene();

    //Scene Loading Here
  }
}
```

Once that is done I then need to add it to the game, load it and activate it. In the game class:

```cs
  TestScene testScene = new TestScene("SceneName");
  
  SceneManager.AddScene(testScene);
  SceneManager.LoadScene("SceneName");
  SceneManager.SetActiveScene("SceneName")
```

Now that is done I can begin working on the player

### World Props and Editor

World Props are a contaier for "addons". They act as the manager for addons, ensuring that all the addons can refrence eachother and works in resolving any addon dependency or disabling addons that would otherwise cause a crash. 

The Prop editor which can be opened as a seperate instance or by pressing F12 + E in game allows me to create and edit props to be use in game.

<img width="1289" height="750" alt="image" src="https://github.com/user-attachments/assets/2fb76595-7944-4bbe-9720-1023c8f22fe3" />

Once opened I am presented with an empty prop and some option to load an existing one. For now I'll use the empty.

<img width="1289" height="750" alt="image" src="https://github.com/user-attachments/assets/a239ed90-06ed-4851-9323-023b6e3c174f" />

Clicking on add Addon will present me with a list of addons supported by the prop editor. I'll add a sprite and player controller

<img width="1289" height="750" alt="image" src="https://github.com/user-attachments/assets/9333616e-1a66-4332-9454-014dfd270e78" />

In the right-hand sidebar I am presented with options for the sprite addon. Filling in the texture path option with a file from the textures folder will allow it to be rendered in the editor window. The player controller needs no tweaks for now but has options for speed. Pressing CTL+S will prompt me to save the prop for use in game

<img width="1289" height="750" alt="image" src="https://github.com/user-attachments/assets/120d425b-801b-449e-a8e1-70f5ca5c8eee" />

A green top bar means all changes have been saved.

### Loading Props

Now assuming I saved the prop to the props folder to load the player into the game I just need 3 lines in the "LoadScene" function.

```cs
  WorldProp playerProp = FileManager.LoadWorldPropFromFile("Player");
  this.AddToScene(playerProp);
```

And that's it. Running the game will now show the player and I will be able to move it.

## A demo of me playing with the prop editor and hot reload


https://github.com/user-attachments/assets/fdd15163-031e-4913-abcd-6b426979bd11









