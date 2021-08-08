using System.Threading.Tasks;

namespace holonsoft.NoQBus.Abstractions.Contracts
{
	public interface IRequestFilter
	{
		public Task<bool> Filter(IRequest request);
	}

	public interface IRequestFilter<TRequest> where TRequest : IRequest
	{
		public Task<bool> Filter(TRequest request);
	}
}
