using Content.Shared._RMC14.Power;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._RMC14.Power;

[UsedImplicitly]
public sealed class RMCPortableGeneratorBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [ViewVariables]
    private RMCPortableGeneratorWindow? _window;

    protected override void Open()
    {
        base.Open();
        _window = this.CreateWindow<RMCPortableGeneratorWindow>();
        _window.SetEntity(Owner);

        _window.OnState += args =>
        {
            if (args)
            {
                Start();
            }
            else
            {
                Stop();
            }
        };

        _window.OnPower += SetTargetPower;
        _window.OnEjectFuel += EjectFuel;
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is not RMCPortableGeneratorUiState msg)
            return;

        _window?.Update(msg);
    }

    public void SetTargetPower(int target)
    {
        SendMessage(new RMCPortableGeneratorSetTargetPowerMessage(target));
    }

    public void Start()
    {
        SendMessage(new RMCPortableGeneratorStartMessage());
    }

    public void Stop()
    {
        SendMessage(new RMCPortableGeneratorStopMessage());
    }

    public void EjectFuel()
    {
        SendMessage(new RMCPortableGeneratorEjectFuelMessage());
    }
}
