using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OpenCC.NET.GUI.Enums;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Presentation;
using Drawing = DocumentFormat.OpenXml.Drawing;
using OpenCCNET;

namespace OpenCC.NET.GUI.ViewModels
{
    public class OptionViewModel : ObservableRecipient
    {
        private SegmentationMode _segmentationMode;
        private CharacterType _originalCharacter;
        private CharacterType _targetCharacter;
        private VariantType _targetVariant;
        private bool _isIdiomConvert;
        private Visibility _radVariantVisibility;
        private Visibility _swIdiomVisibility;
        private bool _isRadTargetSimplifiedEnabled;
        private string _idiomName;
        private string _activeMode;
        private bool _isProcessing;
        public ICommand ConvertCommand { get; }

        public OptionViewModel()
        {
            ConvertCommand = new RelayCommand(RequestData);
            IsActive = true;
            SegmentationMode = SegmentationMode.MaxMatch;
        }

        protected override void OnActivated()
        {
            // 确认当前转换模式(当前所在页面)
            Messenger.Register<OptionViewModel, string, string>(this, "ActiveMode", (_, m) => { _activeMode = m; });
            // 接收文本消息进行转换
            Messenger.Register<OptionViewModel, string, string>(this, "OriginalText",
                (_, m) => { Messenger.Send(ConvertText(m), "ConvertedText"); });
            // 接收文件列表进行转换
            Messenger.Register<OptionViewModel, ICollection<File>, string>(this, "FileList",
                async (_, m) => await ConvertFileAsync(m));
        }

        /// <summary>
        /// 分词方式
        /// </summary>
        public SegmentationMode SegmentationMode
        {
            get => _segmentationMode;
            set
            {
                if (!SetProperty(ref _segmentationMode, value)) return;
                switch (value)
                {
                    case SegmentationMode.Jieba:
                        ZhConverter.ZhSegment.SetMode(SegmentMode.Jieba);
                        break;
                    case SegmentationMode.MaxMatch:
                        ZhConverter.ZhSegment.SetMode(SegmentMode.MaxMatch);
                        break;
                    default:
                        ZhConverter.ZhSegment.SetMode(SegmentMode.MaxMatch);
                        break;
                }
            }
        }

        /// <summary>
        /// 原文字体类型
        /// </summary>
        public CharacterType OriginalCharacter
        {
            get => _originalCharacter;
            set
            {
                SetProperty(ref _originalCharacter, value);
                TargetCharacterValidityCheck();
                IdiomNameAndVisibilityCheck();
            }
        }

        /// <summary>
        /// 目标字体类型
        /// </summary>
        public CharacterType TargetCharacter
        {
            get => _targetCharacter;
            set
            {
                SetProperty(ref _targetCharacter, value);
                TargetVariantValidityCheck();
                IdiomNameAndVisibilityCheck();
            }
        }

        /// <summary>
        /// 目标异体字标准
        /// </summary>
        public VariantType TargetVariant
        {
            get => _targetVariant;
            set
            {
                SetProperty(ref _targetVariant, value);
                IdiomNameAndVisibilityCheck();
            }
        }

        /// <summary>
        /// 是否转换地域用词
        /// </summary>
        public bool IsIdiomConvert
        {
            get => _isIdiomConvert;
            set => SetProperty(ref _isIdiomConvert, value);
        }

        /// <summary>
        /// 目标异体字标准选项可见性
        /// </summary>
        public Visibility RadVariantVisibility
        {
            get => _radVariantVisibility;
            set => SetProperty(ref _radVariantVisibility, value);
        }

        /// <summary>
        /// 是否启用转换为简体
        /// </summary>
        public bool IsRadTargetSimplifiedEnabled
        {
            get => _isRadTargetSimplifiedEnabled;
            set => SetProperty(ref _isRadTargetSimplifiedEnabled, value);
        }

        /// <summary>
        /// 地域名称
        /// </summary>
        public string IdiomName
        {
            get => _idiomName;
            set => SetProperty(ref _idiomName, value);
        }

        /// <summary>
        /// 转换地域用词可见性
        /// </summary>
        public Visibility SwIdiomVisibility
        {
            get => _swIdiomVisibility;
            set => SetProperty(ref _swIdiomVisibility, value);
        }

        /// <summary>
        /// 是否正在进行转换操作
        /// </summary>
        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }

        /// <summary>
        /// 检查目标字体单选框是否可用
        /// 可以繁->繁，不能简->简
        /// </summary>
        private void TargetCharacterValidityCheck()
        {
            if (OriginalCharacter == CharacterType.Simplified)
            {
                TargetCharacter = CharacterType.Traditional;
                IsRadTargetSimplifiedEnabled = false;
            }
            else
            {
                IsRadTargetSimplifiedEnabled = true;
            }
        }

        /// <summary>
        /// 检查目标异体字标准选项是否可用
        /// 繁->简时无异体字
        /// </summary>
        private void TargetVariantValidityCheck()
        {
            RadVariantVisibility =
                TargetCharacter == CharacterType.Simplified ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查地域词汇转换的地域名称和可见性
        /// </summary>
        private void IdiomNameAndVisibilityCheck()
        {
            if (TargetCharacter == CharacterType.Simplified)
            {
                SwIdiomVisibility = Visibility.Visible;
                IdiomName = "转换为大陆地区常用词汇";
            }
            else if (TargetVariant == VariantType.TW)
            {
                SwIdiomVisibility = Visibility.Visible;
                IdiomName = "转换为台湾地区常用词汇";
            }
            else
            {
                IsIdiomConvert = false;
                SwIdiomVisibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 请求原始数据
        /// </summary>
        private void RequestData()
        {
            switch (_activeMode)
            {
                case "Text":
                    Messenger.Send("", "RequestText");
                    break;
                case "File":
                    Messenger.Send("", "RequestFile");
                    break;
            }
        }

        /// <summary>
        /// 文本转换
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string ConvertText(string text)
        {
            var result = text;
            if (OriginalCharacter == CharacterType.Simplified)
            {
                result = TargetVariant switch
                {
                    VariantType.OpenCC => text.ToHantFromHans(),
                    VariantType.TW => text.ToTWFromHans(IsIdiomConvert),
                    VariantType.HK => text.ToHKFromHans(),
                    _ => result
                };
            }
            else
            {
                if (TargetCharacter == CharacterType.Simplified)
                {
                    result = text.ToHansFromTW(IsIdiomConvert);
                }
                else
                {
                    result = TargetVariant switch
                    {
                        VariantType.OpenCC => text.ToHantFromTW(),
                        VariantType.TW => text.ToTWFromHant(IsIdiomConvert),
                        VariantType.HK => text.ToHKFromHant(),
                        _ => result
                    };
                }
            }

            return result;
        }

        /// <summary>
        /// 异步转换文件
        /// </summary>
        /// <param name="files">文件集合</param>
        /// <returns></returns>
        public async Task ConvertFileAsync(ICollection<File> files)
        {
            IsProcessing = true;
            var tasks = files.Select(async file =>
            {
                if (file.Status == FileStatus.Success)
                {
                    return;
                }

                try
                {
                    // 指定输入输出路径
                    var filePath = file.Path;
                    var suffix = GetProcessSuffix();
                    var extension = Path.GetExtension(filePath);
                    var outputPath = Path.Combine(file.OutputFolder,
                        Path.GetFileNameWithoutExtension(filePath) + suffix + extension);

                    Messenger.Send(file, "FileStatusRunning");
                    
                    // 转换
                    switch (extension.ToLower())
                    {
                        case ".pdf":
                            MessageBox.Show("暂不支持PDF格式文件的转换。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            Messenger.Send(file, "FileStatusFail");
                            return;
                        case ".docx":
                            await ConvertDocxFileAsync(filePath, outputPath);
                            break;
                        case ".xlsx":
                            await ConvertXlsxFileAsync(filePath, outputPath);
                            break;
                        case ".pptx":
                            await ConvertPptxFileAsync(filePath, outputPath);
                            break;
                        default:
                            await ConvertTextFileAsync(filePath, outputPath);
                            break;
                    }
                }
                catch (Exception)
                {
                    Messenger.Send(file, "FileStatusFail");
                    return;
                }

                Messenger.Send(file, "FileStatusSuccess");
            });
            await Task.WhenAll(tasks);
            IsProcessing = false;
        }

        /// <summary>
        /// 将当前处理方式转换为字符串作为输出文件的后缀
        /// </summary>
        private string GetProcessSuffix()
        {
            var suffix = new StringBuilder();
            switch (TargetCharacter)
            {
                case CharacterType.Simplified:
                    suffix.Append("_简");
                    break;
                case CharacterType.Traditional:
                    suffix.Append("_繁");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (TargetVariant)
            {
                case VariantType.OpenCC:
                    suffix.Append("_OpenCC");
                    break;
                case VariantType.TW:
                    suffix.Append("_台");
                    break;
                case VariantType.HK:
                    suffix.Append("_港");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (IsIdiomConvert)
            {
                suffix.Append("_转词");
            }

            return suffix.ToString();
        }

        /// <summary>
        /// 转换txt格式的文本文件
        /// </summary>
        /// <param name="filePath">输入文件路径</param>
        /// <param name="outputPath">输出文件路径</param>
        /// <returns></returns>
        private async Task ConvertTextFileAsync(string filePath, string outputPath)
        {
            using var reader = new StreamReader(filePath);
            await using var writer = new StreamWriter(outputPath);

            var lines = new List<string>();
            string originalLine;
            while ((originalLine = await reader.ReadLineAsync()) != null)
            {
                lines.Add(originalLine);
            }

            var childTasks = Enumerable.Range(0, lines.Count).Select(async i =>
            {
                await Task.Run(() => lines[i] = ConvertText(lines[i]));
            });

            await Task.WhenAll(childTasks);

            foreach (var line in lines)
            {
                await writer.WriteLineAsync(line);
            }
        }

        /// <summary>
        /// 转换docx格式的Word文档
        /// </summary>
        /// <param name="filePath">输入文件路径</param>
        /// <param name="outputPath">输出文件路径</param>
        /// <returns></returns>
        private async Task ConvertDocxFileAsync(string filePath, string outputPath)
        {
            System.IO.File.Copy(filePath, outputPath);
            try
            {
                using var docx = WordprocessingDocument.Open(outputPath, true);
                if (docx.MainDocumentPart != null)
                {
                    var document = docx.MainDocumentPart.Document;

                    var tasks = document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>().Select(async text =>
                    {
                        await Task.Run(() => text.Text = ConvertText(text.Text));
                    });

                    await Task.WhenAll(tasks);
                }
            }
            catch (Exception)
            {
                System.IO.File.Delete(outputPath);
                throw;
            }
        }

        /// <summary>
        /// 转换xlsx格式的Excel文档
        /// </summary>
        /// <param name="filePath">输入文件路径</param>
        /// <param name="outputPath">输出文件路径</param>
        /// <returns></returns>
        private async Task ConvertXlsxFileAsync(string filePath, string outputPath)
        {
            System.IO.File.Copy(filePath, outputPath);
            try
            {
                using var spreadsheetDocument = SpreadsheetDocument.Open(outputPath, true);
                var workbookPart = spreadsheetDocument.WorkbookPart;
                if (workbookPart == null) return;

                var sharedStringTablePart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                SharedStringTable sharedStringTable = null;
                if (sharedStringTablePart != null)
                {
                    sharedStringTable = sharedStringTablePart.SharedStringTable;
                }

                foreach (var worksheetPart in workbookPart.WorksheetParts)
                {
                    var sheetData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();
                    if (sheetData == null) continue;

                    var tasks = sheetData.Elements<Row>().SelectMany(row => row.Elements<Cell>()).Select(async cell =>
                    {
                        if (cell.CellValue != null)
                        {
                            string originalText = null;

                            if (cell.DataType is { Value: CellValues.SharedString })
                            {
                                if (sharedStringTable != null && int.TryParse(cell.CellValue.Text, out var ssid))
                                {
                                    originalText = sharedStringTable.ElementAt(ssid).InnerText;
                                }
                            }
                            else
                            {
                                originalText = cell.CellValue.Text;
                            }

                            if (!string.IsNullOrEmpty(originalText))
                            {
                                await Task.Run(() =>
                                {
                                    var convertedText = ConvertText(originalText);
                                    
                                    if (cell.DataType is { Value: CellValues.SharedString })
                                    {
                                        cell.DataType = new EnumValue<CellValues>(CellValues.String);
                                        cell.CellValue.Text = convertedText;
                                    }
                                    else
                                    {
                                        cell.CellValue.Text = convertedText;
                                    }
                                });
                            }
                        }
                    });

                    await Task.WhenAll(tasks);
                    worksheetPart.Worksheet.Save();
                }
                workbookPart.Workbook.Save();
            }
            catch (Exception)
            {
                System.IO.File.Delete(outputPath);
                throw;
            }
        }

        /// <summary>
        /// 转换pptx格式的PowerPoint文档
        /// </summary>
        /// <param name="filePath">输入文件路径</param>
        /// <param name="outputPath">输出文件路径</param>
        /// <returns></returns>
        private async Task ConvertPptxFileAsync(string filePath, string outputPath)
        {
            System.IO.File.Copy(filePath, outputPath);
            try
            {
                using var presentationDocument = PresentationDocument.Open(outputPath, true);
                var presentationPart = presentationDocument.PresentationPart;
                if (presentationPart == null) return;

                var tasks = new List<Task>();

                foreach (var slidePart in presentationPart.SlideParts)
                {
                    tasks.AddRange(slidePart.Slide.Descendants<Drawing.Text>().Select(async text =>
                    {
                        await Task.Run(() => text.Text = ConvertText(text.Text));
                    }));
                }

                foreach (var slideLayoutPart in presentationPart.GetPartsOfType<SlideLayoutPart>())
                {
                    tasks.AddRange(slideLayoutPart.SlideLayout.Descendants<Drawing.Text>().Select(async text =>
                    {
                        await Task.Run(() => text.Text = ConvertText(text.Text));
                    }));
                }

                foreach (var slideMasterPart in presentationPart.SlideMasterParts)
                {
                    tasks.AddRange(slideMasterPart.SlideMaster.Descendants<Drawing.Text>().Select(async text =>
                    {
                        await Task.Run(() => text.Text = ConvertText(text.Text));
                    }));
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception)
            {
                System.IO.File.Delete(outputPath);
                throw;
            }
        }
    }
}