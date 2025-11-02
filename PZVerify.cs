using OldMusicBox.ePUAP.Client.Model.Common;
using OldMusicBox.ePUAP.Client;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using OldMusicBox.ePUAP.Client.Model.VerifySignedDocument;
using Newtonsoft.Json;

namespace PZBridge
{
	internal class PZVerify
	{
		public static void Execute(X509Certificate2 cert, string[] args)
		{
			if (args.Length < 1)
			{
				Program.SendFault("verify: not enough arguments\nsyntax: verify <env>\nstdin: <document> \\n EOF");
				return;
			}

			if (args[0] == "stub") {
				Program.SendFault("verify: this operation isn't supported in stub mode");
				return;
			}

			TpSigning5Client client = new(args[0] == "prod" ? TpSigning5Client.PRODUCTION_URI : TpSigning5Client.INTEGRATION_URI, cert);
			
			string document = Program.GetDocumentFromStandardInput();

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
