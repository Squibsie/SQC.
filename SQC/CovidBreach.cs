using CitizenFX.Core;
using FivePD.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Dynamic;
using FivePD.API.Utils;

namespace SQC
{
    [CalloutProperties("[SQC] Covid Breach", "Squibsie", "1.0")]
    class CovidBreach : FivePD.API.Callout
    {
        Ped CovidPatient;
        Ped Player;
        Random random = new Random();

        bool suspectActive;

        public CovidBreach()
        {
            float x = random.Next(100, 200);
            float y = random.Next(100, 200);
            InitInfo(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(x, y, 0f))));

            this.ShortName = "Covid Breach";
            this.CalloutDescription = "We've got a report of Covid positive patient allegedly disregarding the regulations. Attend and Investigate!";
            this.ResponseCode = 1;

            /* How close the player needs to be to start the action (OnStart())*/
            this.StartDistance = 20f; // 30 feet? metres? unit...
        }

        public async override Task OnAccept()
        {
            this.InitBlip();

            //Spawn the protestors
            CovidPatient = await SpawnPed(PedHash.Chef01SMY, Location, 287.449f);
            CovidPatient.Task.WanderAround();
        }

        public override void OnStart(Ped closest)
        {
            Player = closest;

            CovidPatient.AttachBlip();

            base.OnStart(closest);

            ShowNetworkedNotification("Speak to the subject and determine if there's been a breach", "CHAR_CALL911", "CHAR_CALL911", "Control", "Covid - Breach", 50f);
            ShowNetworkedNotification("If the breach is accidental, consider calling the subject a taxi.", "CHAR_CALL911", "CHAR_CALL911", "Control", "Covid - Breach", 50f);

            int dice = random.Next(0, 100);

            if (dice < 25)
            {
                //Outcome 1 - Incorrect Call
                CitizenFX.Core.Debug.WriteLine("Incorrect Call");
                PedQuestion covidQ1 = new PedQuestion();
                covidQ1.Question = "Have you tested positive for covid recently?";
                covidQ1.Answers = new List<string>
                {
                    "No, I've been lucky!",
                    "Of course not!",
                    "No, No, if I did, I'd be inside!"
                };

                PedQuestion covidQ2 = new PedQuestion();
                covidQ2.Question = "Should you be self-isolating for any reason?";
                covidQ2.Answers = new List<string>
                {
                    "No, not that I'm aware of!",
                    "Nope! Lucky really, plenty of work to get done.",
                    "I hope not! Don't think I'd survive that long with the kids!"
                };

                PedQuestion covidQ3 = new PedQuestion();
                covidQ3.Question = "Have you experienced any symptoms of covid recently? A cough? High Temperature?";
                covidQ3.Answers = new List<string>
                {
                    "No, I've not noticed  any.",
                    "I can't say I have officer.",
                    "Thankfully not!"
                };

                AddPedQuestion(CovidPatient, covidQ1);
                AddPedQuestion(CovidPatient, covidQ2);
                AddPedQuestion(CovidPatient, covidQ3);

            }
            else if (dice > 25 && dice < 50)
            {
                //Outcome 2 - Symptomatic.
                CitizenFX.Core.Debug.WriteLine("Symptomatic");

                PedQuestion covidQ1 = new PedQuestion();
                covidQ1.Question = "Have you tested positive for covid recently?";
                covidQ1.Answers = new List<string>
                {
                    "No, I've not had one!",
                    "Of course not!",
                    "Nope, I can't say I have"
                };

                PedQuestion covidQ2 = new PedQuestion();
                covidQ2.Question = "Should you be self-isolating for any reason?";
                covidQ2.Answers = new List<string>
                {
                    "Nobody's told me too!",
                    "Nope! Not that it wouldn't be a welcome break!",
                    "I hope not! Don't think I'd survive that long with the kids!"
                };

                PedQuestion covidQ3 = new PedQuestion();
                covidQ3.Question = "Have you experienced any symptoms of covid recently? A cough? High Temperature?";
                covidQ3.Answers = new List<string>
                {
                    "Now that you mention it... I have been feeling quite feverish.",
                    "Oh I've been having fits of coughs, but I think its my smokers cough playing up.",
                    "No, but I have been feeling hot and cold all morning."
                };

                AddPedQuestion(CovidPatient, covidQ1);
                AddPedQuestion(CovidPatient, covidQ2);
                AddPedQuestion(CovidPatient, covidQ3);

            }
            else if (dice > 50 && dice < 75)
            {
                //Outcome 3 - Breaching Isolation
                CitizenFX.Core.Debug.WriteLine("Breaching Isolation");

                PedQuestion covidQ1 = new PedQuestion();
                covidQ1.Question = "Have you tested positive for covid recently?";
                covidQ1.Answers = new List<string>
                {
                    "Er... no officer.",
                    "Pah, I didn't even read the results!",
                    "Yeah I just got a text to say I'm positive."
                };

                PedQuestion covidQ2 = new PedQuestion();
                covidQ2.Question = "Should you be self-isolating for any reason?";
                covidQ2.Answers = new List<string>
                {
                    "Yes, but no government can tell me what to do!",
                    "Well yeah, but I just popped out to get some groceries?",
                    "I'm just getting some fresh air, then I'll be straight back inside to isolate!"
                };

                PedQuestion covidQ3 = new PedQuestion();
                covidQ3.Question = "Have you experienced any symptoms of covid recently? A cough? High Temperature?";
                covidQ3.Answers = new List<string>
                {
                    "I've been coughing all week, thats why I went for a test.",
                    "Yeah, I've been feeling pretty awful to be quite honest.",
                    "I've had a tempeerature and a headache for a few days now."
                };

                AddPedQuestion(CovidPatient, covidQ1);
                AddPedQuestion(CovidPatient, covidQ2);
                AddPedQuestion(CovidPatient, covidQ3);

            }
            else if (dice > 75 && dice < 101)
            {
                //Outcome 4 - Covid Denier
                CitizenFX.Core.Debug.WriteLine("Covid Denier");

                Timer();
                suspectActive = true;
                Events.OnPedArrested += OnPedArrested;

                PedQuestion covidQ1 = new PedQuestion();
                covidQ1.Question = "Have you tested positive for covid recently?";
                covidQ1.Answers = new List<string>
                {
                    "Covid is a fraud!",
                    "I don't see how I can test positive for a fake virus officer",
                    "No, thee government are just making up thhe results anyway!"
                };

                PedQuestion covidQ2 = new PedQuestion();
                covidQ2.Question = "Should you be self-isolating for any reason?";
                covidQ2.Answers = new List<string>
                {
                    "You'd love that wouldn't you. You're just a pawn in the game of goverment control!",
                    "No government will tell me what's what. No.",
                    "Why should I self-isolate?!"
                };

                PedQuestion covidQ3 = new PedQuestion();
                covidQ3.Question = "Have you experienced any symptoms of covid recently? A cough? High Temperature?";
                covidQ3.Answers = new List<string>
                {
                    "The symptoms are the same as the common flu! I have the flu!",
                    "I've been coughing all week, but it ain't that fake shit covid!",
                    "I've had flu before, and I have the same now. Covid is a hoax."
                };

                AddPedQuestion(CovidPatient, covidQ1);
                AddPedQuestion(CovidPatient, covidQ2);
                AddPedQuestion(CovidPatient, covidQ3);
            }

        }

        private async Task Timer()
        {
            int timer = 10000;
            while (timer > 0)
            {
                await BaseScript.Delay(1000);
                timer -= 1000;
            }
            if (suspectActive && timer <= 0)
            {
                Impatience();
            }
        }

        public async Task OnPedArrested(Ped ped)
        {
            suspectActive = false;
        }

        public async Task Impatience()
        {
            ShowDialog("Are you quite finished? I'm not hanging about anymore!", 2500, 15f);
            await BaseScript.Delay(500);
            CovidPatient.Task.FleeFrom(Player);
        }

    }
}