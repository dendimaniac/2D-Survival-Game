using System.Collections;
using Data;
using Interfaces;
using NSubstitute;
using NUnit.Framework;
using Signals;
using UnityEngine.TestTools;
using Zenject;

namespace PlaymodeTests
{
    public class ScoreSystemTest : ZenjectIntegrationTestFixture
    {
        [Inject] private Score _score;
        [Inject] private SignalBus _signalBus;
        private HitCombo _fakeHitCombo;
        private IPlayerPrefs _fakePlayerPrefs;

        private void CommonInstall()
        {
            PreInstall();

            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<GameLostSignal>();

            Container.BindInterfacesAndSelfTo<Score>().AsSingle();
            
            _fakeHitCombo = Substitute.For<HitCombo>();
            Container.Bind<HitCombo>().FromInstance(_fakeHitCombo).AsSingle();
            _fakePlayerPrefs = Substitute.For<IPlayerPrefs>();
            Container.Bind<IPlayerPrefs>().FromInstance(_fakePlayerPrefs).AsSingle();

            PostInstall();
        }
        
        [UnityTest]
        public IEnumerator CurrentScoreAndHitCombo_OnStart_ReturnsZero()
        {
            CommonInstall();

            Assert.AreEqual(0, _score.CurrentScore);
            Assert.AreEqual(0, _fakeHitCombo.CurrentHitCombo);
            yield break;
        }
        
        [UnityTest]
        public IEnumerator HitComboResetStreak_OnStart_IsCalled()
        {
            CommonInstall();
            
            _fakeHitCombo.Received(1).ResetStreak();

            yield break;
        }
        
        [UnityTest]
        public IEnumerator PlayerPrefsSetInt_OnGameLostCurrentScoreEqualsHighscore_NotCalled()
        {
            CommonInstall();
            
            _signalBus.Fire<GameLostSignal>();

            _fakePlayerPrefs.Received(1).GetInt(Arg.Any<string>());
            _fakePlayerPrefs.DidNotReceive().SetInt(Arg.Any<string>(), _score.CurrentScore);

            yield break;
        }
        
        
        [UnityTest]
        public IEnumerator PlayerPrefsSetInt_OnGameLostCurrentScoreLessThanHighscore_NotCalled()
        {
            CommonInstall();

            _fakePlayerPrefs.GetInt(Arg.Any<string>()).Returns(_score.CurrentScore + 10);
            _signalBus.Fire<GameLostSignal>();

            _fakePlayerPrefs.Received(1).GetInt(Arg.Any<string>());
            _fakePlayerPrefs.DidNotReceive().SetInt(Arg.Any<string>(), _score.CurrentScore);

            yield break;
        }
        
        [UnityTest]
        public IEnumerator PlayerPrefsSetInt_OnGameLostCurrentScoreMoreThanHighscore_IsCalled()
        {
            CommonInstall();
            
            _score.IncreaseScore();
            _signalBus.Fire<GameLostSignal>();

            _fakePlayerPrefs.Received(1).GetInt(Arg.Any<string>());
            _fakePlayerPrefs.Received(1).SetInt(Arg.Any<string>(), _score.CurrentScore);

            yield break;
        }
    }
}