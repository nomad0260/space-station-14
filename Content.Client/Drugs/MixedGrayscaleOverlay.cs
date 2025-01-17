﻿using Content.Shared.Drugs;
using Content.Shared.StatusEffect;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Drugs;

public sealed class MixedGrayscaleOverlay : Overlay
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IEntitySystemManager _sysMan = default!;

    public override OverlaySpace Space => OverlaySpace.WorldSpace;
    public override bool RequestScreenTexture => true;
    private readonly ShaderInstance _mixedGrayscaleShader;

    public float Intoxication = 0.0f;

    private const float VisualThreshold = 3.0f;
    private const float PowerDivisor = 80.0f;

    private float EffectScale => Math.Clamp((Intoxication - VisualThreshold) / PowerDivisor, 0.0f, 1.0f);

    public MixedGrayscaleOverlay()
    {
        IoCManager.InjectDependencies(this);
        _mixedGrayscaleShader = _prototypeManager.Index<ShaderPrototype>("MixedGrayscale").InstanceUnique();
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        var playerEntity = _playerManager.LocalPlayer?.ControlledEntity;

        if (playerEntity == null)
            return;

        if (!_entityManager.HasComponent<CrazyRussianDrugComponent>(playerEntity)
            || !_entityManager.TryGetComponent<StatusEffectsComponent>(playerEntity, out var status))
            return;

        var statusSys = _sysMan.GetEntitySystem<StatusEffectsSystem>();
        if (!statusSys.TryGetTime(playerEntity.Value, DrugOverlaySystem.MixedGrayscaleKey, out var time, status))
            return;

        var timeLeft = (float) (time.Value.Item2 - time.Value.Item1).TotalSeconds;
        Intoxication += (timeLeft - Intoxication) * args.DeltaSeconds / 16f;
    }

    protected override bool BeforeDraw(in OverlayDrawArgs args)
    {
        if (!_entityManager.TryGetComponent(_playerManager.LocalPlayer?.ControlledEntity, out EyeComponent? eyeComp))
            return false;

        if (args.Viewport.Eye != eyeComp.Eye)
            return false;

        return EffectScale > 0;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (ScreenTexture == null)
            return;

        var handle = args.WorldHandle;
        _mixedGrayscaleShader.SetParameter("SCREEN_TEXTURE", ScreenTexture);
        _mixedGrayscaleShader.SetParameter("effectScale", EffectScale);
        handle.UseShader(_mixedGrayscaleShader);
        handle.DrawRect(args.WorldBounds, Color.White);
        handle.UseShader(null);
    }
}
