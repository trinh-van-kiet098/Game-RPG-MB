using System.Collections.Generic;
using UnityEngine;
using Game.Events;

public class UnitManager : ConfiguredMonoBehaviour
{
    private Dictionary<int, UnitController> unitMap = new Dictionary<int, UnitController>();
    public string pathPlayer = "Assets/Project/ExplosiveLLC/RPG-Character/Prefabs/Player-1.prefab";

    private List<UnitController> activeUnits = new List<UnitController>();
    public bool CanUpdateUnits = false;

    public override void Configure(SystemGameManager system)
    {
        base.Configure(system);
        systemEventManager.OnUnitSpawned += HandleUnitSpawned;
        systemEventManager.OnUnitDied += HandleUnitDied;
    }

    public void RegisterUnit(UnitController unit)
    {
        if (unit == null || unit.ID == 0) return;

        if (!unitMap.ContainsKey(unit.ID))
        {
            unitMap.Add(unit.ID, unit);
            activeUnits.Add(unit);
        }
    }

    public void ClearAllUnits()
    {
        foreach (var unit in activeUnits)
        {
            if (unit != null && unit.gameObject != null)
            {
                Destroy(unit.gameObject);
            }
        }
        unitMap.Clear();
        activeUnits.Clear();
    }

    public void UnregisterUnit(int id)
    {
        if (unitMap.TryGetValue(id, out UnitController unit))
        {
            unitMap.Remove(id);
            activeUnits.Remove(unit);
        }
    }

    public UnitController GetUnitByID(int id)
    {
        if (unitMap.TryGetValue(id, out UnitController unit)) return unit;
        return null;
    }

    private void Update()
    {
        if (CanUpdateUnits == false) return;
        float dt = Time.deltaTime;
        for (int i = activeUnits.Count - 1; i >= 0; i--)
        {
            UnitController unit = activeUnits[i];

            if (unit != null)
            {
                unit.TickUpdate(dt);
            }
        }
    }

    private void HandleUnitSpawned(UnitSpawnedEvent evt)
    {
        if (evt.IsLocalPlayer)
        {
            UnitController spawnedUnit = GetUnitByID(evt.UnitId);
            if (spawnedUnit != null)
            {
                systemEventManager.NotifyChangeCameraMode(new ChangeCameraModeEvent { TargetMode = CameraMode.Home });
            }
        }
    }

    private void HandleUnitDied(UnitDiedGlobalEvent evt)
    {
        UnitController deadUnit = GetUnitByID(evt.DeadUnitId);
        if (deadUnit != null)
        {
            UnregisterUnit(evt.DeadUnitId);
            Destroy(deadUnit.gameObject, 3f);
        }
    }

    private void OnDestroy()
    {
        if (systemEventManager != null)
        {
            systemEventManager.OnUnitSpawned -= HandleUnitSpawned;
            systemEventManager.OnUnitDied -= HandleUnitDied;
        }
    }
}