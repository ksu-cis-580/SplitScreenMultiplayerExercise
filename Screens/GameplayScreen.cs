using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CIS580.GameScreens;
using CIS580.Input;

namespace SplitScreenExercise.Screens;

/// <summary>
/// The main gameplay screen: draws the track with the camera centered on the player's kart.
/// </summary>
public class GameplayScreen : GameScreen
{
    private SpriteBatch _spriteBatch;
    private Texture2D _backgroundTexture;
    private Texture2D _kartTexture;
    private Vector2 _kartVelocity;

    private readonly Color _kartColor;

    private readonly PlayerIndex _playerIndex;

    // The kart's position and heading in track (world) coordinates.
    // It starts just past the start/finish line on the bottom straight.
    private Vector2 _kartPosition = new Vector2(1980, 2985);

    // The racer sprite points up, so rotate a quarter turn to face along the straight
    private float _kartRotation = MathHelper.PiOver2 + 0.12f;

    // Top speed of the kart, in pixels per second
    private const float KartSpeed = 600f;

    // Camera scale: below 1 zooms out, showing more of the track around the kart
    private const float CameraZoom = 0.5f;

    // Digital steering for the keyboard (arrows or WASD) and the gamepad's D-pad.
    // The left thumbstick is read directly in HandleInput for analog control.
    private readonly InputAction _moveUp = new InputAction(
        new[] { Buttons.DPadUp }, new[] { Keys.Up, Keys.W }, false);
    private readonly InputAction _moveDown = new InputAction(
        new[] { Buttons.DPadDown }, new[] { Keys.Down, Keys.S }, false);
    private readonly InputAction _moveLeft = new InputAction(
        new[] { Buttons.DPadLeft }, new[] { Keys.Left, Keys.A }, false);
    private readonly InputAction _moveRight = new InputAction(
        new[] { Buttons.DPadRight }, new[] { Keys.Right, Keys.D }, false);

    /// <summary>
    /// Constructs a new GameplayScreen
    /// </summary>
    /// <param name="kartColor">The tint for the player's kart (chosen on the KartSelectScreen)</param>
    /// <param name="playerIndex">The index of the player who is playing</param>
    public GameplayScreen(Color kartColor, PlayerIndex playerIndex)
    {
        _kartColor = kartColor;
        _playerIndex = playerIndex;
        TransitionOnTime = TimeSpan.FromSeconds(0.5);
        TransitionOffTime = TimeSpan.FromSeconds(0.5);
    }

    public override void LoadContent()
    {
        _backgroundTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/track");
        _kartTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/racer");
        _spriteBatch = ScreenManager.SpriteBatch;
    }

    public override void HandleInput(GameTime gameTime, InputState input)
    {
        base.HandleInput(gameTime, input);

        // Thumbstick Y is positive when pushed up, but screen Y grows downward, so flip it
        Vector2 stick = input.CurrentGamePadStates[(int)_playerIndex].ThumbSticks.Left;
        Vector2 direction = new Vector2(stick.X, -stick.Y);

        // Keyboard and D-pad add full-strength steering in each direction
        if (_moveUp.Occurred(input, _playerIndex, out _)) direction.Y -= 1;
        if (_moveDown.Occurred(input, _playerIndex, out _)) direction.Y += 1;
        if (_moveLeft.Occurred(input, _playerIndex, out _)) direction.X -= 1;
        if (_moveRight.Occurred(input, _playerIndex, out _)) direction.X += 1;

        // Cap at full speed so diagonals (or stick + keys together) aren't faster
        if (direction.LengthSquared() > 1) direction.Normalize();

        _kartVelocity = direction * KartSpeed;
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

        // Update the kart's position based on its velocity
        _kartPosition += _kartVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Update the kart's rotation based on its velocity
        if (_kartVelocity.LengthSquared() > 0)
        {
            // The sprite points up (-Y) at rotation 0, so measure the heading from that axis
            _kartRotation = MathF.Atan2(_kartVelocity.X, -_kartVelocity.Y);
        }
    }

    public override void Draw(GameTime gameTime)
    {
        var viewport = ScreenManager.GraphicsDevice.Viewport;

        // Move the kart to the origin, zoom out around it, then shift it to the middle of the viewport
        Matrix camera = Matrix.CreateTranslation(-_kartPosition.X, -_kartPosition.Y, 0)
            * Matrix.CreateScale(CameraZoom)
            * Matrix.CreateTranslation(viewport.Width / 2f, viewport.Height / 2f, 0);

        _spriteBatch.Begin(transformMatrix: camera);
        _spriteBatch.Draw(_backgroundTexture, Vector2.Zero, Color.White);

        // Draw the kart around its center so it rotates in place
        var kartOrigin = new Vector2(_kartTexture.Width / 2f, _kartTexture.Height / 2f);
        _spriteBatch.Draw(_kartTexture, _kartPosition, null, _kartColor, _kartRotation, kartOrigin, 1f, SpriteEffects.None, 0);
        _spriteBatch.End();

        // Fade in from black as the screen transitions on (and out as it exits)
        if (TransitionPosition > 0) ScreenManager.FadeBackBufferToBlack(TransitionPosition);

        base.Draw(gameTime);
    }
}
