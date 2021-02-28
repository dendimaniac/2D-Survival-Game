using Data;
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
        public void Current_Hit_Combo_Is_Zero_For_New_Instance()
        {
            Assert.AreEqual(0, _hitCombo.CurrentHitCombo);
        }
        
        [Test]
        public void Current_Hit_Combo_Is_Increased_By_One_When_Call_Increase_Streak()
        {
            int initialHitCombo = _hitCombo.CurrentHitCombo;
            
            _hitCombo.IncreaseStreak();
            
            Assert.IsTrue(_hitCombo.CurrentHitCombo > initialHitCombo);
        }
        
        [Test]
        public void Current_Hit_Combo_Is_Reset_To_Zero_When_Call_Reset_Streak()
        {
            _hitCombo.ResetStreak();
            
            Assert.AreEqual(0, _hitCombo.CurrentHitCombo);
        }
        
        [Test]
        public void Hit_Combo_Changed_Event_Is_Invoked_When_Call_Increase_Streak()
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
        public void Hit_Combo_Changed_Event_Is_Invoked_With_Hit_Combo_Amount_When_Call_Increase_Streak()
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
        public void Hit_Combo_Changed_Event_Is_Invoked_When_Call_Reset_Streak()
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
        public void Hit_Combo_Changed_Event_Is_Invoked_With_Hit_Combo_Amount_When_Call_Reset_Streak()
        {
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