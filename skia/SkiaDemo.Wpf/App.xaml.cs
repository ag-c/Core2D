﻿using System;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Core2D.Data.Database;
using Core2D.Editor;
using Core2D.Editor.Factories;
using Core2D.Interfaces;
using Core2D.Renderer;
using FileSystem.DotNetFx;
using FileWriter.PdfSkiaSharp;
using FileWriter.SvgSkiaSharp;
using Microsoft.Win32;
using Renderer.SkiaSharp;
using Serializer.Newtonsoft;
using Serializer.Xaml;
using TextFieldReader.CsvHelper;
using TextFieldWriter.CsvHelper;
using Utilities.Wpf;

namespace SkiaDemo.Wpf
{
    class ServiceProvider : IServiceProvider
    {
        private readonly ILifetimeScope _scope;

        public ServiceProvider(ILifetimeScope scope)
        {
            _scope = scope;
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return _scope.Resolve(serviceType);
        }
    }

    class LocatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ServiceProvider>().As<IServiceProvider>().InstancePerLifetimeScope();
        }
    }

    class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProjectEditor>().As<ProjectEditor>().InstancePerLifetimeScope();
            builder.RegisterType<ProjectFactory>().As<IProjectFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ShapeFactory>().As<IShapeFactory>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(ToolBase).Assembly).As<ToolBase>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(PathToolBase).Assembly).As<PathToolBase>().AsSelf().InstancePerLifetimeScope();
        }
    }

    class DependenciesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DotNetFxFileSystem>().As<IFileSystem>().InstancePerLifetimeScope();
            builder.RegisterType<NewtonsoftJsonSerializer>().As<IJsonSerializer>().InstancePerLifetimeScope();
            builder.RegisterType<PortableXamlSerializer>().As<IXamlSerializer>().InstancePerLifetimeScope();
            builder.RegisterType<PdfWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<SvgWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<CsvHelperReader>().As<ITextFieldReader<XDatabase>>().InstancePerLifetimeScope();
            builder.RegisterType<CsvHelperWriter>().As<ITextFieldWriter<XDatabase>>().InstancePerLifetimeScope();
            builder.Register<ShapeRenderer>((c) => new SkiaRenderer(true, 96.0)).InstancePerDependency();
            builder.RegisterType<WpfTextClipboard>().As<ITextClipboard>().InstancePerLifetimeScope();
        }
    }

    class Win32ImageImporter : IImageImporter
    {
        private readonly IServiceProvider _serviceProvider;

        public Win32ImageImporter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<string> GetImageKeyAsync()
        {
            var dlg = new OpenFileDialog() { Filter = "All (*.*)|*.*" };
            if (dlg.ShowDialog(_serviceProvider.GetService<MainWindow>()) == true)
            {
                var path = dlg.FileName;
                var bytes = System.IO.File.ReadAllBytes(path);
                var key = _serviceProvider.GetService<ProjectEditor>().Project.AddImageFromFile(path, bytes);
                return await Task.Run(() => key);
            }
            return null;
        }
    }

    class AppModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Win32ImageImporter>().As<IImageImporter>().InstancePerLifetimeScope();
        }
    }

    class ViewModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MainWindow>().As<MainWindow>().InstancePerLifetimeScope();
        }
    }

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = new ContainerBuilder();

            builder.RegisterAssemblyModules(typeof(MainWindow).Assembly);

            using (IContainer container = builder.Build())
            {
                container.Resolve<MainWindow>().ShowDialog();
            }
        }
    }
}
