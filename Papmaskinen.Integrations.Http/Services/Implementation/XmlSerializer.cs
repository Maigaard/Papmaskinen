using System.Text;
using System.Xml;
using Papmaskinen.Integrations.Http.Services.SerializerSettings;

namespace Papmaskinen.Integrations.Http.Services.Implementation
{
	public class XmlSerializer : AbstractSerializer<XmlSettings>, ISerializer<XmlSettings>
	{
		public override HttpContent Serialize<TContent>(TContent data, XmlSettings settings = null)
		{
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(TContent));
			var stringWriter = new Utf8StringWriter();
			using (XmlWriter writer = XmlWriter.Create(stringWriter, new XmlWriterSettings { OmitXmlDeclaration = false, Encoding = Encoding.UTF8 }))
			{
				serializer.Serialize(writer, data, settings.Namespaces);
			}

			return new StringContent(stringWriter.ToString(), Encoding.UTF8, "text/xml");
		}

		protected override TResult Deserialize<TResult>(string responseText)
		{
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(TResult));

			TResult result;

			using (TextReader reader = new StringReader(responseText))
			{
				result = (TResult)serializer.Deserialize(reader);
			}

			return result;
		}

		private class Utf8StringWriter : StringWriter
		{
			public override Encoding Encoding => Encoding.UTF8;
		}
	}
}
