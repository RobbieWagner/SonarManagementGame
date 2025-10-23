using UnityEngine;
using RobbieWagnerGames.Managers;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace RobbieWagnerGames.MultiFactorGame
{
    /// <summary>
    /// Simple invisible maze screen. Shows a player image that can be moved on a grid.
    /// When the player attempts a move the screen submits the attempted cell ("x,y") via SubmitInput.
    /// Clearing the screen resets the player to the start cell.
    /// </summary>
    public class InvisibleMaze : AuthenticationScreen
    {
        [Header("Grid")]
        [SerializeField] private int width = 5;
        [SerializeField] private int height = 5;
        [SerializeField] private float cellSize = 32f;
        [SerializeField] private Transform mazeImagePrefab;
        [SerializeField] private GridLayoutGroup grid;

        [Header("Player")]
        [SerializeField] private Transform playerImage; // UI element to move
        [SerializeField] private Vector2Int startCell = new Vector2Int(0, 0);

        private Vector2Int currentCell;

        private void Reset()
        {
            currentCell = startCell;
        }

        private InputAction navigateAction;

        [Header("Input")]
        [Tooltip("Minimum absolute axis value required to register a move (e.g. 0.9)")]
        [SerializeField] private float axisThreshold = 0.9f;
        [Tooltip("Delay before allowing repeat moves when holding the stick (seconds)")]
        [SerializeField] private float initialRepeatDelay = 0.35f;
        [Tooltip("Delay between repeats after the initial delay (seconds)")]
        [SerializeField] private float repeatInterval = 0.12f;

        // hold / repeat state
        private Vector2Int lastMoveDir = Vector2Int.zero;
        private float nextRepeatTime = 0f;

        private void OnEnable()
        {
            // ensure starting position
            currentCell = startCell;
            UpdatePlayerTransform();

            // Subscribe to the UI Navigate action (Vector2) from your InputManager
            if (InputManager.Instance != null)
            {
                navigateAction = InputManager.Instance.GetAction(ActionMapName.UI, "Navigate");
                if (navigateAction != null)
                    navigateAction.performed += OnNavigatePerformed;
            }
        }

        private void OnDisable()
        {
            if (navigateAction != null)
                navigateAction.performed -= OnNavigatePerformed;
        }

        private void OnNavigatePerformed(InputAction.CallbackContext ctx)
        {
            Vector2 raw = ctx.ReadValue<Vector2>();

            // require the axis be close to full deflection to count as an intentional move
            Vector2 dir = Vector2.zero;
            if (Mathf.Abs(raw.x) >= axisThreshold && Mathf.Abs(raw.x) > Mathf.Abs(raw.y))
                dir = raw.x > 0 ? Vector2.right : Vector2.left;
            else if (Mathf.Abs(raw.y) >= axisThreshold && Mathf.Abs(raw.y) > Mathf.Abs(raw.x))
                dir = raw.y > 0 ? Vector2.up : Vector2.down;
            else
                return; // not strong/clear enough

            Vector2Int moveDir = Vector2Int.zero;
            if (dir == Vector2.right) moveDir = Vector2Int.right;
            else if (dir == Vector2.left) moveDir = Vector2Int.left;
            else if (dir == Vector2.up) moveDir = Vector2Int.up;
            else if (dir == Vector2.down) moveDir = Vector2Int.down;

            // handle initial press vs hold-to-repeat
            float now = Time.time;
            if (moveDir != lastMoveDir)
            {
                // new direction: accept immediately and schedule repeats
                AttemptMove(moveDir);
                lastMoveDir = moveDir;
                nextRepeatTime = now + initialRepeatDelay;
            }
            else
            {
                // same direction: only allow if repeat timer expired
                if (now >= nextRepeatTime)
                {
                    AttemptMove(moveDir);
                    nextRepeatTime = now + repeatInterval;
                }
            }
        }

        private void AttemptMove(Vector2Int dir)
        {
            Vector2Int target = currentCell + dir;

            // submit the attempted target cell (step will validate)
            SubmitInput(CellToString(target));

            // tentatively move the player visually; if the step determines invalid it will call Clear()
            if (IsInsideGrid(target))
            {
                currentCell = target;
                UpdatePlayerTransform();
            }
            else
            {
                // out of bounds - still submit so the step can reset
                // visually, keep player in place until step maybe clears
            }
        }

        private string CellToString(Vector2Int cell)
        {
            return $"{cell.x},{cell.y}";
        }

        private bool IsInsideGrid(Vector2Int cell)
        {
            return cell.x >= 0 && cell.x < width && cell.y >= 0 && cell.y < height;
        }

        private void UpdatePlayerTransform()
        {
            if (playerImage == null) return;
            // place player centered on cell; origin at (0,0)
            Vector2 pos = new Vector2(currentCell.x * cellSize, currentCell.y * cellSize);
            playerImage.localPosition = pos;
        }

        public override void DisplayScreen()
        {
            // reset and show
            currentCell = startCell;
            UpdatePlayerTransform();
            gameObject.SetActive(true);
        }

        public override void Clear()
        {
            // reset player to start
            currentCell = startCell;
            UpdatePlayerTransform();
        }

        /// <summary>
        /// Build the maze UI: clear current children and instantiate width*height cells.
        /// Adjusts GridLayoutGroup cell size and spacing to evenly fill the available area.
        /// </summary>
        public void BuildMaze(int newWidth, int newHeight)
        {
            width = Mathf.Max(1, newWidth);
            height = Mathf.Max(1, newHeight);

            if (grid == null || mazeImagePrefab == null) return;

            // Clear existing children
            for (int i = grid.transform.childCount - 1; i >= 0; i--)
            {
                var go = grid.transform.GetChild(i).gameObject;
#if UNITY_EDITOR
                DestroyImmediate(go);
#else
                UnityEngine.Object.Destroy(go);
#endif
            }

            // Get available size from grid rect
            var gridRect = grid.GetComponent<RectTransform>();
            float availW = gridRect.rect.width;
            float availH = gridRect.rect.height;

            // Compute cell size to fit
            float cellW = availW / width;
            float cellH = availH / height;
            float cellSizeF = Mathf.Min(cellW, cellH);

            // Compute spacing such that cells and spacing fill the area and grid is centered
            float spacingX = 0f;
            float spacingY = 0f;
            if (width > 1)
                spacingX = Mathf.Max(0f, (availW - cellSizeF * width) / (width - 1));
            if (height > 1)
                spacingY = Mathf.Max(0f, (availH - cellSizeF * height) / (height - 1));

            grid.cellSize = new Vector2(cellSizeF, cellSizeF);
            grid.spacing = new Vector2(spacingX, spacingY);

            // Compute padding to center the grid
            float totalW = cellSizeF * width + spacingX * (width - 1);
            float totalH = cellSizeF * height + spacingY * (height - 1);
            int padLeft = Mathf.RoundToInt((availW - totalW) * 0.5f);
            int padTop = Mathf.RoundToInt((availH - totalH) * 0.5f);
            grid.padding.left = Mathf.Max(0, padLeft);
            grid.padding.right = Mathf.Max(0, padLeft);
            grid.padding.top = Mathf.Max(0, padTop);
            grid.padding.bottom = Mathf.Max(0, padTop);

            grid.childAlignment = TextAnchor.MiddleCenter;

            // Instantiate cells
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var cell = UnityEngine.Object.Instantiate(mazeImagePrefab, grid.transform);
                    if (cell is RectTransform rt)
                    {
                        rt.localScale = Vector3.one;
                    }
                    else
                    {
                        cell.localScale = Vector3.one;
                    }
                }
            }
        }
    }
}