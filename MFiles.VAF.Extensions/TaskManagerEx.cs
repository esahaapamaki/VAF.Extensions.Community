﻿using MFiles.VAF.AppTasks;
using MFiles.VAF.Configuration.AdminConfigurations;
using MFiles.VAF.Configuration.Domain.Dashboards;
using MFiles.VAF.Core;
using MFiles.VAF.Extensions.Dashboards;
using MFilesAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFiles.VAF.Extensions
{
	public partial class TaskManagerEx<TConfiguration>
		: TaskManager
		where TConfiguration : class, new()
	{
		protected ConfigurableVaultApplicationBase<TConfiguration> VaultApplication { get; private set; }

		public TaskManagerEx
		(
			ConfigurableVaultApplicationBase<TConfiguration> vaultApplication,
			string id, 
			Vault permanentVault, 
			IVaultTransactionRunner transactionRunner,
			TimeSpan? processingInterval = null,
			uint maxConcurrency = 16,
			TimeSpan? maxLockWaitTime = null,
			TaskExceptionSettings exceptionSettings = null
		)
			: base(id, permanentVault, transactionRunner, processingInterval, maxConcurrency, maxLockWaitTime, exceptionSettings)
		{
			this.VaultApplication = vaultApplication
				?? throw new ArgumentNullException(nameof(vaultApplication));
			this.TaskEvent += TaskManagerEx_TaskEvent;
		}

		private void TaskManagerEx_TaskEvent(object sender, TaskManagerEventArgs e)
		{
			if (null == e.Queues || e.Queues.Count == 0)
				return;

			switch (e.EventType)
			{
				case TaskManagerEventType.TaskJobFinished:
					switch (e.JobResult)
					{
						case TaskProcessingJobResult.Complete:
						case TaskProcessingJobResult.Fatal:
							// Re-schedule.
							foreach (var t in e.Tasks)
							{
								// Can we get a next execution date?
								var nextExecutionDate = this.VaultApplication.GetNextTaskProcessorExecution(t.QueueID, t.TaskType);
								if (false == nextExecutionDate.HasValue)
									continue;
								this.AddTask(this.Vault, t.QueueID, t.TaskType, activationTime: nextExecutionDate);
							}
							break;
					}
					break;
			}
		}
	}
}
