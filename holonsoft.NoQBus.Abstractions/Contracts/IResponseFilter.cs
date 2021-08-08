using System.Collections.Generic;
using System.Threading.Tasks;

namespace holonsoft.NoQBus.Abstractions.Contracts
{
	public interface IResponseFilter
	{
		public Task<IEnumerable<IResponse>> Filter(IEnumerable<IResponse> responses);
	}

	public interface IResponseFilter<TResponse> where TResponse : IResponse
	{
		public Task<IEnumerable<TResponse>> Filter(IEnumerable<TResponse> responses);
	}
}
