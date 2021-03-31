using Data;
using NSubstitute;
using NUnit.Framework;
using Zenject;

namespace EditmodeTests
{
    public class HitComboTest
    {
        private HitCombo _hitCombo;
        
        [SetUp]
        public void SetUp()
        {
            _hitCombo = new HitCombo();
        }
        
        [Test]
        public void CurrentHitCombo_NewInstance_ReturnsZero()
        {
            Assert.AreEqual(0, _hitCombo.CurrentHitCombo);
        }
        
        [Test]
        public void IncreaseStreak_CurrentHitComboIncreasedByOne()
        {
            var initialHitCombo = _hitCombo.CurrentHitCombo;
            
            _hitCombo.IncreaseStreak();
            
            Assert.AreEqual(initialHitCombo + 1, _hitCombo.CurrentHitCombo);
        }
        
        [Test]
        public void ResetStreak_CurrentHitComboReturnsZero()
        {
            var initialHitCombo = _hitCombo.CurrentHitCombo;
            _hitCombo.IncreaseStreak();
            
            Assert.IsTrue(_hitCombo.CurrentHitCombo > initialHitCombo);

            _hitCombo.ResetStreak();
            
            Assert.AreEqual(0, _hitCombo.CurrentHitCombo);
        }
        
        [Test]
        public void OnHitComboChangedEvent_IncreaseStreakCalled_IsRaised()
        {
            var hasChangedCombo = false;
            HitCombo.OnHitComboChanged += _ =>
            {
                hasChangedCombo = true;
            };
            
            _hitCombo.IncreaseStreak();
            
            Assert.IsTrue(hasChangedCombo);
        }
        
        [Test]
        public void OnHitComboChangedEvent_IncreaseStreakCalled_IsRaisedWithCurrentHitCombo()
        {
            var newHitCombo = 0;
            HitCombo.OnHitComboChanged += currentHitCombo =>
            {
                newHitCombo = currentHitCombo;
            };
            
            _hitCombo.IncreaseStreak();

            Assert.AreEqual(_hitCombo.CurrentHitCombo,newHitCombo);
        }
        
        [Test]
        public void OnHitComboChangedEvent_ResetStreakCalled_IsRaised()
        {
            var hasChangedCombo = false;
            HitCombo.OnHitComboChanged += _ =>
            {
                hasChangedCombo = true;
            };
            
            _hitCombo.ResetStreak();
            
            Assert.IsTrue(hasChangedCombo);
        }
        
        [Test]
        public void OnHitComboChangedEvent_ResetStreakCalled_IsRaisedWithCurrentHitCombo()
        {
            var initialHitCombo = _hitCombo.CurrentHitCombo;
            _hitCombo.IncreaseStreak();
            
            Assert.IsTrue(_hitCombo.CurrentHitCombo > initialHitCombo);
            
            var newHitCombo = 0;
            HitCombo.OnHitComboChanged += currentHitCombo =>
            {
                newHitCombo = currentHitCombo;
            };
            
            _hitCombo.ResetStreak();

            Assert.AreEqual(_hitCombo.CurrentHitCombo,newHitCombo);
        }
    }
}