using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using VRageMath;
using VRage.Game;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Ingame;
using Sandbox.Game.EntityComponents;
using VRage.Game.Components;
using VRage.Collections;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
public sealed class Program : MyGridProgram
{
    // НАЧАЛО СКРИПТА
    string sortTag = "Sort";
    string componentsTag = "Comp";
    string ingotsTag = "Ingots";
    string toolsTag = "Tools";
    string gravelTag = "Gravel";
    string baseTag = "Base";
    string iceTag = "Ice";
    string bottleTag = "Bottle";
    string refillTag = "Refill";

    string strOre = "MyObjectBuilder_Ore";
    string strIngot = "MyObjectBuilder_Ingot";
    // string strCobalt = "MyObjectBuilder_Ingot/Cobalt";
    // string strGold = "MyObjectBuilder_Ingot/Gold";
    // string strSilver = "MyObjectBuilder_Ingot/Silver";
    // strirUran = "MyObjectBuilder_Ingot/Uranium";
    // string strMagnes = "MyObjectBuilder_Ingot/Magnesium";
    // string strPlatinum = "MyObjectBuilder_Ingot/Platinum";
    // string strIron = "MyObjectBuilder_Ingot/Iron";
    // string strNickel = "MyObjectBuilder_Ingot/Nickel";
    // string strSilicon = "MyObjectBuilder_Ingot/Silicon";
    // string strStone = "MyObjectBuilder_Ingot/Stone";
    string strGravel = "Stone";
    string strIce = "Ice";
    string strHydro = "MyObjectBuilder_GasContainerObject";
    string strOxygen = "MyObjectBuilder_OxygenContainerObject";

    string strComponent = "MyObjectBuilder_Component";
    // string strSteelPlate = "MyObjectBuilder_Component/SteelPlate";
    // string strScrap = "MyObjectBuilder_Ore/Scrap";
    string strTools = "MyObjectBuilder_PhysicalGunObject";
    string strAmmo = "MyObjectBuilder_AmmoMagazine";
    string strConsumable = "MyObjectBuilder_ConsumableItem";
    string strObject = "MyObjectBuilder_PhysicalObject";
    int i = 1;

    public Program()
    {
        i = 0;
        Runtime.UpdateFrequency = UpdateFrequency.Update100;
    }

    public void Main(string args)
    {
        Echo("Сортировка: " + i++);
        var cargoTmp = new List<IMyCargoContainer>();
        var cargoSort = new List<IMyCargoContainer>();
        var cargoComponents = new List<IMyCargoContainer>();
        var cargoIngots = new List<IMyCargoContainer>();
        var cargoTools = new List<IMyCargoContainer>();
        var cargoGravel = new List<IMyCargoContainer>();
        var cargoBase = new List<IMyCargoContainer>();
        var cargoIce = new List<IMyCargoContainer>();
        var cargoBottle = new List<IMyCargoContainer>();
        var assemblers = new List<IMyAssembler>();
        var refineries = new List<IMyRefinery>();
        var connectors = new List<IMyShipConnector>();
        var cryos = new List<IMyCryoChamber>();
        var collectors = new List<IMyShipConnector>();
        var o2h2Gens = new List<IMyGasGenerator>();
        var tanks = new List<IMyGasTank>();
        IMyGasTank oxygenRefill = null;
        IMyGasTank hydrogenRefill = null;

        // block declarations
        GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargoTmp, filterBlock);
        GridTerminalSystem.GetBlocksOfType<IMyAssembler>(assemblers, filterBlock);
        GridTerminalSystem.GetBlocksOfType<IMyRefinery>(refineries, filterBlock);
        GridTerminalSystem.GetBlocksOfType<IMyCryoChamber>(cryos, filterBlock);
        GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors, filterBlock);
        GridTerminalSystem.GetBlocksOfType<IMyGasTank>(tanks, filterBlock);
        GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(o2h2Gens, filterBlock);
        if (cargoTmp.Count > 0)
        {
            for (int i = 0; i < cargoTmp.Count; i++)
            {
                filterCargo(cargoSort, cargoTmp[i], sortTag);
                filterCargo(cargoComponents, cargoTmp[i], componentsTag);
                filterCargo(cargoIngots, cargoTmp[i], ingotsTag);
                filterCargo(cargoTools, cargoTmp[i], toolsTag);
                filterCargo(cargoGravel, cargoTmp[i], gravelTag);
                filterCargo(cargoBase, cargoTmp[i], baseTag);
                filterCargo(cargoIce, cargoTmp[i], iceTag);
                filterCargo(cargoBottle, cargoTmp[i], bottleTag);
            }
            for (int i = 0; (i < tanks.Count) && ((hydrogenRefill == null) || (hydrogenRefill == null)); i++)
            {
                //Echo("" + tanks[i].CustomName);
                if (tanks[i].CustomName.IndexOf(refillTag) > -1)
                {
                    //Echo("" + tanks[i].Capacity);
                    if (tanks[i].Capacity == 15000000)
                    {
                        hydrogenRefill = tanks[i];
                    }
                    if (tanks[i].Capacity == 100000)
                    {
                        oxygenRefill = tanks[i];
                    }
                }
            }

            Echo("Количество компонентов: " + cargoComponents.Count);
            clean(cargoComponents.ConvertAll(x => (IMyEntity)x), cargoSort, 0, filterComponentInverted);

            Echo("Количество слитков: " + cargoIngots.Count);
            clean(cargoIngots.ConvertAll(x => (IMyEntity)x), cargoSort, 0, filterIngotsInverted);

            Echo("Количество Инструментов: " + cargoTools.Count);
            clean(cargoTools.ConvertAll(x => (IMyEntity)x), cargoSort, 0, filterToolsInverted);

            Echo("Количество гравия: " + cargoGravel.Count);
            clean(cargoGravel.ConvertAll(x => (IMyEntity)x), cargoSort, 0, filterGravelInverted);

            Echo("Количество льда: " + cargoIce.Count);
            clean(cargoIce.ConvertAll(x => (IMyEntity)x), cargoSort, 0, filterIceInverted);

            Echo("Количество базовых: " + cargoBase.Count);
            clean(cargoBase.ConvertAll(x => (IMyEntity)x), cargoSort);

            Echo("Количество коллекторов: " + collectors.Count);
            clean(collectors.ConvertAll(x => (IMyEntity)x), cargoSort);

            Echo("Количество коннекторов: " + connectors.Count);
            clean(connectors.ConvertAll(x => (IMyEntity)x), cargoSort);

            Echo("Количество крио: " + cryos.Count);
            clean(cryos.ConvertAll(x => (IMyEntity)x), cargoSort);

            Echo("Количество o2h2: " + o2h2Gens.Count);
            clean(o2h2Gens.ConvertAll(x => (IMyEntity)x), cargoBottle, 0, filterIceInverted);

            Echo("Количество бутылочек: " + cargoBottle.Count);
            clean(cargoBottle.ConvertAll(x => (IMyEntity)x), cargoSort, 0, filterBottleInverted);
            var filledBottles = new List<int>();
            int hydro = 6;
            int oxygen = 6;
            if (hydrogenRefill != null)
            {
                hydrogenRefill.RefillBottles();
                hydrogenRefill.Stockpile = true;
                hydrogenRefill.AutoRefillBottles = true;
                var gasInv = hydrogenRefill.GetInventory();
                var gasBottles = new List<MyInventoryItem>();
                gasInv.GetItems(gasBottles);
                transfer(gasInv, gasBottles, cargoBottle);
                filledBottles.AddList(gasBottles.ConvertAll(x => x.GetHashCode()));
            } else
            {
                Echo("Не найден бак для заправки Н2");
            }
            if (oxygenRefill != null)
            {
                oxygenRefill.RefillBottles();
                oxygenRefill.Stockpile = true;
                oxygenRefill.AutoRefillBottles = true;
                var gasInv = oxygenRefill.GetInventory();
                var gasBottles = new List<MyInventoryItem>();
                gasInv.GetItems(gasBottles);
                transfer(gasInv, gasBottles, cargoBottle);
                filledBottles.AddList(gasBottles.ConvertAll(x => x.GetHashCode()));
            }
            else
            {
                Echo("Не найден бак для заправки О2");
            }

            // Echo("Бутылки в системе:");
            for (int i = 0; i < cargoBottle.Count; i++)
            {
                IMyInventory inv = cargoBottle[i].GetInventory();
                var bottles = new List<MyInventoryItem>();
                inv.GetItems(bottles);
                var needRefillHydrogen = new List<MyInventoryItem>();
                var needRefillOxygen = new List<MyInventoryItem>();
                for (int j = 0; j < bottles.Count; j++)
                {
                    var bottleId = bottles[j].GetHashCode();
                    // Echo(bottleId + " ");
                    if (!filledBottles.Contains(bottleId))
                    {
                        if ((bottles[j].Type.TypeId == strHydro) && (hydro > 0))
                        {
                            hydro--;
                            needRefillHydrogen.Add(bottles[j]);
                        }
                        else if ((bottles[j].Type.TypeId == strOxygen) && (oxygen > 0))
                        {
                            oxygen--;
                            needRefillOxygen.Add(bottles[j]);
                        }
                    }
                }
                if (hydrogenRefill != null)
                {
                    transfer(inv, needRefillHydrogen, hydrogenRefill.GetInventory());
                }
                if (oxygenRefill != null)
                {
                    transfer(inv, needRefillOxygen, oxygenRefill.GetInventory());
                }
            }

            Echo("Количество ассемблеров: " + assemblers.Count);
            clean(assemblers.ConvertAll(x => (IMyEntity)x), cargoSort, 1);
            for (int i = 0; i < assemblers.Count; i++)
            {
                IMyInventory invIngots = assemblers[i].GetInventory(0);
                float assemblerLoad = (float)invIngots.CurrentVolume / (float)invIngots.MaxVolume;
                Echo("Загрузка ассемблера: " + assemblerLoad * 100 + "%");
                if ((float) invIngots.CurrentVolume / (float) invIngots.MaxVolume > 0.9)
                {
                    var ingots = new List<MyInventoryItem>();
                    invIngots.GetItems(ingots);
                    transfer(invIngots, ingots, cargoSort);
                }
            }

            Echo("Количество рефайнери: " + refineries.Count);
            clean(refineries.ConvertAll(x => (IMyEntity)x), cargoSort, 1);

            Echo("Количество сортировочных: " + cargoSort.Count);
            for (int i = 0; i < cargoSort.Count; i++)
            {
                IMyInventory inv = cargoSort[i].GetInventory();

                var test = new List<MyInventoryItem>();
                inv.GetItems(test);
                for (int j = 0; j < test.Count; j++)
                {
                    Echo(test[j].ItemId.ToString());
                    Echo(test[j].Type.TypeId);
                    Echo(test[j].Type.SubtypeId);
                }

                var components = new List<MyInventoryItem>();
                inv.GetItems(components, filterComponents);
                transfer(inv, components, cargoComponents);
                var ingots = new List<MyInventoryItem>();
                inv.GetItems(ingots, filterIngots);
                transfer(inv, ingots, cargoIngots);
                var tools = new List<MyInventoryItem>();
                inv.GetItems(tools, filterTools);
                transfer(inv, tools, cargoTools);
                var gravel = new List<MyInventoryItem>();
                inv.GetItems(gravel, filterGravel);
                transfer(inv, gravel, cargoGravel);
                var ice = new List<MyInventoryItem>();
                inv.GetItems(ice, filterIce);
                transfer(inv, ice, cargoIce);
                var bottles = new List<MyInventoryItem>();
                inv.GetItems(bottles, filterBottle);
                transfer(inv, bottles, cargoBottle);
            }
        }
        else
        {
            Echo("Не найдены контейнеры для сортировки\n");
        }

        Echo("Сортировка включена");
        IMyTextSurface screen = Me.GetSurface(0);
        screen.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
        screen.AddImagesToSelection(new List<string>(new string[] { "Online" }), true);
    }

    public void clean(List<IMyEntity> blocks, List<IMyCargoContainer> targetCargos, int inventory = 0, Func<MyInventoryItem, bool> filter = null)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            IMyInventory inv = blocks[i].GetInventory(inventory);
            var items = new List<MyInventoryItem>();
            inv.GetItems(items, filter);
            transfer(inv, items, targetCargos);
        }
    }

    void transfer(IMyInventory src, List<MyInventoryItem> items, List<IMyCargoContainer> targetCargos)
    {
        for (int i = 0; i < items.Count; i++)
        {
            VRage.MyFixedPoint currentAmount = items[i].Amount;
            for (int j = 0; j < targetCargos.Count; j++)
            {
                var targetInv = targetCargos[j].GetInventory();

                VRage.MyFixedPoint transferAmount;
                if (getVolume(targetInv) > getVolume(items[i], currentAmount))
                {
                    transferAmount = currentAmount;
                } else
                {
                    transferAmount = VRage.MyFixedPoint.Floor((VRage.MyFixedPoint) ((float) getVolume(targetInv) / items[i].Type.GetItemInfo().Volume));
                }

                // Echo("Из " + cargo.CustomName + " попытка перенести  " + items[i].Type.SubtypeId + " в карго " + targetCargos[j].CustomName);
                // Echo("Объём объекта " + items[i].Type.GetItemInfo().Volume + " в карго объёмом" + (targetInv.MaxVolume - targetInv.CurrentVolume));

                if (src.TransferItemTo(targetInv, items[i], transferAmount))
                {
                    // Echo("Удалось");
                    currentAmount -= transferAmount;
                }

                if (currentAmount == 0)
                {
                    break;
                }
                // Echo("Не удалось");
            }
        }
    }

    void transfer(IMyInventory src, List<MyInventoryItem> items, IMyInventory target)
    {
        for (int i = 0; i < items.Count; i++)
        {
            VRage.MyFixedPoint currentAmount = items[i].Amount;
            
            VRage.MyFixedPoint transferAmount;
            if (getVolume(target) > getVolume(items[i], currentAmount))
            {
                transferAmount = currentAmount;
            }
            else
            {
                transferAmount = VRage.MyFixedPoint.Floor((VRage.MyFixedPoint)((float)getVolume(target) / items[i].Type.GetItemInfo().Volume));
            }

            // Echo("Из " + cargo.CustomName + " попытка перенести  " + items[i].Type.SubtypeId + " в карго " + targetCargos[j].CustomName);
            // Echo("Объём объекта " + items[i].Type.GetItemInfo().Volume + " в карго объёмом" + (targetInv.MaxVolume - targetInv.CurrentVolume));

            src.TransferItemTo(target, items[i], transferAmount);
        }
    }

    public void filterCargo(List<IMyCargoContainer> cargos, IMyCargoContainer targetCargo, String tag)
    {
        if (targetCargo.CustomName.IndexOf(tag) > -1)
        {
            cargos.Add(targetCargo);
        }
    }

    /*
    public void filterEnabledOff<T>(List<T> listTmp, List<T> list) where T : IMyFunctionalBlock
    {
        for (int i = 0; i < listTmp.Count; i++)
        {
            if (!listTmp[i].Enabled)
            {
                list.Add(listTmp[i]);
            }
        }
    }
    */

    VRage.MyFixedPoint getVolume(IMyInventory inv)
    {
        return inv.MaxVolume - inv.CurrentVolume;
    }

    VRage.MyFixedPoint getVolume(MyInventoryItem item)
    {
        return item.Type.GetItemInfo().Volume * item.Amount;
    }

    VRage.MyFixedPoint getVolume(MyInventoryItem item, VRage.MyFixedPoint amount)
    {
        return item.Type.GetItemInfo().Volume * amount;
    }

    bool filterBlock(IMyTerminalBlock block)
    {
        return block.CubeGrid == Me.CubeGrid;
    }

    bool filterComponents(MyInventoryItem item)
    {
        // Echo(item.Type.TypeId);
        return item.Type.TypeId == strComponent;
    }

    bool filterComponentInverted(MyInventoryItem item)
    {
        return !filterComponents(item);
    }

    bool filterIngots(MyInventoryItem item)
    {
        return (item.Type.TypeId == strIngot) && (item.Type.SubtypeId != strGravel);
    }

    bool filterIngotsInverted(MyInventoryItem item)
    {
        //Echo(item.Type.TypeId);
        //Echo(item.Type.SubtypeId);
        return !filterIngots(item);
    }

    bool filterTools(MyInventoryItem item)
    {
        // Echo(item.Type.TypeId);
        return (item.Type.TypeId == strTools) || (item.Type.TypeId == strAmmo) || (item.Type.TypeId == strConsumable) || (item.Type.TypeId == strObject);
    }

    bool filterToolsInverted(MyInventoryItem item)
    {
        return !filterTools(item);
    }

    bool filterGravel(MyInventoryItem item)
    {
        // Echo(item.Type.TypeId);
        return (item.Type.TypeId == strIngot) && (item.Type.SubtypeId == strGravel);
    }

    bool filterGravelInverted(MyInventoryItem item)
    {
        return !filterGravel(item);
    }

    bool filterIce(MyInventoryItem item)
    {
        // Echo(item.Type.TypeId);
        return (item.Type.TypeId == strOre) && (item.Type.SubtypeId == strIce);
    }

    bool filterIceInverted(MyInventoryItem item)
    {
        return !filterIce(item);
    }

    bool filterBottle(MyInventoryItem item)
    {
        return (item.Type.TypeId == strHydro) || (item.Type.TypeId == strOxygen);
    }

    bool filterBottleInverted(MyInventoryItem item)
    {
        return !filterBottle(item);
    }

    public void Save()
    { }
    // КОНЕЦ СКРИПТА
}