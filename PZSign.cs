using OldMusicBox.ePUAP.Client.Model.AddDocumentToSigning;
using OldMusicBox.ePUAP.Client.Model.Common;
using OldMusicBox.ePUAP.Client;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;

namespace PZBridge
{
	internal class PZSign
	{
		public static void Execute(X509Certificate2 cert, string[] args)
		{
			if (args.Length < 2)
			{
				Program.SendFault("sign: not enough arguments\nsyntax: sign <id> <info>\nstdin: <document> \\n EOF");
				return;
			}

			TpSigning5Client client = new(args[0].StartsWith("int-") ? TpSigning5Client.INTEGRATION_URI : TpSigning5Client.PRODUCTION_URI, cert);
			string successURL = $"https://localhost/pz/success?id={args[0]}";
			string failureURL = $"https://localhost/pz/failure?id={args[0]}";


			string additionalInfo = string.Join(' ', args[1..]);

			string document = "";

			while (true) {
				string? data = Console.ReadLine();
				if (data == null || data == "EOF") break;
				document += data;
				document += "\r\n";
			}

			byte[] documentBytes = Encoding.UTF8.GetBytes(document);

			FaultModel faultModel;
			AddDocumentToSigning5Response response = client.AddDocumentToSigning(documentBytes, successURL, failureURL, additionalInfo, out faultModel);

			if (response == null)
			{
				if (faultModel == null)
				{
					Program.SendFault("response is null\nfault is null");
					return;
				}
				Program.SendFault(faultModel);
				return;
			}
			Console.Write("{\"ok\": true, \"url\": \"" + response.Url + "\"}");
		}
	}
}
