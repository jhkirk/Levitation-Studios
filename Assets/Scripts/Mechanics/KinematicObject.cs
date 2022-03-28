using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Implements game physics for some in game entity.
    /// </summary>
    public class KinematicObject : MonoBehaviour
    {
        /// <summary>
        /// The minimum normal (dot product) considered suitable for the entity sit on.
        /// </summary>
        public float minGroundNormalY = .65f;
		internal int GravDir = 1;
        /// <summary>
        /// A custom gravity coefficient applied to this entity.
        /// </summary>
        public float gravityModifier = 1f;
		protected Vector2 defGravity;
		public float maxFallVelocity = 20f;
        /// <summary>
        /// The current velocity of the entity.
        /// </summary>
        public Vector2 velocity;
		
        /// <summary>
        /// Is the entity currently sitting on a surface?
        /// </summary>
        /// <value></value>
        public bool IsGrounded { get; private set; }
		
		public GravState gravState;
		//public GravState gravSstate;
		
		
		internal int controlswap = 1;
        protected Vector2 targetVelocity;
        protected Vector2 groundNormal;
        protected Rigidbody2D body;
        protected ContactFilter2D contactFilter;
        protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];

        protected const float minMoveDistance = 0.001f;
        protected const float shellRadius = 0.01f;
		

        /// <summary>
        /// Bounce the object's vertical velocity.
        /// </summary>
        /// <param name="value"></param>
        public void Bounce(float value)
        {
            velocity.y = value;
        }

        /// <summary>
        /// Bounce the objects velocity in a direction.
        /// </summary>
        /// <param name="dir"></param>
        public void Bounce(Vector2 dir)
        {
            velocity.y = dir.y;
            velocity.x = dir.x;
        }

        /// <summary>
        /// Teleport to some position.
        /// </summary>
        /// <param name="position"></param>
        public void Teleport(Vector3 position)
        {
            body.position = position;
            velocity *= 0;
            body.velocity *= 0;
        }

        protected virtual void OnEnable()
        {
			
            body = GetComponent<Rigidbody2D>();
            body.isKinematic = true;
        }

        protected virtual void OnDisable()
        {
            body.isKinematic = false;
        }

        protected virtual void Start()
        {
			
            contactFilter.useTriggers = false;
            contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
            contactFilter.useLayerMask = true;
        }

        protected virtual void Update()
        {
            targetVelocity = Vector2.zero;
            ComputeVelocity();
        }

        protected virtual void ComputeVelocity()
        {

        }

        protected virtual void FixedUpdate()
        {
            //if already falling, fall faster than the jump speed, otherwise use normal gravity.
			
			if(maxFallVelocity > -velocity.y)
			{
            if (-velocity.y > 0 )
                velocity += gravityModifier * defGravity * Time.deltaTime ;
            else
                velocity += defGravity * Time.deltaTime ;
			}
			
            velocity.x = targetVelocity.x;

            IsGrounded = false;

            var deltaPosition = velocity * Time.deltaTime;
			var convertedDeltaPosition = convertAbsVectorToRelativeVector(deltaPosition);

            var moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

            var move = moveAlongGround * convertedDeltaPosition.x;

            PerformMovement(move, false);

			move = Vector2.up * convertedDeltaPosition.y;
			
            PerformMovement(move, true);

        }

        void PerformMovement(Vector2 move, bool yMovement)
        {
            var distance = move.magnitude;

            if (distance > minMoveDistance)
            {
                //check if we hit anything in current direction of travel
                var count = body.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
                for (var i = 0; i < count; i++)
                {
                    var currentNormal = hitBuffer[i].normal;
					currentNormal = convertAbsVectorToRelativeVector(currentNormal);
                    //is this surface flat enough to land on?
                    if ( GravDir * currentNormal.y > minGroundNormalY)
                    {
                        IsGrounded = true;
                        // if moving up, change the groundNormal to new surface normal.
                        if (yMovement)
                        {
                            groundNormal = currentNormal;
                            currentNormal.x = 0;
                        }
                    }
                    if (IsGrounded)
                    {
                        //how much of our velocity aligns with surface normal?
                        var projection = Vector2.Dot(velocity, currentNormal*GravDir);
                        if (projection < 0)
                        {
                            //slower velocity if moving against the normal (up a hill).
                            velocity = velocity - projection * currentNormal*GravDir;
                        }
                    }
                    else
                    {
                        //We are airborne, but hit something, so cancel vertical up and horizontal velocity.
                        velocity.x *= 0;
						if ( -GravDir * currentNormal.y > minGroundNormalY)
							velocity.y = Mathf.Min(velocity.y, 0);
                    }
                    //remove shellDistance from actual move distance.
                    var modifiedDistance = hitBuffer[i].distance - shellRadius;
                    distance = modifiedDistance < distance ? modifiedDistance : distance;
                }
            }
            body.position = body.position + move.normalized * distance;
		
        }
		
		public Vector2 convertAbsVectorToRelativeVector(Vector2 relVec)
		{
			switch (gravState)
            {
                case GravState.Up:
					
                    return rotateVector(relVec, 2);
                    
                case GravState.Down:
					
					return relVec;
                    
                case GravState.Right:
					
                    return rotateVector(relVec, 3);
                    
                case GravState.Left:
					
                    return rotateVector(relVec, 1);
					
                default:
					return relVec;
			}
		}
		
		public Vector2 rotateVector(Vector2 vec, int times)
		{
			var x = vec.x;
			var y = vec.y;
			for (var i = 0; i < times; i++)
                {
					x = vec.x;
					y = vec.y;
					vec.x = y;
					vec.y = -x;
				}
			return vec;
		}
		
		public virtual void changeGravState(int a)
        {
             switch (a)
            {
                case 1:
					controlswap = -1;
                    gravState = GravState.Up;
					GravDir = 1;
                    break;
                case 2:
					controlswap = 1;
					gravState = GravState.Down;
					GravDir = 1;
                    break;
                case 3:
					controlswap = 1;
                    gravState = GravState.Left;
					GravDir = -1;
                    break;
                case 4:
					controlswap = 1;
                    gravState = GravState.Right;
					GravDir = -1;
                    break;
            } 
        }
	
	public enum GravState
		{
			Up,
			Down,
			Left,
			Right
		}
	
    }
}