using OldMusicBox.ePUAP.Client.Model.Common;
using OldMusicBox.ePUAP.Client;
using System.Security.Cryptography.X509Certificates;
using OldMusicBox.ePUAP.Client.Model.GetSignedDocument;
using Newtonsoft.Json;

namespace PZBridge
{
	internal class PZGetStub
	{
		static string Base64Encode(string plainText) 
		{
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		public static void Execute(string[] args)
		{
			string id = args[1].Split('=').Last();
			string tmpFile = Path.Combine(Path.GetTempPath(), $"pzbridge_stub_{id}.xml");

			if (!File.Exists(tmpFile))
			{
				Program.SendFault($"No stub signed document found for id {id}");
				return;
			}

			string xml = File.ReadAllText(tmpFile);

			var response = new {
				Content = Base64Encode(xml),
				Signature = new {
					IsValid = true,
					NaturalPerson = new {
						FirstName = "[name]",
						CurrentFamilyName = "[surname]",
						PersonalIdentifier = "[pesel]"
					},
					SignatureData = new {
						IdentityIssuer = "PZBridge-stub",
						IdentityIssueTimestamp = "[date]"
					}
				},
				IsValid = true
			};

			Console.Write("{\"ok\": true, \"data\": " + Program.ReplaceStubPlaceholders(JsonConvert.SerializeObject(response)) + "}");
		}
	}
}
