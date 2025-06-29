using System.Threading.Tasks;
using ArpSpoof.Commands;
using ArpSpoof.Core;

class Program
{
    static string ASCIIART = @"
   ___   ___  ___  ____  ____________  _  __
  / _ | / _ \/ _ \/ __ \/  _/ __/ __ \/ |/ /
 / __ |/ , _/ ___/ /_/ // /_\ \/ /_/ /    / 
/_/ |_/_/|_/_/   \____/___/___/\____/_/|_/
By github.com/joaostack
";

    /// <summary>
    /// ArpSpoof Program
    /// </summary>
    /// <param name="target"></param>
    /// <param name="gateway"></param>
    static async Task Main(string target, string gateway)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(ASCIIART);
        Console.ResetColor();

        if (string.IsNullOrEmpty(gateway) || string.IsNullOrEmpty(target))
        {
            Console.WriteLine("-?, -h, --help\tShow help and usage information");
            return;
        }

        try
        {
            var CancellationTokenSource = new CancellationTokenSource();
            var ct = CancellationTokenSource.Token;

            var device = DeviceHelper.SelectDevice();
            var gatewayMac = PacketBuild.GetMacAddress(device, gateway, ct);
            Console.WriteLine(gatewayMac.ToString());

            var cmd = new ArpSpoofCommands(target, gateway, gatewayMac.ToString());
            await cmd.Execute();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Console.ResetColor();
        }
    }
}
