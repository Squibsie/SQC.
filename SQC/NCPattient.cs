using CitizenFX.Core;
using FivePD.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Dynamic;
using FivePD.API.Utils;

namespace SQC
{
    [CalloutProperties("[SQC]Non-Compliant Patient", "Squibsie", "1.0")]
    class NCPatient : FivePD.API.Callout
    {
        Random random = new Random();

        int jobSelect;

        Vector3[] amboCoords = new Vector3[1]
        {
            new Vector3(-1109.777f, -951.523f, 1.996f)
        };

        float[] amboHeading = new float[1]
        {
            212.378f
        };

        Vector3[] medicCoords = new Vector3[1]
        {
            new Vector3(-1125.017f, -967.905f, 6.632f)
        };

        float[] medicHeading = new float[1]
        {
            221.1f
        };

        Vector3[] patientCoords = new Vector3[1]
        {
            new Vector3(-1117.839f, -974.057f, 6.627f)
        };

        float[] patientHeading = new float[1]
        {
            45.404f
        };

        Ped patient;
        Ped medic;

        int incidentTimer;

        bool patientActive;

        public NCPatient()
        {
            jobSelect = random.Next(0, 1);

            this.InitInfo(patientCoords[jobSelect]);

            this.ShortName = "Non-Compliant MH Patient";
            this.CalloutDescription = "Call from EMS control, a mental-health patient has escaped during transit and the guard is requesting assistance, as the patient is now non-compliant.";
            this.ResponseCode = 3;

            /* How close the player needs to be to start the action (OnStart())*/
            this.StartDistance = 15f; // 30 feet? metres? unit...

            Events.OnPedArrested += OnPedArrested;
        }

        public async override Task OnAccept()
        {
            this.InitBlip();

            patient = await SpawnPed(PedHash.Beach02AMM, patientCoords[jobSelect], patientHeading[jobSelect]);
            medic = await SpawnPed(PedHash.PrologueSec01, medicCoords[jobSelect], medicHeading[jobSelect]);
            this.medic.Task.GuardCurrentPosition();
            this.patient.Euphoria.BodyFoetal.Start();
            
        }

        public override void OnStart(Ped closest)
        {


            base.OnStart(closest);

            patientActive = true;

            _ = this.StartInteraction(closest, medic, patient);

        }



        public async Task StartInteraction(Ped closest, Ped staff, Ped suspect)
        {

            this.medic.Task.ChatTo(closest);
            ShowDialog("[Guard] Thanks for coming, the patient is just over there. They managed to break out the ambulance!", 3000, 20f);
            await BaseScript.Delay(3500);
            this.patient.Task.ChatTo(closest);
            ShowDialog("[Patient] *Sobbing*", 3000, 20f);
            await BaseScript.Delay(4000);
            ShowDialog("[You] Sir? Do you want to come out for me?", 3000, 20f);
            await BaseScript.Delay(1500);

            int dice = random.Next(0, 100);
            if (dice <= 20)
            {
                TaskSequence sequence = new TaskSequence();

                ShowDialog("[Patient] YOU CAN'T CONTAIN ME!", 3000, 30f);
                sequence.AddTask.FightAgainst(closest);
                sequence.Close();

                patient.Task.PerformSequence(sequence);
            }
            else if (dice > 20 && dice <= 75)
            {
                patient.Euphoria.StopAllBehaviours.Start();
                ShowDialog("[Patient] FIRE MAKES ALL CLEAN AGAIN", 3000, 30f);
                patient.Euphoria.Teeter.Start();
                await BaseScript.Delay(500);
                dice = random.Next(2000, 10000);
                _ = this.outcomeController(dice);
                ShowDialog("[Guard] Fuck! He's covered in fuel!.", 3000, 20f);
                ShowNetworkedNotification("Restrain the suspect before they set light to themselves!", "CHAR_HUMANDEFAULT", "CHAR_HUMANDEFAULT", "Help", " ", 20f);
            }
            else
            {
                ShowDialog("[Patient] GOTTA GO FAST!", 3000, 20f);
                patient.Task.FleeFrom(closest);
                patient.AttachBlip();
                await BaseScript.Delay(5000);
                patient.Task.Cower(30000);
            }
        }

        public async Task outcomeController(int time)
        {
            incidentTimer = time;
            while (incidentTimer > 0)
            {
                await BaseScript.Delay(1000);
                incidentTimer -= 1000;
            }
            if (patientActive)
            {
                CitizenFX.Core.World.AddExplosion(patient.Position, ExplosionType.Molotov1, 5f, 0f);                
            }

        }

        public async Task OnPedArrested(Ped ped)
        {
            if (ped.NetworkId == patient.NetworkId)
            {
                patientActive = false;
                ShowNetworkedNotification("Nice Work, Code 4 or transport as normal.", "CHAR_HUMANDEFAULT", "CHAR_HUMANDEFAULT", "Help", " ", 20f);
            }
        }
    }
}