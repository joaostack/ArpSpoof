using System;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System.Net;
using System.Net.NetworkInformation;
using System.Diagnostics;

class Program
{
    static string targetMac;
    static IPAddress gatewayIp;
    static PhysicalAddress gatewayMac;
    static void Main()
    {
        Console.Clear();
        var devices = CaptureDeviceList.Instance;
        if (devices.Count < 1)
        {
            Console.WriteLine("No iface found!");
            return;
        }

        for (int i = 0; i < devices.Count; i++)
        {
            var dev = devices[i];
            Console.WriteLine("{0}: Name: {1} | Desc: {2} | Mac: {3}", i, dev.Name, dev.Description, dev.MacAddress);
        }

        Console.WriteLine("Select an interface by entering the corresponding number:");

        int selectedIndex;
        while (!int.TryParse(Console.ReadLine(), out selectedIndex) || selectedIndex < 0 || selectedIndex >= devices.Count)
        {
            Console.WriteLine("Invalid selection. Please enter a valid number.");
        }

        var device = devices[selectedIndex];
        Console.WriteLine("Using: {0}", device.Name);
        device.Open(DeviceModes.Promiscuous);

        Console.Write("Enter the target IP address: ");
        var targetIp = IPAddress.Parse(Console.ReadLine());

        Console.Write("Enter the target Gateway IP address: ");
        gatewayIp = IPAddress.Parse(Console.ReadLine());

        // Get MAC from IP ADDRESS

        gatewayMac = GetMacFromIp(device, gatewayIp);
        Console.WriteLine("Gateway MAC: {0}", gatewayMac);
    }

    static PhysicalAddress GetMacFromIp(ILiveDevice device, IPAddress ip)
    {
        var broadcastMac = PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF");

        var stopwatch = new Stopwatch();
        var arpRequest = new ArpPacket(
            ArpOperation.Request,
            broadcastMac,
            ip,
            device.MacAddress,
            gatewayIp
        );

        var ethernetPacket = new EthernetPacket(device.MacAddress, broadcastMac, EthernetType.Arp);
        ethernetPacket.PayloadPacket = arpRequest;

        device.SendPacket(ethernetPacket);

        HashSet<string> seenPackets = new HashSet<string>();
        device.OnPacketArrival += (object s, PacketCapture e) =>
        {
            var packet = Packet.ParsePacket(e.GetPacket().LinkLayerType, e.GetPacket().Data);
            var arpPacket = packet.Extract<ArpPacket>();
            if (arpPacket != null)
            {
                var sourceIp = arpPacket.SenderProtocolAddress.ToString();
                targetMac = arpPacket.SenderHardwareAddress.ToString();
                var data = arpPacket.Operation;
                var key = $"{sourceIp} -> {targetMac}";

                if (!seenPackets.Contains(key))
                {
                    seenPackets.Add(key);
                }
            }
        };

        device.StartCapture();
        stopwatch.Restart();
        while (stopwatch.ElapsedMilliseconds < 2000)
        {
            // Wait
        }

        device.StopCapture();
        stopwatch.Stop();

        return string.IsNullOrEmpty(targetMac) ? null : PhysicalAddress.Parse(targetMac);
    }
}
