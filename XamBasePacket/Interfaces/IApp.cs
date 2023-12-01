using System.Threading.Tasks;
using Prism.Navigation;

namespace XamBasePacket.Interfaces
{
    public interface IApplication
    {
        Task<INavigationResult> ResolveInitialNavigation();
    }
}
