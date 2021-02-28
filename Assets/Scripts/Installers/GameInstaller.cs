using Data;
using Interfaces;
using Signals;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<GameLostSignal>();

            Container.BindInterfacesAndSelfTo<Score>().AsSingle();
            Container.Bind<HitCombo>().AsSingle();

            Container.Bind<IPlayerPrefs>().To<CustomPlayerPrefs>().FromInstance(new CustomPlayerPrefs()).AsSingle();
        }
    }
}