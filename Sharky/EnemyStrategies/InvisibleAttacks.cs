﻿using Sharky.DefaultBot;
using System.Linq;

namespace Sharky.EnemyStrategies
{
    public class InvisibleAttacks : EnemyStrategy
    {
        public InvisibleAttacks(DefaultSharkyBot defaultSharkyBot)
        {
            EnemyStrategyHistory = defaultSharkyBot.EnemyStrategyHistory;
            ChatService = defaultSharkyBot.ChatService;
            ActiveUnitData = defaultSharkyBot.ActiveUnitData;
            SharkyOptions = defaultSharkyBot.SharkyOptions;
            DebugService = defaultSharkyBot.DebugService;
            UnitCountService = defaultSharkyBot.UnitCountService;
            FrameToTimeConverter = defaultSharkyBot.FrameToTimeConverter;
            EnemyData = defaultSharkyBot.EnemyData;
        }

        protected override bool Detect(int frame)
        {
            if (ActiveUnitData.EnemyUnits.Any(e => e.Value.Unit.DisplayType == SC2APIProtocol.DisplayType.Hidden && e.Value.Unit.UnitType != (uint)UnitTypes.PROTOSS_OBSERVER))
            {
                return true;
            }

            if (ActiveUnitData.EnemyUnits.Any(e => e.Value.UnitClassifications.Contains(UnitClassification.Cloakable) && e.Value.Unit.UnitType != (uint)UnitTypes.PROTOSS_OBSERVER))
            {
                return true;
            }

            if (UnitCountService.EnemyCount(UnitTypes.PROTOSS_DARKSHRINE) > 0 || UnitCountService.EnemyCount(UnitTypes.TERRAN_STARPORTTECHLAB) > 0 || UnitCountService.EnemyCount(UnitTypes.ZERG_LURKERDENMP) > 0)
            {
                return true;
            }

            return false;
        }
    }
}
