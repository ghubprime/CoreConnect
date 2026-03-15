using System.Threading.Tasks;

namespace CoreConnect.Desktop.Core.Interfaces
{
    public interface IChatClientService
    {
        Task StartChat(string requesterID, string organizationName);
    }
}
