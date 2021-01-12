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
        public void Ammo_Is_Empty_When_Both_Current_Ammo_And_Current_Max_Ammo_Is_0()
        {
            _ammo = new Ammo(0, 0);

            Assert.IsTrue(_ammo.IsAmmoEmpty());
        }

        [Test]
        public void Ammo_Is_Not_Empty_When_Only_Current_Ammo_Is_0()
        {
            _ammo = new Ammo(0, DefaultMaxAmmoPerClip);

            Assert.IsFalse(_ammo.IsAmmoEmpty());
        }

        [Test]
        public void Ammo_Is_Not_Empty_When_Only_Current_Max_Ammo_Is_0()
        {
            _ammo = new Ammo(DefaultMaxAmmo, 0);

            Assert.IsFalse(_ammo.IsAmmoEmpty());
        }

        [Test]
        public void Current_Ammo_Only_Reduced_By_1_When_Calling_Reduce_Ammo()
        {
            _ammo.ReduceCurrentAmmo();

            Assert.AreEqual(DefaultMaxAmmoPerClip - 1, _ammo.CurrentAmmo);
        }

        [Test]
        public void Current_Ammo_Dont_Go_Below_0_When_Calling_Reduce_Ammo()
        {
            ReduceCurrentAmmo(DefaultMaxAmmoPerClip + 10);

            Assert.AreEqual(0, _ammo.CurrentAmmo);
        }

        [Test]
        public void Current_Ammo_Only_Reduced_By_The_Specified_Amount_When_Calling_Reduce_Ammo_With_Parameter()
        {
            const int reduceAmount = 5;

            ReduceCurrentAmmo(reduceAmount);

            Assert.AreEqual(DefaultMaxAmmoPerClip - reduceAmount, _ammo.CurrentAmmo);
        }

        [Test]
        public void Current_Ammo_Dont_Go_Below_0_When_Calling_Reduce_Ammo_With_Parameter()
        {
            const int reduceAmount = DefaultMaxAmmoPerClip + 10;

            ReduceCurrentAmmo(reduceAmount);

            Assert.AreEqual(0, _ammo.CurrentAmmo);
        }

        [Test]
        public void Emptying_Current_Ammo_Then_Reload_Reset_Current_Ammo_Back_To_Full()
        {
            ReduceCurrentAmmo(DefaultMaxAmmoPerClip);
            _ammo.Reload();

            Assert.AreEqual(DefaultMaxAmmoPerClip, _ammo.CurrentAmmo);
        }

        [Test]
        public void Emptying_Current_Ammo_Then_Reload_Subtract_The_Reload_Amount_From_Current_Max_Ammo()
        {
            ReduceCurrentAmmo(DefaultMaxAmmoPerClip);
            _ammo.Reload();

            Assert.AreEqual(DefaultMaxAmmo - DefaultMaxAmmoPerClip, _ammo.CurrentMaxAmmo);
        }

        [Test]
        public void Reduce_Current_Ammo_By_5_Then_Reload_Reset_Current_Ammo_Back_To_Full()
        {
            ReduceCurrentAmmo(5);
            _ammo.Reload();

            Assert.AreEqual(DefaultMaxAmmoPerClip, _ammo.CurrentAmmo);
        }

        [Test]
        public void Reduce_Current_Ammo_By_5_Then_Reload_Subtract_The_Reload_Amount_From_Current_Max_Ammo()
        {
            const int reduceAmount = 5;

            ReduceCurrentAmmo(reduceAmount);
            _ammo.Reload();

            Assert.AreEqual(DefaultMaxAmmo - reduceAmount, _ammo.CurrentMaxAmmo);
        }

        [Test]
        public void If_Max_Ammo_Is_Not_Enough_To_Reload_To_Full_Then_Reload_To_Max_Possible()
        {
            const int maxAmmo = 5;
            const int maxAmmoPerClip = 10;
            _ammo = new Ammo(maxAmmo, maxAmmoPerClip);
            const int reduceAmount = 8;

            ReduceCurrentAmmo(reduceAmount);
            _ammo.Reload();

            Assert.AreEqual(maxAmmoPerClip - reduceAmount + maxAmmo, _ammo.CurrentAmmo);
        }

        private void ReduceCurrentAmmo(int reduceAmount)
        {
            for (var i = 0; i < reduceAmount; i++)
            {
                _ammo.ReduceCurrentAmmo();
            }
        }
    }
}