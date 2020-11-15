using CitizenFX.Core;
using FivePD.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Dynamic;
using FivePD.API.Utils;

namespace Port_Authority_Callouts
{
    [CalloutProperties("[SQC]Protest", "Squibsie", "1.0")]
    class Protest : FivePD.API.Callout
    {
        Ped ProtestMain;
        Ped Protest2;
        Ped Protest3;
        Ped Protest4;
        Ped Protest5;
        Ped Protest6;
        Ped Protest7;
        Ped Protest8;
        Ped Protest9;
        Ped Protest10;
        Ped Protest11;
        Ped Protest12;

        Random random = new Random();

        int ProtestType;
        public Protest()
        {
            this.InitInfo(new Vector3(224.168f,878.352f,30.492f));

            this.ShortName = "Protest";
            this.CalloutDescription = "A group is forming in Legion Square, possible protest, please respond and investigate.";
            this.ResponseCode = 2;

            /* How close the player needs to be to start the action (OnStart())*/
            this.StartDistance = 50f; // 30 feet? metres? unit...
        }

        public async override Task OnAccept()
        {
            this.InitBlip();

            //Spawn the protestors
            ProtestMain = await SpawnPed(PedHash.AirworkerSMY, new Vector3(216.777f,-876.535f, 32.736f), 287.449f);
            Protest2 = await SpawnPed(PedHash.Armymech01SMY, new Vector3(223.078f,-882.54f,30.492f), 52.675f);
            Protest3 = await SpawnPed(PedHash.Armymech01SMY, new Vector3(224.152f,-880.385f,30.492f), 69.209f);
            Protest4 = await SpawnPed(PedHash.Armymech01SMY, new Vector3(224.168f, -878.352f, 30.492f), 69.209f);
            Protest5 = await SpawnPed(PedHash.Armymech01SMY, new Vector3(223.459f, -875.783f, 30.492f), 52.675f);
            Protest6 = await SpawnPed(PedHash.Armymech01SMY, new Vector3(222.1f, -873.801f, 30.492f), 52.675f);
            Protest7 = await SpawnPed(PedHash.Armymech01SMY, new Vector3(221.362f, -871.36f, 30.492f), 52.675f);
            Protest8 = await SpawnPed(PedHash.Armymech01SMY, new Vector3(220.113f, -869.49f, 30.492f), 52.675f);
            Protest9 = await SpawnPed(PedHash.Armymech01SMY, new Vector3(218.444f, -867.847f, 30.492f), 52.675f);
            Protest10 = await SpawnPed(PedHash.Armymech01SMY, new Vector3(221.63f, -868.161f, 30.492f), 52.675f);
            Protest11 = await SpawnPed(PedHash.Armymech01SMY, new Vector3(223.837f, -873.415f, 30.492f), 52.675f);
            Protest12 = await SpawnPed(PedHash.Armymech01SMY, new Vector3(224.546f, -876.434f, 30.492f), 52.675f);
        }

        public override void OnStart(Ped closest)
        {


            base.OnStart(closest);
            Events.OnPedArrested += OnPedArrested;

            ShowNetworkedNotification("Monitor the protest and ensure it stays lawful.", "CHAR_CALL911", "CHAR_CALL911", "Control", "Protest", 50f);

            int dice = random.Next(0, 100);

            if (dice < 25)
            {
                //Outcome 1 - Peaceful Protest
                ShowDialog("[Protestor] The scientists are just government shills!", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] They're simply used to push their agendas, through a so called trusted source.", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] We cannot let central government rule our lives, we must resist!", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] We must show the establishment that we do not simply lie down!", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] WE ARE NOT SHEEP! WE ARE WOLVES!", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] But violence does not answer any questions.", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] We should establish peaceful modes of protest.", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] Modes that do not see us on the wrong side of the law.", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] As you see, the law are here already.", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] And thank you officers for allowing us to gather and speak.", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] Establish your mode of protest and keep active.", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] Remember to keep your information private, the government will use it against you.", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] That is all I have to say. Thank you for your time!", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protest Crowd] *CHEERS*", 5000, 40f);
                BaseScript.Delay(1500);

                ShowNetworkedNotification("The protest is ending, nice work.", "CHAR_CALL911", "CHAR_CALL911", "Control", "Protest", 50f);
                base.EndCallout();
            }
            else if (dice > 25 && dice < 50)
            {
                //Outcome 2 - Violent protest.
                ShowDialog("[Protestor] We cannot be oppressed!", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] Defund the police!", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] Fuck the police!", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] No Justice, No Peace!", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] Fuck the cops!", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] No Justice, No Peace!", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] Fight the cops!", 5000, 40f);
                BaseScript.Delay(2500);
                Protest12.Task.FightAgainst(closest);
                ShowDialog("[Protestor] Fuck you Pig!", 5000, 40f);
                BaseScript.Delay(5000);
                Protest11.Task.FightAgainst(closest);
                ShowDialog("[Protestor] Don't fucking touch him you!", 5000, 40f);
                BaseScript.Delay(5000);
                Ped RandTarget = AssignedPlayers.SelectRandom();
                Protest10.Task.FightAgainst(RandTarget);
                RandTarget = AssignedPlayers.SelectRandom();
                Protest9.Task.FightAgainst(RandTarget);
                RandTarget = AssignedPlayers.SelectRandom();
                Protest8.Task.FightAgainst(RandTarget);
                ShowDialog("[Protestor] NO JUSTICE, NO PEEEEEAAACCCEEE!", 5000, 40f);
                BaseScript.Delay(5000);
                RandTarget = AssignedPlayers.SelectRandom();
                Protest7.Task.FightAgainst(RandTarget);
                Protest6.Task.FightAgainst(RandTarget);
                RandTarget = AssignedPlayers.SelectRandom();
                Protest5.Task.FightAgainst(RandTarget);
                Protest4.Task.FightAgainst(RandTarget);
                RandTarget = AssignedPlayers.SelectRandom();
                Protest3.Task.FightAgainst(RandTarget);
                Protest2.Task.FightAgainst(RandTarget);

            }
            else if (dice > 50 && dice < 60)
            {
                //Outcome 3 - Armed Protest
                ProtestType = 3; 
                Protest12.Weapons.Give(WeaponHash.CarbineRifle, 60, true, true);
                Protest10.Weapons.Give(WeaponHash.CarbineRifle, 60, true, true);
                Protest8.Weapons.Give(WeaponHash.CarbineRifle, 60, true, true);

                Protest9.Weapons.Give(WeaponHash.AssaultRifle, 60, true, true);
                Protest11.Weapons.Give(WeaponHash.AssaultRifle, 60, true, true);
                Protest7.Weapons.Give(WeaponHash.AssaultRifle, 60, true, true);
                Protest6.Weapons.Give(WeaponHash.AssaultRifle, 60, true, true);

                Protest5.Weapons.Give(WeaponHash.BullpupShotgun, 60, true, true);
                Protest4.Weapons.Give(WeaponHash.DoubleBarrelShotgun, 60, true, true);
                Protest3.Weapons.Give(WeaponHash.SweeperShotgun, 60, true, true);
                Protest2.Weapons.Give(WeaponHash.Musket, 60, true, true);

                ShowDialog("[Protestor] Los Santos! The governor wants your guns!", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] He wants to shit on your constitutional rights!", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] Right now, open carriage is legal. But not for long!", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] We cannot be oppressed!", 5000, 40f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] Stand up for your rights. Fight the establishment!", 5000, 40f);
                BaseScript.Delay(2500);
                ShowNetworkedNotification("The leader of the protest is wanted by the ATF. Arrest him.", "CHAR_CALL911", "CHAR_CALL911", "Control", "Protest", 50f);
                ShowDialog("[Protestor] Do not let them dismantle us!", 5000, 40f);
                BaseScript.Delay(2500);
            }
            else if (dice > 60 && dice < 65)
            {
                ShowDialog("[Protestor] The issue comes that the muslims are taking this place over.", 5000, 30f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] Soon enough, wee'll be bowing to them.", 5000, 30f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] They'll be shutting down cluckin bell because it ain't halal!", 5000, 30f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] The governor don't give a fuck. He should be blocking their applications!", 5000, 30f);
                BaseScript.Delay(5000);
                ShowDialog("[Protestor] Else we'll end up with a mosque on every damn corner of this city.", 5000, 30f);
                BaseScript.Delay(5000);
                dice = random.Next(0, 100);
                if (dice < 50)
                {
                    ShowDialog("[Counter-Protestor] Fuck you racist!", 5000, 30f);
                    BaseScript.Delay(5000);
                    ShowDialog("[Protestor] Haha, you think you'll win here?", 5000, 30f);
                    BaseScript.Delay(5000);
                    ShowDialog("[Counter-Protestor] I'll see you in pieces!", 5000, 30f);
                    BaseScript.Delay(250);
                    CitizenFX.Core.World.AddExplosion(Protest5.Position, ExplosionType.StickyBomb, 10f, 10f);
                }
                else
                {
                    ShowDialog("[Protestor] Then they'll take our women", 5000, 30f);
                    BaseScript.Delay(5000);
                    dice = random.Next(0, 100);
                    if (dice < 50)
                    {
                        ShowDialog("[Counter-Protestor] Fuck you racist!", 5000, 30f);
                        BaseScript.Delay(5000);
                        ShowDialog("[Protestor] Haha, you think you'll win here?", 5000, 30f);
                        BaseScript.Delay(5000);
                        ShowDialog("[Counter-Protestor] I'll see you in pieces!", 5000, 30f);
                        BaseScript.Delay(250);
                        CitizenFX.Core.World.AddExplosion(Protest5.Position, ExplosionType.StickyBomb, 10f, 10f);
                    } else
                    {
                        ShowDialog("[Protestor] Then they'll take our women", 5000, 30f);
                        BaseScript.Delay(5000);
                        //Complete Dialogue
                    }
                }


            }

        }

        public async Task OnPedArrested(Ped Ped, Ped Player)
        {
            if(ProtestType == 3)
            {
                int dice = random.Next(0, 100);
                if(dice >= 50) 
                {
                    ShowDialog("[Protestor] You're nothing more than a slave to the state!", 5000, 40f);
                    BaseScript.Delay(2500);
                    ShowDialog("[Protestor 2] Fucking let him go!", 5000, 40f);
                    BaseScript.Delay(2500);
                    Protest2.Task.AimAt(Player, 5000);
                    Protest3.Task.AimAt(Player, 5000);
                    Protest5.Task.AimAt(Player, 5000);
                    ShowDialog("[Protestor 2] They're going to take him away!", 5000, 40f);
                    BaseScript.Delay(2500);
                    Ped RandTarget = AssignedPlayers.SelectRandom();
                    Protest2.Task.FightAgainst(RandTarget, 5000);
                    RandTarget = AssignedPlayers.SelectRandom();
                    ShowDialog("[Protestor 2] Fuck you guys!", 5000, 40f);
                    Protest3.Task.FightAgainst(RandTarget, 5000);
                    RandTarget = AssignedPlayers.SelectRandom();
                    Protest5.Task.FightAgainst(RandTarget, 5000);
                    Protest7.Task.FightAgainst(RandTarget, 5000);
                    Protest4.Task.FleeFrom(Player);
                    Protest6.Task.FleeFrom(Player);
                    Protest8.Task.FleeFrom(Player);
                    Protest9.Task.FleeFrom(Player);
                    Protest10.Task.FleeFrom(Player);
                    Protest11.Task.FleeFrom(Player);
                    Protest12.Task.FleeFrom(Player);
                } else
                {
                    ShowDialog("[Protestor 2] Fucking let him go!", 5000, 40f);
                    BaseScript.Delay(2500);
                    ShowDialog("[Protestor 2] You can't do this!", 5000, 40f);
                    BaseScript.Delay(2500);
                    Protest2.Task.AimAt(Player, 10000);
                    ShowDialog("[Protestor 3] Are you mad? don't aim at the cops!", 2500, 40f);
                    BaseScript.Delay(500);
                    Protest3.Task.FightAgainst(Protest2);
                    BaseScript.Delay(250);
                    Protest5.Task.FightAgainst(Protest3);

                    Protest4.Task.ReactAndFlee(Protest5);
                    Protest6.Task.ReactAndFlee(Protest5);
                    Protest7.Task.ReactAndFlee(Protest5);
                    Protest8.Task.ReactAndFlee(Protest5);
                    Protest9.Task.ReactAndFlee(Protest5);
                    Protest10.Task.ReactAndFlee(Protest5);
                    Protest11.Task.ReactAndFlee(Protest5);
                    Protest12.Task.ReactAndFlee(Protest5);

                    BaseScript.Delay(2500);
                    Protest3.Task.FleeFrom(Protest2);
                }
            }
        }

    }
}