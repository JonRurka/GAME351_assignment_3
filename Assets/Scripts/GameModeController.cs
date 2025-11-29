using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeController : MonoBehaviour
{
    public enum GameState
    {
        Intro, // approx 72 seconds
        MomentsBeforeFight, // 3 seconds
        TexasRedDual, // approx 2 seconds or less
        FightingBandits, // 10 seconds in any fight state
        Relaxing // In Relax until shots are fired, then go back to "fight" state.
    }

    public GameObject cinema_Cam;
    public PlayerController player;
    public float fight_duration = 10;
    public GameState current_state;


    public AudioSource fire_trigger_clip;

    private SongController song_controller;

    private float fight_counter = 0;
    private bool is_in_fight = false;

    private float general_timer;
    private float instructions_timer;

    // Start is called before the first frame update
    void Start()
    {
        current_state = GameState.Intro;
        song_controller = GetComponent<SongController>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (current_state)
        {
            case GameState.Intro:
                update_intro();
                break;

            case GameState.MomentsBeforeFight:
                update_before_fight();
                break;

            case GameState.TexasRedDual:
                update_tx_red_fight();
                break;

            case GameState.FightingBandits:
                update_bandits_fight();
                break;

            case GameState.Relaxing:
                update_relaxing();
                break;
        }
    }

    void update_intro()
    {
        // transition to before 
    }

    void update_before_fight()
    {
        general_timer -= Time.deltaTime;

        if (general_timer <= 0)
        {

            // Trigger "Fire" clip
            fire_trigger_clip.time = 0.6f;
            fire_trigger_clip.Play();
            player.SetMode(PlayerController.PlayerMode.Dual);
            current_state = GameState.TexasRedDual;
        }
    }

    void update_tx_red_fight()
    {
        // State changes to bandit fight if player kills Texas Red,or
        // lost display if player dies.
    }

    void update_bandits_fight()
    {
        fight_counter -= Time.deltaTime;

        if (fight_counter < 0)
        {
            is_in_fight = false;
            song_controller.SwitchSong(SongController.SongType.Relax);
            current_state = GameState.Relaxing;
        }
    }

    void update_relaxing()
    {
        // If player fires gun in relax state, trigger bandit fight state.
    }

    private void OnGUI()
    {
        if (current_state == GameState.MomentsBeforeFight && general_timer >= 1)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 35;
            style.normal.textColor = Color.red;
            style.normal.background = null;
            style.hover.background = null;
            style.focused.background = null;
            style.active.background = null;

            GUI.TextField(new Rect(Screen.width / 2 - 250, Screen.height * (1.0f / 3), 500, 100), "Shoot when you hear FIRE!!!", style);
        }
    }

    public void SwitchToPlayerCam()
    {
        cinema_Cam.SetActive(false);
        Debug.Log("Switching to player camera.");
    }

    public void StartPlaying()
    {
        current_state = GameState.MomentsBeforeFight;
        general_timer = 3;
        is_in_fight = true;
        fight_counter = fight_duration;
        instructions_timer = 2;
        song_controller.StartSong(SongController.SongType.Flight);
    }

    public void StartBanditFight()
    {
        if (is_in_fight)
            return;

        fight_counter = fight_duration;
        song_controller.StartSong(SongController.SongType.Flight);
        is_in_fight = true;
    }
    

    
}
