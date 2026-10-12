using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CIS580.Input;

/// <summary>
/// Tracks the current and prior frame's keyboard and gamepad states, so that
/// we can tell both whether a key or button is held down, and whether it
/// was newly pressed this frame.
/// </summary>
/// <remarks>
/// The game should call Update() once at the start of each frame.
/// </remarks>
public class InputState
{
    // The default maximum number of player devices
    // Typically 4 controllers
    private const int MaxInputs = 4;

    // Current GamePad and Keyboard states
    public readonly GamePadState[] CurrentGamePadStates;
    public KeyboardState CurrentKeyboardState { get; private set; }

    // Prior frame GamePad and Keyboard states
    private readonly GamePadState[] _priorGamePadStates;
    private KeyboardState _priorKeyboardState;

    /// <summary>
    /// Tracks whether a GamePad was ever connected for each player index
    /// </summary>
    public readonly bool[] GamePadWasConnected;

    /// <summary>
    /// Initializes a new instance of the InputState class.
    /// </summary>
    public InputState()
    {
        CurrentGamePadStates = new GamePadState[MaxInputs];
        _priorGamePadStates = new GamePadState[MaxInputs];
        
        GamePadWasConnected = new bool[MaxInputs];
    }

    /// <summary>
    /// Updates the input state for the current frame.
    /// </summary>
    public void Update()
    {
        for (var i = 0; i < MaxInputs; i++)
        {
            _priorGamePadStates[i] = CurrentGamePadStates[i];
            CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

            if (CurrentGamePadStates[i].IsConnected)
                GamePadWasConnected[i] = true;
        }

        _priorKeyboardState = CurrentKeyboardState;
        CurrentKeyboardState = Keyboard.GetState();
    }

    /// <summary>
    /// Determines if a key is currently held down.
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <returns>True if pressed, False otherwise</returns>
    public bool IsKeyPressed(Keys key)
    {
        return CurrentKeyboardState.IsKeyDown(key);
    }

    /// <summary>
    /// Determines if a button is currently held down.
    /// </summary>
    /// <param name="button">The button to check for</param>
    /// <param name="controllingPlayer">An optional player index to check</param>
    /// <param name="playerIndex">The player index the button was pressed by</param>
    /// <returns>True if pressed, False otherwise</returns>
    public bool IsButtonPressed(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
    {
        // If a player index was specified, check if that player pressed the button
        if (controllingPlayer.HasValue)
        {
            playerIndex = controllingPlayer.Value;
            int i = (int)playerIndex;
            return CurrentGamePadStates[i].IsButtonDown(button);
        }

        // Otherwise, check all players for the button press
        return IsButtonPressed(button, PlayerIndex.One, out playerIndex) ||
               IsButtonPressed(button, PlayerIndex.Two, out playerIndex) ||
               IsButtonPressed(button, PlayerIndex.Three, out playerIndex) ||
               IsButtonPressed(button, PlayerIndex.Four, out playerIndex);
    }

    /// <summary>
    /// Determines if a key was newly pressed during this frame.
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <returns>True if pressed, False otherwise</returns>
    public bool IsNewKeyPress(Keys key)
    {
        return CurrentKeyboardState.IsKeyDown(key) && _priorKeyboardState.IsKeyUp(key);
    }

    /// <summary>
    /// Determines if a button was newly pressed during this frame.
    /// </summary>
    /// <param name="button">The button to check for</param>
    /// <param name="controllingPlayer">An optional player index to check</param>
    /// <param name="playerIndex">The player index the button was pressed by</param>
    /// <returns>True if newly pressed, False otherwise</returns>
    public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
    {
        // If a player index was specified, check if that player pressed the button
        if (controllingPlayer.HasValue)
        {
            playerIndex = controllingPlayer.Value;
            int i = (int)playerIndex;
            return CurrentGamePadStates[i].IsButtonDown(button) && _priorGamePadStates[i].IsButtonUp(button);
        }

        // Otherwise, check all players for the button press
        return IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
               IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
               IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
               IsNewButtonPress(button, PlayerIndex.Four, out playerIndex);
    }
}