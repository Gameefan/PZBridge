using OldMusicBox.ePUAP.Client.Model.AddDocumentToSigning;
using OldMusicBox.ePUAP.Client.Model.Common;
using OldMusicBox.ePUAP.Client;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PZBridge
{
	internal class PZSign
	{
		public static void Execute(X509Certificate2 cert, string[] args)
		{
			if (args.Length < 4)
			{
				Program.SendFault("sign: not enough arguments\nsyntax: sign <env> <host> <id> <info>\nstdin: <document> \\n EOF");
				return;
			}

			if (args[0] == "stub") {
				PZSignStub.Execute(args);
				return;
			}

			TpSigning5Client client = new(args[0] == "prod" ? TpSigning5Client.PRODUCTION_URI : TpSigning5Client.INTEGRATION_URI, cert);
			string successURL = $"{args[1]}/pz/success?id={args[2]}";
			string failureURL = $"{args[1]}/pz/failure?id={args[2]}";

			string additionalInfo = string.Join(' ', args[3..]);

			string document = Program.GetDocumentFromStandardInput();

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
