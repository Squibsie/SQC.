using CitizenFX.Core;
using FivePD.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Dynamic;
using FivePD.API.Utils;

namespace Port_Authority_Callouts
{
    [CalloutProperties("[SQC]Fare Evader", "Squibsie", "1.0")]
    class FareEvader : FivePD.API.Callout
    {
        new Random random = new Random();

        int jobSelect;

        new Vector3[] staffCoords = new Vector3[3]
        {
            new Vector3(-842.679f, -126.332f, 28.185f),
            new Vector3(258.7f, -1204.958f, 29.289f),
            new Vector3(-1022.83f, -2751.468f, 0.8f)
        };

        new float[] staffHeading = new float[3]
        {
            121.194f,
            316.902f,
            139.17f
        };

        new Vector3[] suspectCoords = new Vector3[3]
        {
            new Vector3(-842.98f, -125.074f, 28.185f),
            new Vector3(260.882f, -1204.176f, 29.289f),
            new Vector3(-1024.422f, -2752.422f, 0.8f)
        };

        new float[] suspectHeading = new float[3]
        {
            214.439f,
            85.063f,
            251.217f
        };

        Ped suspect;
        Ped staff;

        public FareEvader()
        {
            jobSelect = random.Next(0, 3);

            this.InitInfo(staffCoords[jobSelect]);

            this.ShortName = "Fare Evader";
            this.CalloutDescription = "Transit security have detained a person who has evaded paying the fare for transit, respond code 2 and investigate. \n Can't start the call? The call might be underground or above you in the station!";
            this.ResponseCode = 2;

            /* How close the player needs to be to start the action (OnStart())*/
            this.StartDistance = 5f; // 30 feet? metres? unit...
        }

        public async override Task OnAccept()
        {
            this.InitBlip();

            suspect = await SpawnPed(PedHash.Dockwork01SMM, suspectCoords[jobSelect], suspectHeading[jobSelect]);
            staff = await SpawnPed(PedHash.Security01SMM, staffCoords[jobSelect], staffHeading[jobSelect]);
            this.staff.Task.ChatTo(suspect);
        }

        public override void OnStart(Ped closest)
        {


            base.OnStart(closest);

            _ = this.StartInteraction(closest, staff, suspect);

        }

        public async Task StartInteraction(Ped closest, Ped staff, Ped suspect)
        {

            this.staff.Task.ChatTo(closest);
            ShowDialog("[Transit Guard] Oh hello officer, I've stopped this guy for fare evading.", 3000, 20f);
            this.staff.Task.LookAt(suspect, 1000);
            await BaseScript.Delay(4000);
            this.suspect.Task.ChatTo(closest);
            ShowDialog("[Suspect] I've done nothing wrong, I have a ticket!", 3000, 20f);
            await BaseScript.Delay(4000);
            ShowDialog("[You] Ok, not to worry. Do you have a valid ticket?", 3000, 20f);
            await BaseScript.Delay(1500);

            int dice = random.Next(0, 100);
            if (dice <= 20)
            {
                TaskSequence sequence = new TaskSequence();

                ShowDialog("[Suspect] Get Fucked!", 3000, 30f);
                sequence.AddTask.FightAgainst(staff, 5000);
                sequence.AddTask.FleeFrom(closest);
                sequence.Close();

                suspect.Task.PerformSequence(sequence);
                suspect.AttachBlip();
            }
            else if (dice > 20 && dice <= 75)
            {
                PedData data = new PedData();
                Item Tix = new Item();

                dice = random.Next(0, 100);
                if (dice <= 50)
                {
                    Tix.Name = "Fraudulent Metro Ticket";
                    Tix.IsIllegal = true; 
                }
                else
                {
                    Tix.Name = "Valid Metro Ticket";
                    Tix.IsIllegal = false;
                }
                data.Items = new List<Item>();
                data.Items.Add(Tix);
                Utilities.SetPedData(suspect.NetworkId, data);

                await BaseScript.Delay(1500);
                ShowDialog("[Suspect] Fucking hell! Search me then!", 3000, 20f);
                suspect.Task.HandsUp(-1);
            }
            else
            {
                await BaseScript.Delay(1500);
                ShowDialog("[Suspect] Certainly officer, I just didn't like this guard's attitude.", 3000, 20f);
                ShowNetworkedNotification("The suspect shows a valid ticket to ride.", "CHAR_HUMANDEFAULT", "CHAR_HUMANDEFAULT", "Ticket Evader", " ", 20f);
            }
        }
    }
}