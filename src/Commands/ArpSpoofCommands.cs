using System.Net;

namespace ArpSpoof.Commands;

public class ArpSpoofCommands
{
    private readonly string _targetIp;
    private readonly string _targetGateway;
    private readonly string _targetGatewayMac;

    public ArpSpoofCommands(string targetIp, string targetGateway, string targetGatewayMac)
    {
        _targetIp = targetIp;
        _targetGateway = targetGateway;
        _targetGatewayMac = targetGatewayMac;
    }

    public async Task Execute()
    {
        throw new NotImplementedException();
    }
}