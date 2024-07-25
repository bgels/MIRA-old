using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Freeroam, Battle }
public class GameController : MonoBehaviour
{
    [SerializeField] playerMovement playerController;
    [SerializeField] BattleScript battleSystem;
    [SerializeField] Camera worldCamera;
    GameState state;

    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
    }

    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<Party>();
        var wildMob = FindObjectOfType<MapArea>().GetComponent<MapArea>().getRandomWildMob();

        battleSystem.StartBattle(playerParty, wildMob);
        
    }
    void EndBattle(bool won)
    {
        state = GameState.Freeroam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }
    private void Update()
    {
        if(state == GameState.Freeroam)
        {
            playerController.HandleUpdate();
        }
        else if(state== GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
    }
}

