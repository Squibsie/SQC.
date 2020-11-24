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


        Vector3[] medicCoords = new Vector3[7]
        {
            new Vector3(-1125.017f, -967.905f, 6.632f),
            new Vector3(-1067.747f, -1047.33f, 6.077f),
            new Vector3(-1132.347f, -1074.691f, 2.181f),
            new Vector3(-836.259f, -1204.975f, 6.684f),
            new Vector3(-860.406f, -1143.95f, 6.991f),
            new Vector3(-664.673f, -759.4f, 35.576f),
            new Vector3(-678.294f, -766.228f, 34.525f)
        };

        float[] medicHeading = new float[7]
        {
            221.1f,
            284.22f,
            352.794f,
            192.624f,
            45.509f,
            244.798f,
            189.059f
        };

        Vector3[] patientCoords = new Vector3[7]
        {
            new Vector3(-1117.839f, -974.057f, 6.627f),
            new Vector3(-1060.887f, -1062.503f, 6.412f),
            new Vector3(-1124.67f, -1086.396f, 3.15f),
            new Vector3(-834.897f, -1212.814f, 6.912f),
            new Vector3(-866.755f, -1137.96f, 6.992f),
            new Vector3(656.472f, -760.723f, 37.347f),
            new Vector3(-676.297f, -775.011f, 33.009f)
        };

        float[] patientHeading = new float[7]
        {
            45.404f,
            10.275f,
            125.363f,
            58.079f,
            264.612f,
            89.495f,
            189.059f
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
            this.CalloutDescription = "Call from EMS control, a mental-health patient has escaped from a nearby community facility and the guard is requesting assistance, as the patient is now non-compliant.";
            this.ResponseCode = 3;

            /* How close the player needs to be to start the action (OnStart())*/
            this.StartDistance = 15f; // 30 feet? metres? unit...

            base.Tick += IsPedRestrained;
        }

        public async override Task OnAccept()
        {
            this.InitBlip();

            patient = await SpawnPed(PedHash.Beach02AMM, patientCoords[jobSelect], patientHeading[jobSelect]);
            medic = await SpawnPed(PedHash.Security01SMM, medicCoords[jobSelect], medicHeading[jobSelect]);
            this.medic.Task.GuardCurrentPosition();
            this.patient.Ragdoll();
            
        }

        public override void OnStart(Ped closest)
        {


            base.OnStart(closest);

            patientActive = true;

            var interaction = StartInteraction(closest, medic, patient);

        }



        public async Task StartInteraction(Ped closest, Ped staff, Ped suspect)
        {

            this.medic.Task.ChatTo(closest);
            ShowDialog("[Guard] Thanks for coming, the patient is just over there. They managed to break out the facility!", 3000, 20f);
            await BaseScript.Delay(3500);
            this.patient.Task.ChatTo(closest);
            ShowDialog("[Patient] *Sobbing*", 3000, 20f);
            await BaseScript.Delay(4000);
            ShowDialog("[You] Sir? Do you want to come out for me?", 3000, 20f);
            await BaseScript.Delay(1500);

            int dice = random.Next(0, 100);
            if (dice <= 20)
            { 

                ShowDialog("[Patient] YOU CAN'T CONTAIN ME!", 3000, 30f);
                patient.Task.FightAgainst(closest);
                
            }
            else if (dice > 20 && dice <= 50)
            {
                ShowDialog("[Patient] FIRE MAKES ALL CLEAN AGAIN", 3000, 30f);
                patient.Euphoria.Teeter.Start();
                await BaseScript.Delay(500);
                dice = random.Next(3000, 15000);
                _ = this.outcomeController(dice);
                ShowDialog("[Guard] Fuck! He's covered in fuel!.", 3000, 20f);
                ShowNetworkedNotification("Restrain the suspect before they set light to themselves!", "CHAR_HUMANDEFAULT", "CHAR_HUMANDEFAULT", "Help", " ", 20f);
            }
            else
            {
                ShowDialog("[Patient] GOTTA GO FAST!", 3000, 20f);
                patient.Task.FleeFrom(closest);
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
                CitizenFX.Core.World.AddExplosion(patient.Position, ExplosionType.Molotov1, 5f, 0f, null, false);
                this.patient.CancelRagdoll();
            }

        }

        public async Task IsPedRestrained()
        {
            if (patient.IsCuffed)
            {
                patientActive = false;
            }
        }
    }
}