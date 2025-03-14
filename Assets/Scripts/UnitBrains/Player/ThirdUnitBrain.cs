using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GluonGui.Dialog;
using UnitBrains.Player;
using UnityEngine;
using Utilities;

public class ThirdUnitBrain : DefaultPlayerUnitBrain
{
    public override string TargetUnitName => "Ironclad Behemoth";
    private static int _idCount = -1;
    private int _id = _idCount++;
    private const int SmartTargetNumber = 3;
    private List<Vector2Int> _targetsOutOfRange = new();

    private float _movedAt = float.MinValue;
    private float _attackedAt = float.MinValue;
    private const float StanceSwapCooldown = 1f;

    public override void Update(float deltaTime, float time)
    {

    }

    public override Vector2Int GetNextStep()
    {
        Vector2Int result = new();
        if (_targetsOutOfRange.Count == 0 || IsTargetInRange(_targetsOutOfRange[0]) || Time.time - _attackedAt <= StanceSwapCooldown)
        {
            result = unit.Pos;
        }
        else
        {
            result = unit.Pos.CalcNextStepTowards(_targetsOutOfRange[0]);
            _movedAt = Time.time;
        }
        _targetsOutOfRange.Clear();

        return result;
    }

    protected override List<Vector2Int> SelectTargets()
    {
        List<Vector2Int> result = new();
        List<Vector2Int> sortedTargets = GetAllTargets().ToList();
        SortByDistanceToOwnBase(sortedTargets);
        Vector2Int target = sortedTargets.Count < SmartTargetNumber ? sortedTargets[_id % sortedTargets.Count] : sortedTargets[_id % SmartTargetNumber];
        if (IsTargetInRange(target))
        {
            if (Time.time - _movedAt > StanceSwapCooldown)
            {
                result.Clear();
                result.Add(target);
            }
            _attackedAt = Time.time;
        }
        else
        {
            _targetsOutOfRange.Clear();
            _targetsOutOfRange.Add(target);
        }

        return result;
        ///////////////////////////////////////
    }
}
