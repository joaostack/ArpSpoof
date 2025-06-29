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
    static void Main(string target, string gateway)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(ASCIIART);
        Console.ResetColor();

        if (string.IsNullOrEmpty(gateway) || string.IsNullOrEmpty(target))
        {
            Console.WriteLine("-?, -h, --help\tShow help and usage information");
            return;
        }

        var CancellationTokenSource = new CancellationTokenSource();
        var ct = CancellationTokenSource.Token;

        var device = DeviceHelper.SelectDevice();
        var gatewayMac = PacketBuild.GetMacAddress(device, gateway, ct);
        Console.WriteLine(gatewayMac);

        var cmd = new ArpSpoofCommands(target, gateway, gatewayMac.ToString());
        cmd.Execute();
    }
}
