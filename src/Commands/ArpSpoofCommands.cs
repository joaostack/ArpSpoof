using System.Net;
using ArpSpoof.Core;
using SharpPcap;
using System.Net.NetworkInformation;

namespace ArpSpoof.Commands;

public class ArpSpoofCommands
{
    private ILiveDevice _device;
    private IPAddress _targetIp;
    private PhysicalAddress _targetMac;
    private IPAddress _targetGateway;
    private PhysicalAddress _targetGatewayMac;

    public ArpSpoofCommands(ILiveDevice device, IPAddress targetIp, PhysicalAddress targetMac, IPAddress targetGateway, PhysicalAddress targetGatewayMac)
    {
        _device = device;
        _targetIp = targetIp;
        _targetMac = targetMac;
        _targetGateway = targetGateway;
        _targetGatewayMac = targetGatewayMac;
    }

    public void Execute()
    {
        PacketBuild.Spoof(_device, _targetIp, _targetMac, _targetGateway, _targetGatewayMac);
    }
}
