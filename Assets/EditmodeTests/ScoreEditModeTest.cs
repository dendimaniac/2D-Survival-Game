using Data;
using Interfaces;
using NSubstitute;
using NUnit.Framework;
using Zenject;

namespace EditmodeTests
{
    [TestFixture]
    public class ScoreEditModeTest : ZenjectUnitTestFixture
    {
        [Inject]
        private Score _score;

        private IPlayerPrefs _fakePlayerPrefs;

        private void CommonInstall()
        {
            SignalBusInstaller.Install(Container);
            Container.Bind<Score>().AsSingle();
            var fakeHitCombo = Substitute.For<HitCombo>();
            Container.Bind<HitCombo>().FromInstance(fakeHitCombo).AsSingle();
            _fakePlayerPrefs = Substitute.For<IPlayerPrefs>();
            Container.Bind<IPlayerPrefs>().FromInstance(_fakePlayerPrefs).AsSingle();
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
            
            Assert.AreEqual(_score.ScoreIncrement + _score.ScoreIncrement * 2, _score.CurrentScore);
        }
        
        [Test]
        public void Score_Changed_Event_Invoked_When_Increase_Score()
        {
            var hasChangedScore = false;
            Score.OnScoreChanged += _ =>
            {
                hasChangedScore = true;
            };
            
            _score.IncreaseScore();
            
            Assert.IsTrue(hasChangedScore);
        }
        
        [Test]
        public void Score_Changed_Event_With_Score_Amount_Invoked_When_Increase_Score()
        {
            var initialScore = _score.CurrentScore;
            var newScore = 0;
            Score.OnScoreChanged += newCurrentScore =>    
            {
                newScore = newCurrentScore;
            };
            
            _score.IncreaseScore();
            
            Assert.IsTrue(newScore > initialScore);
        }
        
        [Test]
        public void Score_Changed_Event_Invoked_When_Reset_Score()
        {
            var hasChangedScore = false;
            Score.OnScoreChanged += _ =>
            {
                hasChangedScore = true;
            };
            
            _score.ResetScore();
            
            Assert.IsTrue(hasChangedScore);
        }
        
        [Test]
        public void Score_Changed_Event_Invoked_With_Score_Amount_When_Reset_Score()
        {
            var initialScore = _score.CurrentScore;
            var newScore = 0;
            Score.OnScoreChanged += newCurrentScore =>    
            {
                newScore = newCurrentScore;
            };
            
            _score.ResetScore();
            
            Assert.AreEqual(initialScore, newScore);
        }
        
        [Test]
        public void Load_High_Score_Loads_From_Player_Prefs()
        {
            _fakePlayerPrefs.GetInt(Arg.Any<string>()).Returns(0);

            var savedHighScore = _score.LoadHighScore();
            
            Assert.AreEqual(0, savedHighScore);
            
            _fakePlayerPrefs.GetInt(Arg.Any<string>()).Returns(10);
            
            savedHighScore = _score.LoadHighScore();
            
            Assert.AreEqual(10, savedHighScore);
        }
    }
}