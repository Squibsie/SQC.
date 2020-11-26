using CitizenFX.Core;
using FivePD.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Dynamic;
using FivePD.API.Utils;

namespace SQC
{
    [CalloutProperties("[SQC] Robbery In Progress", "Squibsie", "1.0")]
    class RobberyAndSearch : FivePD.API.Callout
    {
        Random random = new Random();

        Ped suspect;
        Ped victim;

        PedHash suspectHash;
        String suspectDesc;

        bool suspectActive;

        Blip suspectBlip;

        Blip victimBlip;

        public RobberyAndSearch()
        {
            float x = random.Next(250, 800); //TODO: Change Dist!
            float y = random.Next(250, 800);
            InitInfo(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(x, y, 0f))));

            this.ShortName = "Robbery in Progress";
            this.CalloutDescription = "Informant is reporting that they have just been robbed, suspect is likely still in the area. Attend, investigate and locate the suspect!";
            this.ResponseCode = 3;

            /* How close the player needs to be to start the action (OnStart())*/
            this.StartDistance = 50f; // 30 feet? metres? unit...
        }

        public async override Task OnAccept()
        {
            this.InitBlip();
            victim = await SpawnPed(RandomUtils.GetRandomPed(), Location, 287.449f);
            suspect = await SpawnPed(SuspectSelector(), new Vector3(Location.X + 3f, Location.Y, Location.Z));
            suspect.AlwaysKeepTask = true;
            suspect.BlockPermanentEvents = true;

            TaskSequence tSeq = new TaskSequence();
            tSeq.AddTask.FleeFrom(victim, 10000);
            tSeq.AddTask.WanderAround();
            tSeq.Close();

            suspect.Task.PerformSequence(tSeq);

            PedData data = new PedData();
            Item Cards = new Item();
            Item Phone = new Item();

            Cards.Name = "Stolen Credit Cards";
            Cards.IsIllegal = true;

            Phone.Name = "Stolen Mobile Phone";
            Phone.IsIllegal = true;

            data.Items = new List<Item>();

            data.Items.Add(Phone);
            data.Items.Add(Cards);

            Utilities.SetPedData(suspect.NetworkId, data);

            suspectActive = true;

            base.Tick += IsPedRestrained;
        }


        public override void OnStart(Ped closest)
        {
            victimBlip = victim.AttachBlip();
            victimBlip.Name = "Robbery Victim";            
            Utilities.SyncBlip(victimBlip);

            victim.Task.ChatTo(closest);

            Locator();

            var data = victim.GetData();

            ShowNetworkedNotification("Speak to the victim and ask questions to establish suspect description.", "CHAR_CALL911", "CHAR_CALL911", "Control", "Robbery", 50f);
            ShowNetworkedNotification("Then coordinate area search to find them!", "CHAR_CALL911", "CHAR_CALL911", "Control", "Robbery", 50f);

            PedQuestion robberyQ1 = new PedQuestion();
            robberyQ1.Question = "What happened?";
            robberyQ1.Answers = new List<string>
                {
                    "This person grabbed me and took my phone and cards!"
                };

            PedQuestion robberyQ2 = new PedQuestion();
            robberyQ2.Question = "What did they look like?";
            robberyQ2.Answers = new List<string>
                {
                    suspectDesc
                };

            PedQuestion robberyQ3 = new PedQuestion();
            robberyQ3.Question = "Where did he go?";
            robberyQ3.Answers = new List<string>
                {
                    "I'm not sure, I was too panicked, he just ran off."
                };

            AddPedQuestion(victim, robberyQ1);
            AddPedQuestion(victim, robberyQ2);
            AddPedQuestion(victim, robberyQ3);

        }

        public PedHash SuspectSelector()
        {
            int dice = random.Next(0, 4);
            switch (dice)
            {
                case 1:
                    suspectHash = PedHash.Terry;
                    suspectDesc = "He's a white guy, blonde hair, wearing a leather biker style jacket.";  
                    break;
                case 2:
                    suspectHash = PedHash.TylerDixon;
                    suspectDesc = "He's a white guy, dark hair. He's shirtless with bluey purple shorts on."; 
                    break;
                case 3:
                    suspectHash = PedHash.Tonya;
                    suspectDesc = "Shes a female, wearing a lime coloured shirt and short shorts"; 
                    break;                
            }
            return suspectHash;
        }

        public async Task Locator()
        {
            var time = 5;
            while (time > 0)
            {
                await BaseScript.Delay(60000);
                time--;                
                CitizenFX.Core.Debug.WriteLine(time.ToString());
            }
            if (time <= 0 && suspectActive)
            {
                ShowNetworkedNotification("CCTV have located a person matching description, sending to your map!", "CHAR_CALL911", "CHAR_CALL911", "Control", "Recent Robbery", 50f);
                suspectBlip = suspect.AttachBlip();
                suspectBlip.Name = "Robbery Suspect";
                Utilities.SyncBlip(suspectBlip);
            }
        }

        public override void OnCancelAfter()
        {            
            if (suspectBlip != null)
            {
                Utilities.SyncBlipDelete(suspectBlip);
            }
            if (victimBlip != null)
            {
                Utilities.SyncBlipDelete(victimBlip);
            }

        }

        public async Task IsPedRestrained()
        {
            if (suspect.IsCuffed)
            {
                suspectActive = false;
            }
        }
    }
}
