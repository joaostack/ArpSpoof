
# ArpSpoof

A simple ARP spoofing tool written in C# for Windows/Linux environments.  
Designed for ethical hacking and penetration testing purposes.

---

## Features

- List all network interfaces and select one to attack
- Discover gateway MAC address automatically
- Attempt to retrieve target MAC address by IP
- Simple console interface with clear messages
- Written in C#

---

## Installation

1. Clone the repository:

```bash
git clone https://github.com/joaostack/ArpSpoof.git
cd ArpSpoof
```

2. Restore dependencies:

```bash
dotnet restore
```

---

## Usage

Run the project with `sudo` (required for raw packet sending):

```bash
sudo dotnet run --project src -- --target-address <TARGET_IP> --gateway-address <GATEWAY_IP>
```

Example:

```bash
sudo dotnet run --project src -- --target-address 192.168.2.221 --gateway-address 192.168.2.1
```

You will be prompted to select a network interface.  
The program will attempt to discover MAC addresses and start spoofing.

---

## Dependencies

- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
- [SharpPcap](https://github.com/chmorgan/sharppcap)
- [System.CommandLine](https://github.com/dotnet/command-line-api)

---

## Contributing

Feel free to open issues, submit bug reports, or suggest improvements.  
Please respect ethical hacking principles and only test in environments you own or have permission to test.

---

## Author

João H. (joaostack) – [GitHub](https://github.com/joaostack)
