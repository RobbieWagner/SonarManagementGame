using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.MultiFactorGame
{
    /// <summary>
    /// Controller for the InvisibleMaze screen. Validates attempted moves submitted by the screen.
    /// If a move hits a blocked cell or is out of bounds, the screen is cleared (reset).
    /// If a move reaches the configured goal cell, the authentication step completes.
    /// </summary>
    public class InvisibleMazeAuthenticationStep : AuthenticationStep
    {
        [Header("Optional Maze Parameters")]
        [SerializeField] private int mazeWidth = 5;
        [SerializeField] private int mazeHeight = 5;

        [Header("Maze Logic")]
        [Tooltip("Goal cell that completes the step when reached")]
        [SerializeField] private Vector2Int goalCell = new Vector2Int(4, 4);

        [Tooltip("Cells that are blocked (invisible walls)")]
        [SerializeField] private Vector2Int[] blockedCells = new Vector2Int[0];

        private HashSet<Vector2Int> blockedSet;

        private void Awake()
        {
            blockedSet = new HashSet<Vector2Int>(blockedCells);
        }

        public override void StartStep()
        {
            base.StartStep();
            // instruct the maze screen to build to the configured size when this step starts
            if (authenticationScreen is InvisibleMaze maze)
            {
                maze.BuildMaze(mazeWidth, mazeHeight);
            }
        }

        public override void ContinueStep()
        {
            base.ContinueStep();
            if (authenticationScreen is InvisibleMaze maze)
            {
                maze.BuildMaze(mazeWidth, mazeHeight);
            }
        }

        protected override void ValidateInput(string input)
        {
            // input expected in format "x,y"
            if (string.IsNullOrWhiteSpace(input))
            {
                ResetScreen();
                return;
            }

            var parts = input.Split(',');
            if (parts.Length != 2)
            {
                ResetScreen();
                return;
            }

            if (!int.TryParse(parts[0], out int x) || !int.TryParse(parts[1], out int y))
            {
                ResetScreen();
                return;
            }

            var target = new Vector2Int(x, y);

            // if blocked -> reset
            if (blockedSet != null && blockedSet.Contains(target))
            {
                ResetScreen();
                return;
            }

            // if reached goal -> complete
            if (target == goalCell)
            {
                EndStep();
                return;
            }

            // otherwise it's a valid move but not final; do nothing (user can continue)
        }

        private void ResetScreen()
        {
            if (authenticationScreen != null)
            {
                authenticationScreen.Clear();
            }
            // keep the step active so the user can try again
        }
    }
}