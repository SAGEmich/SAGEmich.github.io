using System;
using Configs;
using UnityEngine;
using FourForces.LinearFramework.Characters;
using FourForces.LinearFramework.Core;
using FourForces.LinearFramework.Database;
using FourForces.LinearFramework.Locations;

namespace Visualization
{
    public class FieldLocationElement : BaseGenericInteractableLocationElement<FieldLocationElement.FieldLocationElementData>
    {
        [SerializeField]
        private GameObject notPreparedPhase;
        [SerializeField]
        private GameObject readyToSeedPhase;
        [SerializeField]
        private GameObject[] grainPlantPhases;


        public override void UpdateVisualization(BaseLocationElementData databaseData)
        {
            base.UpdateVisualization(databaseData);

            notPreparedPhase.gameObject.SetActive(false);
            readyToSeedPhase.gameObject.SetActive(false);

            foreach(GameObject phase in grainPlantPhases)
            {
                phase.SetActive(false);
            }

            if(data.growthPhase == FieldGrowthPhase.NotPrepared)
            {
                notPreparedPhase.SetActive(true);
            }
            else if(data.growthPhase == FieldGrowthPhase.ReadyToSeed)
            {
                readyToSeedPhase.SetActive(true);
            }
            else
            {
                if(data.SeedItemConfigData.plantType == PlantType.Grain)
                {
                    grainPlantPhases[data.growthProgress].SetActive(true);
                }
            }

            //IS THIS REALLY OK??? - TO CHECK IT ONE DAY FOR POSSIBLE MEMORY LEAK???
            GameController.Instance.timeTickEvent -= OnTickEvent;
            GameController.Instance.timeTickEvent += OnTickEvent;
        }

        public override void InvokeInteraction(BaseCharacterController controller)
        {
            var characterController = controller as BaseHumanoidCharacterController;
            if(characterController == null)
            {
                return;
            }

            if(data.growthPhase == FieldGrowthPhase.NotPrepared && characterController.FirstHandWeaponConfig != null && characterController.FirstHandWeaponConfig.canPlowField)
            {
                characterController.Action_FieldPrepare();
                data.growthPhase = FieldGrowthPhase.ReadyToSeed;
            }

            if(data.growthPhase == FieldGrowthPhase.ReadyToSeed && characterController.FirstHandSeedConfig != null &&
               characterController.FirstHandSeedConfig.plantType != PlantType.None)
            {
                data.growthPhase = FieldGrowthPhase.Seeded;
                data.seedItemId = characterController.FirstHandSeedConfig.id;

                data.growthProgress = 0;
                data.phaseChangeTime = GameController.Instance.CurrentDate + BalanceConfig.Current.FieldPhaseGrowthTimeMinutes();

                characterController.Action_FieldSeeded(characterController.FirstHandSeedConfig.id);
            }

            if(data.growthPhase == FieldGrowthPhase.Harvest && characterController.FirstHandWeaponConfig != null && characterController.FirstHandWeaponConfig.canHarvestField)
            {
                characterController.Action_FieldHarvested(data.SeedItemConfigData);

                data.seedItemId = null;
                data.growthPhase = FieldGrowthPhase.NotPrepared;
            }

            UpdateVisualization(data);
        }

        private void OnTickEvent(GameDate gameDate)
        {
            if(data.growthPhase == FieldGrowthPhase.NotPrepared || data.growthPhase == FieldGrowthPhase.ReadyToSeed || data.growthPhase == FieldGrowthPhase.Harvest)
            {
                return;
            }

            if(data.phaseChangeTime > gameDate)
            {
                return;
            }

            if((data.growthProgress + 2) < grainPlantPhases.Length)
            {
                data.growthProgress += 1;
                data.phaseChangeTime = GameController.Instance.CurrentDate + BalanceConfig.Current.FieldPhaseGrowthTimeMinutes();
            }
            else
            {
                data.growthProgress = grainPlantPhases.Length - 1;
                data.growthPhase = FieldGrowthPhase.Harvest;
            }

            UpdateVisualization(data);
        }

        private void OnDisable()
        {
            if(GameController.Instance != null)
            {
                GameController.Instance.timeTickEvent -= OnTickEvent;
            }
        }


        [Serializable]
        public class FieldLocationElementData : BaseLocationElementData
        {
            public string seedItemId;
            public GameDate phaseChangeTime;

            public int growthProgress;
            public FieldGrowthPhase growthPhase;


            public SeedItemConfigData SeedItemConfigData
            {
                get { return ItemsConfig.Instance.Get(ItemsConfig.Categories.Seeds, seedItemId) as SeedItemConfigData; }
            }
        }

        public enum FieldGrowthPhase : byte
        {
            NotPrepared,
            ReadyToSeed,
            Seeded,
            Harvest
        }
    }
}