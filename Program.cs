using OldMusicBox.ePUAP.Client.Model.Common;
using System.Security.Cryptography.X509Certificates;

namespace PZBridge
{
	internal class Program
	{
		public static void SendFault(string message)
		{
			Console.Write("{\"ok\": false, \"fault\": \"" + message.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n") + "\"}");
		}
		public static void SendFault(FaultModel fault)
		{
			string faultString = fault.FaultString.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
			string faultCode = fault.FaultCode.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
			string detail = fault.Detail?.Wyjatek?.Komunikat.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n") ?? "";
			Console.Write("{\"ok\": false, \"fault\": {\"faultString\": \"" + faultString + "\", \"faultCode\": \"" + faultCode + "\", \"detail\": \"" + detail + "\"}}");
		}

		public static string GetDocumentFromStandardInput()
		{
			string document = "";

			while (true)
			{
				string? data = Console.ReadLine();

				// FIXME: Maybe use a dynamic sentinel instead of the hard-coded "EOF"
				if (data == null || data == "EOF") break;

				if (data.StartsWith('='))
					data = data[1..];

				document += data;
				document += "\r\n";
			}

			return document;
		}

		public static string ReplaceStubPlaceholders(string input)
		{
			Random rand = new Random();

			// Generate random UTC date within the last 5 years
			DateTime start = DateTime.UtcNow.AddYears(-5);
			int range = (DateTime.UtcNow - start).Days;
			DateTime randomDate = start.AddDays(rand.Next(range))
									.AddHours(rand.Next(0, 24))
									.AddMinutes(rand.Next(0, 60))
									.AddSeconds(rand.Next(0, 60));

			string[] names = { "Alice", "Bob", "Charlie", "Diana", "Eve", "Frank" };
			string[] surnames = { "Smith", "Johnson", "Brown", "Taylor", "Anderson", "Clark" };

			string randomName = names[rand.Next(names.Length)];
			string randomSurname = surnames[rand.Next(surnames.Length)];

			string result = input
				.Replace("[date]", randomDate.ToString("u"))   // Universal sortable UTC format
				.Replace("[name]", randomName)
				.Replace("[surname]", randomSurname)
				.Replace("[pesel]", rand.Next(111111, 999999).ToString() + rand.Next(11111, 99999).ToString());

			return result;
		}

		static void SetupConsole(int bufferSize)
		{
			Stream inStream = Console.OpenStandardInput(bufferSize);
			Console.SetIn(new StreamReader(inStream, Console.InputEncoding, false, bufferSize));
		}

		static void Main(string[] args)
		{
			SetupConsole(1 * 1024 * 1024); // 1 MiB
			if (args.Length < 3)
			{
				SendFault("not enough arguments\nsyntax: PZBridge <certfile> <certpwd> <command> [args...]");
				return;
			}

			X509Certificate2 cert = new X509Certificate2(args[0], args[1]);

			try
			{
				switch (args[2])
				{
					case "sign":
						PZSign.Execute(cert, args[3..]);
						break;
					case "verify":
						PZVerify.Execute(cert, args[3..]);
						break;
					case "get":
						PZGet.Execute(cert, args[3..]);
						break;
					default:
						SendFault($"invalid command: {args[2]}");
						break;
				}
			}
			catch (Exception ex)
			{
				SendFault(ex.Message);
			}
		}
	}
}
