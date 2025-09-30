using System;
using System.Collections.Generic;
using Bean;
using Bean.Debug;
using Bean.PhysicsSystem;
using Microsoft.Xna.Framework;
using Random = Bean.Random;

namespace DemoGame
{
    public class MushroomSpawner : Prop
    {
        public Rectangle SpawnBounds;

        public Dictionary<Vector2, bool> TurretPoints = new Dictionary<Vector2, bool>();
        public bool TurretsFull;
        public Dictionary<Vector2, bool> PoisonPoints = new Dictionary<Vector2, bool>();
        public bool PosionFull;

        private List<Mushroom> _mushrooms = new List<Mushroom>();

        public override void Update()
        {
            base.Update();
        }

        public void SpawnMushrooms(int numberOfShrooms)
        {
            for (int i = 0; i < numberOfShrooms; i++)
            {
                while (true)
                {
                    Vector2 position = new Vector2(Random.RandomInt(this.SpawnBounds.Left, this.SpawnBounds.Right), Random.RandomInt(this.SpawnBounds.Top, this.SpawnBounds.Bottom));

                    Collider[] colliders = Raycasts.Instance.OverlapBoxAll(position, 32);

                    bool flowControl = false;

                    foreach (Collider collider in colliders)
                    {
                        DebugServer.Log(collider.Parent.Tag, this);

                        if (collider.Parent.Tag == "NoSpawn")
                        {
                            flowControl = true;
                            break;
                        }
                    }

                    if (flowControl)
                        continue;

                    int group = Random.RandomInt(0, 5);
                    int chance = Random.RandomInt(0, 100);

                    if (group == 1)
                        SpawnAttackMushroom(chance, position);
                    else
                        SpawnStationMushroom(chance, position);

                    break;
                }
            }
        }

        private void SpawnAttackMushroom(int chance, Vector2 position)
        {
            if (chance <= 100)
            {
                DumbMushroom mushroom = new DumbMushroom() { Position = position };

                mushroom.DelayStart(Random.RandomFloat(0, 1));

                this._mushrooms.Add(mushroom);

                mushroom.OnDeath += (object sender, EventArgs e) =>
                {
                    this._mushrooms.Remove(mushroom);
                };

                this.Scene.AddToScene(mushroom);
            }
        }

        private void SpawnStationMushroom(int chance, Vector2 position)
        {
            if (chance <= 20) //20%
            {
                DodgingMushroom mushroom = new DodgingMushroom() { Position = position };

                mushroom.DelayStart(Random.RandomFloat(0, 1));

                this._mushrooms.Add(mushroom);

                mushroom.OnDeath += (object sender, EventArgs e) =>
                {
                    this._mushrooms.Remove(mushroom);
                };

                this.Scene.AddToScene(mushroom);
            }
            else if (chance <= 30) // 30%
            {
                ToxicMushroom mushroom = new ToxicMushroom() { Position = position };

                mushroom.DelayStart(Random.RandomFloat(0, 1));

                this._mushrooms.Add(mushroom);

                mushroom.OnDeath += (object sender, EventArgs e) =>
                {
                    this._mushrooms.Remove(mushroom);
                };

                this.Scene.AddToScene(mushroom);
            }
            else if (chance <= 100) // 50%
            {
                RangedMushroom mushroom = new RangedMushroom() { Position = position };

                mushroom.DelayStart(Random.RandomFloat(0, 1));

                this._mushrooms.Add(mushroom);

                mushroom.OnDeath += (object sender, EventArgs e) =>
                {
                    this._mushrooms.Remove(mushroom);
                };

                this.Scene.AddToScene(mushroom);
            }
        }

    }
}