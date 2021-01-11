using Data;
using NSubstitute;
using NUnit.Framework;
using Zenject;

namespace EditmodeTests
{
    [TestFixture]
    public class ScoreTest : ZenjectUnitTestFixture
    {
        [Inject]
        private Score _score;
        private HitCombo _fakeHitCombo;

        private void CommonInstall()
        {
            SignalBusInstaller.Install(Container);
            Container.Bind<Score>().AsSingle();
            _fakeHitCombo = Substitute.For<HitCombo>();
            Container.Bind<HitCombo>().FromInstance(_fakeHitCombo).AsSingle();
            Container.Inject(this);
        }
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            CommonInstall();
        }

        [Test]
        public void ResetScore_Should_Set_The_Score_To_0()
        {
            _score.ResetScore();
            
            Assert.AreEqual(0, _score.CurrentScore);
        }

        [Test]
        public void IncreaseScore_Should_Increase_Score_By_Increment_Amount()
        {
            _score.ResetScore();
            _score.IncreaseScore();
            
            Assert.AreEqual(_score.ScoreIncrement, _score.CurrentScore);
        }
        
        [Test]
        public void IncreaseScore_Twice_Should_Increase_Score_By_Increment_Amount_Times_Three()
        {
            _score.ResetScore();
            _score.IncreaseScore();
            _score.IncreaseScore();
            
            Assert.AreEqual(_score.ScoreIncrement + _score.ScoreIncrement * _score.HitCombo.CurrentHitCombo, _score.CurrentScore);
        }
    }
}