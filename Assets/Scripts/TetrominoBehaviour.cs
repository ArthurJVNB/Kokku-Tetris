using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoBehaviour : MonoBehaviour
{
    public static Action onMoved;
    public static Action endedMovement;
    public static List<Vector3Int> occupiedPositions;
    public static bool[,] occupied;

    // DEBUG
    public List<Vector3Int> DEBUG_occupiedPositions;
    public bool[,] DEBUG_stack;

    [SerializeField] private Vector3 pivot;

    //private Collider gameArea;
    private float timeNextFall;
    private List<Transform> blocks;
    private Vector2Int gameArea;

    private float timeToFall;

    #region STATIC METHODS
    public static void Destroy(Transform block)
    {
        RemoveFromOccupiedMatrix(block);
        //Vector3Int position = Vector3Int.RoundToInt(block.position);
        //Debug.LogWarning("Destroy [" + position.x + "," + position.y + "]");
        //Destroy(block.gameObject);
        block.gameObject.SetActive(false);
    }

    public static void AddToOccupiedMatrix(Transform block)
    {
        Vector3Int position = Vector3Int.RoundToInt(block.position);
        occupiedPositions.Add(position);
        occupied[position.x, position.y] = true;
        //Debug.Log("AddToOccupiedMatrix [" + position.x + "," + position.y + "]");
    }

    public static void AddToOccupiedMatrix(Transform[] blocks)
    {
        foreach (var block in blocks)
        {
            AddToOccupiedMatrix(block.transform);
        }
    }

    public static void RemoveFromOccupiedMatrix(Transform block)
    {
        Vector3Int position = Vector3Int.RoundToInt(block.position);
        occupiedPositions.Remove(position);
        //Debug.Log("RemoveFromOccupiedMatrix [" + position.x + "," + position.y + "]");
        occupied[position.x, position.y] = false;
    }

    public static void RemoveFromOccupiedMatrix(Transform[] blocks)
    {
        foreach (var block in blocks)
        {
            RemoveFromOccupiedMatrix(block);
        }
    }

    public static void ClearOccupiedMatrix()
    {
        occupiedPositions.Clear();
        occupied = null;
    }

    public static void MoveDown(Transform block)
    {
        //RemoveFromOccupiedMatrix(block);
        block.position += Vector3Int.down;
        //AddToOccupiedMatrix(block);

        onMoved?.Invoke();
    }
    #endregion

    private void Awake()
    {
        if (occupiedPositions == null) occupiedPositions = new List<Vector3Int>();
        DEBUG_occupiedPositions = occupiedPositions;
        DEBUG_stack = occupied;

        timeNextFall = Time.time + timeToFall;

        blocks = new List<Transform>();
        foreach (Transform child in transform)
        {
            blocks.Add(child);
        }

        enabled = false;
    }

    public void Initialize(Vector2Int gameArea, float speed)
    {
        this.gameArea = gameArea;
        //occupied = new bool[(int)gameArea.bounds.size.x, (int)gameArea.bounds.size.y];
        if (occupied == null) occupied = new bool[gameArea.x, gameArea.y];
        timeToFall = speed;

        enabled = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            // LEFT
            transform.position += Vector3.left;
            if (!ValidMove) transform.position -= Vector3.left;
            else onMoved?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            // RIGHT
            transform.position += Vector3.right;
            if (!ValidMove) transform.position -= Vector3.right;
            else onMoved?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            // ROTATE CLOCKWISE
            transform.RotateAround(transform.TransformPoint(pivot), Vector3.forward, -90);
            if (!ValidMove) transform.RotateAround(transform.TransformPoint(pivot), Vector3.forward, 90);
            //else onMoved?.Invoke();
        }

        MoveDown();
    }

    private void MoveDown()
    {
        if (Time.time > (Input.GetKey(KeyCode.S) ? (timeNextFall - timeToFall * .9f) : timeNextFall))
        {
            timeNextFall = Time.time + timeToFall;
            transform.position += Vector3.down;
            onMoved?.Invoke();

            if (!ValidMove)
            {
                // END OF MOVEMENT
                transform.position -= Vector3.down;
                EndMovement();
            }
        }
    }

    public void EndMovement()
    {
        foreach (var block in blocks)
        {
            AddToOccupiedMatrix(block);
            block.parent = null;
            Destroy(gameObject);
            //enabled = false;
        }

        endedMovement?.Invoke();
    }

    public bool ValidMove
    {
        get
        {
            foreach (var block in blocks)
            {
                Vector3Int position = Vector3Int.RoundToInt(block.position);
                //if (!gameArea.bounds.Contains(block.transform.position) || occupiedPositions.Contains(Vector3Int.RoundToInt(block.position)))
                if (OutOfBounds(position.x, position.y) || occupied[position.x, position.y])
                {
                    return false;
                }
            }

            return true;
        }
    }

    private bool OutOfBounds (int x, int y)
    {
        if (x < 0 || x >= gameArea.x || y < 0 || y >= gameArea.y)
            return true;
        return false;
    }

    private void OnDrawGizmos()
    {
        if (occupied != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < occupied.GetLength(0); i++)
            {
                for (int j = 0; j < occupied.GetLength(1); j++)
                {
                    if (occupied[i, j])
                        Gizmos.DrawWireCube(new Vector3(i, j), Vector3.one);
                }
            }
        }

        //if (occupiedPositions != null)
        //{
        //    foreach (var position in occupiedPositions)
        //    {
        //        if (position != null)
        //            Gizmos.DrawWireCube(position, Vector3.one);
        //    }
        //}
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.TransformPoint(pivot), .2f);
    }
}
