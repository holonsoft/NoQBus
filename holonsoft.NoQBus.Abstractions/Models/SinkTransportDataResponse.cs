using System;

namespace holonsoft.NoQBus.Abstractions.Models
{
	public class SinkTransportDataResponse
	{
#if NET5_0_OR_GREATER
		public Guid RequestIdentifier { get; init; }
		public SinkTransportDataResponseEntry[] ResponseEntries { get; init; }
#else
	public Guid RequestIdentifier { get; set; }
		public SinkTransportDataResponseEntry[] ResponseEntries { get; set; }
#endif

		public SinkTransportDataResponse() { } //for serializer

		public SinkTransportDataResponse(Guid requestIdentifier, params SinkTransportDataResponseEntry[] responseEntries) : this()
		{
			RequestIdentifier = requestIdentifier;
			ResponseEntries = responseEntries;
		}

		public SinkTransportDataResponse(SinkTransportDataRequest request, params SinkTransportDataResponseEntry[] responseEntries)
			 : this(request.RequestIdentifier, responseEntries) { }
	}
}
