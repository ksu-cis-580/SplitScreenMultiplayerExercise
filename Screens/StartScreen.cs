using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CIS580.GameScreens;
using CIS580.Input; 

namespace SplitScreenExercise.Screens;

/// <summary>
/// Represents the start screen (the first screen the player sees).
/// Displays the game title and a menu for choosing the number of players.
/// </summary>
public class StartScreen : GameScreen
{
    private SpriteFont _titleFont;
    private string _title = "Go Kart Racing!";
    private SpriteFont _instructionFont;
    private string _instructions = "Use the arrow keys or gamepad, then press Enter or Start";

    // The menu options, in the order they are displayed
    private readonly string[] _menuOptions = new string[]
    {
        "Single Player (keyboard or gamepad)",
        "Two Player (keyboard or gamepad)",
        "Four Player (gamepad only)"
    };

    // The index of the currently highlighted menu option
    private int _selectedOption = 0;

    /// <summary>
    /// Defines the input action to start the game.
    /// </summary>
    private static readonly InputAction StartGameAction = new InputAction(
        new[] { Buttons.Start, Buttons.A },
        new[] { Keys.Enter },
        true);

    /// <summary>
    /// Defines the input action to move the menu selection up.
    /// </summary>
    private static readonly InputAction MenuUpAction = new InputAction(
        new[] { Buttons.DPadUp, Buttons.LeftThumbstickUp },
        new[] { Keys.Up },
        true);

    /// <summary>
    /// Defines the input action to move the menu selection down.
    /// </summary>
    private static readonly InputAction MenuDownAction = new InputAction(
        new[] { Buttons.DPadDown, Buttons.LeftThumbstickDown },
        new[] { Keys.Down },
        true);

    /// <summary>
    /// Initializes a new instance of the <see cref="StartScreen"/> class.
    /// </summary>
    public StartScreen()
    {
        TransitionOnTime = TimeSpan.FromSeconds(1.5);
        TransitionOffTime = TimeSpan.FromSeconds(0.15);
    }

    public override void LoadContent()
    {
        _titleFont = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/TitleFont");
        _instructionFont = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/InstructionFont");
    }

    public override void HandleInput(GameTime gameTime, InputState input)
    {
        base.HandleInput(gameTime, input);

        PlayerIndex playerIndex;

        if (IsActive && MenuUpAction.Occurred(input, null, out playerIndex))
        {
            _selectedOption = (_selectedOption - 1 + _menuOptions.Length) % _menuOptions.Length;
        }
        else if (IsActive && MenuDownAction.Occurred(input, null, out playerIndex))
        {
            _selectedOption = (_selectedOption + 1) % _menuOptions.Length;
        }

        if (IsActive && StartGameAction.Occurred(input, null, out playerIndex))
        {
            switch (_selectedOption)
            {
                case 0: // Single Player
                    // Transition to the next screen 
                    ScreenManager.AddScreen(new KartSelectScreen(), playerIndex);
                    ExitScreen();
                    break;
                case 1: // Two Player
                    // TODO: Launch two player mode
                    break;
                case 2: // Four Player
                    // TODO: Launch four player mode
                    break;
            }
        }
    }

    public override void Draw(GameTime gameTime)
    {
        SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
        Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
        Vector2 titleSize = _titleFont.MeasureString(_title);
        Vector2 instructionsSize = _instructionFont.MeasureString(_instructions);

        spriteBatch.Begin();

        // Draw the title centered on the screen
        spriteBatch.DrawString(_titleFont, _title, new Vector2((viewport.Width - titleSize.X) / 2, viewport.Height / 4), Color.White);

        // Draw the menu options centered below the title, highlighting the selected one
        float y = viewport.Height / 2;
        for (int i = 0; i < _menuOptions.Length; i++)
        {
            Vector2 optionSize = _instructionFont.MeasureString(_menuOptions[i]);
            Color color = i == _selectedOption ? Color.Yellow : Color.White;
            spriteBatch.DrawString(_instructionFont, _menuOptions[i], new Vector2((viewport.Width - optionSize.X) / 2, y), color);
            y += _instructionFont.LineSpacing;
        }

        // Draw the instructions centered below the menu
        y += _instructionFont.LineSpacing;
        spriteBatch.DrawString(_instructionFont, _instructions, new Vector2((viewport.Width - instructionsSize.X) / 2, y), Color.White);

        spriteBatch.End();


        // Fade in from black as the screen transitions on (and out as it exits)
        if (TransitionPosition > 0) ScreenManager.FadeBackBufferToBlack(TransitionPosition);

    }
}