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
            // Setup initial state by creating game objects from scratch, loading prefabs/scenes, etc

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
        public IEnumerator Current_Score_And_Hit_Combo_Is_Zero_On_Start()
        {
            CommonInstall();

            Assert.AreEqual(0, _score.CurrentScore);
            Assert.AreEqual(0, _fakeHitCombo.CurrentHitCombo);
            yield break;
        }

        [UnityTest]
        public IEnumerator Current_Score_And_Hit_Combo_Is_Reset_To_Zero_On_Start()
        {
            CommonInstall();

            Assert.AreEqual(0, _score.CurrentScore);
            _fakeHitCombo.Received(1).ResetStreak();
            Assert.AreEqual(0, _fakeHitCombo.CurrentHitCombo);

            yield break;
        }
        
        [UnityTest]
        public IEnumerator Failed_To_Save_Score_If_Current_Score_Equals_Saved_Highscore_When_Game_Lost()
        {
            CommonInstall();
            
            _signalBus.Fire<GameLostSignal>();

            _fakePlayerPrefs.Received(1).GetInt(Arg.Any<string>());
            _fakePlayerPrefs.DidNotReceive().SetInt(Arg.Any<string>(), _score.CurrentScore);

            yield break;
        }
        
        [UnityTest]
        public IEnumerator Failed_To_Save_Score_If_Current_Score_Less_Than_Saved_Highscore_When_Game_Lost()
        {
            CommonInstall();

            _fakePlayerPrefs.GetInt(Arg.Any<string>()).Returns(_score.CurrentScore + 10);
            _signalBus.Fire<GameLostSignal>();

            _fakePlayerPrefs.Received(1).GetInt(Arg.Any<string>());
            _fakePlayerPrefs.DidNotReceive().SetInt(Arg.Any<string>(), _score.CurrentScore);

            yield break;
        }
        
        [UnityTest]
        public IEnumerator Save_Score_If_Current_Score_Higher_Than_Saved_Highscore_When_Game_Lost()
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