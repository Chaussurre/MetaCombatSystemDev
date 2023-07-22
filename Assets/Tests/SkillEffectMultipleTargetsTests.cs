using NUnit.Framework;
using UnityEngine;
using System.Reflection;

namespace MetaCombatSystem.Skills.Tests
{
    public class SkillEffectMultipleTargetsTests
    {
        Skill skill;
        MockSkillEffectMultipleTargets skillEffect;
        SkillTarget skillTarget1;
        SkillTarget skillTarget2;

        [SetUp]
        public void SetUp()
        {
            skill = new GameObject("skill").AddComponent<Skill>();
            skill.MinAndMaxNumberOfTargets = new(2, 2);

            var awakeMethod = typeof(Skill).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
            awakeMethod.Invoke(skill, null);

            skillEffect = new GameObject("skill effect").AddComponent<MockSkillEffectMultipleTargets>();
            skillEffect.FirstAndLastTargets = new(0, 2);
            skill.Effects = new() { skillEffect };

            skillTarget1 = new GameObject("target 1").AddComponent<SkillTarget>();
            skillTarget2 = new GameObject("target 2").AddComponent<SkillTarget>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(skill.gameObject);
            Object.DestroyImmediate(skillEffect.gameObject);
            Object.DestroyImmediate(skillTarget1.gameObject);
            Object.DestroyImmediate(skillTarget2.gameObject);
        }

        [Test]
        public void TriggerSkill()
        {
            Assert.IsTrue(skill.AddTarget(skillTarget1), "skill should be able to add a first target");
            Assert.IsTrue(skill.AddTarget(skillTarget2), "skill should be able to add a second target");

            Assert.IsTrue(skill.ReadyToTrigger(), "Skill should be ready to trigger on two targets");

            skill.Trigger();

            Assert.IsFalse(skillEffect.TargetAreSame, "the two targets should be different");
            Assert.AreSame(skillTarget1, skillEffect.LastTarget, "Last target should be the first target");
        }

        [Test]
        public void NotEnoughTargets()
        {
            skill.AddTarget(skillTarget1);

            Assert.IsFalse(skill.ReadyToTrigger(), "Skill shouldn't be able to trigger on one target");

            skill.Trigger();

            Assert.IsNull(skillEffect.LastTarget, "Effect shouldn't have triggered");
        }

        // Define a mock SkillEffectMultipleTargets class for testing
        private class MockSkillEffectMultipleTargets : SkillEffectMultipleTargets
        {
            public bool TargetAreSame;
            public SkillTarget LastTarget;

            public override void SetUpEffect()
            {
                LastTarget = null;
            }

            public override int GetNumberOfTargets()
            {
                return 2;
            }

            protected override void EffectTrigger(SkillTarget target, int i)
            {
                if (i == 0)
                    LastTarget = target;
                else
                    TargetAreSame = LastTarget == target;
            }
        }
    }
}
