using FeatherForge;
using NUnit.Framework;
using UnityEngine;

namespace MetaCombatSystem.StatusManagement.Tests
{
    public class StatusManagerTests
    {
        private StatusManager unityEventStatusManager;
        private StatusAlteration statusAlteration;

        [SetUp]
        public void Setup()
        {
            unityEventStatusManager = new GameObject().AddComponent<StatusManager>();
            statusAlteration = ScriptableObject.CreateInstance<StatusAlteration>();
            statusAlteration.EffectPrefab = new GameObject().AddComponent<StatusAlterationEffect>();
        }

        [Test]
        public void AddStacks()
        {
            unityEventStatusManager.ChangeStacks(statusAlteration, 5, null);

            Assert.AreEqual(5, unityEventStatusManager.StatusAlterations[0].Stacks);
        }

        [Test]
        public void AddStacksWithMax()
        {
            statusAlteration.EffectPrefab.HasMax = true;
            statusAlteration.EffectPrefab.MaxStacks = 3;

            unityEventStatusManager.ChangeStacks(statusAlteration, 5, null);

            Assert.AreEqual(3, unityEventStatusManager.StatusAlterations[0].Stacks);
        }

        [Test]
        public void AddStacksWithModifier()
        {
            unityEventStatusManager.GetDataWatcher(statusAlteration).AddModifier(BufferMinusOne);

            unityEventStatusManager.ChangeStacks(statusAlteration, 5, null);

            Assert.AreEqual(4, unityEventStatusManager.StatusAlterations[0].Stacks);
        }

        private void BufferMinusOne(DataWatcher<StatusAlterationEffect.StatusStackChange>.DataWatcherBuffer buffer)
        {
            buffer.DataBuffer.Delta -= 1;
        }

        [Test]
        public void RemoveStacks()
        {
            unityEventStatusManager.ChangeStacks(statusAlteration, 5, null);
            Assert.AreEqual(5, unityEventStatusManager.StatusAlterations[0].Stacks);
            
            unityEventStatusManager.ChangeStacks(statusAlteration, -2, null);
            Assert.AreEqual(3, unityEventStatusManager.StatusAlterations[0].Stacks);
        }

        [Test]
        public void RemoveStacksNegative()
        {
            unityEventStatusManager.ChangeStacks(statusAlteration, 5, null);
            Assert.AreEqual(5, unityEventStatusManager.StatusAlterations[0].Stacks);

            unityEventStatusManager.ChangeStacks(statusAlteration, -78, null);
            Assert.AreEqual(0, unityEventStatusManager.StatusAlterations[0].Stacks);
        }

        [Test]
        public void GetDataWatcher()
        {
            var returnedDataWatcher = unityEventStatusManager.GetDataWatcher(statusAlteration);

            Assert.IsNotNull(returnedDataWatcher);
            Assert.AreSame(unityEventStatusManager.StatusAlterations[0].DataWatcher, returnedDataWatcher);
        }
    }
}
