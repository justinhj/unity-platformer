using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Mechanics {

    public class PenguinController : KinematicObject
    {
        internal Animator animator;
        // Start is called before the first frame update

        internal GameObject player;

        void Awake() {
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
                    float delta = body.position.x - playerPos.x;
                    Debug.LogWarning($"Player x difference {delta}");
                }
            } else {
                 Debug.LogError("Player not found!");
            }
            UpdateMovement(playerPos);
            base.Update();
        }
        private void UpdateMovement(Vector2 target) {
            
        }
    }
}
