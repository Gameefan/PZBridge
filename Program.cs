﻿using OldMusicBox.ePUAP.Client.Model.AddDocumentToSigning;
using OldMusicBox.ePUAP.Client.Model.Common;
using OldMusicBox.ePUAP.Client;
using System.Security.Cryptography.X509Certificates;
using System.Text;

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

		static void SetupConsole(int bufferSize)
		{
			Stream inStream = Console.OpenStandardInput(bufferSize);
			Console.SetIn(new StreamReader(inStream, Console.InputEncoding, false, bufferSize));
		}

		static void Main(string[] args)
		{
			SetupConsole(1 * 1024 * 1024); // 1 MiB
			if(args.Length < 3)
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
