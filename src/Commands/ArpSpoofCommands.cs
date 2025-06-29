public class ArpSpoofCommands
{
    private readonly string _targetIp;
    private readonly string _targetGateway;

    public ArpSpoofCommands(string targetIp, string targetGateway)
    {
        _targetIp = targetIp;
        _targetGateway = targetGateway;
    }

    public async Task Execute()
    {
        throw new NotImplementedException();
    }
}