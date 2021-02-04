using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip movementSound;
    [SerializeField] private AudioClip tetrominoPlacementSound;
    [SerializeField] private AudioClip scoreSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip clickSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        TetrominoBehaviour.onMoved += PlayMoveSound;
        TetrominoBehaviour.endedMovement += PlayPlacementSound;
        GameplayManager.onScored += PlayScoredSound;
        GameplayManager.onGameOver += PlayGameOverSound;
    }

    private void OnDisable()
    {
        TetrominoBehaviour.onMoved -= PlayMoveSound;
        TetrominoBehaviour.endedMovement -= PlayPlacementSound;
        GameplayManager.onScored -= PlayScoredSound;
        GameplayManager.onGameOver -= PlayGameOverSound;
    }

    private void PlayMoveSound()
    {
        audioSource.PlayOneShot(movementSound);
    }

    private void PlayPlacementSound()
    {
        audioSource.PlayOneShot(tetrominoPlacementSound);
    }

    private void PlayScoredSound(int score)
    {
        audioSource.PlayOneShot(scoreSound);
    }

    private void PlayGameOverSound()
    {
        audioSource.PlayOneShot(gameOverSound);
    }
}
