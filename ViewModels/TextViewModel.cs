using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using OpenCCNET;

namespace OpenCC.NET.GUI.ViewModels
{
    public class TextViewModel : ObservableRecipient
    {
        private string _originalText;
        private string _convertedText;

        public TextViewModel()
        {
            IsActive = true;
        }

        protected override void OnActivated()
        {
            // 收到请求后发送原始文本
            Messenger.Register<TextViewModel, string, string>(this, "RequestText", (_, m) =>
            {
                if (!string.IsNullOrWhiteSpace(OriginalText))
                {
                    Messenger.Send(OriginalText, "OriginalText");
                }
            });
            // 接受转换后的文本显示
            Messenger.Register<TextViewModel, string, string>(this, "ConvertedText", (_, m) => { ConvertedText = m; });
        }
        
        public string OriginalText
        {
            get => _originalText;
            set => SetProperty(ref _originalText, value);
        }
        
        public string ConvertedText
        {
            get => _convertedText;
            set => SetProperty(ref _convertedText, value);
        }
    }
}