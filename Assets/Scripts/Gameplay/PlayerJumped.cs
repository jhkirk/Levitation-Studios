using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Core;
using Platformer.Mechanics;

namespace Platformer.Gameplay
{
	
    /// <summary>
    /// Fired when the player performs a Jump.
    /// </summary>
    /// <typeparam name="PlayerJumped"></typeparam>
    public class PlayerJumped : Simulation.Event<PlayerJumped>
    {
		private Rigidbody2D rb;
        public PlayerController player;

        public override void Execute()
        {
            if (player.audioSource && player.jumpAudio){
                player.audioSource.PlayOneShot(player.jumpAudio);
			}
			
			
			//rb = player.GetComponent<Rigidbody2D>();
			//player.gravityModifier *= -1;
        }
    }
}