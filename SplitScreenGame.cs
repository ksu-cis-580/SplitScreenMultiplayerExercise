using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CIS580.GameScreens;

namespace SplitScreenExercise;

/// <summary>
/// Represents the main game class for the split-screen example. This class is responsible for initializing, updating, and drawing the game.
/// </summary>
public class SplitScreenExampleGame : Game
{
    /// <summary>
    /// The graphics device manager is responsible for managing the graphics device and its settings.
    /// </summary>
    private GraphicsDeviceManager _graphics;

    /// <summary>
    /// The sprite batch is used to draw 2D sprites to the screen.
    /// </summary>
    private SpriteBatch _spriteBatch;

    /// <summary>
    /// The screen manager is responsible for managing the different screens in the game.
    /// </summary>  
    private ScreenManager _screenManager; 

    

    /// <summary>
    /// Initializes a new instance of the <see cref="SplitScreenExampleGame"/> class.
    /// </summary>
    public SplitScreenExampleGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    /// <summary>
    /// Initializes the game and its components. This method is called after the game is constructed and before the game loop starts.
    /// </summary>
    protected override void Initialize()
    {
        // Initialize the ScreenManager and add it to the game's components list.
        _screenManager = new ScreenManager(this);
        Components.Add(_screenManager);

        // Add the initial screen (StartScreen) to the ScreenManager.
        _screenManager.AddScreen(new StartScreen(), null);

        base.Initialize();
    }

    /// <summary>
    /// Loads the game content, such as textures, sounds, and other assets. This method is called once per game and is used to load all necessary resources.
    /// </summary>
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    /// <summary>
    /// Updates the game state based on the elapsed game time.
    /// </summary>
    /// <param name="gameTime">The elapsed game time.</param>
    protected override void Update(GameTime gameTime)
    {
        // Global exit condition - useful for debugging and quick exits during development
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Everything else is handled by the ScreenManager, so we don't need to do anything else here.

        base.Update(gameTime);
    }

    /// <summary>
    /// Draws the game content to the screen. This method is called once per frame and is responsible for rendering the game's visuals.
    /// </summary>
    /// <param name="gameTime">The elapsed game time.</param>
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Everything else is handled by the ScreenManager, so we don't need to do anything else here.

        base.Draw(gameTime);
    }
}
