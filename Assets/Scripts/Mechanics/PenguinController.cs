using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Mechanics {

    public class PenguinController : KinematicObject
    {
        internal Animator animator;
        // Start is called before the first frame update

        private GameObject player;
        private bool airborne = false;

        private SpriteRenderer spriteRenderer;

        void Awake() {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void Update() {

            // Find the player
            Vector2 playerPos = new Vector2(0,0);

            if(player == null) {
                // Note that the player tag is also on enemies lol so I made an ActualPlayer tag
                // and added to the player
                player = GameObject.FindWithTag("ActualPlayer");
            }

            // todo you should store the player component not the player
            if(player != null) {
                if(!player.TryGetComponent<PlayerController>(out var pc)) {
                    Debug.LogError($"Player controller not found");
                    MonoBehaviour[] monoBehaviours = player.GetComponents<MonoBehaviour>();
                    foreach(MonoBehaviour mb in monoBehaviours) {
                        Debug.LogWarning($"behavior {mb.ToString()}");
                    }
                } else {
                    playerPos = pc.GetBodyPosition();
                    // Debug.LogWarning($"Player x difference {delta}");
                }
            } else {
                 Debug.LogError("Player not found!");
            }

            FixedUpdate();
            UpdateMovement(playerPos);
            base.Update();

            // if(IsGrounded) {
            //     Debug.Log($"IsGrounded {IsGrounded}");
            // }
        }
        private void UpdateMovement(Vector2 target) {
            // Look at the player
            float delta = body.position.x - target.x;
            if(delta > 0) {
                spriteRenderer.flipX = true;
            } else {
                spriteRenderer.flipX = false;
            }

            // TODO why double needed?
            // velocity += -2 * Time.deltaTime * Physics2D.gravity;
            // velocity += Time.deltaTime * Physics2D.gravity;
        }
        protected override void ComputeVelocity()
        {
            // Called by update on the base kinematic object



            base.ComputeVelocity();
        }
    }
}
