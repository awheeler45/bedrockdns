using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BEDROCKDNS {
    class Program {
        static void Main(string[] args) {
            if (args.Length < 1) {
                Console.WriteLine("Example: dotnet run 192.168.1.14");
                return;
            }
            var redirectIp = args[0];
            if (!IPAddress.TryParse(redirectIp, out _)) {
                Console.WriteLine("Invalid IP address format.");
                return;
            }

            Console.WriteLine("Starting DNS server on port 53...");
            try {
                var listener = new UdpClient();
                listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                listener.Client.Bind(new IPEndPoint(IPAddress.Any, 53));

                var endpoint = new IPEndPoint(IPAddress.Any, 0);

                while (true) {
                    var request = listener.Receive(ref endpoint);
                    Console.WriteLine($"Received query from {endpoint}");

                    var response = CreateDnsResponse(request, redirectIp);
                    listener.Send(response, response.Length, endpoint);
                    Console.WriteLine($"Sent response to {endpoint}");
                }
            } catch (SocketException ex) {
                Console.WriteLine($"Error starting DNS server: {ex.Message}");
                if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse) {
                    Console.WriteLine("Port 53 is already in use. Ensure no other application is using it.");
                }
            }
        }

        static byte[] CreateDnsResponse(byte[] request, string redirectIp) {
            // Parse the domain
            var domain = ExtractDomainFromRequest(request);
            Console.WriteLine($"Query for: {domain}");

            switch (domain) {
                case "play.inpvp.net":
                case "mco.lbsg.net":
                case "play.enchanted.gg":
                case "mco.cubecraft.net":
                case "geo.hivebedrock.network":
                case "play.galaxite.net":
                    Console.WriteLine($"Redirecting {domain} to {redirectIp}");
                    return BuildDnsResponse(request, redirectIp);

                default:
                    // Forward unrecognized queries
                    Console.WriteLine($"Forwarding query for: {domain}");
                    return ForwardDnsQuery(request);
            }
        }

        static byte[] ForwardDnsQuery(byte[] request) {
            using (var udpClient = new UdpClient()) {
                udpClient.Connect("8.8.8.8", 53);
                udpClient.Send(request, request.Length);

                var remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
                var response = udpClient.Receive(ref remoteEndpoint);
                Console.WriteLine("Forwarded response received.");
                return response;
            }
        }

        static string ExtractDomainFromRequest(byte[] request) {
            var domainParts = new StringBuilder();
            int index = 12;
            while (request[index] != 0) {
                int length = request[index++];
                for (int i = 0; i < length; i++) {
                    domainParts.Append((char)request[index++]);
                }
                domainParts.Append('.');
            }
            return domainParts.ToString().TrimEnd('.');
        }

        static byte[] BuildDnsResponse(byte[] request, string ipAddress) {
            Console.WriteLine("Request data:");
            Console.WriteLine(BitConverter.ToString(request));

            int queryNameLength = 0;
            int index = 12;

            // Parse the query name length
            while (index < request.Length && request[index] != 0) {
                if (index + request[index] >= request.Length) {
                    throw new ArgumentException("Malformed DNS request: label exceeds packet size.");
                }
                queryNameLength += request[index] + 1;
                index += request[index] + 1;
            }
            queryNameLength++;

            // Validate queryNameLength
            if (12 + queryNameLength > request.Length) {
                throw new ArgumentException("Malformed DNS request: query name length exceeds packet size.");
            }

            // Determine query type (A or AAAA)
            int queryTypeOffset = 12 + queryNameLength;
            ushort queryType = (ushort)((request[queryTypeOffset] << 8) | request[queryTypeOffset + 1]);

            // Calculate response size: header (12) + question (queryNameLength + 4)
            int responseSize = 12 + queryNameLength + 4;

            if (queryType == 0x0001) {
                responseSize += 16;
            }

            var response = new byte[responseSize];

            // Copy the DNS header
            Buffer.BlockCopy(request, 0, response, 0, 12);

            // Set response flags: QR (response), AA (authoritative answer), RA (recursion available)
            response[2] = 0x81;
            response[3] = 0x80;

            // Set the question count
            response[4] = 0x00;
            response[5] = 0x01;

            if (queryType == 0x0001) {
                // Set the answer count
                response[6] = 0x00;
                response[7] = 0x01;
            } else {
                // No answer for unsupported query types
                response[6] = 0x00;
                response[7] = 0x00;
            }

            // Set the authority and additional counts to 0
            response[8] = 0x00;
            response[9] = 0x00;
            response[10] = 0x00;
            response[11] = 0x00;

            // Copy the question section exactly as is
            Buffer.BlockCopy(request, 12, response, 12, queryNameLength + 4);

            if (queryType == 0x0001) {
                // Answer section starts after the question
                int answerStart = 12 + queryNameLength + 4;

                response[answerStart++] = 0xC0;
                response[answerStart++] = 0x0C;
                response[answerStart++] = 0x00;
                response[answerStart++] = 0x01;
                response[answerStart++] = 0x00;
                response[answerStart++] = 0x01;
                response[answerStart++] = 0x00;
                response[answerStart++] = 0x00;
                response[answerStart++] = 0x0e;
                response[answerStart++] = 0x10;
                response[answerStart++] = 0x00;
                response[answerStart++] = 0x04;

                // Add the IP address in RDATA
                var ipBytes = IPAddress.Parse(ipAddress).GetAddressBytes();
                Buffer.BlockCopy(ipBytes, 0, response, answerStart, ipBytes.Length);
            }

            Console.WriteLine("Final Response data:");
            Console.WriteLine(BitConverter.ToString(response));

            return response;
        }
    }
}