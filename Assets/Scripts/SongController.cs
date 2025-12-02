using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongController : MonoBehaviour
{
    public enum SongType
    {
        Fight,
        Relax
    }

    public float intro_song_offset = 89;
    public bool is_playing;
    public bool is_in_fade;
    public bool is_out_fade;
    public float fade_in_duration;
    public float fade_out_duration;

    public AudioClip intro_song;
    public AudioClip fight_song;
    public AudioClip relax_song;

    private AudioClip next_clip;

    private AudioSource song_source;

    private float fade_timer = 0;
    private float start_volume;
    private bool has_next_clip;

    // Start is called before the first frame update
    void Start()
    {
        song_source = GetComponent<AudioSource>();
        start_volume = song_source.volume;
    }

    // Update is called once per frame
    void Update()
    {
        is_playing = song_source.isPlaying;

        if (is_in_fade)
        {
            song_source.volume = Mathf.Clamp01(start_volume * fade_timer / fade_in_duration);
            fade_timer += Time.deltaTime;

            if (fade_timer >= fade_in_duration)
            {
                is_in_fade = false;
            }
        }

        if (is_out_fade)
        {
            song_source.volume = Mathf.Clamp01(start_volume * fade_timer / fade_in_duration);
            fade_timer -= Time.deltaTime;

            if (fade_timer < 0)
            {
                is_out_fade = false;

                if (has_next_clip)
                {
                    song_source.time = 0;
                    song_source.clip = next_clip;
                    song_source.Play();
                    start_in_fade();
                }
            }
        }
    }

    public void StartSong(SongType type)
    {
        AudioClip clip = get_song(type);
        song_source.clip = clip;
        song_source.time = 0;
        song_source.Play();
        start_in_fade();
    }

    public void SwitchSong(SongType type)
    {
        AudioClip clip = get_song(type);
        fade_to_song(clip);
    }

    private AudioClip get_song(SongType type)
    {
        AudioClip res = relax_song;
        switch (type)
        {
            case SongType.Fight:
                res = fight_song;
                break;

            case SongType.Relax:
                res = relax_song;
                break;
        }
        return res;
    }

    private void start_in_fade()
    {
        is_in_fade = true;
        fade_timer = 0;
        song_source.volume = 0;
    }

    private void start_out_fade()
    {
        is_out_fade = true;
        fade_timer = fade_out_duration;
        song_source.volume = start_volume;
    }

    private void fade_to_song(AudioClip next)
    {
        next_clip = next;
        has_next_clip = true;
        start_out_fade();
    }
}
