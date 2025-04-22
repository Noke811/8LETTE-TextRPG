namespace _8LETTE_TextRPG
{
    internal class MainGame
    {
        static void Main(string[] args)
        {
            //�̸� �Է�
            Console.WriteLine("�̸��� �Է����ּ���.");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("\n>> ");
            Console.ResetColor();
            string userName = Console.ReadLine() ?? "8LETTE";
            userName = userName == "" ? "8LETTE" : userName;

            //���� ����
            List<Job> jobs = Job.GetJobs();
            Job selectedJob;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("������ �������ּ���.");
                for (int i = 0; i < jobs.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {jobs[i].Name}");
                }
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("\n>> ");
                Console.ResetColor();

                if (int.TryParse(Console.ReadLine(), out int num) &&
                    1 <= num && num <= jobs.Count)
                {
                    selectedJob = jobs[num - 1];
                    break;
                }
            }

            Player player = new Player(userName, selectedJob);

            Screen? current = TownScreen.instance;
            while (current != null)
            {
                current.player = player;
                current.Show();
                current = current.Next();
            }
        }
    }
}
