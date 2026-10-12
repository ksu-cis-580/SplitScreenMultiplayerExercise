using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CIS580.Input;

namespace CIS580.GameScreens;

/// <summary>
/// The ScreenManager is a component which manages one or more GameScreen instance.
/// It maintains a stack of screens, calls their Update and Draw methods when 
/// appropriate, and automatically routes input to the topmost screen.
/// </summary>
public class ScreenManager : DrawableGameComponent
{
    private readonly List<GameScreen> _screens = new List<GameScreen>();
    private readonly List<GameScreen> _tmpScreensList = new List<GameScreen>();

    private readonly InputState _input = new InputState();

    private bool _isInitialized;

    /// <summary>
    /// A SpriteBatch shared by all GameScreens
    /// </summary>
    public SpriteBatch SpriteBatch { get; private set; }

    /// <summary>
    /// A blank texture that can be used by the screens.
    /// </summary>
    public Texture2D BlankTexture { get; private set; }

    /// <summary>
    /// Constructs a new ScreenManager
    /// </summary>
    /// <param name="game">The game this ScreenManager belongs to</param>
    public ScreenManager(Game game) : base(game) { }

    /// <summary>
    /// Initializes the ScreenManager
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        _isInitialized = true;
    }

    /// <summary>
    /// Loads content for the ScreenManager and its screens
    /// </summary>
    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        // Programatically create a 1x1 white texture that can be used for drawing
        BlankTexture = new Texture2D(GraphicsDevice, 1, 1);
        BlankTexture.SetData(new[] { Color.White });

        // Tell each of the screens to load their content 
        foreach (var screen in _screens)
        {
            screen.LoadContent();
        }
    }

    /// <summary>
    /// Unloads content for the ScreenManager's screens
    /// </summary>
    protected override void UnloadContent()
    {
        foreach (var screen in _screens)
        {
            screen.Unload();
        }
    }

    /// <summary>
    /// Updates all screens managed by the ScreenManager
    /// </summary>
    /// <param name="gameTime">An object representing time in the game</param>
    public override void Update(GameTime gameTime)
    {
        // Read the keyboard and gamepads once per frame, before any screen handles input
        _input.Update();

        // Make a copy of the screen list, to avoid confusion if 
        // the process of updating a screen adds or removes others
        _tmpScreensList.Clear();
        _tmpScreensList.AddRange(_screens);

        bool otherScreenHasFocus = !Game.IsActive;
        bool coveredByOtherScreen = false;

        while (_tmpScreensList.Count > 0)
        {
            // Pop the topmost screen 
            var screen = _tmpScreensList[_tmpScreensList.Count - 1];
            _tmpScreensList.RemoveAt(_tmpScreensList.Count - 1);

            screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.Active)
            {
                // if this is the first active screen, let it handle input 
                if (!otherScreenHasFocus)
                {
                    screen.HandleInput(gameTime, _input);
                    otherScreenHasFocus = true;
                }

                // if this is an active non-popup, all subsequent 
                // screens are covered 
                if (!screen.IsPopup) coveredByOtherScreen = true;
            }
        }
    }

    /// <summary>
    /// Draws the appropriate screens managed by the SceneManager
    /// </summary>
    /// <param name="gameTime">An object representing time in the game</param>
    public override void Draw(GameTime gameTime)
    {
        foreach (var screen in _screens)
        {
            if (screen.ScreenState == ScreenState.Hidden) continue;

            screen.Draw(gameTime);
        }
    }

    /// <summary>
    /// Adds a screen to the ScreenManager
    /// </summary>
    /// <param name="screen">The screen to add</param>
    public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
    {
        screen.ControllingPlayer = controllingPlayer;
        screen.ScreenManager = this;
        screen.IsExiting = false;

        // If we have a graphics device, tell the screen to load content
        if (_isInitialized) screen.LoadContent();

        _screens.Add(screen);
    }

    /// <summary>
    /// Removes a screen from the ScreenManager.
    /// </summary>
    /// <param name="screen">The screen to remove</param>
    public void RemoveScreen(GameScreen screen)
    {
        // If we have a graphics device, tell the screen to unload its content 
        if (_isInitialized) screen.Unload();

        _screens.Remove(screen);
        _tmpScreensList.Remove(screen);
    }

    /// <summary>
    /// Exposes an array holding all the screens managed by the ScreenManager
    /// </summary>
    /// <returns>An array containing references to all current screens</returns>
    public GameScreen[] GetScreens()
    {
        return _screens.ToArray();
    }

    /// <summary>
    /// Draws a translucent black fullscreen sprite, used for fading
    /// screens in and out, and for darkening the background behind popups.
    /// </summary>
    /// <param name="alpha">The opacity of the black overlay (0 to 1)</param>
    public void FadeBackBufferToBlack(float alpha)
    {
        SpriteBatch.Begin();
        SpriteBatch.Draw(BlankTexture, GraphicsDevice.Viewport.Bounds, Color.Black * alpha);
        SpriteBatch.End();
    }
}
