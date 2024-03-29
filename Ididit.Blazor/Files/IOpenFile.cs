﻿using Microsoft.AspNetCore.Components;

namespace Ididit.Blazor.Files;

public interface IOpenFile
{
    RenderFragment OpenFileDialog(string css, string content, Func<string, Stream, Task> onFileOpened);
}
