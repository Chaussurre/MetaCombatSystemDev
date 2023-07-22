using NUnit.Framework;
using UnityEngine;
using System.Reflection;

namespace MetaCombatSystem.Skills.Tests
{
    public class SkillEffectMonoTargetTests
    {
        Skill skill;
        MockSkillEffectMonoTarget skillEffect;
        MockSkillEffectMonoTargetNoValidTarget noValidTargetsSkillEffect;
        SkillTarget skillTarget1;
        SkillTarget skillTarget2;

        [SetUp]
        public void SetUp()
        {
            skill = new GameObject("skill").AddComponent<Skill>();
            skill.MinAndMaxNumberOfTargets = new(1, 2);

            var awakeMethod = typeof(Skill).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
            awakeMethod.Invoke(skill, null);

            skillEffect = new GameObject("skill effect").AddComponent<MockSkillEffectMonoTarget>();
            skillEffect.FirstAndLastTargets = new(0, 1);
            skill.Effects = new() { skillEffect };
            
            noValidTargetsSkillEffect = new GameObject("no valid Targers").AddComponent<MockSkillEffectMonoTargetNoValidTarget>();
            noValidTargetsSkillEffect.FirstAndLastTargets = new(0, 1);

            skillTarget1 = new GameObject("target 1").AddComponent<SkillTarget>();
            skillTarget2 = new GameObject("target 2").AddComponent<SkillTarget>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(skill.gameObject);
            Object.DestroyImmediate(skillEffect.gameObject);
            Object.DestroyImmediate(noValidTargetsSkillEffect.gameObject);
            Object.DestroyImmediate(skillTarget1.gameObject);
            Object.DestroyImmediate(skillTarget2.gameObject);
        }

        [Test]
        public void EffectTriggerOnTargets()
        {
            skillEffect.EffectTrigger(skillTarget1);
            Assert.AreEqual(1, skillEffect.TriggerCount, "EffectTrigger should be called once");
            Assert.AreEqual(skillTarget1, skillEffect.LastTriggeredTarget, "Last target should be the first target");

            skillEffect.EffectTrigger(skillTarget2);
            Assert.AreEqual(2, skillEffect.TriggerCount, "EffectTrigger should be called twice");
            Assert.AreEqual(skillTarget2, skillEffect.LastTriggeredTarget, "Last target should be the second target");
        }

        [Test]
        public void TriggerSkill()
        {
            skill.AddTarget(skillTarget1);
            skill.AddTarget(skillTarget2);

            Assert.IsTrue(skill.ReadyToTrigger(), "Skill should be ready to trigger on two targets");

            skill.Trigger();

            Assert.AreEqual(2, skillEffect.TriggerCount, "EffectTrigger should be called twice");
            Assert.AreSame(skillTarget2, skillEffect.LastTriggeredTarget, "Last target should be the second target");
        }

        [Test]
        public void IsTargetValid()
        {
            Assert.IsTrue(skillEffect.IsTargetValid(skillTarget1), "IsTargetValid should return true by default.");
        }

        [Test]
        public void InvalidTarget()
        {
            Assert.IsFalse(noValidTargetsSkillEffect.IsTargetValid(skillTarget2), "IsTargetValid should be overriden to return false");
        }


        [Test]
        public void SkillInvalidTargets()
        {
            skill.Effects.Add(noValidTargetsSkillEffect);

            skill.AddTarget(skillTarget1);
            skill.AddTarget(skillTarget2);

            Assert.IsFalse(skill.ReadyToTrigger(), "Skill should not be ready to trigger on two targets");
        }

        // Define a mock SkillEffectMonoTarget class for testing
        private class MockSkillEffectMonoTarget : SkillEffectMonoTarget
        {
            public int TriggerCount = 0;
            public SkillTarget LastTriggeredTarget;

            public override void SetUpEffect()
            {
                TriggerCount = 0;
                LastTriggeredTarget = null;
            }

            public override void EffectTrigger(SkillTarget target)
            {
                TriggerCount++;
                LastTriggeredTarget = target;
            }
        }

        private class MockSkillEffectMonoTargetNoValidTarget : SkillEffectMonoTarget
        {
            public bool called = false;

            public override void EffectTrigger(SkillTarget target)
            {
                called = true;
            }

            public override bool IsTargetValid(SkillTarget target)
            {
                return false;
            }
        }
    }
}
