using OldMusicBox.ePUAP.Client.Model.AddDocumentToSigning;
using OldMusicBox.ePUAP.Client.Model.Common;
using OldMusicBox.ePUAP.Client;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace PZBridge
{
	internal class PZSignStub
	{
		const string FAKE_SIGNATURE =
@"<ds:Signature xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"">
	<ds:Object>
		<xades:QualifyingProperties xmlns:xades=""http://uri.etsi.org/01903/v1.3.2#"">
			<xades:SignedProperties>
				<xades:SignedSignatureProperties>
					<xades:SigningTime>[date]</xades:SigningTime>
					<xades:SignerRole>
						<xades:ClaimedRoles>
							<xades:ClaimedRole>
								<pz:EPSignature  xmlns:pz=""http://crd.gov.pl/xml/schematy/podpis_zaufany/"">
									<pz:NaturalPerson>
										<pz:CurrentFamilyName>[surname]</pz:CurrentFamilyName>
										<pz:FirstName>[name]</pz:FirstName>
									</pz:NaturalPerson>
								</pz:EPSignature>
							</xades:ClaimedRole>
						</xades:ClaimedRoles>
					</xades:SignerRole>
				</xades:SignedSignatureProperties>
			</xades:SignedProperties>
		</xades:QualifyingProperties>
	</ds:Object>
</ds:Signature>";

		public static void Execute(string[] args)
		{
			string host = args[1];
			string id = args[2];
			string redirectURL = $"https://example.com/sign?id={id}";
			string document = Program.GetDocumentFromStandardInput();
			string signature = Program.ReplaceStubPlaceholders(FAKE_SIGNATURE);

			try {
				XDocument xml = XDocument.Parse(document);
				xml.Root?.Add(XElement.Parse(signature));
				string tmpFile = Path.Combine(Path.GetTempPath(), $"pzbridge_stub_{id}.xml");
				File.WriteAllText(tmpFile, xml.ToString());

				Console.Write("{\"ok\": true, \"url\": \"" + redirectURL + "\"}");
			}
			catch (Exception ex) {				
				Program.SendFault("Invalid XML or file write error: " + ex.Message);
			}
		}
	}
}
