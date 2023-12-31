﻿using MossWPF.Domain;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MossWPF.Services
{
    public interface IMossCommunication
    {
        void Connect();
        void Disconnect();
        string ReceiveResponse(int replySize);
        void SendOptions();
    }

    public class MossCommunication : IDisposable, IMossCommunication
    {
        private const string FileUploadFormat = "file {0} {1} {2} {3}\n";

        private Socket _mossSocket;
        private MossSubmission request;
        private IAppConfiguration settings;

        public MossCommunication(MossSubmission submission, IAppConfiguration settings)
        {
            request = submission;
            this.settings = settings;
        }

        public void Connect()
        {
            var hostEntry = Dns.GetHostEntry(settings.ServerSettings.Server);
            var address = hostEntry.AddressList[0];
            var ipe = new IPEndPoint(address, settings.ServerSettings.Port);

            _mossSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _mossSocket.Connect(ipe);
        }

        public void Disconnect()
        {
            SendOption(settings.ScriptSettings.Opt_end, string.Empty);
        }

        public void SendOptions()
        {
            SendOption(settings.ScriptSettings.Opt_moss, settings.UserOptions.UserId.ToString(CultureInfo.InvariantCulture));
            SendOption(settings.ScriptSettings.Opt_d, request.UseDirectoryMode ? "1" : "0");
            SendOption(settings.ScriptSettings.Opt_x, request.UseExperimental ? "1" : "0");
            SendOption(settings.ScriptSettings.Opt_m, request.Sensitivity.ToString(CultureInfo.InvariantCulture));
            SendOption(settings.ScriptSettings.Opt_n, request.ResultsToShow.ToString(CultureInfo.InvariantCulture));

            if (request.BaseFiles.Count != 0)
            {
                foreach (var file in request.BaseFiles)
                {
                    SendFile(file.Path, 0);
                }
            }

            if (request.SourceFiles.Count != 0)
            {
                int fileCount = 1;
                foreach (var file in request.SourceFiles)
                {
                    SendFile(file.Path, fileCount++);
                }
            }


        }

        public string ReceiveResponse(int replySize)
        {
            SendOption("query 0", request.Comments);
            var bytes = new byte[replySize];
            _mossSocket.Receive(bytes);
            return Encoding.UTF8.GetString(bytes);
        }

        public void Dispose()
        {
            _mossSocket?.Dispose();
        }

        private void SendOption(string option, string value)
        {
            _mossSocket.Send(Encoding.UTF8.GetBytes($"{option} {value}\n"));
        }

        private void SendFile(string file, int number)
        {
            var fileInfo = new FileInfo(file);
            _mossSocket.Send(
                Encoding.UTF8.GetBytes(
            string.Format(
                        FileUploadFormat,
                        number,
                        request.SelectedLanguage.Code,
                        fileInfo.Length,
                        fileInfo.FullName.Replace("\\", "/").Replace(" ", string.Empty))));
            Console.WriteLine(fileInfo.FullName.Replace("\\", "/").Replace(" ", string.Empty));
            _mossSocket.BeginSendFile(file, null, _mossSocket);
        }
    }
}
