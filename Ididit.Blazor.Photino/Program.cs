﻿using Ididit.Backup;
using Ididit.Blazor.Files;
using Ididit.Blazor.Layout;
using Ididit.Data;
using Ididit.EntityFrameworkCore;
using Ididit.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Photino.Blazor;
using System;
using System.Diagnostics;

namespace Ididit.Blazor.Photino;

public class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        PhotinoBlazorAppBuilder builder = PhotinoBlazorAppBuilder.CreateDefault(args);

        builder.Services.AddLogging();

        builder.Services.AddServices<OnClickMarkdownExtension>();
        builder.Services.AddDataAccess("OpenHabitTracker.db");
        builder.Services.AddBackup();
        builder.Services.AddScoped<IOpenFile, OpenFile>();
        builder.Services.AddScoped<JsInterop>();
        builder.Services.AddScoped<ISaveFile, SaveFile>();
        builder.Services.AddScoped<INavBarFragment, NavBarFragment>();
        builder.Services.AddScoped<IAssemblyProvider, AssemblyProvider>();

        // register root component and selector
        builder.RootComponents.Add<Routes>("app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        PhotinoBlazorApp app = builder.Build();

        IDataAccess dataAccess = app.Services.GetRequiredService<IDataAccess>();
        dataAccess.Initialize();

        // customize window
        //if (!OperatingSystem.IsLinux()) // TODO: find out why this works in Photino sample
        app.MainWindow.SetIconFile("favicon.ico");
        app.MainWindow.SetTitle("OpenHabitTracker");
        app.MainWindow.SetUseOsDefaultSize(false);
        app.MainWindow.SetSize(1680, 1050);
        app.MainWindow.SetUseOsDefaultLocation(false);
        app.MainWindow.SetTop(0);
        app.MainWindow.SetLeft(0);

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            try
            {
                string? message = error.ExceptionObject.ToString();

                System.Diagnostics.Debug.WriteLine(message);

                string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenHabitTracker", "Error.log");
                System.IO.File.WriteAllText(path, message);

                app.MainWindow.ShowMessage("Error", message);
            }
            catch
            {
            }
        };

        app.Run();
    }

    [JSInvokable]
    public static void OpenLink(string url)
    {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
}
