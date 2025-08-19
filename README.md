# ArpPoison

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

## Demo 1
<img width="481" height="315" alt="Screenshot_20250819_134945" src="https://github.com/user-attachments/assets/b32f64bf-9b21-4e4e-9616-9b6474bcd5bc" />

## Demo 2
<img width="628" height="205" alt="Screenshot_20250819_134933" src="https://github.com/user-attachments/assets/7e19f7c9-ab6d-44b3-a97f-41093420fc67" />

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

<b>João H.</b> (joaostack) – [GitHub](https://github.com/joaostack)

