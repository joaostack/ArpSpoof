using System.Threading.Tasks;
using ArpSpoof.Commands;
using ArpSpoof.Core;
using SharpPcap;

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
            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            var device = DeviceHelper.SelectDevice();
            DeviceHelper.OpenDevice(device);

            var gatewayMac = await PacketBuild.GetMacAddress(device, gateway, ct);
            Console.WriteLine(gatewayMac.ToString());

            var cmd = new ArpSpoofCommands(target, gateway, gatewayMac.ToString());
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
