using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;


namespace Platformer.Mechanics
{
    [RequireComponent(typeof(Collider2D))]
    public class SpikeHazard : MonoBehaviour
    {

        void Awake()
        {

        }

        void OnTriggerEnter2D(Collider2D other){
            var player = other.gameObject.GetComponent<PlayerController>();
            if (player != null) OnPlayerEnter(player);
        }

        void OnPlayerEnter(PlayerController player)
        {
            //send an event into the gameplay system to perform some behaviour.
            var ev = Schedule<PlayerEnteredSpikeHazard>();
            ev.spikezone = this;
            ev.player = player;
            ev.respawnPoint = player.respawnPoint;
        }
    }
}
