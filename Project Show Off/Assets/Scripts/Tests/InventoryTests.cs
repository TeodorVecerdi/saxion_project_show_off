using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityCommons;
using Runtime.Data;
using static Runtime.Data.TrashMaterial.Types;

namespace Tests {
    public class InventoryTests {
        private Dictionary<TrashMaterial.Types, TrashMaterial> trash;

        [OneTimeSetUp]
        public void Setup() {
            trash = Resources.LoadAll<TrashMaterial>("Trash Materials").ToDictionary(material => material.Type, material => material);
        }

        [Test] public void TotalMassCorrect() {
            var inventory = new MaterialInventory();
            Assert.Zero(inventory.TotalMass);
            inventory.Add(trash[Metal], 10.0f);
            Assert.AreEqual(inventory.TotalMass, 10.0f);
            inventory.Add(trash[Glass], 3.141592f);
            Assert.AreEqual(inventory.TotalMass, 13.141592f);
            inventory.Remove(trash[Metal], 5.0f);
            Assert.AreEqual(inventory.TotalMass, 8.141592F);
        }

        [Test] public void AddMaterialCorrect() {
            var inventory = new MaterialInventory();

            var randMasses = new[] {Mathf.Floor(Rand.Float * 100), Mathf.Floor(Rand.Float * 100), Mathf.Floor(Rand.Float * 100)};

            inventory.Add(trash[Glass], randMasses[0]);
            inventory.Add(new ItemStack(trash[Glass], randMasses[1]));
            inventory.Add(new MaterialInventory {{trash[Glass], randMasses[2]}});

            Assert.AreEqual(inventory.TotalMass, randMasses.Sum());
            Assert.AreEqual(inventory.GetTrashMaterialMass(trash[Glass]), randMasses.Sum());
            Assert.Zero(inventory.GetTrashMaterialMass(trash[Metal]) + inventory.GetTrashMaterialMass(trash[Plastic]) + inventory.GetTrashMaterialMass(trash[Paper]));
        }

        [Test] public void RemoveMaterialCorrect() {
            var randMasses = new[] {Mathf.Floor(Rand.Float * 100), Mathf.Floor(Rand.Float * 100), Mathf.Floor(Rand.Float * 100)};
            var randTypes = new[] {Glass, Metal, Paper};

            var inventory = new MaterialInventory {
                {trash[randTypes[0]], randMasses[0]},
                {trash[randTypes[1]], randMasses[1]},
                {trash[randTypes[2]], randMasses[2]}
            };

            Assert.AreEqual(inventory.TotalMass, randMasses.Sum());

            inventory.Remove(trash[randTypes[0]], randMasses[0]);
            Assert.Zero(inventory.GetTrashMaterialMass(trash[randTypes[0]]));

            inventory.Remove(trash[randTypes[1]], randMasses[1]);
            Assert.Zero(inventory.GetTrashMaterialMass(trash[randTypes[1]]));

            inventory.Remove(trash[randTypes[2]], randMasses[2]);
            Assert.Zero(inventory.GetTrashMaterialMass(trash[randTypes[2]]));
        }

        [Test] public void RemoveMaterialThrowsOnMoreThanContained() {
            var inventory = new MaterialInventory {{trash[Glass], 100.0f}};

            Assert.DoesNotThrow(() => { inventory.Remove(trash[Glass], 50.0f); });

            Assert.Throws<Exception>(() => { inventory.Remove(trash[Glass], 5000.0f); });
        }

        [Test] public void GetMassCorrect() {
            var randMasses = new[] {
                Rand.Float * 1000,
                Rand.Float * 1000,
                Rand.Float * 1000,
                Rand.Float * 1000
            };

            var inventory = new MaterialInventory {
                {trash[Glass], 0},
                {trash[Metal], 0},
                {trash[Paper], 0},
                {trash[Plastic], 0},
            };
            
            inventory.Add(trash[Glass], randMasses[0]);
            inventory.Add(trash[Metal], randMasses[1]);
            inventory.Add(trash[Paper], randMasses[2]);
            inventory.Add(trash[Plastic], randMasses[3]);
            
            Assert.AreEqual(inventory.GetTrashMaterialMass(trash[Glass]), randMasses[0]);
            Assert.AreEqual(inventory.GetTrashMaterialMass(trash[Metal]), randMasses[1]);
            Assert.AreEqual(inventory.GetTrashMaterialMass(trash[Paper]), randMasses[2]);
            Assert.AreEqual(inventory.GetTrashMaterialMass(trash[Plastic]), randMasses[3]);
        }

        [Test] public void ForEachCorrect() {
            var randMasses = new[] {
                Rand.Float * 1000,
                Rand.Float * 1000,
                Rand.Float * 1000,
                Rand.Float * 1000
            };

            var types = new[] {
                Glass, Metal, Paper, Plastic
            };
            
            var shuffled = randMasses.Zip(types, (f, t) => (f, t)).ToList();
            shuffled.Shuffle();

            var inventory = new MaterialInventory {
                {trash[shuffled[0].t], 0},
                {trash[shuffled[1].t], 0},
                {trash[shuffled[2].t], 0},
                {trash[shuffled[3].t], 0},
            };

            inventory.Add(trash[shuffled[0].t], shuffled[0].f);
            inventory.Add(trash[shuffled[1].t], shuffled[1].f);
            inventory.Add(trash[shuffled[2].t], shuffled[2].f);
            inventory.Add(trash[shuffled[3].t], shuffled[3].f);

            var idx = 0;
            foreach (var itemStack in inventory) {
                Assert.AreEqual(itemStack.Mass, shuffled[idx].f);
                Assert.AreEqual(itemStack.TrashMaterial.Type, shuffled[idx].t);
                idx++;
            }
        }

        [Test] public void ClearCorrect() {
            var inventory = new MaterialInventory {
                {trash[Glass], Rand.Float * 1000},
                {trash[Metal], Rand.Float * 1000},
                {trash[Paper], Rand.Float * 1000},
                {trash[Plastic], Rand.Float * 1000},
            };

            Assert.NotZero(inventory.TotalMass);

            inventory.Clear();

            Assert.Zero(inventory.GetTrashMaterialMass(trash[Glass]));
            Assert.Zero(inventory.GetTrashMaterialMass(trash[Metal]));
            Assert.Zero(inventory.GetTrashMaterialMass(trash[Paper]));
            Assert.Zero(inventory.GetTrashMaterialMass(trash[Plastic]));
            Assert.Zero(inventory.TotalMass);
        }
    }
}