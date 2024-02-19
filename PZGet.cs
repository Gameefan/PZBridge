using OldMusicBox.ePUAP.Client.Model.AddDocumentToSigning;
using OldMusicBox.ePUAP.Client.Model.Common;
using OldMusicBox.ePUAP.Client;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;
using OldMusicBox.ePUAP.Client.Model.VerifySignedDocument;
using OldMusicBox.ePUAP.Client.Model.GetSignedDocument;
using Newtonsoft.Json;

namespace PZBridge
{
	internal class PZGet
	{
		public static void Execute(X509Certificate2 cert, string[] args)
		{
			if (args.Length < 2)
			{
				Program.SendFault("get: not enough arguments\nsyntax: get <env> <url>");
				return;
			}

			TpSigning5Client client = new(args[0] == "prod" ? TpSigning5Client.PRODUCTION_URI : TpSigning5Client.INTEGRATION_URI, cert);
			
			FaultModel faultModel;
			GetSignedDocument5Response response = client.GetSignedDocument(args[1], out faultModel);
			if(faultModel != null)
			{
				Program.SendFault(faultModel);
				return;
			}
			Console.Write("{\"ok\": true, \"data\": " + JsonConvert.SerializeObject(response) + "}");
		}
	}
}
