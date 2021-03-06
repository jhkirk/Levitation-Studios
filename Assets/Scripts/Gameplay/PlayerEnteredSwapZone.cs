using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when a player enters a gravity swap zone.
    /// </summary>
    /// <typeparam name="PlayerCollision"></typeparam>
    public class PlayerEnteredSwapZone : Simulation.Event<PlayerEnteredSwapZone>
    {
        public PlayerController player;
        public SwapZone swapzone;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            
		   player.changeGravyState(swapzone.type);
               	
        }
    }
}
