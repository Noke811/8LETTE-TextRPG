namespace _8LETTE_TextRPG
{
    internal class MainGame
    {

        static void Main(string[] args)
        {
            List<Job> jobs = Job.GetJobs(); // �������� �߰�
            Job selectedJob = jobs[0]; // ���� ���� �߰�

            Player player = new Player("chad", jobs[0]); // �÷��̾� �����߰�

            StatusScreen.instance.Player = player; // ���º��� ȭ�鿡 �÷��̾� ���� ���� �߰�

            Monster infLoopMonster = new InfLoop();

            Screen? current = TownScreen.instance;


            while (current != null)
            {
                current.Show();
                current = current.Next();
            }
        }
    }
}
