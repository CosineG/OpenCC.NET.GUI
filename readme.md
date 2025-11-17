# OpenCC.NET GUI

![](opencc-net-gui.png)

## 介绍

OpenCC.NET GUI 是基于 OpenCC (Open Chinese Convert, 开放中文转换)，使用 [OpenCC.NET](https://github.com/CosineG/OpenCC.NET) 实现的中文转换工具，支持中文简繁体之间词汇级别的转换，同时还支持地域间异体字以及词汇的转换。

## 功能

- 支持文本和批量文件的简繁转换。
- 文件格式：支持纯文本、Word (`.docx`)、Excel (`.xlsx`) 和 PowerPoint (`.pptx`)。暂不支持 PDF 格式。
- 转换选项：
  - 可自定义原文、目标格式（简/繁）。
  - 支持 OpenCC、台湾、香港异体字标准。
  - 支持大陆、台湾地区词汇转换。
  - 可选“最大匹配”或“结巴分词”模式。


## 开始

### 获取

可以直接在仓库页面右侧获取最新Release版本，或下载源码后自行编译。若无法运行，请确保安装了 [.NET 8 运行时](https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/runtime-desktop-8.0.22-windows-x64-installer)。

### 使用

软件提供了文本编辑转换和文件批量转换两种功能。

#### 文本编辑转换

此模式下直接在文本框中输入你所需要转换的语句，在右侧选择转换选项后，点击转换按钮即可得到结果。

![](screenshot-1.png)

#### 文件批量转换

此模式下可以批量导入文件进行转换。当前支持纯文本和 `.docx`, `.pptx`, `.xlsx` 格式，未知格式将默认按照纯文本进行转换，暂不支持 PDF。

![](screenshot-2.png)

## 引用

### OpenCC.NET

[OpenCC.NET](https://github.com/CosineG/OpenCC.NET)实现文本转换。

### OpenCC

[OpenCC](https://github.com/BYVoid/OpenCC)为OpenCC.NET提供词库。

### jieba.NET

[jieba.NET](https://github.com/anderscui/jieba.NET)为OpenCC.NET提供分词。

### ModernWpf

[ModernWpf](https://github.com/Kinnara/ModernWpf)提供UI控件。

### Ookii.Dialogs.Wpf

[Ookii.Dialogs.Wpf](https://github.com/ookii-dialogs/ookii-dialogs-wpf)提供选择文件夹窗口（没错，WPF没有自带选择文件夹窗口）。