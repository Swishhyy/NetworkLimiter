using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using NetworkLimiter.src.NetworkMonitorApp.Interfaces;
using System.Threading;
using System.Security.Cryptography.X509Certificates;

namespace NetworkLimiter.src.NetworkMonitorApp.Platform.Windows
{
    public class WindowsBandwidthLimiter : IBandwidthLimiter
    {
        private ICaptureDevice _device;
        private long _bytesPerSecond;

        public void SetLimit(string interfaceName, long bytesPerSecond)
        {
            _bytesPerSecond = bytesPerSecond;
            _device = GetDevice(interfaceName);

            if (_device == null)
            {
                throw new Exception("Unable to initialize the network device.");
            }

            // Configure the device with promiscuous mode and a 1000ms read timeout
            var config = new DeviceConfiguration
            {
                Mode = DeviceModes.Promiscuous,
                ReadTimeout = 1000
            };

            _device.Open(config); // Use DeviceConfiguration to open the device

            // Attach event handler for packet arrival
            _device.OnPacketArrival += new PacketArrivalEventHandler(OnPacketArrival);

            // Start capturing packets
            _device.StartCapture();
        }

        public void RemoveLimit(string interfaceName)
        {
            if (_device != null)
            {
                _device.StopCapture();
                _device.Close();
            }
        }

        private void ListAvailableInterfaces()
        {
            foreach (var dev in CaptureDeviceList.Instance)
            {
                Console.WriteLine($"Name: {dev.Name}, Description: {dev.Description}");
            }
        }

        private void OnPacketArrival(object sender, PacketCapture e)
        {
            try
            {
                var packet = Packet.ParsePacket(e.GetPacket().LinkLayerType, e.GetPacket().Data);
                long packetSize = e.GetPacket().Data.Length;

                // Simulate throttling by adding a delay if needed
                if (packetSize > _bytesPerSecond)
                {
                    Thread.Sleep(100); // Delay to simulate throttling
                }

                // Send the packet back into the network if the device supports injection
                if (_device is IInjectionDevice injectionDevice)
                {
                    injectionDevice.SendPacket(e.GetPacket());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending packet: {ex.Message}");
            }
        }

        private ICaptureDevice GetDevice(string interfaceName)
        {
            var device = CaptureDeviceList.Instance.FirstOrDefault(dev => dev.Name == interfaceName);
            if (device != null)
            {
                return device;
            }
            throw new ArgumentException($"Interface {interfaceName} not found.");
        }
    }
}
