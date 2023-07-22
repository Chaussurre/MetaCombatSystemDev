using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;

namespace MetaCombatSystem.Skills.Tests
{
    public class SkillTargetTests
    {
        [Test]
        public void GetTargets()
        {
            var gameObject = new GameObject("Target");
            var skillTarget = gameObject.AddComponent<SkillTarget>();
            var targetType1 = gameObject.AddComponent<SomeBehavior>();
            var targetType2 = gameObject.AddComponent<AnotherBehavior>();
            skillTarget.TargetTypes = new List<Behaviour> { targetType1, targetType2 };

            var result1 = skillTarget.GetTarget<SomeBehavior>();
            var result2 = skillTarget.GetTarget<AnotherBehavior>();
            var result3 = skillTarget.GetTarget<NonExistentBehavior>();

            Assert.AreEqual(targetType1, result1, "GetTarget should return the correct type 1.");
            Assert.AreEqual(targetType2, result2, "GetTarget should return the correct type 2.");
            Assert.IsNull(result3, "GetTarget should return null for a non-existent type.");
        }

        // Define some behavior classes for testing purposes
        private class SomeBehavior : MonoBehaviour { }
        private class AnotherBehavior : MonoBehaviour { }
        private class NonExistentBehavior : MonoBehaviour { }
    }
}
