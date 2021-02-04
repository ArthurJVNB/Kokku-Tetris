using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static int Score { get; private set; }
    public static Action<int> onScored;
    public static Action onGameOver;
    public static Action onGameStart;
    
    public TetrominoBehaviour currentTetromino;
    public TetrominoBehaviour nextTetromino;

    [SerializeField] private Vector3Int spawnPosition;
    [SerializeField] private Vector3 previewPosition;
    [SerializeField] private string previewTag = "Preview";
    [SerializeField] private string inGameTag = "In Game";
    [SerializeField] private TetrominoBehaviour[] tetrominoes;
    [SerializeField] private LayerMask blockLayerMask;
    [SerializeField] private Vector2Int gameArea = new Vector2Int(10, 20);

    private float speed = 1;

    private void Awake()
    {
        Score = 0;
    }

    private void OnEnable()
    {
        onGameStart?.Invoke();

        TetrominoBehaviour.endedMovement += UpdateGameState;
    }

    private void OnDisable()
    {
        TetrominoBehaviour.endedMovement -= UpdateGameState;
        TetrominoBehaviour.ClearOccupiedMatrix();
    }

    private void Start()
    {
        GenerateNextTetromino();
        NewTetromino();
    }

    private void UpdateGameState()
    {
        StartCoroutine(UpdateGameStateRoutine());
    }

    private IEnumerator UpdateGameStateRoutine()
    {
        yield return new WaitForFixedUpdate();
        while (ClearCompleteRow(out int y))
        {
            // SO IT CLEARED ROWS
            yield return new WaitForSeconds(speed);
            MoveDownBlocksAboveY(y);
            yield return new WaitForFixedUpdate();
        }

        NewTetromino();
    }

    private bool ClearCompleteRow(out int rowY)
    {
        bool hadCompleteRow = false;
        rowY = -1;

        if (GetCompleteRow(out Transform[] rowBlocks))
        {
            hadCompleteRow = true;
            Debug.Log(rowBlocks.Length);
            rowY = Mathf.RoundToInt(rowBlocks[0].position.y);
            foreach (var block in rowBlocks)
            {
                TetrominoBehaviour.Destroy(block);
                Score += 50;
                onScored?.Invoke(Score);
            }
            //numberOfClearedRows++;
        }

        IncreaseDifficulty();

        return hadCompleteRow;
    }

    private bool GetCompleteRow(out Transform[] rowBlocks)
    {
        bool[,] occupied = TetrominoBehaviour.occupied;
        for (int y = 0; y < occupied.GetLength(1); y++)
        {
            int count = 0;
            for (int x = 0; x < occupied.GetLength(0); x++)
            {
                if (occupied[x, y])
                {
                    count++;

                    if (count >= gameArea.x)
                    {
                        // FULL ROW
                        //Debug.LogWarning("FULL ROW AT: " + y);

                        // Getting row blocks
                        int halfWidth = Mathf.RoundToInt((0 + gameArea.x) / 2f);
                        Collider[] blockColliders = Physics.OverlapBox(new Vector3Int(halfWidth, y, 0), new Vector3(halfWidth, .1f, .1f), Quaternion.identity, blockLayerMask);
                        rowBlocks = new Transform[blockColliders.Length];
                        for (int i = 0; i < rowBlocks.Length; i++)
                        {
                            rowBlocks[i] = blockColliders[i].transform;
                        }

                        return true;
                    }
                }
            }
        }

        rowBlocks = new Transform[0];
        return false;
    }

    private void MoveDownBlocksAboveY(int y)
    {
        float halfWidth = gameArea.x / 2f;
        float halfHeigth = (gameArea.y - y) / 2f;
        float centerX = halfWidth;
        float centerY = y + halfHeigth;

        Collider[] colliders = Physics.OverlapBox(new Vector3(centerX, centerY), new Vector3(halfWidth, halfHeigth), Quaternion.identity, blockLayerMask);
        Transform[] blocks = Utils.GetTransformsFromColliders(colliders);
        
        TetrominoBehaviour.RemoveFromOccupiedMatrix(blocks);

        // Moving blocks down
        foreach (var block in blocks)
        {
            Vector3Int position = Vector3Int.RoundToInt(block.transform.position);
            if (position.y > y && block.CompareTag(inGameTag))
            {
                //Moving down
                TetrominoBehaviour.MoveDown(block.transform);
            }
        }

        TetrominoBehaviour.AddToOccupiedMatrix(Utils.GetTransformsByTag(blocks, inGameTag));
    }

    private void IncreaseDifficulty()
    {
        speed *= .99f;
    }

    private void NewTetromino()
    {
        SpawnTetromino();
        GenerateNextTetromino();

        if (!currentTetromino.ValidMove)
            GameOver();
    }

    private void SpawnTetromino()
    {
        currentTetromino = nextTetromino;
        currentTetromino.transform.position = spawnPosition;
        currentTetromino.transform.localScale = Vector3.one;
        Utils.ChangeChildTags(currentTetromino.transform, inGameTag);
        currentTetromino.Initialize(gameArea, speed);
    }

    private void GenerateNextTetromino()
    {
        nextTetromino = Instantiate(tetrominoes[UnityEngine.Random.Range(0, tetrominoes.Length)], spawnPosition, Quaternion.identity);
        nextTetromino.transform.position = previewPosition;
        nextTetromino.transform.localScale = Vector3.one * .5f;
        Utils.ChangeChildTags(nextTetromino.transform, previewTag);
    }

    private void GameOver()
    {
        Debug.LogWarning("GAME OVER");
        TetrominoBehaviour.ClearOccupiedMatrix();
        onGameOver?.Invoke();
        TetrominoBehaviour.endedMovement -= UpdateGameState;
    }

    private void OnDrawGizmos()
    {
        // Show spawn position
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(spawnPosition, new Vector3(4, 4));
        Gizmos.DrawWireSphere(spawnPosition, .5f);

        // Show preview position;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(previewPosition, new Vector3(2, 2));
        Gizmos.DrawWireSphere(previewPosition, .5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.grey;
        for (int i = 0; i < gameArea.x; i++)
        {
            for (int j = 0; j < gameArea.y; j++)
            {
                Gizmos.DrawWireCube(new Vector3(i, j), Vector3.one);
            }
        }
    }
}
