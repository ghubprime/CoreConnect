using CoreConnect.Shared.Dtos;
using System.Threading.Tasks;

namespace CoreConnect.Agent.Interfaces;

public interface IDeviceInformationService
{
    Task<DeviceClientDto> CreateDevice(string deviceId, string orgId);
}
