namespace _8LETTE_TextRPG
{
    internal class MainGame
    {
        static void Main(string[] args)
        {
            List<Job> jobs = Job.GetJobs(); //�������� �߰�
            Job selectedJob = jobs[0];      //�������� �߰�

            Player player = new Player("chad", jobs[0]);

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
