using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;


namespace Platformer.Mechanics
{
    [RequireComponent(typeof(Collider2D))]
    public class SwapZone : MonoBehaviour
    {

        public AudioClip tokenCollectAudio;
        [Tooltip("If true, animation will start at a random position in the sequence.")]
        public bool randomAnimationStartTime = false;
		
		public TokenType ttype;
		internal int type;
        internal int exitType;

		
        [Tooltip("List of frames that make up the animation.")]
        public Sprite[] idleAnimation, collectedAnimation;

        internal Sprite[] sprites = new Sprite[0];

        internal SpriteRenderer _renderer;

        //unique index which is assigned by the TokenController in a scene.
        internal int tokenIndex = -1;
        internal TokenController controller;
        //active frame in animation, updated by the controller.
        internal int frame = 0;
        internal bool collected = false;

        void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            if (randomAnimationStartTime)
                frame = Random.Range(0, sprites.Length);
            sprites = idleAnimation;
			switch (ttype)
            {
                case TokenType.UpToken:
                    type = 1;
                    exitType = 2;
                    break;
                case TokenType.DownToken:
					type = 2;
                    exitType = 2;
                    break;
                case TokenType.LeftToken:
                    type = 3;
                    exitType = 2;
                    break;
                case TokenType.RightToken:
                    type = 4;
                    exitType = 2;
                    break;
			}
        }

        void OnTriggerEnter2D(Collider2D other){
            var player = other.gameObject.GetComponent<PlayerController>();
            if (player != null) OnPlayerEnter(player);
        }

        void OnTriggerExit2D(Collider2D other){
            var player = other.gameObject.GetComponent<PlayerController>();
            if (player != null) OnPlayerExit(player);
        }

        void OnPlayerEnter(PlayerController player)
        {
            //send an event into the gameplay system to perform some behaviour.
            var ev = Schedule<PlayerEnteredSwapZone>();
            ev.swapzone = this;
            ev.player = player;
        }

        void OnPlayerExit(PlayerController player)
        {
            //send an event into the gameplay system to perform some behaviour.
            var ev = Schedule<PlayerExitedSwapZone>();
            ev.swapzone = this;
            ev.player = player;
        }

        public enum TokenType
        {
            UpToken,
            DownToken,
            LeftToken,
            RightToken
        }

    }
}