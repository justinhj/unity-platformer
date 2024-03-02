using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using System;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;
        private LineRenderer lineRenderer;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool triggerJump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;

        private Vector3? WallJumpAnchor; 
        public float MaxWallJumpDistance = 2f;
        public LayerMask collisionLayer; // Specify which layers to consider for collision
        private bool triggerWallJump;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            lineRenderer = GetComponent<LineRenderer>();
            WallJumpAnchor = null;
        }

        void UpdateWallJumpAnchor() {
            // Cast a ray forward from the player's position
            float direction = spriteRenderer.flipX ? -1.0f : 1.0f;
            Vector2 rayDirection = direction * MaxWallJumpDistance * Vector3.right;
            WallJumpAnchor = SpeculativeCollide(rayDirection);
        }

        public Vector3? GetWallJumpAnchor() {
            return WallJumpAnchor;
        }

        private bool CanWallJump() {
            return WallJumpAnchor.HasValue;
        }
        protected override void Update()
        {
            if (controlEnabled)
            {
                // move is a vector2 for player movement input
                // What are the units?
                move.x = Input.GetAxis("Horizontal");
                // TODO the double jump should allow jumping when not grounded and second jump is allowed
                if(Input.GetButtonDown("Jump")) {
                    if(jumpState == JumpState.Grounded)
                        jumpState = JumpState.PrepareToJump;
                    if(jumpState == JumpState.InFlight && CanWallJump()) {
                        jumpState = JumpState.PrepareToWallJump;
                    }
                }
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }
            }
            else
            {
                move.x = 0;
            }
            
            UpdateWallJumpAnchor();
            Debug.Log($"state pre update jump {jumpState}");
            UpdateJumpState();
            Debug.Log($"state post update jump {jumpState}");
            base.Update();
        }

        public Vector2 GetBodyPosition() {
            return body.position;
        }

        void UpdateJumpState()
        {
            triggerJump = false;
            triggerWallJump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    triggerJump = true;
                    stopJump = false;
                    break;
                case JumpState.PrepareToWallJump:
                    jumpState = JumpState.WallJumping;
                    triggerWallJump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.WallJumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.WallJumpInFlight;
                    }
                    break;
                case JumpState.InFlight:
                case JumpState.WallJumpInFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (triggerJump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                triggerJump = false;
            }
            else if (triggerWallJump)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier * 2;
                triggerWallJump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            // Orient the sprite in the facing direction
            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            PrepareToWallJump,
            Jumping,
            WallJumping,
            InFlight,
            WallJumpInFlight,
            Landed
        }
    }
}