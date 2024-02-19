using OldMusicBox.ePUAP.Client.Model.AddDocumentToSigning;
using OldMusicBox.ePUAP.Client.Model.Common;
using OldMusicBox.ePUAP.Client;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;
using OldMusicBox.ePUAP.Client.Model.VerifySignedDocument;
using Newtonsoft.Json;

namespace PZBridge
{
	internal class PZVerify
	{
		public static void Execute(X509Certificate2 cert, string[] args)
		{
			if (args.Length < 2)
			{
				Program.SendFault("verify: not enough arguments\nsyntax: verify <id> <info>\nstdin: <document> \\n EOF");
				return;
			}

			TpSigning5Client client = new(args[0].StartsWith("int-") ? TpSigning5Client.INTEGRATION_URI : TpSigning5Client.PRODUCTION_URI, cert);
			
			string document = "";
			while (true) {
				string? data = Console.ReadLine();
				if (data == null || data == "EOF") break;
				document += data;
				document += "\r\n";
			}

			byte[] documentBytes = Encoding.UTF8.GetBytes(document);

			FaultModel faultModel;
			VerifySignedDocument5Response response = client.VerifySignedDocument(documentBytes, out faultModel);
			if(faultModel != null)
			{
				Program.SendFault(faultModel);
				return;
			}
			Console.Write("{\"ok\": true, \"data\": " + JsonConvert.SerializeObject(response) + "}");
		}
	}
}
