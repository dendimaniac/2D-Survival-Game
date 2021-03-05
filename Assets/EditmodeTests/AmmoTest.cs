using Attacks;
using NUnit.Framework;

namespace EditmodeTests
{
    public class AmmoTest
    {
        private Ammo _ammo;
        private const int DefaultMaxAmmo = 30;
        private const int DefaultMaxAmmoPerClip = 10;

        [SetUp]
        public void SetUp()
        {
            _ammo = new Ammo(DefaultMaxAmmo, DefaultMaxAmmoPerClip);
        }

        [Test]
        public void IsAmmoEmpty_InitializedWithZeroMaxAmmoAndMaxAmmoPerClip_ReturnsTrue()
        {
            _ammo = new Ammo(0, 0);

            Assert.IsTrue(_ammo.IsAmmoEmpty());
        }

        [Test]
        public void IsAmmoEmpty_InitializedWithZeroMaxAmmoButNonZeroMaxAmmoPerClip_ReturnsFalse()
        {
            _ammo = new Ammo(0, DefaultMaxAmmoPerClip);

            Assert.IsFalse(_ammo.IsAmmoEmpty());
        }

        [Test]
        public void IsAmmoEmpty_InitializedWithNonZeroMaxAmmoButZeroMaxAmmoPerClip_ReturnsFalse()
        {
            _ammo = new Ammo(DefaultMaxAmmo, 0);

            Assert.IsFalse(_ammo.IsAmmoEmpty());
        }

        [Test]
        public void ReduceCurrentAmmo_CurrentAmmoReducedByOne()
        {
            _ammo.ReduceCurrentAmmo();

            Assert.AreEqual(DefaultMaxAmmoPerClip - 1, _ammo.CurrentAmmo);
        }
        
        [Test]
        public void ReduceCurrentAmmo_CurrentMaxAmmoNotChanged()
        {
            _ammo.ReduceCurrentAmmo();

            Assert.AreEqual(DefaultMaxAmmo, _ammo.CurrentMaxAmmo);
        }

        [Test]
        public void ReduceCurrentAmmo_CalledWithMoreThanMaxAmmoPerClip_CurrentAmmoReturnsZero()
        {
            const int overAmount = 10;
            
            _ammo.ReduceCurrentAmmo(DefaultMaxAmmoPerClip + overAmount);

            Assert.AreEqual(0, _ammo.CurrentAmmo);
        }
        
        [Test]
        public void ReduceCurrentAmmo_CalledWithMoreThanMaxAmmoPerClip_CurrentMaxAmmoUnchanged()
        {
            const int overAmount = 10;
            
            _ammo.ReduceCurrentAmmo(DefaultMaxAmmoPerClip + overAmount);

            Assert.AreEqual(DefaultMaxAmmo, _ammo.CurrentMaxAmmo);
        }

        [Test]
        public void ReduceCurrentAmmo_CalledWithReduceAmountOfFive_CurrentAmmoReducedByFive()
        {
            const int reduceAmount = 5;

            _ammo.ReduceCurrentAmmo(reduceAmount);

            Assert.AreEqual(DefaultMaxAmmoPerClip - reduceAmount, _ammo.CurrentAmmo);
        }

        [Test]
        public void Reload_EmptyCurrentAmmo_ResetCurrentAmmoToMaxAmmoPerClip()
        {
            _ammo.ReduceCurrentAmmo(DefaultMaxAmmoPerClip);
            
            _ammo.Reload();

            Assert.AreEqual(DefaultMaxAmmoPerClip, _ammo.CurrentAmmo);
        }

        [Test]
        public void Reload_EmptyCurrentAmmo_ReduceCurrentMaxAmmoByMaxAmmoPerClip()
        {
            _ammo.ReduceCurrentAmmo(DefaultMaxAmmoPerClip);
            
            _ammo.Reload();

            Assert.AreEqual(DefaultMaxAmmo - DefaultMaxAmmoPerClip, _ammo.CurrentMaxAmmo);
        }

        [Test]
        public void Reload_HalfCurrentAmmo_ResetCurrentAmmoToMaxAmmoPerClip()
        {
            const int reduceAmount = DefaultMaxAmmoPerClip / 2;
            _ammo.ReduceCurrentAmmo(reduceAmount);
            
            _ammo.Reload();

            Assert.AreEqual(DefaultMaxAmmoPerClip, _ammo.CurrentAmmo);
        }

        [Test]
        public void Reload_HalfCurrentAmmo_ReduceCurrentMaxAmmoByReducedAmount()
        {
            const int reduceAmount = DefaultMaxAmmoPerClip / 2;
            _ammo.ReduceCurrentAmmo(reduceAmount);
            
            _ammo.Reload();

            Assert.AreEqual(DefaultMaxAmmo - reduceAmount, _ammo.CurrentMaxAmmo);
        }

        [Test]
        public void Reload_CurrentMaxAmmoLessThanMaxAmmoPerClip_AddCurrentMaxAmmoToCurrentAmmo()
        {
            const int maxAmmo = 5;
            const int maxAmmoPerClip = 10;
            _ammo = new Ammo(maxAmmo, maxAmmoPerClip);
            const int reduceAmount = 8;
            
            _ammo.ReduceCurrentAmmo(reduceAmount);
            
            _ammo.Reload();

            Assert.AreEqual(maxAmmoPerClip - reduceAmount + maxAmmo, _ammo.CurrentAmmo);
        }
        
        [Test]
        public void Reload_CurrentMaxAmmoLessThanMaxAmmoPerClip_CurrentMaxAmmoReturnsZero()
        {
            const int maxAmmo = 5;
            const int maxAmmoPerClip = 10;
            _ammo = new Ammo(maxAmmo, maxAmmoPerClip);
            const int reduceAmount = 8;
            
            _ammo.ReduceCurrentAmmo(reduceAmount);
            
            _ammo.Reload();

            Assert.AreEqual(0, _ammo.CurrentMaxAmmo);
        }
    }
}