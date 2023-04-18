namespace Dining_Philsophers
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //lavet er obj af klassen Fork
            Fork fork = new Fork();

            //forløkke der laver 5 filosoffer af Philopher klassen
            //i'et bruges til id til filosofferne og tænke- og spisetid sættes til random nr, der skal bruges i thread.sleep
            for (int i = 0; i < 5; i++)
            {
                int pId = i;
                int thinkTime = new Random().Next(1000, 3000);
                int eatTime = new Random().Next(1000, 3000);
                Philopher philopher = new Philopher(pId, thinkTime, eatTime, fork);
            }
        }

        class Philopher
        {
            int pId = 0;
            int thinkTime;
            int eatTime;
            int right;
            int left;
            Fork fork;

            public Philopher(int pId, int thinkTime, int eatTime, Fork fork)
            {
                this.pId = pId;
                this.thinkTime = thinkTime;
                this.eatTime = eatTime;
                this.fork = fork;

                //filosoffens højre gaffels findes på index lig med filosoffens id: hvis id er lig med 0 skal filosoffen tage gaflen på index 4, altså den 5. gaffel
                //ellers er filosoffens id minus 1
                right = pId == 0 ? 4 : pId - 1;

                //filosoffens venstre gaffels findes på det index, der har filosoffens id +1 (her er der nød til at plusses med 1 da der ikke kan divideres med nul)
                // modulus 5, en gang for hver filosof, for at sikre der alle har adgang til en gaffel til venstre
                left = (pId + 1) % 5;

                //der laves nu ny tråd, da denne nye tråd skabes inde i konstruktøren, oprettes der en ny tråd for hver obj der laves
                Thread p = new Thread(eat);
                p.Start();
            }

            public void eat()
            {
                //filosofferne får navne
                string phil = "";

                switch (pId)
                {
                    case 0:
                        phil = "Sokrates";
                        break;
                    case 1:
                        phil = "Platon";
                        break;
                    case 2:
                        phil = "Immanuel Kant";
                        break;
                    case 3:
                        phil = "Marcus Aurelius";
                        break;
                    case 4:
                        phil = "Søren Kirkegaard";
                        break;

                    default:
                        break;
                }

                while (true)
                {
                    try
                    {
                        //filosoffen starter med at tænke, da thinkTime er sat til et random tal mellem 1000 og 3000
                        //bliver der altså en tænkepause på mellem 1 og 3 sek før de "rækker ud efter gaflerne"
                        Console.WriteLine(phil + " is thinking.....");
                        Thread.Sleep(thinkTime);

                        //metoden GetFork klades
                        fork.GetFork(left, right);

                        //filosoffen spiser og eatTime er ligeledes sat til et random tal mellem 1000 og 3000
                        //derfor spises der i mellem 1 og 3 sek
                        Console.WriteLine(phil + " is eating.....");
                        Thread.Sleep(eatTime);

                        //metoden ReleaseFork klades
                        fork.ReleaseFork(left, right);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("error" + e);
                    }
                }
            }
        }

        class Fork
        {
            //lås til at spære objekterne inde
            object _lock = new object();

            //fem gafler i et bool array
            bool[] forks = new bool[5];

            public void GetFork(int left, int right)
            {
                //monitor bruges til synkroniseret adgang til objekter, når monitor bruges, opnås der adgang til flere funtioner såsom .Enter .Exit .PulseAll
                // .Enter der kræver en eksklusiv laf til et specifikt objekt
                Monitor.Enter(_lock);
                try
                {
                    //åbner låsen og venter på at objekterne bliver ledige
                    while (forks[left] || forks[right]) Monitor.Wait(_lock);
                    {
                        forks[left] = true;
                        forks[right] = true;
                    }
                }
                finally
                {
                    //låser objektet inde igen
                    Monitor.Exit(_lock);
                }
            }

            public void ReleaseFork(int left, int right)
            {
                Monitor.Enter(_lock);
                try
                {
                    forks[left] = false;
                    forks[right] = false;
                    //informere alle ventende tråde, om status ændring på objektet
                    Monitor.PulseAll(_lock);
                }
                finally
                {
                    Monitor.Exit(_lock);
                }
            }
        }
    }
}