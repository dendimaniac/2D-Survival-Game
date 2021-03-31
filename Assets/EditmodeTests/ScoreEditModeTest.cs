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
        public void ResetScore_CurrentScoreReturnsZero()
        {
            _score.ResetScore();
            
            Assert.AreEqual(0, _score.CurrentScore);
        }

        [Test]
        public void IncreaseScore_CalledOneTime_IncreaseCurrentScoreByBaseIncrementAmount()
        {
            _score.IncreaseScore();
            
            Assert.AreEqual(_score.ScoreIncrement, _score.CurrentScore);
        }
        
        [Test]
        public void IncreaseScore_CalledTwoTimes_IncreaseCurrentScoreByThreeTimesIncrementAmount()
        {
            _score.IncreaseScore();
            _score.IncreaseScore();
            
            Assert.AreEqual(_score.ScoreIncrement + _score.ScoreIncrement * 2, _score.CurrentScore);
        }
        
        [Test]
        public void OnScoreChangedEvent_IncreaseScoreCalled_IsRaised()
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
        public void OnScoreChangedEvent_IncreaseScoreCalled_IsRaisedWithNewScore()
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
        public void OnScoreChangedEvent_ResetScoreCalled_IsRaised()
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
        public void OnScoreChangedEvent_ResetScoreCalled_IsRaisedWithNewScore()
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
        public void LoadHighScore_ReturnsScoreFromPlayerPrefs()
        {
            _fakePlayerPrefs.GetInt(Arg.Any<string>()).Returns(10);
            
            var savedHighScore = _score.LoadHighScore();
            
            Assert.AreEqual(10, savedHighScore);
        }
    }
}