using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace MyFirstStS2Mod.Scripts.Powers;

[RegisterPower]
public sealed class QiQiaoPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
}
