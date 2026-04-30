using Godot;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Scaffolding.Content;
using MyFirstStS2Mod.Scripts;

namespace MyFirstStS2Mod.Scripts.Events;

public abstract class MyFirstEvent : ModEventTemplate
{
    private const string PlaceholderEventPath = $"res://{Entry.ModId}/images/events/placeholder_event.svg";

    public override EventAssetProfile AssetProfile => new(
        InitialPortraitPath: PlaceholderEventPath
    );

    protected bool IsRunInAct(IRunState runState, int actIndex)
    {
        return RuntimeReflection.GetRunActIndex(runState) == actIndex;
    }

    protected bool IsFirstVisit(IRunState runState)
    {
        return !runState.VisitedEventIds.Contains(Id);
    }

    protected Task FinishInitialPageAsync()
    {
        SetEventFinished(PageDescription("FINISHED"));
        return Task.CompletedTask;
    }
}
