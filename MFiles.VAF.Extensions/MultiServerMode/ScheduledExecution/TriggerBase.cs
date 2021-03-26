﻿using MFiles.VAF.Configuration;
using System;
using System.Runtime.Serialization;

namespace MFiles.VAF.Extensions.MultiServerMode.ScheduledExecution

{
	/// <summary>
	/// Class used for configuration purposes.
	/// </summary>
	[DataContract]
	public class Trigger
		: TriggerBase
	{
		/// <summary>
		/// The type of trigger this is (e.g. Daily, Weekly).
		/// </summary>
		[DataMember]
		public new TriggerType Type
		{
			get => base.Type;
			set => base.Type = value;
		}

		[DataMember]
		[JsonConfEditor
		(
			Label = "Configuration",
			Hidden = true,
			ShowWhen = ".parent._children{.key == 'Type' && .value == 'Daily' }"
		)]
		public DailyTrigger DailyTriggerConfiguration { get; set; }
			= new DailyTrigger();

		[DataMember]
		[JsonConfEditor
		(
			Label = "Configuration",
			Hidden = true,
			ShowWhen = ".parent._children{.key == 'Type' && .value == 'Weekly' }"
		)]
		public WeeklyTrigger WeeklyTriggerConfiguration { get; set; }
			= new WeeklyTrigger();

		[DataMember]
		[JsonConfEditor
		(
			Label = "Configuration",
			Hidden = true,
			ShowWhen = ".parent._children{.key == 'Type' && .value == 'Monthly' }"
		)]
		public DayOfMonthTrigger DayOfMonthTriggerConfiguration { get; set; }
			= new DayOfMonthTrigger();

		/// <inheritdoc />
		public override DateTime? GetNextExecution(DateTime? after = null)
		{
			switch (this.Type)
			{
				case TriggerType.Daily:
					return this.DailyTriggerConfiguration?.GetNextExecution(after);
				case TriggerType.Weekly:
					return this.WeeklyTriggerConfiguration?.GetNextExecution(after);
				case TriggerType.Monthly:
					return this.DayOfMonthTriggerConfiguration?.GetNextExecution(after);
				default:
					return null;
			}
		}

		/// <inheritdoc />
		public override string ToString()
		{
			switch (this.Type)
			{
				case TriggerType.Daily:
					return this.DailyTriggerConfiguration?.ToString();
				case TriggerType.Weekly:
					return this.WeeklyTriggerConfiguration?.ToString();
				case TriggerType.Monthly:
					return this.DayOfMonthTriggerConfiguration?.ToString();
				default:
					return null;
			}
		}
	}
	public abstract class TriggerBase
	{
		/// <summary>
		/// The type of trigger this is (e.g. Daily, Weekly).
		/// </summary>
		public TriggerType Type { get; set; } = TriggerType.Unknown;

		/// <summary>
		/// Gets the next execution datetime for this trigger.
		/// </summary>
		/// <param name="after">The time after which the schedule should run.  Defaults to now (i.e. next-run time) if not provided.</param>
		/// <returns>The next execution time.</returns>
		public abstract DateTime? GetNextExecution(DateTime? after = null);
	}
}