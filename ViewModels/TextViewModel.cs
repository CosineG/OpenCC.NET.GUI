using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using OpenCCNET;

namespace OpenCC.NET.GUI.ViewModels
{
    public class TextViewModel : ObservableRecipient
    {
        private string _originalText;
        private string _convertedText;

        public ICommand PasteCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand CopyConvertedCommand { get; }
        public ICommand ClearConvertedCommand { get; }

        public TextViewModel()
        {
            IsActive = true;
            PasteCommand = new RelayCommand(() =>
            {
                if (Clipboard.ContainsText())
                {
                    OriginalText = Clipboard.GetText();
                }
            });
            ClearCommand = new RelayCommand(() => OriginalText = "");
            CopyConvertedCommand = new RelayCommand(() =>
            {
                if (!string.IsNullOrEmpty(ConvertedText))
                {
                    Clipboard.SetText(ConvertedText);
                }
            });
            ClearConvertedCommand = new RelayCommand(() => ConvertedText = "");
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