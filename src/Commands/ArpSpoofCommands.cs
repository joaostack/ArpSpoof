using System.Net;
using ArpSpoof.Core;
using SharpPcap;
using System.Net.NetworkInformation;

namespace ArpSpoof.Commands;

/// <summary>
/// Provides methods for executing ARP spoofing commands.
/// </summary>
public class ArpSpoofCommands
{
    private ILiveDevice _device;
    private IPAddress _targetIp;
    private PhysicalAddress _targetMac;
    private IPAddress _targetGateway;
    private PhysicalAddress _targetGatewayMac;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArpSpoofCommands"/> class.
    /// </summary>
    /// <param name="device">The live device to use for sending packets.</param>
    /// <param name="targetIp">The IP address of the target device.</param>
    /// <param name="targetMac">The MAC address of the target device.</param>
    /// <param name="targetGateway">The IP address of the target gateway.</param>
    /// <param name="targetGatewayMac">The MAC address of the target gateway.</param>
    public ArpSpoofCommands(ILiveDevice device, IPAddress targetIp, PhysicalAddress targetMac, IPAddress targetGateway, PhysicalAddress targetGatewayMac)
    {
        _device = device;
        _targetIp = targetIp;
        _targetMac = targetMac;
        _targetGateway = targetGateway;
        _targetGatewayMac = targetGatewayMac;
    }

    /// <summary>
    /// Executes the ARP spoofing command.
    /// </summary>
    public async Task ExecuteAsync(CancellationToken ct)
    {
        try
        {
            Console.WriteLine("[+] ArpSpoof started, press CTRL+C to cancel.");

            while (!ct.IsCancellationRequested)
            {
                PacketBuild.Spoof(_device, _targetIp, _targetMac, _targetGateway, _targetGatewayMac);
                await Task.Delay(2000, ct);
            }
        }
        catch (TaskCanceledException)
        {
            _device.Close();
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
