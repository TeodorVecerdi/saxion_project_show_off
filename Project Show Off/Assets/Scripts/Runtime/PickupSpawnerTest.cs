using System;
using System.Collections.Generic;
using UnityCommons;
using UnityEngine;

namespace Runtime {
    public class PickupSpawnerTest : MonoBehaviour{
        public Vector3 From;
        public Vector3 To;
        public float SpawnTime;
        public List<Pickup> Pickups;
        
        private float timer = 0f;

        private void Update() {
            timer += Time.deltaTime;
            if (timer >= SpawnTime) {
                timer -= SpawnTime;
                SpawnPickup();
            }
        }

        private void SpawnPickup() {
            var pickup = Rand.ListItem(Pickups);
            var spawnX = Rand.Range(From.x, To.x);
            var spawnZ = Rand.Range(From.z, To.z);
            Instantiate(pickup, new Vector3(spawnX, 10, spawnZ), Quaternion.Euler(Rand.Float * 360, Rand.Float * 360, Rand.Float * 360));
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(From, 0.5f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(To, 0.5f);
        }
    }
}