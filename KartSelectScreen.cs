using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CIS580.GameScreens;
using CIS580.Input; 

namespace SplitScreenExercise;

/// <summary>
/// Represents the kart selection screen where players can choose their karts before starting the game.
/// </summary>;
public class KartSelectScreen : GameScreen
{
    private SpriteBatch _spriteBatch;
    private SpriteFont _instructionFont;
    private Vector2 _instructionPositionLine1;
    private string _instructionLine1 = "Select your kart!";
    private Vector2 _instructionPositionLine2;
    private string _instructionLine2 = "Use the arrow keys or gamepad.";

    private Texture2D _kartTexture;
    private Vector2 _kartPosition;
    private int _currentKartIndex = 0;

    // The karts sit on an endless horizontal strip, one viewport width apart.
    // _targetSlot is the slot we're moving to; _slot eases toward it each frame,
    // so rapid presses just retarget the slide instead of restarting it.
    private int _targetSlot = 0;

    // The current slot, which eases toward _targetSlot each frame. This is a float so we can slide smoothly between slots.
    private float _slot = 0;

    // How quickly the strip catches up to its target (higher is snappier)
    private const float SlideSpeed = 12f;

    // Colors available for karts; the index into this array is the kart's "type"
    private readonly Color[] _availableColors = new Color[]
    {
        Color.Red,
        Color.Blue,
        Color.Green,
        Color.Yellow,
        Color.Purple,
        Color.Orange,
        Color.Pink
    };
    
    // Defines the input action to select the kart to the left.
    private InputAction _selectLeft = new InputAction(
        new[] { Buttons.DPadLeft, Buttons.LeftThumbstickLeft },
        new[] { Keys.Left },
        true);

    // Defines the input action to select the kart to the right.
    private InputAction _selectRight = new InputAction( 
        new[] { Buttons.DPadRight, Buttons.LeftThumbstickRight },
        new[] { Keys.Right },
        true);

    private InputAction _startGame = new InputAction(
        new[] { Buttons.Start, Buttons.A },
        new[] { Keys.Enter },
        true);

    /// <summary>
    /// Initializes a new instance of the <see cref="KartSelectScreen"/> class.
    /// </summary>
    public KartSelectScreen()
    {
        TransitionOnTime = TimeSpan.FromSeconds(0.5);
        TransitionOffTime = TimeSpan.FromSeconds(0.5);
    }

    /// <summary>
    /// Loads the content for the <see cref="KartSelectScreen"/> class.
    /// </summary>
    public override void LoadContent()
    {
        _instructionFont = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/InstructionFont");
        _kartTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/racer");
        _spriteBatch = ScreenManager.SpriteBatch;

        _kartPosition = new Vector2((ScreenManager.GraphicsDevice.Viewport.Width - _kartTexture.Width) / 2, 100);
        _instructionPositionLine1 = new Vector2((ScreenManager.GraphicsDevice.Viewport.Width - _instructionFont.MeasureString(_instructionLine1).X) / 2, 150 + _kartTexture.Height);
        _instructionPositionLine2 = new Vector2((ScreenManager.GraphicsDevice.Viewport.Width - _instructionFont.MeasureString(_instructionLine2).X) / 2, _instructionPositionLine1.Y + _instructionFont.LineSpacing);
    }

    /// <summary>
    /// Handles the input for the <see cref="KartSelectScreen"/> class.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    /// <param name="input">The input state.</param>
    public override void HandleInput(GameTime gameTime, InputState input)
    {
        base.HandleInput(gameTime, input);

        PlayerIndex playerIndex;

        if (IsActive && _selectLeft.Occurred(input, null, out playerIndex))
        {
            _targetSlot--;
            _currentKartIndex = Wrap(_targetSlot);
        }
        else if (IsActive && _selectRight.Occurred(input, null, out playerIndex))
        {
            _targetSlot++;
            _currentKartIndex = Wrap(_targetSlot);
        }

        if(IsActive && _startGame.Occurred(input, null, out playerIndex))
        {
            // Transition to the gameplay screen with the selected kart color
            Color selectedColor = _availableColors[_currentKartIndex];
            ScreenManager.AddScreen(new GameplayScreen(selectedColor, playerIndex), playerIndex);
            ExitScreen();
        }
    }

    /// <summary>
    /// Updates the <see cref="KartSelectScreen"/>. Animates sliding karts during selection.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    /// <param name="otherScreenHasFocus">True if another screen has focus.</param>
    /// <param name="coveredByOtherScreen">True if this screen is covered by another screen.</param>
    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

        // Ease toward the target slot; frame-rate independent exponential smoothing
        float t = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _slot = MathHelper.Lerp(_slot, _targetSlot, 1 - MathF.Exp(-SlideSpeed * t));
        if (MathF.Abs(_targetSlot - _slot) < 0.001f) _slot = _targetSlot;
    }

    /// <summary>
    /// Maps a slot on the endless strip to an index into the available colors
    /// </summary>
    private int Wrap(int slot)
    {
        return slot % _availableColors.Length;
    }

    /// <summary>
    /// Draws the KartSelectScreen. 
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    public override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin();
        // At most two karts are on screen mid-slide: the slots on either side of _slot
        float width = ScreenManager.GraphicsDevice.Viewport.Width;
        int left = (int)MathF.Floor(_slot);
        for (int slot = left; slot <= left + 1; slot++)
        {
            var position = _kartPosition + new Vector2((slot - _slot) * width, 0);
            _spriteBatch.Draw(_kartTexture, position, _availableColors[Wrap(slot)]);
        }
        _spriteBatch.DrawString(_instructionFont, _instructionLine1, _instructionPositionLine1, Color.White);
        _spriteBatch.DrawString(_instructionFont, _instructionLine2, _instructionPositionLine2, Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);


        // Fade in from black as the screen transitions on (and out as it exits)
        if (TransitionPosition > 0) ScreenManager.FadeBackBufferToBlack(TransitionPosition);

    }
}