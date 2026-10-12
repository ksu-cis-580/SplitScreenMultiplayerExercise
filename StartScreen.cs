using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CIS580.GameScreens;
using CIS580.Input; 

namespace SplitScreenExercise;

/// <summary>
/// Represents the start screen (the first screen the player sees).
/// Displays the game title and instructions to start the game.
/// </summary>
public class StartScreen : GameScreen
{
    private SpriteFont _titleFont;
    private string _title = "Go Kart Racing!";
    private SpriteFont _instructionFont;
    private string _instructions = "Press Enter or Start Button to Start";

    /// <summary>
    /// Defines the input action to start the game.
    /// </summary>
    private static readonly InputAction StartGameAction = new InputAction(
        new[] { Buttons.Start, Buttons.A },
        new[] { Keys.Enter },
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

        if (IsActive && StartGameAction.Occurred(input, null, out playerIndex))
        {
            // Transition to the next screen 
            ScreenManager.AddScreen(new KartSelectScreen(), playerIndex);
            ExitScreen();
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

        // Draw the instructions centered below the title
        spriteBatch.DrawString(_instructionFont, _instructions, new Vector2((viewport.Width - instructionsSize.X) / 2, viewport.Height / 2), Color.White);

        spriteBatch.End();
    }
}