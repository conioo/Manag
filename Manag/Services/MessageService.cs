using Common.Helpers;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Manag.Utilities;
using System;
using System.Diagnostics;

namespace Manag.Services
{
    public class MessageService : Message.MessageBase
    {
        public override Task<Empty> ShowError(ErrorRequest request, ServerCallContext context)
        {
            Random random = new Random();

            for (int i = 0; i < request.Count; ++i)
            {
                int index = random.Next(ErrorDictionary.Errors.Count);

                MessageBox.Show(ErrorDictionary.Errors[index].Item2, ErrorDictionary.Errors[index].Item1, MessageBoxButtons.MB_ABORTRETRYIGNORE, MessageBoxIcon.MB_ICONERROR, MessageBoxDefaultButton.MB_DEFBUTTON2, MessageBoxModality.MB_SYSTEMMODAL, MessageBoxOptions.MB_TOPMOST);
            }

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> ErrorInterval(ErrorIntervalMessage request, ServerCallContext context)
        {
            _ = Interval(request);

            return Task.FromResult(new Empty());
        }

        public override Task<MessageResponse> ShowMessage(MessageRequest request, ServerCallContext context)
        {
            NotificationResult lastResult = NotificationResult.ID_NO;

            for (int i = 0; i < request.Count; ++i)
            {
                lastResult = MessageBox.Show(request.Content, request.Caption, MessageBoxButtons.MB_YESNO, MessageBoxIcon.MB_ICONINFORMATION, MessageBoxDefaultButton.MB_DEFBUTTON2, MessageBoxModality.MB_SYSTEMMODAL, MessageBoxOptions.MB_TOPMOST);
            }

            if (lastResult == NotificationResult.ID_YES)
            {
                return Task.FromResult(new MessageResponse() { Result = true});
            }
            else
            {
                return Task.FromResult(new MessageResponse() { Result = false });
            }
        }

        private async Task Interval(ErrorIntervalMessage request)
        {
            Random random = new Random();

            for (int i = 0; i < request.Count; ++i)
            {
                for (int k = 0; k < request.CountPerInterval; ++k)
                {
                    int index = random.Next(ErrorDictionary.Errors.Count);

                    MessageBox.Show(ErrorDictionary.Errors[index].Item2, ErrorDictionary.Errors[index].Item1, MessageBoxButtons.MB_ABORTRETRYIGNORE, MessageBoxIcon.MB_ICONERROR, MessageBoxDefaultButton.MB_DEFBUTTON2, MessageBoxModality.MB_SYSTEMMODAL, MessageBoxOptions.MB_TOPMOST);
                }

                Thread.Sleep(request.IntervalInSeconds * 1000);
            }
        }
    }
}
