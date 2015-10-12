using FinApp.MiddleWare;
using System.Threading.Tasks;

namespace YqlDataDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Parallel.Invoke(() => GetSectors());
            }
        }

        static void GetSectors()
        {
            YqlService service = new YqlService();
        }

    }
}
