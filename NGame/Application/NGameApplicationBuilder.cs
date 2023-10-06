﻿using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NGame.Setup;

namespace NGame.Application;



public class NGameApplicationBuilder : INGameApplicationBuilder
{
	private readonly HostApplicationBuilder _builder =
		new(
			new HostApplicationBuilderSettings
			{
				ApplicationName =
					Assembly
						.GetEntryAssembly()?
						.GetCustomAttribute<AssemblyTitleAttribute>()?
						.Title
			}
		);


	public NGameApplicationBuilder()
	{
		Environment = new NGameEnvironment(_builder.Environment);

		_builder.Services.AddSingleton(Environment);
		_builder.Services.AddSingleton<IApplicationEvents, ApplicationEvents>();
		_builder.Services.AddSingleton<IGameRunner, GameRunner>();
	}


	public INGameEnvironment Environment { get; }
	public IConfigurationRoot Configuration => _builder.Configuration;
	public IServiceCollection Services => _builder.Services;
	public ILoggingBuilder Logging => _builder.Logging;


	public NGameApplication Build()
	{
		var host = _builder.Build();
		return new NGameApplication(Environment, host);
	}
}
