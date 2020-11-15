using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using FivePD.API;
using System;
using System.Threading.Tasks;


namespace Port_Authority_Callouts
{
    [CalloutProperties("[SQC] Drunk Transit Passenger", "Squibsie", "1.0")]
    public class DrunkSubway : FivePD.API.Callout
    {
        Ped DrunkSuspect;

        Random random = new Random();

        int jobSelect;

        public Vector3[] jobCoords = new Vector3[24]
        {
            new Vector3(117.993f,-1729.749f,30.111f),
            new Vector3(-207.209f,-1014.799f,30.138f),
            new Vector3(-498.847f,-669.211f,11.809f),
            new Vector3(-465.103f,-702.633f,20.032f),
            new Vector3(-1338.35f,-487.778f,15.045f),
            new Vector3(-1371.673f,-469.867f,23.27f),
            new Vector3(-1342.293f,-510.099f,23.269f),
            new Vector3(-815.078f,-133.64f,19.95f),
            new Vector3(-813.423f,-149.21f,28.175f),
            new Vector3(-843.628f,-126.72f,28.185f),
            new Vector3(262.132f,-1210.41f,29.338f),
            new Vector3(-548.34f,-1300.656f,26.902f),
            new Vector3(-556.505f,-1351.31f,24.488f),
            new Vector3(-888.622f,-2334.129f,-11.733f),
            new Vector3(-909.493f,-2325.078f,-3.508f),
            new Vector3(-935.399f,-2333.784f,6.763f),
            new Vector3(-908.548f,-2336.269f,6.709f),
            new Vector3(-1082.5f,-2710.69f,-7.41f),
            new Vector3(-1042.169f,-2740.234f,13.902f),
            new Vector3(-292.555f,-327.964f,10.063f),
            new Vector3(-248.041f,-312.447f,21.646f),
            new Vector3(-249.617f,-319.479f,30.03f),
            new Vector3(453.254f,-611.222f,28.575f),
            new Vector3(433.321f,-654.101f,28.756f)
        };

        public DrunkSubway()
        {
            jobSelect = random.Next(0, 24);

            this.InitInfo(jobCoords[jobSelect]);

            this.ShortName = "Aggressive Drunk Person on Transit Property";
            this.CalloutDescription = "Transit staff are reporting a drunk person who is being aggressive towards staff and passengers, respond code 3. \n Can't start the call? The call might be underground or above you in the station!";
            this.ResponseCode = 3;

            /* How close the player needs to be to start the action (OnStart())*/
            this.StartDistance = 20f; // 30 feet? metres? unit...

        }

        public async override Task OnAccept()
        {
            this.InitBlip();

            DrunkSuspect = await SpawnPed(PedHash.Salton02AMM, this.Location, 12);
            this.DrunkSuspect.Task.WanderAround(this.Location, 15f);
        }

        public override void OnStart(Ped player)
        {
            base.OnStart(player);
            DrunkSuspect.AttachBlip();

            int dice = random.Next(0, 100);

            if (dice < 25)
            {
                ShowDialog("[Suspect] What the fuck are you doing here!", 5000, 30f);
                this.Attack(player);
            }
            else if (dice > 25 && dice < 75)
            {
                ShowDialog("[Suspect] Oh fuck, who called the cops?!", 5000, 30f);
                this.Flee(player);
            }
            else
            {
                ShowDialog("[Suspect] Fucks sake, I'm sorry officer.", 5000, 30f);
                this.Surrender();
            }
        }
        public async void Flee(Ped player)
        {
            this.DrunkSuspect.Task.FleeFrom(player);

            await BaseScript.Delay(random.Next(5500, 7500));
            int x = random.Next(1, 100 + 1);

            TaskSequence sequence = new TaskSequence();
            bool changedTask = false;
            if (x <= 30)
            {
                /* 30 % to attack the closest ped */
                sequence.AddTask.FightAgainst(GetClosestPed(this.DrunkSuspect));
                ShowDialog("[Suspect] Get out of my fucking way!", 5000, 30f);
                sequence.AddTask.FleeFrom(player);
                changedTask = true;
            }
            else if (x > 30 && x < 50)
            {
                /* 20% to attack the player */
                sequence.AddTask.FightAgainst(player);
                ShowDialog("[Suspect] Why the fuck are you chasing me cop?!", 5000, 30f);
                sequence.AddTask.FleeFrom(player);
                changedTask = true;
            }
            sequence.Close();

            if (changedTask)
            {

                ClearPedTasks(this.DrunkSuspect.Handle);
                ClearPedTasksImmediately(this.DrunkSuspect.Handle);

                this.DrunkSuspect.Task.PerformSequence(sequence);
            }
            Tick += RandomBehaviour;
        }
        public async Task RandomBehaviour()
        {
            await BaseScript.Delay(random.Next(4000, 6500));

            int x = random.Next(1, 100 + 1);
            if (x <= 25)
            {
                ClearPedTasks(this.DrunkSuspect.Handle);
                ClearPedTasksImmediately(this.DrunkSuspect.Handle);

                this.DrunkSuspect.Task.FightAgainst(GetClosestPed(DrunkSuspect));
            }
            else if (x > 25 && x <= 40)
            {
                ClearPedTasks(this.DrunkSuspect.Handle);
                ClearPedTasksImmediately(this.DrunkSuspect.Handle);

                this.DrunkSuspect.Task.ReactAndFlee(GetClosestPed(DrunkSuspect));
            }
        }

        private Ped GetClosestPed(Ped p)
        {
            Ped[] all = World.GetAllPeds();
            if (all.Length == 0)
                return null;
            float closest = float.MaxValue;
            Ped close = null;
            foreach (Ped ped in all)
            {
                if (Game.PlayerPed == ped)
                {
                    continue;
                }
                float distance = World.GetDistance(ped.Position, p.Position);
                if (distance < closest)
                {
                    close = ped;
                    closest = distance;
                }
            };
            return close;
        }

        public void Attack(Ped player)
        {
            TaskSequence sequence = new TaskSequence();

            sequence.AddTask.FightAgainst(player);
            sequence.AddTask.FleeFrom(player);

            sequence.Close();
            DrunkSuspect.Task.PerformSequence(sequence);
        }
        public void Surrender()
        {
            DrunkSuspect.Task.HandsUp(-1);
        }
    }
}