using UnityEngine;
using System.Collections;

[System.Serializable]
public class WeaponTwoSounds
{
    public AudioClip audio_hit_1, audio_hit_2, audio_dead_1, audio_dead_2, audio_melee_attack_1, audio_melee_attack_2;
}
public class WeaponTwo : MonoBehaviour 
{
    // melee weapons are actually firebutton 2 weapons so this class is now called WeaponTwo
    public enum MeleeType { GiantStomp, KrakenGrapple, Thunderclap, TailSlap, Roar, Blink, Whirlwind, BirdSpeedBurst, JuggerPunch,  WraithPunch, PoisonArrow, Stinger, AdeptShield, DragonBiteOne, DragoneBiteTwo, DragonBiteThree, DragonBiteFour   };
    public MeleeType meleeType;
    public WeaponTwoSounds audioClips;


    private float meleeAttackNext = 0.0f;
    private float meleeAttackRate = 1.0f;
    private AudioSource audioSource;
    private Transform enemy;

    public void MeleeAttack()
    {
        if (Time.time > meleeAttackNext)
        {
            meleeAttackNext = Time.time + meleeAttackRate;
            float rand = Random.value;
            if (rand <= 0.4f)
            {
                audioSource.clip = audioClips.audio_melee_attack_1;
            }
            else
            {
                audioSource.clip = audioClips.audio_melee_attack_2;
            }
            audioSource.PlayOneShot(audioSource.clip);
            enemy.GetComponent<Enemy>()._isHit = true;
        }
    }
}
