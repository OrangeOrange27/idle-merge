
namespace Package.Pooling.Infrastructures
{
	public interface IPoolStrategy : ICreatePoolStrategy, IGetPoolStrategy, IReturnPoolStrategy
	{
	}
}
