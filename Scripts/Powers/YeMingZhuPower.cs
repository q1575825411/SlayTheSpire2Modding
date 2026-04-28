using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace MyFirstStS2Mod.Scripts.Powers;

[RegisterPower]
public class YeMingZhuPower : ModPowerTemplate
{
    private bool _triggered;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public bool TryConsumeEnergyRecovery()
    {
        if (_triggered)
        {
            return false;
        }

        _triggered = true;
        return true;
    }
}
