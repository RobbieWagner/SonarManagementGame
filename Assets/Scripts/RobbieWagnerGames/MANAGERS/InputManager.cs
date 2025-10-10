using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using RobbieWagnerGames.Utilities;

namespace RobbieWagnerGames.Managers
{
    /// <summary>
    /// Action maps available in the game
    /// </summary>
    public enum ActionMapName
    {
        EXPLORATION,
        FISHING,
        DIALOGUE,
        UI,
        PAUSE
    }

    /// <summary>
    /// Singleton manager for handling all game input
    /// </summary>
    public class InputManager : MonoBehaviourSingleton<InputManager>
    {
        [Header("Input Configuration")]
        [SerializeField] private bool disableAllOnStart = true;
        
        private GameControls gameControls;
        public readonly Dictionary<ActionMapName, InputActionMap> actionMaps = new Dictionary<ActionMapName, InputActionMap>();
        private ActionMapName currentActiveMap;

        public GameControls Controls => gameControls;
        public ActionMapName CurrentActiveMap => currentActiveMap;

        protected override void Awake()
        {
            base.Awake();
            InitializeInputSystem();
        }

        private void InitializeInputSystem()
        {
            gameControls = new GameControls();
            
            // Map enum values to action maps
            foreach (ActionMapName mapName in Enum.GetValues(typeof(ActionMapName)))
            {
                string mapNameString = mapName.ToString();
                if (gameControls.asset.FindActionMap(mapNameString) != null)
                {
                    actionMaps[mapName] = gameControls.asset.FindActionMap(mapNameString);
                }
                else
                {
                    Debug.LogWarning($"Action map {mapName} not found in controls asset", this);
                }
            }

            if (disableAllOnStart)
            {
                DisableAllActionMaps();
            }
            else
            {
                gameControls.Enable();
            }
        }

        /// <summary>
        /// Enable a specific action map and optionally disable others
        /// </summary>
        public void EnableActionMap(ActionMapName mapName, bool disableOthers = true)
        {
            if (disableOthers)
            {
                DisableAllActionMaps();
            }

            if (actionMaps.TryGetValue(mapName, out var actionMap))
            {
                actionMap.Enable();
                currentActiveMap = mapName;
                // If enabling EXPLORATION, lock and center the cursor
                if (mapName == ActionMapName.EXPLORATION)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    // Move cursor to center (works in editor, not always in build)
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                }
            }
            else
            {
                Debug.LogWarning($"Could not enable action map {mapName}: not found", this);
            }
        }

        /// <summary>
        /// Disable a specific action map
        /// </summary>
        public void DisableActionMap(ActionMapName mapName)
        {
            if (actionMaps.TryGetValue(mapName, out var actionMap))
            {
                actionMap.Disable();
                if (currentActiveMap == mapName)
                {
                    currentActiveMap = default;
                }
                // If disabling EXPLORATION, unlock and show the cursor
                if (mapName == ActionMapName.EXPLORATION)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
            else
            {
                Debug.LogWarning($"Could not disable action map {mapName}: not found", this);
            }
        }

        /// <summary>
        /// Disable all action maps
        /// </summary>
        public void DisableAllActionMaps()
        {
            foreach (var mapPair in actionMaps)
            {
                mapPair.Value.Disable();
                // If disabling EXPLORATION, unlock and show the cursor
                if (mapPair.Key == ActionMapName.EXPLORATION)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
            currentActiveMap = default;
        }

        /// <summary>
        /// Get a specific action map
        /// </summary>
        public InputActionMap GetActionMap(ActionMapName mapName)
        {
            return actionMaps.TryGetValue(mapName, out var actionMap) ? actionMap : null;
        }

        /// <summary>
        /// Get a specific input action by name
        /// </summary>
        public InputAction GetAction(ActionMapName mapName, string actionName)
        {
            var actionMap = GetActionMap(mapName);
            return actionMap?.FindAction(actionName);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DisableAllActionMaps();
            gameControls?.Dispose();
        }
    }
}