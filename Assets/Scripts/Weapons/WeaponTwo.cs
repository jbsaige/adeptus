using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class WeaponTwo : MonoBehaviour 
{
    // melee weapons are actually firebutton 2 weapons so this class is now called WeaponTwo
    public enum MeleeType { GiantStomp, KrakenGrapple, Thunderclap, TailSlap, Roar, Blink, Whirlwind, BirdSpeedBurst, JuggerPunch,  WraithPunch, PoisonArrow, Stinger, AdeptShield, DragonBiteOne, DragoneBiteTwo, DragonBiteThree, DragonBiteFour   };
    public MeleeType meleeType;


}
