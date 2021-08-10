using System.Threading.Tasks;
using holonsoft.NoQBus.Abstractions.Models;

namespace holonsoft.NoQBus.Abstractions.Contracts
{
	/// <summary>
	/// Provides an interface for remoting request/response
	/// Hint: fire & forget message will be supported, but return internally a <see cref="VoidResponse"/>
	/// </summary>
	public interface IRemoteMessageBus
	{
		/// <summary>
		/// Get an array of responses from a remote endpoint
		/// </summary>
		/// <param name="request"></param>
		/// <returns>A task with an array of<see cref="IResponse"/></returns>
		public Task<IResponse[]> GetResponsesForRemoteRequest(IRequest request);
	}
}
