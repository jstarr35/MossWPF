using MossWPF.Domain.Configurations;
using MossWPF.Domain.Models;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MossWPF.Services
{


    public record MossSocketResult(bool Success, string? ErrorMessage=null, string? Response=null, Exception? Exception=default);

    public interface IMossCommunication
    {
        Task<MossSocketResult> DisconnectAsync();
        Task<MossSocketResult> TryConnectAsync();
        Task<MossSocketResult> TryReceiveResponseAsync();
        Task<MossSocketResult> ReceiveResponseAsync();
        Task<MossSocketResult> TrySendOptions();
    }

    public class MossCommunication : IDisposable, IMossCommunication
    {
        private const string FileUploadFormat = "file {0} {1} {2} {3}\n";

        private Socket? _mossSocket;
        private MossSubmission request;
        private readonly IAppConfiguration settings;

        public MossCommunication(MossSubmission submission, IAppConfiguration settings)
        {
            request = submission;
            this.settings = settings;
        }

        public async Task<MossSocketResult> TryConnectAsync()
        {
            try
            {
                var hostEntry = Dns.GetHostEntry(settings.ServerSettings.Server);
                var address = hostEntry.AddressList[0];
                var ipe = new IPEndPoint(address, settings.ServerSettings.Port);

                _mossSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Debug.WriteLine("Socket Connectings...");
                await _mossSocket.ConnectAsync(ipe);
                Debug.WriteLine("Socket Connected.");
                return new(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
                return new(false, "Unable to connect to Moss server.", null, ex);
            }
        }

        public async Task<MossSocketResult> DisconnectAsync()
        {
            var disconnect = await TrySendOption(settings.ScriptSettings.Opt_end, string.Empty);
            if (!disconnect.Success) return disconnect;

            return new(true);
        }

        public async Task<MossSocketResult> TrySendOptions()
        {
            Debug.WriteLine("Sending User ID.");
            var opSendM = await TrySendOption(settings.ScriptSettings.Opt_moss, request.UserId);
            if (!opSendM.Success) return opSendM;
            Debug.WriteLine("Sending -d.");
            var opSendD = await TrySendOption(settings.ScriptSettings.Opt_d, request.UseDirectoryMode ? "1" : "0");
            if (!opSendD.Success) return opSendD;
            Debug.WriteLine("Sending -x.");
            var opSendX = await TrySendOption(settings.ScriptSettings.Opt_x, request.UseExperimental ? "1" : "0");
            if (!opSendX.Success) return opSendX;
            Debug.WriteLine("Sending -m.");
            var opSendm = await TrySendOption(settings.ScriptSettings.Opt_m, request.Sensitivity.ToString(CultureInfo.InvariantCulture));
            if (!opSendm.Success) return opSendm;
            Debug.WriteLine("Sending -n.");
            var opSendn = await TrySendOption(settings.ScriptSettings.Opt_n, request.ResultsToShow.ToString(CultureInfo.InvariantCulture));
            if (!opSendn.Success) return opSendn;

            if (request.BaseFiles.Count != 0)
            {
                Debug.WriteLine("Sending Base Files");
                foreach (var file in request.BaseFiles)
                {
                    var baseSend = await TrySendFile(file.Path, 0);
                    if (!baseSend.Success) return baseSend;

                }
            }

            if (request.SourceFiles.Count != 0)
            {
                Debug.WriteLine("Sending Source Files");
                int fileCount = 1;
                foreach (var file in request.SourceFiles)
                {
                    var sourceSend = await TrySendFile(file.Path, fileCount++);
                    if (!sourceSend.Success) return sourceSend;
                }
            }

            return new(true);
        }

        public async Task<MossSocketResult> TryReceiveResponseAsync()
        {
            if (_mossSocket is null)
                return new(false, "Moss Socket has not been created");

            try
            {
                var optionSend = await TrySendOption("query 0", request.Comments);
                if (!optionSend.Success)
                    return optionSend;

                // Create a CancellationTokenSource with the desired timeout
                using (var cts = new CancellationTokenSource(settings.ServerSettings.ReceiveTimeout))
                {
                    var bytes = new byte[settings.ServerSettings.ReplySize];
                    Debug.WriteLine("Receiving response...");

                    // Pass the CancellationToken from CancellationTokenSource to ReceiveAsync
                    var result = await _mossSocket.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None, cts.Token);

                    if (cts.Token.IsCancellationRequested)
                    {
                        // Timeout occurred
                        return new(false, "ReceiveTimeout while receiving response from the server.");
                    }

                    Debug.WriteLine("Response received.");

                    var response = Encoding.UTF8.GetString(bytes);
                    return new(true, null, response);
                }
            }
            catch (Exception e)
            {
                return new(false, "Error receiving response from server.", null, e);
            }
        }

        public async Task<MossSocketResult> ReceiveResponseAsync()
        {
            try
            {
                await TrySendOption("query 0", request.Comments);
                var bytes = new byte[settings.ServerSettings.ReplySize];
                _mossSocket.Receive(bytes);
                var response = Encoding.UTF8.GetString(bytes);
                return new(true, null, response);
            }
            catch (Exception ex)
            {

                var errorMessage = "Error receiving response from server.";
                return new(false, errorMessage, null, ex);
            }
        }

        public void Dispose()
        {
            _mossSocket?.Dispose();
        }

        private async Task<MossSocketResult> TrySendOption(string option, string value)
        {
            try
            {
                await _mossSocket!.SendAsync(Encoding.UTF8.GetBytes($"{option} {value}\n"), SocketFlags.None);
                return new(true);
            }
            catch (Exception ex)
            {
                return new(false, $"Failed to send option {option} {value}", null, ex);
            }
        }

        private async Task<MossSocketResult> TrySendFile(string file, int number)
        {

            try
            {
                var fileInfo = new FileInfo(file);
                _mossSocket!.SendTimeout = settings.ServerSettings.SendTimeout;
                await _mossSocket.SendAsync(
                    Encoding.UTF8.GetBytes(
                string.Format(
                            FileUploadFormat,
                            number,
                            request.SelectedLanguage.Code,
                            fileInfo.Length,
                            fileInfo.FullName.Replace("\\", "/").Replace(" ", string.Empty))), SocketFlags.None);
                Debug.WriteLine(fileInfo.FullName.Replace("\\", "/").Replace(" ", string.Empty));
                _mossSocket.BeginSendFile(file, null, _mossSocket);
                return new(true);
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
            {
                return new(false, "Timeout while sending file to the server.", null, ex);
            }
            catch (Exception ex)
            {
                return new(false, null, null, ex);

            }
            finally
            {
                _mossSocket!.SendTimeout = 0;
            }
        }
    }
}
